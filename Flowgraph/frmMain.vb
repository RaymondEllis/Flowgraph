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
'   Email: RaymondEllis*live.com
#End Region

Public Class frmMain
   
    Public SaveOnExit As Boolean = True
    Private ToolTipText As String = ""

#Region "Load & Close"


    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing


        'RemoveFromFGS
        If SaveOnExit Then
            'Right now there is no way of knowing if anything has changed. So we will always ask to save any changes.
            Select Case MsgBox("Do you want to save any changes you may have made?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question, "Save") 'ToDo: Should be Cancel by default.
                Case MsgBoxResult.Yes
                    menuSave_Click(sender, e)

                Case MsgBoxResult.Cancel
                    e.Cancel = True
                    Return
            End Select
        End If
        'EndRemoveFromFGS

        'Dispose all objects.
        For Each obj As Object In Objects
            obj.Dispose()
        Next
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-US")
        Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
        Me.Text = "Flowgraph v" & Application.ProductVersion.ToString

        'Set the current directory to the exe path.
        Environment.CurrentDirectory = IO.Path.GetDirectoryName(Environment.GetCommandLineArgs(0))

        'Do command line stuff.
        Dim args As String() = Environment.GetCommandLineArgs
        Dim a As Integer = 1
        Do While a < args.Length
            Select Case LCase(args(a))
                Case "startatcursor"
                    'This block of code will move the window to the mouse position.
                    Dim x As Integer = MousePosition.X - (Me.Width * 0.5) 'Set the window x center to the mouse position x
                    Dim y As Integer = MousePosition.Y - (Me.Height * 0.5) 'Set the window y center to the mouse position y
                    Dim scr As Rectangle = Screen.GetWorkingArea(MousePosition) 'Get the current screen that the mouse is on.
                    If x < scr.Left Then x = scr.Left 'If the window is too far left. Then set it to the left of the screen.
                    If y < scr.Top Then y = scr.Top 'If the window is too far up. Then set it to the top of the screen.
                    If x + Me.Width > scr.Right Then x = scr.Right - Me.Width 'If the window is too far right. Then set it to the right of the screen.
                    If y + Me.Height > scr.Bottom Then y = scr.Bottom - Me.Height 'If the window is too far down. Then set it to the bottom of the screen.
                    Me.Location = New Point(x, y) 'Set the window location.

                    'RemoveFromFGS
                Case "nosaveonexit"
                    SaveOnExit = False

                Case "logsave"
                    a += 1
                    If a >= args.Length - 1 Then Exit Do
                    Plugins.LogSavePriority = Integer.Parse(args(a))
                Case "logshow"
                    a += 1
                    If a >= args.Length - 1 Then Exit Do
                    Plugins.LogShowPriority = Integer.Parse(args(a))

                Case Else
                    If IO.File.Exists(args(a)) Then
                        FileToOpen = args(a)
                    ElseIf IO.File.Exists(args(a) & ".fgs") Then
                        FileToOpen = args(a) & ".fgs"
                    End If
                    'EndRemoveFromFGS
            End Select
            a += 1
        Loop

        'AddToFGSmenuNew.Dispose()
        'AddToFGSmenuOpen.Dispose()
        'AddToFGSmenuSave.Dispose()
        'AddToFGSmenuSaveAs.Dispose()
    End Sub

    Private FileToOpen As String = ""
    Private Sub frmMain_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'Load the plugin stuff.
        Load_PluginSystem()

        'If there is a file to open then open it.
        If FileToOpen <> "" Then Open(FileToOpen)
    End Sub

#End Region

#Region "Mouse"

    Private Sub frmMain_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDoubleClick
        Mouse.Location = e.Location

        If Plugins.UpdateUI = False Then Return

        If e.Button = Windows.Forms.MouseButtons.Left Then
            For i As Integer = Objects.Count - 1 To 0 Step -1
                'RemoveFromFGS
                If Objects(i).IntersectsWithOutput(Mouse) Then
                    Objects(i).Output(Objects(i).Intersection).Disconnect()
                    DoDraw(True)

                    Return
                ElseIf Objects(i).IntersectsWithInput(Mouse) Then
                    Objects(i).Input(Objects(i).Intersection).Disconnect()
                    DoDraw(True)

                    Return
                End If
                'EndRemoveFromFGS
                'If the mouse intersects with a object then send the duble click event to the object.
                If Mouse.IntersectsWith(Objects(i).Rect) Then
                    Objects(i).MouseDoubleClick(e)

                    Return
                End If



            Next

        End If
    End Sub

    Private Sub frmMain_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        Mouse.Location = e.Location

        If Plugins.UpdateUI = False Then Return

        Select Case Tool
            Case ToolType.None
                If e.Button = Windows.Forms.MouseButtons.Left Then

                    For i As Integer = Objects.Count - 1 To 0 Step -1
                        'If the mouse intersects with the title bar then move the object.
                        If Mouse.IntersectsWith(Objects(i).TitleBar) Then
                            Tool = ToolType.Move
                            ToolObject = Objects(i).Index
                            ToolOffset = Mouse.Location - DirectCast(Objects(i).Rect, Rectangle).Location

                            Return
                        End If

                        'RemoveFromFGS
                        If Objects(i).IntersectsWithOutput(Mouse) Then
                            Tool = ToolType.Connect
                            ToolObject = Objects(i).Index
                            ToolInt = Objects(i).Intersection
                            ToolOffset = e.Location

                            Return
                        End If
                        'EndRemoveFromFGS
                    Next
                End If

                For i As Integer = Objects.Count - 1 To 0 Step -1
                    If Mouse.IntersectsWith(Objects(i).Rect) Then
                        Objects(i).MouseDown(e)
                        Return
                    End If
                Next



        End Select


    End Sub

    Private Sub frmMain_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        Mouse.Location = e.Location

        If Plugins.UpdateUI = False Then Return

        Select Case Tool
            Case ToolType.None
                'Check to see if the mouse is in a object.
                For i As Integer = Objects.Count - 1 To 0 Step -1
                    If Mouse.IntersectsWith(Objects(i).Rect) Then
                        Objects(i).MouseUp(e)
                        Return
                    End If
                Next
                'RemoveFromFGS
                If e.Button = Windows.Forms.MouseButtons.Right Then
                    Plugins.Menu.Open(-1, AddItem)
                End If
                'EndRemoveFromFGS

            Case ToolType.Menu
                If e.Button = Windows.Forms.MouseButtons.Left Then
                    Plugins.Menu.MouseUp()
                    DoDraw(True)
                ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
                    For i As Integer = Objects.Count - 1 To 0 Step -1
                        If Mouse.IntersectsWith(Objects(i).Rect) Then
                            Tool = ToolType.None
                            Objects(i).MouseUp(e)
                            DoDraw(True)
                            Return
                        End If
                    Next
                    Plugins.Menu.Open(-1, AddItem)
                End If

            Case ToolType.Move
                Tool = ToolType.None


                'RemoveFromFGS
            Case ToolType.Connect
                Tool = ToolType.None

                For i As Integer = Objects.Count - 1 To 0 Step -1
                    If Objects(i).Index <> ToolObject Then
                        If Objects(i).IntersectsWithInput(Mouse) Then

                            'Try and connect.
                            If Objects(ToolObject).Output(ToolInt).TryConnect(Objects(i).Index, Objects(i).Intersection) Then
                                'Add one to connected if it successfully connected.
                                Objects(i).Input(Objects(i).Intersection).Connected += 1
                            End If


                            Exit For
                        End If
                    End If
                Next


                DoDraw(True)
                'EndRemoveFromFGS
        End Select

    End Sub


    Private Sub frmMain_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        Mouse.Location = e.Location

        If Plugins.UpdateUI = False Then Return

        '###Tooltip
        'Reset tooltip text to noting.
        ToolTipText = ""

        'Check it see if the mouse is hovering over a input or a output.
        For i As Integer = Objects.Count - 1 To 0 Step -1 'Loop through each object until we found a input/output or we made it through them all.
            If Objects(i).IntersectsWithInput(Mouse) Then 'Check input.
                ToolTipText = Objects(i).Input(Objects(i).Intersection).ToString
                Exit For
            ElseIf Objects(i).IntersectsWithOutput(Mouse) Then 'Check output.
                ToolTipText = Objects(i).Output(Objects(i).Intersection).ToString
                Exit For
            End If
        Next
        'If there is tooltip text to display then display it.
        If Not ToolTipText = "" Then
            If lblToolTip.Text <> ToolTipText Or lblToolTip.Visible = False Then
                lblToolTip.Text = ToolTipText
                lblToolTip.Location = Mouse.Location + New Point(10, 17)
                lblToolTip.Visible = True
            End If
        Else 'Other wise we disable the tooltib label.
            lblToolTip.Visible = False
        End If

        '###End Tooltip

        Select Case Tool
            Case ToolType.None
                'Check to see if the mouse is in a object.
                For i As Integer = Objects.Count - 1 To 0 Step -1
                    If Mouse.IntersectsWith(Objects(i).Rect) Then
                        Objects(i).MouseMove(e)
                        Return
                    End If
                Next

            Case ToolType.Menu
                If Plugins.Menu.MouseMove() Then DoDraw(Plugins.Menu.Rect)

            Case ToolType.Move
                Objects(ToolObject).SetPosition(e.X - ToolOffset.X, e.Y - ToolOffset.Y)

                'RemoveFromFGS
            Case ToolType.Connect
                DoDraw(True)
                'EndRemoveFromFGS

        End Select

    End Sub
#End Region

    'Draw everything.
    Private Sub frmMain_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality

        If Plugins.UpdateUI = False Then Return
        'Draw the objects.
        For i As Integer = 0 To Objects.Count - 1
            If e.ClipRectangle.IntersectsWith(Objects(i).rect) Then
                Objects(i).Draw(e.Graphics)
            End If
        Next

        'Draw the connectors.
        For Each obj As Object In Objects
            obj.DrawConnectors(e.Graphics)
        Next


        Select Case Tool 'RemoveFromFGS
            Case ToolType.Connect 'If we are using teh connect tool then draw the line.
                e.Graphics.DrawLine(ConnectorPen, ToolOffset, Mouse.Location)
                'EndRemoveFromFGS
            Case ToolType.Menu
                If e.ClipRectangle.IntersectsWith(Plugins.Menu.Rect) Then Plugins.Menu.Draw(e.Graphics)

        End Select

    End Sub

#Region "Controls"

    'RemoveFromFGS
    Private Sub frmMain_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize

        If (Me.Size.Width > Me.MinimumSize.Width And Me.Size.Height > Me.MinimumSize.Height) And Me.WindowState = FormWindowState.Normal AndAlso _
            Plugins.WindowSize <> Me.ClientSize Then
            'Snap to the grid size.
            Me.ClientSize = SnapToGrid(Me.ClientSize)
            Plugins.WindowSize = Me.ClientSize
        End If

        Plugins.WindowState = Me.WindowState
    End Sub



    Private Sub frmMain_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(Windows.Forms.DataFormats.FileDrop) Or
            e.Data.GetDataPresent(Windows.Forms.DataFormats.UnicodeText) Then
            e.Effect = DragDropEffects.All
        End If
        'Dim tmp As String = e.Data.GetFormats()(0).ToString
        'For n As Integer = 1 To e.Data.GetFormats.Length - 1
        '    tmp &= ", " & e.Data.GetFormats()(n)
        'Next
        'Me.Text = tmp
    End Sub

    Private Sub frmMain_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop
        If e.Data.GetDataPresent(Windows.Forms.DataFormats.FileDrop) Then
            Dim file As String() = e.Data.GetData(Windows.Forms.DataFormats.FileDrop)
            If file.Length > 0 AndAlso file(0) <> "" Then Open(file(0))

        ElseIf e.Data.GetDataPresent(Windows.Forms.DataFormats.UnicodeText) Then
            Dim data As String = e.Data.GetData(Windows.Forms.DataFormats.UnicodeText)
            If data <> "" Then Open(data, False)
        End If
    End Sub
    'EndRemoveFromFGS


#End Region

#Region "Plugin stuff"
    Public Sub Load_PluginSystem()
        AddHandler AddControlEvent, AddressOf AddControl
        AddHandler RemoveControlEvent, AddressOf RemoveControl

        AddHandler DrawEvent, AddressOf Draw

        'RemoveFromFGS
        AddHandler OpenedEvent, AddressOf Opened
        'EndRemoveFromFGS

        Load_Plugin(Me) 'Load the plugin stuff. (auto draw, connector pen, etc..)
    End Sub

    Private Sub Draw(ByVal region As Rectangle)
        If region.IsEmpty Then
            Me.Invalidate()
        Else
            Me.Invalidate(region)
        End If
    End Sub

    'RemoveFromFGS
    Private Sub Opened()
        'menuDisableUI.Checked = Not Plugins.UpdateUI
        If menuDisableUI.Checked = Plugins.UpdateUI Then menuDisableUI_Click(Nothing, Nothing)

        If LoadedFile = "" Then
            Me.Text = "Unsaved - Flowgraph v" & Application.ProductVersion.ToString
        Else
            Me.Text = IO.Path.GetFileName(LoadedFile) & " - Flowgraph v" & Application.ProductVersion.ToString
        End If
    End Sub
    'EndRemoveFromFGS

    Public Sub AddControl(ByVal Control As Control)
        Me.Controls.Add(Control)
        lblToolTip.BringToFront()
    End Sub
    Public Sub RemoveControl(ByVal Control As Control)
        Me.Controls.Remove(Control)
    End Sub
#End Region


#Region "Menu"

#Region "File"
    'RemoveFromFGS
    Private Sub menuNew_Click(sender As System.Object, e As System.EventArgs) Handles menuNew.Click
        If MsgBox("Are you sure you want to erase everything?", MsgBoxStyle.YesNo, "New") = MsgBoxResult.Yes Then
            ClearObjects()
            LoadedFile = ""
            DoDraw(True)
            Me.Text = "Unsaved - Flowgraph v" & Application.ProductVersion.ToString
        End If
    End Sub
    Private Sub menuOpen_Click(sender As System.Object, e As System.EventArgs) Handles menuOpen.Click
        Dim ofd As New OpenFileDialog
        ofd.Filter = "FlowGraphSetting files (*.fgs)|*.fgs|All files (*.*)|*.*"
        If ofd.ShowDialog = Windows.Forms.DialogResult.OK Then
            Open(ofd.FileName)
        End If
    End Sub
    Private Sub menuSave_Click(sender As System.Object, e As System.EventArgs) Handles menuSave.Click
        If LoadedFile.Trim = "" Then
            menuSaveAs_Click(sender, e)
        Else
            Save(LoadedFile)
        End If
    End Sub
    Private Sub menuSaveAs_Click(sender As System.Object, e As System.EventArgs) Handles menuSaveAs.Click
        Dim sfd As New SaveFileDialog
        sfd.Filter = "FlowGraphSetting files (*.fgs)|*.fgs|All files (*.*)|*.*"
        If sfd.ShowDialog = Windows.Forms.DialogResult.OK Then
            Save(sfd.FileName)
            Me.Text = IO.Path.GetFileName(LoadedFile) & " - Flowgraph v" & Application.ProductVersion.ToString
        End If
    End Sub
    'EndRemoveFromFGS
    Private Sub menuExit_Click(sender As System.Object, e As System.EventArgs) Handles menuExit.Click
        Me.Close()
    End Sub
#End Region

#Region "View"
    Private Sub menuDisableUI_Click(sender As System.Object, e As System.EventArgs) Handles menuDisableUI.Click
        Plugins.UpdateUI = menuDisableUI.Checked

        'Hide/Show all the controls.
        For i As Integer = 2 To Controls.Count - 1
            Controls(i).Visible = menuDisableUI.Checked
        Next

        'Resize the form.
        If Not menuDisableUI.Checked Then
            Me.Size = Me.MinimumSize
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Else
            Me.ClientSize = Plugins.WindowSize
            Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        End If

        menuDisableUI.Checked = Not menuDisableUI.Checked
    End Sub

    Private Sub menuSimpleLines_Click(sender As System.Object, e As System.EventArgs) Handles menuSimpleLines.Click
        menuSimpleLines.Checked = Not menuSimpleLines.Checked
        Plugins.ComplexLines = Not menuSimpleLines.Checked
        Me.Invalidate()
    End Sub

    Private Sub mnuSource_Click(sender As System.Object, e As System.EventArgs) Handles mnuSource.Click
        MsgBox("Under construction!", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Source - Flowgraph")
    End Sub
#End Region

#Region "Help"
    Dim About As New frmAbout
    Private Sub menuAbout_Click(sender As System.Object, e As System.EventArgs) Handles menuAbout.Click
        About.ShowDialog()
    End Sub
    Private Sub menuBasicUse_Click(sender As System.Object, e As System.EventArgs) Handles menuBasicUse.Click
        System.Diagnostics.Process.Start("https://code.google.com/p/flowgraph/wiki/Flowgraph")
    End Sub
#End Region

#End Region




End Class
