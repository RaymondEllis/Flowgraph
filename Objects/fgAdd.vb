﻿Public Class fgAdd
    Inherits BaseObject

    Public Sub New(ByVal Position As Point)
        Setup("fgAdd", Position, 60, 50) 'Setup the base rectangles.

        'Create one input.
        Inputs(New String() {"Value 1|Number", "Value 2|Number"})
        'Create 4 outputs.
        Outputs(New String() {"Out value|Number"})

        'Set the title.
        Title = "Add"

    End Sub

    Private Value1, Value2 As Integer
    Public Overrides Sub Receive(ByVal Data As Object, ByVal sender As DataFlow)

        If sender.Index = 0 Then
            Value1 = Data
        Else
            Value2 = Data
        End If

        Send(Value1 + Value2)
    End Sub

End Class
