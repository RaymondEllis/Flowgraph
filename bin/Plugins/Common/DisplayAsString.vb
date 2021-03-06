﻿'AddMenuObject|Display as string,Plugins.Common.DisplayAsString,100|Common
Namespace Common
    Public Class DisplayAsString
        Inherits BaseObject

        Public Sub New(ByVal StartPosition As Point, ByVal UserData As String)
            Setup(UserData, StartPosition, 105) 'Setup the base rectangles.


            'Create one input.
            Inputs(New String() {"Value to display."})
            'Input(0).MaxConnected = 1 'Only allow one connection.

            'Set the title.
            Title = "Display as string"
            File = "Common\DisplayAsString.vb"

            MenuItems.Add(New Menu.Node("Set String", , 70))
        End Sub

        Public Overrides Function Save() As SimpleD.Group
            Dim g As SimpleD.Group = MyBase.Save()

            g.SetValue("Data", Data)

            Return g
        End Function
        Public Overrides Sub Load(ByVal g As SimpleD.Group)
            g.GetValue("Data", Data, True)

            MyBase.Load(g)
        End Sub

        Public Overrides Sub MenuSelected(ByVal Result As Menu.Node)
            MyBase.MenuSelected(Result)

            If Result.Result = Global.Plugins.Menu.Result.SelectedItem Then
                If Result.Name = "Set String" Then
                    Me.Data = InputBox("Set string", "THIS IS THE TITLE")
                End If
            End If
        End Sub

        Private Data As String
        Private DataSize, OldSize As SizeF
        Public Overrides Sub Receive(ByVal Data As Object, ByVal sender As DataFlow)
            Me.Data = Data.ToString() 'Set the data.
            DataSize = Nothing 'Set the data size to nothing so we will check the size later.

            'Tell auto draw we want to draw.
            DoDraw(Rect)
        End Sub

        Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
            'Lets measure the size if data size is nothing.
            If DataSize = Nothing Then
                DataSize = g.MeasureString("String= " & Data, DefaultFont) 'Measure the string.
                If DataSize.Width < 75 Then DataSize.Width = 75 'Set the min width.

                'Did the size change?
                If DataSize <> OldSize Then
                    'If so then we set the size of the base object
                    MyBase.SetSize(DataSize.Width, DataSize.Height, True)
                    OldSize = DataSize 'Then set the old size.

                    DoDraw(True)
                End If

            End If

            'Draw the base stuff like the title outputs etc..
            MyBase.Draw(g)


            'Draw the value.
            g.DrawString("String= " & Data, DefaultFont, DefaultFontBrush, Position)
        End Sub

    End Class
End Namespace