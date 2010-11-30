﻿'AddMenuObject|Keyboard 128keys,Plugins.MIDI_Keyboard,120,128-0-0-11|MIDI
'AddMenuObject|Keyboard 88keys,Plugins.MIDI_Keyboard,120,88-26-5-4|MIDI
'AddMenuObject|Keyboard 61keys,Plugins.MIDI_Keyboard,120,61-40-4-4|MIDI

Public Class MIDI_Keyboard
    Inherits BaseObject

    Private Enabled As Boolean = True


    Private numChannel As New NumericUpDown
    Private chkFilterOtherChannels As New CheckBox

    Private NumKeys As Byte
    Private Offset, Offset2, OctaveOffset As Byte
    Private Width As Integer

#Region "Object stuff"
    Public Sub New(ByVal StartPosition As Point, ByVal UserData As String)
        Try
            Dim data() As String = Split(UserData, "-")
            NumKeys = data(0)
            Offset = data(1)
            Offset2 = data(2)
            OctaveOffset = data(3)
        Catch ex As Exception
            NumKeys = 24
            Offset = 62
            Offset2 = 2
            OctaveOffset = 4
        End Try
        Width = NumKeys * 5.9

        Setup(UserData, StartPosition, Width, 95) 'Setup the base rectangles.

        'Create one output.
        Outputs(New String() {"Channel Message,ChannelMessage,ChannelMessageBuilder"})

        Inputs(New String() {"Enable,Boolean", "Channel Message,ChannelMessage"})



        'Set the title.
        Title = "MIDI Keyboard"

        chkFilterOtherChannels.Text = "Filter out other channels"
        chkFilterOtherChannels.Width = 139
        chkFilterOtherChannels.Height = 15
        chkFilterOtherChannels.Checked = False
        chkFilterOtherChannels.Location = Position + New Point(5, 19)
        AddControl(chkFilterOtherChannels)

        numChannel.Minimum = 1
        numChannel.Maximum = 16
        numChannel.Width = 40
        numChannel.Location = Position + New Point(55, 0)
        AddControl(numChannel)


    End Sub

    Public Overrides Sub Dispose()
        chkFilterOtherChannels.Dispose()
        numChannel.Dispose()


        MyBase.Dispose()
    End Sub

    Public Overrides Sub Moving()
        chkFilterOtherChannels.Location = Position + New Point(5, 19)
        numChannel.Location = Position + New Point(55, 0)
    End Sub

    Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
        MyBase.Draw(g)

        g.DrawString("Channel:", DefaultFont, DefaultFontBrush, Position.X + 5, Position.Y + 3)

        g.FillRectangle(Brushes.White, Position.X, Position.Y + 40, Width, 50)

        Dim OctavePos As Byte = SetBounds(Offset + OctaveOffset, 0, 11) '+ 4
        Dim x As Integer = Position.X
        Dim y As Integer = Position.Y + 40
        Dim p As Integer = -1

        For n As Integer = 0 To NumKeys - 1
            OctavePos += 1
            If OctavePos = 12 Then OctavePos = 0
            Select Case OctavePos
                Case 0, 2, 4, 5, 7, 9, 11
                    p += 1
            End Select

            Select Case OctavePos
                Case 0 'C
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p), y, 6, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If

                Case 1 'C#
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.DarkGreen, x + (10 * p) + 6, y, 6, 25)
                    Else
                        g.FillRectangle(Brushes.Black, x + (10 * p) + 6, y, 6, 25)
                    End If

                Case 2 'D
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p) + 2, y, 5, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If

                Case 3 'D#
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.DarkGreen, x + (10 * p) + 7, y, 6, 25)
                    Else
                        g.FillRectangle(Brushes.Black, x + (10 * p) + 7, y, 6, 25)
                    End If

                Case 4 'E
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p) + 3, y, 7, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If

                Case 5 'F
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p), y, 6, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If

                Case 6 'F#
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.DarkGreen, x + (10 * p) + 6, y, 6, 25)
                    Else
                        g.FillRectangle(Brushes.Black, x + (10 * p) + 6, y, 6, 25)
                    End If

                Case 7 'G
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p) + 2, y, 5, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If

                Case 8 'G#
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.DarkGreen, x + (10 * p) + 7, y, 6, 25)
                    Else
                        g.FillRectangle(Brushes.Black, x + (10 * p) + 7, y, 6, 25)
                    End If

                Case 9 'A
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p) + 2, y, 5, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If

                Case 10 'A#
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.DarkGreen, x + (10 * p) + 7, y, 6, 25)
                    Else
                        g.FillRectangle(Brushes.Black, x + (10 * p) + 7, y, 6, 25)
                    End If

                Case 11 'B
                    If Note(Offset + n - Offset2) Then
                        g.FillRectangle(Brushes.Green, x + (10 * p) + 3, y, 7, 25)
                        g.FillRectangle(Brushes.Green, x + (10 * p), y + 25, 10, 25)
                    End If


                    'Case 0, 2, 4, 5, 7, 9, 11
                    '    If Note(Offset + i - 4) Then
                    '        g.FillRectangle(Brushes.Green, x + (10 * i), y + 20, 10, 30)
                    '    End If

                    'Case 1, 3, 6, 8, 10

                    '    Dim b As Brush = Brushes.Black
                    '    If Note(Offset + i - 4) = True Then b = Brushes.Green
                    '    g.FillRectangle(b, x + (10 * i), y, 7, 20)

            End Select
        Next

    End Sub

    Public Overrides Sub Receive(ByVal Data As Object, ByVal sender As DataFlow)
        Select Case sender.Index
            Case 0 'Enable
                If Data <> Enabled Then
                    Enabled = Data
                End If


            Case 1 'ChannelMessage
                If Not Enabled Then Return
                If Data.MidiChannel <> numChannel.Value - 1 Then
                    If chkFilterOtherChannels.Checked Then
                        Return
                    Else
                        Send(Data)
                        Return
                    End If
                End If

                Dim message As Sanford.Multimedia.Midi.ChannelMessageBuilder
                If Data.GetType = GetType(Sanford.Multimedia.Midi.ChannelMessage) Then
                    message = New Sanford.Multimedia.Midi.ChannelMessageBuilder(Data)
                Else
                    message = Data
                End If



                'Is it a note (on or off)?
                If (message.Command = Sanford.Multimedia.Midi.ChannelCommand.NoteOn Or _
                    message.Command = Sanford.Multimedia.Midi.ChannelCommand.NoteOff) Then

                    'Is the note on? (volume more then 0)
                    If message.Data2 > 0 Then
                        Note(message.Data1) = True
                    Else
                        Note(message.Data1) = False
                    End If
                    DoDraw(True)

                End If

                Send(message)
        End Select
    End Sub

    Public Overrides Sub Load(ByVal g As SimpleD.Group)

        g.Get_Value("Enabled", Enabled, False)
        g.Get_Value("FilterOtherChannels", chkFilterOtherChannels.Checked)

        g.Get_Value("Channel", numChannel.Value, False)
        MyBase.Load(g)
    End Sub

    Public Overrides Function Save() As SimpleD.Group
        Dim g As SimpleD.Group = MyBase.Save()

        g.Set_Value("Enabled", Enabled)
        g.Set_Value("FilterOtherChannels", chkFilterOtherChannels.Checked)
        g.Set_Value("Channel", numChannel.Value)


        Return g
    End Function

    Public Overrides Sub MouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        'Is the mouse over the keyboard?
        Dim x As Integer = Position.X
        Dim y As Integer = Position.Y + 40
        If Mouse.IntersectsWith(New Rectangle(x, y, Width, 50)) Then
            Dim OctavePos As Byte = SetBounds(Offset + OctaveOffset, 0, 11) '+ 4
            Dim p As Integer = -1

            For n As Integer = 0 To NumKeys - 1
                OctavePos += 1
                If OctavePos = 12 Then OctavePos = 0
                Select Case OctavePos
                    Case 0, 2, 4, 5, 7, 9, 11
                        p += 1
                End Select

                Select Case OctavePos
                    Case 0 'C
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p), y, 6, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 1 'C#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 6, y, 6, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If


                    Case 2 'D
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 2, y, 5, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 3 'D#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 7, y, 6, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 4 'E
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 3, y, 7, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 5 'F
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p), y, 6, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 6 'F#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 6, y, 6, 25)) Then
                            PressNote(Offset + n - Offset2)
                        End If

                    Case 7 'G
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 2, y, 5, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 8 'G#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 7, y, 6, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 9 'A
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 2, y, 5, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 10 'A#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 7, y, 6, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 11 'B
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 3, y, 7, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            PressNote(Offset + n - Offset2)
                            Return
                        End If


                End Select
            Next

            Return
        End If
        MyBase.MouseDown(e)
    End Sub
    Public Overrides Sub MouseUp(ByVal e As System.Windows.Forms.MouseEventArgs)
        'Is the mouse over the keyboard?
        Dim x As Integer = Position.X
        Dim y As Integer = Position.Y + 40
        If Mouse.IntersectsWith(New Rectangle(x, y, Width, 50)) Then
            Dim OctavePos As Byte = SetBounds(Offset + OctaveOffset, 0, 11) '+ 4
            Dim p As Integer = -1

            For n As Integer = 0 To NumKeys - 1
                OctavePos += 1
                If OctavePos = 12 Then OctavePos = 0
                Select Case OctavePos
                    Case 0, 2, 4, 5, 7, 9, 11
                        p += 1
                End Select

                Select Case OctavePos
                    Case 0 'C
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p), y, 6, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 1 'C#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 6, y, 6, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If


                    Case 2 'D
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 2, y, 5, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 3 'D#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 7, y, 6, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 4 'E
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 3, y, 7, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 5 'F
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p), y, 6, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 6 'F#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 6, y, 6, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                        End If

                    Case 7 'G
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 2, y, 5, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 8 'G#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 7, y, 6, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 9 'A
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 2, y, 5, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 10 'A#
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 7, y, 6, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If

                    Case 11 'B
                        If Mouse.IntersectsWith(New Rectangle(x + (10 * p) + 3, y, 7, 25)) Or _
                            Mouse.IntersectsWith(New Rectangle(x + (10 * p), y + 25, 10, 25)) Then
                            ReleaseNote(Offset + n - Offset2)
                            Return
                        End If


                End Select
            Next
            Return
        End If
        MyBase.MouseUp(e)
    End Sub
#End Region

#Region "Misc stuff"
    Private Note(127) As Boolean

    ''' <summary>
    ''' Release note at ID
    ''' </summary>
    ''' <param name="ID"></param>
    ''' <remarks></remarks>
    Private Sub ReleaseNote(ByVal ID As Integer)
        Dim tmp As New Sanford.Multimedia.Midi.ChannelMessageBuilder
        tmp.Command = Sanford.Multimedia.Midi.ChannelCommand.NoteOff
        tmp.Data1 = ID
        tmp.Data2 = 0
        Note(ID) = False
        Send(tmp)
        DoDraw(True)
    End Sub

    Private Sub PressNote(ByVal ID As Integer)
        Dim tmp As New Sanford.Multimedia.Midi.ChannelMessageBuilder
        tmp.Command = Sanford.Multimedia.Midi.ChannelCommand.NoteOn
        tmp.Data1 = ID
        tmp.Data2 = 127
        Note(ID) = True
        Send(tmp)
        DoDraw(True)
    End Sub

    ''' <summary>
    ''' This will wrap a number around so it is in the boundaries.
    ''' v1 changed to byte and removed max trys.
    ''' </summary>
    Public Function SetBounds(ByVal value As Byte, ByVal Min As Byte, ByVal Max As Byte) As Single
Restart:
        If value < Min Then
            value = Max + (value - Min)
            GoTo Restart
        ElseIf value > Max Then
            value = Min + (value - Max)
            GoTo Restart
        End If
        Return value
    End Function
#End Region

End Class