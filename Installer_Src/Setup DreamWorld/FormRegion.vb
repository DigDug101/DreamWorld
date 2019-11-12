#Region "Copyright"

' Copyright 2014 Fred Beckhusen for www.Outworldz.com https://opensource.org/licenses/AGPL

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

    Dim changed As Boolean
    Dim initted As Boolean = False

    ' needed a flag to see if we are initted as the dialogs change on start.
    ' true if we need to save a form
    Dim isNew As Boolean = False

    Dim n As Integer = 0
    Dim oldname As String = ""
    Dim PropRegionClass As RegionMaker
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

    Public Property N1 As Integer
        Get
            Return n
        End Get
        Set(value As Integer)
            n = value
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

    Public Sub Init(Name As String)

        '!!!  remove for production
        If Debugger.IsAttached = False Then
            SmartStartCheckBox.Enabled = False
            Form1.Settings.SmartStart = False
        End If
        Form1.Settings.SmartStart = False

        Me.Focus()

        If Name Is Nothing Then Return
        Name = Name.Trim() ' remove spaces

        PropRegionClass = RegionMaker.Instance()
        PropRegionClass.GetAllRegions()

        If Name.Length = 0 Then
            IsNew1 = True
            RegionName.Text = Name & "Name of Region"
            UUID.Text = Guid.NewGuid().ToString
            SizeX.Text = 256.ToString(Form1.Invarient)
            SizeY.Text = 256.ToString(Form1.Invarient)
            CoordX.Text = (PropRegionClass.LargestX() + 4).ToString(Form1.Invarient)
            CoordY.Text = (PropRegionClass.LargestY() + 0).ToString(Form1.Invarient)
            RegionPort.Text = (PropRegionClass.LargestPort() + 1).ToString(Form1.Invarient)
            EnabledCheckBox.Checked = True
            RadioButton1.Checked = True
            SmartStartCheckBox.Checked = False
            NonphysicalPrimMax.Text = 1024.ToString(Form1.Invarient)
            PhysicalPrimMax.Text = 64.ToString(Form1.Invarient)
            ClampPrimSize.Checked = False
            MaxPrims.Text = 45000.ToString(Form1.Invarient)
            MaxAgents.Text = 100.ToString(Form1.Invarient)
            ScriptTimerTextBox.Text = 0.2.ToString(Form1.Invarient)
            DisableGBCheckBox.Checked = False
            N1 = PropRegionClass.CreateRegion("")
        Else

            IsNew1 = False
            N1 = PropRegionClass.FindRegionByName(Name)
            Oldname1 = PropRegionClass.RegionName(N1) ' backup in case of rename
            EnabledCheckBox.Checked = PropRegionClass.RegionEnabled(N1)
            RegionName.Text = Name
            Me.Text = Name & " Region" ' on screen
            RegionName.Text = PropRegionClass.RegionName(N1) ' on form
            UUID.Text = PropRegionClass.UUID(N1)   ' on screen

            If UUID.Text.Length = 0 Then
                MsgBox("Error: UUID Is zero!")
                Me.Close()
            End If

            NonphysicalPrimMax.Text = CStr(PropRegionClass.NonPhysicalPrimMax(N1))
            PhysicalPrimMax.Text = CStr(PropRegionClass.PhysicalPrimMax(N1))
            ClampPrimSize.Checked = PropRegionClass.ClampPrimSize(N1)
            MaxPrims.Text = CStr(PropRegionClass.MaxPrims(N1))
            MaxAgents.Text = CStr(PropRegionClass.MaxAgents(N1))

            ' Size buttons can be zero
            If PropRegionClass.SizeY(N1) = 0 And PropRegionClass.SizeX(N1) = 0 Then
                RadioButton1.Checked = True
                RadioButton2.Checked = False
                RadioButton3.Checked = False
                RadioButton4.Checked = False
                SizeX.Text = 256.ToString(Form1.Invarient)
                SizeY.Text = 256.ToString(Form1.Invarient)
            ElseIf PropRegionClass.SizeY(N1) = 256 And PropRegionClass.SizeX(N1) = 256 Then
                RadioButton1.Checked = True
                RadioButton2.Checked = False
                RadioButton3.Checked = False
                RadioButton4.Checked = False
                SizeX.Text = 256.ToString(Form1.Invarient)
                SizeY.Text = 256.ToString(Form1.Invarient)
            ElseIf PropRegionClass.SizeY(N1) = 512 And PropRegionClass.SizeX(N1) = 512 Then
                RadioButton1.Checked = False
                RadioButton2.Checked = True
                RadioButton3.Checked = False
                RadioButton4.Checked = False
                SizeX.Text = 512.ToString(Form1.Invarient)
                SizeY.Text = 512.ToString(Form1.Invarient)
            ElseIf PropRegionClass.SizeY(N1) = 768 And PropRegionClass.SizeX(N1) = 768 Then
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                RadioButton3.Checked = True
                RadioButton4.Checked = False
                SizeX.Text = 768.ToString(Form1.Invarient)
                SizeY.Text = 768.ToString(Form1.Invarient)
            ElseIf PropRegionClass.SizeY(N1) = 1024 And PropRegionClass.SizeX(N1) = 1024 Then
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                RadioButton3.Checked = False
                RadioButton4.Checked = True
                SizeX.Text = 1024.ToString(Form1.Invarient)
                SizeY.Text = 1024.ToString(Form1.Invarient)
            Else
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                RadioButton3.Checked = False
                RadioButton4.Checked = False
                SizeX.Text = CStr(PropRegionClass.SizeX(N1))
                SizeY.Text = CStr(PropRegionClass.SizeY(N1))
            End If

            ' global coordinates
            If PropRegionClass.CoordX(N1) <> 0 Then
                CoordX.Text = PropRegionClass.CoordX(N1).ToString(Form1.Invarient)
            End If

            If PropRegionClass.CoordY(N1) <> 0 Then
                CoordY.Text = PropRegionClass.CoordY(N1).ToString(Form1.Invarient)
            End If

            ' and port
            If PropRegionClass.RegionPort(N1) <> 0 Then
                RegionPort.Text = PropRegionClass.RegionPort(N1).ToString(Form1.Invarient)
            End If
        End If

        If PropRegionClass.DisallowResidents(N1) = "True" Then
            DisallowResidents.Checked = True
        End If

        If PropRegionClass.DisallowForeigners(N1) = "True" Then
            DisallowForeigners.Checked = True
        End If

        ScriptTimerTextBox.Text = PropRegionClass.MinTimerInterval(N1).ToString(Form1.Invarient)
        FrametimeBox.Text = PropRegionClass.FrameTime(N1).ToString(Form1.Invarient)

        If PropRegionClass.SkipAutobackup(N1) = "True" Then
            SkipAutoCheckBox.Checked = "True"
        End If

        If Form1.Settings.SmartStart Then
            Select Case PropRegionClass.SmartStart(N1)
                Case ""
                    SmartStartCheckBox.Checked = False
                Case "True"
                    SmartStartCheckBox.Checked = vbTrue
            End Select
        End If

        Select Case PropRegionClass.DisableGloebits(N1)
            Case ""
                DisableGBCheckBox.Checked = False
            Case "False"
                DisableGBCheckBox.Checked = False
            Case "True"
                DisableGBCheckBox.Checked = True
        End Select

        RName1 = Name

        ''''''''''''''''''''''''''''' DREAMGRID REGION LOAD '''''''''''''''''

        If PropRegionClass.MapType(N1).Length = 0 Then
            Maps_Use_Default.Checked = True
        ElseIf PropRegionClass.MapType(N1) = "None" Then
            MapNone.Checked = True
            MapPicture.Image = My.Resources.blankbox
        ElseIf PropRegionClass.MapType(N1) = "Simple" Then
            MapSimple.Checked = True
            MapPicture.Image = My.Resources.Simple
        ElseIf PropRegionClass.MapType(N1) = "Good" Then
            MapGood.Checked = True
            MapPicture.Image = My.Resources.Good
        ElseIf PropRegionClass.MapType(N1) = "Better" Then
            MapBetter.Checked = True
            MapPicture.Image = My.Resources.Better
        ElseIf PropRegionClass.MapType(N1) = "Best" Then
            MapBest.Checked = True
            MapPicture.Image = My.Resources.Best
        End If

        Select Case PropRegionClass.Physics(N1)
            Case "" : Physics_Default.Checked = True
            Case "-1" : Physics_Default.Checked = True
            Case "0" : PhysicsNone.Checked = True
            Case "1" : PhysicsODE.Checked = True
            Case "2" : PhysicsBullet.Checked = True
            Case "3" : PhysicsSeparate.Checked = True
            Case "4" : PhysicsubODE.Checked = True
            Case "5" : Physicsubhybrid.Checked = True
            Case Else : Physics_Default.Checked = True
        End Select

        MaxPrims.Text = PropRegionClass.MaxPrims(N1).ToString(Form1.Invarient)

        Select Case PropRegionClass.AllowGods(N1)
            Case ""
                AllowGods.Checked = False
            Case "False"
                AllowGods.Checked = False
            Case "True"
                AllowGods.Checked = True
                Gods_Use_Default.Checked = False

        End Select

        Select Case PropRegionClass.RegionGod(N1)
            Case ""
                RegionGod.Checked = False
            Case "False"
                RegionGod.Checked = False
            Case "True"
                RegionGod.Checked = True
                Gods_Use_Default.Checked = False
        End Select

        Select Case PropRegionClass.ManagerGod(N1)
            Case ""
                ManagerGod.Checked = False
            Case "False"
                ManagerGod.Checked = False
            Case "True"
                ManagerGod.Checked = True
                Gods_Use_Default.Checked = False
        End Select

        ' if none, turn it off
        If PropRegionClass.AllowGods(N1) = "False" And
             PropRegionClass.RegionGod(N1) = "False" And
            PropRegionClass.ManagerGod(N1) = "False" Then
            Gods_Use_Default.Checked = True
        End If

        Select Case PropRegionClass.RegionSnapShot(N1)
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

        Select Case PropRegionClass.Birds(N1)
            Case ""
                BirdsCheckBox.Checked = False
            Case "False"
                BirdsCheckBox.Checked = False
            Case "True"
                BirdsCheckBox.Checked = True
        End Select

        Select Case PropRegionClass.Tides(N1)
            Case ""
                TidesCheckbox.Checked = False
            Case "False"
                TidesCheckbox.Checked = False
            Case "True"
                TidesCheckbox.Checked = True
        End Select

        Select Case PropRegionClass.Teleport(N1)
            Case ""
                TPCheckBox1.Checked = False
            Case "False"
                TPCheckBox1.Checked = False
            Case "True"
                TPCheckBox1.Checked = True
        End Select

        Select Case PropRegionClass.DisallowForeigners(N1)
            Case ""
                DisallowForeigners.Checked = False
            Case "False"
                DisallowForeigners.Checked = False
            Case "True"
                DisallowForeigners.Checked = True
        End Select

        Select Case PropRegionClass.DisallowResidents(N1)
            Case ""
                DisallowResidents.Checked = False
            Case "False"
                DisallowResidents.Checked = False
            Case "True"
                DisallowResidents.Checked = True
        End Select

        Me.Show() ' time to show the results
        Me.Activate()

        Initted1 = True
        Form1.HelpOnce("Region")

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim message = RegionValidate()
        If Len(message) > 0 Then
            Dim v = MsgBox(message + vbCrLf + "Discard all changes And Exit anyway?", vbYesNo, "Info")
            If v = vbYes Then

                'If RegionList.InstanceExists Then
                'Propregionclass.GetAllRegions()
                RegionList.LoadMyListView()
                'End If

                Me.Close()
            End If
        Else

            Form1.PropViewedSettings = True ' set this so it will force a rescan of the regions on startup

            WriteRegion(N1)
            'Propregionclass.GetAllRegions()
            Form1.CopyOpensimProto(RegionName.Text)

            RegionList.LoadMyListView()

            Changed1 = False
            Me.Close()
        End If

    End Sub

    Private Sub FormRegion_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        If Changed1 Then

            Form1.PropViewedSettings = True
            Dim v = MsgBox("Save changes?", vbYesNo, "Region Save")
            If v = vbYes Then
                Dim message = RegionValidate()
                If Len(message) > 0 Then
                    v = MsgBox(message + vbCrLf + "Discard all changes And Exit anyway?", vbYesNo, "Info")
                    If v = vbYes Then
                        If RegionList.InstanceExists Then
                            'Propregionclass.GetAllRegions()
                            RegionList.LoadMyListView()
                        End If
                        Me.Close()
                    End If
                Else
                    WriteRegion(N1)

                    Form1.CopyOpensimProto(RegionName.Text)
                    Form1.PropUpdateView() = True
                    PropRegionClass.GetAllRegions()
                End If
            End If
        End If

    End Sub

#End Region

#Region "Functions"

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

    Shared Function RegionChosen() As String

        Dim Chooseform As New Choice ' form for choosing a set of regions
        ' Show testDialog as a modal dialog and determine if DialogResult = OK.

        Chooseform.FillGrid("Group")  ' populate the grid with either Group or RegionName

        Dim chosen As String
        Chooseform.ShowDialog()
        Try
            ' Read the chosen sim name
            chosen = Chooseform.DataGridView.CurrentCell.Value.ToString()
            If chosen = "! Add New Name" Then
                chosen = InputBox("Enter the New Dos Box name")
            End If
        Catch ex As Exception
            chosen = ""
        End Try

        Chooseform.Dispose()
        Return chosen

    End Function

    Private Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click

        Dim msg = MsgBox("Are you sure you want To delete this region? ", vbYesNo, "Delete?")
        If msg = vbYes Then
            FileStuff.DeleteFile(Form1.PropOpensimBinPath & "bin\Regions\" + RegionName.Text + "\Region\" + RegionName.Text + ".bak")
            Try
                My.Computer.FileSystem.RenameFile(PropRegionClass.RegionPath(N1), RegionName.Text + ".bak")
                PropRegionClass.GetAllRegions()
            Catch ex As Exception
            End Try
        End If

        Form1.Settings.PortsChanged = True
        Form1.PropUpdateView = True

        Me.Close()

    End Sub

    Private Function IsPowerOf256(x As Integer) As Boolean

        Dim y As Single = Convert.ToSingle(x)
        While y > 0
            y -= 256
        End While
        If y = 0 Then
            Return True
        End If
        Return False

    End Function

    Private Function RegionValidate() As String

        Dim Message As String

        If Len(RegionName.Text) = 0 Then
            Message = "Region name must not be blank"
            Form1.ErrorLog(Message)
            Return Message
        End If

        ' UUID
        Dim result As Guid
        If Not Guid.TryParse(UUID.Text, result) Then
            Message = "Region UUID Is invalid " + UUID.Text
            Form1.ErrorLog(Message)
            Return Message
        End If

        ' global coords
        If Convert.ToInt16(CoordX.Text, Form1.Invarient) = 0 Then
            Message = "Region Coordinate X cannot be zero"
            Form1.ErrorLog(Message)
            Return Message
        ElseIf Convert.ToInt16(CoordX.Text, Form1.Invarient) > 65536 Then
            Message = "Region Coordinate X Is too large"
            Form1.ErrorLog(Message)
            Return Message
        End If

        If Convert.ToInt16(CoordY.Text, Form1.Invarient) = 0 Then
            Message = "Region CoordY cannot be zero"
            Form1.ErrorLog(Message)
            Return Message
        ElseIf Convert.ToInt16(CoordY.Text, Form1.Invarient) > 65536 Then
            Message = "Region CoordY Is too large"
            Form1.ErrorLog(Message)
            Return Message
        End If

        If Convert.ToInt16(RegionPort.Text, Form1.Invarient) = 0 Then
            Message = "Region Port cannot be zero Or undefined"
            Form1.ErrorLog(Message)
            Return Message
        End If

        Dim aresult As Guid
        If Not Guid.TryParse(UUID.Text, aresult) Then
            Message = "Not a valid UUID"
            Form1.ErrorLog(Message)
            Return Message
        End If

        If (NonphysicalPrimMax.Text.Length = 0) Or (CType(NonphysicalPrimMax.Text, Integer) <= 0) Then
            Message = "Not a valid Non-Physical Prim Max Value. Must be greater than 0."
            Form1.ErrorLog(Message)
            Return Message
        End If

        If (PhysicalPrimMax.Text.Length = 0) Or (CType(PhysicalPrimMax.Text, Integer) <= 0) Then
            Message = "Not a valid Physical Prim Max Value. Must be greater than 0."
            Form1.ErrorLog(Message)
            Return Message
        End If

        If (MaxPrims.Text.Length = 0) Or (CType(MaxPrims.Text, Integer) <= 0) Then
            Message = "Not a valid MaxPrims Value. Must be greater than 0."
            Form1.ErrorLog(Message)
            Return Message
        End If

        If (MaxAgents.Text.Length = 0) Or (CType(MaxAgents.Text, Integer) <= 0) Then
            Message = "Not a valid MaxAgents Value. Must be greater than 0."
            Form1.ErrorLog(Message)
            Return Message
        End If

        Return ""
    End Function

    Private Sub WriteRegion(n As Integer)

        ' save the Region File, choose an existing DOS box to put it in, or make a new one
        Form1.Settings.PortsChanged = True

        Dim Filepath = PropRegionClass.RegionPath(n)
        Dim Folderpath = PropRegionClass.FolderPath(n)

        ' rename is possible
        If Oldname1 <> RegionName.Text And Not IsNew1 Then
            Try
                My.Computer.FileSystem.RenameFile(Filepath, RegionName.Text + ".ini")
                Filepath = Folderpath + "\" + RegionName.Text + ".ini"
                PropRegionClass.RegionPath(n) = Filepath
            Catch ex As FileNotFoundException
                Debug.Print(ex.Message)
            End Try
        End If

        ' might be a new region, so give them a choice

        If IsNew1 Then
            Dim NewGroup As String

            NewGroup = RegionChosen()
            If NewGroup.Length = 0 Then
                Form1.Print("Aborted")
                Return
            End If

            If Not Directory.Exists(Filepath) Or Filepath.Length = 0 Then
                Directory.CreateDirectory(Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup + "\Region")
            End If

            PropRegionClass.RegionPath(n) = Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup + "\Region\" + RegionName.Text + ".ini"
            PropRegionClass.FolderPath(n) = Form1.PropOpensimBinPath & "bin\Regions\" + NewGroup

        End If

        Dim Snapshot As String = ""
        If PublishDefault.Checked Then
            Snapshot = ""
        ElseIf NoPublish.Checked Then
            Snapshot = "False"
        ElseIf Publish.Checked Then
            Snapshot = "True"
        End If

        PropRegionClass.RegionSnapShot(n) = Snapshot

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

        PropRegionClass.MapType(n) = Map

        Dim Phys As Integer = 2
        If Physics_Default.Checked Then
            Phys = -1
        ElseIf PhysicsNone.Checked Then
            Phys = 0
        ElseIf PhysicsODE.Checked Then
            Phys = 1
        ElseIf PhysicsBullet.Checked Then
            Phys = 2
        ElseIf PhysicsSeparate.Checked Then
            Phys = 3
        ElseIf PhysicsubODE.Checked Then
            Phys = 4
        ElseIf Physicsubhybrid.Checked Then
            Phys = 5
        End If

        If Physics_Default.Checked Then
            PropRegionClass.Physics(n) = Phys
        End If

        If Gods_Use_Default.Checked Then
            AllowGods.Checked = False
            RegionGod.Checked = False
            ManagerGod.Checked = False
        End If
        If AllowGods.Checked Then
            PropRegionClass.AllowGods(n) = "True"
        Else
            PropRegionClass.AllowGods(n) = ""
        End If
        If RegionGod.Checked Then
            PropRegionClass.RegionGod(n) = "True"
        Else
            PropRegionClass.RegionGod(n) = ""
        End If
        If ManagerGod.Checked Then
            PropRegionClass.ManagerGod(n) = "True"
        Else
            PropRegionClass.ManagerGod(n) = ""
        End If

        Dim Host = Form1.Settings.ExternalHostName

        If DisallowForeigners.Checked Then
            PropRegionClass.DisallowForeigners(n) = "True"
        Else
            PropRegionClass.DisallowForeigners(n) = ""
        End If

        If DisallowResidents.Checked Then
            PropRegionClass.DisallowResidents(n) = "True"
        Else
            PropRegionClass.DisallowResidents(n) = ""
        End If

        If SkipAutoCheckBox.Checked Then
            PropRegionClass.SkipAutobackup(n) = "True"
        Else
            PropRegionClass.SkipAutobackup(n) = ""
        End If

        If BirdsCheckBox.Checked Then
            PropRegionClass.Birds(n) = "True"
        Else
            PropRegionClass.Birds(n) = ""
        End If

        If TidesCheckbox.Checked Then
            PropRegionClass.Tides(n) = "True"
        Else
            PropRegionClass.Tides(n) = ""
        End If

        If TPCheckBox1.Checked Then
            PropRegionClass.Teleport(n) = "True"
        Else
            PropRegionClass.Teleport(n) = ""
        End If

        If DisableGBCheckBox.Checked Then
            PropRegionClass.DisableGloebits(n) = "True"
        Else
            PropRegionClass.DisableGloebits(n) = ""
        End If

        If SmartStartCheckBox.Checked Then
            PropRegionClass.SmartStart(n) = "True"
        Else
            PropRegionClass.SmartStart(n) = ""
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
                            "SizeX = " & SizeX.Text & vbCrLf &
                            "SizeY = " & SizeY.Text & vbCrLf &
                            "Enabled = " & CStr(EnabledCheckBox.Checked) & vbCrLf &
                            "NonPhysicalPrimMax = " & NonphysicalPrimMax.Text & vbCrLf &
                            "PhysicalPrimMax = " & PhysicalPrimMax.Text & vbCrLf &
                            "ClampPrimSize = " & CStr(ClampPrimSize.Checked) & vbCrLf &
                            "MaxAgents = " & MaxAgents.Text & vbCrLf &
                            "MaxPrims = " & MaxPrims.Text & vbCrLf &
                            "RegionType = Estate" & vbCrLf & vbCrLf &
                            ";# Extended region properties from Dreamgrid" & vbCrLf &
                            "MinTimerInterval = " & ScriptTimerTextBox.Text & vbCrLf &
                            "RegionSnapShot = " & Snapshot & vbCrLf &
                            "MapType = " & Map & vbCrLf &
                            "Physics = " & Phys & vbCrLf &
                            "AllowGods = " & PropRegionClass.AllowGods(n) & vbCrLf &
                            "RegionGod = " & PropRegionClass.RegionGod(n) & vbCrLf &
                            "ManagerGod = " & PropRegionClass.ManagerGod(n) & vbCrLf &
                            "Birds = " & PropRegionClass.Birds(n) & vbCrLf &
                            "Tides = " & PropRegionClass.Tides(n) & vbCrLf &
                            "Teleport = " & PropRegionClass.Teleport(n) & vbCrLf &
                            "DisableGloebits = " & PropRegionClass.DisableGloebits(n) & vbCrLf &
                            "DisallowForeigners = " & PropRegionClass.DisallowForeigners(n) & vbCrLf &
                            "DisallowResidents = " & PropRegionClass.DisallowResidents(n) & vbCrLf &
                            "SkipAutoBackup = " & PropRegionClass.SkipAutobackup(n) & vbCrLf &
                            "SmartStart = " & PropRegionClass.SmartStart(n) & vbCrLf

        Debug.Print(Region)

        Try
            Using outputFile As New StreamWriter(PropRegionClass.RegionPath(n), False)
                outputFile.Write(Region)
            End Using
        Catch ex As UnauthorizedAccessException
            MsgBox("Cannot write region:" + ex.Message)
        Catch ex As ArgumentNullException
            MsgBox("Cannot write region:" + ex.Message)
        Catch ex As ArgumentException
            MsgBox("Cannot write region:" + ex.Message)
        Catch ex As DirectoryNotFoundException
            MsgBox("Cannot write region:" + ex.Message)
        Catch ex As PathTooLongException
            MsgBox("Cannot write region:" + ex.Message)
        Catch ex As IOException
            MsgBox("Cannot write region:" + ex.Message)
        Catch ex As Security.SecurityException
            MsgBox("Cannot write region:" + ex.Message)
        End Try

        Form1.PropUpdateView = True

        Oldname1 = RegionName.Text

    End Sub

#End Region

#Region "Changed"

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

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged

        If Not initted Then Return
        If Initted1 And RadioButton1.Checked Then
            SizeX.Text = 256.ToString(Form1.Invarient)
            SizeY.Text = 256.ToString(Form1.Invarient)
            Changed1 = True
        End If

    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged

        If Initted1 And RadioButton2.Checked Then
            SizeX.Text = 512.ToString(Form1.Invarient)
            SizeY.Text = 512.ToString(Form1.Invarient)
            Changed1 = True
        End If

    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged

        If Initted1 And RadioButton3.Checked Then
            SizeX.Text = 768.ToString(Form1.Invarient)
            SizeY.Text = 768.ToString(Form1.Invarient)
            Changed1 = True
        End If

    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged

        If Initted1 And RadioButton4.Checked Then
            SizeX.Text = 1024.ToString(Form1.Invarient)
            SizeY.Text = 1024.ToString(Form1.Invarient)
            Changed1 = True
        End If

    End Sub

    Private Sub RLost(sender As Object, e As EventArgs) Handles RegionName.LostFocus
        RegionName.Text = RegionName.Text.Trim() ' remove spaces
    End Sub

    Private Sub RLostFocus(sender As Object, e As EventArgs) Handles RegionName.TextChanged
        If Len(RegionName.Text) > 0 And Initted1 Then
            If Not FilenameIsOK(RegionName.Text) Then
                MsgBox("Region name can't use special characters such as < > : """" / \ | ? *", vbInformation, "Info")
                Return
            End If
            Changed1 = True
        End If
    End Sub

    Private Sub SizeX_Changed(sender As Object, e As EventArgs) Handles SizeX.LostFocus

        Dim digitsOnly As Regex = New Regex("[^\d]")
        SizeX.Text = digitsOnly.Replace(SizeX.Text, "")

        If Initted1 And SizeX.Text.Length >= 0 Then
            If Not IsPowerOf256(CType(SizeX.Text, Integer)) Then
                MsgBox("Must be a multiple of 256: 256,512,768,1024,1280,1536,1792,2048,2304,2560, ..", vbInformation, "Size X,Y")
            Else
                If CType(SizeX.Text, Double) > 1024 Then
                    RadioButton1.Checked = False
                    RadioButton2.Checked = False
                    RadioButton3.Checked = False
                    RadioButton4.Checked = False
                End If
                SizeY.Text = SizeX.Text
                Changed1 = True
            End If
        End If

    End Sub

    Private Sub UUID_LostFocus(sender As Object, e As EventArgs) Handles UUID.LostFocus

        If UUID.Text <> UUID.Text And Initted1 Then
            Dim resp = MsgBox("Changing the UUID will lose all data in the old sim and create a new, empty sim. Are you sure you wish to change the UUID?", vbYesNo, "Info")
            If resp = vbYes Then
                Changed1 = True
                Dim result As Guid
                If Guid.TryParse(UUID.Text, result) Then
                Else
                    Dim ok = MsgBox("Not a valid UUID. Do you want a new, Random UUID?", vbOKCancel, "Info")
                    If ok = vbOK Then
                        UUID.Text = System.Guid.NewGuid.ToString
                    End If
                End If
            End If
        End If

    End Sub

#End Region

#Region "MoreExtras"

    Private Sub AllowGods_CheckedChanged(sender As Object, e As EventArgs) Handles AllowGods.CheckedChanged

        If AllowGods.Checked Then
            Gods_Use_Default.Checked = False
            Form1.Log("Info", "Region " + Name + " is allowing Gods")
        Else
            If AllowGods.Checked = False And
                RegionGod.Checked = False And
                ManagerGod.Checked = False Then
                Gods_Use_Default.Checked = True
            End If
            Form1.Log("Info", "Region " + Name + " is not allowing Gods")
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub BirdsCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles BirdsCheckBox.CheckedChanged

        If BirdsCheckBox.Checked Then
            Form1.Log("Info", "Region " + Name + " has birds enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim response As MsgBoxResult = MsgBox("This will allow another region to be placed at this spot. Continue?", vbYesNo)
        If response = vbYes Then

            Form1.StartMySQL()
            Form1.StartRobust()

            Dim X = Form1.PropRegionClass.FindRegionByName(RegionName.Text)
            If X > -1 Then

                If Form1.CheckPort(Form1.Settings.PrivateURL, PropRegionClass.GroupPort(X)) Then
                    Form1.SequentialPause()
                    Form1.ConsoleCommand(PropRegionClass.GroupName(X), "q{ENTER}" + vbCrLf)
                End If
                Dim loopctr = 60 ' wait a minute
                While Form1.CheckPort(Form1.Settings.PrivateURL, PropRegionClass.GroupPort(X)) And loopctr > 0
                    Form1.Sleep(1000)
                    loopctr -= 1
                End While

                If loopctr > 0 Then
                    Form1.ConsoleCommand("Robust", "deregister region id " + UUID.Text + "{ENTER}" + vbCrLf)
                    Form1.Print("Region deregistered")
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

    Private Sub DatabaseSetupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatabaseSetupToolStripMenuItem.Click
        Form1.Help("Region")
    End Sub

    Private Sub EnableMaxPrims_text(sender As Object, e As EventArgs) Handles MaxPrims.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        MaxPrims.Text = digitsOnly.Replace(MaxPrims.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub GodHelp_Click(sender As Object, e As EventArgs) Handles GodHelp.Click
        Form1.Help("Permissions")
    End Sub

    Private Sub Gods_Use_Default_CheckedChanged(sender As Object, e As EventArgs) Handles Gods_Use_Default.CheckedChanged

        If Gods_Use_Default.Checked Then
            AllowGods.Checked = False
            RegionGod.Checked = False
            ManagerGod.Checked = False
            Form1.Log("Info", "Region " + Name + " is set to default for Gods")
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub HelpToolStripMenuItem1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ManagerGod_CheckedChanged(sender As Object, e As EventArgs) Handles ManagerGod.CheckedChanged

        If ManagerGod.Checked Then
            Gods_Use_Default.Checked = False
            Form1.Log("Info", "Region " + Name + " is allowing Manager Gods")
        Else
            If AllowGods.Checked = False And
                RegionGod.Checked = False And
                ManagerGod.Checked = False Then
                Gods_Use_Default.Checked = True
            End If
            Form1.Log("Info", "Region " + Name + " is not allowing Manager Gods")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapBest_CheckedChanged(sender As Object, e As EventArgs) Handles MapBest.CheckedChanged

        If MapBest.Checked Then
            Form1.Log("Info", "Region " + Name + " Map is set to Best")
            MapPicture.Image = My.Resources.Best
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapBetter_CheckedChanged(sender As Object, e As EventArgs) Handles MapBetter.CheckedChanged

        If MapBetter.Checked Then
            Form1.Log("Info", "Region " + Name + " Map is set to Better")
            MapPicture.Image = My.Resources.Better
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapGood_CheckedChanged(sender As Object, e As EventArgs) Handles MapGood.CheckedChanged

        If MapGood.Checked Then
            Form1.Log("Info", "Region " + Name + " Map is set to Good")
            MapPicture.Image = My.Resources.Good
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub MapHelp_Click(sender As Object, e As EventArgs) Handles MapHelp.Click
        Form1.Help("Region")
    End Sub

    Private Sub MapNone_CheckedChanged(sender As Object, e As EventArgs) Handles MapNone.CheckedChanged

        If MapNone.Checked Then
            Form1.Log("Info", "Region " + Name + " Map is set to None")
            MapPicture.Image = My.Resources.blankbox
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Maps_Use_Default_changed(sender As Object, e As EventArgs) Handles Maps_Use_Default.CheckedChanged

        If Maps_Use_Default.Checked Then
            Form1.Log("Info", "Region " + Name + " Map is set to Default")
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
            Form1.Log("Info", "Region " + Name + " Map is set to Simple")
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
            Form1.Log("Info", "Region " + Name + " is not set to publish snapshots")
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
            Form1.Log("Info", "Region " + Name + " Physics is set to default")
            PhysicsNone.Checked = False
            PhysicsODE.Checked = False
            PhysicsubODE.Checked = False
            PhysicsBullet.Checked = False
            PhysicsSeparate.Checked = False
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsBullet_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsBullet.CheckedChanged

        If PhysicsBullet.Checked Then
            Form1.Log("Info", "Region " + Name + " Physics Is set to Bullet")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsNone_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsNone.CheckedChanged

        If PhysicsNone.Checked Then
            Form1.Log("Info", "Region " + Name + " Physics Is set to None")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsODE_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsODE.CheckedChanged

        If PhysicsODE.Checked Then
            Form1.Log("Info", "Region " + Name + " Physics Is set to ODE")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsSeparate_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsSeparate.CheckedChanged

        If PhysicsSeparate.Checked Then
            Form1.Log("Info", "Region " + Name + " Physics Is set to Bullet in a Thread")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PhysicsubODE_CheckedChanged(sender As Object, e As EventArgs) Handles PhysicsubODE.CheckedChanged

        If PhysicsubODE.Checked Then
            Form1.Log("Info", "Region " + Name + " Physics Is set to Ubit's ODE")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub Publish_CheckedChanged(sender As Object, e As EventArgs) Handles Publish.CheckedChanged

        If Publish.Checked Then
            Form1.Log("Info", "Region " + Name + " is publishing snapshots")
        Else
            Form1.Log("Info", "Region " + Name + " is not publishing snapshots")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub PublishDefault_CheckedChanged(sender As Object, e As EventArgs) Handles PublishDefault.CheckedChanged

        If PublishDefault.Checked Then
            Form1.Log("Info", "Region " + Name + " is set to default for snapshots")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles Physicsubhybrid.CheckedChanged
        If Physicsubhybrid.Checked Then
            Form1.Log("Info", "Region " + Name + " Physics Is set to Ubit's Hybrid ")
        End If
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub RegionGod_CheckedChanged(sender As Object, e As EventArgs) Handles RegionGod.CheckedChanged

        If RegionGod.Checked Then
            Gods_Use_Default.Checked = False
            Form1.Log("Info", "Region " + Name + " is allowing Region Gods")
        Else
            If AllowGods.Checked = False And
                RegionGod.Checked = False And
                ManagerGod.Checked = False Then
                Gods_Use_Default.Checked = True
            End If
            Form1.Log("Info", "Region " + Name + " is not allowing Region Gods")
        End If

        If Initted1 Then Changed1 = True

    End Sub

    Private Sub ScriptTimerTextBox_TextChanged(sender As Object, e As EventArgs) Handles ScriptTimerTextBox.TextChanged
        Dim digitsOnly As Regex = New Regex("[^\d\.]")
        ScriptTimerTextBox.Text = digitsOnly.Replace(ScriptTimerTextBox.Text, "")
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub SizeX_TextChanged(sender As Object, e As EventArgs) Handles SizeX.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        SizeX.Text = digitsOnly.Replace(SizeX.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub SizeY_TextChanged(sender As Object, e As EventArgs) Handles SizeY.TextChanged

        Dim digitsOnly As Regex = New Regex("[^\d]")
        SizeY.Text = digitsOnly.Replace(SizeY.Text, "")
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub SkipAutoCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles SkipAutoCheckBox.CheckedChanged
        If Initted1 Then Changed1 = True
    End Sub

    Private Sub SmartStartCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles SmartStartCheckBox.CheckedChanged

        If SmartStartCheckBox.Checked Then
            Form1.Log("Info", "Region " + Name + " has Smart Start enabled")

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
            Form1.Log("Info", "Region " + Name + " has tides enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub TPCheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles TPCheckBox1.CheckedChanged

        If TPCheckBox1.Checked Then
            Form1.Log("Info", "Region " + Name + " has Teleport Board enabled")
        End If
        If Initted1 Then Changed1 = True

    End Sub

    Private Sub UUID_TextChanged(sender As Object, e As EventArgs) Handles UUID.TextChanged
        If Initted1 Then Changed1 = True
    End Sub

#End Region

End Class
