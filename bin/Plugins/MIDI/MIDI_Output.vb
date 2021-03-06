﻿'AddMenuObject|Output,Plugins.MIDI_Output|MIDI
'AddReferences(Sanford.Slim.dll)
Public Class MIDI_Output
    Inherits BaseObject

    Private Enabled As Boolean = True

    Private WithEvents Device As Sanford.Multimedia.Midi.OutputDevice

    Private numChannel As New NumericUpDown
    Private WithEvents chkMessageChannels As New CheckBox

    Private WithEvents comDevices As New ComboBox

#Region "Object stuff"
    Public Sub New(ByVal StartPosition As Point, ByVal UserData As String)
        Setup(UserData, StartPosition, 205) 'Setup the base rectangles.

        Inputs(New String() {"Enable,Boolean", _
                             "Channel Message,ChannelMessage,ChannelMessageBuilder", "SysCommonMessage,SysCommonMessage,SysCommonMessageBuilder", _
                             "SysExMessage,SysExMessage", "SysRealtimeMessage,SysRealtimeMessage"})

        'Set the title.
        Title = "MIDI Output"
        File = "MIDI\MIDI_Output.vb"

        chkMessageChannels.Text = "Same as message"
        chkMessageChannels.Width = 117
        chkMessageChannels.Checked = True
        chkMessageChannels.Location = Position + New Point(86, 25)
        AddControl(chkMessageChannels)


        numChannel.Minimum = 1
        numChannel.Maximum = 16
        numChannel.Width = 40
        numChannel.Enabled = False
        numChannel.Location = Position + New Point(45, 25)
        AddControl(numChannel)



        If Sanford.Multimedia.Midi.OutputDevice.DeviceCount > 0 Then
            comDevices.Width = 200
            comDevices.Location = Position
            comDevices.DropDownStyle = ComboBoxStyle.DropDownList

            For i As Integer = 0 To Sanford.Multimedia.Midi.OutputDevice.DeviceCount - 1
                comDevices.Items.Add(Sanford.Multimedia.Midi.OutputDevice.GetDeviceCapabilities(i).name)
            Next
            'comDevices.SelectedIndex = 0

            AddControl(comDevices)
        Else
            Log("Could not find any MIDI output devices!")
        End If

    End Sub

    Public Overrides Sub Dispose()
        chkMessageChannels.Dispose()
        numChannel.Dispose()
        comDevices.Dispose()

        If Device IsNot Nothing Then
            Device.Close()
            Device = Nothing
        End If
        MyBase.Dispose()
    End Sub

    Public Overrides Sub Moving()
        chkMessageChannels.Location = Position + New Point(86, 25)
        numChannel.Location = Position + New Point(45, 25)
        comDevices.Location = Position
    End Sub

    Public Overrides Sub Receive(ByVal Data As Object, ByVal sender As DataFlow)
        Select Case sender.Index
            Case 0 'Enable
                If Data <> Enabled Then
                    Enabled = Data
                End If


            Case 1 'ChannelMessage
                If Not Enabled Then Return

                If chkMessageChannels.Checked Then
                    If Data.GetType = GetType(Sanford.Multimedia.Midi.ChannelMessageBuilder) Then
                        Data.Build()
                        Device.Send(Data.Result)
                    ElseIf Data.GetType = GetType(Sanford.Multimedia.Midi.ChannelMessage) Then
                        Device.Send(Data)
                    Else
                        Log("Not ChannelMessageBuilder or ChannelMessage")
                        Return
                    End If
                Else

                    Dim message As Sanford.Multimedia.Midi.ChannelMessageBuilder
                    If Data.GetType = GetType(Sanford.Multimedia.Midi.ChannelMessageBuilder) Then
                        message = Data
                    ElseIf Data.GetType = GetType(Sanford.Multimedia.Midi.ChannelMessage) Then
                        message = New Sanford.Multimedia.Midi.ChannelMessageBuilder(Data)
                    Else
                        Log("Not ChannelMessageBuilder or ChannelMessage")
                        Return
                    End If

                    message.MidiChannel = numChannel.Value - 1
                    message.Build()
                    Device.Send(message.Result)

                End If


            Case 2 'SysCommonMessage
                If Not Enabled Then Return
                If Data.GetType = GetType(Sanford.Multimedia.Midi.SysCommonMessageBuilder) Then
                    Dim message As Sanford.Multimedia.Midi.SysCommonMessageBuilder = Data
                    message.Build()
                    Device.Send(message.Result)
                ElseIf Data.GetType = GetType(Sanford.Multimedia.Midi.SysCommonMessage) Then
                    Device.Send(Data)
                Else
                    Log("Not SysCommonMessageBuilder or SysCommonMessage")
                    Return
                End If

            Case 3 'SysExMessage
                If Not Enabled Then Return
                Device.Send(Data)

            Case 4 'SysRealtimeMessage
                If Not Enabled Then Return
                Device.Send(Data)
        End Select
    End Sub

    Public Overrides Sub Draw(ByVal g As System.Drawing.Graphics)
        MyBase.Draw(g)

        g.DrawString("Channel:", DefaultFont, DefaultFontBrush, Position.X, Position.Y + 28)
    End Sub

    Public Overrides Sub Load(ByVal g As SimpleD.Group)

        g.GetValue("Enabled", Enabled, False)
        g.GetValue("DeviceID", comDevices.SelectedIndex, True)
        g.GetValue("MessageChannels", chkMessageChannels.Checked, True)
        Try
            g.GetValue("Channel", numChannel.Value, True)
        Catch ex As Exception
        End Try

        MyBase.Load(g)
    End Sub

    Public Overrides Function Save() As SimpleD.Group
        Dim g As SimpleD.Group = MyBase.Save()

        g.SetValue("Enabled", Enabled)
        g.SetValue("DeviceID", comDevices.SelectedIndex)
        g.SetValue("MessageChannels", chkMessageChannels.Checked)
        g.SetValue("Channel", numChannel.Value)


        Return g
    End Function
#End Region

#Region "Control events"

    Private Sub comDevices_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles comDevices.SelectedIndexChanged
        If comDevices.SelectedIndex = -1 Then Return
        If Device IsNot Nothing Then
            Device.Close()
            Device = Nothing
        End If


        Try
            'Create the device.
            Device = New Sanford.Multimedia.Midi.OutputDevice(comDevices.SelectedIndex)
        Catch ex As Exception
            Log(ex.Message)
            Device = Nothing
        End Try
    End Sub

    Private Sub chkMessageChannels_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkMessageChannels.CheckedChanged
        numChannel.Enabled = Not chkMessageChannels.Checked
    End Sub
#End Region

#Region "MIDI events"
    Private Sub Device_Error(ByVal sender As Object, ByVal e As Sanford.Multimedia.ErrorEventArgs) Handles Device.Error
        Log(e.Error.Message)
    End Sub
#End Region

End Class
