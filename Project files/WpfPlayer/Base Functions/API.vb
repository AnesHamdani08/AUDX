Imports System.Windows.Interop
Imports WpfPlayer

Public Class API
    Public Property AllowSetCalls As Boolean = True
    Public Property AllowGetCalls As Boolean = True
    Public Property Log As Boolean = False
    Public Property AllowRaisingEvents As Boolean = True
    ''' <summary>
    ''' Recieve only , running from here
    ''' </summary>
    Private WithEvents RNamedPipeServer As New NamedPipeManager("MuPlayAPI_MuPlay") 'Recieve only , running from here
    ''' <summary>
    ''' Send only , running from API
    ''' </summary>
    Private WithEvents SNamedPipeServer As New NamedPipeManager("MuPlayAPI_API") 'Send only , running from API
    Private Window As MainWindow = TryCast(Application.Current.MainWindow, MainWindow)
    Private WithEvents Player As Player = Window.MainPlayer
    Private Spacer As String = ">"
    Public Sub New()
        RNamedPipeServer.StartServer()
    End Sub
    ''' <summary>
    ''' DON'T KILL ME
    ''' </summary>
    Public Sub Dispose()
        RNamedPipeServer.StopServer()
    End Sub
    Private Sub NamedPipeServer_ReceiveString(obj As String) Handles RNamedPipeServer.ReceiveString
        Dim CMDARGS = obj.Split(Spacer)
        'Dim Rargs As New List(Of String)
        'Dim Args = CMDARGS
        'For Each arg In Args
        '    If Not String.IsNullOrEmpty(arg) AndAlso Not String.IsNullOrWhiteSpace(arg) Then
        '        Rargs.Add(arg)
        '    End If
        'Next
        Dim ID As CMDType = CMDARGS(0) 'Rargs.Item(0)
        Dim CMD As String
        Try
            CMD = CMDARGS(1) 'Rargs.Item(1)
        Catch ex As Exception
        End Try
        If Log Then
            Application.Current.Dispatcher.BeginInvoke(Sub()
                                                           My.Windows.Console.Log("API CALL : ID :  " & ID.ToString & "[" & ID & "]" & " ARGS:" & String.Join(";", CMDARGS))
                                                       End Sub)
        End If
        Select Case ID
            Case CMDType.GetCover
                If AllowGetCalls Then
                    Dim MEMCover As New IO.MemoryStream
                    Window.Dispatcher.Invoke(Sub()
                                                 Utils.BitmapFromImageSource(Player.CurrentMediaCover).Save(MEMCover, System.Drawing.Imaging.ImageFormat.Png)
                                             End Sub)
                    Dim STRCover = Convert.ToBase64String(MEMCover.ToArray)
                    SNamedPipeServer.Write(ID & Spacer & STRCover, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetCurrent
                If AllowGetCalls Then
                    SNamedPipeServer.Write(ID & Spacer & Player.CurrentMediaTitle & Spacer & Player.CurrentMediaArtist & Spacer & "TBI" & Spacer & Player.CurrentMediaType, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetRepeatShuffleType
                If AllowGetCalls Then
                    SNamedPipeServer.Write(ID & Spacer & Player.RepeateType & Spacer & Player.Shuffle, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetIsMono
                If AllowGetCalls Then
                    If Player.Mono Then
                        SNamedPipeServer.Write(ID & Spacer & 1, My.Settings.API_TIMEOUT)
                    Else
                        SNamedPipeServer.Write(ID & Spacer & 0, My.Settings.API_TIMEOUT)
                    End If
                End If
            Case CMDType.GetVolume
                If AllowGetCalls Then
                    SNamedPipeServer.Write(ID & Spacer & Player.Volume, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetPosition
                If AllowGetCalls Then
                    SNamedPipeServer.Write(ID & Spacer & Player.GetPosition, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetLength
                If AllowGetCalls Then
                    SNamedPipeServer.Write(ID & Spacer & Player.GetLength, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetTags
                If AllowGetCalls Then
                    Dim Tag = Utils.GetSongInfo(Player.SourceURL)
                    SNamedPipeServer.Write(ID & Spacer & Tag(0) & Spacer & Tag(1) & Spacer & Tag(2) & Spacer & Tag(3) & Spacer & Tag(4) & Spacer & Tag(5) & Spacer & Tag(6) & Spacer & Tag(7), My.Settings.API_TIMEOUT)
                End If
            Case CMDType.GetPeak
                If AllowGetCalls Then
                    Dim Peak = Player.GetPeak
                    SNamedPipeServer.Write(ID & Spacer & Peak.Left & Spacer & Peak.Right & Spacer & Peak.Master, My.Settings.API_TIMEOUT)
                End If
            Case CMDType.Ping
                SNamedPipeServer.Write(ID & Spacer & AllowGetCalls & Spacer & AllowSetCalls & Spacer & AllowRaisingEvents & Spacer & Process.GetCurrentProcess.Id)
            Case CMDType.SetPlay
                If AllowSetCalls Then
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Player.StreamPlay()
                                                          End Sub)
                End If
            Case CMDType.SetPause
                If AllowSetCalls Then
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Player.StreamPause()
                                                          End Sub)
                End If
            Case CMDType.SetNext
                If AllowSetCalls Then
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Window.media_next_btn_Click(Nothing, New RoutedEventArgs)
                                                          End Sub)
                End If
            Case CMDType.SetPrevious
                If AllowSetCalls Then
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Window.media_prev_btn_Click(Nothing, New RoutedEventArgs)
                                                          End Sub)
                End If
            Case CMDType.SetVolume
                If AllowSetCalls Then
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Player.SetVolume(CMD / 100)
                                                          End Sub)
                End If
            Case CMDType.SetPosition
                If AllowSetCalls Then
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Player.SetPosition(CMD)
                                                          End Sub)
                End If
            Case CMDType.PluginRegister
                Utils.RemoveAt(CMDARGS, 0)
                Dim Plugin = Utils.PluginItem.FromString(Spacer, String.Join(Spacer, CMDARGS))
                Select Case MessageBox.Show("Do you want to install the following plugin: " & Plugin.Name & vbCrLf & "Note: to manage api permissions go to ""Other"" in Settings", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    Case MessageBoxResult.Yes
                        Dim Rnd As New Random
                        Dim PlugID As Integer = Rnd.Next
                        Plugin.ID = PlugID
                        My.Settings.API_Plugins.Add(Plugin.ToString(Spacer))
                        My.Settings.Save()
                        SNamedPipeServer.Write(CMDType.PluginRegister & Spacer & PlugID)
                    Case MessageBoxResult.No
                        SNamedPipeServer.Write(CMDType.PluginRegister & Spacer & 1)
                End Select
            Case CMDType.PluginPing
                Dim PlugID As Integer : Integer.TryParse(CMD, PlugID)
                If PlugID <> 0 Then
                    Dim Plugins As New List(Of Utils.PluginItem)
                    For Each plug In My.Settings.API_Plugins
                        Plugins.Add(Utils.PluginItem.FromString(Spacer, plug))
                    Next
                    Dim DBPlug = Plugins.FirstOrDefault(Function(k) k.ID = PlugID)
                    If DBPlug IsNot Nothing Then
                        SNamedPipeServer.Write(CMDType.PluginPing & Spacer & True)
                    Else
                        SNamedPipeServer.Write(CMDType.PluginPing & Spacer & False)
                    End If
                End If
        End Select
    End Sub
#Region "Player Events"
    Private Sub Player_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles Player.MediaLoaded
        If AllowRaisingEvents Then
            SNamedPipeServer.Write(CMDType.EventOnMediaLoaded & Spacer & "NOTHING", My.Settings.API_TIMEOUT)
        End If
    End Sub

    Private Sub Player_OnRepeatChanged(NewType As Player.RepeateBehaviour) Handles Player.OnRepeatChanged
        If AllowRaisingEvents Then
            SNamedPipeServer.Write(CMDType.EventOnRepeatChanged & Spacer & NewType, My.Settings.API_TIMEOUT)
        End If
    End Sub

    Private Sub Player_OnShuffleChanged(NewType As Player.RepeateBehaviour) Handles Player.OnShuffleChanged
        If AllowRaisingEvents Then
            SNamedPipeServer.Write(CMDType.EventOnShuffleChanged & Spacer & NewType, My.Settings.API_TIMEOUT)
        End If
    End Sub

    Private Sub Player_PlayerStateChanged(State As Player.State) Handles Player.PlayerStateChanged
        If AllowRaisingEvents Then
            SNamedPipeServer.Write(CMDType.EventOnStateChanged & Spacer & State, My.Settings.API_TIMEOUT)
        End If
    End Sub

    Private Sub Player_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles Player.VolumeChanged
        If AllowRaisingEvents Then
            SNamedPipeServer.Write(CMDType.EventOnVolumeChanged & Spacer & NewVal & Spacer & IsMuted, My.Settings.API_TIMEOUT)
        End If
    End Sub
#End Region

    Public Enum CMDType
        GetCurrent = 0
        GetCover = 1
        GetRepeatShuffleType = 2
        GetIsMono = 3
        GetVolume = 4
        GetPosition = 5
        GetLength = 6
        GetTags = 7
        GetPeak = 8
        Ping = 9
        SetPlay = 1000
        SetPause = 1001
        SetNext = 1002
        SetPrevious = 1003
        SetPosition = 1004
        SetVolume = 1005
        EventOnMediaLoaded = 2000
        EventOnRepeatChanged = 2001
        EventOnShuffleChanged = 2002
        EventOnStateChanged = 2003
        EventOnVolumeChanged = 2004
        PluginRegister = 3000
        PluginPing = 3001
    End Enum
End Class
