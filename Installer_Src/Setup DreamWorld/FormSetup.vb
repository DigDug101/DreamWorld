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
'DEALINGS IN THE SOFTWARE.

#End Region

Imports System.Globalization
Imports System.IO
Imports System.Management
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Ionic.Zip
Imports IWshRuntimeLibrary
Imports MySql.Data.MySqlClient

Public Class Form1

#Region "Version"

    Private _MyVersion As String = "3.26"
    Private _SimVersion As String = "0.9.1.0 Server Release Notes #defa235859889dbd"

#End Region

#Region "Declarations"

    Private WithEvents ApacheProcess As New Process()

    Private WithEvents IcecastProcess As New Process()

    Private WithEvents ProcessMySql As Process = New Process()

    Private WithEvents RobustProcess As New Process()

    Private WithEvents UpdateProcess As New Process()

    Private _Aborting As Boolean = False

    Private _ApacheCrashCounter As Integer = 0

    Private _ApacheExited As Integer = 0

    Private _ApacheProcessID As Integer = 0

    Private _ApacheUninstalling As Boolean = False

    Private _ContentAvailable As Boolean = False

    Private _CPUMAX As Single = 75

    Private _CurSlashDir As String

    Private _debugOn As Boolean = False

    Private _DNSSTimer As Integer = 0

    Private _Domain As String = "http://www.outworldz.com"

    Private _ExitHandlerIsBusy As Boolean = False

    Private _exitList As New ArrayList()

    Private _ForceMerge As Boolean = False

    Private _ForceParcel As Boolean = False

    Private _ForceTerrain As Boolean = False

    Private _gUseIcons As Boolean = False

    Private _IcecastCrashCounter As Integer = 0

    Private _IceCastExited As Integer = 0

    Private _IcecastProcID As Integer

    Private _Initted As Boolean = False

    Private _invarient As CultureInfo = New CultureInfo("")

    Private _IPv4Address As String

    Private _IsRunning As Boolean = False

    Private _KillSource As Boolean = False

    Private _Language As New Culture

    Private _MaxPortUsed As Integer = 0

    Private _myFolder As String

    Private _mySetting As New MySettings

    Private _MysqlCrashCounter As Integer = 0

    Private _MysqlExited As Integer = 0

    Private _myUPnpMap As UPnp

    Private _OldSimVersion As String

    Private _OpensimBinPath As String

    Private _PortsChanged As Boolean = True

    Private _regionClass As RegionMaker

    Private _RegionCrashCounter As Integer = 0

    Private _regionForm As RegionList

    Private _regionHandles As New Dictionary(Of Integer, String)

    Private _RestartApache As Integer = 0

    Private _RestartIceCast As Integer = 0

    Private _RestartMysql As Integer = 0

    Private _RestartNow As Boolean = False

    Private _RestartRobust As Boolean

    Private _RobustCrashCounter As Integer = 0

    Private _RobustExited As Boolean = False

    Private _RobustProcID As Integer

    Private _SecureDomain As String = "https://outworldz.com"

    ' https, which breaks stuff
    Private _SelectedBox As String = ""

    Private _StopMysql As Boolean = True

    Private _UpdateView As Boolean = True

    ' "" = Invariant Culture
    Private _UserName As String = ""

    Private _viewedSettings As Boolean = False

    Private Adv As AdvancedForm

    Private cpu As New PerformanceCounter

    Private MyCPUCollection(181) As Double

    ' Graph
    Private MyRAMCollection(181) As Double

    Private speed As Single

    Private speed1 As Single

    Private speed2 As Single

    Private speed3 As Single

    Private Update_version As String = Nothing

    Private ws As NetServer

    Public Event ApacheExited As EventHandler

    Public Event Exited As EventHandler

    Public Event RobustExited As EventHandler

    Public Enum SHOWWINDOWENUM As Integer
        SWHIDE = 0
        SWSHOWNORMAL = 1
        SWNORMAL = 1
        SWSHOWMINIMIZED = 2
        SWSHOWMAXIMIZED = 3
        SWMAXIMIZE = 3
        SWSHOWNOACTIVATE = 4
        SWSHOW = 5
        SWMINIMIZE = 6
        SWSHOWMINNOACTIVE = 7
        SWSHOWNA = 8
        SWRESTORE = 9
        SWSHOWDEFAULT = 10
        SWFORCEMINIMIZE = 11
        SWMAX = 11
    End Enum

#End Region

#Region "ScreenSize"

    Private Handler As New EventHandler(AddressOf Resize_page)
    Private newScreenPosition As ScreenPos
    Private ScreenPosition As ScreenPos

    Private Sub Form1_Layout(sender As Object, e As LayoutEventArgs) Handles Me.Layout

        Dim Y = Me.Height - 120
        TextBox1.Size = New Size(TextBox1.Size.Width, Y)

    End Sub

    'The following detects  the location of the form in screen coordinates
    Private Sub Resize_page(ByVal sender As Object, ByVal e As EventArgs)

        ScreenPosition.SaveXY(Me.Left, Me.Top)
        ScreenPosition.SaveHW(Me.Height, Me.Width)
    End Sub

    Private Sub SetScreen()

        ScreenPosition = New ScreenPos("Form1")
        AddHandler ResizeEnd, Handler
        Dim xy As List(Of Integer) = ScreenPosition.GetXY()
        Left = xy.Item(0)
        Top = xy.Item(1)

        Dim hw As List(Of Integer) = ScreenPosition.GetHW()

        If hw.Item(0) = 0 Then
            Me.Height = 180
        Else
            Me.Height = hw.Item(0)
        End If

        If hw.Item(1) = 0 Then
            Me.Width = 320
        Else
            Me.Width = hw.Item(1)
        End If

        ScreenPosition.SaveHW(Me.Height, Me.Width)

    End Sub

#End Region

#Region "Properties"

    Public Property Invarient As CultureInfo
        Get
            Return _invarient
        End Get
        Set(value As CultureInfo)
            _invarient = value
        End Set
    End Property

    Public Property PropAborting As Boolean
        Get
            Return _Aborting
        End Get
        Set(value As Boolean)
            _Aborting = value
        End Set
    End Property

    Public Property PropApacheExited() As Boolean
        Get
            Return _ApacheExited
        End Get
        Set(ByVal Value As Boolean)
            _ApacheExited = Value
        End Set
    End Property

    Public Property PropApacheUninstalling() As Boolean
        Get
            Return _ApacheUninstalling
        End Get
        Set(ByVal Value As Boolean)
            _ApacheUninstalling = Value
        End Set
    End Property

    Public Property PropContentAvailable() As Boolean
        Get
            Return _ContentAvailable
        End Get
        Set(ByVal Value As Boolean)
            _ContentAvailable = Value
        End Set
    End Property

    Public Property PropCPUMAX As Single
        Get
            Return _CPUMAX
        End Get
        Set(value As Single)
            _CPUMAX = value
        End Set
    End Property

    Public Property PropCurSlashDir As String
        Get
            Return _CurSlashDir
        End Get
        Set(value As String)
            _CurSlashDir = value
        End Set
    End Property

    Public Property PropDebug As Boolean
        Get
            Return _debugOn
        End Get
        Set(value As Boolean)
            _debugOn = value
        End Set
    End Property

    Public Property PropDNSSTimer() As Integer
        Get
            Return _DNSSTimer
        End Get
        Set(ByVal Value As Integer)
            _DNSSTimer = Value
        End Set
    End Property

    Public Property PropDomain As String
        Get
            Return _Domain
        End Get
        Set(value As String)
            _Domain = value
        End Set
    End Property

    Public Property PropExitHandlerIsBusy() As Boolean
        Get
            Return _ExitHandlerIsBusy
        End Get
        Set(ByVal Value As Boolean)
            _ExitHandlerIsBusy = Value
        End Set
    End Property

    Public ReadOnly Property PropExitList As ArrayList
        Get
            Return _exitList
        End Get
    End Property

    Public Property PropForceMerge As Boolean
        Get
            Return _ForceMerge
        End Get
        Set(value As Boolean)
            _ForceMerge = value
        End Set
    End Property

    Public Property PropForceParcel As Boolean
        Get
            Return _ForceParcel
        End Get
        Set(value As Boolean)
            _ForceParcel = value
        End Set
    End Property

    Public Property PropForceTerrain As Boolean
        Get
            Return _ForceTerrain
        End Get
        Set(value As Boolean)
            _ForceTerrain = value
        End Set
    End Property

    Public Property PropgApacheProcessID As Integer
        Get
            Return _ApacheProcessID
        End Get
        Set(value As Integer)
            _ApacheProcessID = value
        End Set
    End Property

    Public Property PropIceCastExited() As Boolean
        Get
            Return _IceCastExited
        End Get
        Set(ByVal Value As Boolean)
            _IceCastExited = Value
        End Set
    End Property

    Public Property PropIcecastProcID As Integer
        Get
            Return _IcecastProcID
        End Get
        Set(value As Integer)
            _IcecastProcID = value
        End Set
    End Property

    Public Property PropInitted() As Boolean
        Get
            Return _Initted
        End Get
        Set(ByVal Value As Boolean)
            _Initted = Value
        End Set
    End Property

    Public Property PropIPv4Address() As String
        Get
            Return _IPv4Address
        End Get
        Set(ByVal Value As String)
            _IPv4Address = Value
        End Set
    End Property

    Public Property PropKillSource As Boolean
        Get
            Return _KillSource
        End Get
        Set(value As Boolean)
            _KillSource = value
        End Set
    End Property

    Public Property PropMaxPortUsed As Integer
        Get
            Return _MaxPortUsed
        End Get
        Set(value As Integer)
            _MaxPortUsed = value
        End Set
    End Property

    Public Property PropMyFolder As String
        Get
            Return _myFolder
        End Get
        Set(value As String)
            _myFolder = value
        End Set
    End Property

    Public Property PropMysqlExited() As Boolean
        Get
            Return _MysqlExited
        End Get
        Set(ByVal Value As Boolean)
            _MysqlExited = Value
        End Set
    End Property

    Public Property PropMyUPnpMap As UPnp
        Get
            Return _myUPnpMap
        End Get
        Set(value As UPnp)
            _myUPnpMap = value
        End Set
    End Property

    Public Property PropMyVersion As String
        Get
            Return _MyVersion
        End Get
        Set(value As String)
            _MyVersion = value
        End Set
    End Property

    Public Property PropOpensimBinPath As String
        Get
            Return _OpensimBinPath
        End Get
        Set(value As String)
            _OpensimBinPath = value
        End Set
    End Property

    Public Property PropOpensimIsRunning() As Boolean
        Get
            Return _IsRunning
        End Get
        Set(ByVal Value As Boolean)
            _IsRunning = Value
        End Set
    End Property

    Public Property PropRegionClass As RegionMaker
        Get
            Return _regionClass
        End Get
        Set(value As RegionMaker)
            _regionClass = value
        End Set
    End Property

    Public Property PropRegionForm As RegionList
        Get
            Return _regionForm
        End Get
        Set(value As RegionList)
            _regionForm = value
        End Set
    End Property

    Public ReadOnly Property PropRegionHandles As Dictionary(Of Integer, String)
        Get
            Return _regionHandles
        End Get
    End Property

    Public Property PropRestartApache() As Boolean
        Get
            Return _RestartApache
        End Get
        Set(ByVal Value As Boolean)
            _RestartApache = Value
        End Set
    End Property

    Public Property PropRestartMySql() As Boolean
        Get
            Return _RestartMysql
        End Get
        Set(ByVal Value As Boolean)
            _RestartMysql = Value
        End Set
    End Property

    Public Property PropRestartNow As Boolean
        Get
            Return _RestartNow
        End Get
        Set(value As Boolean)
            _RestartNow = value
        End Set
    End Property

    Public Property PropRestartRobust As Boolean
        Get
            Return _RestartRobust
        End Get
        Set(value As Boolean)
            _RestartRobust = value
        End Set
    End Property

    Public Property PropRobustExited() As Boolean
        Get
            Return _RobustExited
        End Get
        Set(ByVal Value As Boolean)
            _RobustExited = Value
        End Set
    End Property

    Public Property PropRobustProcID As Integer
        Get
            Return _RobustProcID
        End Get
        Set(value As Integer)
            _RobustProcID = value
        End Set
    End Property

    Public Property PropSelectedBox As String
        Get
            Return _SelectedBox
        End Get
        Set(value As String)
            _SelectedBox = value
        End Set
    End Property

    Public Property PropSimVersion As String
        Get
            Return _SimVersion
        End Get
        Set(value As String)
            _SimVersion = value
        End Set
    End Property

    Public Property PropStopMysql As Boolean
        Get
            Return _StopMysql
        End Get
        Set(value As Boolean)
            _StopMysql = value
        End Set
    End Property

    Public Property PropUpdateView As Boolean
        Get
            Return _UpdateView
        End Get
        Set(value As Boolean)
            _UpdateView = value
        End Set
    End Property

    Public Property PropUseIcons As Boolean
        Get
            Return _gUseIcons
        End Get
        Set(value As Boolean)
            _gUseIcons = value
        End Set
    End Property

    Public Property PropUserName As String
        Get
            Return _UserName
        End Get
        Set(value As String)
            _UserName = value
        End Set
    End Property

    Public Property PropViewedSettings As Boolean
        Get
            Return _viewedSettings
        End Get
        Set(value As Boolean)
            Diagnostics.Debug.Print("ViewedSettings =" & value)
            _viewedSettings = value
        End Set
    End Property

    Public Property PropWebServer As NetServer
        Get
            Return ws
        End Get
        Set(value As NetServer)
            ws = value
        End Set
    End Property

    Public Property SecureDomain As String
        Get
            Return _SecureDomain
        End Get
        Set(value As String)
            _SecureDomain = value
        End Set
    End Property

    Public Property Settings As MySettings
        Get
            Return _mySetting
        End Get
        Set(value As MySettings)
            _mySetting = value
        End Set
    End Property

    Public Property SimVersion As String
        Get
            Return _SimVersion
        End Get
        Set(value As String)
            _SimVersion = value
        End Set
    End Property

#End Region

#Region "StartStop"

    ''' <summary>
    ''' Startup() Starts opensimulator system Called by Start Button or by AutoStart
    ''' </summary>
    Public Sub Startup(Optional SkipSmartStart As Boolean = False)

        Print("Version = " & PropMyVersion)

        With cpu
            .CategoryName = "Processor"
            .CounterName = "% Processor Time"
            .InstanceName = "_Total"
        End With

        Dim DefaultName As String = ""
        Print(My.Resources.Starting)

        Dim N = PropRegionClass.FindRegionByName(Settings.WelcomeRegion)
        If N = -1 Then
            Dim result = MsgBox("The default 'Welcome' region " & DefaultName & " is not found in the system. Continue?", vbYesNo)
            If result = vbNo Then
                Print("Stopped.")
                Return
            End If
        End If
        If PropRegionClass.RegionEnabled(N) = False Then
            Dim result = MsgBox("The default 'Welcome' region " & DefaultName & " is not enabled. Continue?", vbYesNo)
            If result = vbNo Then
                Print(My.Resources.Stopped)
                Return
            End If
        End If

        PropOpensimIsRunning() = True

        PropExitHandlerIsBusy = False
        PropAborting = False  ' suppress exit warning messages
        ProgressBar1.Value = 0
        ProgressBar1.Visible = True
        ToolBar(False)
        Buttons(BusyButton)

        GridNames.SetServerNames()

        If Settings.PortsChanged Then
            Print("Setup Ports")
            RegionMaker.UpdateAllRegionPorts() ' must be done before we are running
            Print("Setup Firewall")
            Firewall.SetFirewall()   ' must be after UpdateAllRegionPorts
        End If

        ' clear region error handlers
        PropRegionHandles.Clear()

        If Settings.AutoBackup Then
            ' add 30 minutes to allow time to auto backup and then restart
            Dim BTime As Integer = CInt(Settings.AutobackupInterval)
            If Settings.AutoRestartInterval > 0 And Settings.AutoRestartInterval < BTime Then
                Settings.AutoRestartInterval = BTime + 30
                Print("Upping AutoRestart Time to " & CStr(BTime) + " + 30 Minutes.")
            End If
        End If

        PropOpensimIsRunning() = True

        If PropViewedSettings Then

            If SetPublicIP() Then
                OpenPorts()
            End If

            Print("Read Region INI files")
            PropRegionClass.GetAllRegions()
            If Not SetIniData() Then Return   ' set up the INI files
        End If

        If Not StartMySQL() Then
            ProgressBar1.Value = 0
            ProgressBar1.Visible = True
            ToolBar(False)
            Buttons(StartButton)
            Print("Stopped")
            Return
        End If

        SetupSearch()

        StartApache()

        ' old files to clean up

        If Settings.BirdsModuleStartup Then
            Try
                My.Computer.FileSystem.CopyFile(PropOpensimBinPath & "\bin\OpenSimBirds.Module.bak", PropOpensimBinPath & "\bin\OpenSimBirds.Module.dll")
            Catch ex As ArgumentNullException
            Catch ex As ArgumentException
            Catch ex As FileNotFoundException
            Catch ex As PathTooLongException
            Catch ex As IOException
            Catch ex As NotSupportedException
            Catch ex As UnauthorizedAccessException
            Catch ex As System.Security.SecurityException
            End Try
        Else
            FileStuff.DeleteFile(PropOpensimBinPath & "\bin\OpenSimBirds.Module.dll")
        End If

        If Not StartRobust() Then
            Return
        End If

        If Not Settings.RunOnce And Settings.ServerType = "Robust" Then
            ConsoleCommand("Robust", "create user{ENTER}")
            MsgBox("Please type the Grid Owner's avatar name into the Robust window. Press <enter> for UUID and Model name. Then press this OK button", vbInformation, "Info")

            If Settings.ConsoleShow = False Then
                ShowDOSWindow(GetHwnd("Robust"), SHOWWINDOWENUM.SWMINIMIZE)
            End If

            Settings.RunOnce = True
            Settings.SaveSettings()
        End If

        Timer1.Interval = 1000
        Timer1.Start() 'Timer starts functioning

        StartIcecast()

        ' Launch the rockets
        Print("Start Regions")
        If Not StartOpensimulator(SkipSmartStart) Then
            Return
        End If

        ' show the IAR and OAR menu when we are up
        If PropContentAvailable Then
            IslandToolStripMenuItem.Visible = True
            ClothingInventoryToolStripMenuItem.Visible = True
        End If

        Buttons(StopButton)
        ProgressBar1.Value = 100
        Print("Grid address is" & vbCrLf & "http://" & Settings.BaseHostName & ":" & Settings.HttpPort)

        ' done with boot up
        ProgressBar1.Visible = False
        ToolBar(True)

    End Sub

    Private Sub Form1_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Closed
        ReallyQuit()
    End Sub

    ''' <summary>
    ''' Form Load is main() for all DreamGrid
    ''' </summary>
    ''' <param name="sender">Unused</param>
    ''' <param name="e">Unused</param>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Me.Hide()

        ' show box styled nicely.
        Application.EnableVisualStyles()
        Buttons(BusyButton)
        ProgressBar1.Visible = True
        ToolBar(False)

        ProgressBar1.Minimum = 0
        ProgressBar1.Maximum = 100
        ProgressBar1.Value = 0
        TextBox1.BackColor = Me.BackColor
        ' init the scrolling text box
        TextBox1.SelectionStart = 0
        TextBox1.ScrollToCaret()
        TextBox1.SelectionStart = TextBox1.Text.Length
        TextBox1.ScrollToCaret()
        Me.Show()

        ' setup a debug path
        PropMyFolder = My.Application.Info.DirectoryPath

        If Debugger.IsAttached Then
            ' for debugging when compiling
            PropDebug = True
            PropMyFolder = PropMyFolder.Replace("\Installer_Src\Setup DreamWorld\bin\Debug", "")
            PropMyFolder = PropMyFolder.Replace("\Installer_Src\Setup DreamWorld\bin\Release", "")
            ' for testing, as the compiler buries itself in ../../../debug
        End If

        PropCurSlashDir = PropMyFolder.Replace("\", "/")    ' because MySQL uses Unix like slashes, that's why
        PropOpensimBinPath() = PropMyFolder & "\OutworldzFiles\Opensim\"

        SetScreen()     ' move Form to fit screen from SetXY.ini

        If Not System.IO.File.Exists(PropMyFolder & "\OutworldzFiles\Settings.ini") Then
            Print("Installing Desktop icon clicky thingy")
            Create_ShortCut(PropMyFolder & "\Start.exe")
            BumpProgress10()
        End If

        PropViewedSettings = True

        Settings.Init(PropMyFolder)
        Settings.Myfolder = PropMyFolder
        Settings.OpensimBinPath = PropOpensimBinPath

        If Me.Width > 320 Then
            PictureBox1.Image = My.Resources.Arrow2Left
        Else
            PictureBox1.Image = My.Resources.Arrow2Right
        End If

        ' Save a random machine ID - we don't want any data to be sent that's personal or
        ' identifiable, but it needs to be unique
        Randomize()
        If Settings.MachineID().Length = 0 Then Settings.MachineID() = RandomNumber.Random  ' a random machine ID may be generated.  Happens only once

        ' WebUI
        ViewWebUI.Visible = Settings.WifiEnabled

        Me.Text += " V" & PropMyVersion

        PropOpensimIsRunning() = False ' true when opensim is running

        ProgressBar1.Value = 0

        Me.Show()

        Print("Getting regions")

        PropRegionClass = RegionMaker.Instance()
        Adv = New AdvancedForm

        PropInitted = True

        ClearLogFiles() ' clear log files

        If Not IO.File.Exists(PropMyFolder & "\BareTail.udm") Then
            IO.File.Copy(PropMyFolder & "\BareTail.udm.bak", PropMyFolder & "\BareTail.udm")
        End If

        GridNames.SetServerNames()

        CheckDefaultPorts()
        PropMyUPnpMap = New UPnp()
        If SetPublicIP() Then
            OpenPorts()
        End If

        SetQuickEditOff()

        SetLoopback()

        'mnuShow shows the DOS box for Opensimulator
        mnuShow.Checked = Settings.ConsoleShow
        mnuHide.Checked = Not Settings.ConsoleShow

        If Not SetIniData() Then Return

        CheckForUpdates()

        'must start after region Class Is instantiated
        PropWebServer = NetServer.GetWebServer

        Print("Starting HTTP Server ")
        PropWebServer.StartServer(PropMyFolder, Settings)

        CheckDiagPort()

        If Settings.PortsChanged Then
            Print("Setup Ports")
            RegionMaker.UpdateAllRegionPorts() ' must be after SetIniData
            Print("Setup Firewall")
            Firewall.SetFirewall()   ' must be after UpdateAllRegionPorts
        End If

        mnuSettings.Visible = True
        SetIAROARContent() ' load IAR and OAR web content
        LoadLocalIAROAR() ' load IAR and OAR local content

        If Settings.Password = "secret" Then
            BumpProgress10()
            Dim Password = New PassGen
            Settings.Password = Password.GeneratePass()
        End If

        Print("Setup Graphs")
        ' Graph fill
        Dim i = 0
        While i < 180
            MyCPUCollection(i) = 0
            i += 1
        End While

        Dim msChart = ChartWrapper1.TheChart
        msChart.ChartAreas(0).AxisX.Maximum = 180
        msChart.ChartAreas(0).AxisX.Minimum = 0
        msChart.ChartAreas(0).AxisY.Maximum = 100
        msChart.ChartAreas(0).AxisY.Minimum = 0
        msChart.ChartAreas(0).AxisY.LabelStyle.Enabled = True
        msChart.ChartAreas(0).AxisX.LabelStyle.Enabled = False
        ChartWrapper1.AddMarkers = True
        ChartWrapper1.MarkerFreq = 60

        i = 0
        While i < 180
            MyRAMCollection(i) = 0
            i += 1
        End While

        msChart = ChartWrapper2.TheChart
        msChart.ChartAreas(0).AxisX.Maximum = 180
        msChart.ChartAreas(0).AxisX.Minimum = 0
        msChart.ChartAreas(0).AxisY.Maximum = 100
        msChart.ChartAreas(0).AxisY.Minimum = 0
        msChart.ChartAreas(0).AxisX.LabelStyle.Enabled = False
        msChart.ChartAreas(0).AxisY.LabelStyle.Enabled = True
        ChartWrapper2.AddMarkers = True
        ChartWrapper2.MarkerFreq = 60

        If Settings.RegionListVisible Then
            ShowRegionform()
        End If
        Sleep(2000)
        Print("Check MySql")
        Dim isMySqlRunning = CheckPort("127.0.0.1", Settings.MySqlRobustDBPort)
        If isMySqlRunning Then PropStopMysql() = False

        Buttons(StartButton)
        ProgressBar1.Value = 100

        If Settings.Autostart Then
            Print("Auto Startup")
            Startup()
        Else
            Settings.SaveSettings()
            Print("Ready to Launch!" & vbCrLf & "Click 'Start' to begin." & vbCrLf)
        End If

        HelpOnce("License") ' license on bottom
        HelpOnce("Startup")

        ProgressBar1.Value = 100

    End Sub

    Private Sub JustQuitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JustQuitToolStripMenuItem.Click
        ProgressBar1.Value = 0
        Print("Zzzz...")
        End
    End Sub

#End Region

#Region "Kill"

    Public Function KillAll() As Boolean

#Disable Warning CA1820 ' Test for empty strings using string length
        If AvatarLabel.Text <> "" Then
#Enable Warning CA1820 ' Test for empty strings using string length
            If CType(AvatarLabel.Text, Integer) > 0 Then
                Dim response = MsgBox("There are " & AvatarLabel.Text & " avatars in world! Do you really wish to quit?", vbYesNo)
                If response = vbNo Then Return False
            End If
        End If

        AvatarLabel.Text = ""
        PropAborting = True
        ProgressBar1.Value = 100
        ProgressBar1.Visible = True
        ToolBar(False)
        ' close everything as gracefully as possible.

        StopIcecast()
        StopApache()

        Dim n As Integer = PropRegionClass.RegionCount()

        Dim TotalRunningRegions As Integer

        For Each Regionnumber As Integer In PropRegionClass.RegionNumbers
            If PropRegionClass.IsBooted(Regionnumber) Then
                TotalRunningRegions += 1
            End If
        Next
        Log("Info", "Total Enabled Regions=" & CStr(TotalRunningRegions))

        For Each X As Integer In PropRegionClass.RegionNumbers
            If PropOpensimIsRunning() And PropRegionClass.RegionEnabled(X) And
            Not (PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.RecyclingDown _
            Or PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.ShuttingDown _
            Or PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Stopped) Then
                Print(PropRegionClass.RegionName(X) & " is stopping")
                SequentialPause()
                ConsoleCommand(PropRegionClass.GroupName(X), "q{ENTER}" & vbCrLf)
                PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.ShuttingDown
                PropRegionClass.Timer(X) = RegionMaker.REGIONTIMER.Stopped
                PropUpdateView = True ' make form refresh
            End If
        Next

        Dim counter = 600 ' 10 minutes to quit all regions
        Dim last As Integer = PropRegionClass.RegionNumbers.Count

        ' only wait if the port 8001 is working
        If PropUseIcons Then
            If PropOpensimIsRunning Then Print("Waiting for all regions to exit")

            While (counter > 0 And PropOpensimIsRunning())
                Sleep(1000)
                ' decrement progress bar according to the ratio of what we had / what we have now
                counter -= 1
                Dim CountisRunning As Integer = 0

                For Each X As Integer In PropRegionClass.RegionNumbers
                    If (Not PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Stopped) And PropRegionClass.RegionEnabled(X) Then
                        If CheckPort(Settings.PrivateURL, PropRegionClass.GroupPort(X)) Then
                            CountisRunning += 1
                        Else
                            StopGroup(PropRegionClass.GroupName(X))
                        End If
                        'PropUpdateView = True ' make form refresh
                    End If

                    If CountisRunning = 0 Then Exit For
                Next

                If CountisRunning = 1 And last > 1 Then
                    Print("1 region is still running")
                    last = 1
                    PropUpdateView = True ' make form refresh
                Else
                    If CountisRunning <> last Then
                        Print(CStr(CountisRunning) & " regions are still running")
                        last = CountisRunning
                        PropUpdateView = True ' make form refresh
                    End If
                End If

                If CountisRunning = 0 Then
                    counter = 0
                    ProgressBar1.Value = 0
                End If

                Dim v As Double = CountisRunning / TotalRunningRegions * 100
                If v >= 0 And v <= 100 Then
                    ProgressBar1.Value = CType(v, Integer)
                End If
            End While
            PropUpdateView = True ' make form refresh
        End If

        PropRegionHandles.Clear()
        StopAllRegions()
        StopRobust()

        ' cannot load OAR or IAR, either
        IslandToolStripMenuItem.Visible = False
        ClothingInventoryToolStripMenuItem.Visible = False
        Timer1.Stop()
        PropOpensimIsRunning() = False

        ProgressBar1.Value = 0
        ProgressBar1.Visible = False
        ToolBar(False)
        Return True

    End Function

    Public Sub StopRobust()

        ConsoleCommand("Robust", "q{ENTER}" & vbCrLf)
        Dim ctr As Integer = 0
        ' wait 60 seconds for robust to quit
        While IsRobustRunning() And ctr < 60
            Sleep(1000)
            ctr += 1
            Application.DoEvents()
        End While
        ' trust, but verify
        If Not IsRobustRunning() Then
            RobustPictureBox.Image = My.Resources.nav_plain_blue
            ToolTip1.SetToolTip(RobustPictureBox, "Stopped")
        End If

    End Sub

    Private Sub KillFiles(AL As List(Of String))

        For Each filename As String In AL
            FileStuff.DeleteFile(PropMyFolder & filename)
        Next

    End Sub

    Private Sub KillFolder(AL As List(Of String))

        For Each folder As String In AL
            Try
                System.IO.Directory.Delete(PropMyFolder & folder, True)
            Catch ex As Exception
                Diagnostics.Debug.Print(ex.Message)
            End Try
        Next

    End Sub

    Private Sub KillOldFiles()

        Dim files As New List(Of String) From {
        "\Shoutcast", ' deprecated
        "\Icecast",   ' moved to Outworldzfiles
        "\Outworldzfiles\Opensim\bin\addins"
        }

        If PropKillSource Then
            files.Add("Outworldzfiles\Opensim\.nant")
            files.Add("Outworldzfiles\Opensim\addon-modules")
            files.Add("Outworldzfiles\Opensim\doc")
            files.Add("Outworldzfiles\Opensim\Opensim")
            files.Add("Outworldzfiles\Opensim\Prebuild")
            files.Add("Outworldzfiles\Opensim\share")
            files.Add("Outworldzfiles\Opensim\Thirdparty")

        End If

        KillFolder(files)   ' wipe these folders out
        files.Clear() ' now do a list of files to clean up

        ' necessary to kill these off as it is a badly behaved
        files.Add("\Outworldzfiles\Opensim\bin\OpenSim.Additional.AutoRestart.dll")
        files.Add("\Outworldzfiles\Opensim\bin\OpenSim.Additional.AutoRestart.pdb")
        files.Add("\Outworldzfiles\Opensim\bin\config-include\Birds.ini") ' no need for birds yet
        files.Add("SET_externalIP-Log.txt")

        ' crapload of old DLLS have to be eliminated
        CleanDLLs() ' drop old opensim dll's

        If PropKillSource Then
            files.Add("\Outworldzfiles\Opensim\BUILDING.md")
            files.Add("\Outworldzfiles\Opensim\compile.bat")
            files.Add("\Outworldzfiles\Opensim\Makefile")
            files.Add("\Outworldzfiles\Opensim\nant-color")
            files.Add("\Outworldzfiles\Opensim\OpenSim.build")
            files.Add("\Outworldzfiles\Opensim\OpenSim.sln")
            files.Add("\Outworldzfiles\Opensim\prebuild.xml")
            files.Add("\Outworldzfiles\Opensim\runprebuild.bat")
            files.Add("\Outworldzfiles\Opensim\runprebuild.sh")
            files.Add("\Outworldzfiles\Opensim\TESTING.txt")
        End If

        KillFiles(files)   ' wipe these files out

    End Sub

    Private Sub MnuExit_Click(sender As System.Object, e As EventArgs) Handles mnuExit.Click
        ReallyQuit()
    End Sub

    Private Sub ReallyQuit()

        If Not KillAll() Then Return
        PropWebServer.StopWebServer()
        PropAborting = True
        StopMysql()
        Print("I'll tell you my next dream when I wake up.")
        ProgressBar1.Value = 0
        Print("Zzzz...")
        Sleep(2000)
        End

    End Sub

    Private Sub SetLoopback()

        Dim Adapters = NetworkInterface.GetAllNetworkInterfaces()
        For Each adapter As NetworkInterface In Adapters
            Diagnostics.Debug.Print(adapter.Name)

            If adapter.Name = "Loopback" Then
                Print("Setting Loopback to WAN IP address")
                Using LoopbackProcess As New Process
                    LoopbackProcess.StartInfo.UseShellExecute = True ' so we can redirect streams
                    LoopbackProcess.StartInfo.FileName = PropMyFolder & "\NAT_Loopback_Tool.bat"
                    LoopbackProcess.StartInfo.CreateNoWindow = False
                    LoopbackProcess.StartInfo.Arguments = """" & adapter.Name & """"
                    LoopbackProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    LoopbackProcess.Start()
                    LoopbackProcess.WaitForExit()
                End Using
            End If
        Next

    End Sub

    ''' <summary>
    ''' Start Button on main form
    ''' </summary>
    Private Sub StartButton_Click(sender As System.Object, e As EventArgs) Handles StartButton.Click
        Startup()
    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim ln As Integer = TextBox1.Text.Length
        TextBox1.SelectionStart = ln
        TextBox1.ScrollToCaret()
    End Sub

    ''' <summary>
    ''' Kill processes by name
    ''' </summary>
    ''' <param name="processName"></param>
    ''' <returns></returns>
    Private Sub Zap(processName As String)

        ' Kill process by name
        For Each P As Process In System.Diagnostics.Process.GetProcessesByName(processName)
            Log("Info", "Stopping process " & processName)
            Try
                P.Kill()
            Catch
            End Try

        Next

    End Sub

#End Region

#Region "Menus"

    Public Sub Buttons(button As System.Object)
        ' Turns off all 4 stacked buttons, then enables one of them
        BusyButton.Visible = False
        StopButton.Visible = False
        StartButton.Visible = False
        InstallButton.Visible = False
        button.Visible = True
        Application.DoEvents()

    End Sub

    Public Sub Print(Value As String)

        Log("Info", Value)
        TextBox1.Text = TextBox1.Text & vbCrLf & Value
        Trim()

    End Sub

    Private Sub ConsoleCOmmandsToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ConsoleCOmmandsToolStripMenuItem1.Click
        Dim webAddress As String = "http://opensimulator.org/wiki/Server_Commands"
        Process.Start(webAddress)
    End Sub

    Private Sub Create_ShortCut(ByVal sTargetPath As String)
        ' Requires reference to Windows Script Host Object Model
        Dim WshShell As WshShellClass = New WshShellClass
        Dim MyShortcut As IWshShortcut
        Log("Info", "creating shortcut on desktop")
        ' The shortcut will be created on the desktop
        Dim DesktopFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        MyShortcut = CType(WshShell.CreateShortcut(DesktopFolder & "\Outworldz.lnk"), IWshShortcut)
        MyShortcut.TargetPath = sTargetPath
        MyShortcut.IconLocation = WshShell.ExpandEnvironmentStrings(PropMyFolder & "\Start.exe")
        MyShortcut.WorkingDirectory = PropMyFolder
        MyShortcut.Save()

    End Sub

    Private Sub MnuAbout_Click(sender As System.Object, e As EventArgs) Handles mnuAbout.Click

        Print("(c) 2017 Outworldz,LLC" & vbCrLf & "Version " & PropMyVersion)
        Dim webAddress As String = SecureDomain & "/Outworldz_Installer"
        Process.Start(webAddress)

    End Sub

    Private Sub MnuHide_Click(sender As System.Object, e As EventArgs) Handles mnuHide.Click
        Print("The Opensimulator Console will not be shown. You can still interact with it with Help->Opensim Console")
        mnuShow.Checked = False
        mnuHide.Checked = True

        Settings.ConsoleShow = mnuShow.Checked
        Settings.SaveSettings()

        If PropOpensimIsRunning() Then
            Print("The Opensimulator Console will not be shown. Change will occur when Opensim is restarted")
        End If

    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As System.Object, e As EventArgs) Handles mnuShow.Click

        Print("The Opensimulator Console will be shown when Opensim is running.")
        mnuShow.Checked = True
        mnuHide.Checked = False

        Settings.ConsoleShow = mnuShow.Checked
        Settings.SaveSettings()

        If PropOpensimIsRunning() Then
            Print("The Opensimulator Console will be shown the next time the system is started.")
        End If

    End Sub

    Private Sub StopButton_Click_1(sender As System.Object, e As EventArgs) Handles StopButton.Click

        Print("Stopping")
        Buttons(BusyButton)
        If Not KillAll() Then Return
        Buttons(StartButton)
        Print("Stopped")
        ProgressBar1.Visible = False
        ToolBar(False)

    End Sub

    Private Sub Trim()
        If TextBox1.Text.Length > TextBox1.MaxLength - 100 Then
            TextBox1.Text = Mid(TextBox1.Text, 500)
        End If
    End Sub

    Private Sub WebUIToolStripMenuItem_Click(sender As System.Object, e As EventArgs)
        Print("The Web UI lets you add or view settings for the default avatar. ")
        If PropOpensimIsRunning() Then
            Dim webAddress As String = "http://127.0.0.1:" & Settings.HttpPort
            Process.Start(webAddress)
        End If
    End Sub

#End Region

#Region "INI"

    Public Sub CopyOpensimProto(Optional name As String = "")
        If name Is Nothing Then Return
        If name.Length > 0 Then
            Dim X = PropRegionClass.FindRegionByName(name)
            If (X > -1) Then Opensimproto(X)
        Else
            ' COPY OPENSIM.INI prototype to all region folders and set the Sim Name
            For Each X As Integer In PropRegionClass.RegionNumbers
                Opensimproto(X)
            Next
        End If

    End Sub

    Public Sub CopyWifi(Page As String)
        Try
            System.IO.Directory.Delete(PropOpensimBinPath & "WifiPages", True)
            System.IO.Directory.Delete(PropOpensimBinPath & "bin\WifiPages", True)
        Catch ex As Exception
            Log("Info", ex.Message)
        End Try

        Try
            My.Computer.FileSystem.CopyDirectory(PropOpensimBinPath & "WifiPages-" & Page, PropOpensimBinPath & "WifiPages", True)
            My.Computer.FileSystem.CopyDirectory(PropOpensimBinPath & "bin\WifiPages-" & Page, PropOpensimBinPath & "\bin\WifiPages", True)
        Catch ex As Exception
            ErrorLog(ex.Message)
        End Try

    End Sub

    Public Sub DelLibrary()

        Try
            System.IO.File.Delete(PropOpensimBinPath & "bin\Library\Clothing Library (small).iar")
            System.IO.File.Delete(PropOpensimBinPath & "bin\Library\Objects Library (small).iar")
        Catch ex As Exception
            Diagnostics.Debug.Print(ex.Message)
        End Try
    End Sub

    Public Sub DoGloebits()

        'Gloebits.ini
        Settings.LoadIni(PropOpensimBinPath & "bin\Gloebit.ini", ";")
        If Settings.GloebitsEnable Then
            Settings.SetIni("Gloebit", "Enabled", "True")
        Else
            Settings.SetIni("Gloebit", "Enabled", "False")
        End If

        If Settings.GloebitsMode Then
            Settings.SetIni("Gloebit", "GLBEnvironment", "production")
            Settings.SetIni("Gloebit", "GLBKey", Settings.GLProdKey)
            Settings.SetIni("Gloebit", "GLBSecret", Settings.GLProdSecret)
        Else
            Settings.SetIni("Gloebit", "GLBEnvironment", "sandbox")
            Settings.SetIni("Gloebit", "GLBKey", Settings.GLSandKey)
            Settings.SetIni("Gloebit", "GLBSecret", Settings.GLSandSecret)
        End If

        Settings.SetIni("Gloebit", "GLBOwnerName", Settings.GLBOwnerName)
        Settings.SetIni("Gloebit", "GLBOwnerEmail", Settings.GLBOwnerEmail)

        Settings.SetIni("Gloebit", "GLBSpecificConnectionString", Settings.RobustDBConnection)

        Settings.SaveINI()

    End Sub

    ''' <summary>
    ''' Loads the INI file for the proper grid type for parsing
    ''' </summary>
    ''' <returns>Returns the path to the proper Opensim.ini prototype.</returns>
    Public Function GetOpensimProto() As String

        Select Case Settings.ServerType
            Case "Robust"
                Settings.LoadIni(PropOpensimBinPath & "bin\Opensim.proto", ";")
                Return PropOpensimBinPath & "bin\Opensim.proto"
            Case "Region"
                Settings.LoadIni(PropOpensimBinPath & "bin\OpensimRegion.proto", ";")
                Return PropOpensimBinPath & "bin\OpensimRegion.proto"
            Case "OsGrid"
                Settings.LoadIni(PropOpensimBinPath & "bin\OpensimOsGrid.proto", ";")
                Return PropOpensimBinPath & "bin\OpensimOsGrid.proto"
            Case "Metro"
                Settings.LoadIni(PropOpensimBinPath & "bin\OpensimMetro.proto", ";")
                Return PropOpensimBinPath & "bin\OpensimMetro.proto"

        End Select
        Return Nothing

    End Function

    Public Sub Opensimproto(X As Integer)

        Dim regionName = PropRegionClass.RegionName(X)
        Dim pathname = PropRegionClass.IniPath(X)
        Diagnostics.Debug.Print(regionName)

        Try

            Settings.LoadIni(GetOpensimProto(), ";")

            Settings.SetIni("Const", "BaseHostname", Settings.BaseHostName)

            Settings.SetIni("Const", "PublicPort", CStr(Settings.HttpPort)) ' 8002
            Settings.SetIni("Const", "PrivURL", "http://" & CStr(Settings.PrivateURL)) ' local IP
            Settings.SetIni("Const", "http_listener_port", CStr(PropRegionClass.RegionPort(X))) ' varies with region

            ' set new Min Timer Interval for how fast a script can go. Can be set in region files as a float, or  nothing
            Dim Xtime As Double = 1 / 11   '1/11 of a second is as fast as she can go
            If PropRegionClass.MinTimerInterval(X) <> "" Then
                Xtime = CDbl(PropRegionClass.MinTimerInterval(X))
            End If
            Settings.SetIni("XEngine", "MinTimerInterval", Convert.ToString(Xtime, Invarient))

            Dim name = PropRegionClass.RegionName(X)

            ' save the http listener port away for the group
            PropRegionClass.GroupPort(X) = PropRegionClass.RegionPort(X)

            Settings.SetIni("Const", "PrivatePort", CStr(Settings.PrivatePort)) '8003
            Settings.SetIni("Const", "RegionFolderName", CStr(PropRegionClass.GroupName(X)))
            Settings.SaveINI()

            My.Computer.FileSystem.CopyFile(GetOpensimProto(), pathname & "Opensim.ini", True)
        Catch ex As Exception
            Print("Error Failed to set the Opensim.ini for sim " & regionName & ":" & ex.Message)
            ErrorLog("Error Failed to set the Opensim.ini for sim " & regionName & ":" & ex.Message)
        End Try

    End Sub

    Public Sub SaveIceCast()

        Dim rgx As New Regex("[^a-zA-Z0-9 ]")
        Dim name As String = rgx.Replace(Settings.SimName, "")

        Dim icecast As String = "<icecast>" & vbCrLf +
                           "<hostname>" & Settings.PublicIP & "</hostname>" & vbCrLf +
                            "<location>" & name & "</location>" & vbCrLf +
                            "<admin>" & Settings.AdminEmail & "</admin>" & vbCrLf +
                            "<shoutcast-mount>/stream</shoutcast-mount>" & vbCrLf +
                            "<listen-socket>" & vbCrLf +
                            "    <port>" & CStr(Settings.SCPortBase) & "</port>" & vbCrLf +
                            "</listen-socket>" & vbCrLf +
                            "<listen-socket>" & vbCrLf +
                            "   <port>" & CStr(Settings.SCPortBase1) & "</port>" & vbCrLf +
                            "   <shoutcast-compat>1</shoutcast-compat>" & vbCrLf +
                            "</listen-socket>" & vbCrLf +
                             "<limits>" & vbCrLf +
                              "   <clients>20</clients>" & vbCrLf +
                              "    <sources>4</sources>" & vbCrLf +
                              "    <queue-size>524288</queue-size>" & vbCrLf +
                              "     <client-timeout>30</client-timeout>" & vbCrLf +
                              "    <header-timeout>15</header-timeout>" & vbCrLf +
                              "    <source-timeout>10</source-timeout>" & vbCrLf +
                              "    <burst-on-connect>1</burst-on-connect>" & vbCrLf +
                              "    <burst-size>65535</burst-size>" & vbCrLf +
                              "</limits>" & vbCrLf +
                              "<authentication>" & vbCrLf +
                                  "<source-password>" & Settings.SCPassword & "</source-password>" & vbCrLf +
                                  "<relay-password>" & Settings.SCPassword & "</relay-password>" & vbCrLf +
                                  "<admin-user>admin</admin-user>" & vbCrLf +
                                  "<admin-password>" & Settings.SCPassword & "</admin-password>" & vbCrLf +
                              "</authentication>" & vbCrLf +
                              "<http-headers>" & vbCrLf +
                              "    <header name=" & """" & "Access-Control-Allow-Origin" & """" & " value=" & """" & "*" & """" & "/>" & vbCrLf +
                              "</http-headers>" & vbCrLf +
                              "<fileserve>1</fileserve>" & vbCrLf +
                              "<paths>" & vbCrLf +
                                  "<logdir>./log</logdir>" & vbCrLf +
                                  "<webroot>./web</webroot>" & vbCrLf +
                                  "<adminroot>./admin</adminroot>" & vbCrLf &  '
                                   "<alias source=" & """" & "/" & """" & " destination=" & """" & "/status.xsl" & """" & "/>" & vbCrLf +
                              "</paths>" & vbCrLf +
                              "<logging>" & vbCrLf +
                                  "<accesslog>access.log</accesslog>" & vbCrLf +
                                  "<errorlog>error.log</errorlog>" & vbCrLf +
                                  "<loglevel>3</loglevel>" & vbCrLf +
                                  "<logsize>10000</logsize>" & vbCrLf +
                              "</logging>" & vbCrLf +
                          "</icecast>" & vbCrLf

        Using outputFile As New StreamWriter(PropMyFolder & "\Outworldzfiles\Icecast\icecast_run.xml", False)
            outputFile.WriteLine(icecast)
        End Using

    End Sub

    Private Sub DoBirds()

        Dim BirdFile = PropOpensimBinPath & "bin\addon-modules\OpenSimBirds\config\OpenSimBirds.ini"
        System.IO.File.Delete(BirdFile)
        Dim BirdData As String = ""

        ' Birds setup per region
        For Each RegionNum As Integer In PropRegionClass.RegionNumbers

            Dim simName = PropRegionClass.RegionName(RegionNum)

            Settings.LoadIni(PropRegionClass.RegionPath(RegionNum), ";")

            If Settings.BirdsModuleStartup And PropRegionClass.Birds(RegionNum) = "True" Then

                BirdData = BirdData & "[" & simName & "]" & vbCrLf &
            ";this Is the default And determines whether the module does anything" & vbCrLf &
            "BirdsModuleStartup = True" & vbCrLf & vbCrLf &
            ";set to false to disable the birds from appearing in this region" & vbCrLf &
            "BirdsEnabled = True" & vbCrLf & vbCrLf &
            ";which channel do we listen on for in world commands" & vbCrLf &
            "BirdsChatChannel = " & CStr(Settings.BirdsChatChannel) & vbCrLf & vbCrLf &
            ";the number of birds to flock" & vbCrLf &
            "BirdsFlockSize = " & CStr(Settings.BirdsFlockSize) & vbCrLf & vbCrLf &
            ";how far each bird can travel per update" & vbCrLf &
            "BirdsMaxSpeed = " & Settings.BirdsMaxSpeed.ToString(Invarient) & vbCrLf & vbCrLf &
            ";the maximum acceleration allowed to the current velocity of the bird" & vbCrLf &
            "BirdsMaxForce = " & Settings.BirdsMaxForce.ToString(Invarient) & vbCrLf & vbCrLf &
            ";max distance for other birds to be considered in the same flock as us" & vbCrLf &
            "BirdsNeighbourDistance = " & Settings.BirdsNeighbourDistance.ToString(Invarient) & vbCrLf & vbCrLf &
            ";how far away from other birds we would Like To stay" & vbCrLf &
            "BirdsDesiredSeparation = " & Settings.BirdsDesiredSeparation.ToString(Invarient) & vbCrLf & vbCrLf &
            ";how close To the edges Of things can we Get without being worried" & vbCrLf &
            "BirdsTolerance = " & Settings.BirdsTolerance.ToString(Invarient) & vbCrLf & vbCrLf &
            ";how close To the edge Of a region can we Get?" & vbCrLf &
            "BirdsBorderSize = " & Settings.BirdsBorderSize.ToString(Invarient) & vbCrLf & vbCrLf &
            ";how high are we allowed To flock" & vbCrLf &
            "BirdsMaxHeight = " & Settings.BirdsMaxHeight.ToString(Invarient) & vbCrLf & vbCrLf &
            ";By Default the Module will create a flock Of plain wooden spheres," & vbCrLf &
            ";however this can be overridden To the name Of an existing prim that" & vbCrLf &
            ";needs To already exist In the scene - i.e. be rezzed In the region." & vbCrLf &
            "BirdsPrim = " & Settings.BirdsPrim & vbCrLf & vbCrLf &
            ";who Is allowed to send commands via chat Or script List of UUIDs Or ESTATE_OWNER Or ESTATE_MANAGER" & vbCrLf &
            ";Or everyone if Not specified" & vbCrLf &
            "BirdsAllowedControllers = ESTATE_OWNER, ESTATE_MANAGER" & vbCrLf & vbCrLf & vbCrLf

            End If
        Next
        IO.File.WriteAllText(BirdFile, BirdData, Encoding.Default) 'The text file will be created if it does not already exist

    End Sub

    Private Sub DoFlotsamINI()

        Settings.LoadIni(PropOpensimBinPath & "bin\config-include\FlotsamCache.ini", ";")
        Settings.SetIni("AssetCache", "LogLevel", Settings.CacheLogLevel)
        Settings.SetIni("AssetCache", "CacheDirectory", Settings.CacheFolder)
        Settings.SetIni("AssetCache", "FileCacheEnabled", Settings.CacheEnabled)
        Settings.SetIni("AssetCache", "FileCacheTimeout", Settings.CacheTimeout)
        Settings.SaveINI()

    End Sub

    Private Sub DoGridCommon()

        'Choose a GridCommon.ini to use.
        Dim GridCommon As String = "GridcommonGridServer"

        Select Case Settings.ServerType
            Case "Robust"
                My.Computer.FileSystem.CopyDirectory(PropOpensimBinPath & "bin\Library.proto", PropOpensimBinPath & "bin\Library", True)
                GridCommon = "Gridcommon-GridServer.ini"
            Case "Region"
                My.Computer.FileSystem.CopyDirectory(PropOpensimBinPath & "bin\Library.proto", PropOpensimBinPath & "bin\Library", True)
                GridCommon = "Gridcommon-RegionServer.ini"
            Case "OsGrid"
                GridCommon = "Gridcommon-OsGridServer.ini"
            Case "Metro"
                GridCommon = "Gridcommon-MetroServer.ini"

        End Select

        ' Put that gridcommon.ini file in place
        FileStuff.CopyFile(PropOpensimBinPath & "bin\config-include\" & GridCommon, IO.Path.Combine(PropOpensimBinPath, "bin\config-include\GridCommon.ini"), True)

        Settings.LoadIni(PropOpensimBinPath & "bin\config-include\GridCommon.ini", ";")
        Settings.SetIni("HGInventoryAccessModule", "OutboundPermission", CStr(Settings.OutBoundPermissions))
        Settings.SaveINI()

    End Sub

    Private Sub DoMySQL()

        ' load and patch it up for MySQL
        Settings.LoadIni(PropOpensimBinPath & "bin\config-include\Gridcommon.ini", ";")

        Settings.SetIni("DatabaseService", "ConnectionString", Settings.RegionDBConnection)
        Settings.SaveINI()

    End Sub

    Private Sub DoOpensimINI()

        ' Opensim.ini
        Settings.LoadIni(GetOpensimProto(), ";")

        Select Case Settings.ServerType
            Case "Robust"
                If Settings.SearchEnabled Then
                    ' RegionSnapShot
                    Settings.SetIni("DataSnapshot", "index_sims", "True")
                    If Settings.SearchLocal Then
                        Settings.SetIni("DataSnapshot", "data_services", "${Const|BaseURL}:" & CStr(Settings.ApachePort) & "/Search/register.php;http://www.hyperica.com/Search/register.php")
                        Settings.SetIni("Search", "SearchURL", "${Const|BaseURL}:" & CStr(Settings.ApachePort) & "/Search/query.php")
                        Settings.SetIni("Search", "SimulatorFeatures", "${Const|BaseURL}:" & CStr(Settings.ApachePort) & "/Search/query.php")
                    Else
                        Settings.SetIni("DataSnapshot", "data_services", "http://www.hyperica.com/Search/register.php")
                        Settings.SetIni("Search", "SearchURL", "http://www.hyperica.com/Search/query.php")
                        Settings.SetIni("Search", "SimulatorFeatures", "http://www.hyperica.com/Search/query.php")
                    End If
                Else
                    Settings.SetIni("DataSnapshot", "index_sims", "False")
                End If

                Settings.SetIni("Const", "PrivURL", "http://" & Settings.PrivateURL)
                Settings.SetIni("Const", "GridName", Settings.SimName)

            Case "Region"
            Case "OSGrid"
            Case "Metro"

        End Select

        ' Support viewers object cache, default true
        ' users may need to reduce viewer bandwidth if some prims Or terrain parts fail to rez.
        ' change to false if you need to use old viewers that do Not support this feature

        Settings.SetIni("ClientStack.LindenUDP", "SupportViewerObjectsCache", CStr(Settings.SupportViewerObjectsCache))

        ' set new Min Timer Interval for how fast a script can go.
        Settings.SetIni("XEngine", "MinTimerInterval", CStr(Settings.MinTimerInterval))

        ' all grids requires these setting in Opensim.ini
        Settings.SetIni("Const", "DiagnosticsPort", CStr(Settings.DiagnosticPort))

        ' Get Opensimulator Scripts to date if needed
        If Settings.DeleteScriptsOnStartupLevel <> SimVersion Then
            KillOldFiles()  ' wipe out DLL's
            ClrCache.WipeScripts()
            Settings.DeleteScriptsOnStartupLevel() = SimVersion ' we have scripts cleared to proper Opensim Version
            Settings.SaveSettings()

            Settings.SetIni("XEngine", "DeleteScriptsOnStartup", "True")
        Else
            Settings.SetIni("XEngine", "DeleteScriptsOnStartup", "False")
        End If

        If Settings.LSLHTTP Then
            ' do nothing - let them edit it
        Else
            Settings.SetIni("Network", "OutboundDisallowForUserScriptsExcept", Settings.PrivateURL & "/32")
        End If

        Settings.SetIni("Network", "ExternalHostNameForLSL", Settings.BaseHostName)
        Settings.SetIni("PrimLimitsModule", "EnforcePrimLimits", CStr(Settings.Primlimits))

        If Settings.Primlimits Then
            Settings.SetIni("Permissions", "permissionmodules", "DefaultPermissionsModule, PrimLimitsModule")
        Else
            Settings.SetIni("Permissions", "permissionmodules", "DefaultPermissionsModule")
        End If

        If Settings.GloebitsEnable Then
            Settings.SetIni("Startup", "economymodule", "Gloebit")
        Else
            Settings.SetIni("Startup", "economymodule", "BetaGridLikeMoneyModule")
        End If

        ' Main Frame time
        ' This defines the rate of several simulation events.
        ' Default value should meet most needs.
        ' It can be reduced To improve the simulation Of moving objects, with possible increase of CPU and network loads.
        'FrameTime = 0.0909

        Settings.SetIni("Startup", "FrameTime", Convert.ToString(1 / 11, Invarient))

        ' LSL emails
        Settings.SetIni("SMTP", "SMTP_SERVER_HOSTNAME", Settings.SmtpHost)
        Settings.SetIni("SMTP", "SMTP_SERVER_PORT", CStr(Settings.SmtpPort))
        Settings.SetIni("SMTP", "SMTP_SERVER_LOGIN", Settings.SmtPropUserName)
        Settings.SetIni("SMTP", "SMTP_SERVER_PASSWORD", Settings.SmtpPassword)
        Settings.SetIni("SMTP", "host_domain_header_from", Settings.BaseHostName)

        ' the old Clouds
        If Settings.Clouds Then
            Settings.SetIni("Cloud", "enabled", "True")
            Settings.SetIni("Cloud", "density", Settings.Density.ToString(Invarient))
        Else
            Settings.SetIni("Cloud", "enabled", "False")
        End If

        ' Gods

        If (Settings.RegionOwnerIsGod Or Settings.RegionManagerIsGod) Then
            Settings.SetIni("Permissions", "allow_grid_gods", "True")
        Else
            Settings.SetIni("Permissions", "allow_grid_gods", "False")
        End If

        ' Physics choices for meshmerizer, where Ubit's ODE requires a special one
        ' ZeroMesher meshing = Meshmerizer meshing = ubODEMeshmerizer

        ' 0 = physics = none 1 = OpenDynamicsEngine 2 = physics = BulletSim 3 = physics = BulletSim
        ' with threads 4 = physics = ubODE

        Select Case Settings.Physics
            Case 0
                Settings.SetIni("Startup", "meshing", "ZeroMesher")
                Settings.SetIni("Startup", "physics", "basicphysics")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
            Case 1
                Settings.SetIni("Startup", "meshing", "Meshmerizer")
                Settings.SetIni("Startup", "physics", "OpenDynamicsEngine")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
            Case 2
                Settings.SetIni("Startup", "meshing", "Meshmerizer")
                Settings.SetIni("Startup", "physics", "BulletSim")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
            Case 3
                Settings.SetIni("Startup", "meshing", "Meshmerizer")
                Settings.SetIni("Startup", "physics", "BulletSim")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "True")
            Case 4
                Settings.SetIni("Startup", "meshing", "ubODEMeshmerizer")
                Settings.SetIni("Startup", "physics", "ubODE")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
            Case 5
                Settings.SetIni("Startup", "meshing", "Meshmerizer")
                Settings.SetIni("Startup", "physics", "ubODE")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
            Case Else
                Settings.SetIni("Startup", "meshing", "Meshmerizer")
                Settings.SetIni("Startup", "physics", "BulletSim")
                Settings.SetIni("Startup", "UseSeparatePhysicsThread", "True")
        End Select

        Settings.SetIni("Map", "RenderMaxHeight", CInt(Settings.RenderMaxHeight))
        Settings.SetIni("Map", "RenderMinHeight", CInt(Settings.RenderMinHeight))

        If Settings.MapType = "None" Then
            Settings.SetIni("Map", "GenerateMaptiles", "False")
        ElseIf Settings.MapType = "Simple" Then
            Settings.SetIni("Map", "GenerateMaptiles", "True")
            Settings.SetIni("Map", "MapImageModule", "MapImageModule")  ' versus Warp3DImageModule
            Settings.SetIni("Map", "TextureOnMapTile", "False")         ' versus true
            Settings.SetIni("Map", "DrawPrimOnMapTile", "False")
            Settings.SetIni("Map", "TexturePrims", "False")
            Settings.SetIni("Map", "RenderMeshes", "False")
        ElseIf Settings.MapType = "Good" Then
            Settings.SetIni("Map", "GenerateMaptiles", "True")
            Settings.SetIni("Map", "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
            Settings.SetIni("Map", "TextureOnMapTile", "False")         ' versus true
            Settings.SetIni("Map", "DrawPrimOnMapTile", "False")
            Settings.SetIni("Map", "TexturePrims", "False")
            Settings.SetIni("Map", "RenderMeshes", "False")
        ElseIf Settings.MapType = "Better" Then
            Settings.SetIni("Map", "GenerateMaptiles", "True")
            Settings.SetIni("Map", "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
            Settings.SetIni("Map", "TextureOnMapTile", "True")         ' versus true
            Settings.SetIni("Map", "DrawPrimOnMapTile", "True")
            Settings.SetIni("Map", "TexturePrims", "False")
            Settings.SetIni("Map", "RenderMeshes", "False")
        ElseIf Settings.MapType = "Best" Then
            Settings.SetIni("Map", "GenerateMaptiles", "True")
            Settings.SetIni("Map", "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
            Settings.SetIni("Map", "TextureOnMapTile", "True")      ' versus true
            Settings.SetIni("Map", "DrawPrimOnMapTile", "True")
            Settings.SetIni("Map", "TexturePrims", "True")
            Settings.SetIni("Map", "RenderMeshes", "True")
        End If

        ' Voice
        If Settings.VivoxEnabled Then
            Settings.SetIni("VivoxVoice", "enabled", "True")
        Else
            Settings.SetIni("VivoxVoice", "enabled", "False")
        End If
        Settings.SetIni("VivoxVoice", "vivox_admin_user", Settings.VivoxUserName)
        Settings.SetIni("VivoxVoice", "vivox_admin_password", Settings.VivoxPassword)

        Settings.SaveINI()

    End Sub

    Private Sub DoRegions()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Regions - write all region.ini files with public IP and Public port
        ' has to be bound late so regions data is there.

        If Not Settings.PortsChanged And Not PropViewedSettings Then Return
        Settings.PortsChanged = False
        PropViewedSettings = False

        ' Self setting Region Ports
        Dim SimPort As Integer = CType(Settings.FirstRegionPort(), Integer)

        For Each RegionNum As Integer In PropRegionClass.RegionNumbers

            Dim simName = PropRegionClass.RegionName(RegionNum)

            Settings.LoadIni(PropRegionClass.RegionPath(RegionNum), ";")

            ' Autobackup
            If Settings.AutoBackup &
                PropRegionClass.SkipAutobackup(RegionNum) = "" Or
                PropRegionClass.SkipAutobackup(RegionNum) = "False" Then
                Settings.SetIni(simName, "AutoBackup", "True")
            Else
                Settings.SetIni(simName, "AutoBackup", "False")
            End If

            Settings.SetIni(simName, "InternalPort", CStr(SimPort))
            PropMaxPortUsed = SimPort
            PropRegionClass.RegionPort(RegionNum) = SimPort
            SimPort += 1

            Settings.SetIni(simName, "ExternalHostName", ExternLocalServerName())

            ' not a standard INI, only use by the Dreamers
            If PropRegionClass.RegionEnabled(RegionNum) Then
                Settings.SetIni(simName, "Enabled", "True")
            Else
                Settings.SetIni(simName, "Enabled", "False")
            End If

            ' Extended in v 2.1
            Settings.SetIni(simName, "NonPhysicalPrimMax", CStr(PropRegionClass.NonPhysicalPrimMax(RegionNum)))
            Settings.SetIni(simName, "PhysicalPrimMax", CStr(PropRegionClass.PhysicalPrimMax(RegionNum)))
            If (Settings.Primlimits) Then
                Settings.SetIni(simName, "MaxPrims", CStr(PropRegionClass.MaxPrims(RegionNum)))
            Else
                Settings.SetIni(simName, "MaxPrims", "")
            End If

            Settings.SetIni(simName, "MaxAgents", CStr(PropRegionClass.MaxAgents(RegionNum)))
            Settings.SetIni(simName, "ClampPrimSize", CStr(PropRegionClass.ClampPrimSize(RegionNum)))
            Settings.SetIni(simName, "MaxPrims", CStr(PropRegionClass.MaxPrims(RegionNum)))

            ' Optional
            ' Extended in v 2.31 optional things
            If PropRegionClass.MapType(RegionNum) = "None" Then
                Settings.SetIni(simName, "GenerateMaptiles", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Simple" Then
                Settings.SetIni(simName, "GenerateMaptiles", "True")
                Settings.SetIni(simName, "MapImageModule", "MapImageModule")  ' versus Warp3DImageModule
                Settings.SetIni(simName, "TextureOnMapTile", "False")         ' versus True
                Settings.SetIni(simName, "DrawPrimOnMapTile", "False")
                Settings.SetIni(simName, "TexturePrims", "False")
                Settings.SetIni(simName, "RenderMeshes", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Good" Then
                Settings.SetIni(simName, "GenerateMaptiles", "True")
                Settings.SetIni(simName, "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
                Settings.SetIni(simName, "TextureOnMapTile", "False")         ' versus True
                Settings.SetIni(simName, "DrawPrimOnMapTile", "False")
                Settings.SetIni(simName, "TexturePrims", "False")
                Settings.SetIni(simName, "RenderMeshes", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Better" Then
                Settings.SetIni(simName, "GenerateMaptiles", "True")
                Settings.SetIni(simName, "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
                Settings.SetIni(simName, "TextureOnMapTile", "True")         ' versus True
                Settings.SetIni(simName, "DrawPrimOnMapTile", "True")
                Settings.SetIni(simName, "TexturePrims", "False")
                Settings.SetIni(simName, "RenderMeshes", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Best" Then
                Settings.SetIni(simName, "GenerateMaptiles", "True")
                Settings.SetIni(simName, "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
                Settings.SetIni(simName, "TextureOnMapTile", "True")      ' versus True
                Settings.SetIni(simName, "DrawPrimOnMapTile", "True")
                Settings.SetIni(simName, "TexturePrims", "True")
                Settings.SetIni(simName, "RenderMeshes", "True")
            Else
                Settings.SetIni(simName, "GenerateMaptiles", "")
                Settings.SetIni(simName, "MapImageModule", "")  ' versus MapImageModule
                Settings.SetIni(simName, "TextureOnMapTile", "")      ' versus True
                Settings.SetIni(simName, "DrawPrimOnMapTile", "")
                Settings.SetIni(simName, "TexturePrims", "")
                Settings.SetIni(simName, "RenderMeshes", "")
            End If

            Settings.SetIni(simName, "DisableGloebits", PropRegionClass.DisableGloebits(RegionNum))
            Settings.SetIni(simName, "AllowGods", PropRegionClass.AllowGods(RegionNum))
            Settings.SetIni(simName, "RegionGod", PropRegionClass.RegionGod(RegionNum))
            Settings.SetIni(simName, "ManagerGod", PropRegionClass.ManagerGod(RegionNum))
            Settings.SetIni(simName, "RegionSnapShot", PropRegionClass.RegionSnapShot(RegionNum))
            Settings.SetIni(simName, "Birds", PropRegionClass.Birds(RegionNum))
            Settings.SetIni(simName, "Tides", PropRegionClass.Tides(RegionNum))
            Settings.SetIni(simName, "Teleport", PropRegionClass.Teleport(RegionNum))
            Settings.SetIni(simName, "DisallowForeigners", PropRegionClass.DisallowForeigners(RegionNum))
            Settings.SetIni(simName, "DisallowResidents", PropRegionClass.DisallowResidents(RegionNum))
            Settings.SetIni(simName, "SkipAutoBackup", PropRegionClass.SkipAutobackup(RegionNum))
            Settings.SetIni(simName, "Physics", PropRegionClass.Physics(RegionNum))
            Settings.SetIni(simName, "FrameTime", PropRegionClass.FrameTime(RegionNum))

            Settings.SaveINI()

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' Opensim.ini in Region Folder specific to this region
            Settings.LoadIni(PropOpensimBinPath & "bin\Regions\" & PropRegionClass.GroupName(RegionNum) & "\Opensim.ini", ";")

            ' Autobackup
            If Settings.AutoBackup Then
                Settings.SetIni("AutoBackupModule", "AutoBackup", "True")
            End If

            If Settings.AutoBackup And PropRegionClass.SkipAutobackup(RegionNum) = "" Then
                Settings.SetIni("AutoBackupModule", "AutoBackup", "True")
            End If

            If Settings.AutoBackup And PropRegionClass.SkipAutobackup(RegionNum) = "True" Then
                Settings.SetIni("AutoBackupModule", "AutoBackup", "False")
            End If

            If Not Settings.AutoBackup Then
                Settings.SetIni("AutoBackupModule", "AutoBackup", "False")
            End If

            Settings.SetIni("AutoBackupModule", "AutoBackupInterval", Settings.AutobackupInterval)
            Settings.SetIni("AutoBackupModule", "AutoBackupKeepFilesForDays", CStr(Settings.KeepForDays))
            Settings.SetIni("AutoBackupModule", "AutoBackupDir", BackupPath())

            If PropRegionClass.MapType(RegionNum) = "Simple" Then
                Settings.SetIni("Map", "GenerateMaptiles", "True")
                Settings.SetIni("Map", "MapImageModule", "MapImageModule")  ' versus Warp3DImageModule
                Settings.SetIni("Map", "TextureOnMapTile", "False")         ' versus True
                Settings.SetIni("Map", "DrawPrimOnMapTile", "False")
                Settings.SetIni("Map", "TexturePrims", "False")
                Settings.SetIni("Map", "RenderMeshes", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Good" Then
                Settings.SetIni(simName, "GenerateMaptiles", "True")
                Settings.SetIni("Map", "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
                Settings.SetIni("Map", "TextureOnMapTile", "False")         ' versus True
                Settings.SetIni("Map", "DrawPrimOnMapTile", "False")
                Settings.SetIni("Map", "TexturePrims", "False")
                Settings.SetIni("Map", "RenderMeshes", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Better" Then
                Settings.SetIni("Map", "GenerateMaptiles", "True")
                Settings.SetIni("Map", "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
                Settings.SetIni("Map", "TextureOnMapTile", "True")         ' versus True
                Settings.SetIni("Map", "DrawPrimOnMapTile", "True")
                Settings.SetIni("Map", "TexturePrims", "False")
                Settings.SetIni("Map", "RenderMeshes", "False")
            ElseIf PropRegionClass.MapType(RegionNum) = "Best" Then
                Settings.SetIni("Map", "GenerateMaptiles", "True")
                Settings.SetIni("Map", "MapImageModule", "Warp3DImageModule")  ' versus MapImageModule
                Settings.SetIni("Map", "TextureOnMapTile", "True")      ' versus True
                Settings.SetIni("Map", "DrawPrimOnMapTile", "True")
                Settings.SetIni("Map", "TexturePrims", "True")
                Settings.SetIni("Map", "RenderMeshes", "True")
            End If

            Select Case PropRegionClass.Physics(RegionNum)
                Case ""
                    Settings.SetIni("Startup", "meshing", "Meshmerizer")
                    Settings.SetIni("Startup", "physics", "BulletSim")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "True")
                Case "0"
                    Settings.SetIni("Startup", "meshing", "ZeroMesher")
                    Settings.SetIni("Startup", "physics", "basicphysics")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
                Case "1"
                    Settings.SetIni("Startup", "meshing", "Meshmerizer")
                    Settings.SetIni("Startup", "physics", "OpenDynamicsEngine")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
                Case "2"
                    Settings.SetIni("Startup", "meshing", "Meshmerizer")
                    Settings.SetIni("Startup", "physics", "BulletSim")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
                Case "3"
                    Settings.SetIni("Startup", "meshing", "Meshmerizer")
                    Settings.SetIni("Startup", "physics", "BulletSim")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "True")
                Case "4"
                    Settings.SetIni("Startup", "meshing", "ubODEMeshmerizer")
                    Settings.SetIni("Startup", "physics", "ubODE")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
                Case "5"
                    Settings.SetIni("Startup", "meshing", "Meshmerizer")
                    Settings.SetIni("Startup", "physics", "ubODE")
                    Settings.SetIni("Startup", "UseSeparatePhysicsThread", "False")
                Case Else
                    ' do nothing
            End Select

            Select Case PropRegionClass.AllowGods(RegionNum)
                Case ""
                    Settings.SetIni("Permissions", "allow_grid_gods", CStr(Settings.AllowGridGods))
                Case "False"
                    Settings.SetIni("Permissions", "allow_grid_gods", "False")
                Case "True"
                    Settings.SetIni("Permissions", "allow_grid_gods", "True")
            End Select

            Select Case PropRegionClass.RegionGod(RegionNum)
                Case ""
                    Settings.SetIni("Permissions", "region_owner_is_god", CStr(Settings.RegionOwnerIsGod))
                Case "False"
                    Settings.SetIni("Permissions", "region_owner_is_god", "False")
                Case "True"
                    Settings.SetIni("Permissions", "region_owner_is_god", "True")
            End Select

            Select Case PropRegionClass.ManagerGod(RegionNum)
                Case ""
                    Settings.SetIni("Permissions", "region_manager_is_god", CStr(Settings.RegionManagerIsGod))
                Case "False"
                    Settings.SetIni("Permissions", "region_manager_is_god", "False")
                Case "True"
                    Settings.SetIni("Permissions", "region_manager_is_god", "True")
            End Select

            ' no main setting for these
            Settings.SetIni("SmartStart", "Enabled", CStr(PropRegionClass.SmartStart(RegionNum)))
            Settings.SetIni("DisallowForeigners", "Enabled", CStr(PropRegionClass.DisallowForeigners(RegionNum)))
            Settings.SetIni("DisallowResidents", "Enabled", CStr(PropRegionClass.DisallowResidents(RegionNum)))

            ' V3.15
            Settings.SetIni("Startup", "NonPhysicalPrimMax", CStr(PropRegionClass.NonPhysicalPrimMax(RegionNum)))
            Settings.SetIni("Startup", "PhysicalPrimMax", CStr(PropRegionClass.PhysicalPrimMax(RegionNum)))
            Settings.SetIni("XEngine", "MinTimerInterval", CStr(PropRegionClass.MinTimerInterval(RegionNum)))

            If PropRegionClass.DisableGloebits(RegionNum) = "True" Then
                Settings.SetIni("Startup", "economymodule", "BetaGridLikeMoneyModule")
            End If

            ' Search
            Select Case PropRegionClass.Snapshot(RegionNum)
                Case ""
                    Settings.SetIni("DataSnapshot", "index_sims", CStr(Settings.SearchEnabled))
                Case "True"
                    Settings.SetIni("DataSnapshot", "index_sims", "True")
                Case "False"
                    Settings.SetIni("DataSnapshot", "index_sims", "False")
            End Select

            Settings.SaveINI()
        Next

    End Sub

    Private Sub DoRobust()

        ''''''''''''''''''''''''''''''''''''''''''
        If Settings.ServerType = "Robust" Then
            ' Robust Process
            Settings.LoadIni(PropOpensimBinPath & "bin\Robust.HG.ini", ";")

            Settings.SetIni("DatabaseService", "ConnectionString", Settings.RobustDBConnection)
            Settings.SetIni("Const", "GridName", Settings.SimName)
            Settings.SetIni("Const", "BaseURL", "http://" & Settings.PublicIP)
            Settings.SetIni("Const", "PrivURL", "http://" & Settings.PrivateURL)
            Settings.SetIni("Const", "PublicPort", CStr(Settings.HttpPort)) ' 8002
            Settings.SetIni("Const", "PrivatePort", CStr(Settings.PrivatePort))
            Settings.SetIni("Const", "http_listener_port", CStr(Settings.HttpPort))
            Settings.SetIni("GridInfoService", "welcome", Settings.SplashPage)

            If Settings.Suitcase() Then
                Settings.SetIni("HGInventoryService", "LocalServiceModule", "OpenSim.Services.HypergridService.dll:HGSuitcaseInventoryService")
            Else
                Settings.SetIni("HGInventoryService", "LocalServiceModule", "OpenSim.Services.HypergridService.dll:HGInventoryService")
            End If

            ' LSL emails
            Settings.SetIni("SMTP", "SMTP_SERVER_HOSTNAME", Settings.SmtpHost)
            Settings.SetIni("SMTP", "SMTP_SERVER_PORT", CStr(Settings.SmtpPort))
            Settings.SetIni("SMTP", "SMTP_SERVER_LOGIN", Settings.SmtPropUserName)
            Settings.SetIni("SMTP", "SMTP_SERVER_PASSWORD", Settings.SmtpPassword)

            If Settings.SearchLocal Then
                Settings.SetIni("LoginService", "SearchURL", "${Const|BaseURL}:" & CStr(Settings.ApachePort) & "/Search/query.php")
            Else
                Settings.SetIni("LoginService", "SearchURL", "http://www.hyperica.com/Search/query.php")
            End If

            Settings.SetIni("LoginService", "WelcomeMessage", Settings.WelcomeMessage)

            'FSASSETS
            If Settings.FsAssetsEnabled Then
                Settings.SetIni("AssetService", "LocalServiceModule", "OpenSim.Services.FSAssetService.dll:FSAssetConnector")
                Settings.SetIni("HGAssetService", "LocalServiceModule", "OpenSim.Services.HypergridService.dll:HGFSAssetService")
            Else
                Settings.SetIni("AssetService", "LocalServiceModule", "OpenSim.Services.AssetService.dll:AssetService")
                Settings.SetIni("HGAssetService", "LocalServiceModule", "OpenSim.Services.HypergridService.dll:HGAssetService")
            End If

            Settings.SetIni("AssetService", "BaseDirectory", Settings.BaseDirectory & "/data")
            Settings.SetIni("AssetService", "SpoolDirectory", Settings.BaseDirectory & "/tmp")
            Settings.SetIni("AssetService", "ShowConsoleStats", Settings.ShowConsoleStats)

            Settings.SetIni("SmartStart", "Enabled", CStr(Settings.SmartStart))

            Settings.SaveINI()

        End If

    End Sub

    Private Sub DoTides()
        Dim TideData As String = ""
        Dim TideFile = PropOpensimBinPath & "bin\addon-modules\OpenSimTide\config\OpenSimTide.ini"
        System.IO.File.Delete(TideFile)

        For Each RegionNum As Integer In PropRegionClass.RegionNumbers
            Dim simName = PropRegionClass.RegionName(RegionNum)
            'Tides Setup per region
            If Settings.TideEnabled And PropRegionClass.Tides(RegionNum) = "True" Then

                TideData = TideData & ";; Set the Tide settings per named region" & vbCrLf &
                    "[" & simName & "]" & vbCrLf &
                ";this determines whether the module does anything in this region" & vbCrLf &
                ";# {TideEnabled} {} {Enable the tide to come in And out?} {true false} false" & vbCrLf &
                "TideEnabled = True" & vbCrLf &
                    vbCrLf &
                ";; Tides currently only work on single regions And varregions (non megaregions) " & vbCrLf &
                ";# surrounded completely by water" & vbCrLf &
                ";; Anything else will produce weird results where you may see a big" & vbCrLf &
                ";; vertical 'step' in the ocean" & vbCrLf &
                ";; update the tide every x simulator frames" & vbCrLf &
                "TideUpdateRate = 50" & vbCrLf &
                    vbCrLf &
                ";; low And high water marks in metres" & vbCrLf &
                "TideHighWater = " & Convert.ToString(Settings.TideHighLevel(), Invarient) & vbCrLf &
                "TideLowWater = " & Convert.ToString(Settings.TideLowLevel(), Invarient) & vbCrLf &
                vbCrLf &
                ";; how long in seconds for a complete cycle time low->high->low" & vbCrLf &
                "TideCycleTime = " & CStr(Settings.CycleTime()) & vbCrLf &
                    vbCrLf &
                ";; provide tide information on the console?" & vbCrLf &
                "TideInfoDebug = " & CStr(Settings.TideInfoDebug) & vbCrLf &
                    vbCrLf &
                ";; chat tide info to the whole region?" & vbCrLf &
                "TideInfoBroadcast = " & Settings.BroadcastTideInfo() & vbCrLf &
                    vbCrLf &
                ";; which channel to region chat on for the full tide info" & vbCrLf &
                "TideInfoChannel = " & CStr(Settings.TideInfoChannel) & vbCrLf &
                vbCrLf &
                ";; which channel to region chat on for just the tide level in metres" & vbCrLf &
                "TideLevelChannel = " & CStr(Settings.TideLevelChannel()) & vbCrLf &
                    vbCrLf &
                ";; How many times to repeat Tide Warning messages at high/low tide" & vbCrLf &
                "TideAnnounceCount = 1" & vbCrLf & vbCrLf & vbCrLf & vbCrLf
            End If

        Next
        IO.File.WriteAllText(TideFile, TideData, Encoding.Default) 'The text file will be created if it does not already exist

    End Sub

    Private Sub DoTOS()

        ' TOSModule is disabled in Grids
        If (False) Then
            Settings.LoadIni(PropOpensimBinPath & "bin\DivaTOS.ini", ";")

            'Disable it as it is broken for now.

            'Settings.SetIni("TOSModule", "Enabled", Settings.TOSEnabled)
            Settings.SetIni("TOSModule", "Enabled", CStr(False))
            'Settings.SetIni("TOSModule", "Message", Settings.TOSMessage)
            'Settings.SetIni("TOSModule", "Timeout", Settings.TOSTimeout)
            Settings.SetIni("TOSModule", "ShowToLocalUsers", CStr(Settings.ShowToLocalUsers))
            Settings.SetIni("TOSModule", "ShowToForeignUsers", CStr(Settings.ShowToForeignUsers))
            Settings.SetIni("TOSModule", "TOS_URL", "http://" & Settings.PublicIP & ":" & Settings.HttpPort & "/wifi/termsofservice.html")
            Settings.SaveINI()
        End If

    End Sub

    Private Sub DoWifi()

        Settings.LoadIni(PropOpensimBinPath & "bin\Wifi.ini", ";")
        Settings.SetIni("DatabaseService", "ConnectionString", Settings.RobustDBConnection)

        ' Wifi Section

        If Settings.ServerType = "Robust" Then ' wifi could be on or off
            If (Settings.WifiEnabled) Then
                Settings.SetIni("WifiService", "Enabled", "True")
            Else
                Settings.SetIni("WifiService", "Enabled", "False")
            End If
        Else ' it is always off
            ' shutdown wifi in Attached mode
            Settings.SetIni("WifiService", "Enabled", "False")
        End If

        Settings.SetIni("WifiService", "GridName", Settings.SimName)
        Settings.SetIni("WifiService", "LoginURL", "http://" & Settings.PublicIP & ":" & Settings.HttpPort)
        Settings.SetIni("WifiService", "WebAddress", "http://" & Settings.PublicIP & ":" & Settings.HttpPort)

        ' Wifi Admin'
        Settings.SetIni("WifiService", "AdminFirst", Settings.AdminFirst)    ' Wifi
        Settings.SetIni("WifiService", "AdminLast", Settings.AdminLast)      ' Admin
        Settings.SetIni("WifiService", "AdminPassword", Settings.Password)   ' secret
        Settings.SetIni("WifiService", "AdminEmail", Settings.AdminEmail)    ' send notifications to this person

        'Gmail and other SMTP mailers
        ' Gmail requires you set to set low security access
        Settings.SetIni("WifiService", "SmtpHost", Settings.SmtpHost)
        Settings.SetIni("WifiService", "SmtpPort", CStr(Settings.SmtpPort))
        Settings.SetIni("WifiService", "SmtpUsername", Settings.SmtPropUserName)
        Settings.SetIni("WifiService", "SmtpPassword", Settings.SmtpPassword)

        Settings.SetIni("WifiService", "HomeLocation", Settings.WelcomeRegion & "/" & Settings.HomeVectorX & "/" & Settings.HomeVectorY & "/" & Settings.HomeVectorZ)

        If Settings.AccountConfirmationRequired Then
            Settings.SetIni("WifiService", "AccountConfirmationRequired", "True")
        Else
            Settings.SetIni("WifiService", "AccountConfirmationRequired", "False")
        End If

        Settings.SaveINI()

    End Sub

    Private Sub EditForeigners()

        ' adds a list like 'Region_Test_1 = "DisallowForeigners"' to Gridcommon.ini

        Dim Authorizationlist As String = ""
        For Each RegionNum As Integer In PropRegionClass.RegionNumbers

            Dim simName = PropRegionClass.RegionName(RegionNum)
            '(replace spaces with underscore)
            simName = simName.Replace(" ", "_")    ' because this is a screwy thing they did in the INI file
            Dim df As Boolean = False
            Dim dr As Boolean = False
            If PropRegionClass.DisallowForeigners(RegionNum) = "True" Then
                df = True
            End If
            If PropRegionClass.DisallowResidents(RegionNum) = "True" Then
                dr = True
            End If
            If Not dr And Not df Then

            ElseIf dr And Not df Then
                Authorizationlist += "Region_" & simName & " = DisallowResidents" & vbCrLf
            ElseIf Not dr And df Then
                Authorizationlist += "Region_" & simName & " = DisallowForeigners" & vbCrLf
            ElseIf dr And df Then
                Authorizationlist += "Region_" & simName & " = DisallowResidents " & vbCrLf
            End If

        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim reader As StreamReader
        Dim line As String
        Dim Output As String = ""

        reader = System.IO.File.OpenText(PropOpensimBinPath & "bin\config-include\GridCommon.ini")
        'now loop through each line
        Dim skip As Boolean = False
        While reader.Peek <> -1
            line = reader.ReadLine()

            If line.Contains("; START") Then
                Output += line & vbCrLf
                Output += Authorizationlist
                skip = True
            ElseIf line.Contains("; END") Then
                Output += line & vbCrLf
                skip = False
            Else
                If Not skip Then Output += line & vbCrLf
            End If

        End While

        'close the reader
        reader.Close()

        FileStuff.DeleteFile(PropOpensimBinPath & "bin\config-include\GridCommon.ini")

        Using outputFile As New StreamWriter(PropOpensimBinPath & "bin\config-include\Gridcommon.ini")
            outputFile.Write(Output)
        End Using

    End Sub

    Private Sub SetDefaultSims()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' set the defaults in the INI for the viewer to use. Painful to do as it's a Left hand side
        ' edit must be done before other edits to Robust.HG.ini as this makes the actual Robust.HG.ifile
        Dim reader As StreamReader
        Dim line As String

        Try
            ' add this sim name as a default to the file as HG regions, and add the other regions as fallback
            ' it may have been deleted
            Dim o As Integer = PropRegionClass.FindRegionByName(Settings.WelcomeRegion)

            If o < 0 Then
                Return
            End If

            Dim DefaultName = Settings.WelcomeRegion
            '(replace spaces with underscore)
            DefaultName = DefaultName.Replace(" ", "_")    ' because this is a screwy thing they did in the INI file

            FileStuff.DeleteFile(PropOpensimBinPath & "bin\Robust.HG.ini")

            Using outputFile As New StreamWriter(PropOpensimBinPath & "bin\Robust.HG.ini")
                reader = System.IO.File.OpenText(PropOpensimBinPath & "bin\Robust.HG.ini.proto")
                'now loop through each line
                While reader.Peek <> -1
                    line = reader.ReadLine()

                    If line.Contains("Region_REPLACE") Then

                        line = "Region_" & DefaultName & " = " & """" & "DefaultRegion, DefaultHGRegion" & """"
                        Diagnostics.Debug.Print(line)
                        outputFile.WriteLine(line)

                        If Settings.SmartStart Then
                            For Each RegionNum As Integer In PropRegionClass.RegionNumbers
                                Dim RegionName = PropRegionClass.RegionName(RegionNum)

                                If RegionName <> Settings.WelcomeRegion Then
                                    If PropRegionClass.SmartStart(RegionNum) Then
                                        RegionName = RegionName.Replace(" ", "_")    ' because this is a screwy thing they did in the INI file
                                        line = "Region_" & RegionName & " = " & "FallbackRegion, Persistent"
                                    Else
                                        RegionName = RegionName.Replace(" ", "_")    ' because this is a screwy thing they did in the INI file
                                        line = "Region_" & RegionName & " = " & "FallbackRegion"
                                    End If

                                    Diagnostics.Debug.Print(line)
                                    outputFile.WriteLine(line)
                                Else
                                    Diagnostics.Debug.Print(line)
                                End If

                            Next
                        Else
                            For Each RegionNum As Integer In PropRegionClass.RegionNumbers
                                Dim RegionName = PropRegionClass.RegionName(RegionNum)
                                If RegionName <> Settings.WelcomeRegion _
                                And PropRegionClass.SmartStart(RegionNum) Then

                                    RegionName = RegionName.Replace(" ", "_")    ' because this is a screwy thing they did in the INI file
                                    line = "Region_" & RegionName & " = " & "FallbackRegion"
                                    Diagnostics.Debug.Print(line)
                                    outputFile.WriteLine(line)

                                End If
                            Next
                        End If
                    Else
                        outputFile.WriteLine(line)
                    End If

                End While
            End Using
            'close your reader
            reader.Close()
        Catch ex As Exception
            MsgBox("Could not set default sim for visitors. See Regions Panel.", vbInformation, "Settings")
        Finally

        End Try

    End Sub

    Private Function SetIniData() As Boolean

        Print("Creating INI Files")

        SetDefaultSims() ' do not swap order of this with  DoRobust. This creates Robust.HG.ini from the .proto
        DoRobust()
        DoTOS()
        DoGridCommon()
        EditForeigners()
        DelLibrary()
        DoMySQL()
        DoFlotsamINI()
        DoOpensimINI()
        DoWifi()
        DoGloebits()
        CopyOpensimProto()
        DoRegions() ' must be after Gloebits
        DoTides()
        DoBirds()
        MapSetup()
        DoPHP()
        DoApache()

        Return True

    End Function

#End Region

#Region "Checks"

    Public Sub CheckDefaultPorts()
        Try
            If Settings.DiagnosticPort = Settings.HttpPort _
        Or Settings.DiagnosticPort = Settings.PrivatePort _
        Or Settings.HttpPort = Settings.PrivatePort Then
                Settings.DiagnosticPort = 8001
                Settings.HttpPort = 8002
                Settings.PrivatePort = 8003

                MsgBox("Port conflict detected. Sim Ports have been reset to the defaults", vbInformation, "Error")
            End If
        Catch
        End Try

    End Sub

    ''' <summary>
    ''' Gets the External Host name which can be either the Public IP or a Host name.
    ''' </summary>
    ''' <returns>Host for regions</returns>
    Public Function ExternLocalServerName() As String

        Dim Host As String

        If Settings.ExternalHostName.Length > 0 Then
            Host = Settings.ExternalHostName
        Else
            Host = Settings.PublicIP
        End If
        Return Host

    End Function

    Public Sub SetRegionINI(regionname As String, key As String, value As String)

        Dim X = PropRegionClass.FindRegionByName(regionname)
        Settings.LoadIni(PropRegionClass.RegionPath(X), ";")
        Settings.SetIni(regionname, key, value)
        Settings.SaveINI()

    End Sub

#End Region

#Region "ToolBars"

    Public Sub ToolBar(visible As Boolean)

        Label3.Visible = visible
        AvatarLabel.Visible = visible
        PercentCPU.Visible = visible
        PercentRAM.Visible = visible

    End Sub

    Private Sub AdminUIToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ViewWebUI.Click
        If PropOpensimIsRunning() Then
            If Settings.ApacheEnable Then
                Dim webAddress As String = "http://127.0.0.1:" & CStr(Settings.ApachePort)
                Process.Start(webAddress)
            Else
                Dim webAddress As String = "http://127.0.0.1:" & Settings.HttpPort
                Process.Start(webAddress)
                Print("Log in as '" & Settings.AdminFirst & " " & Settings.AdminLast & "' with a password of " & Settings.Password & " to add user accounts.")
            End If
        Else
            If Settings.ApacheEnable Then
                Dim webAddress As String = "http://127.0.0.1:" & CStr(Settings.ApachePort)
                Process.Start(webAddress)
            Else
                Print("Opensim is not running. Cannot open the Web Interface.")
            End If
        End If

    End Sub

    Private Sub AdvancedSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AdvancedSettingsToolStripMenuItem.Click

        If PropInitted Then
            Adv.Activate()
            Adv.Visible = True
        End If

    End Sub

    Private Sub BusyButton_Click(sender As Object, e As EventArgs) Handles BusyButton.Click

        StopAllRegions()

        PropUpdateView = True ' make form refresh
        ' cannot load OAR or IAR, either
        IslandToolStripMenuItem.Visible = False
        ClothingInventoryToolStripMenuItem.Visible = False
        Timer1.Stop()
        PropOpensimIsRunning() = False

        ProgressBar1.Value = 0
        ProgressBar1.Visible = False
        ToolBar(False)

        Print("DreamGrid Stopped/Aborted")
        Buttons(StopButton)
        Timer1.Enabled = False
        PropAborting = True

    End Sub

    Private Sub LoopBackToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoopBackToolStripMenuItem.Click

        Help("Loopback Fixes")

    End Sub

    Private Sub MoreContentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoreContentToolStripMenuItem.Click

        Dim webAddress As String = SecureDomain & "/cgi/freesculpts.plx"
        Process.Start(webAddress)
        Print("Get OAR and IAR files to load into your Sim")

    End Sub

    Private Sub StopAllRegions()

        For Each X As Integer In PropRegionClass.RegionNumbers
            PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Stopped
            PropRegionClass.ProcessID(X) = 0
            PropRegionClass.Timer(X) = RegionMaker.REGIONTIMER.Stopped
        Next
        Try
            PropExitList.Clear()
        Catch
        End Try

    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Dim webAddress As String = SecureDomain() & "/Outworldz_Installer/PortForwarding.htm"
        Process.Start(webAddress)
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        Print("Starting UPnp Control Panel")
        Dim pi As ProcessStartInfo = New ProcessStartInfo With {
                .Arguments = "",
                .FileName = PropMyFolder & "\UPnpPortForwardManager.exe",
                .WindowStyle = ProcessWindowStyle.Normal
            }
        Using ProcessUpnp As Process = New Process With {
                .StartInfo = pi
            }
            Try
                ProcessUpnp.Start()
            Catch ex As ObjectDisposedException
                ErrorLog("ErrorUPnp failed to launch: " & ex.Message)
            Catch ex As InvalidOperationException
                ErrorLog("ErrorUPnp failed to launch: " & ex.Message)
            Catch ex As System.ComponentModel.Win32Exception
                ErrorLog("ErrorUPnp failed to launch: " & ex.Message)
            End Try
        End Using

    End Sub

#End Region

#Region "Apache"

    Public Sub StartApache()

        If Settings.SearchEnabled Then
            Dim SiteMapContents = "<?xml version=""1.0"" encoding=""UTF-8""?>" & vbCrLf
            SiteMapContents += "<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">" & vbCrLf
            SiteMapContents += "<url>" & vbCrLf
            SiteMapContents += "<loc>http://" & Settings.PublicIP & ":" & CStr(Settings.ApachePort) & "/" & "</loc>" & vbCrLf
            SiteMapContents += "<changefreq>daily</changefreq>" & vbCrLf
            SiteMapContents += "<priority>1.0</priority>" & vbCrLf
            SiteMapContents += "</url>" & vbCrLf
            SiteMapContents += "</urlset>" & vbCrLf

            Using outputFile As New StreamWriter(PropMyFolder & "\OutworldzFiles\Apache\htdocs\Sitemap.xml", False)
                outputFile.WriteLine(SiteMapContents)
            End Using
        End If

        If Not Settings.ApacheEnable Then
            ApachePictureBox.Image = My.Resources.nav_plain_blue
            ToolTip1.SetToolTip(ApachePictureBox, "Disabled")
            Print("Apache web server is not enabled.")
            Return
        End If

        ApachePictureBox.Image = My.Resources.nav_plain_red
        ToolTip1.SetToolTip(ApachePictureBox, "Offline")
        Application.DoEvents()

        If Settings.ApachePort = 80 Then
            ApacheProcess.StartInfo.UseShellExecute = True ' so we can redirect streams
            ApacheProcess.StartInfo.FileName = "net"
            ApacheProcess.StartInfo.CreateNoWindow = True
            ApacheProcess.StartInfo.Arguments = "stop W3SVC"
            ApacheProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            ApacheProcess.Start()
            Application.DoEvents()
            ApacheProcess.WaitForExit()
        End If

        Print("Check Apache")
        ' Stop MSFT server if we are on port 80 and enabled

        Dim Running = CheckPort(Settings.PrivateURL, CType(Settings.ApachePort, Integer))
        If Running Then
            Print("Webserver is running")
            ApachePictureBox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(ApachePictureBox, "Apache is running")
            PropApacheExited = False
            Return
        End If
        Application.DoEvents()

        If Settings.ApacheService Then
            Print("Checking Apache service")
            PropApacheUninstalling = True
            ApacheProcess.StartInfo.FileName = "sc"
            ApacheProcess.StartInfo.Arguments = "stop " & "ApacheHTTPServer"
            ApacheProcess.Start()
            Application.DoEvents()
            ApacheProcess.WaitForExit()

            ApacheProcess.StartInfo.Arguments = "stop " & """" & "Apache HTTP Server" & """"
            ApacheProcess.Start()
            Application.DoEvents()
            ApacheProcess.WaitForExit()

            ApacheProcess.StartInfo.FileName = "sc"
            ApacheProcess.StartInfo.Arguments = " delete  " & """" & "Apache HTTP Server" & """"
            ApacheProcess.Start()
            Application.DoEvents()
            ApacheProcess.WaitForExit()

            ApacheProcess.StartInfo.Arguments = " delete  " & "ApacheHTTPServer"
            ApacheProcess.Start()
            Application.DoEvents()
            ApacheProcess.WaitForExit()

            Sleep(4000)

            Try
                Using ApacheProcess As New Process With {
                        .EnableRaisingEvents = True
                    }
                    ApacheProcess.StartInfo.UseShellExecute = True ' so we can redirect streams
                    ApacheProcess.StartInfo.FileName = PropMyFolder & "\Outworldzfiles\Apache\bin\httpd.exe"
                    ApacheProcess.StartInfo.Arguments = "-k install -n " & """" & "ApacheHTTPServer" & """"
                    ApacheProcess.StartInfo.CreateNoWindow = True
                    ApacheProcess.StartInfo.WorkingDirectory = PropMyFolder & "\Outworldzfiles\Apache\bin\"
                    ApacheProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    ApacheProcess.Start()
                    Application.DoEvents()
                    ApacheProcess.WaitForExit()

                    If ApacheProcess.ExitCode <> 0 Then
                        Print("Apache Did not install")
                    Else
                        PropApacheUninstalling = False ' installed now, trap errors
                    End If
                    Sleep(100)
                    Print("Starting Apache web server")
                    ApacheProcess.StartInfo.FileName = "net"
                    ApacheProcess.StartInfo.Arguments = "start " & "ApacheHTTPServer"
                    ApacheProcess.Start()
                    Application.DoEvents()
                    ApacheProcess.WaitForExit()

                    If ApacheProcess.ExitCode <> 0 Then
                        Print("Apache failed to start:" & CStr(ApacheProcess.ExitCode))
                    Else
                        ApachePictureBox.Image = My.Resources.nav_plain_green
                        ToolTip1.SetToolTip(ApachePictureBox, "Webserver is running")
                        Application.DoEvents()
                    End If
                End Using
            Catch ex As Exception
                Print("Apache failed to start:" & ex.Message)
            End Try
        Else

            ' Start Apache manually
            Try
                Dim ApacheProcess2 As New Process With {
                    .EnableRaisingEvents = True
                }
                ApacheProcess2.StartInfo.UseShellExecute = True ' so we can redirect streams
                ApacheProcess2.StartInfo.FileName = PropMyFolder & "\Outworldzfiles\Apache\bin\httpd.exe"
                ApacheProcess2.StartInfo.CreateNoWindow = True
                ApacheProcess2.StartInfo.WorkingDirectory = PropMyFolder & "\Outworldzfiles\Apache\bin\"
                ApacheProcess2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                ApacheProcess2.StartInfo.Arguments = ""
                ApacheProcess2.Start()
                PropgApacheProcessID = ApacheProcess2.Id
            Catch ex As Exception
                Print("Error: Apache did not start: " & ex.Message)
                ErrorLog("Error: Apache did not start: " & ex.Message)
                ApachePictureBox.Image = My.Resources.nav_plain_red
                ToolTip1.SetToolTip(ApachePictureBox, "Webserver did not start")
                Application.DoEvents()
                Return
            End Try

            Application.DoEvents()
            ' Wait for Apache to start listening

            Dim counter = 0
            While PropgApacheProcessID = 0 And PropOpensimIsRunning

                BumpProgress(1)
                counter += 1
                ' wait 10 seconds for it to start
                If counter > 100 Then
                    Print("Error:Apache failed to start")
                    Return
                End If
                Application.DoEvents()
                Sleep(100)

                Dim isRunning = CheckPort(Settings.PrivateURL, CType(Settings.ApachePort, Integer))
                If isRunning Then
                    Print("Apache webserver is running")
                    ApachePictureBox.Image = My.Resources.nav_plain_green
                    ToolTip1.SetToolTip(ApachePictureBox, "Webserver is running")
                    PropApacheExited = False
                    Return
                End If

            End While

        End If

    End Sub

    Private Sub DoApache()

        If Not Settings.ApacheEnable Then Return

        ' lean rightward paths for Apache
        Dim ini = PropMyFolder & "\Outworldzfiles\Apache\conf\httpd.conf"
        Settings.LoadLiteralIni(ini)
        Settings.SetLiteralIni("Listen", "Listen " & CStr(Settings.ApachePort))
        Settings.SetLiteralIni("ServerRoot", "ServerRoot " & """" & PropCurSlashDir & "/Outworldzfiles/Apache" & """")
        Settings.SetLiteralIni("DocumentRoot", "DocumentRoot " & """" & PropCurSlashDir & "/Outworldzfiles/Apache/htdocs" & """")
        Settings.SetLiteralIni("Use VDir", "Use VDir " & """" & PropCurSlashDir & "/Outworldzfiles/Apache/htdocs" & """")
        Settings.SetLiteralIni("PHPIniDir", "PHPIniDir " & """" & PropCurSlashDir & "/Outworldzfiles/PHP7" & """")
        Settings.SetLiteralIni("ServerName", "ServerName " & Settings.PublicIP)
        Settings.SetLiteralIni("ServerAdmin", "ServerAdmin " & Settings.AdminEmail)
        Settings.SetLiteralIni("<VirtualHost", "<VirtualHost  *:" & CStr(Settings.ApachePort) & ">")
        Settings.SetLiteralIni("ErrorLog", "ErrorLog " & """|bin/rotatelogs.exe  -l \" & """" & PropCurSlashDir & "/Outworldzfiles/Apache/logs/Error-%Y-%m-%d.log" & "\" & """" & " 86400""")
        Settings.SetLiteralIni("CustomLog", "CustomLog " & """|bin/rotatelogs.exe -l \" & """" & PropCurSlashDir & "/Outworldzfiles/Apache/logs/access-%Y-%m-%d.log" & "\" & """" & " 86400""" & " common env=!dontlog""")
        ' needed for Php5 upgrade
        Settings.SetLiteralIni("LoadModule php5_module", "LoadModule php7_module")
        Settings.SetLiteralIni("LoadModule php7_module", "LoadModule php7_module " & """" & PropCurSlashDir & "/Outworldzfiles/PHP7/php7apache2_4.dll" & """")

        Settings.SaveLiteralIni(ini, "httpd.conf")

        Try
            Directory.Delete(PropMyFolder & "\Outworldzfiles\PHP5", True)
        Catch ex As DirectoryNotFoundException
        Catch ex As IOException
        Catch ex As UnauthorizedAccessException
        Catch ex As ArgumentException
        End Try

        ' lean rightward paths for Apache
        ini = PropMyFolder & "\Outworldzfiles\Apache\conf\extra\httpd-ssl.conf"
        Settings.LoadLiteralIni(ini)
        Settings.SetLiteralIni("Listen", "Listen " & Settings.PrivateURL & ":" & "443")
        Settings.SetLiteralIni("DocumentRoot", "DocumentRoot " & """" & PropCurSlashDir & "/Outworldzfiles/Apache/htdocs""")
        Settings.SetLiteralIni("ServerName", "ServerName " & Settings.PublicIP)
        Settings.SetLiteralIni("SSLSessionCache", "SSLSessionCache shmcb:""" & PropCurSlashDir & "/Outworldzfiles/Apache/logs" & "/ssl_scache(512000)""")
        Settings.SaveLiteralIni(ini, "httpd-ssl.conf")

    End Sub

    Private Sub DoPHP()

        Dim ini = PropMyFolder & "\Outworldzfiles\PHP7\php.ini"
        Settings.LoadLiteralIni(ini)
        Settings.SetLiteralIni("extension_dir", "extension_dir = " & """" & PropCurSlashDir & "/OutworldzFiles/PHP7/ext""")
        Settings.SetLiteralIni("doc_root", "doc_root = """ & PropCurSlashDir & "/OutworldzFiles/Apache/htdocs""")
        Settings.SaveLiteralIni(ini, "php.ini")

    End Sub

    ''' <summary>
    ''' Check is Apache port 80 or 8000 is up
    ''' </summary>
    ''' <returns>boolean</returns>
    Private Function IsApacheRunning() As Boolean

        Using client As New WebClient ' download client for web pages
            Dim Up As String
            Try
                Up = client.DownloadString("http://" & Settings.PublicIP & ":" & CStr(Settings.ApachePort) & "/?_Opensim=" & RandomNumber.Random)
            Catch ex As ArgumentNullException
                If ex.Message.Contains("200 OK") Then Return True
                Return False
            Catch ex As WebException
                If ex.Message.Contains("200 OK") Then Return True
                Return False
            Catch ex As NotSupportedException
                If ex.Message.Contains("200 OK") Then Return True
                Return False
            End Try
            If Up.Length = 0 And PropOpensimIsRunning() Then
                Return False
            End If

        End Using

        Return True

    End Function

    Private Sub KillApache()

        If Not Settings.ApacheEnable Then Return
        If Settings.ApacheService Then Return

        If Settings.ApacheService Then
            Using ApacheProcess As New Process()
                Print("Stopping Apache ")
                Try
                    ApacheProcess.StartInfo.FileName = "net.exe"
                    ApacheProcess.StartInfo.Arguments = "stop ApacheHTTPServer"
                    ApacheProcess.StartInfo.CreateNoWindow = True
                    ApacheProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    ApacheProcess.Start()
                    ApacheProcess.WaitForExit()
                    Dim code = ApacheProcess.ExitCode
                    If code <> 0 Then
                        Log("Info", "No Apache to stop")
                    End If
                Catch ex As Exception
                    Print("Error Apache did Not stop" & ex.Message)

                End Try
            End Using
        Else
            Zap("httpd")
            Zap("rotatelogs")
        End If

    End Sub

    Private Sub MapSetup()

        Dim phptext = "<?php " & vbCrLf &
"/* General Domain */" & vbCrLf &
"$CONF_domain        = " & """" & Settings.PublicIP & """" & "; " & vbCrLf &
"$CONF_port          = " & """" & Settings.HttpPort & """" & "; " & vbCrLf &
"$CONF_sim_domain    = " & """" & "http://" & Settings.PublicIP & "/" & """" & ";" & vbCrLf &
"$CONF_install_path  = " & """" & "/Metromap" & """" & ";   // Installation path " & vbCrLf &
"/* MySQL Database */ " & vbCrLf &
"$CONF_db_server     = " & """" & Settings.RobustServer & """" & "; // Address Of Robust Server " & vbCrLf &
"$CONF_db_port       = " & """" & CStr(Settings.MySqlRobustDBPort) & """" & "; // Robust port " & vbCrLf &
"$CONF_db_user       = " & """" & Settings.RobustUsername & """" & ";  // login " & vbCrLf &
"$CONF_db_pass       = " & """" & Settings.RobustPassword & """" & ";  // password " & vbCrLf &
"$CONF_db_database   = " & """" & Settings.RobustDataBaseName & """" & ";     // Name Of Robust Server " & vbCrLf &
"/* The Coordinates Of the Grid-Center */ " & vbCrLf &
"$CONF_center_coord_x = " & """" & CStr(Settings.MapCenterX) & """" & ";		// the Center-X-Coordinate " & vbCrLf &
"$CONF_center_coord_y = " & """" & CStr(Settings.MapCenterY) & """" & ";		// the Center-Y-Coordinate " & vbCrLf &
"// style-sheet items" & vbCrLf &
"$CONF_style_sheet     = " & """" & "/css/stylesheet.css" & """" & ";          //Link To your StyleSheet" & vbCrLf &
"?>"

        Using outputFile As New StreamWriter(PropMyFolder & "\OutworldzFiles\Apache\htdocs\MetroMap\includes\config.php", False)
            outputFile.WriteLine(phptext)
        End Using

        phptext = "<?php " & vbCrLf &
"$DB_GRIDNAME = " & """" & Settings.PublicIP & ":" & Settings.HttpPort & """" & ";" & vbCrLf &
"$DB_HOST = " & """" & Settings.RobustServer & """" & ";" & vbCrLf &
"$DB_PORT = " & """" & CStr(Settings.MySqlRobustDBPort) & """" & "; // Robust port " & vbCrLf &
"$DB_USER = " & """" & Settings.RobustUsername & """" & ";" & vbCrLf &
"$DB_PASSWORD = " & """" & Settings.RobustPassword & """" & ";" & vbCrLf &
"$DB_NAME = " & """" & "ossearch" & """" & ";" & vbCrLf &
"?>"

        Using outputFile As New StreamWriter(PropMyFolder & "\OutworldzFiles\Apache\htdocs\Search\databaseinfo.php", False)
            outputFile.WriteLine(phptext)
        End Using
        Using outputFile As New StreamWriter(PropMyFolder & "\OutworldzFiles\PHP7\databaseinfo.php", False)
            outputFile.WriteLine(phptext)
        End Using

    End Sub

    Private Sub StopApache()

        If Not Settings.ApacheEnable Then Return

        If Not Settings.ApacheService Then
            Print("Stopping Apache ")
            Zap("httpd")
            Zap("rotatelogs")
            ApachePictureBox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(ApachePictureBox, "Stopped")
        End If

    End Sub

#End Region

#Region "Icecast"

    Public Sub StartIcecast()

        If Not Settings.SCEnable Then
            IceCastPicturebox.Image = My.Resources.nav_plain_blue
            ToolTip1.SetToolTip(IceCastPicturebox, "Icecast is disabled")
            Return
        End If

        Dim IceCastRunning = CheckPort(Settings.PublicIP, Settings.SCPortBase)
        If IceCastRunning Then
            IceCastPicturebox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(IceCastPicturebox, "Icecast is running")
            Return
        End If

        FileStuff.DeleteFile(PropMyFolder & "\Outworldzfiles\Icecast\log\access.log")

        FileStuff.DeleteFile(PropMyFolder & "\Outworldzfiles\Icecast\log\error.log")

        PropIcecastProcID = 0
        Print("Starting Icecast")

        Try
            IcecastProcess.EnableRaisingEvents = True
            IcecastProcess.StartInfo.UseShellExecute = True ' so we can redirect streams
            IcecastProcess.StartInfo.FileName = PropMyFolder & "\Outworldzfiles\icecast\icecast.bat"
            IcecastProcess.StartInfo.CreateNoWindow = False
            IcecastProcess.StartInfo.WorkingDirectory = PropMyFolder & "\Outworldzfiles\icecast"

            If Settings.ConsoleShow Then
                IcecastProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal
            Else
                IcecastProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
            End If

            'IcecastProcess.StartInfo.Arguments = "-c .\icecast_run.xml"
            IcecastProcess.Start()
            PropIcecastProcID = IcecastProcess.Id

            SetWindowTextCall(IcecastProcess, "Icecast")
            ShowDOSWindow(IcecastProcess.MainWindowHandle, SHOWWINDOWENUM.SWMINIMIZE)
        Catch ex As Exception
            Print("Error: Icecast did not start: " & ex.Message)
            ErrorLog("Error: Icecast did not start: " & ex.Message)
            IceCastPicturebox.Image = My.Resources.nav_plain_red
            ToolTip1.SetToolTip(IceCastPicturebox, "Icecast Failed to start")
            Return
        End Try

        IceCastPicturebox.Image = My.Resources.nav_plain_green
        ToolTip1.SetToolTip(IceCastPicturebox, "Icecast is running")

        PropIceCastExited = False

    End Sub

#End Region

#Region "Robust"

    Public Function StartRobust() As Boolean

        If IsRobustRunning() Then
            RobustPictureBox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(RobustPictureBox, "Robust is running")

            For Each p In Process.GetProcesses
                If p.MainWindowTitle = "Robust" Then
                    PropRobustProcID = p.Id
                    Return True
                End If
            Next
        End If

        RobustPictureBox.Image = My.Resources.nav_plain_blue
        ToolTip1.SetToolTip(RobustPictureBox, "Robust is Off")
        If Settings.ServerType <> "Robust" Then Return True

        If Settings.RobustServer <> "127.0.0.1" And Settings.RobustServer <> "localhost" Then
            Print("Using Robust on " & Settings.RobustServer)
            RobustPictureBox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(RobustPictureBox, "Robust is running")
            Return True
        End If

        PropRobustProcID = Nothing
        Print("Starting Robust")

        Try
            RobustProcess.EnableRaisingEvents = True
            RobustProcess.StartInfo.UseShellExecute = True ' so we can redirect streams
            RobustProcess.StartInfo.FileName = PropOpensimBinPath & "bin\robust.exe"

            RobustProcess.StartInfo.CreateNoWindow = False
            RobustProcess.StartInfo.WorkingDirectory = PropOpensimBinPath & "bin"
            RobustProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal
            RobustProcess.StartInfo.Arguments = "-inifile Robust.HG.ini"
            RobustProcess.Start()
            PropRobustProcID = RobustProcess.Id
            SetWindowTextCall(RobustProcess, "Robust")
        Catch ex As Exception
            Print("Error: Robust did not start: " & ex.Message)
            ErrorLog("Error: Robust did not start: " & ex.Message)
            KillAll()
            Buttons(StartButton)
            RobustPictureBox.Image = My.Resources.nav_plain_red
            ToolTip1.SetToolTip(RobustPictureBox, "Robust did not start")
            Return False
        End Try

        ' Wait for Robust to start listening

        Dim counter = 0
        While Not IsRobustRunning() And PropOpensimIsRunning
            Application.DoEvents()
            BumpProgress(1)
            counter += 1
            ' wait a minute for it to start
            If counter > 100 Then
                Print("Error:Robust failed to start")
                Buttons(StartButton)
                Dim yesno = MsgBox("Robust did not start. Do you want to see the log file?", vbYesNo, "Error")
                If (yesno = vbYes) Then
                    Dim Log As String = """" & PropOpensimBinPath & "bin\Robust.log" & """"
                    System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe " & Log)
                End If
                Buttons(StartButton)
                RobustPictureBox.Image = My.Resources.nav_plain_red
                ToolTip1.SetToolTip(RobustPictureBox, "Robust did not start")
                Return False
            End If
            Application.DoEvents()
            Sleep(100)

        End While

        If Settings.ConsoleShow = False Then
            ShowDOSWindow(GetHwnd("Robust"), SHOWWINDOWENUM.SWMINIMIZE)
        End If
        RobustPictureBox.Image = My.Resources.nav_plain_green
        Log("Info", "Robust is running")
        ToolTip1.SetToolTip(RobustPictureBox, "Robust is running")

        PropRobustExited = False

        Application.DoEvents()
        Return True

    End Function

#End Region

#Region "Opensimulator"

    Public Function StartOpensimulator(Optional SkipSmartStart As Boolean = False) As Boolean

        PropExitHandlerIsBusy = False
        PropAborting = False
        Timer1.Start() 'Timer starts functioning

        StartRobust()

        Dim Len = PropRegionClass.RegionCount()
        Dim counter = 1
        ProgressBar1.Value = CType(counter / Len, Integer)

        ' Boot them up
        For Each X As Integer In PropRegionClass.RegionNumbers()
            If PropRegionClass.RegionEnabled(X) Then
                Boot(PropRegionClass, PropRegionClass.RegionName(X), SkipSmartStart)
                ProgressBar1.Value = CType(counter / Len * 100, Integer)
                counter += 1
            End If
        Next

        Return True

    End Function

#End Region

#Region "Exited"

    ' Handle Exited event and display process information.
    Private Sub ApacheProcess_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles ApacheProcess.Exited

        If PropAborting Then Return
        If PropApacheUninstalling Then Return

        If Settings.RestartOnCrash And _ApacheCrashCounter < 10 Then
            _ApacheCrashCounter += 1
            PropApacheExited = True
            Return
        End If
        _ApacheCrashCounter = 0
        PropgApacheProcessID = Nothing

        Dim yesno = MsgBox("Apache quit after 10 retries. Do you want to see the error log file?", vbYesNo, "Error")
        If (yesno = vbYes) Then
            Dim Apachelog As String = PropMyFolder & "\Outworldzfiles\Apache\logs\error*.log"
            System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & Apachelog & """")
        End If

    End Sub

    Private Sub IceCast_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles IcecastProcess.Exited

        If PropAborting Then Return

        If Settings.RestartOnCrash And _IcecastCrashCounter < 10 Then
            _IcecastCrashCounter += 1
            PropIceCastExited = True
            Return
        End If
        _IcecastCrashCounter = 0

        Dim yesno = MsgBox("Icecast quit after 10 retries. Do you want to see the error log file?", vbYesNo, "Error")

        If (yesno = vbYes) Then
            Dim IceCastLog As String = PropMyFolder & "\Outworldzfiles\Icecast\log\error.log"
            System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & IceCastLog & """")
        End If

    End Sub

    Private Sub Mysql_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles ProcessMySql.Exited

        If PropAborting Then Return

        If Settings.RestartOnCrash And _MysqlCrashCounter < 10 Then
            _MysqlCrashCounter += 1
            PropMysqlExited = True
            Return
        End If
        _MysqlCrashCounter = 0

        Dim yesno = MsgBox("Mysql quit after 10 retries. Do you want to see the error log file?", vbYesNo, "Error")
        If (yesno = vbYes) Then
            Dim MysqlLog As String = PropMyFolder & "\OutworldzFiles\mysql\data"
            Dim files() As String
            files = Directory.GetFiles(MysqlLog, "*.err", SearchOption.TopDirectoryOnly)
            For Each FileName As String In files
                System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & FileName & """")
            Next
        End If
    End Sub

    ' Handle Exited event and display process information.
    Private Sub RobustProcess_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles RobustProcess.Exited

        PropRobustProcID = Nothing
        If PropAborting Then Return

        If PropRestartRobust Then
            PropRobustExited = True
            Return
        End If

        If Settings.RestartOnCrash And _RobustCrashCounter < 10 Then
            PropRobustExited = True
            _RobustCrashCounter += 1
            Return
        End If
        _RobustCrashCounter = 0

        Dim yesno = MsgBox("Robust exited after 10 retries. Do you want to see the error log file?", vbYesNo, "Error")
        If (yesno = vbYes) Then
            Dim MysqlLog As String = PropOpensimBinPath & "bin\Robust.log"
            System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & MysqlLog & """")
        End If

    End Sub

#End Region

#Region "Boot"

    ''' <summary>
    ''' Starts Opensim for a given name
    ''' </summary>
    ''' <param name="BootName">Name of region to start</param>
    ''' <returns>success = true</returns>
    Public Function Boot(Regionclass As RegionMaker, BootName As String, Optional SkipSmartStart As Boolean = False) As Boolean
        If Regionclass Is Nothing Then Return False
        If RegionMaker.Instance Is Nothing Then
            ErrorLog("Tried to start a region but there is no regionclass!")
            Return False
        End If

        If PropAborting Then Return True

        PropOpensimIsRunning() = True

        Buttons(StopButton)

        Dim RegionNumber = Regionclass.FindRegionByName(BootName)
        If Regionclass.SmartStart(RegionNumber) = "True" And Settings.SmartStart And Not SkipSmartStart Then
            Print("Smart Start " & BootName)
            Return True
        End If

        Log("Info", "Region: Starting Region " & BootName)

        If Regionclass.IsBooted(RegionNumber) Then
            Log("Info", "Region " & BootName & " already running")
            Return True
        End If

        If Regionclass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.RecyclingUp Then
            Log("Info", "Region " & BootName & " skipped as it is already Warming Up")
            Return True
        End If

        If Regionclass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.Booting Then
            Log("Info", "Region " & BootName & " skipped as it is already Booted Up")
            Return True
        End If

        If Regionclass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.ShuttingDown Then
            Log("Info", "Region " & BootName & " skipped as it is already Shutting Down")
            Return True
        End If

        If Regionclass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.RecyclingDown Then
            Log("Info", "Region " & BootName & " skipped as it is already Recycling Down")
            Return True
        End If

        Application.DoEvents()
        Dim isRegionRunning = CheckPort("127.0.0.1", Regionclass.GroupPort(RegionNumber))
        If isRegionRunning Then
            Print(BootName & " is already running")
            Dim listP = Process.GetProcesses

            For Each p In listP
                If p.MainWindowTitle = Regionclass.GroupName(RegionNumber) Then
                    Regionclass.ProcessID(RegionNumber) = p.Id
                End If
            Next

            Regionclass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.Booted ' force it up
            Return False
        End If

        Environment.SetEnvironmentVariable("OSIM_LOGPATH", Settings.OpensimBinPath() & "bin\Regions\" & PropRegionClass.GroupName(RegionNumber))

        Dim myProcess As Process = GetNewProcess()
        Dim Groupname = Regionclass.GroupName(RegionNumber)
        Print("Starting " & Groupname)

        myProcess.EnableRaisingEvents = True
        myProcess.StartInfo.UseShellExecute = True ' so we can redirect streams
        myProcess.StartInfo.WorkingDirectory = Settings.OpensimBinPath() & "bin"

        myProcess.StartInfo.FileName = """" & Settings.OpensimBinPath() & "bin\OpenSim.exe" & """"
        myProcess.StartInfo.CreateNoWindow = False
        myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        myProcess.StartInfo.Arguments = " -inidirectory=" & """" & "./Regions/" & Regionclass.GroupName(RegionNumber) & """"

        FileStuff.DeleteFile(Settings.OpensimBinPath() & "bin\Regions\" & Regionclass.GroupName(RegionNumber) & "\Opensim.log")
        FileStuff.DeleteFile(Settings.OpensimBinPath() & "bin\Regions\" & Regionclass.GroupName(RegionNumber) & "\PID.pid")
        FileStuff.DeleteFile(Settings.OpensimBinPath() & "bin\regions\" & Regionclass.GroupName(RegionNumber) & "\OpensimConsole.log")
        FileStuff.DeleteFile(Settings.OpensimBinPath() & "bin\regions\" & Regionclass.GroupName(RegionNumber) & "\OpenSimStats.log")

        If myProcess.Start() Then

            Dim hasPID As Boolean = False
            Dim TooMany As Integer = 0
            Do While Not hasPID And TooMany < 100
                Try
                    Sleep(100)
                    TooMany += 1
                    Dim p As Process = Process.GetProcessById(myProcess.Id)
                    If p.ProcessName.Length > 0 Then
                        hasPID = True
                    End If
                Catch ex As ArgumentException
                Catch ex As InvalidOperationException
                End Try
            Loop

            For Each num In Regionclass.RegionListByGroupNum(Groupname)
                Log("Debug", "Process started for " & Regionclass.RegionName(num) & " PID=" & CStr(myProcess.Id) & " Num:" & CStr(num))
                Regionclass.Status(num) = RegionMaker.SIMSTATUSENUM.Booting
                Regionclass.ProcessID(num) = myProcess.Id
                Regionclass.Timer(num) = RegionMaker.REGIONTIMER.StartCounting
            Next

            PropUpdateView = True ' make form refresh
            Application.DoEvents()
            SetWindowTextCall(myProcess, Regionclass.GroupName(RegionNumber))

            Log("Debug", "Created Process Number " & CStr(myProcess.Id) & " in  RegionHandles(" & CStr(PropRegionHandles.Count) & ") " & "Group:" & Groupname)
            PropRegionHandles.Add(myProcess.Id, Groupname) ' save in the list of exit events in case it crashes or exits

            SequentialPause()

        End If
        Return True

    End Function

    Public Sub ForceStopGroup(Groupname As String)

        For Each RegionNumber In PropRegionClass.RegionListByGroupNum(Groupname)

            ' Called by a sim restart, do not change status
            'If Not PropRegionClass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.RecyclingDown Then
            PropRegionClass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.Stopped
            Log("Info", PropRegionClass.RegionName(RegionNumber) & " Stopped")
            ' End If

            PropRegionClass.Timer(RegionNumber) = RegionMaker.REGIONTIMER.Stopped
        Next
        Log("Info", Groupname & " Group is now stopped")
        PropUpdateView = True ' make form refresh

    End Sub

    ''' <summary>
    ''' Creates and exit handler for each region
    ''' </summary>
    ''' <returns>a process handle</returns>
    Public Function GetNewProcess() As Process

        Dim handle = New Handler
        Return handle.Init(PropRegionHandles, PropExitList)

    End Function

    Public Sub StopGroup(Groupname As String)

        For Each RegionNumber In PropRegionClass.RegionListByGroupNum(Groupname)
            ' Called by a sim restart, do not change status
            'If Not PropRegionClass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.RecyclingDown Then
            PropRegionClass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.Stopped
            Log("Info", PropRegionClass.RegionName(RegionNumber) & " Stopped")
            'End If
            PropRegionClass.Timer(RegionNumber) = RegionMaker.REGIONTIMER.Stopped
        Next
        Log("Info", Groupname & " Group is now stopped")
        PropUpdateView = True ' make form refresh

    End Sub

#End Region

#Region "ExitHandlers"

    Private Sub ExitHandlerPoll()

        ' background process to scan for things to do.
        If PropExitHandlerIsBusy Then Return ' not reentrant

        If PropAborting Then Return ' not if we are aborting

        If PropRestartRobust And PropRobustExited = True Then
            StartRobust()
        End If
        ' From the cross-threaded exited function.  These can only be set if Settings.RestartOnCrash is true
        If PropMysqlExited Then
            StartMySQL()
        End If
        If PropRobustExited Then
            StartRobust()
        End If
        If PropApacheExited Then
            StartApache()
        End If
        If PropIceCastExited Then
            StartIcecast()
        End If

        Dim GroupName As String
        Dim RegionNumber As Integer
        Dim TimerValue As Integer

        For Each X As Integer In PropRegionClass.RegionNumbers

            ' count up to auto restart , when high enough, restart the sim
            If PropRegionClass.Timer(X) >= 0 Then
                PropRegionClass.Timer(X) = PropRegionClass.Timer(X) + 1
            End If

            If PropOpensimIsRunning() And Not PropAborting And PropRegionClass.Timer(X) >= 0 Then
                TimerValue = PropRegionClass.Timer(X)
                ' if it is past time and no one is in the sim...
                GroupName = PropRegionClass.GroupName(X)

                ' Smart shutdown
                If PropRegionClass.SmartStart(X) = "True" And Settings.SmartStart And (TimerValue * 6) >= 60 And Not AvatarsIsInGroup(GroupName) Then
                    SequentialPause()
                    ConsoleCommand(PropRegionClass.GroupName(X), "q{ENTER}" & vbCrLf)
                    Print("Smart Stop " & GroupName)
                    ' shut down all regions in the DOS box
                    For Each Y In PropRegionClass.RegionListByGroupNum(GroupName)
                        PropRegionClass.Timer(Y) = RegionMaker.REGIONTIMER.Stopped
                        PropRegionClass.Status(Y) = RegionMaker.SIMSTATUSENUM.ShuttingDown
                    Next
                    PropUpdateView = True ' make form refresh
                End If

                If (TimerValue / 12) >= (Settings.AutoRestartInterval()) _
                    And Settings.AutoRestartInterval() > 0 _
                    And Not AvatarsIsInGroup(GroupName) _
                    And PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Booted Then
                    ' shut down the group when one minute has gone by, or multiple thereof.
                    Try
                        If ShowDOSWindow(GetHwnd(GroupName), SHOWWINDOWENUM.SWRESTORE) Then
                            SequentialPause()
                            ConsoleCommand(PropRegionClass.GroupName(X), "q{ENTER}" & vbCrLf)
                            Print("AutoRestarting " & GroupName)
                            ' shut down all regions in the DOS box
                            For Each Y In PropRegionClass.RegionListByGroupNum(GroupName)
                                PropRegionClass.Timer(Y) = RegionMaker.REGIONTIMER.Stopped
                                PropRegionClass.Status(Y) = RegionMaker.SIMSTATUSENUM.RecyclingDown
                            Next
                        Else
                            ' shut down all regions in the DOS box
                            For Each Y In PropRegionClass.RegionListByGroupNum(GroupName)
                                PropRegionClass.Timer(Y) = RegionMaker.REGIONTIMER.Stopped
                                PropRegionClass.Status(Y) = RegionMaker.SIMSTATUSENUM.Stopped
                            Next
                        End If
                        PropUpdateView = True ' make form refresh
                    Catch ex As Exception
                        ErrorLog(ex.Message)
                        ' shut down all regions in the DOS box
                        For Each Y In PropRegionClass.RegionListByGroupNum(GroupName)
                            PropRegionClass.Timer(Y) = RegionMaker.REGIONTIMER.Stopped
                            PropRegionClass.Status(Y) = RegionMaker.SIMSTATUSENUM.RecyclingDown
                        Next
                    End Try
                End If

            End If

            ' if a restart is signaled, boot it up
            If PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Autostart And Not PropAborting Then
                PropUpdateView = True
                Boot(PropRegionClass, PropRegionClass.RegionName(X), True)
                PropUpdateView = True
            End If

            ' if a restart is signaled, boot it up
            If PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.RestartPending And Not PropAborting Then
                PropUpdateView = True
                Boot(PropRegionClass, PropRegionClass.RegionName(X))
                PropUpdateView = True
            End If

        Next

        PropRestartNow = False
        If PropExitList.Count = 0 Then Return
        If PropExitHandlerIsBusy Then Return
        PropExitHandlerIsBusy = True

        Dim RegionName = PropExitList(0).ToString()
        PropExitList.RemoveAt(0)

        Print(RegionName & " shutdown")
        Dim RegionList = PropRegionClass.RegionListByGroupNum(RegionName)
        ' Need a region number and a Name. Name is either a region or a Group. For groups we need to
        ' get a region name from the group
        GroupName = RegionName ' assume a group
        RegionNumber = PropRegionClass.FindRegionByName(RegionName)

        If RegionNumber >= 0 Then
            GroupName = PropRegionClass.GroupName(RegionNumber) ' Yup, Get Name of the Dos box
        Else
            ' Nope, grab the first region, Group name is already set
            RegionNumber = RegionList(0)
        End If

        Dim Status = PropRegionClass.Status(RegionNumber)
        TimerValue = PropRegionClass.Timer(RegionNumber)

        'Auto restart phase begins
        If PropOpensimIsRunning() And Status = RegionMaker.SIMSTATUSENUM.RecyclingDown Then
            Print("Restart Queued for " & GroupName)
            For Each R In RegionList
                PropRegionClass.Status(R) = RegionMaker.SIMSTATUSENUM.RestartPending
            Next
            PropUpdateView = True ' make form refresh
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' Maybe we crashed during warm up or running. Skip prompt if auto restart on crash and
        ' restart the beast
        If (Status = RegionMaker.SIMSTATUSENUM.RecyclingUp _
            Or Status = RegionMaker.SIMSTATUSENUM.Booting) _
            Or PropRegionClass.IsBooted(RegionNumber) _
            And TimerValue >= 0 Then

            If Settings.RestartOnCrash Then
                PropUpdateView = True
                ' shut down all regions in the DOS box
                Print("DOS Box " & GroupName & " quit unexpectedly.  Restarting now...")
                For Each Y In PropRegionClass.RegionListByGroupNum(GroupName)
                    PropRegionClass.Timer(Y) = RegionMaker.REGIONTIMER.Stopped
                    PropRegionClass.Status(Y) = RegionMaker.SIMSTATUSENUM.RestartPending
                Next
            Else
                Dim yesno = MsgBox("DOS Box " & GroupName & " quit unexpectedly. Do you want to see the log file?", vbYesNo, "Error")
                If (yesno = vbYes) Then
                    System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & PropRegionClass.IniPath(RegionNumber) & "Opensim.log" & """")
                End If
                StopGroup(GroupName)
                PropUpdateView = True
            End If

        End If

        If Status = RegionMaker.SIMSTATUSENUM.ShuttingDown Then
            PropRegionClass.Status(RegionNumber) = RegionMaker.SIMSTATUSENUM.Stopped
            PropUpdateView = True
        End If

        PropExitHandlerIsBusy = False

    End Sub

    ''' <summary>
    ''' Check is Robust port 8002 is up
    ''' </summary>
    ''' <returns>boolean</returns>
    Private Function IsRobustRunning() As Boolean

        Using client As New WebClient ' download client for web pages
            Dim Up As String
            Try
                Up = client.DownloadString("http://" & Settings.RobustServer & ":" & Settings.HttpPort & "/?_Opensim=" & RandomNumber.Random())
            Catch ex As ArgumentNullException
                If ex.Message.Contains("404") Then Return True
                Return False
            Catch ex As WebException
                If ex.Message.Contains("404") Then Return True
                Return False
            Catch ex As NotSupportedException
                If ex.Message.Contains("404") Then Return True
                Return False
            End Try

            If Up.Length = 0 And PropOpensimIsRunning() Then
                Return False
            End If
        End Using
        Return True

    End Function

#End Region

#Region "Logging"

    Public Sub ErrorLog(message As String)
        Logger("Error", message, "Error")
    End Sub

    ''' <summary>
    ''' Log(string) to Outworldz.log
    ''' </summary>
    ''' <param name="message"></param>
    Public Sub Log(category As String, message As String)
        Logger(category, message, "Outworldz")
    End Sub

    Public Sub Logger(category As String, message As String, file As String)
        Try
            Using outputFile As New StreamWriter(PropMyFolder & "\OutworldzFiles\" & file & ".log", True)
                outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", Invarient) & ":" & category & ":" & message)
                Diagnostics.Debug.Print(message)
            End Using
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Deletes old log files
    ''' </summary>
    Private Sub ClearLogFiles()

        Dim Logfiles = New List(Of String) From {
            PropMyFolder & "\OutworldzFiles\Error.log",
            PropMyFolder & "\OutworldzFiles\Outworldz.log",
            PropMyFolder & "\OutworldzFiles\Opensim\bin\OpenSimConsoleHistory.txt",
            PropMyFolder & "\OutworldzFiles\Diagnostics.log",
            PropMyFolder & "\OutworldzFiles\UPnp.log",
            PropMyFolder & "\OutworldzFiles\Opensim\bin\Robust.log",
            PropMyFolder & "\OutworldzFiles\http.log",
            PropMyFolder & "\OutworldzFiles\PHPLog.log",
            PropMyFolder & "\http.log"      ' an old mistake
        }

        For Each thing As String In Logfiles
            ' clear out the log files
            FileStuff.DeleteFile(thing)
        Next

    End Sub

    ''' <summary>
    ''' Shows the log buttons if diags fail
    ''' </summary>
    Private Sub ShowLog()

        System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & PropMyFolder & "\OutworldzFiles\Outworldz.log" & """")

    End Sub

#End Region

#Region "Subs"

    ''' <summary>
    ''' Sleep(ms)
    ''' </summary>
    ''' <param name="value">millseconds</param>
    Public Shared Sub Sleep(value As Integer)

        ' value is in milliseconds, but we do it in 10 passes so we can doevents() to free up console
        Dim sleeptime = value / 10  ' now in tenths
        Dim counter = 10
        While counter > 0
            Application.DoEvents()
            Thread.Sleep(CType(sleeptime, Integer))
            counter -= 1
        End While

    End Sub

    ''' <summary>
    ''' Sends keystrokes to Opensim. Always sends and enter button before to clear and use keys
    ''' </summary>
    ''' <param name="ProcessID">PID of the DOS box</param>
    ''' <param name="command">String</param>
    ''' <returns></returns>
    Public Function ConsoleCommand(name As String, command As String) As Boolean

        If command Is Nothing Then Return False
        If command.Length > 0 Then

            Dim PID As Integer
            If name <> "Robust" Then

                Dim X As List(Of Integer) = PropRegionClass.RegionListByGroupNum(name)

                PID = PropRegionClass.ProcessID(X(0))
                Try
                    If PID >= 0 Then ShowDOSWindow(Process.GetProcessById(PID).MainWindowHandle, SHOWWINDOWENUM.SWRESTORE)
                Catch ex As Exception
                    Diagnostics.Debug.Print("Catch:" & ex.Message)
                    Return False
                End Try
            Else
                PID = PropRobustProcID
                Try
                    ShowDOSWindow(Process.GetProcessById(PID).MainWindowHandle, SHOWWINDOWENUM.SWRESTORE)
                Catch ex As Exception
                    Diagnostics.Debug.Print("Catch:" & ex.Message)
                    Return False
                End Try
            End If

            Try
                'plus sign(+), caret(^), percent sign (%), tilde (~), And parentheses ()
                command = command.Replace("+", "{+}")
                command = command.Replace("^", "{^}")
                command = command.Replace("%", "{%}")
                command = command.Replace("(", "{(}")
                command = command.Replace(")", "{)}")

                AppActivate(PID)
                SendKeys.SendWait(SendableKeys("{ENTER}" & vbCrLf))
                SendKeys.SendWait(SendableKeys(command))
            Catch ex As Exception
                ' ErrorLog("Error:" & ex.Message)
                Diagnostics.Debug.Print("Cannot find window " & name)
                'PropRegionClass.RegionDump()
                Me.Focus()
                Return False

            End Try
            Me.Focus()
            'Application.DoEvents()
        End If

        Return True

    End Function

    Public Function GetHwnd(Groupname As String) As IntPtr

        If Groupname = "Robust" Then
            Dim h As IntPtr
            Try
                h = RobustProcess.MainWindowHandle
            Catch ex As Exception
                h = IntPtr.Zero
            End Try
            Return h
        End If

        Dim Regionlist = PropRegionClass.RegionListByGroupNum(Groupname)

        For Each X As Integer In Regionlist
            Dim pid = PropRegionClass.ProcessID(X)

            Dim ctr = 20   ' 2 seconds
            Dim found As Boolean = False
            While Not found And ctr > 0
                Sleep(100)

                For Each pList As Process In Process.GetProcesses()
                    If pList.Id = pid Then
                        Return pList.MainWindowHandle
                    End If
                    Application.DoEvents()
                    Application.DoEvents()
                    ctr -= 1
                Next
            End While
        Next
        Return IntPtr.Zero

    End Function

    ''' <summary>
    ''' SetWindowTextCall is here to wrap the SetWindowtext API call. This call fails when there is
    ''' no hwnd as Windows takes its sweet time to get that. Also, may fail to write the title. It
    ''' has a timer to make sure we do not get stuck
    ''' </summary>
    ''' <param name="hwnd">Handle to the window to change the text on</param>
    ''' <param name="windowName">the name of the Window</param>
    Public Function SetWindowTextCall(myProcess As Process, windowName As String) As Boolean

        If myProcess Is Nothing Then
            Return False
        End If

        Dim WindowCounter As Integer = 0
        Try
            While myProcess.MainWindowHandle = CType(0, IntPtr)
                Sleep(100)
                WindowCounter += 1
                If WindowCounter > 200 Then '  20 seconds for process to start
                    ErrorLog("Cannot get MainWindowHandle for " & windowName)
                    Return False
                End If
            End While
        Catch ex As PlatformNotSupportedException
            Return False
        Catch ex As InvalidOperationException
            Return False
        Catch ex As NotSupportedException
            Return False
        End Try

        WindowCounter = 0

        Dim hwnd As IntPtr = myProcess.MainWindowHandle
        If CType(hwnd, Integer) = 0 Then
            ErrorLog("hwnd = 0")
        End If
        Dim status = False
        While status = False
            Sleep(100)
            SetWindowText(hwnd, windowName)
            status = NativeMethods.SetWindowText(hwnd, windowName)
            WindowCounter += 1
            If WindowCounter > 600 Then '  60 seconds
                ErrorLog("Cannot get handle for " & windowName)
                Exit While
            End If
            Application.DoEvents()
        End While
        Return True

    End Function

    'Private Sub Chart1_Customize(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChartWrapper1.TheChart.Customize

    ' ChartWrapper1.ChartAreas(0).AxisY.CustomLabels.RemoveAt(2) 'Will remove the third label

    'End Sub

    ''' <summary>
    ''' query MySQL to find any avatars in the DOS box so we can stop it, or not
    ''' </summary>
    ''' <param name="groupname"></param>
    ''' <returns></returns>
    Private Function AvatarsIsInGroup(groupname As String) As Boolean

        Dim present As Integer = 0
        For Each RegionNum As Integer In PropRegionClass.RegionListByGroupNum(groupname)
            present += PropRegionClass.AvatarCount(RegionNum)
        Next

        Return CType(present, Boolean)

    End Function

    Private Sub BumpProgress(bump As Integer)

        Dim nextval As Integer = ProgressBar1.Value + bump
        If nextval > 100 Then
            nextval = 100
        End If
        ProgressBar1.Value = nextval

        Application.DoEvents()

    End Sub

    Private Sub BumpProgress10()

        Dim nextval As Integer = ProgressBar1.Value + 10
        If nextval > 100 Then
            nextval = 100
        End If
        ProgressBar1.Value = nextval
        Application.DoEvents()

    End Sub

    Private Sub Chart()
        ' Graph https://github.com/sinairv/MSChartWrapper
        Try
            ' running average
            speed3 = speed2
            speed2 = speed1
            speed1 = speed
            speed = cpu.NextValue()

            Dim newspeed As Double = (speed + speed1 + speed2 + speed3) / 4

            Dim i = 180
            While i >= 0
                MyCPUCollection(i + 1) = MyCPUCollection(i)
                i -= 1
            End While

            MyCPUCollection(0) = newspeed
            PercentCPU.Text = String.Format(Invarient, "{0: 0}% CPU", newspeed)
        Catch ex As Exception
            ErrorLog(ex.Message)
        End Try

        ''reverse series

        ChartWrapper1.ClearChart()
        ChartWrapper1.AddLinePlot("CPU", MyCPUCollection)

        'RAM

        Dim wql As ObjectQuery = New ObjectQuery("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem")
        Dim searcher As ManagementObjectSearcher = New ManagementObjectSearcher(wql)
        Dim results As ManagementObjectCollection = searcher.Get()
        searcher.Dispose()

        Try
            For Each result In results
                Dim value = ((result("TotalVisibleMemorySize") - result("FreePhysicalMemory")) / result("TotalVisibleMemorySize")) * 100

                Dim j = 180
                While j >= 0
                    MyRAMCollection(j + 1) = MyRAMCollection(j)
                    j -= 1
                End While
                MyRAMCollection(0) = CDbl(value)
                value = Math.Round(value)
                PercentRAM.Text = CStr(value) & "% RAM"
            Next
        Catch ex As Exception
            Log("Error", ex.Message)
        End Try

        ChartWrapper2.ClearChart()
        ChartWrapper2.AddLinePlot("RAM", MyRAMCollection)

    End Sub

    '' makes a list of teleports for the prims to use
    Private Sub RegionListHTML()

        'http://localhost:8002/bin/data/teleports.htm
        'Outworldz|Welcome||www.outworldz.com:9000:Welcome|128,128,96|
        '*|Welcome||www.outworldz.com9000Welcome|128,128,96|
        Dim HTML As String
        Dim HTMLFILE = PropOpensimBinPath & "bin\data\teleports.htm"
        HTML = "Welcome to |" & Settings.SimName & "||" & Settings.PublicIP & ":" & Settings.HttpPort & ":" & Settings.WelcomeRegion & "||" & vbCrLf
        Dim ToSort As New List(Of String)
        Using NewSQLConn As New MySqlConnection(Settings.RobustMysqlConnection())
            Dim UserStmt = "SELECT regionName from REGIONS"
            Try
                NewSQLConn.Open()
                Dim cmd As MySqlCommand = New MySqlCommand(UserStmt, NewSQLConn)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                While reader.Read()
                    Dim LongName = reader.GetString(0)
                    Diagnostics.Debug.Print("regionname {0}>", LongName)

                    Dim RegionNumber = PropRegionClass.FindRegionByName(LongName)
                    If RegionNumber >= 0 And PropRegionClass.Teleport(RegionNumber) = "True" Then
                        ToSort.Add(LongName)
                    End If

                End While
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)

            End Try
        End Using

        ' Acquire keys And sort them.
        ToSort.Sort()

        For Each S As String In ToSort
            HTML = HTML & "*|" & S & "||" & Settings.PublicIP & ":" & Settings.HttpPort & ":" & S & "||" & vbCrLf
        Next

        FileStuff.DeleteFile(HTMLFILE)

        Try
            Using outputFile As New StreamWriter(HTMLFILE, True)
                outputFile.WriteLine(HTML)
            End Using
        Catch ex As Exception
            ErrorLog("Error:Failed to create file:" & ex.Message)
        End Try

    End Sub

    Private Sub ShowHyperGridAddressToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowHyperGridAddressToolStripMenuItem.Click

        Print("Grid address is " & vbCrLf & "http://" & Settings.PublicIP & ":" & Settings.HttpPort)

    End Sub

    Private Function Stripqq(input As String) As String

        Return Replace(input, """", "")

    End Function

    ''' <summary>
    ''' Timer runs every second registers DNS,looks for web server stuff that arrives, restarts any
    ''' sims , updates lists of agents builds teleports.html for older teleport checks for crashed regions
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As EventArgs) Handles Timer1.Tick

        Chart() ' do charts collection each second

        If Not PropOpensimIsRunning() Then
            Timer1.Stop()
            Return
        End If
        PropDNSSTimer += 1

        If PropAborting Then Return

        ' 5 seconds check for a restart RegionRestart requires  MOD 5
        If PropDNSSTimer Mod 5 = 0 Then
            PropRegionClass.CheckPost()
            ScanAgents() ' update agent count
            ExitHandlerPoll() ' see if any regions have exited and set it up for Region Restart
        End If

        ' Just once at the Minute Mark
        If PropDNSSTimer = 60 Then
            RegionListHTML() ' create HTML for older 2.4 region teleporters
            GetEvents() ' get the events from the Outworldz main server for all grids
        End If

        ' Just once at the 5 minute mark, after regions have booted.
        If PropDNSSTimer = 300 Then
            RunDataSnapshot() ' Fetch assets marked for search at boot
        End If

        ' every 5 minutes
        If PropDNSSTimer Mod 300 = 0 Then
            RegionListHTML() ' create HTML for older 2.4 region teleporters
            CrashDetector.Find()
            RunDataSnapshot() ' Fetch assets marked for search- the Snapshot module itself only checks ever 10
        End If

        'hour
        If PropDNSSTimer Mod 3600 = 0 Then
            RegisterDNS()
            GetEvents() ' get the events from the Outworldz main server for all grids
        End If

        If Settings.EventTimerEnabled And PropDNSSTimer Mod 3600 = 0 Then
            GetEvents() ' get the events from the Outworldz main server for all grids
        End If

    End Sub

#End Region

#Region "IAROAR"

    Public Function ChooseRegion(Optional JustRunning As Boolean = False) As String

        ' Show testDialog as a modal dialog and determine if DialogResult = OK.
        Dim chosen As String = ""
        Using Chooseform As New Choice ' form for choosing a set of regions
            Chooseform.FillGrid("Region", JustRunning)  ' populate the grid with either Group or RegionName
            Dim ret = Chooseform.ShowDialog()
            If ret = DialogResult.Cancel Then Return ""
            Try
                ' Read the chosen sim name
                chosen = Chooseform.DataGridView.CurrentCell.Value.ToString()
            Catch ex As Exception
                ErrorLog("Warn: Could not choose a displayed region. " & ex.Message)
            End Try
        End Using
        Return chosen

    End Function

    Public Function LoadIARContent(thing As String) As Boolean

        If Not PropOpensimIsRunning() Then
            Print("Opensim is not running. Cannot load an IAR at this time.")
            Return False
        End If

        Dim num As Integer = -1

        ' find one that is running
        For Each RegionNum In PropRegionClass.RegionNumbers
            If PropRegionClass.IsBooted(RegionNum) Then
                num = RegionNum
            End If
        Next
        If num = -1 Then
            MsgBox("No regions are ready, so cannot load the IAR", vbInformation, "Info")
            Return False
        End If

        Dim Path As String = InputBox("Folder to save  Inventory (""/"", ""/Objects"", ""/Objects/Somefolder..."")", "Folder Name", "/Objects")

        Dim user = InputBox("First and Last name that will get this IAR?")
        Dim password = InputBox("Password for user " & user & "?")
        If user.Length > 0 And password.Length > 0 Then

            ConsoleCommand(PropRegionClass.GroupName(num), "load iar --merge " & user & " " & Path & " " & password & " " & """" & thing & """" & "{ENTER}" & vbCrLf)
            ConsoleCommand(PropRegionClass.GroupName(num), "alert IAR content Is loaded{ENTER}" & vbCrLf)
            Print("Opensim Is loading your item. You will find it in Inventory in " & Path & " soon.")
        Else
            Print("Load IAR canceled - must use the full user name and password.")
        End If
        Me.Focus()
        Return True

    End Function

    Public Function VarChooser(RegionName As String) As String

        Dim RegionNumber = PropRegionClass.FindRegionByName(RegionName)
        Dim size = PropRegionClass.SizeX(RegionNumber)
        If size = 256 Then  ' 1x1
            Using VarForm As New FormDisplacement1X1 ' form for choosing a  region in  a var
                ' Show testDialog as a modal dialog and determine if DialogResult = OK.
                VarForm.Init(RegionNumber)
                VarForm.ShowDialog()
            End Using
        ElseIf size = 512 Then  ' 2x2
            Using VarForm As New FormDisplacement2x2 ' form for choosing a  region in  a var
                ' Show testDialog as a modal dialog and determine if DialogResult = OK.
                VarForm.ShowDialog()
            End Using
        ElseIf size = 768 Then ' 3x3
            Using VarForm As New FormDisplacement3x3 ' form for choosing a  region in  a var
                ' Show testDialog as a modal dialog and determine if DialogResult = OK.
                VarForm.ShowDialog()
            End Using
        ElseIf size = 1024 Then ' 4x4
            Using VarForm As New FormDisplacement ' form for choosing a region in  a var
                ' Show testDialog as a modal dialog and determine if DialogResult = OK.
                VarForm.ShowDialog()
            End Using
        Else
            Return ""
        End If

        Return PropSelectedBox

    End Function

    Private Sub AddLog(name As String)
        Dim LogMenu As New ToolStripMenuItem With {
                .Text = name,
                .ToolTipText = "Click to view this log",
                .Size = New Size(269, 26),
                .Image = My.Resources.Resources.document_view,
                .DisplayStyle = ToolStripItemDisplayStyle.Text
            }
        AddHandler LogMenu.Click, New EventHandler(AddressOf LogViewClick)
        ViewLogsToolStripMenuItem.Visible = True
        ViewLogsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {LogMenu})

    End Sub

    Private Sub AllRegionsOARsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllTheRegionsOarsToolStripMenuItem.Click

        If Not PropOpensimIsRunning() Then
            Print("Opensim Is Not running. Cannot save an OAR at this time.")
            Return
        End If

        Dim n As Integer = 0
        Dim L As New List(Of String)

        For Each RegionNumber In PropRegionClass.RegionNumbers
            If PropRegionClass.IsBooted(RegionNumber) Then
                Dim Group = PropRegionClass.GroupName(RegionNumber)
                For Each Y In PropRegionClass.RegionListByGroupNum(Group)
                    If Not L.Contains(PropRegionClass.RegionName(Y)) Then
                        ConsoleCommand(PropRegionClass.GroupName(RegionNumber), "change region " & """" & PropRegionClass.RegionName(Y) & """" & "{ENTER}" & vbCrLf)
                        ConsoleCommand(PropRegionClass.GroupName(RegionNumber), "save oar  " & """" & BackupPath() & PropRegionClass.RegionName(Y) & "_" & DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", Invarient) & ".oar" & """" & "{ENTER}" & vbCrLf)
                        L.Add(PropRegionClass.RegionName(Y))
                    End If
                Next
            End If
            n += 1
        Next

    End Sub

    Private Function BackupPath() As String

        'Autobackup must exist. if not create it
        ' if they set the folder somewhere else, it may have been deleted, so reset it to default
        If Settings.BackupFolder.ToLower(Invarient) = "autobackup" Then
            BackupPath = PropCurSlashDir & "/OutworldzFiles/AutoBackup/"
            If Not Directory.Exists(BackupPath) Then
                MkDir(BackupPath)
            End If
        Else
            BackupPath = Settings.BackupFolder & "/"
            BackupPath = BackupPath.Replace("\", "/")    ' because Opensim uses Unix-like slashes, that's why

            If Not Directory.Exists(BackupPath) Then
                BackupPath = PropCurSlashDir & "/OutworldzFiles/Autobackup/"

                If Not Directory.Exists(BackupPath) Then
                    MkDir(BackupPath)
                End If

                MsgBox("Autobackup folder cannot be located, so it has been reset to the default:" & BackupPath)
                Settings.BackupFolder = "AutoBackup"
                Settings.SaveSettings()
            End If
        End If

    End Function

    Private Sub IarClick(sender As Object, e As EventArgs)

        Dim file As String = Mid(CStr(sender.text), 1, InStr(CStr(sender.text), "|") - 2)
        file = PropDomain() & "/Outworldz_Installer/IAR/" & file 'make a real URL
        If LoadIARContent(file) Then
            Print("Opensimulator will load " & file & ".  This may take time to load.")
        End If
        sender.checked = True

    End Sub

    Private Sub LoadInventoryIARToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadInventoryIARToolStripMenuItem.Click

        If PropOpensimIsRunning() Then
            ' Create an instance of the open file dialog box. Set filter options and filter index.
            Dim openFileDialog1 As OpenFileDialog = New OpenFileDialog With {
                .InitialDirectory = """" & PropMyFolder & "/" & """",
                .Filter = "Inventory IAR (*.iar)|*.iar|All Files (*.*)|*.*",
                .FilterIndex = 1,
                .Multiselect = False
            }

            ' Call the ShowDialog method to show the dialogbox.
            Dim UserClickedOK As DialogResult = openFileDialog1.ShowDialog

            ' Process input if the user clicked OK.
            If UserClickedOK = DialogResult.OK Then
                Dim thing = openFileDialog1.FileName
                If thing.Length > 0 Then
                    thing = thing.Replace("\", "/")    ' because Opensim uses Unix-like slashes, that's why
                    If LoadIARContent(thing) Then
                        Print("Opensimulator will load " & thing & ".  This may take time to load.")
                    End If
                End If
            End If
            openFileDialog1.Dispose()
        Else
            Print("Opensim Is Not running. Cannot load an IAR at this time.")
        End If

    End Sub

    Private Function LoadOARContent(thing As String) As Boolean

        If Not PropOpensimIsRunning() Then
            Print("Opensim has to be started to load an OAR file.")
            Return False
        End If

        Dim region = ChooseRegion(True)
        If region.Length = 0 Then Return False

        Dim offset = VarChooser(region)

        Dim backMeUp = MsgBox("Make a backup first?", vbYesNo, "Backup?")
        Dim num = PropRegionClass.FindRegionByName(region)
        If num < 0 Then
            MsgBox("Cannot find region")
            Return False
        End If
        Dim GroupName = PropRegionClass.GroupName(num)
        Dim once As Boolean = False
        For Each Y In PropRegionClass.RegionListByGroupNum(GroupName)
            Try
                If Not once Then
                    Print("Opensimulator will load " & thing & ". This may take some time.")
                    thing = thing.Replace("\", "/")    ' because Opensim uses UNIX-like slashes, that's why

                    ConsoleCommand(PropRegionClass.GroupName(Y), "change region " & region & "{ENTER}" & vbCrLf)
                    If backMeUp = vbYes Then
                        ConsoleCommand(PropRegionClass.GroupName(Y), "alert CPU Intensive Backup Started {ENTER}" & vbCrLf)
                        ConsoleCommand(PropRegionClass.GroupName(Y), "save oar " & BackupPath() & "Backup_" & DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", Invarient) & ".oar" & """" & "{ENTER}" & vbCrLf)
                    End If
                    ConsoleCommand(PropRegionClass.GroupName(Y), "alert New content Is loading ...{ENTER}" & vbCrLf)

                    Dim ForceParcel As String = ""
                    If PropForceParcel() Then ForceParcel = " --force-parcels "
                    Dim ForceTerrain As String = ""
                    If PropForceTerrain Then ForceTerrain = " --force-terrain "
                    Dim ForceMerge As String = ""
                    If PropForceMerge Then ForceMerge = " --merge "
                    Dim UserName As String = ""
                    If PropUserName.Length > 0 Then UserName = " --default-user " & """" & PropUserName & """" & " "

                    ConsoleCommand(PropRegionClass.GroupName(Y), "load oar " & UserName & ForceMerge & ForceTerrain & ForceParcel & offset & """" & thing & """" & "{ENTER}" & vbCrLf)
                    ConsoleCommand(PropRegionClass.GroupName(Y), "alert New content just loaded. {ENTER}" & vbCrLf)
                    once = True
                End If
            Catch ex As Exception
                ErrorLog("Error:  " & ex.Message)
            End Try
        Next

        Me.Focus()
        Return True

    End Function

    Private Sub LoadRegionOarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadRegionOarToolStripMenuItem.Click

        If PropOpensimIsRunning() Then
            Dim chosen = ChooseRegion(True)
            If chosen.Length = 0 Then Return
            Dim RegionNumber As Integer = PropRegionClass.FindRegionByName(chosen)

            ' Create an instance of the open file dialog box. Set filter options and filter index.
            Using openFileDialog1 As OpenFileDialog = New OpenFileDialog With {
                .InitialDirectory = BackupPath(),
                .Filter = "Opensim OAR(*.OAR,*.GZ,*.TGZ)|*.oar;*.gz;*.tgz;*.OAR;*.GZ;*.TGZ|All Files (*.*)|*.*",
                .FilterIndex = 1,
                .Multiselect = False
                }

                ' Call the ShowDialog method to show the dialogbox.
                Dim UserClickedOK As DialogResult = openFileDialog1.ShowDialog

                ' Process input if the user clicked OK.
                If UserClickedOK = DialogResult.OK Then

                    Dim offset = VarChooser(chosen)
                    If offset.Length = 0 Then Return

                    Dim backMeUp = MsgBox("Make a backup first and then load the new content?", vbYesNo, "Backup?")
                    Dim thing = openFileDialog1.FileName
                    If thing.Length > 0 Then
                        thing = thing.Replace("\", "/")    ' because Opensim uses UNIX-like slashes, that's why

                        Dim Group = PropRegionClass.GroupName(RegionNumber)
                        For Each Y In PropRegionClass.RegionListByGroupNum(Group)

                            ConsoleCommand(PropRegionClass.GroupName(Y), "change region " & chosen & "{ENTER}" & vbCrLf)
                            If backMeUp = vbYes Then
                                ConsoleCommand(PropRegionClass.GroupName(Y), "alert CPU Intensive Backup Started{ENTER}" & vbCrLf)
                                ConsoleCommand(PropRegionClass.GroupName(Y), "save oar  " & """" & BackupPath() & "Backup_" & DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", Invarient) & ".oar" & """" & "{ENTER}" & vbCrLf)
                            End If
                            ConsoleCommand(PropRegionClass.GroupName(Y), "alert New content Is loading..{ENTER}" & vbCrLf)

                            Dim ForceParcel As String = ""
                            If PropForceParcel() Then ForceParcel = " --force-parcels "
                            Dim ForceTerrain As String = ""
                            If PropForceTerrain Then ForceTerrain = " --force-terrain "
                            Dim ForceMerge As String = ""
                            If PropForceMerge Then ForceMerge = " --merge "
                            Dim UserName As String = ""
                            If PropUserName.Length > 0 Then UserName = " --default-user " & """" & PropUserName & """" & " "

                            ConsoleCommand(PropRegionClass.GroupName(Y), "load oar " & UserName & ForceMerge & ForceTerrain & ForceParcel & offset & """" & thing & """" & "{ENTER}" & vbCrLf)
                            ConsoleCommand(PropRegionClass.GroupName(Y), "alert New content just loaded." & "{ENTER}" & vbCrLf)

                        Next
                    End If
                End If

            End Using
        Else
            Print("Opensim Is Not running. Cannot load the OAR file.")
        End If

    End Sub

    Private Sub OarClick(sender As Object, e As EventArgs)

        Dim File As String = Mid(CStr(sender.text), 1, InStr(CStr(sender.text), "|") - 2)
        File = PropDomain() & "/Outworldz_Installer/OAR/" & File 'make a real URL
        LoadOARContent(File)
        sender.checked = True

    End Sub

    Private Sub SaveInventoryIARToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveInventoryIARToolStripMenuItem.Click

        If PropOpensimIsRunning() Then

            Using SaveIAR As New FormIARSave
                SaveIAR.ShowDialog()
                Dim chosen = SaveIAR.DialogResult()
                If chosen = DialogResult.OK Then

                    Dim itemName = SaveIAR.GObject
                    If itemName.Length = 0 Then
                        MsgBox("Must have an object to save")
                        Return
                    End If

                    Dim ToBackup As String

                    Dim BackupName = SaveIAR.GBackupName

                    If Not BackupName.EndsWith(".iar", StringComparison.InvariantCultureIgnoreCase) Then
                        BackupName += ".iar"
                    End If

                    If String.IsNullOrEmpty(SaveIAR.GBackupPath) Or SaveIAR.GBackupPath = "AutoBackup" Then
                        ToBackup = BackupPath() & "" & BackupName
                    Else
                        ToBackup = BackupName
                    End If

                    Dim Name = SaveIAR.GAvatarName

                    Dim Password = SaveIAR.GPassword

                    Dim flag As Boolean = False
                    For Each RegionNumber As Integer In PropRegionClass.RegionNumbers
                        Dim GName = PropRegionClass.GroupName(RegionNumber)
                        Dim RNUm = PropRegionClass.FindRegionByName(GName)
                        If PropRegionClass.IsBooted(RegionNumber) And Not flag Then
                            ConsoleCommand(PropRegionClass.GroupName(RegionNumber), "save iar " _
                                       & Name & " " _
                                       & """" & itemName & """" _
                                       & " " & """" & Password & """" & " " _
                                       & """" & ToBackup & """" _
                                       & "{ENTER}" & vbCrLf
                                      )
                            flag = True
                            Print("Saving " & BackupName & " to " & BackupPath())
                        End If
                    Next
                End If
            End Using
        Else
            Print("Opensim Is not running. Cannot make an IAR now.")
        End If

    End Sub

    Private Sub SaveRegionOARToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveRegionOARToolStripMenuItem.Click

        If PropOpensimIsRunning() Then

            Dim chosen = ChooseRegion(True)
            If chosen.Length = 0 Then Return
            Dim RegionNumber As Integer = PropRegionClass.FindRegionByName(chosen)

            Dim Message, title, defaultValue As String
            Dim myValue As String
            ' Set prompt.
            Message = "Enter a name for your backup:"
            title = "Backup to OAR"
            defaultValue = chosen & "_" & DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss", Invarient) & ".oar"

            ' Display message, title, and default value.
            myValue = InputBox(Message, title, defaultValue)
            ' If user has clicked Cancel, set myValue to defaultValue
            If myValue.Length = 0 Then Return

            If PropRegionClass.IsBooted(RegionNumber) Then
                Dim Group = PropRegionClass.GroupName(RegionNumber)
                ConsoleCommand(PropRegionClass.GroupName(RegionNumber), "alert CPU Intensive Backup Started{ENTER}" & vbCrLf)
                ConsoleCommand(PropRegionClass.GroupName(RegionNumber), "change region " & """" & chosen & """" & "{ENTER}" & vbCrLf)
                ConsoleCommand(PropRegionClass.GroupName(RegionNumber), "save oar " & """" & BackupPath() & myValue & """" & "{ENTER}" & vbCrLf)
            End If
            Me.Focus()
            Print("Saving " & myValue & " to " & BackupPath())
        Else
            Print("Opensim is not running. Cannot make a backup now.")
        End If

    End Sub

    Private Sub SetIAROARContent()

        Dim oars As String = ""
        Using client As New WebClient ' download client for web pages
            IslandToolStripMenuItem.Visible = False
            ClothingInventoryToolStripMenuItem.Visible = False
            Print("Refreshing Free OARs")
            Try
                oars = client.DownloadString(SecureDomain() & "/Outworldz_Installer/Content.plx?type=OAR&r=" & RandomNumber.Random())
            Catch ex As ArgumentNullException
                ErrorLog("No Oars, dang, something Is wrong with the Internet :-(")
                Return
            Catch ex As WebException
                ErrorLog("No Oars, dang, something Is wrong with the Internet :-(")
                Return
            Catch ex As NotSupportedException
                ErrorLog("No Oars, dang, something Is wrong with the Internet :-(")
                Return
            End Try
        End Using

        Dim line As String = ""
        Application.DoEvents()
        Using oarreader = New StringReader(oars)
            Dim ContentSeen As Boolean = False
            While Not ContentSeen
                line = oarreader.ReadLine()
                If line <> Nothing Then
                    Log("Info", "" & line)
                    Dim OarMenu As New ToolStripMenuItem With {
                        .Text = line,
                        .ToolTipText = "Click to load this content",
                        .DisplayStyle = ToolStripItemDisplayStyle.Text
                    }
                    AddHandler OarMenu.Click, New EventHandler(AddressOf OarClick)
                    IslandToolStripMenuItem.Visible = True
                    IslandToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {OarMenu})
                    PropContentAvailable = True
                Else
                    ContentSeen = True
                End If
            End While
        End Using

        BumpProgress10()

        ' read help files for menu

        Dim folders() = IO.Directory.GetFiles(PropMyFolder & "\Outworldzfiles\Help")

        For Each aline As String In folders

            If aline.EndsWith(".rtf", StringComparison.InvariantCultureIgnoreCase) Then
                aline = System.IO.Path.GetFileNameWithoutExtension(aline)
                Dim HelpMenu As New ToolStripMenuItem With {
                    .Text = aline,
                    .ToolTipText = "Click to load this content",
                    .DisplayStyle = ToolStripItemDisplayStyle.Text,
                    .Image = My.Resources.question_and_answer
                }
                AddHandler HelpMenu.Click, New EventHandler(AddressOf HelpClick)
                HelpOnSettingsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {HelpMenu})
            End If

        Next

        Log("Info", "OARS loaded")
        Print("Refreshing Free IARs")
        Dim iars As String = ""

        Using client As New WebClient ' download client for web pages
            Try
                iars = client.DownloadString(SecureDomain() & "/Outworldz_Installer/Content.plx?type=IAR&r=" & RandomNumber.Random())
            Catch ex As ArgumentNullException
                ErrorLog("No IARS, dang, something Is wrong with the Internet :-(")
                Return
            Catch ex As WebException
                ErrorLog("No IARS, dang, something Is wrong with the Internet :-(")
                Return
            Catch ex As NotSupportedException
                ErrorLog("No IARS, dang, something Is wrong with the Internet :-(")
                Return
            End Try

            Using iarreader As New StringReader(iars)
                Dim ContentSeen As Boolean = False
                While Not ContentSeen
                    line = iarreader.ReadLine()
                    If line <> Nothing Then
                        Log("Info", "" & line)
                        Dim IarMenu As New ToolStripMenuItem With {
                            .Text = line,
                            .ToolTipText = "Click to load this content the next time the simulator is started",
                            .DisplayStyle = ToolStripItemDisplayStyle.Text
                        }
                        AddHandler IarMenu.Click, New EventHandler(AddressOf IarClick)
                        ClothingInventoryToolStripMenuItem.Visible = True
                        ClothingInventoryToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {IarMenu})
                        PropContentAvailable = True
                    Else
                        ContentSeen = True
                    End If
                End While
            End Using
        End Using
        Log("Info", "IARS loaded")

        BumpProgress10()
        AddLog("All Logs")
        AddLog("Robust")
        AddLog("Outworldz")
        AddLog("Icecast")
        AddLog("MySQL")
        AddLog("All Settings")
        AddLog("--- Regions ---")
        For Each X As Integer In PropRegionClass.RegionNumbers
            Dim Name = PropRegionClass.RegionName(X)
            AddLog("Region " & Name)
        Next

        BumpProgress10()

    End Sub

    ''' <summary>
    ''' Upload in a separate thread the photo, if any. Cannot be called unless main web server is
    ''' known to be on line.
    ''' </summary>
    Private Sub UploadPhoto()

        If System.IO.File.Exists(PropMyFolder & "\OutworldzFiles\Photo.png") Then
            Dim Myupload As New UploadImage
            Myupload.PostContentUploadFile()
        End If

    End Sub

#End Region

#Region "Updates"

    Public Sub CheckForUpdates()

        Using client As New WebClient ' download client for web pages
            Print("Checking for Updates")
            Try
                Update_version = client.DownloadString(SecureDomain() & "/Outworldz_Installer/UpdateGrid.plx?fill=1" & GetPostData())
            Catch ex As ArgumentNullException
                ErrorLog("Dang:The Outworldz web site is down")
                Return
            Catch ex As WebException
                ErrorLog("Dang:The Outworldz web site is down")
                Return
            Catch ex As NotSupportedException
                ErrorLog("Dang:The Outworldz web site is down")
                Return
            End Try
        End Using
        If Update_version.Length = 0 Then Update_version = "0"
        Dim Delta As Single = 0
        Try
            Delta = Convert.ToSingle(Update_version, Invarient) - Convert.ToSingle(PropMyVersion, Invarient)
        Catch ex As FormatException
        Catch ex As OverflowException
        End Try

        If Delta > 0 Then

            If System.IO.File.Exists(PropMyFolder & "\DreamGrid-V" & CStr(Update_version) & ".zip") Then
                Dim result = MsgBox("Update V" & Update_version & " has been downloaded. Install it now?", vbYesNo)
                If result = vbOK Then
                    UpdaterGo("DreamGrid-V" & Convert.ToString(Update_version, Invarient) & ".zip")
                End If
                Return
            End If

            Print("Update V" & Update_version & " is available. Downloading it in background.")
            Dim pi As ProcessStartInfo = New ProcessStartInfo With {
                .Arguments = "DreamGrid-V" & Convert.ToString(Update_version, Invarient) & ".zip",
                .FileName = """" & PropMyFolder & "\Downloader.exe" & """"
            }

            If Debugger.IsAttached Then
                pi.WindowStyle = ProcessWindowStyle.Normal
            Else
                pi.WindowStyle = ProcessWindowStyle.Minimized
            End If

            UpdateProcess.StartInfo = pi
            UpdateProcess.EnableRaisingEvents = True
            Try
                UpdateProcess.Start()
            Catch ex As ObjectDisposedException
                Print("Error: Could Not launch Downloader.exe. Perhaps you can launch it manually. ")
            Catch ex As InvalidOperationException
                Print("Error: Could not launch Downloader.exe. Perhaps you can launch it manually.")
            Catch ex As ComponentModel.Win32Exception
                Print("Error: Could not launch Downloader.exe. Perhaps you can launch it manually.")
            End Try
        End If

        BumpProgress10()

    End Sub

    Private Sub CheckForUpdatesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CHeckForUpdatesToolStripMenuItem.Click

        CheckForUpdates()

    End Sub

    Private Sub UpdaterGo(Filename As String)

        KillApache()
        StopMysql()

        Dim pUpdate As Process = New Process()
        Dim pi As ProcessStartInfo = New ProcessStartInfo With {
            .Arguments = Filename,
            .FileName = """" & PropMyFolder & "\DreamGridSetup.exe" & """"
        }
        pUpdate.StartInfo = pi
        Try
            Print("I'll see you again when I wake up all fresh and new!")
            Log("Info", "Launch Updater and exiting")
            pUpdate.Start()
        Catch ex As ObjectDisposedException
            ErrorLog("Error: Could not launch DreamGridInstaller.exe.")
        Catch ex As InvalidOperationException
            ErrorLog("Error: Could not launch DreamGridInstaller.exe.")
        Catch ex As ComponentModel.Win32Exception
            ErrorLog("Error: Could not launch DreamGridInstaller.exe.")
        End Try
        End ' program

    End Sub

    Private Sub UpdaterProcess_Exited(ByVal sender As Object, ByVal e As EventArgs) Handles UpdateProcess.Exited

        Dim ExitCode = UpdateProcess.ExitCode
        If ExitCode = 0 Then
            Dim result = MsgBox("Update Version" & Update_version & " has been downloaded. Do you want to exit DreamGrid and install the update?", vbYesNo)
            If result = vbYes Then
                UpdaterGo("DreamGrid-V" & Convert.ToString(Update_version, Invarient) & ".zip")
            End If
        Else
            ErrorLog("Could not download an Update: ExitCode=" & CStr(ExitCode))
        End If

    End Sub

#End Region

#Region "Diagnostics"

    Public Function CheckPort(ServerAddress As String, Port As Integer) As Boolean

        Using ClientSocket As New TcpClient
            Try
                ClientSocket.Connect(ServerAddress, Port)
            Catch ex As ArgumentNullException
                Return False
            Catch ex As ArgumentOutOfRangeException
                Return False
            Catch ex As SocketException
                Return False
            Catch ex As ObjectDisposedException
                Return False
            End Try

            If ClientSocket.Connected Then
                Log("Info", " port probe success on port " & CStr(Port))
                Return True
            End If
        End Using
        CheckPort = False

    End Function

    Public Function SetPublicIP() As Boolean

        ' LAN USE
        If Settings.EnableHypergrid Then
            Print("Setup Hypergrid")
            BumpProgress10()
            If Settings.DNSName.Length > 0 Then
                Settings.PublicIP = Settings.DNSName()
                Settings.SaveSettings()
                Dim ret = RegisterDNS()
                Return ret
            Else

#Disable Warning BC42025 ' Access of shared member, constant member, enum member or nested type through an instance
                Settings.PublicIP = PropMyUPnpMap.LocalIP
#Enable Warning BC42025 ' Access of shared member, constant member, enum member or nested type through an instance
                Dim ret = RegisterDNS()
                Settings.SaveSettings()
                Return ret
            End If

        End If

        ' HG USE

        If Not IPCheck.IsPrivateIP(Settings.DNSName) Then
            BumpProgress10()
            Print("Registering Statistics")
            Settings.PublicIP = Settings.DNSName
            Settings.SaveSettings()
            Dim x = Settings.PublicIP.ToLower(Invarient)
            If x.Contains("outworldz.net") Then
                Print("Registering DynDNS address http://" & Settings.PublicIP & ":" & Settings.HttpPort)
            End If

            If RegisterDNS() Then
                Return True
            End If

        End If

        If Settings.PublicIP = "localhost" Or Settings.PublicIP = "127.0.0.1" Then
            RegisterDNS()
            Return True
        End If

        Log("Info", "Public IP=" & Settings.PublicIP)
        TestPublicLoopback()
        If Settings.DiagFailed Then

            Using client As New WebClient ' download client for web pages
                Try
                    ' Set Public IP
                    Settings.PublicIP = client.DownloadString("http://api.ipify.org/?r=" & RandomNumber.Random())
                Catch ex As ArgumentNullException
                    ErrorLog("Dang:The api.ipify.org web site is down")
                    Settings.DiagFailed = True
                Catch ex As WebException
                    ErrorLog("Dang:The api.ipify.org web site is down")
                    Settings.DiagFailed = True
                Catch ex As NotSupportedException
                    ErrorLog("Dang:The api.ipify.org web site is down")
                    Settings.DiagFailed = True
                End Try
            End Using

            Settings.SaveSettings()
            BumpProgress10()

            Return True
        End If

        Settings.PublicIP = PropMyUPnpMap.LocalIP
        Settings.SaveSettings()

        BumpProgress10()
        Return False

    End Function

#End Region

#Region "Diagnostics"

    Private Sub CheckDiagPort()

        PropUseIcons = True
        Print("Check Diagnostics port")
        Dim wsstarted = CheckPort("127.0.0.1", CType(Settings.DiagnosticPort, Integer))
        If wsstarted = False Then
            MsgBox("Diagnostics port " & Settings.DiagnosticPort & " is not working DreamGrid is not running at a high enough security level,  or blocked by firewall or anti virus, so region icons are disabled.", vbInformation, "There is a problem")
            PropUseIcons = False
        End If

    End Sub

    Private Sub DiagnosticsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DiagnosticsToolStripMenuItem.Click

        If Not PropOpensimIsRunning() Then
            Print("Cannot run diagnostics unless Opensimulator is running. Click 'Start' and try again.")
            Return
        End If
        ProgressBar1.Value = 0
        DoDiag()
        If Settings.DiagFailed = True Then
            Print("Hypergrid Diagnostics failed. These can be re-run at any time.  See Help->Network Diagnostics', 'Loopback', and 'Port Forwards'")
        Else
            Print("Tests passed for In and Out")
            Print("Hypergrid should be working.")
        End If
        ProgressBar1.Value = 100

    End Sub

    Private Sub DoDiag()

        If IPCheck.IsPrivateIP(Settings.DNSName) Then
            Print("You are on a LAN IP. Test skipped.")
            Return
        End If

        Print("---------------------------")
        Print("Running Network Diagnostics")

        Settings.DiagFailed = False

        OpenPorts() ' Open router ports with UPnp
        ProbePublicPort() ' Probe using Outworldz like Canyouseeme.org does on HTTP port
        TestPrivateLoopback()   ' Diagnostics
        TestPublicLoopback()    ' Http port
        TestAllRegionPorts()    ' All Dos boxes, actually

        If Settings.DiagFailed Then
            Dim answer = MsgBox("Diagnostics failed. Do you want to see the log?", vbYesNo)
            If answer = vbYes Then
                ShowLog()
            End If
        Else
            NewDNSName()
        End If
        Print("---------------------------")

    End Sub

    Private Sub PortTest(Weblink As String, Port As Integer)

        Dim result As String = ""
        Using client As New WebClient
            Try
                result = client.DownloadString(Weblink)
            Catch ex As ArgumentNullException
                ErrorLog("Err:Loopback fail:" & result & ":" & ex.Message)
            Catch ex As WebException  ' not an error as could be a 404 from Diva being off
            Catch ex As NotSupportedException
                ErrorLog("Err:Loopback fail:" & result & ":" & ex.Message)
            End Try
        End Using

        If result.Contains("DOCTYPE") Or result.Contains("Ooops!") Or result.Length = 0 Then
            Print("Loopback Passed for " & CStr(Port))
            Log("Info", "Passed:" & result)
        Else
            Print("Failed!")
            Settings.LoopBackDiag = False
            Settings.DiagFailed = True
            Log("Info", "Failed:" & result)
        End If

    End Sub

    Private Sub ProbePublicPort()

        If Settings.ServerType <> "Robust" Then
            Return
        End If

        Dim isPortOpen As String = ""
        Using client As New WebClient ' download client for web pages

            ' collect some stats and test loopback with a HTTP_ GET to the webserver. Send unique,
            ' anonymous random ID, both of the versions of Opensim and this program, and the
            ' diagnostics test results See my privacy policy at https://www.outworldz.com/privacy.htm

            Print("Checking Port Forwards")
            Dim Url = SecureDomain() & "/cgi/probetest.plx?IP=" & Settings.PublicIP & "&Port=" & Settings.HttpPort & GetPostData()
            Try
                isPortOpen = client.DownloadString(Url)
            Catch ex As ArgumentNullException
                ErrorLog("Dang: The Outworldz web site is down. Use Canyouseeme.org on port 8002 instead")
            Catch ex As WebException
                ErrorLog("Dang:The Outworldz web site is down. Use Canyouseeme.org on port 8002 instead")
            Catch ex As NotSupportedException
                ErrorLog("Dang:The Outworldz web site is down. Use Canyouseeme.org on port 8002 instead")
            End Try
        End Using

        If isPortOpen = "yes" Then
            Print("Incoming Hypergrid is working")
        Else
            Settings.LoopBackDiag = False
            Settings.DiagFailed = True
            Log("Warn", "Failed:" & isPortOpen)
            Print("Internet address " & Settings.PublicIP & ":" & Settings.HttpPort & " appears to not be forwarded to this machine in your router, so Hypergrid is not available. This can possibly be fixed by 'Port Forwards' in your router.  See Help->Port Forwards.")
        End If

    End Sub

    Private Sub TestAllRegionPorts()

        Dim result As String = ""
        Dim Len = PropRegionClass.RegionCount()
        Dim counter = 1
        ProgressBar1.Value = CType(counter / Len, Integer)

        Dim Used As New List(Of String)
        ' Boot them up
        For Each X As Integer In PropRegionClass.RegionNumbers()
            If PropRegionClass.IsBooted(X) Then

                Dim RegionName = PropRegionClass.RegionName(X)

                If Used.Contains(RegionName) Then Continue For
                Used.Add(RegionName)

                Dim Port = PropRegionClass.GroupPort(X)
                Print("Checking Loopback for " & RegionName)
                ProgressBar1.Value = CType(counter / Len * 100, Integer)
                PortTest("http://" & Settings.PublicIP & ":" & Port & "/?_TestLoopback=" & RandomNumber.Random, Port)

            End If
        Next

    End Sub

    Private Sub TestPrivateLoopback()

        Dim result As String = ""
        Print("Checking LAN Loopback")
        Dim weblink = "http://" & Settings.PrivateURL & ":" & Settings.DiagnosticPort & "/?_TestLoopback=" & RandomNumber.Random()
        Using client As New WebClient
            Try
                result = client.DownloadString(weblink)
            Catch ex As ArgumentNullException
                ErrorLog("Err:Loopback fail:" & weblink & ":" & ex.Message)
            Catch ex As WebException
                ErrorLog("Err:Loopback fail:" & weblink & ":" & ex.Message)
            Catch ex As NotSupportedException
                ErrorLog("Err:Loopback fail:" & weblink & ":" & ex.Message)
            End Try
        End Using

        If result = "Test Completed" Then
            Print("Passed LAN Loopback")
        Else
            Print("Failed to connect to " & weblink)
            Settings.LoopBackDiag = False
            Settings.DiagFailed = True
        End If

    End Sub

    Private Sub TestPublicLoopback()

        If IPCheck.IsPrivateIP(Settings.PublicIP) Then
            Log("Info", "Private IP, Loopback test skipped")
            Return
        End If

        If Settings.ServerType <> "Robust" Then
            Log("Info", "Server type is Region, Loopback on HTTP Port skipped")
            Return
        End If
        Print("Checking Router Loopback")
        PortTest("http://" & Settings.PublicIP & ":" & Settings.HttpPort & "/?_TestLoopback=" & RandomNumber.Random, Settings.HttpPort)

    End Sub

#End Region

#Region "UPnP"

    Public Function OpenRouterPorts() As Boolean

        If Not PropMyUPnpMap.UPnpEnabled And Settings.UPnPEnabled Then
            Log("UPnP", "UPnP is not working in the router")
            Settings.UPnPEnabled = False
            Settings.SaveSettings()
            Return False
        End If

        If Not Settings.UPnPEnabled Then
            Log("UPnP", "UPnP is not Enabled")
            Return False
        End If

        Log("UPnP", "Local IP seems to be " & PropMyUPnpMap.LocalIP)

        Try
            If Settings.SCEnable Then
                'Icecast 8080
                If PropMyUPnpMap.Exists(Convert.ToInt16(Settings.SCPortBase), UPnp.MyProtocol.TCP) Then
                    PropMyUPnpMap.Remove(Convert.ToInt16(Settings.SCPortBase), UPnp.MyProtocol.TCP)
                End If
                PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, CType(Settings.SCPortBase, Integer), UPnp.MyProtocol.TCP, "Icecast TCP Public " & CStr(Settings.SCPortBase))
                Print("Icecast Port is set to " & CStr(Settings.SCPortBase))
                BumpProgress10()
                If PropMyUPnpMap.Exists(Convert.ToInt16(Settings.SCPortBase1), UPnp.MyProtocol.TCP) Then
                    PropMyUPnpMap.Remove(Convert.ToInt16(Settings.SCPortBase1), UPnp.MyProtocol.TCP)
                End If
                PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, CType(Settings.SCPortBase1, Integer), UPnp.MyProtocol.TCP, "Icecast1 TCP Public " & CStr(Settings.SCPortBase))
                Print("Icecast Port1 is set to " & CStr(Settings.SCPortBase1))
            End If

            If Settings.ApachePort > 0 Then
                If PropMyUPnpMap.Exists(Settings.ApachePort, UPnp.MyProtocol.TCP) Then
                    PropMyUPnpMap.Remove(Settings.ApachePort, UPnp.MyProtocol.TCP)
                End If
                PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, Settings.ApachePort, UPnp.MyProtocol.TCP, "Icecast1 TCP Public " & CStr(Settings.SCPortBase))
                Print("Apache Port is set to " & CType(Settings.ApachePort, String))
            End If

            ' 8002 for TCP and UDP
            If PropMyUPnpMap.Exists(Convert.ToInt16(Settings.HttpPort, Invarient), UPnp.MyProtocol.TCP) Then
                PropMyUPnpMap.Remove(Convert.ToInt16(Settings.HttpPort, Invarient), UPnp.MyProtocol.TCP)
            End If
            PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, Convert.ToInt16(Settings.HttpPort, Invarient), UPnp.MyProtocol.TCP, "Opensim TCP Grid " & Settings.HttpPort)
            Print("Grid TCP Port is set to " & Settings.HttpPort)
            BumpProgress10()

            If PropMyUPnpMap.Exists(Convert.ToInt16(Settings.HttpPort, Invarient), UPnp.MyProtocol.UDP) Then
                PropMyUPnpMap.Remove(Convert.ToInt16(Settings.HttpPort, Invarient), UPnp.MyProtocol.UDP)
            End If
            PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, Convert.ToInt16(Settings.HttpPort, Invarient), UPnp.MyProtocol.UDP, "Opensim UDP Grid " & Settings.HttpPort)
            Print("Grid UDP Port is set to " & Settings.HttpPort)
            BumpProgress10()

            For Each X As Integer In PropRegionClass.RegionNumbers
                Dim R As Integer = PropRegionClass.RegionPort(X)
                Application.DoEvents()

                If PropMyUPnpMap.Exists(R, UPnp.MyProtocol.UDP) Then
                    PropMyUPnpMap.Remove(R, UPnp.MyProtocol.UDP)
                    Application.DoEvents()
                End If
                PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, R, UPnp.MyProtocol.UDP, "Opensim UDP Region " & PropRegionClass.RegionName(X) & " ")
                Print("Region UDP " & PropRegionClass.RegionName(X) & " is set to " & CStr(R))
                BumpProgress(1)
                Application.DoEvents()
                If PropMyUPnpMap.Exists(R, UPnp.MyProtocol.TCP) Then
                    PropMyUPnpMap.Remove(R, UPnp.MyProtocol.TCP)
                    Application.DoEvents()
                End If
                PropMyUPnpMap.Add(PropMyUPnpMap.LocalIP, R, UPnp.MyProtocol.TCP, "Opensim TCP Region " & PropRegionClass.RegionName(X) & " ")
                Print("Region TCP " & PropRegionClass.RegionName(X) & " is set to " & CStr(R))
                BumpProgress(1)
            Next

            BumpProgress10()
        Catch e As Exception
            Log("UPnP", "UPnP Exception caught:  " & e.Message)
            Return False
        End Try
        Return True 'successfully added

    End Function

    Private Function GetPostData() As String

        Dim UPnp As String = "Fail"
        If Settings.UPnpDiag Then
            UPnp = "Pass"
        End If
        Dim Loopb As String = "Fail"
        If Settings.LoopBackDiag Then
            Loopb = "Pass"
        End If

        Dim Grid As String = "Grid"

        ' no DNS password used if DNS name is null
        Dim m = Settings.MachineID()
        If Settings.DNSName.Length = 0 Then
            m = ""
        End If

        Dim data As String = "&MachineID=" & m _
        & "&FriendlyName=" & WebUtility.UrlEncode(Settings.SimName) _
        & "&V=" & WebUtility.UrlEncode(Convert.ToString(PropMyVersion, Invarient)) _
        & "&OV=" & WebUtility.UrlEncode(CStr(PropSimVersion)) _
        & "&uPnp=" & CStr(UPnp) _
        & "&Loop=" & CStr(Loopb) _
        & "&Type=" & CStr(Grid) _
        & "&Ver=" & CStr(PropUseIcons) _
        & "&isPublic=" & CStr(Settings.GDPR()) _
        & "&r=" & RandomNumber.Random()
        Return data

    End Function

    Private Function OpenPorts() As Boolean

        Print("Check Router Ports")
        Try
            If OpenRouterPorts() Then ' open UPnp port
                Log("Info", "UPnP: Ok")
                Settings.UPnpDiag = True
                Settings.SaveSettings()
                BumpProgress10()
                Return True
            Else
                Print("Info:UPnP is disabled.")
                Settings.UPnpDiag = False
                Settings.SaveSettings()
                BumpProgress10()
                Return False
            End If
        Catch e As Exception
            Log("Error", " UPnP Exception: " & e.Message)
            Settings.UPnpDiag = False
            Settings.SaveSettings()
            BumpProgress10()
            Return False
        End Try

    End Function

#End Region

#Region "MySQL"

    Public Sub BackupDB()
        If Not StartMySQL() Then
            ProgressBar1.Value = 0
            ProgressBar1.Visible = True
            ToolBar(False)
            Buttons(StartButton)
            Print("Stopped")
            Return
        End If

        Print("Starting a slow but extensive Database Backup => Autobackup folder")
        Using pMySqlBackup As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo With {
            .Arguments = "",
            .WindowStyle = ProcessWindowStyle.Normal,
            .WorkingDirectory = PropMyFolder & "\OutworldzFiles\mysql\bin\",
            .FileName = PropMyFolder & "\OutworldzFiles\mysql\bin\BackupMysql.bat"
            }
            pMySqlBackup.StartInfo = pi
            pMySqlBackup.Start()
        End Using

    End Sub

    Public Function CheckMysql() As Boolean

        Dim version As String = Nothing
        Try
            version = MysqlInterface.IsMySqlRunning()
        Catch
            Log("Info", "MySQL was not running")
        End Try

        If version Is Nothing Then
            Return False
        End If
        Return True

    End Function

    Public Function StartMySQL() As Boolean

        Dim isMySqlRunning = CheckPort(Settings.RobustServer(), Settings.MySqlRobustDBPort)

        If isMySqlRunning Then
            MysqlPictureBox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(MysqlPictureBox, "Mysql is Running")
            PropMysqlExited = False
            Return True
        End If

        ' Build data folder if it does not exist
        MakeMysql()

        MysqlPictureBox.Image = My.Resources.nav_plain_red
        ToolTip1.SetToolTip(MysqlPictureBox, "Stopped")
        Application.DoEvents()
        ' Start MySql in background.

        BumpProgress10()
        Dim StartValue = ProgressBar1.Value
        Print("Starting MySql Database")

        ' SAVE INI file
        Settings.LoadIni(PropMyFolder & "\OutworldzFiles\mysql\my.ini", "#")
        Settings.SetIni("mysqld", "basedir", """" & PropCurSlashDir & "/OutworldzFiles/Mysql" & """")
        Settings.SetIni("mysqld", "datadir", """" & PropCurSlashDir & "/OutworldzFiles/Mysql/Data" & """")
        Settings.SetIni("mysqld", "port", CStr(Settings.MySqlRobustDBPort))
        Settings.SetIni("client", "port", CStr(Settings.MySqlRobustDBPort))
        Settings.SaveINI()

        ' create test program slants the other way:
        Dim testProgram As String = PropMyFolder & "\OutworldzFiles\Mysql\bin\StartManually.bat"
        FileStuff.DeleteFile(testProgram)

        Try
            Using outputFile As New StreamWriter(testProgram, True)
                outputFile.WriteLine("@REM A program to run Mysql manually for troubleshooting." & vbCrLf _
                                 & "mysqld.exe --defaults-file=" & """" & PropCurSlashDir & "/OutworldzFiles/mysql/my.ini" & """")
            End Using
        Catch ex As Exception
            ErrorLog("Error:Cannot write:" & ex.Message)
        End Try

        CreateService()
        CreateStopMySql()

        BumpProgress(5)

        ' Mysql was not running, so lets start it up.
        Dim pi As ProcessStartInfo = New ProcessStartInfo With {
            .Arguments = "--defaults-file=" & """" & PropCurSlashDir & "/OutworldzFiles/mysql/my.ini" & """",
            .WindowStyle = ProcessWindowStyle.Hidden,
            .FileName = """" & PropMyFolder & "\OutworldzFiles\mysql\bin\mysqld.exe" & """"
        }
        ProcessMySql.StartInfo = pi
        ProcessMySql.EnableRaisingEvents = True
        ProcessMySql.Start()

        ' wait for MySql to come up
        Dim MysqlOk As Boolean
        While Not MysqlOk And PropOpensimIsRunning And Not PropAborting

            BumpProgress(1)
            Application.DoEvents()

            Dim MysqlLog As String = PropMyFolder & "\OutworldzFiles\mysql\data"
            If ProgressBar1.Value = 100 Then ' about 30 seconds when it fails

                Dim yesno = MsgBox("The database did not start. Do you want to see the log file?", vbYesNo, "Error")
                If (yesno = vbYes) Then
                    Dim files() As String
                    files = Directory.GetFiles(MysqlLog, "*.err", SearchOption.TopDirectoryOnly)
                    For Each FileName As String In files
                        System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", """" & FileName & """")
                    Next
                End If
                Buttons(StartButton)
                Return False
            End If

            ' check again
            Sleep(2000)
            MysqlOk = CheckMysql()
        End While

        If Not PropOpensimIsRunning Then Return False

        PropMysqlExited = False

        MysqlPictureBox.Image = My.Resources.nav_plain_green
        ToolTip1.SetToolTip(MysqlPictureBox, "Running")
        PropMysqlExited = False

        Return True

    End Function

    Private Sub BackupDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackupDatabaseToolStripMenuItem.Click

        BackupDB()

    End Sub

    Private Sub CheckAndRepairDatbaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckAndRepairDatbaseToolStripMenuItem.Click

        If Not StartMySQL() Then
            ProgressBar1.Value = 0
            ProgressBar1.Visible = True
            ToolBar(False)
            Buttons(StartButton)
            Print("Stopped")
            Return
        End If

        Dim pi As ProcessStartInfo = New ProcessStartInfo()

        ChDir(PropMyFolder & "\OutworldzFiles\mysql\bin")
        pi.WindowStyle = ProcessWindowStyle.Normal
        pi.Arguments = CStr(Settings.MySqlRobustDBPort)

        pi.FileName = "CheckAndRepair.bat"
        Using pMySqlDiag1 As Process = New Process With {
                .StartInfo = pi
            }
            pMySqlDiag1.Start()
            pMySqlDiag1.WaitForExit()
        End Using

        ChDir(PropMyFolder)

    End Sub

    Private Sub CreateService()

        ' create test program slants the other way:
        Dim testProgram As String = PropMyFolder & "\OutworldzFiles\Mysql\bin\InstallAsAService.bat"
        FileStuff.DeleteFile(testProgram)

        Try
            Using outputFile As New StreamWriter(testProgram, True)
                outputFile.WriteLine("@REM Program to run Mysql as a Service" & vbCrLf +
            "mysqld.exe --install Mysql --defaults-file=" & """" & PropCurSlashDir & "/OutworldzFiles/mysql/my.ini" & """" & vbCrLf & "net start Mysql" & vbCrLf)
            End Using
        Catch ex As Exception
            ErrorLog("Error:Install As A Service" & ex.Message)
        End Try

    End Sub

    Private Sub CreateStopMySql()

        ' create test program slants the other way:
        Dim testProgram As String = PropMyFolder & "\OutworldzFiles\Mysql\bin\StopMySQL.bat"
        FileStuff.DeleteFile(testProgram)
        Try
            Using outputFile As New StreamWriter(testProgram, True)
                outputFile.WriteLine("@REM Program to stop Mysql" & vbCrLf +
            "mysqladmin.exe -u root --port " & CStr(Settings.MySqlRobustDBPort) & " shutdown" & vbCrLf & "@pause" & vbCrLf)
            End Using
        Catch ex As Exception
            ErrorLog("Error:StopMySQL.bat" & ex.Message)
        End Try

    End Sub

    Private Sub MakeMysql()

        Dim m As String = PropMyFolder & "\OutworldzFiles\Mysql\"
        If Not System.IO.File.Exists(m & "\Data\ibdata1") Then
            Print("Creating new, blank database")
            Using zip As ZipFile = ZipFile.Read(m & "\Blank-Mysql-Data-folder.zip")
                For Each ZipEntry In zip
                    Application.DoEvents()
                    ZipEntry.Extract(m, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
                Next
            End Using
        End If

    End Sub

    Private Sub RestoreDatabaseToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles RestoreDatabaseToolStripMenuItem1.Click

        If PropOpensimIsRunning() Then
            Print("Cannot restore when Opensim is running. Click [Stop] and try again.")
            Return
        End If

        If Not StartMySQL() Then
            ProgressBar1.Value = 0
            ProgressBar1.Visible = True
            ToolBar(False)
            Buttons(StartButton)
            Print("Stopped")
            Return
        End If

        ' Create an instance of the open file dialog box. Set filter options and filter index.
        Dim openFileDialog1 As OpenFileDialog = New OpenFileDialog With {
            .InitialDirectory = BackupPath(),
            .Filter = "BackupFile (*.sql)|*.sql|All Files (*.*)|*.*",
            .FilterIndex = 1,
            .Multiselect = False
        }

        ' Call the ShowDialog method to show the dialogbox.
        Dim UserClickedOK As DialogResult = openFileDialog1.ShowDialog

        ' Process input if the user clicked OK.
        If UserClickedOK = DialogResult.OK Then
            Dim thing = openFileDialog1.FileName
            If thing.Length > 0 Then

                Dim yesno = MsgBox("Are you sure? Your database will re-loaded from the backup and all existing content replaced. Avatars, sims, inventory, all of it.", vbYesNo, "Restore?")
                If yesno = vbYes Then
                    ' thing = thing.Replace("\", "/") ' because Opensim uses UNIX-like slashes,
                    ' that's why

                    FileStuff.DeleteFile(PropMyFolder & "\OutworldzFiles\mysql\bin\RestoreMysql.bat")

                    Try
                        Dim filename As String = PropMyFolder & "\OutworldzFiles\mysql\bin\RestoreMysql.bat"
                        Using outputFile As New StreamWriter(filename, True)
                            outputFile.WriteLine("@REM A program to restore Mysql from a backup" & vbCrLf _
                                & "mysql -u root opensim <  " & """" & thing & """" _
                                & vbCrLf & "@pause" & vbCrLf)
                        End Using
                    Catch ex As Exception
                        ErrorLog("Failed to create restore file:" & ex.Message)
                        Return
                    End Try

                    Print("Starting restore - do not interrupt!")
                    Dim pMySqlRestore As Process = New Process()
                    ' pi.Arguments = thing
                    Dim pi As ProcessStartInfo = New ProcessStartInfo With {
                        .WindowStyle = ProcessWindowStyle.Normal,
                        .WorkingDirectory = PropMyFolder & "\OutworldzFiles\mysql\bin\",
                        .FileName = PropMyFolder & "\OutworldzFiles\mysql\bin\RestoreMysql.bat"
                    }
                    pMySqlRestore.StartInfo = pi
                    pMySqlRestore.Start()
                    Print("")
                End If
            Else
                Print("Restore canceled")
            End If
        End If
    End Sub

    Private Sub StopIcecast()

        Zap("icecast")
        IceCastPicturebox.Image = My.Resources.nav_plain_red
        ToolTip1.SetToolTip(IceCastPicturebox, "Stopped")

    End Sub

    Private Sub StopMysql()

        Dim isMySqlRunning = CheckPort("127.0.0.1", Settings.MySqlRobustDBPort)

        If Not isMySqlRunning Then
            MysqlPictureBox.Image = My.Resources.nav_plain_red
            ToolTip1.SetToolTip(MysqlPictureBox, "Stopped")
            Application.DoEvents()
            Return
        End If

        If Not PropStopMysql Then
            MysqlPictureBox.Image = My.Resources.nav_plain_green
            ToolTip1.SetToolTip(MysqlPictureBox, "Running")
            Application.DoEvents()
            Print("MySQL was running when I woke up, so I am leaving MySQL on.")
            Return
        End If

        Print("Stopping MySql")

        Dim p As Process = New Process()
        Dim pi As ProcessStartInfo = New ProcessStartInfo With {
            .Arguments = "--port " & CStr(Settings.MySqlRobustDBPort) & " -u root shutdown",
            .FileName = """" & PropMyFolder & "\OutworldzFiles\mysql\bin\mysqladmin.exe" & """",
            .UseShellExecute = True, ' so we can redirect streams and minimize
            .WindowStyle = ProcessWindowStyle.Hidden
        }
        p.StartInfo = pi

        Try
            p.Start()
            p.WaitForExit()
            p.Close()
            MysqlPictureBox.Image = My.Resources.nav_plain_red
            ToolTip1.SetToolTip(MysqlPictureBox, "Stopped")
            Application.DoEvents()
        Catch ex As Exception
            ErrorLog("Error: failed to stop MySQL:" & ex.Message)
        End Try

    End Sub

#End Region

#Region "DNS"

    Public Function DoGetHostAddresses(hostName As [String]) As String

        Try
            Dim IPList As IPHostEntry = System.Net.Dns.GetHostEntry(hostName)

            For Each IPaddress In IPList.AddressList
                If (IPaddress.AddressFamily = Sockets.AddressFamily.InterNetwork) Then
                    Dim ip = IPaddress.ToString()
                    Return ip
                End If
            Next
            Return String.Empty
        Catch ex As Exception
            ErrorLog("Warn:Unable to resolve name:" & ex.Message)
        End Try
        Return String.Empty

    End Function

    Public Function GetNewDnsName() As String

        Dim client As New WebClient
        Dim Checkname As String
        Try
            Checkname = client.DownloadString("http://outworldz.net/getnewname.plx/?r=" & RandomNumber.Random)
        Catch ex As ArgumentNullException
            ErrorLog("Error:Cannot get new name:" & ex.Message)
            client.Dispose()
            Return ""
        Catch ex As WebException
            ErrorLog("Error:Cannot get new name:" & ex.Message)
            client.Dispose()
            Return ""
        Catch ex As NotSupportedException
            ErrorLog("Error:Cannot get new name:" & ex.Message)
            client.Dispose()
            Return ""
        End Try
        client.Dispose()
        Return Checkname

    End Function

    Public Function RegisterDNS() As Boolean

        If Settings.DNSName.Length = 0 Then
            Return True
        End If

        If IPCheck.IsPrivateIP(Settings.DNSName) Then
            Return True
        End If

        'Print("Checking " & "http://" & Settings.DNSName & ":" & Settings.HttpPort)

        Dim client As New WebClient
        Dim Checkname As String

        Try
            Application.DoEvents()
            Checkname = client.DownloadString("http://outworldz.net/dns.plx?GridName=" & Settings.DNSName & GetPostData())
        Catch ex As ArgumentNullException
            ErrorLog("Warn: Cannot check the DNS Name " & ex.Message)
            Return False
        Catch ex As Net.WebException
            ErrorLog("Warn: Cannot check the DNS Name " & ex.Message)
            Return False
        Catch ex As NotSupportedException
            ErrorLog("Warn: Cannot check the DNS Name " & ex.Message)
            Return False
        Finally
            client.Dispose()
        End Try

        If Checkname = "UPDATED" Then Return True
        Return False

    End Function

    Public Function RegisterName(name As String) As String

        Dim Checkname As String = String.Empty
        If Settings.ServerType <> "Robust" Then
            Return name
        End If
        Dim client As New WebClient ' download client for web pages
        Try
            Checkname = client.DownloadString("http://outworldz.net/dns.plx/?GridName=" & name & GetPostData())
        Catch ex As ArgumentNullException
            ErrorLog("Warn: Cannot register the DNS Name " & ex.Message)
            Return ""
        Catch ex As Net.WebException
            ErrorLog("Warn: Cannot register the DNS Name " & ex.Message)
            Return ""
        Catch ex As NotSupportedException
            ErrorLog("Warn: Cannot register the DNS Name " & ex.Message)
            Return ""
        Finally
            client.Dispose()
        End Try
        If Checkname = "UPDATED" Then
            Return name
        End If
        If Checkname = "NAK" Then
            MsgBox("Dynamic DNS Name already in use. Maybe you are using the wrong password?")
        End If
        Return ""

    End Function

    Private Sub NewDNSName()

        If Settings.DNSName.Length = 0 And Settings.EnableHypergrid Then
            Dim newname = GetNewDnsName()
            If newname.Length >= 0 Then
                If RegisterName(newname).Length >= 0 Then
                    BumpProgress10()
                    Settings.DNSName = newname
                    Settings.PublicIP = newname
                    Settings.SaveSettings()
                    MsgBox("Your system's name has been set to " & newname & ". You can change the name in the DNS menu at any time", vbInformation, "Info")
                End If
            End If
            BumpProgress10()
        End If

    End Sub

#End Region

#Region "Regions"

    Public Sub LoadRegionsStatsBar()

        SimulatorStatsToolStripMenuItem.DropDownItems.Clear()
        SimulatorStatsToolStripMenuItem.Visible = False

        If PropRegionClass Is Nothing Then Return

        For Each RegionNum In PropRegionClass.RegionNumbers

            Dim Menu As New ToolStripMenuItem With {
                .Text = PropRegionClass.RegionName(RegionNum),
                .ToolTipText = "Click to view stats on " & PropRegionClass.RegionName(RegionNum),
                .DisplayStyle = ToolStripItemDisplayStyle.Text
            }
            If PropRegionClass.IsBooted(RegionNum) Then
                Menu.Enabled = True
            Else
                Menu.Enabled = False
            End If

            AddHandler Menu.Click, New EventHandler(AddressOf Statmenu)
            SimulatorStatsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {Menu})
            SimulatorStatsToolStripMenuItem.Visible = True

        Next
    End Sub

    Private Sub RegionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RegionsToolStripMenuItem.Click

        ShowRegionform()

    End Sub

    Private Sub ScanAgents()
        ' Scan all the regions
        Dim sbttl As Integer = 0
        Try
            ToolTip1.SetToolTip(Label3, "")
            For Each RegionNum As Integer In PropRegionClass.RegionNumbers
                If PropRegionClass.IsBooted(RegionNum) Then
                    Dim count As Integer = MysqlInterface.IsUserPresent(PropRegionClass.UUID(RegionNum))
                    sbttl += count
                    If count > 0 Then
                        ToolTip1.SetToolTip(Label3, PropRegionClass.RegionName(RegionNum) & " " & ToolTip1.GetToolTip(Label3))
                    End If
                    PropRegionClass.AvatarCount(RegionNum) = count
                Else
                    PropRegionClass.AvatarCount(RegionNum) = 0
                End If
            Next
        Catch
        End Try

        AvatarLabel.Text = CStr(sbttl)
    End Sub

    Private Sub ShowRegionform()

        If RegionList.InstanceExists = False Then
            PropRegionForm = New RegionList
            PropRegionForm.Show()
            PropRegionForm.Activate()
        Else
            PropRegionForm.Show()
            PropRegionForm.Activate()
        End If

    End Sub

    Private Sub Statmenu(sender As Object, e As EventArgs)
        If PropOpensimIsRunning() Then
            Dim regionnum = PropRegionClass.FindRegionByName(CStr(sender.text))
            Dim port As String = CStr(PropRegionClass.RegionPort(regionnum))
            Dim webAddress As String = "http://localhost:" & Settings.HttpPort & "/bin/data/sim.html?port=" & port
            Process.Start(webAddress)
        Else
            Print("Opensim is not running. Cannot open the Web Interface.")
        End If
    End Sub

#End Region

#Region "Alerts"

    Private Sub AddUserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddUserToolStripMenuItem.Click
        ConsoleCommand("Robust", "create user{ENTER}")
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles All.Click
        SendMsg("all")
    End Sub

    Private Sub AllUsersAllSimsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JustOneRegionToolStripMenuItem.Click

        If Not PropOpensimIsRunning() Then
            Print("Opensim is not running")
            Return
        End If
        Dim rname = ChooseRegion(True)
        If rname.Length > 0 Then
            Dim Message = InputBox("What do you want to say to this region?")
            Dim X = PropRegionClass.FindRegionByName(rname)
            ConsoleCommand(PropRegionClass.GroupName(X), "change region  " & PropRegionClass.RegionName(X) & "{ENTER}" & vbCrLf)
            ConsoleCommand(PropRegionClass.GroupName(X), "alert " & Message & "{ENTER}" & vbCrLf)
        End If

    End Sub

    Private Sub ChangePasswordToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChangePasswordToolStripMenuItem.Click
        ConsoleCommand("Robust", "reset user password{ENTER}")
    End Sub

    Private Sub Debug_Click(sender As Object, e As EventArgs) Handles Debug.Click
        SendMsg("debug")
    End Sub

    Private Sub ErrorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ErrorToolStripMenuItem.Click
        SendMsg("error")
    End Sub

    Private Sub Fatal1_Click(sender As Object, e As EventArgs) Handles Fatal1.Click
        SendMsg("fatal")
    End Sub

    Private Sub Info_Click(sender As Object, e As EventArgs) Handles Info.Click
        SendMsg("info")
    End Sub

    Private Sub JustOneRegionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllUsersAllSimsToolStripMenuItem.Click

        If Not PropOpensimIsRunning() Then
            Print("Opensim is not running")
            Return
        End If

        Dim HowManyAreOnline As Integer = 0
        Dim Message = InputBox("What do you want to say to everyone on line?")
        If Message.Length > 0 Then
            For Each X As Integer In PropRegionClass.RegionNumbers
                If PropRegionClass.AvatarCount(X) > 0 Then
                    HowManyAreOnline += 1
                    ConsoleCommand(PropRegionClass.GroupName(X), "change region  " & PropRegionClass.RegionName(X) & "{ENTER}" & vbCrLf)
                    ConsoleCommand(PropRegionClass.GroupName(X), "alert " & Message & "{ENTER}" & vbCrLf)
                End If

            Next
            If HowManyAreOnline = 0 Then
                Print("Nobody is on line")
            Else
                Print("Message sent to " & CStr(HowManyAreOnline) & " regions")
            End If
        End If

    End Sub

    Private Sub Off1_Click(sender As Object, e As EventArgs) Handles Off1.Click
        SendMsg("off")
    End Sub

    Private Sub RestartOneRegionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartOneRegionToolStripMenuItem.Click
        If Not PropOpensimIsRunning() Then
            Print("Opensim is not running")
            Return
        End If
        Dim name = ChooseRegion(True)
        Dim X = PropRegionClass.FindRegionByName(name)
        If X > -1 Then
            ConsoleCommand(PropRegionClass.GroupName(X), "change region " & name & "{ENTER}" & vbCrLf)
            ConsoleCommand(PropRegionClass.GroupName(X), "restart region " & name & "{ENTER}" & vbCrLf)
            PropUpdateView = True ' make form refresh
        End If

    End Sub

    Private Sub RestartTheInstanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartTheInstanceToolStripMenuItem.Click
        If Not PropOpensimIsRunning() Then
            Print("Opensim is not running")
            Return
        End If
        Dim name = ChooseRegion(True)
        Dim X = PropRegionClass.FindRegionByName(name)
        If X > -1 Then
            ConsoleCommand(PropRegionClass.GroupName(X), "restart{ENTER}" & vbCrLf)
            PropUpdateView = True ' make form refresh
        End If

    End Sub

    Private Sub ScriptsResumeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScriptsResumeToolStripMenuItem.Click
        SendScriptCmd("scripts resume")
    End Sub

    Private Sub ScriptsStartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScriptsStartToolStripMenuItem.Click
        SendScriptCmd("scripts start")
    End Sub

    Private Sub ScriptsStopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScriptsStopToolStripMenuItem.Click
        SendScriptCmd("scripts stop")
    End Sub

    Private Sub ScriptsSuspendToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScriptsSuspendToolStripMenuItem.Click
        SendScriptCmd("scripts suspend")
    End Sub

    Private Sub SendMsg(msg As String)

        If Not PropOpensimIsRunning() Then Print("Opensim is not running")

        For Each Regionnumber As Integer In PropRegionClass.RegionNumbers
            If PropRegionClass.IsBooted(Regionnumber) Then
                ConsoleCommand(PropRegionClass.GroupName(Regionnumber), "set log level " & msg & "{ENTER}" & vbCrLf)
            End If
        Next
        ConsoleCommand("Robust", "set log level " & msg & "{ENTER}" & vbCrLf)

    End Sub

    Private Sub SendScriptCmd(cmd As String)
        If Not PropOpensimIsRunning() Then
            Print("Opensim is not running")
            Return
        End If
        Dim rname = ChooseRegion(True)
        Dim X = PropRegionClass.FindRegionByName(rname)
        If X > -1 Then
            ConsoleCommand(PropRegionClass.GroupName(X), "change region " & rname & "{ENTER}" & vbCrLf)
            ConsoleCommand(PropRegionClass.GroupName(X), cmd & "{ENTER}" & vbCrLf)
        End If

    End Sub

    Private Sub ShowUserDetailsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowUserDetailsToolStripMenuItem.Click
        Dim person = InputBox("Enter the first and last name of the user:")
        If person.Length > 0 Then
            ConsoleCommand("Robust", "show account " & person & "{ENTER}")
        End If
    End Sub

    Private Sub ViewIcecastWebPageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewIcecastWebPageToolStripMenuItem.Click
        If PropOpensimIsRunning() And Settings.SCEnable Then
            Dim webAddress As String = "http://" & Settings.PublicIP & ":" & CStr(Settings.SCPortBase)
            Print("Icecast lets you stream music into your sim. The Music URL is " & webAddress & "/stream")
            Process.Start(webAddress)
        ElseIf Settings.SCEnable = False Then
            Print("Shoutcast is not Enabled.")
        Else
            Print("Opensim is not running. Click Start to boot the system.")
        End If
    End Sub

#End Region

#Region "LocalOARIAR"

    Private Sub BackupCriticalFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackupCriticalFilesToolStripMenuItem.Click

        Dim CriticalForm As New FormBackupCheckboxes
        CriticalForm.Activate()
        CriticalForm.Visible = True

    End Sub

    Private Sub BackupIarClick(sender As Object, e As EventArgs)

        Dim File As String = PropMyFolder & "/OutworldzFiles/AutoBackup/" & sender.text.ToString() 'make a real URL
        If LoadIARContent(File) Then
            Print("Opensimulator will load " & sender.text.ToString() & ".  This may take time to load.")
        End If

    End Sub

    Private Sub BackupOarClick(sender As Object, e As EventArgs)

        Dim File = PropMyFolder & "/OutworldzFiles/AutoBackup/" & sender.text.ToString() 'make a real URL
        If LoadOARContent(File) Then
            Print("Opensimulator will load " & sender.text.ToString() & ".  This may take time to load.")
        End If

    End Sub

    Private Sub HelpOnIARSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpOnIARSToolStripMenuItem.Click
        Dim webAddress As String = "http://opensimulator.org/wiki/Inventory_Archives"
        Process.Start(webAddress)
    End Sub

    Private Sub HelpOnOARsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpOnOARsToolStripMenuItem.Click
        Dim webAddress As String = "http://opensimulator.org/wiki/Load_Oar_0.9.0%2B"
        Process.Start(webAddress)
    End Sub

    Private Sub LoadLocalIAROAR()
        ''' <summary>
        ''' Loads OAR and IAR from the menu
        ''' </summary>
        ''' <remarks>Handles both the IAR/OAR and Autobackup folders</remarks>

        Dim MaxFileNum As Integer = 10
        Dim counter = MaxFileNum
        Dim Filename = PropMyFolder & "\OutworldzFiles\OAR\"
        Dim OARs As Array = Directory.GetFiles(Filename, "*.OAR", SearchOption.TopDirectoryOnly)

        For Each OAR As String In OARs
            counter -= 1
            If counter > 0 Then
                Dim Name = Path.GetFileName(OAR)
                Dim OarMenu As New ToolStripMenuItem With {
                    .Text = Name,
                    .ToolTipText = "Click to load this content",
                    .DisplayStyle = ToolStripItemDisplayStyle.Text
                }
                AddHandler OarMenu.Click, New EventHandler(AddressOf LocalOarClick)
                LoadLocalOARSToolStripMenuItem.Visible = True
                LoadLocalOARSToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {OarMenu})
                Log("Info", "Set OAR " & Name)
            End If

        Next

        If Settings.BackupFolder = "AutoBackup" Then
            Filename = PropMyFolder & "\OutworldzFiles\AutoBackup\"
        Else
            Filename = Settings.BackupFolder
        End If

        Log("Info", "Auto OAR")
        Try
            Dim AutoOARs As Array = Directory.GetFiles(Filename, "*.OAR", SearchOption.TopDirectoryOnly)
            counter = MaxFileNum

            For Each OAR As String In AutoOARs
                counter -= 1
                If counter > 0 Then
                    Dim Name = Path.GetFileName(OAR)
                    Dim OarMenu As New ToolStripMenuItem With {
                        .Text = Name,
                        .ToolTipText = "Click to load this content",
                        .DisplayStyle = ToolStripItemDisplayStyle.Text
                    }
                    AddHandler OarMenu.Click, New EventHandler(AddressOf BackupOarClick)
                    LoadLocalOARSToolStripMenuItem.Visible = True
                    LoadLocalOARSToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {OarMenu})
                    Log("Info", Name)
                End If

            Next
        Catch
        End Try
        ' now for the IARs

        Log("Info", "Local IAR")
        Filename = PropMyFolder & "\OutworldzFiles\IAR\"
        Dim IARs As Array = Directory.GetFiles(Filename, "*.IAR", SearchOption.TopDirectoryOnly)
        counter = MaxFileNum
        For Each IAR As String In IARs
            counter -= 1
            If counter > 0 Then
                Dim Name = Path.GetFileName(IAR)
                Dim IarMenu As New ToolStripMenuItem With {
                    .Text = Name,
                    .ToolTipText = "Click to load this content",
                    .DisplayStyle = ToolStripItemDisplayStyle.Text
                }
                AddHandler IarMenu.Click, New EventHandler(AddressOf LocalIarClick)
                LoadLocalIARsToolStripMenuItem.Visible = True
                LoadLocalIARsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {IarMenu})
                Log("Info", Name)
            End If

        Next

        If Settings.BackupFolder = "AutoBackup" Then
            Filename = PropMyFolder & "\OutworldzFiles\AutoBackup\"
        Else
            Filename = Settings.BackupFolder
        End If

        Try
            Log("Info", "Auto IAR")
            Dim AutoIARs As Array = Directory.GetFiles(Filename, "*.IAR", SearchOption.TopDirectoryOnly)
            counter = MaxFileNum
            For Each IAR As String In AutoIARs
                counter -= 1
                If counter > 0 Then
                    Dim Name = Path.GetFileName(IAR)
                    Dim IarMenu As New ToolStripMenuItem With {
                        .Text = Name,
                        .ToolTipText = "Click to load this content",
                        .DisplayStyle = ToolStripItemDisplayStyle.Text
                    }
                    AddHandler IarMenu.Click, New EventHandler(AddressOf BackupIarClick)
                    LoadLocalIARsToolStripMenuItem.Visible = True
                    LoadLocalIARsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {IarMenu})
                    Log("Info", Name)
                End If

            Next
        Catch
        End Try

    End Sub

    Private Sub LocalIarClick(sender As Object, e As EventArgs)

        Dim File As String = PropMyFolder & "/OutworldzFiles/IAR/" & sender.text.ToString() 'make a real URL
        If LoadIARContent(File) Then
            Print("Opensimulator will load " & sender.text.ToString() & ".  This may take time to load.")
        End If

    End Sub

    Private Sub LocalOarClick(sender As Object, e As EventArgs)

        Dim File = PropMyFolder & "/OutworldzFiles/OAR/" & sender.text.ToString() 'make a real URL
        If LoadOARContent(File) Then
            Print("Opensimulator will load " & sender.text.ToString() & ".  This may take time to load.")
        End If

    End Sub

    Private Sub TechnicalInfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TechnicalInfoToolStripMenuItem.Click
        Dim webAddress As String = "https://www.outworldz.com/Outworldz_installer/technical.htm"
        Process.Start(webAddress)
    End Sub

    Private Sub TroubleshootingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TroubleshootingToolStripMenuItem.Click
        Dim webAddress As String = "https://www.outworldz.com/Outworldz_installer/Manual_TroubleShooting.htm"
        Process.Start(webAddress)
    End Sub

#End Region

#Region "Help"

    Public Shared Sub Help(page As String)

        ' Set the new form's desktop location so it appears below and to the right of the current form.

        FormHelp.Activate()
        FormHelp.Visible = True
        FormHelp.Init(page)

    End Sub

    Public Sub HelpOnce(Webpage As String)

        newScreenPosition = New ScreenPos(Webpage)
        If Not newScreenPosition.Exists() Then
            ' Set the new form's desktop location so it appears below and to the right of the current form.
            Dim FormHelp As New FormHelp
            FormHelp.Activate()
            FormHelp.Visible = True
            FormHelp.Init(Webpage)

        End If

    End Sub

    Public Sub Viewlog(name As String)
        If name Is Nothing Then Return
        Dim AllLogs As Boolean = False
        Dim path As New List(Of String)

        If name.StartsWith("Region ", StringComparison.InvariantCultureIgnoreCase) Then
            name = Replace(name, "Region ", "", 1, 1)
            name = PropRegionClass.GroupName(PropRegionClass.FindRegionByName(name))
            path.Add("""" & PropOpensimBinPath & "bin\Regions\" & name & "\Opensim.log" & """")
        Else
            If name = "All Logs" Then AllLogs = True
            If name = "Robust" Or AllLogs Then path.Add("""" & PropOpensimBinPath & "bin\Robust.log" & """")
            If name = "Outworldz" Or AllLogs Then path.Add("""" & PropMyFolder & "\Outworldzfiles\Outworldz.log" & """")
            If name = "UPnP" Or AllLogs Then path.Add("""" & PropMyFolder & "\Outworldzfiles\Upnp.log" & """")
            If name = "Icecast" Or AllLogs Then path.Add(" " & """" & PropMyFolder & "\Outworldzfiles\Icecast\log\error.log" & """")
            If name = "All Settings" Or AllLogs Then path.Add("""" & PropMyFolder & "\Outworldzfiles\Settings.ini" & """")
            If name = "--- Regions ---" Then Return

            If AllLogs Then
                For Each Regionnumber In PropRegionClass.RegionNumbers
                    name = PropRegionClass.GroupName(Regionnumber)
                    path.Add("""" & PropOpensimBinPath & "bin\Regions\" & name & "\Opensim.log" & """")
                Next
            End If

            If name = "MySQL" Or AllLogs Then
                Dim MysqlLog As String = PropMyFolder & "\OutworldzFiles\mysql\data"
                Dim files() As String
                files = Directory.GetFiles(MysqlLog, "*.err", SearchOption.TopDirectoryOnly)
                For Each FileName As String In files
                    path.Add("""" & FileName & """")
                Next

            End If
        End If
        ' Filter distinct elements, and convert back into list.
        Dim result As List(Of String) = path.Distinct().ToList

        Dim logs As String = ""
        For Each item In result
            Log("View", item)
            logs = logs & " " & item
        Next

        Try
            System.Diagnostics.Process.Start(PropMyFolder & "\baretail.exe", logs)
        Catch
        End Try

    End Sub

    Private Sub HelpClick(sender As Object, e As EventArgs)

        If sender.text.ToString() <> "Dreamgrid Manual.pdf" Then Help(sender.text.ToString())

    End Sub

    Private Sub HelpStartingUpToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles HelpStartingUpToolStripMenuItem1.Click

        Help("Startup")

    End Sub

    Private Sub JobEngineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles JobEngineToolStripMenuItem.Click
        For Each RegionNum As Integer In PropRegionClass.RegionListByGroupNum("*")
            ConsoleCommand(PropRegionClass.RegionName(RegionNum), "debug jobengine status{ENTER}" & vbCrLf)
        Next
    End Sub

    Private Sub LogViewClick(sender As Object, e As EventArgs)

        Dim name As String = sender.text.ToString()

        Viewlog(name)
    End Sub

    Private Sub PDFManualToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PDFManualToolStripMenuItem.Click
        Dim webAddress As String = PropMyFolder & "\Outworldzfiles\Help\Dreamgrid Manual.pdf"
        Process.Start(webAddress)
    End Sub

    Private Sub RevisionHistoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RevisionHistoryToolStripMenuItem.Click
        Help("Revisions")
    End Sub

    Private Sub ThreadpoolsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ThreadpoolsToolStripMenuItem.Click
        For Each RegionNum As Integer In PropRegionClass.RegionListByGroupNum("*")
            ConsoleCommand(PropRegionClass.RegionName(RegionNum), "show threads{ENTER}" & vbCrLf)
        Next
    End Sub

    Private Sub XengineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XengineToolStripMenuItem.Click
        For Each RegionNum As Integer In PropRegionClass.RegionListByGroupNum("*")
            ConsoleCommand(PropRegionClass.RegionName(RegionNum), "xengine status{ENTER}" & vbCrLf)
        Next
    End Sub

#End Region

#Region "Capslock"

    Public Shared Function SendableKeys(Str As String) As String

        If My.Computer.Keyboard.CapsLock Then
            For Pos = 1 To Len(Str)
                Dim C As String = Mid(Str, Pos, 1)
                Mid(Str, Pos) = CStr(IIf(UCase(C) = C, LCase(C), UCase(C)))
            Next
        End If
        Return Str

    End Function

#End Region

#Region "QuickEdit"

    Private Sub SetQuickEditOff()
        Dim pi As ProcessStartInfo = New ProcessStartInfo With {
            .Arguments = "Set-ItemProperty -path HKCU:\Console -name QuickEdit -value 0",
            .FileName = "powershell.exe",
            .WindowStyle = ProcessWindowStyle.Hidden,
            .Verb = "runas"
        }
        Using PowerShell As Process = New Process With {
             .StartInfo = pi
            }

            Try
                PowerShell.Start()
            Catch ex As Exception
                Log("Error", "Could not set Quickedit Off:" & ex.Message)
            End Try
        End Using

    End Sub

#End Region

#Region "Sequential"

    Public Sub SequentialPause()

        If Settings.Sequential Then

            For Each X As Integer In PropRegionClass.RegionNumbers
                If PropOpensimIsRunning() And PropRegionClass.RegionEnabled(X) And
                    Not (PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.RecyclingDown _
                    Or PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.ShuttingDown _
                    Or PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Stopped) Then

                    Dim ctr = 600 ' 1 minute max to start a region
                    Dim WaitForIt = True
                    While WaitForIt
                        Sleep(100)
                        If PropRegionClass.RegionEnabled(X) _
                    And Not PropAborting _
                    And (PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.RecyclingUp Or
                        PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.ShuttingDown Or
                        PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.RecyclingDown Or
                        PropRegionClass.Status(X) = RegionMaker.SIMSTATUSENUM.Booting) Then
                            WaitForIt = True
                        Else
                            WaitForIt = False
                        End If
                        ctr -= 1
                        If ctr <= 0 Then WaitForIt = False
                    End While
                End If
            Next
        Else
            Dim ctr = 600 ' 1 minute max to start a region
            Dim WaitForIt = True
            While WaitForIt
                Sleep(100)
                If cpu.NextValue() < PropCPUMAX Then
                    WaitForIt = False
                    ctr -= 1
                    If ctr <= 0 Then WaitForIt = False
                End If
            End While

        End If

    End Sub

#End Region

#Region "Search"

    Public Shared Function CompareDLLignoreCase(tofind As String, dll As List(Of String)) As Boolean
        If dll Is Nothing Then Return False
        For Each filename In dll
            If tofind.ToLower(Form1.Invarient) = filename.ToLower(Form1.Invarient) Then Return True
        Next
        Return False
    End Function

    Public Shared Sub DeleteEvents(Connection As MySqlConnection)

        Dim stm = "delete from events"
        Using cmd As MySqlCommand = New MySqlCommand(stm, Connection)
            Dim rowsdeleted = cmd.ExecuteNonQuery()
            Diagnostics.Debug.Print("Rows: {0}", rowsdeleted.ToString(Form1.Invarient))
        End Using

    End Sub

    Public Shared Function GetDlls(fname As String) As List(Of String)

        Dim DllList As New List(Of String)

        If System.IO.File.Exists(fname) Then
            Dim line As String
            Using reader As StreamReader = System.IO.File.OpenText(fname)
                'now loop through each line
                While reader.Peek <> -1
                    line = reader.ReadLine()
                    DllList.Add(line)
                End While
            End Using
        End If
        Return DllList

    End Function

    ''' <summary>
    ''' This method starts at the specified directory. It traverses all subdirectories. It returns a
    ''' List of those directories.
    ''' </summary>
    Public Shared Function GetFilesRecursive(ByVal initial As String) As List(Of String)
        ' This list stores the results.
        Dim result As New List(Of String)

        ' This stack stores the directories to process.
        Dim stack As New Stack(Of String)

        ' Add the initial directory
        stack.Push(initial)

        ' Continue processing for each stacked directory
        Do While (stack.Count > 0)
            ' Get top directory string
            Dim dir As String = stack.Pop
            Try
                ' Add all immediate file paths
                result.AddRange(Directory.GetFiles(dir, "*.dll"))

                ' Loop through all subdirectories and add them to the stack.
                Dim directoryName As String = ""

                'Save, but skip scriptengines
                For Each directoryName In Directory.GetDirectories(dir)
                    If Not directoryName.Contains("ScriptEngines") Then
                        stack.Push(directoryName)
                    Else
                        Diagnostics.Debug.Print("Skipping script")
                    End If
                Next
            Catch ex As Exception
            End Try
        Loop

        ' Return the list
        Return result
    End Function

    Public Shared Function ShowDOSWindow(handle As IntPtr, command As SHOWWINDOWENUM) As Boolean

        Dim ctr = 50
        If handle <> IntPtr.Zero Then
            Dim x = False

            While Not x And ctr > 0
                Sleep(100)
                Try
                    x = NativeMethods.ShowWindow(handle, command)
                    If x Then Return True
                Catch ex As Exception
                End Try
                ctr -= 1
            End While
        End If
        Return False

    End Function

    Public Shared Sub WriteEvent(Connection As MySqlConnection, D As Dictionary(Of String, String))

        If D Is Nothing Then Return

        Dim stm = "insert into events (simname,category,creatoruuid, owneruuid,name, description, dateUTC,duration,covercharge, coveramount,parcelUUID, globalPos,gateway,eventflags) values (" _
                        & "'" & D.Item("simname") & "'," _
                        & "'" & D.Item("category") & "'," _
                        & "'" & D.Item("creatoruuid") & "'," _
                        & "'" & D.Item("owneruuid") & "'," _
                        & "'" & D.Item("name") & "'," _
                        & "'" & D.Item("description") & "'," _
                        & "'" & D.Item("dateUTC") & "'," _
                        & "'" & D.Item("duration") & "'," _
                        & "'" & D.Item("covercharge") & "'," _
                        & "'" & D.Item("coveramount") & "'," _
                        & "'" & D.Item("parcelUUID") & "'," _
                        & "'" & D.Item("globalPos") & "'," _
                        & "'" & D.Item("gateway") & "'," _
                        & "'" & D.Item("eventflags") & "')"

#Disable Warning CA2100 ' Review SQL queries for security vulnerabilities
        Using cmd As MySqlCommand = New MySqlCommand(stm, Connection)
#Enable Warning CA2100 ' Review SQL queries for security vulnerabilities
            Dim rowsinserted = cmd.ExecuteNonQuery()
            Diagnostics.Debug.Print("Insert: {0}", CStr(rowsinserted))
        End Using

    End Sub

    Private Sub CleanDLLs()

        Dim dlls As List(Of String) = GetDlls(PropMyFolder & "/dlls.txt")
        Dim localdlls As List(Of String) = GetFilesRecursive(PropOpensimBinPath & "bin")
        For Each localdllname In localdlls
            Dim x = localdllname.IndexOf("OutworldzFiles", StringComparison.InvariantCulture)
            Dim newlocaldllname = Mid(localdllname, x)
            If Not CompareDLLignoreCase(newlocaldllname, dlls) Then
                Log("INFO", "Deleting dll " & localdllname)
                FileStuff.DeleteFile(localdllname)
            End If
        Next

    End Sub

    Private Sub GetEvents()

        If Not Settings.SearchEnabled Then Return

        Dim Simevents As New Dictionary(Of String, String)
        Dim ctr As Integer = 0
        Try
            Using osconnection = New MySqlConnection(Settings.OSSearchConnectionString())
                Try
                    osconnection.Open()
                Catch ex As InvalidOperationException
                    Log("Error", "Failed to Connect to OsSearch")
                    Return
                Catch ex As MySqlException
                    Log("Error", "Failed to Connect to OsSearch")
                    Return
                End Try
                DeleteEvents(osconnection)

                Using client As New WebClient()
                    Dim Stream = client.OpenRead(SecureDomain() & "/events.txt?r=" & RandomNumber.Random)
                    Using reader = New StreamReader(Stream)
                        While reader.Peek <> -1
                            Dim s = reader.ReadLine

                            ctr += 1
                            ' Split line on comma.
                            Dim array As String() = s.Split("|".ToCharArray())
                            Simevents.Clear()
                            ' Loop over each string received.
                            Dim part As String
                            For Each part In array
                                ' Display to console.
                                Dim a As String() = part.Split("^".ToCharArray())
                                If a.Count = 2 Then
                                    a(1) = a(1).Replace("'", "\'")
                                    a(1) = a(1).Replace("`", vbLf)
                                    Console.WriteLine("{0}:{1}", a(0), a(1))
                                    Simevents.Add(a(0), a(1))
                                End If
                            Next
                            WriteEvent(osconnection, Simevents)
                        End While
                    End Using ' reader

                End Using ' client
            End Using ' osconnection
        Catch ex As Exception
            ErrorLog(ex.Message)
        End Try

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        If PictureBox1.AccessibleName = "Arrow2Left" Then
            Me.Width = 575
            Me.Height = 425
            PictureBox1.Image = My.Resources.Arrow2Left
            PictureBox1.AccessibleName = "Arrow2Right"
        Else
            PictureBox1.Image = My.Resources.Arrow2Right
            PictureBox1.AccessibleName = "Arrow2Left"
            Me.Width = 320
            Me.Height = 180
        End If

    End Sub

    Private Sub RunDataSnapshot()

        If Not Settings.SearchLocal Then Return
        Diagnostics.Debug.Print("Scanning Data snapshot")
        Dim pi As ProcessStartInfo = New ProcessStartInfo()

        FileIO.FileSystem.CurrentDirectory = PropMyFolder & "\Outworldzfiles\PHP7\"
        pi.FileName = "Run_parser.bat"
        pi.UseShellExecute = False  ' needed to make window hidden
        pi.WindowStyle = ProcessWindowStyle.Hidden
        Dim ProcessPHP As Process = New Process With {
            .StartInfo = pi
        }
        ProcessPHP.StartInfo.CreateNoWindow = True
        Using ProcessPHP
            Try
                ProcessPHP.Start()
                ProcessPHP.WaitForExit()
            Catch ex As Exception
                ErrorLog("Error ProcessPHP failed to launch: " & ex.Message)
                FileIO.FileSystem.CurrentDirectory = PropMyFolder
            End Try
        End Using

    End Sub

    Private Sub SetupSearch()

        If Settings.ServerType = "Metro" _
            Or Settings.ServerType = "OsGrid" Then Return

        If Not Settings.SearchMigration = 2 Then

            MysqlInterface.DeleteSearchDatabase()

            Print("Setup search database")
            Dim pi As ProcessStartInfo = New ProcessStartInfo()

            FileIO.FileSystem.CurrentDirectory = PropMyFolder & "\Outworldzfiles\mysql\bin\"
            pi.FileName = "Create_OsSearch.bat"
            pi.UseShellExecute = True
            pi.CreateNoWindow = False
            pi.WindowStyle = ProcessWindowStyle.Hidden
            Using ProcessMysql As Process = New Process With {
                    .StartInfo = pi
                }

                Try
                    ProcessMysql.Start()
                    ProcessMysql.WaitForExit()
                Catch ex As Exception
                    ErrorLog("Error ProcessMysql failed to launch: " & ex.Message)
                    FileIO.FileSystem.CurrentDirectory = PropMyFolder
                    Return
                End Try
            End Using

            FileIO.FileSystem.CurrentDirectory = PropMyFolder

            Settings.SearchMigration = 2
            Settings.SaveSettings()

        End If

    End Sub

#End Region

End Class
