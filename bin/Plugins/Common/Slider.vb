﻿'AddMenuObject|Slider,Plugins.Common.Slider|Common
Namespace Common
    Public Class Slider
        Inherits BaseObject

        Private Enabled As Boolean = True

        Private Value As Decimal


        Public Sub New(ByVal StartPosition As Point, ByVal UserData As String)
            Setup(UserData, StartPosition, 100) 'ToDo: Slider   make the with resizable.


            Outputs(New String() {"Value,0-1Normalized"})
            Inputs(New String() {"Enable,Boolean", "Value,0-1Normalized,Boolean"})

            'Set the title.
            Title = "Slider"
            File = "Common\Slider.vb"

            Value = 0.5
        End Sub

        Public Overrides Sub Receive(ByVal Data As Object, ByVal sender As DataFlow)
            Select Case sender.Index
                Case 0 'Enable
                    Enabled = Data

                Case 1 'Set value
                    If Not Enabled Then Return
                    If Data.GetType Is GetType(Boolean) Then
                        If Data = True Then
                            Value = 1
                        Else
                            Value = 0
                        End If
                    Else

                        Value = Data
                        If Value > 1 Then Value = 1
                        If Value < 0 Then Value = 0
                    End If

                    Send(Value)

            End Select

            DoDraw(Rect)
        End Sub

        Public Overrides Sub MouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.MouseDown(e)
            If Not Enabled Then Return
            If e.Button = MouseButtons.Left Then
                Dim x As Integer = e.X - Position.X
                If x >= Size.Width Then
                    Value = 1
                ElseIf x <= 0 Then
                    Value = 0
                Else
                    Value = x / Size.Width
                End If
                Send(Value)
                DoDraw(Rect)
            End If
        End Sub

        Public Overrides Sub MouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
            MyBase.MouseMove(e)
            If Not Enabled Then Return
            If e.Button = MouseButtons.Left Then
                Dim x As Integer = e.X - Position.X
                Dim NewValue As Decimal = 0
                If x >= Size.Width Then
                    NewValue = 1
                ElseIf x <= 0 Then
                    NewValue = 0
                Else
                    NewValue = x / Size.Width
                End If
                If Value = NewValue Then Return
                Value = NewValue

                Send(Value)
                DoDraw(Rect)
            End If
        End Sub

        Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
            MyBase.Draw(g)

            If Enabled Then
                g.FillRectangle(Brushes.Blue, Position.X, Position.Y, Value * Size.Width, 20)
            Else
                g.FillRectangle(Brushes.Gray, Position.X, Position.Y, Value * Size.Width, 20)
            End If
            g.DrawString(Math.Round(Value * 100) & "%", DefaultFont, Brushes.White, Position + New Point(Size.Width * 0.5, 3))
        End Sub

        Public Overrides Sub Load(ByVal g As SimpleD.Group)

            g.GetValue("Enabled", Enabled, False)
            MyBase.Load(g)
        End Sub

        Public Overrides Function Save() As SimpleD.Group
            Dim g As SimpleD.Group = MyBase.Save()

            g.SetValue("Enabled", Enabled)

            Return g
        End Function
    End Class
End Namespace