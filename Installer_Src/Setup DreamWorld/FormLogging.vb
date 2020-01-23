﻿Public Class FormLogging

#Region "Private Fields"

    Dim initted As Boolean = False

#End Region

#Region "FormPos"

    Private _screenPosition As ScreenPos
    Private Handler As New EventHandler(AddressOf Resize_page)

    Public Property ScreenPosition As ScreenPos
        Get
            Return _screenPosition
        End Get
        Set(value As ScreenPos)
            _screenPosition = value
        End Set
    End Property

    'The following detects  the location of the form in screen coordinates
    Private Sub Resize_page(ByVal sender As Object, ByVal e As System.EventArgs)
        'Me.Text = "Form screen position = " + Me.Location.ToString
        ScreenPosition.SaveXY(Me.Left, Me.Top)
    End Sub

    Private Sub SetScreen()
        Me.Show()
        ScreenPosition = New ScreenPos(Me.Name)
        AddHandler ResizeEnd, Handler
        Dim xy As List(Of Integer) = ScreenPosition.GetXY()
        Me.Left = xy.Item(0)
        Me.Top = xy.Item(1)
    End Sub

#End Region

#Region "Start/Stop"

    Private Sub Close_form(sender As Object, e As EventArgs) Handles Me.Closed

        Form1.Settings.SaveSettings()

    End Sub

    Private Sub Loaded(sender As Object, e As EventArgs) Handles Me.Load

        SetScreen()

        Select Case Form1.Settings.LogLevel.ToUpperInvariant
            Case "OFF"
                RadioOff.Checked = True
            Case "DEBUG"
                RadioDebug.Checked = True
            Case "INFO"
                RadioInfo.Checked = True
            Case "WARN"
                RadioWarn.Checked = True
            Case "ERROR"
                RadioError.Checked = True
            Case "FATAL"
                RadioFatal.Checked = True
            Case "ALL"
                RadioAll.Checked = True
            Case Else
                RadioInfo.Checked = True
        End Select

        Form1.HelpOnce("Logging")
        initted = True

    End Sub

#End Region

#Region "Help"

    Private Sub HelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpToolStripMenuItem.Click
        Form1.Help("Logging")
    End Sub

    Private Sub LoggingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoggingToolStripMenuItem.Click
        Form1.Help("Logging")
    End Sub

#End Region

#Region "SetLogging"

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioOff.CheckedChanged
        If Not initted Then Return
        If RadioOff.Checked Then Form1.SendMsg("off")
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioDebug.CheckedChanged
        If Not initted Then Return
        If RadioDebug.Checked Then Form1.SendMsg("debug")
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioInfo.CheckedChanged
        If Not initted Then Return
        If RadioInfo.Checked Then Form1.SendMsg("info")
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioWarn.CheckedChanged
        If Not initted Then Return
        If RadioWarn.Checked Then Form1.SendMsg("warn")
    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioError.CheckedChanged
        If Not initted Then Return
        If RadioError.Checked Then Form1.SendMsg("error")
    End Sub

    Private Sub RadioButton6_CheckedChanged(sender As Object, e As EventArgs) Handles RadioFatal.CheckedChanged
        If Not initted Then Return
        If RadioFatal.Checked Then Form1.SendMsg("fatal")
    End Sub

    Private Sub RadioButton7_CheckedChanged(sender As Object, e As EventArgs) Handles RadioAll.CheckedChanged
        If Not initted Then Return
        If RadioAll.Checked Then Form1.SendMsg("all")
    End Sub

#End Region

End Class