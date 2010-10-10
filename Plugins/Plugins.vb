﻿#Region "License & Contact"
'License:
'   Copyright (c) 2010 Raymond Ellis
'   
'   This software is provided 'as-is', without any express or implied
'   warranty. In no event will the authors be held liable for any damages
'   arising from the use of this software.
'
'   Permission is granted to anyone to use this software for any purpose,
'   including commercial applications, and to alter it and redistribute it
'   freely, subject to the following restrictions:
'
'       1. The origin of this software must not be misrepresented; you must not
'           claim that you wrote the original software. If you use this software
'           in a product, an acknowledgment in the product documentation would be
'           appreciated but is not required.
'
'       2. Altered source versions must be plainly marked as such, and must not be
'           misrepresented as being the original software.
'
'       3. This notice may not be removed or altered from any source
'           distribution.
'
'
'Contact:
'   Raymond Ellis
'   Email: RaymondEllis@live.com
#End Region

Public Module Plugins
    Event DrawEvent()
    Event AddControlEvent(ByVal Control As Control)
    Event RemoveControlEvent(ByVal Control As Control)

    'The snap grid size.
    Public GridSize As Integer = 5

    'The default font to use.
    Public DefaultFont As Font = SystemFonts.DefaultFont
    Public DefaultFontBrush As Brush = SystemBrushes.ControlText

    'The pen used to connect objects.
    Public ConnectorPen As New Pen(Color.FromArgb(80, 80, 80), 3)

    'Used to check if the mouse is inside a rectangle.
    Public Mouse As Rectangle

    'The list of objects.
    Public Objects As New List(Of Object)

    Public Enum ToolType
        None
        Move
        Connect
        Menu
    End Enum
    Public Tool As ToolType
    Public ToolOffset As Point
    Public ToolObject As Integer
    Public ToolInt As Integer

    Public Sub ResetObjectIndexs(ByVal RemovedIndex As Integer)
        For n As Integer = 0 To Objects.Count - 1
            Objects(n).Index = n


            If Objects(n).Output IsNot Nothing Then
                For o As Integer = 0 To Objects(n).Output.Length - 1
                    Dim i As Integer = 0
                    Do
                        If Objects(n).Output(o).Flow(i).obj = RemovedIndex Then
                            Objects(n).Output(o).Flow(i) = Nothing
                            Objects(n).Output(o).Flow.RemoveAt(i)
                        ElseIf Objects(n).Output(o).Flow(i).obj > RemovedIndex Then
                            Objects(n).Output(o).Flow(i).obj -= 1
                            i += 1
                        Else
                            i += 1
                        End If
                    Loop Until i = Objects(n).Output(o).Flow.Count
                Next
            End If
        Next
    End Sub

    Public Sub RemoveAt(ByVal Index As Integer)
        Objects(Index).Distroy()
        Objects(Index) = Nothing
        Objects.RemoveAt(Index)

        ResetObjectIndexs(Index)
    End Sub


    ''' <summary>
    ''' Loads the stuff in modMain.
    ''' </summary>
    Public Sub Load_Main()
        'Setup the auto draw timmer.
        tmrDraw.Interval = 200
        tmrDraw.Enabled = True


        AddObject_Setup()


    End Sub

#Region "Open & Save"
    Public LoadedFile As String = ""

    Public Sub Open(ByVal File As String)
        If Not IO.File.Exists(File) Then
            MsgBox("Could not find file:" & vbNewLine & File, , "Error loading")
            Return
        End If

        ClearObjects()

        Dim sd As New SimpleD.SimpleD
        sd.FromFile(File)

        Dim g As SimpleD.Group = sd.Get_Group("Main")
        Dim numObj As Integer = g.Get_Value("Objects")
        For n As Integer = 0 To numObj
            g = sd.Get_Group("Object" & n)
            Dim pos As String() = Split(g.Get_Value("position"), ",")
            Dim obj As Integer = AddObject(g.Get_Value("name"), New Point(pos(0), pos(1)))
            'Objects(obj).Load(g)

        Next

        For n As Integer = 0 To numObj
            g = sd.Get_Group("Object" & n)
            Objects(n).Load(g)
        Next

        LoadedFile = File
    End Sub

    Public Sub Save(ByVal File As String)

        Dim sd As New SimpleD.SimpleD
        Dim g As SimpleD.Group = sd.Create_Group("Main")
        g.Add("Objects", Objects.Count - 1)

        For Each obj As Object In Objects
            sd.Add_Group(obj.Save)
        Next

        sd.ToFile(File)

        LoadedFile = File
    End Sub

    Public Sub ClearObjects()
        For Each obj As Object In Objects
            obj.Distroy()
        Next
        Objects.Clear()
    End Sub
#End Region

#Region "Auto draw"
    Private DoNotDraw As Boolean = True

    ''' <summary>
    ''' Tells auto draw to draw when the time comes.
    ''' </summary>
    ''' <param name="HeighPriority">If it's a heigh priority, then it will draw as soon as possible.</param>
    Public Sub DoDraw(Optional ByVal HeighPriority As Boolean = False)

        'If it is a heigh priority. then we will not wait for the next timmer tick and just draw.
        If HeighPriority Then
            RaiseEvent DrawEvent()

        Else 'Other wise we wait for the timer.
            DoNotDraw = False 'Tell the timer it can draw.
        End If

    End Sub

    Private WithEvents tmrDraw As New Timer
    Private Sub tmrDraw_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrDraw.Tick
        If DoNotDraw Then Return

        RaiseEvent DrawEvent()

        DoNotDraw = True
    End Sub
#End Region

    Public Sub AddControl(ByVal Control As Control)
        RaiseEvent AddControlEvent(Control)
    End Sub
    Public Sub RemoveControl(ByVal Control As Control)
        RaiseEvent RemoveControlEvent(Control)
    End Sub


    Public AddItems As New List(Of MenuNode)

    ''' <summary>
    ''' Add a new object from the class name.
    ''' </summary>
    ''' <param name="Name">ex: 'Plugins.fgAdd'</param>
    ''' <param name="Position">You shouldn't need help here.</param>
    ''' <returns>-1 if not found. other wise returns object index.</returns>
    ''' <remarks></remarks>
    Public Function AddObject(ByVal Name As String, ByVal Position As Point) As Integer
        'NOTE: I am pretty sure there is a faster way to do this.
        'But I got this working first, so until it is a problem it will stay like this.
        Try
            Objects.Add(Activator.CreateInstance(Type.[GetType](Name), New Object() {Position}))
            Return Objects.Count - 1
        Catch ex As Exception
            MsgBox("Could not create object: " & Name)
            Return -1
        End Try
    End Function

    Public Sub AddObject_Setup()
        'Is the plugins library newer then the objects file?
        If IO.File.GetLastWriteTime("Plugins.dll") > IO.File.GetLastWriteTime("Plugins\Objects.list") Then
            
            'The plugins have changed. So lets find all of the objects.

            Dim Scripts As String() = IO.Directory.GetFiles("Plugins\", "*.??", IO.SearchOption.AllDirectories)
            Dim ObjectList As String = ""
            For Each File As String In Scripts
                SearchForItems(File, ObjectList)
            Next

            'Write all of the objects found to the file.
            Dim sw As New IO.StreamWriter("Plugins\Objects.list", False)
            sw.Write(ObjectList)
            sw.Close()

        Else
            'Objects.list is newer, so lets get the items from it.
            SearchForItems("Plugins\Objects.list")
        End If
    End Sub

    ''' <summary>
    ''' Search thru a file and fill the add object menu. With found objects.
    ''' </summary>
    ''' <param name="File">The file to search thru.</param>
    ''' <param name="ObjectList">Will fill string with each line that has "AddMenuObject".
    ''' Unlis set to "DoNotFill"</param>
    ''' <remarks></remarks>
    Private Sub SearchForItems(ByVal File As String, Optional ByRef ObjectList As String = "DoNotFill", Optional ByVal SearchWholeFile As Boolean = False)
        Dim sr As New IO.StreamReader(File)
        Dim StartIndex As Integer
        Do
            Dim line As String = sr.ReadLine 'Get the next line out of the file.
            StartIndex = line.IndexOf("AddMenuObject", StringComparison.OrdinalIgnoreCase) 'Get the index of "AddMenuObject".

            'If we found "AddMenuObject" then.
            If StartIndex > -1 Then
                'Should split like:
                'AddMenuObject|Name,ClassName,Optional Width|Group1,Group2,Group3,etc..
                'Groups are optional.
                Dim SplitLine As String() = Split(line, "|")
                Select Case SplitLine.Length
                    Case 2 'No groups. 
                        AddNode(AddItems, Split(SplitLine(1), ","), New String() {})

                    Case 3 'Has Group(s) 
                        AddNode(AddItems, Split(SplitLine(1), ","), Split(SplitLine(2), ","))

                End Select

                'Fill object list(if not "DoNotFill").
                If ObjectList = "DoNotFill" Then
                ElseIf ObjectList = "" Then
                    ObjectList = line
                Else
                    ObjectList &= vbNewLine & line
                End If
            End If

        Loop Until sr.EndOfStream Or (StartIndex = -1 And Not SearchWholeFile)
        sr.Close()
    End Sub

End Module