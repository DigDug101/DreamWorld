#Region "Copyright"

' Copyright 2014 Fred Beckhusen for outworldz.com https://opensource.org/licenses/AGPL

'Permission Is hereby granted, free Of charge, to any person obtaining a copy of this software
' And associated documentation files (the "Software"), to deal in the Software without restriction,
'including without limitation the rights To use, copy, modify, merge, publish, distribute, sublicense,
'And/Or sell copies Of the Software, And To permit persons To whom the Software Is furnished To
'Do so, subject To the following conditions:

'The above copyright notice And this permission notice shall be included In all copies Or '
'substantial portions Of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or IMPLIED,
' INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY, FITNESS For A PARTICULAR
'PURPOSE And NONINFRINGEMENT.In NO Event SHALL THE AUTHORS Or COPYRIGHT HOLDERS BE LIABLE
'For ANY CLAIM, DAMAGES Or OTHER LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or
'OTHERWISE, ARISING FROM, OUT Of Or In CONNECTION With THE SOFTWARE Or THE USE Or OTHER
'DEALINGS IN THE SOFTWARE.Imports System

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions

Public Class FormRegion

#Region "Declarations"

    Dim _RegionUUID As String = ""
    Dim BoxSize As Integer = 256
    Dim changed As Boolean
    Dim initted As Boolean = False

    ' needed a flag to see if we are initted as the dialogs change on start. true if we need to save a form
    Dim isNew As Boolean = False

    Dim oldname As String = ""
    Dim RName As String

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Property Changed1 As Boolean
        Get
            Return changed
        End Get
        Set(value As Boolean)
            changed = value
        End Set
    End Property

    Public Property Initted1 As Boolean
        Get
            Return initted
        End Get
        Set(value As Boolean)
            initted = value
        End Set
    End Property

    Public Property IsNew1 As Boolean
        Get
            Return isNew
        End Get
        Set(value As Boolean)
            isNew = value
        End Set
    End Property

    Public Property Oldname1 As String
        Get
            Return oldname
        End Get
        Set(value As String)
            oldname = value
        End Set
    End Property

    Public Property RegionUUID As String
        Get
            Return _RegionUUID
        End Get
        Set(value As String)
            _RegionUUID = value
        End Set
    End Property

    Public Property RName1 As String
        Get
            Return RName
        End Get
        Set(value As String)
            RName = value
        End Set
    End Property

#End Region

#Region "Start_Stop"

    Public Shared Function FilenameIsOK(ByVal fileName As String) As Boolean
        ' check for invalid chars in file name for INI file
        If fileName Is Nothing Then Return False
        Dim value As Boolean = False
        Try
            value = Not fileName.Intersect(Path.GetInvalidFileNameChars()).Any()
        Catch ex As ArgumentNullException
        End Try

        Return value

    End Function

    Shared Function RegionChosen(regionName As String) As String

        Dim Chooseform As New Choice ' form for choosing a set of regions
        ' Show testDialog as a modal dialog and determine if DialogResult = OK.

        Chooseform.FillGrid("Group")  ' populate the grid with either Group or RegionName

        Dim chosen As String
        Chooseform.ShowDialog()


        Try
            ' Read the chosen sim name
            chosen = Chooseform.DataGridView.CurrentCell.Value.ToString()
            If chosen = "! Add New Name" Then
                chosen = InputBox(My.Resources.Enter_Dos_Name, "", regionName)
            End If
        Catch ex As ArgumentOutOfRangeException
            chosen = ""
        Catch ex As InvalidOperationException
            chosen = ""
        Catch ex As ArgumentException
            chosen = ""
        End Try

        Chooseform.Dispose()
        Return chosen

    End Function

    Public Sub Init(Name As String)

        '!!! remove for production
        If Debugger.IsAttached = False Then
            SmartStartCheckBox.Enabled = False
            Form1.Settings.SmartStart = False
        End If

        Me.Focus()

        If Name Is Nothing Then Return
        Name = Name.Trim() ' remove spaces

        Form1.PropRegionClass = RegionMaker.Instance()


        ' NEW REGION
        If Name.Length = 0 Then
            IsNew1 = True
            Gods_Use_Default.Checked = True
            RegionName.Text = My.Resources.Name_of_Region_Word
            UUID.Text = Guid.NewGuid().ToString

            CoordX.Text = (Form1.PropRegionClass.LargestX() + 4).ToString(Globalization.CultureInfo.InvariantCulture)
            CoordY.Text = (Form1.PropRegionClass.LargestY() + 0).ToString(Globalization.CultureInfo.InvariantCulture)
            RegionPort.Text = (Form1.PropRegionClass.LargestPort() + 1).ToString(Globalization.CultureInfo.InvariantCulture)
            EnabledCheckBox.Checked = True
            RadioButton1.Checked = True
            SmartStartCheckBox.Checked = False
            NonphysicalPrimMax.Text = 1024.ToString(Globalization.CultureInfo.InvariantCulture)
            PhysicalPrimMax.Text = 64.ToString(Globalization.CultureInfo.InvariantCulture)
            ClampPrimSize.Checked = False
            MaxPrims.Text = 45000.ToString(Globalization.CultureInfo.InvariantCulture)
            MaxAgents.Text = 100.ToString(Globalization.CultureInfo.InvariantCulture)
            RegionUUID = Form1.PropRegionClass.CreateRegion("")
            Gods_Use_Default.Checked = True
        Else
            ' OLD REGION EDITED all this is required to be filled in!
            IsNew1 = False
            RegionUUID = Form1.PropRegionClass.FindRegionByName(Name)
            Oldname1 = Form1.PropRegionClass.RegionName(RegionUUID) ' backup in case of rename
            EnabledCheckBox.Checked = Form1.PropRegionClass.RegionEnabled(RegionUUID)
            Me.Text = Name & " Region" ' on screen
            RegionName.Text = Name
            UUID.Text = RegionUUID
            NonphysicalPrimMax.Text = CStr(Form1.PropRegionClass.NonPhysicalPrimMax(RegionUUID))
            PhysicalPrimMax.Text = CStr(Form1.PropRegionClass.PhysicalPrimMax(RegionUUID))
            ClampPrimSize.Checked = Form1.PropRegionClass.ClampPrimSize(RegionUUID)
            MaxPrims.Text = Form1.PropRegionClass.MaxPrims(RegionUUID)
            MaxAgents.Text = Form1.PropRegionClass.MaxAgents(RegionUUID)
            RegionPort.Text = Form1.PropRegionClass.RegionPort(RegionUUID)
            ' Size buttons can be zero
            If Form1.PropRegionClass.SizeY(RegionUUID) = 0 Or Form1.PropRegionClass.SizeX(RegionUUID) = 0 Then
                RadioButton1.Checked = True
                BoxSize = 256
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 Then
                RadioButton1.Checked = True
                BoxSize = 256 * 1
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 2 Then
                RadioButton2.Checked = True
                BoxSize = 256 * 2
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 3 Then
                RadioButton3.Checked = True
                BoxSize = 256 * 3
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 4 Then
                RadioButton4.Checked = True
                BoxSize = 256 * 4
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 5 Then
                RadioButton5.Checked = True
                BoxSize = 256 * 5
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 6 Then
                RadioButton6.Checked = True
                BoxSize = 256 * 6
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 7 Then
                RadioButton7.Checked = True
                BoxSize = 256 * 7
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 8 Then
                RadioButton8.Checked = True
                BoxSize = 256 * 8
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 9 Then
                RadioButton9.Checked = True
                BoxSize = 256 * 9
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 10 Then
                RadioButton10.Checked = True
                BoxSize = 256 * 10
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 11 Then
                RadioButton11.Checked = True
                BoxSize = 256 * 11
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 12 Then
                RadioButton12.Checked = True
                BoxSize = 256 * 12
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 13 Then
                RadioButton13.Checked = True
                BoxSize = 256 * 14
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 14 Then
                RadioButton14.Checked = True
                BoxSize = 256 * 14
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 15 Then
                RadioButton15.Checked = True
                BoxSize = 256 * 15
            ElseIf Form1.PropRegionClass.SizeY(RegionUUID) = 256 * 16 Then
                RadioButton16.Checked = True
                BoxSize = 256 * 16
            Else
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                RadioButton3.Checked = False
                RadioButton4.Checked = False
                RadioButton5.Checked = False
                RadioButton6.Checked = False
                RadioButton7.Checked = False
                RadioButton8.Checked = False
                RadioButton9.Checked = False
                RadioButton10.Checked = False
                RadioButton11.Checked = False
                RadioButton12.Checked = False
                RadioButton13.Checked = False
                RadioButton14.Checked = False
                RadioButton15.Checked = False
                RadioButton16.Checked = False
            End If

            ' global coordinates
            If Form1.PropRegionClass.CoordX(RegionUUID) <> 0 Then
                CoordX.Text = Form1.PropRegionClass.CoordX(RegionUUID).ToString(Globalization.CultureInfo.InvariantCulture)
            End If

            If Form1.PropRegionClass.CoordY(RegionUUID) <> 0 Then
                CoordY.Text = Form1.PropRegionClass.CoordY(RegionUUID).ToString(Globalization.CultureInfo.InvariantCulture)
            End If

        End If

        ' The following are all options.
        If Form1.PropRegionClass.DisallowResidents(RegionUUID) = "True" Then
            DisallowResidents.Checked = True
        End If

        If Form1.PropRegionClass.DisallowForeigners(RegionUUID) = "True" Then
            DisallowForeigners.Checked = True
        End If

        ScriptTimerTextBox.Text = Form1.PropRegionClass.MinTimerInterval(RegionUUID).ToString(Globalization.CultureInfo.InvariantCulture)
        FrametimeBox.Text = Form1.PropRegionClass.FrameTime(RegionUUID).ToString(Globalization.CultureInfo.InvariantCulture)

        If Form1.PropRegionClass.SkipAutobackup(RegionUUID) = "True" Then
            SkipAutoCheckBox.Checked = "True"
        End If
        If Form1.PropRegionClass.SmartStart(RegionUUID) = "True" Then
            SmartStartCheckBox.Checked = True
        End If

        Select Case Form1.PropRegionClass.DisableGloebits(RegionUUID)
            Case ""
                DisableGBCheckBox.Checked = False
            Case "False"
                DisableGBCheckBox.Checked = False
            Case "True"
                DisableGBCheckBox.Checked = True
        End Select

        RName1 = Name

        ''''''''''''''''''''''''''''' DREAMGRID REGION LOAD '''''''''''''''''

        If Form1.PropRegionClass.MapType(RegionUUID).Length = 0 Then
            Maps_Use_Default.Checked = True
        ElseIf Form1.PropRegionClass.MapType(RegionUUID) = "None" Then
            MapNone.Checked = True
            MapPicture.Image = My.Resources.blankbox
        ElseIf Form1.PropRegionClass.MapType(RegionUUID) = "Simple" Then
            MapSimple.Checked = True
            MapPicture.Image = My.Resources.Simple
        ElseIf Form1.PropRegionClass.MapType(RegionUUID) = "Good" Then
            MapGood.Checked = True
            MapPicture.Image = My.Resources.Good
        ElseIf Form1.PropRegionClass.MapType(RegionUUID) = "Better" Then
            MapBetter.Checked = True
            MapPicture.Image = My.Resources.Better
        ElseIf Form1.PropRegionClass.MapType(RegionUUID) = "Best" Then
            MapBest.Checked = True
            MapPicture.Image = My.Resources.Best
        End If

        Select Case Form1.PropRegionClass.Physics(RegionUUID)
            Case "" : Physics_Default.Checked = True
            Case "-1" : Physics_Default.Checked = True
            Case "0" : Physics_Default.Checked = True
            Case "1" : Physics_Default.Checked = True
            Case "2" : Physics_Default.Checked = True
            Case "3" : PhysicsSeparate.Checked = True
            Case "4" : PhysicsubODE.Checked = True
            Case "5" : Physics_Default.Checked = True
            Case Else : Physics_Default.Checked = True
        End Select

        If Form1.PropRegionClass.GodDefault(RegionUUID) Then
            AllowGods.Checked = False
            RegionGod.Checked = False
            ManagerGod.Checked = False
            Gods_Use_Default.Checked = True
        Else
            Select Case Form1.PropRegionClass.AllowGods(RegionUUID)
                Case ""
                    AllowGods.Checked = False
                Case "False"
                    AllowGods.Checked = False
                    Gods_Use_Default.Checked = False
                Case "True"
                    AllowGods.Checked = True
                    Gods_Use_Default.Checked = False
            End Select

            Select Case Form1.PropRegionClass.RegionGod(RegionUUID)
                Case ""
                    RegionGod.Checked = False
                Case "False"
                    RegionGod.Checked = False
                    Gods_Use_Default.Checked = False
                Case "True"
                    RegionGod.Checked = True
                    Gods_Use_Default.Checked = False
            End Select

            Select Case Form1.PropRegionClass.ManagerGod(RegionUUID)
                Case ""
                    ManagerGod.Checked = False
                Case "False"
                    ManagerGod.Checked = False
                    Gods_Use_Default.Checked = False
                Case "True"
                    ManagerGod.Checked = True
                    Gods_Use_Default.Checked = False
            End Select

            ' if none selected, turn default on. This updates old code to new GodDefault global

            If Form1.PropRegionClass.AllowGods(RegionUUID) = "" And
                 Form1.PropRegionClass.RegionGod(RegionUUID) = "" And
                Form1.PropRegionClass.ManagerGod(RegionUUID) = "" Then
                Gods_Use_Default.Checked = True
            End If
        End If

        Select Case Form1.PropRegionClass.RegionSnapShot(RegionUUID)
            Case ""
                PublishDefault.Checked = True
                NoPublish.Checked = False
                Publish.Checked = False
            Case "False"
                PublishDefault.Checked = False
                NoPublish.Checked = True
                Publish.Checked = False
            Case "True"
                PublishDefault.Checked = False
                NoPublish.Checked = False
                Publish.Checked = True
        End Select

        Select Case Form1.PropRegionClass.Birds(RegionUUID)
            Case ""
                BirdsCheckBox.Checked = False
            Case "False"
                BirdsCheckBox.Checked = False
            Case "True"
                BirdsCheckBox.Checked = True
        End Select

        Select Case Form1.PropRegionClass.Tides(RegionUUID)
            Case ""
                TidesCheckbox.Checked = False
            Case "False"
                TidesCheckbox.Checked = False
            Case "True"
                TidesCheckbox.Checked = True
        End Select

        Select Case Form1.PropRegionClass.Teleport(RegionUUID)
            Case ""
                TPCheckBox1.Checked = False
            Case "False"
                TPCheckBox1.Checked = False
            Case "True"
                TPCheckBox1.Checked = True
        End Select

        Select Case Form1.PropRegionClass.DisallowForeigners(RegionUUID)
            Case ""
                DisallowForeigners.Checked = False
            Case "False"
                DisallowForeigners.Checked = False
            Case "True"
                DisallowForeigners.Checked = True
        End Select

        Select Case Form1.PropRegionClass.DisallowResidents(RegionUUID)
            Case ""
                DisallowResidents.Checked = False
            Case "False"
                DisallowResidents.Checked = False
            Case "True"
                DisallowResidents.Checked = True
        End Select

        Select Case Form1.PropRegionClass.ScriptEngine(RegionUUID)
            Case ""
                ScriptDefaultButton.Checked = True
            Case "XEngine"
                XEngineButton.Checked = False
            Case "YEngine"
                YEngineButton.Checked = True
        End Select

        Try
            Me.Show() ' time to show the results
            Me.Activate()

            Initted1 = True
            Form1.HelpOnce("Region")
#Disable Warning CA1031
        Catch ex As Exception
#Enable Warning CA1031
        End Try

    End Sub


    Private Sub BirdsCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles BirdsCheckBox.CheckedChanged

        If BirdsCheckBox.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " has birds enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim message = RegionValidate()
        If Len(message) > 0 Then
            Dim v = MsgBox(message + vbCrLf + My.Resources.Discard_Exit, vbYesNo, My.Resources.Info)
            If v = vbYes Then
                Me.Close()
            End If
        Else
            Form1.PropViewedSettings = True ' set this so it will force a rescan of the regions on startup
            WriteRegion(RegionUUID)
            RegionMaker.CopyOpensimProto(RegionName.Text)
            Form1.PropUpdateView = True ' make form refresh
            Changed1 = False
            Me.Close()
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles DeregisterButton.Click

        Dim response As MsgBoxResult = MsgBox(My.Resources.Another_Region, vbYesNo)
        If response = vbYes Then

            Form1.StartMySQL()
            Form1.StartRobust()

            Dim RegionUUID As String = Form1.PropRegionClass.FindRegionByName(RegionName.Text)
            If RegionUUID.Length > 0 Then
                If Form1.CheckPort(Form1.Settings.PrivateURL, Form1.PropRegionClass.GroupPort(RegionUUID)) Then
                    Form1.SequentialPause()
                    Form1.ConsoleCommand(RegionUUID, "q{ENTER}" + vbCrLf)
                End If
                Dim loopctr = 60 ' wait a minute
                While Form1.CheckPort(Form1.Settings.PrivateURL, Form1.PropRegionClass.GroupPort(RegionUUID)) And loopctr > 0
                    Form1.Sleep(1000)
                    loopctr -= 1
                End While

                If loopctr > 0 Then
                    Form1.ConsoleCommand("Robust", "deregister region id " + RegionUUID + "{ENTER}" + vbCrLf)
                    Form1.Print(My.Resources.Reion_Removed)
                End If
            End If

        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles DisallowForeigners.CheckedChanged
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub CheckBox1_CheckedChanged_1(sender As Object, e As EventArgs) Handles DisallowResidents.CheckedChanged
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub ClampPrimSize_CheckedChanged(sender As Object, e As EventArgs) Handles ClampPrimSize.CheckedChanged
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub CoordX_TextChanged(sender As Object, e As EventArgs) Handles CoordX.TextChanged

        If Not initted Then Return
        Dim digitsOnly As Regex = New Regex("[^\d]")
        CoordX.Text = digitsOnly.Replace(CoordX.Text, "")
        Changed1 = True

    End Sub

    Private Sub Coordy_TextChanged(sender As Object, e As EventArgs) Handles CoordY.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        CoordY.Text = digitsOnly.Replace(CoordY.Text, "")
        If Initted1 And CoordY.Text.Length >= 0 Then
            Changed1 = True
        End If

    End Sub

    Private Sub DatabaseSetupToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Form1.Help("Region")
    End Sub

    Private Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click

        Dim msg = MsgBox(My.Resources.Are_you_Sure_Delete_Region, vbYesNo, My.Resources.Info)
        If msg = vbYes Then
            FileStuff.DeleteFile(Form1.PropOpensimBinPath & "bin\Regions\" + RegionName.Text + "\Region\" + RegionName.Text + ".bak")
            Try
                My.Computer.FileSystem.RenameFile(Form1.PropRegionClass.RegionPath(RegionUUID), RegionName.Text + ".bak")
            Catch ex As ArgumentNullException
            Catch ex As ArgumentException
            Catch ex As FileNotFoundException
            Catch ex As PathTooLongException
            Catch ex As IOException
            Catch ex As NotSupportedException
            Catch ex As Security.SecurityException
            Catch ex As UnauthorizedAccessException
            End Try
        End If

        Form1.PropRegionClass.DeleteRegion(RegionUUID)
        Form1.PropRegionClass.GetAllRegions()

        Form1.PropUpdateView = True

        Me.Close()

    End Sub

    Private Sub EnabledCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles EnabledCheckBox.CheckedChanged
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub EnableMaxPrims_text(sender As Object, e As EventArgs) Handles MaxPrims.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        MaxPrims.Text = digitsOnly.Replace(MaxPrims.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub FormRegion_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        If Changed1 Then
            Form1.PropViewedSettings = True
            Dim v = MsgBox(My.Resources.Save_changes_word, vbYesNo, My.Resources.Save_changes_word)
            If v = vbYes Then
                Dim message = RegionValidate()
                If Len(message) > 0 Then
                    v = MsgBox(message + vbCrLf + My.Resources.Discard_Exit, vbYesNo, My.Resources.Info)
                    If v = vbYes Then
                        Me.Close()
                    End If
                Else
                    WriteRegion(RegionUUID)
                    RegionMaker.CopyOpensimProto(RegionName.Text)

                    Form1.PropUpdateView() = True
                    Changed1 = False
                End If
            End If
        End If

    End Sub

#End Region

#Region "Functions"

#Region "Private Methods"

    Private Sub GodHelp_Click(sender As Object, e As EventArgs) Handles GodHelp.Click
        Form1.Help("Permissions")
    End Sub


    Private Sub MapBest_CheckedChanged(sender As Object, e As EventArgs) Handles MapBest.CheckedChanged

        If MapBest.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Map Is set to Best")
            MapPicture.Image = My.Resources.Best
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapBetter_CheckedChanged(sender As Object, e As EventArgs) Handles MapBetter.CheckedChanged

        If MapBetter.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Map Is set to Better")
            MapPicture.Image = My.Resources.Better
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapGood_CheckedChanged(sender As Object, e As EventArgs) Handles MapGood.CheckedChanged

        If MapGood.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Map Is set to Good")
            MapPicture.Image = My.Resources.Good
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapHelp_Click(sender As Object, e As EventArgs) Handles MapHelp.Click

        Form1.Help("Region")

    End Sub

    Private Sub MapNone_CheckedChanged(sender As Object, e As EventArgs) Handles MapNone.CheckedChanged

        If MapNone.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Map Is set to None")
            MapPicture.Image = My.Resources.blankbox
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Maps_Use_Default_changed(sender As Object, e As EventArgs) Handles Maps_Use_Default.CheckedChanged

        If Maps_Use_Default.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Map Is set to Default")
            MapNone.Checked = False
            MapSimple.Checked = False
            MapGood.Checked = False
            MapBetter.Checked = False
            MapBest.Checked = False

            If Form1.Settings.MapType = "None" Then
                MapPicture.Image = My.Resources.blankbox
            ElseIf Form1.Settings.MapType = "Simple" Then
                MapPicture.Image = My.Resources.Simple
            ElseIf Form1.Settings.MapType = "Good" Then
                MapPicture.Image = My.Resources.Good
            ElseIf Form1.Settings.MapType = "Better" Then
                MapPicture.Image = My.Resources.Better
            ElseIf Form1.Settings.MapType = "Best" Then
                Form1.Settings.MapType = "Best"
            End If
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapSimple_CheckedChanged(sender As Object, e As EventArgs) Handles MapSimple.CheckedChanged

        If MapSimple.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Map Is set to Simple")
            MapPicture.Image = My.Resources.Simple
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MaxAgents_TextChanged(sender As Object, e As EventArgs) Handles MaxAgents.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        MaxAgents.Text = digitsOnly.Replace(MaxAgents.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub NonphysicalPrimMax_TextChanged(sender As Object, e As EventArgs) Handles NonphysicalPrimMax.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        NonphysicalPrimMax.Text = digitsOnly.Replace(NonphysicalPrimMax.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub NoPublish_CheckedChanged(sender As Object, e As EventArgs) Handles NoPublish.CheckedChanged

        If NoPublish.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Is Not set to publish snapshots")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicalPrimMax_TextChanged(sender As Object, e As EventArgs) Handles PhysicalPrimMax.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        PhysicalPrimMax.Text = digitsOnly.Replace(PhysicalPrimMax.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Physics_Default_CheckedChanged(sender As Object, e As EventArgs) Handles Physics_Default.CheckedChanged

        If Physics_Default.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Physics Is set to default")
            PhysicsubODE.Checked = False
            PhysicsSeparate.Checked = False
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsSeparate_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsSeparate.CheckedChanged

        If PhysicsSeparate.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Physics Is set to Bullet in a Thread")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsubODE_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsubODE.CheckedChanged

        If PhysicsubODE.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Physics Is set to Ubit's ODE")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Publish_CheckedChanged(sender As Object, e As EventArgs) Handles Publish.CheckedChanged

        If Publish.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " is publishing snapshots")
        Else
            Form1.Log(My.Resources.Info, "Region " + Name + " is not publishing snapshots")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PublishDefault_CheckedChanged(sender As Object, e As EventArgs) Handles PublishDefault.CheckedChanged

        If PublishDefault.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " is set to default for snapshots")
        End If
        If Initted1 Then Changed1 = True

    End Sub


    Private Function RegionValidate() As String

        Dim Message As String

        If Len(RegionName.Text) = 0 Then
            Message = My.Resources.Region_name_must_not_be_blank_word
            Return Message
        End If

        ' UUID
        Dim result As Guid
        If Not Guid.TryParse(UUID.Text, result) Then
            Message = My.Resources.Region_UUID_Is_invalid_word & " " & +UUID.Text
            Return Message
        End If

        ' global coords
        If Convert.ToInt32("0" & CoordX.Text, Globalization.CultureInfo.InvariantCulture) < 0 Then
            Message = My.Resources.Region_Coordinate_X_cannot_be_less_than_0_word
            Return Message
        ElseIf Convert.ToInt32("0" & CoordX.Text, Globalization.CultureInfo.InvariantCulture) > 65536 Then
            Message = My.Resources.Region_Coordinate_X_is_too_large
            Return Message
        End If

        If Convert.ToInt32("0" & CoordY.Text, Globalization.CultureInfo.InvariantCulture) < 32 Then
            Message = My.Resources.Region_Coordinate_Y_cannot_be_less_than_32
            Return Message
        ElseIf Convert.ToInt32("0" & CoordY.Text, Globalization.CultureInfo.InvariantCulture) > 65536 Then
            Message = My.Resources.Region_Coordinate_Y_Is_too_large
            Return Message
        End If

        Dim aresult As Guid
        If Not Guid.TryParse(UUID.Text, aresult) Then
            Message = My.Resources.UUID0
            Return Message
        End If

        Try
            If (NonphysicalPrimMax.Text.Length = 0) Or (CType(NonphysicalPrimMax.Text, Integer) <= 0) Then
                Message = My.Resources.NVNonPhysPrim
                Return Message
            End If
        Catch ex As InvalidCastException
            Message = My.Resources.NVNonPhysPrim
            Return Message
        Catch ex As OverflowException
            Message = My.Resources.NVNonPhysPrim
            Return Message
        End Try

        Try
            If (PhysicalPrimMax.Text.Length = 0) Or (CType(PhysicalPrimMax.Text, Integer) <= 0) Then
                Message = My.Resources.NVPhysPrim
                Return Message
            End If
        Catch ex As InvalidCastException
            Message = My.Resources.NVPhysPrim
            Return Message
        Catch ex As OverflowException
            Message = My.Resources.NVPhysPrim
            Return Message
        End Try

        Try
            If (MaxPrims.Text.Length = 0) Or (CType(MaxPrims.Text, Integer) <= 0) Then
                Message = My.Resources.NVMaxPrim
                Return Message
            End If
        Catch ex As InvalidCastException
            Message = My.Resources.NVMaxPrim
            Return Message
        Catch ex As OverflowException
            Message = My.Resources.NVMaxPrim
            Return Message
        End Try
        Try
            If (MaxAgents.Text.Length = 0) Or (CType(MaxAgents.Text, Integer) <= 0) Then
                Message = My.Resources.NVMaxAgaents
                Return Message
            End If
        Catch ex As InvalidCastException
            Message = My.Resources.NVMaxAgaents
            Return Message
        Catch ex As OverflowException
            Message = My.Resources.NVMaxAgaents
            Return Message
        End Try
        Return ""
    End Function

    Private Sub RLost(sender As Object, e As EventArgs) Handles RegionName.LostFocus
        RegionName.Text = RegionName.Text.Trim() ' remove spaces
    End Sub

    Private Sub RLostFocus(sender As Object, e As EventArgs) Handles RegionName.TextChanged
        If Len(RegionName.Text) > 0 And Initted1 Then
            If Not FilenameIsOK(RegionName.Text) Then
                MsgBox(My.Resources.Region_Names_Special & " < > : """" / \ | ? *", vbInformation, My.Resources.Info)
                Return
            End If

            Changed1 = True
        End If
    End Sub

    Private Sub ScriptTimerTextBox_TextChanged(sender As Object, e As EventArgs) Handles ScriptTimerTextBox.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d\.]")
        ScriptTimerTextBox.Text = digitsOnly.Replace(ScriptTimerTextBox.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub SkipAutoCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles SkipAutoCheckBox.CheckedChanged

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub SmartStartCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles SmartStartCheckBox.CheckedChanged

        If SmartStartCheckBox.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " has Smart Start enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub StopHGCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles DisableGBCheckBox.CheckedChanged

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles FrametimeBox.TextChanged

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub TidesCheckbox_CheckedChanged(sender As Object, e As EventArgs) Handles TidesCheckbox.CheckedChanged

        If TidesCheckbox.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " has tides enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub ToolStripMenuItem30_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem30.Click
        Form1.Help("Region")
    End Sub

    Private Sub TPCheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles TPCheckBox1.CheckedChanged

        If TPCheckBox1.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " has Teleport Board enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub UUID_LostFocus(sender As Object, e As EventArgs) Handles UUID.LostFocus

        If UUID.Text <> UUID.Text And Initted1 Then
            Dim resp = MsgBox(My.Resources.Change_UUID, vbYesNo, My.Resources.Info)
            If resp = vbYes Then
                Changed1 = True
                Dim result As Guid
                If Guid.TryParse(UUID.Text, result) Then
                Else
                    Dim ok = MsgBox(My.Resources.NotValidUUID, vbOKCancel, My.Resources.Info)
                    If ok = vbOK Then
                        UUID.Text = System.Guid.NewGuid.ToString
                    End If
                End If
            End If
        End If

    End Sub

    Private Sub UUID_TextChanged(sender As Object, e As EventArgs) Handles UUID.TextChanged

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub WriteRegion(RegionUUID As String)

        ' save the Region File, choose an existing DOS box to put it in, or make a new one

        Dim Filepath = Form1.PropRegionClass.RegionPath(RegionUUID)
        Dim Folderpath = Form1.PropRegionClass.FolderPath(RegionUUID)

        ' rename is possible
        If Oldname1 <> RegionName.Text And Not IsNew1 Then
            Try
                My.Computer.FileSystem.RenameFile(Filepath, RegionName.Text + ".ini")
            Catch ex As ArgumentNullException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As ArgumentException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As FileNotFoundException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As PathTooLongException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As IOException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As NotSupportedException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As Security.SecurityException
                Form1.Print(My.Resources.Aborted_word)
                Return
            Catch ex As UnauthorizedAccessException
                Form1.Print(My.Resources.Aborted_word)
                Return
            End Try

            Filepath = Folderpath + "\" + RegionName.Text + ".ini"
            Form1.PropRegionClass.RegionPath(RegionUUID) = Filepath

        End If

        ' might be a new region, so give them a choice

        If IsNew1 Then
            Dim NewGroup As String

            NewGroup = RegionChosen(RegionName.Text)
            If NewGroup.Length = 0 Then
                Form1.Print(My.Resources.Aborted_word)
                Return
            End If

            If Not Directory.Exists(Filepath) Or Filepath.Length = 0 Then
                Try
                    Directory.CreateDirectory(Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup + "\Region")
                Catch ex As ArgumentException
                    Form1.Print(My.Resources.Aborted_word)
                    Return
                Catch ex As IO.PathTooLongException
                    Form1.Print(My.Resources.Aborted_word)
                    Return
                Catch ex As NotSupportedException
                    Form1.Print(My.Resources.Aborted_word)
                    Return
                Catch ex As UnauthorizedAccessException
                    Form1.Print(My.Resources.Aborted_word)
                    Return
                Catch ex As IO.IOException
                    Form1.Print(My.Resources.Aborted_word)
                    Return
                End Try
            End If

            Form1.PropRegionClass.RegionPath(RegionUUID) = Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup + "\Region\" + RegionName.Text + ".ini"
            Filepath = Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup + "\Region\" + RegionName.Text + ".ini"
            Form1.PropRegionClass.FolderPath(RegionUUID) = Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup

        End If

        ' save the changes to the memory structure, then to disk
        Form1.PropRegionClass.UUID(RegionUUID) = UUID.Text
        Form1.PropRegionClass.CoordX(RegionUUID) = CoordX.Text
        Form1.PropRegionClass.CoordY(RegionUUID) = CoordY.Text
        Form1.PropRegionClass.RegionPort(RegionUUID) = RegionPort.Text
        Form1.PropRegionClass.SizeX(RegionUUID) = BoxSize
        Form1.PropRegionClass.SizeY(RegionUUID) = BoxSize
        Form1.PropRegionClass.RegionEnabled(RegionUUID) = EnabledCheckBox.Checked
        Form1.PropRegionClass.NonPhysicalPrimMax(RegionUUID) = NonphysicalPrimMax.Text
        Form1.PropRegionClass.PhysicalPrimMax(RegionUUID) = PhysicalPrimMax.Text
        Form1.PropRegionClass.ClampPrimSize(RegionUUID) = ClampPrimSize.Checked
        Form1.PropRegionClass.MaxAgents(RegionUUID) = MaxAgents.Text
        Form1.PropRegionClass.MaxPrims(RegionUUID) = MaxPrims.Text
        Form1.PropRegionClass.MinTimerInterval(RegionUUID) = ScriptTimerTextBox.Text
        Form1.PropRegionClass.FrameTime(RegionUUID) = FrametimeBox.Text

        Dim Snapshot As String = ""
        If PublishDefault.Checked Then
            Snapshot = ""
        ElseIf NoPublish.Checked Then
            Snapshot = "False"
        ElseIf Publish.Checked Then
            Snapshot = "True"
        End If

        Form1.PropRegionClass.RegionSnapShot(RegionUUID) = Snapshot

        Dim Map As String = ""
        If MapNone.Checked Then
            Map = ""
        ElseIf MapNone.Checked Then
            Map = "None"
        ElseIf MapSimple.Checked Then
            Map = "Simple"
        ElseIf MapGood.Checked Then
            Map = "Good"
        ElseIf MapBetter.Checked Then
            Map = "Better"
        ElseIf MapBest.Checked Then
            Map = "Best"
        End If

        Form1.PropRegionClass.MapType(RegionUUID) = Map

        Dim Phys As Integer = 2
        If Physics_Default.Checked Then
            Phys = -1
        ElseIf PhysicsSeparate.Checked Then
            Phys = 3
        ElseIf PhysicsubODE.Checked Then
            Phys = 4
        End If

        If Physics_Default.Checked Then
            Form1.PropRegionClass.Physics(RegionUUID) = Phys
        End If

        If Gods_Use_Default.Checked Then
            Form1.PropRegionClass.GodDefault(RegionUUID) = "True"
            Form1.PropRegionClass.AllowGods(RegionUUID) = ""
            Form1.PropRegionClass.RegionGod(RegionUUID) = ""
            Form1.PropRegionClass.ManagerGod(RegionUUID) = ""
        Else
            Form1.PropRegionClass.GodDefault(RegionUUID) = "False"
            Form1.PropRegionClass.AllowGods(RegionUUID) = CStr(AllowGods.Checked)
            Form1.PropRegionClass.RegionGod(RegionUUID) = CStr(RegionGod.Checked)
            Form1.PropRegionClass.ManagerGod(RegionUUID) = CStr(ManagerGod.Checked)
        End If

        Dim Host = Form1.Settings.ExternalHostName

        If DisallowForeigners.Checked Then
            Form1.PropRegionClass.DisallowForeigners(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.DisallowForeigners(RegionUUID) = ""
        End If

        If DisallowResidents.Checked Then
            Form1.PropRegionClass.DisallowResidents(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.DisallowResidents(RegionUUID) = ""
        End If

        If SkipAutoCheckBox.Checked Then
            Form1.PropRegionClass.SkipAutobackup(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.SkipAutobackup(RegionUUID) = ""
        End If

        If BirdsCheckBox.Checked Then
            Form1.PropRegionClass.Birds(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.Birds(RegionUUID) = ""
        End If

        If TidesCheckbox.Checked Then
            Form1.PropRegionClass.Tides(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.Tides(RegionUUID) = ""
        End If

        If TPCheckBox1.Checked Then
            Form1.PropRegionClass.Teleport(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.Teleport(RegionUUID) = ""
        End If

        If DisableGBCheckBox.Checked Then
            Form1.PropRegionClass.DisableGloebits(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.DisableGloebits(RegionUUID) = ""
        End If

        If SmartStartCheckBox.Checked Then
            Form1.PropRegionClass.SmartStart(RegionUUID) = "True"
        Else
            Form1.PropRegionClass.SmartStart(RegionUUID) = ""
        End If

        Dim ScriptEngine As String = ""
        If XEngineButton.Checked = True Then
            ScriptEngine = "XEngine"
            Form1.PropRegionClass.ScriptEngine(RegionUUID) = "XEngine"
        End If
        If YEngineButton.Checked = True Then
            ScriptEngine = "YEngine"
            Form1.PropRegionClass.ScriptEngine(RegionUUID) = "YEngine"
        End If

        Dim Region = "; * Regions configuration file" &
                            "; * This Is Your World. See Common Settings->[Region Settings]." & vbCrLf &
                            "; Automatically changed by Dreamworld" & vbCrLf &
                            "[" & RegionName.Text & "]" & vbCrLf &
                            "RegionUUID = " & UUID.Text & vbCrLf &
                            "Location = " & CoordX.Text & "," & CoordY.Text & vbCrLf &
                            "InternalAddress = 0.0.0.0" & vbCrLf &
                            "InternalPort = " & RegionPort.Text & vbCrLf &
                            "AllowAlternatePorts = False" & vbCrLf &
                            "ExternalHostName = " & Host & vbCrLf &
                            "SizeX = " & BoxSize & vbCrLf &
                            "SizeY = " & BoxSize & vbCrLf &
                            "Enabled = " & CStr(EnabledCheckBox.Checked) & vbCrLf &
                            "NonPhysicalPrimMax = " & NonphysicalPrimMax.Text & vbCrLf &
                            "PhysicalPrimMax = " & PhysicalPrimMax.Text & vbCrLf &
                            "ClampPrimSize = " & CStr(ClampPrimSize.Checked) & vbCrLf &
                            "MaxAgents = " & MaxAgents.Text & vbCrLf &
                            "MaxPrims = " & MaxPrims.Text & vbCrLf &
                            "RegionType = Estate" & vbCrLf & vbCrLf &
                            ";# Extended region properties from Dreamgrid" & vbCrLf &
                            "MinTimerInterval = " & ScriptTimerTextBox.Text & vbCrLf &
                            "FrameTime = " & FrametimeBox.Text & vbCrLf &
                            "RegionSnapShot = " & Snapshot & vbCrLf &
                            "MapType = " & Map & vbCrLf &
                            "Physics = " & Phys & vbCrLf &
                            "GodDefault = " & Form1.PropRegionClass.GodDefault(RegionUUID) & vbCrLf &
                            "AllowGods = " & Form1.PropRegionClass.AllowGods(RegionUUID) & vbCrLf &
                            "RegionGod = " & Form1.PropRegionClass.RegionGod(RegionUUID) & vbCrLf &
                            "ManagerGod = " & Form1.PropRegionClass.ManagerGod(RegionUUID) & vbCrLf &
                            "Birds = " & Form1.PropRegionClass.Birds(RegionUUID) & vbCrLf &
                            "Tides = " & Form1.PropRegionClass.Tides(RegionUUID) & vbCrLf &
                            "Teleport = " & Form1.PropRegionClass.Teleport(RegionUUID) & vbCrLf &
                            "DisableGloebits = " & Form1.PropRegionClass.DisableGloebits(RegionUUID) & vbCrLf &
                            "DisallowForeigners = " & Form1.PropRegionClass.DisallowForeigners(RegionUUID) & vbCrLf &
                            "DisallowResidents = " & Form1.PropRegionClass.DisallowResidents(RegionUUID) & vbCrLf &
                            "SkipAutoBackup = " & Form1.PropRegionClass.SkipAutobackup(RegionUUID) & vbCrLf &
                            "ScriptEngine = " & Form1.PropRegionClass.ScriptEngine(RegionUUID) & vbCrLf &
                            "SmartStart = " & Form1.PropRegionClass.SmartStart(RegionUUID) & vbCrLf

        'Debug.Print(Region)

        Try
            Using outputFile As New StreamWriter(Filepath, False)
                outputFile.Write(Region)
            End Using
        Catch ex As UnauthorizedAccessException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        Catch ex As ArgumentNullException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        Catch ex As ArgumentException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        Catch ex As DirectoryNotFoundException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        Catch ex As PathTooLongException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        Catch ex As IOException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        Catch ex As Security.SecurityException
            MsgBox(My.Resources.Cannot_save_region_word + ex.Message)
        End Try

        Form1.PropRegionClass.GetAllRegions()
        Form1.PropUpdateView = True

        Oldname1 = RegionName.Text

    End Sub

#End Region

    Private Sub XEngineButton_CheckedChanged(sender As Object, e As EventArgs) Handles XEngineButton.CheckedChanged

        If XEngineButton.Checked Then
            ScriptDefaultButton.Checked = False
            YEngineButton.Checked = False
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub YEngineButton_CheckedChanged(sender As Object, e As EventArgs) Handles YEngineButton.CheckedChanged

        If YEngineButton.Checked Then
            ScriptDefaultButton.Checked = False
            XEngineButton.Checked = False
        End If
        If Initted1 Then Changed1 = True

    End Sub

#End Region

#Region "Radio"

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged

        If Initted1 And RadioButton1.Checked Then
            Changed1 = True
            BoxSize = 256
        End If

    End Sub

    Private Sub RadioButton10_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton9.CheckedChanged

        If Initted1 And RadioButton9.Checked Then
            Changed1 = True
            BoxSize = 256 * 9
        End If

    End Sub

    Private Sub RadioButton11_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton12.CheckedChanged

        If Initted1 And RadioButton12.Checked Then
            Changed1 = True
            BoxSize = 256 * 12
        End If

    End Sub

    Private Sub RadioButton12_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton11.CheckedChanged

        If Initted1 And RadioButton11.Checked Then
            Changed1 = True
            BoxSize = 256 * 11
        End If

    End Sub

    Private Sub RadioButton13_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton16.CheckedChanged

        If Initted1 And RadioButton16.Checked Then
            Changed1 = True
            BoxSize = 256 * 16
        End If

    End Sub

    Private Sub RadioButton14_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton15.CheckedChanged

        If Initted1 And RadioButton15.Checked Then
            Changed1 = True
            BoxSize = 256 * 15
        End If

    End Sub

    Private Sub RadioButton15_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton14.CheckedChanged

        If Initted1 And RadioButton14.Checked Then
            Changed1 = True
            BoxSize = 256 * 14
        End If

    End Sub

    Private Sub RadioButton16_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton13.CheckedChanged

        If Initted1 And RadioButton13.Checked Then
            Changed1 = True
            BoxSize = 256 * 13
        End If

    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged

        If Initted1 And RadioButton2.Checked Then
            Changed1 = True
            BoxSize = 256 * 2
        End If

    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged

        If Initted1 And RadioButton3.Checked Then
            Changed1 = True
            BoxSize = 256 * 3
        End If

    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged

        If Initted1 And RadioButton4.Checked Then
            Changed1 = True
            BoxSize = 256 * 4
        End If

    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton8.CheckedChanged

        If Initted1 And RadioButton8.Checked Then
            Changed1 = True
            BoxSize = 256 * 8
        End If

    End Sub

    Private Sub RadioButton5_CheckedChanged_1(sender As Object, e As EventArgs) Handles ScriptDefaultButton.CheckedChanged

        If ScriptDefaultButton.Checked Then
            XEngineButton.Checked = False
            YEngineButton.Checked = False
        End If

    End Sub

    Private Sub RadioButton6_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton7.CheckedChanged

        If Initted1 And RadioButton7.Checked Then
            Changed1 = True
            BoxSize = 256 * 7
        End If

    End Sub

    Private Sub RadioButton7_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton6.CheckedChanged

        If Initted1 And RadioButton6.Checked Then
            Changed1 = True
            BoxSize = 256 * 6
        End If

    End Sub

    Private Sub RadioButton8_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton5.CheckedChanged

        If Initted1 And RadioButton5.Checked Then
            Changed1 = True
            BoxSize = 256 * 5
        End If

    End Sub

    Private Sub RadioButton9_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton10.CheckedChanged

        If Initted1 And RadioButton10.Checked Then
            Changed1 = True
            BoxSize = 256 * 10
        End If

    End Sub

#End Region

#Region "Gods"

    Private Sub RegionGod_CheckedChanged(sender As Object, e As EventArgs) Handles RegionGod.CheckedChanged

        If RegionGod.Checked Then
            Gods_Use_Default.Checked = False
            Form1.Log(My.Resources.Info, "Region " + Name + " is allowing Region Gods")
        Else
            Form1.Log(My.Resources.Info, "Region " + Name + " is not allowing Region Gods")
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub AllowGods_CheckedChanged(sender As Object, e As EventArgs) Handles AllowGods.CheckedChanged

        If AllowGods.Checked Then
            Gods_Use_Default.Checked = False
            Form1.Log(My.Resources.Info, "Region " + Name + " Is allowing Gods")
        Else
            Form1.Log(My.Resources.Info, "Region " + Name + " is not allowing Region Gods")
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub ManagerGod_CheckedChanged(sender As Object, e As EventArgs) Handles ManagerGod.CheckedChanged

        If ManagerGod.Checked Then
            Gods_Use_Default.Checked = False
            Form1.Log(My.Resources.Info, "Region " + Name + " Is allowing Manager Gods")
        Else
            Form1.Log(My.Resources.Info, "Region " + Name + " Is Not allowing Manager Gods")
        End If
        If Initted1 Then Changed1 = True

    End Sub


    Private Sub Gods_Use_Default_CheckedChanged(sender As Object, e As EventArgs) Handles Gods_Use_Default.CheckedChanged

        If Gods_Use_Default.Checked Then
            Form1.Log(My.Resources.Info, "Region " + Name + " Is set to default for Gods")
            AllowGods.Checked = False
            RegionGod.Checked = False
            ManagerGod.Checked = False
            Gods_Use_Default.Checked = True
        Else
            Form1.Log(My.Resources.Info, "Region " + Name + " is not allowing Region Gods")
        End If

        If Initted1 Then Changed1 = True

    End Sub


#End Region

End Class
