Imports System
Imports System.ComponentModel
Imports Microsoft.WindowsAPICodePack.Taskbar
Imports System.Speech.Synthesis
Imports Un4seen.Bass.AddOn.Sfx
Imports System.Windows.Media.Animation
Imports WpfPlayer
Imports System.Windows.Interop
Imports Microsoft.WindowsAPICodePack.Shell
Imports Windows.Media.Playback
Imports HandyControl.Themes
Imports Windows.Media
Imports HandyControl.Data

Class MainWindow
    Private WithEvents MainUIManager As New System.Windows.Threading.DispatcherTimer With {.IsEnabled = False, .Interval = New TimeSpan(0, 0, 0, 0, 100)}
    'Public WithEvents MainUIVisualizerUpdator As New System.Windows.Threading.DispatcherTimer With {.IsEnabled = False, .Interval = New TimeSpan(0, 0, 0, 0, 33)}
    Public WithEvents MainUIVisualizerUpdator As New Forms.Timer With {.Enabled = False, .Interval = 33}
    Public WithEvents MainPlayer As New Player(Me, AddressOf MainPlayer_MediaEnded) With {.SkipSilences = My.Settings.SkipSilences, .FadeAudio = My.Settings.OnMediaChange_FadeAudio, .AutoPlay = My.Settings.Player_AutoPlay}
    Private PreviewPlayer As New Player(Me, Nothing) With {.FadeAudio = False, .AutoPlay = True}
    Public WithEvents MainPlaylist As New Playlist
    Public WithEvents MainLibrary As Library = Nothing
    Public WithEvents MusicBrainz As New MusicBrainz
    Public WithEvents Lyrics As New Lyrics
    Public WithEvents StreamDownloader As New StreamDownloader(MainPlayer)
    Public WithEvents SoundCloud As New SoundCloud
    Public playlistItems As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Public libraryItems As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Public libraryArtistsItems As New ObjectModel.ObservableCollection(Of ArtistItem)
    Private libraryArtistsLVItems As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Public libraryYearsItems As New ObjectModel.ObservableCollection(Of ArtistItem)
    Private libraryYearsLVItems As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Private libraryFavouritesItems As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Public WithEvents RPCClient As DiscordRPC.DiscordRpcClient
    Private _UpdatePlaylist As Boolean = True
    Private IsPlaylistEditMode As Boolean = False
    Private WithEvents TaskBarPrev As ThumbnailToolBarButton
    Private WithEvents TaskBarPlayPause As ThumbnailToolBarButton
    Private WithEvents TaskBarNext As ThumbnailToolBarButton
    Public Previcon As System.Drawing.Icon
    Public Nexticon As System.Drawing.Icon
    Public Playicon As System.Drawing.Icon
    Public Pauseicon As System.Drawing.Icon
    Public Loadingicon As System.Drawing.Icon
    Public MediaBar As New mediabar(MainPlayer)
    Private _source As HwndSource
    Dim Hotkey As GlobalHotkey
    Public HotkeyState As Boolean() = {False, False, False, False, False, False, False, False, False}
    Public Gtracks As List(Of String)
    Public GArtists As List(Of Library.ArtistElement)
    Public GYears As List(Of Library.ArtistElement)
    'Windows 10 10240 Only------------------------------------------------------------------------------------------
#Const WIN1010240 = True
#If WIN1010240 Then
    Dim UWPPlayer As New Windows.Media.Playback.MediaPlayer()
    Dim IsUWPPlayerAvailable As Boolean = False
    Dim WithEvents SMTCPlayer As Windows.Media.SystemMediaTransportControls = UWPPlayer.SystemMediaTransportControls
#End If
    Public Property UpdatePlaylist As Boolean
        Get
            Return _UpdatePlaylist
        End Get
        Set(value As Boolean)
            If value = True Then
                _UpdatePlaylist = True
            Else
                _UpdatePlaylist = False
                SetUpdatePlaylistDefault()
            End If
        End Set
    End Property
    Dim Synth As New SpeechSynthesizer
    Dim isSFXLoaded As Boolean = False
    Dim Visualiser_Target As New Forms.PictureBox With {.Dock = Forms.DockStyle.Fill, .SizeMode = Forms.PictureBoxSizeMode.StretchImage}
    Dim PosDurSwitch As Boolean = False
    'Dim WithEvents BS As New BuildString
    Public TaskbarThumbnailManager As CustomTaskBarThumb
    Dim PipeManager As NamedPipeManager = New NamedPipeManager("MuPlayPipe")
    Public PIPO As API 'Yep PIPO seems a good name
#Region "Utils"
    Public Sub Overlay(IsVisible As Boolean, isLoading As Boolean)
        If IsVisible = True Then
            QuickAccess_Overlay.Visibility = Visibility.Visible
            Dim BlurFx As New System.Windows.Media.Effects.BlurEffect With {.KernelType = System.Windows.Media.Effects.KernelType.Gaussian, .Radius = 5, .RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance}
            Home_Rec1.Effect = BlurFx
            Home_Rec2.Effect = BlurFx
            Home_Rec3.Effect = BlurFx
            Home_Rec4.Effect = BlurFx
            Home_Rec5.Effect = BlurFx
            Home_Rec6.Effect = BlurFx
            Home_Rec7.Effect = BlurFx
            Home_TopBarCanvas.Effect = BlurFx
            BlurFx = Nothing
            If isLoading Then
                overlay_loadingline.Visibility = Visibility.Visible
            End If
        Else
            QuickAccess_Overlay.Visibility = Visibility.Collapsed
            Dim BlurFx As New System.Windows.Media.Effects.DropShadowEffect
            Home_Rec1.Effect = BlurFx
            Home_Rec2.Effect = BlurFx
            Home_Rec3.Effect = BlurFx
            Home_Rec4.Effect = BlurFx
            Home_Rec5.Effect = BlurFx
            Home_Rec6.Effect = BlurFx
            Home_Rec7.Effect = BlurFx
            Home_TopBarCanvas.Effect = Nothing
            BlurFx = Nothing
            overlay_loadingline.Visibility = Visibility.Hidden
        End If
    End Sub
    Public Sub UpdateSkin(ByVal skin As HandyControl.Themes.ApplicationTheme)
        ThemeManager.Current.ApplicationTheme = skin
        Try
            Select Case skin
                Case HandyControl.Themes.ApplicationTheme.Dark
                    'TitleBar_Clock.Foreground = Brushes.White
                    'Visualiser_Host.Background = Brushes.Black                    
                Case HandyControl.Themes.ApplicationTheme.Light
                    'TitleBar_Clock.Foreground = Brushes.Black
                    'Visualiser_Host.Background = Nothing                    
            End Select
        Catch ex As Exception
        End Try
    End Sub
    Private Sub SystemThemeChanged(sender As Object, e As FunctionEventArgs(Of ThemeManager.SystemTheme))
        Select Case e.Info.CurrentTheme
            Case ApplicationTheme.Dark
                Home_FancyBackground.Background = Brushes.Black
            Case ApplicationTheme.Light
                Home_FancyBackground.Background = Nothing
        End Select
    End Sub
    Public Async Sub ShowNotification(Title As String, Message As String, Icon As HandyControl.Data.NotifyIconInfoType)
        MEDIA_TITLE.Visibility = Visibility.Collapsed
        MEDIA_ARTIST.Visibility = Visibility.Collapsed
        NotifyBlock.Content = Message
        NotifyBlock.Visibility = Visibility.Visible
        NotifyIconMain.ShowBalloonTip(Title, Message, Icon)
        NotifyIconMain.BlinkInterval = TimeSpan.FromMilliseconds(500)
        If My.Settings.Notificationtts Then
            Dim oldvol = MainPlayer.Volume
            Await MainPlayer.FadeVol(0.1, 1)
            Await Task.Run(Sub()
                               Synth.Speak(Message)
                           End Sub)
            Await MainPlayer.FadeVol(oldvol, 1)
        End If
        Try
            NotifyIconMain.IsBlink = True
            Await Task.Delay(6000)
            NotifyIconMain.IsBlink = False
        Catch ex As Exception
            NotifyIconMain.IsBlink = False
        End Try
        MEDIA_TITLE.Visibility = Visibility.Visible
        MEDIA_ARTIST.Visibility = Visibility.Visible
        NotifyBlock.Content = Nothing
        NotifyBlock.Visibility = Visibility.Collapsed
    End Sub
    Public Sub RefreshRecommended()
        If My.Settings.LibrariesPath.Count <> 0 Then
            ''Dim files As New List(Of String)
            ''For Each path In My.Settings.LibrariesPath
            ''    For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
            ''        files.Add(song)
            ''    Next
            ''Next
            Dim files = Gtracks
            Dim Cover As System.Drawing.Bitmap = Nothing
            Dim Rnd As New Random
            Dim SongInfo As String()
            Try
                Home_Rec1.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec1.Tag)
                Home_Rec1.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec1.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec1.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec1.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            Try
                Home_Rec2.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec2.Tag)
                Home_Rec2.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec2.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec2.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec2.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            Try
                Home_Rec3.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec3.Tag)
                Home_Rec3.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec3.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec3.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec3.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            Try
                Home_Rec4.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec4.Tag)
                Home_Rec4.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec4.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec4.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec4.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            Try
                Home_Rec5.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec5.Tag)
                Home_Rec5.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec5.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec5.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec5.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            Try
                Home_Rec6.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec6.Tag)
                Home_Rec6.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec6.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec6.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec6.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            Try
                Home_Rec7.Tag = files(Rnd.Next(0, files.Count - 1))
                SongInfo = Utils.GetSongInfo(Home_Rec7.Tag)
                Home_Rec7.ToolTip = SongInfo(1) & "[" & SongInfo(4) & "] By: " & SongInfo(0) & " From: " & SongInfo(2) & "[" & SongInfo(3) & "]"
                Cover = Utils.GetAlbumArt(Home_Rec7.Tag)
                If Cover IsNot Nothing Then
                    Home_Rec7.Source = Utils.ImageSourceFromBitmap(Cover)
                Else
                    Home_Rec7.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"))
                End If
            Catch ex As Exception
            End Try
            files = Nothing
            Cover = Nothing
            Rnd = Nothing
            SongInfo = Nothing
        End If
    End Sub
    Private Sub RefreshPlaylistNums()
        For i As Integer = 0 To Playlist_Main.Items.Count - 1
            playlistItems(i).Num = i + 1
        Next
    End Sub
    Private Async Sub SetUpdatePlaylistDefault()
        Await Task.Delay(10)
        UpdatePlaylist = True
    End Sub
    Public Sub AddToJlist(path As String)
        Try
            JumpList.AddToRecent(path)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Protected Function WndProc(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
        Select Case msg
            Case Constants.WM_HOTKEY_MSG_ID
                HandleHotkey(wParam.ToString)
                My.Windows.Console.Log(wParam.ToString & "-" & lParam.ToString)
                'Case &H400 '                
                '    BS.BuildString(lParam)
                '    My.Windows.Console.Log(wParam.ToString & "-" & lParam.ToString)
                'Case &H500 'api                
                '    BS.BuildString(lParam)
                '    My.Windows.Console.Log(wParam.ToString & "-" & lParam.ToString)
        End Select
    End Function
    'Private Sub BS_StringOK(Result As String) Handles BS.StringOK
    '    My.Windows.Console.Log("Received APPMSG : " & Result)
    '    If Result.Split(">>")(0) = "-api" Then
    '        Dim ToHwnd = New IntPtr(CInt(Result.Split(">>")(3).Replace("-", "")))
    '        Select Case Result.Split(">>")(2)
    '            Case "-NOTIF_SHOW"
    '                ShowNotification("API", Process.GetProcessById(Result.Split(">>")(4).Replace("-", "")).ProcessName & "has now access to MuPlay.", HandyControl.Data.NotifyIconInfoType.Warning)
    '            Case "-GET_URL"
    '                BS.PostString(ToHwnd, &H400, 0, MainPlayer.SourceURL)
    '        End Select
    '    Else
    '        Dim Rargs As New List(Of String)
    '        Dim Args = Result.Split(">>")
    '        For Each arg In Args
    '            If Not String.IsNullOrEmpty(arg) AndAlso Not String.IsNullOrWhiteSpace(arg) Then
    '                Rargs.Add(arg)
    '            End If
    '        Next
    '        For Each song In Rargs
    '            MainPlayer.LoadSong(song, MainPlaylist)
    '        Next
    '        MainPlayer.StreamPlay()
    '    End If
    'End Sub
#End Region
    Private Sub RPCClient_OnConnectionFailed() Handles RPCClient.OnConnectionFailed
        'Me.Dispatcher.BeginInvoke(New System.Threading.ThreadStart(Sub()
        '                                                               ShowNotification("MuPlay", "Discord Rich Presence failed to connect.", HandyControl.Data.NotifyIconInfoType.Error)
        '                                                           End Sub))
    End Sub
    Private Sub RPCClient_OnConnectionEstablished() Handles RPCClient.OnConnectionEstablished
        Me.Dispatcher.BeginInvoke(New System.Threading.ThreadStart(Sub()
                                                                       ShowNotification("MuPlay", "Discord Rich Presence connected successfully.", HandyControl.Data.NotifyIconInfoType.Info)
                                                                   End Sub))
    End Sub

    Private Sub MainUIManager_Tick() Handles MainUIManager.Tick
        TitleBar_Clock.Text = DateTime.Now.ToString("HH:mm")
        If Not media_track.IsMouseOver Then
            media_track.Value = MainPlayer.GetPosition
        End If
        If PosDurSwitch Then
            Dim PosnDurCnt As String = MainPlayer.GetPosition & " | " & MainPlayer.GetLength
            media_posndur.Content = PosnDurCnt
            Resources("Seek_Tip") = PosnDurCnt
            PosnDurCnt = Nothing
        Else
            Dim PosnDurCnt As String = Utils.SecsToMins(MainPlayer.GetPosition) & " | " & Utils.SecsToMins(MainPlayer.GetLength)
            media_posndur.Content = PosnDurCnt
            Resources("Seek_Tip") = PosnDurCnt
            PosnDurCnt = Nothing
        End If
#If WIN1010240 Then
        SMTCPlayer.UpdateTimelineProperties(New SystemMediaTransportControlsTimelineProperties With {.StartTime = TimeSpan.Zero, .EndTime = TimeSpan.FromSeconds(MainPlayer.GetLength), .Position = TimeSpan.FromSeconds(MainPlayer.GetPosition), .MinSeekTime = TimeSpan.FromSeconds(1), .MaxSeekTime = TimeSpan.FromSeconds(10)})
#End If
        If MainPlayer.PlayerState <> Player.State.Undefined Then
            Try
                TaskbarManager.Instance.SetProgressValue(MainPlayer.GetPosition, MainPlayer.GetLength, Application.Current.MainWindow)
            Catch ex As Exception
            End Try
        End If
    End Sub
    Private Sub MainUIVisualizerUpdator_Tick() Handles MainUIVisualizerUpdator.Tick
        Try
            If MainTabCtrl.SelectedIndex = 0 Then
                Home_Visualizer.Source = Utils.ImageSourceFromBitmap(MainPlayer.CreateVisualizer(Player.Visualizers.SpectumLine, 225, 115, System.Drawing.Color.Black, System.Drawing.Color.Black, System.Drawing.Color.DarkGray, System.Drawing.Color.Empty, 5, 2, 1, 25, False, False, False))
                'media_cover.Source = Utils.ImageSourceFromBitmap(MainPlayer.CreateVisualizer(Player.Visualizers.SpectrumPeak, media_cover.Width, media_cover.Height, System.Drawing.Color.Black, System.Drawing.Color.Red, Nothing, System.Drawing.Color.Empty, 5, 1, 1, 200, False, False, False))
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub drawerclosebtn_Click(sender As Object, e As RoutedEventArgs) Handles drawerclosebtn.Click
        DrawerLeft.IsOpen = False
    End Sub
    Private Async Sub HandleNamedPipe_OpenRequest(filesToOpen As String)
        'Dim Rargs As New List(Of String)
        Dim Args = filesToOpen.Split(">")
        'For Each arg In Args
        '    If Not String.IsNullOrEmpty(arg) AndAlso Not String.IsNullOrWhiteSpace(arg) Then
        '        Rargs.Add(arg)
        '    End If
        'Next
        Await Dispatcher.BeginInvoke(Sub()
                                         For Each song In Args
                                             MainPlaylist.Add(song, Player.StreamTypes.Local)
                                         Next
                                         MainPlayer.LoadSong(MainPlaylist.JumpTo(MainPlaylist.Count - 1), MainPlaylist, False)
                                         MainPlayer.StreamPlay()
                                     End Sub)
    End Sub
    Private Sub Window_Initialized(sender As Object, e As EventArgs)
        If My.Settings.API Then
            PIPO = New API
        End If
        PipeManager.StartServer()
        AddHandler PipeManager.ReceiveString, AddressOf HandleNamedPipe_OpenRequest
        If MainPlayer.IsInitialized = True Then
            MainUIManager.Start()
        Else
            If MainPlayer.IsInitializedReason IsNot Nothing Then
                If MessageBox.Show(Me, "Error: " & MainPlayer.IsInitializedReason.InnerException.Message & vbCrLf & "Pressing Yes will run MuPlay but full with errors.", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Error) = MessageBoxResult.Yes Then
                    Close()
                End If
            Else
                If MessageBox.Show(Me, "An unknow error occured. Pressing Yes will run MuPlay but full with errors.", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Error) = MessageBoxResult.No Then
                    Close()
                End If
            End If
        End If
        Dim s As Style = New Style()
        s.Setters.Add(New Setter(UIElement.VisibilityProperty, Visibility.Collapsed))
        MainTabCtrl.ItemContainerStyle = s
        media_loading.Visibility = Visibility.Visible
        media_play_btn.Visibility = Visibility.Hidden
        If My.Settings.UseDiscordRPC Then
            RPCClient = New DiscordRPC.DiscordRpcClient("779393129366683719")
        End If
        Try
            If BassSfx.BASS_SFX_Init(System.Diagnostics.Process.GetCurrentProcess().Handle, Visualiser_Host.Handle) Then
                isSFXLoaded = True
            End If
        Catch ex As Exception
            isSFXLoaded = False
        End Try
        For Each dsp In My.Settings.DSP_Plugins
            MainPlayer.ToBeLoadedDSP.Add(dsp)
        Next
        '' Deezerapi = Deezer.DeezerSession.CreateNew
        System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
    End Sub
    Private Sub MainWindow_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        ''Jlist
        'Dim Tlist = Microsoft.WindowsAPICodePack.Taskbar.JumpList.CreateJumpList
        'Tlist.KnownCategoryToDisplay = JumpListKnownCategoryType.Recent
        'Dim TlistCat As New JumpListCustomCategory("Playlists")
        'Dim PS_ALL As New JumpListLink(Utils.AppPath, "All songs") With {.IconReference = New IconReference(Utils.AppPath, 0), .Arguments = "-p -a"}
        'Dim PS_FAV As New JumpListLink(Utils.AppPath, "All favourites") With {.IconReference = New IconReference(Utils.AppPath, 0), .Arguments = "-p -f"}
        'Dim PS_SFL As New JumpListLink(Utils.AppPath, "All songs shuffled") With {.IconReference = New IconReference(Utils.AppPath, 0), .Arguments = "-p -s"}
        'Dim PS_RND As New JumpListLink(Utils.AppPath, "10 random songs") With {.IconReference = New IconReference(Utils.AppPath, 0), .Arguments = "-p -r"}
        'TlistCat.AddJumpListItems({PS_ALL, PS_FAV, PS_SFL, PS_RND})
        'Tlist.AddCustomCategories(TlistCat)
        'Tlist.Refresh()
        ''Done Jlist
    End Sub
    Private Sub PrimWindow_Closed(sender As Object, e As EventArgs) Handles PrimWindow.Closed
        PipeManager.StopServer()
        _source.RemoveHook(AddressOf WndProc)
        _source = Nothing
    End Sub
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Load()
    End Sub
    Public FancyBackgroundManager As ParticleManager
    Private Async Function Load() As Task
        Dim wasitfirststart As Boolean = My.Settings.IsFirstStart
        Await Dispatcher.BeginInvoke(Async Function()
                                         If My.Settings.IsFirstStart Then
                                             My.Windows.InitSetup.Owner = Me
                                             My.Windows.InitSetup.ShowDialog()
                                             My.Settings.IsFirstStart = False
                                             My.Settings.Save()
                                         End If
                                         Dim ghostwindow As New Window With {.WindowState = WindowState.Minimized}
                                         ghostwindow.Show()
                                         TaskbarThumbnailManager = New CustomTaskBarThumb(ghostwindow)
                                         If Not String.IsNullOrEmpty(My.Settings.Library_Path) Then
                                             MainLibrary = New Library(My.Settings.Library_Path)
                                             If MainLibrary.IsLoaded = False Then
                                                 If MessageBox.Show(Me, "Something is wrong with the library configuration." & vbCrLf & "Yes: manually locate library file" & vbCrLf & "No: recreate the library files", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Error) = MessageBoxResult.Yes Then
                                                     Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .InitialDirectory = Utils.AppDataPath}
                                                     Do
                                                         If OFD.ShowDialog Then
                                                             If Not String.IsNullOrEmpty(MainLibrary.LoadLibrary(OFD.FileName)) Then
                                                                 My.Settings.Library_Path = OFD.FileName
                                                                 My.Settings.Save()
                                                                 Exit Do
                                                             End If
                                                         End If
                                                     Loop
                                                 Else
                                                     Dim Temp_List As New List(Of String)
                                                     For Each path In My.Settings.LibrariesPath
                                                         Temp_List.Add(path)
                                                     Next
                                                     If MainLibrary.LoadLibrary(Await Library.MakeLibrary(Utils.AppDataPath, Temp_List)) Then
                                                         My.Settings.Library_Path = MainLibrary.LibraryPath
                                                         My.Settings.Save()
                                                         Temp_List = Nothing
                                                         Dim files As New List(Of String)
                                                         For Each path In My.Settings.LibrariesPath
                                                             For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                                                 files.Add(song)
                                                             Next
                                                         Next
                                                         Await MainLibrary.AddTracksToLibraryAsync(files)
                                                     Else
                                                         MessageBox.Show(Me, "An unknown error occured with your library. We strongly recommend you to remake the library" & vbCrLf & "Use the rebuild command ""library make"" from the command excuter ""SHIFT+C""", "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
                                                     End If
                                                 End If
                                             End If
                                         Else
                                             Select Case MessageBox.Show(Me, "MuPlay didn't find any library object to load data from." & vbCrLf & "In order to use the library explorer you have to make a library first." & vbCrLf & "Do you want to make one now or locate a previous library ?" & vbCrLf & "Yes: Make one" & vbCrLf & "No: Locate one" & vbCrLf & "Cancel: Continue anyway", "MuPlay", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning)
                                                 Case MessageBoxResult.Yes
                                                     Dim Temp_List As New List(Of String)
                                                     For Each path In My.Settings.LibrariesPath
                                                         Temp_List.Add(path)
                                                     Next
                                                     Dim libpath = Await Library.MakeLibrary(Utils.AppDataPath, Temp_List)
                                                     My.Settings.Library_Path = libpath
                                                     My.Settings.Save()
                                                     Temp_List = Nothing
                                                     If String.IsNullOrEmpty(libpath) Then
                                                         MessageBox.Show(Me, "MuPlay couldn't make a library, try again later.", "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
                                                     Else
                                                         MainLibrary = New Library(My.Settings.Library_Path)
                                                         Dim files As New List(Of String)
                                                         For Each path In My.Settings.LibrariesPath
                                                             For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                                                 files.Add(song)
                                                             Next
                                                         Next
                                                         Await MainLibrary.AddTracksToLibraryAsync(files)
                                                     End If
                                                 Case MessageBoxResult.No
                                                     Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .InitialDirectory = Utils.AppDataPath}
                                                     Do
                                                         If OFD.ShowDialog Then
                                                             MainLibrary = New Library(OFD.FileName)
                                                             If MainLibrary IsNot Nothing Then
                                                                 My.Settings.Library_Path = OFD.FileName
                                                                 My.Settings.Save()
                                                                 Exit Do
                                                             End If
                                                         End If
                                                     Loop
                                             End Select
                                         End If
                                         Playlist_Main.ItemsSource = playlistItems
                                         Library_Tracks_Lview.ItemsSource = libraryItems
                                         Library_Artists_Lview.ItemsSource = libraryArtistsItems
                                         Library_Years_Lview.ItemsSource = libraryYearsItems
                                         Library_ArtistsLV.ItemsSource = libraryArtistsLVItems
                                         Library_YearsLV.ItemsSource = libraryYearsLVItems
                                         Library_Favourites_Lview.ItemsSource = libraryFavouritesItems
                                         If Not String.IsNullOrEmpty(My.Settings.LastMedia) Then
                                             Dim Dialog As New Ookii.Dialogs.Wpf.TaskDialog With {.AllowDialogCancellation = False, .CenterParent = True}
                                             If My.Settings.LastPlaylist.Count <= 1 Then
                                                 Dialog.MainInstruction = "Do you want continue your last session ?"
                                             Else
                                                 Dialog.MainInstruction = "Do you want continue your last session " & My.Settings.LastPlaylist.Count & " Song were played last time."
                                             End If
                                             Dialog.WindowTitle = "MuPlayer"
                                             Dim GoToMedia As New Ookii.Dialogs.Wpf.TaskDialogButton With {.Text = "Yes", .CommandLinkNote = My.Settings.LastMediaTitle & " By: " & My.Settings.LastMediaArtist & " > " & Utils.SecsToMins(My.Settings.LastMediaSeek) & "/" & Utils.SecsToMins(My.Settings.LastMediaDuration) & "[" & System.Enum.GetName(GetType(Player.StreamTypes), My.Settings.LastMediaType) & "]"}
                                             Dim GoToHome As New Ookii.Dialogs.Wpf.TaskDialogButton With {.Text = "No"}
                                             Dialog.ButtonStyle = Ookii.Dialogs.Wpf.TaskDialogButtonStyle.CommandLinks
                                             Dialog.Buttons.Add(GoToMedia)
                                             Dialog.Buttons.Add(GoToHome)
                                             If Dialog.ShowDialog(Application.Current.MainWindow) Is GoToMedia Then
                                                 If My.Settings.LastPlaylist.Count >= 1 Then
                                                     For Each song In My.Settings.LastPlaylist
                                                         UpdatePlaylist = False
                                                         Dim sSong = song.Substring(0, song.IndexOf(">>"))
                                                         Dim sType = song.Substring(song.IndexOf(">>") + 2, 1)
                                                         Select Case sType
                                                             Case 0 'URL
                                                                 'And after many tests we conclude :
                                                                 Dim TrimmedSong As String = song.Substring(song.IndexOf(">>") + 5)
                                                                 Dim OCMTitle As String = TrimmedSong.Substring(0, TrimmedSong.IndexOf(">>"))
                                                                 TrimmedSong = TrimmedSong.Substring(TrimmedSong.IndexOf(">>") + 2)
                                                                 Dim OCMArtist As String = TrimmedSong.Substring(0, TrimmedSong.IndexOf(">>"))
                                                                 TrimmedSong = TrimmedSong.Substring(TrimmedSong.IndexOf(">>") + 2)
                                                                 Dim OCMYear As Integer = TrimmedSong
                                                                 MainPlaylist.Add(Nothing, Player.StreamTypes.URL, True, True, sSong, True, OCMTitle, OCMArtist, OCMYear)
                                                             Case 1 'YouTube
                                                                 'No retries here just copy/paste
                                                                 Dim TrimmedSong As String = song.Substring(song.IndexOf(">>") + 5)
                                                                 Dim OCMTitle As String = TrimmedSong.Substring(0, TrimmedSong.IndexOf(">>"))
                                                                 TrimmedSong = TrimmedSong.Substring(TrimmedSong.IndexOf(">>") + 2)
                                                                 Dim OCMArtist As String = TrimmedSong.Substring(0, TrimmedSong.IndexOf(">>"))
                                                                 TrimmedSong = TrimmedSong.Substring(TrimmedSong.IndexOf(">>") + 2)
                                                                 Dim OCMYear As Integer = TrimmedSong
                                                                 MainPlaylist.Add(Nothing, Player.StreamTypes.Youtube, True, True, sSong, True, OCMTitle, OCMArtist, OCMYear)
                                                             Case 3 'Local
                                                                 MainPlaylist.Add(sSong, sType)
                                                         End Select
                                                         sSong = Nothing
                                                         sType = Nothing
                                                     Next
                                                     MainPlaylist.SetIndex(My.Settings.LastMediaIndex)
                                                 End If
                                                 Select Case My.Settings.LastMediaType
                                                     Case 0 'URL                        
                                                         MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, My.Settings.LastMedia, True, "Not Available", "Not Available", Nothing, 0)
                                                     Case 1 'YouTube
                                                         Overlay(True, True)
                                                         Await MainPlayer.LoadStreamAsync(My.Settings.LastMedia, Player.StreamTypes.Youtube, MainPlaylist, Nothing, True)
                                                         Overlay(False, False)
                                                     Case 3 'Local
                                                         MainPlayer.LoadSong(My.Settings.LastMedia, MainPlaylist, False)
                                                         MainPlayer.SetPosition(My.Settings.LastMediaSeek)
                                                 End Select
                                             End If
                                             Dialog.Dispose()
                                         End If
                                         If Not My.Settings.UseAnimations Then
                                             Switches_TopBarCanvasVisChanger.IsChecked = True
                                             Switches_RecChanger.IsChecked = True
                                         End If
                                         If My.Settings.UseDiscordRPC Then
                                             RPCClient.Initialize()
                                         End If
                                         If My.Settings.DefaultTheme <> 2 Then
                                             Try
                                                 UpdateSkin(My.Settings.DefaultTheme)
                                             Catch ex As Exception
                                             End Try
                                         Else
                                             ThemeManager.Current.UsingSystemTheme = True
                                             ThemeManager.Current.ApplicationTheme = ThemeManager.GetSystemTheme
                                             ThemeManager.Current.AccentColor = ThemeManager.Current.GetAccentColorFromSystem
                                             AddHandler ThemeManager.Current.SystemThemeChanged, AddressOf SystemThemeChanged
                                         End If
                                         Visualiser_Host.Child = Visualiser_Target
                                         If My.Settings.BackgroundType = 1 Then
                                             Home_Background.Visibility = Visibility.Hidden
                                             Home_FancyBackground.Visibility = Visibility.Visible
                                             'DoAnim(1)
                                             'AniCounter += 1
                                             FancyBackgroundManager = New ParticleManager(FancyCanvas, Me, 500, 5)
                                         End If
                                         'Taskbar Stuff
                                         Dim Previconstream = Application.GetResourceStream(New Uri("pack://application:,,,/WpfPlayer;component/Res/previous.ico")).Stream
                                         Previcon = New System.Drawing.Icon(Previconstream)
                                         Dim Playiconstream = Application.GetResourceStream(New Uri("pack://application:,,,/WpfPlayer;component/Res/play.ico")).Stream
                                         Playicon = New System.Drawing.Icon(Playiconstream)
                                         Dim Pauseiconstream = Application.GetResourceStream(New Uri("pack://application:,,,/WpfPlayer;component/Res/pause.ico")).Stream
                                         Pauseicon = New System.Drawing.Icon(Pauseiconstream)
                                         Dim Nexticonstream = Application.GetResourceStream(New Uri("pack://application:,,,/WpfPlayer;component/Res/next.ico")).Stream
                                         Nexticon = New System.Drawing.Icon(Nexticonstream)
                                         Dim Loadingiconstream = Application.GetResourceStream(New Uri("pack://application:,,,/WpfPlayer;component/Res/cdisk.ico")).Stream
                                         Loadingicon = New System.Drawing.Icon(Loadingiconstream)
                                         TaskBarPrev = New ThumbnailToolBarButton(Previcon, "Previous")
                                         TaskBarPlayPause = New ThumbnailToolBarButton(Playicon, "Play")
                                         TaskBarNext = New ThumbnailToolBarButton(Nexticon, "Next")
                                         Dim helper = New WindowInteropHelper(Me)
                                         TaskbarManager.Instance.ThumbnailToolBars.AddButtons(helper.Handle, {TaskBarPrev, TaskBarPlayPause, TaskBarNext})
                                         Previconstream.Dispose()
                                         Playiconstream.Dispose()
                                         Pauseiconstream.Dispose()
                                         Nexticonstream.Dispose()
                                         Loadingiconstream.Dispose()
                                         _source = HwndSource.FromHwnd(helper.Handle)
                                         _source.AddHook(AddressOf WndProc)
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_PlayPause_MOD), My.Settings.GlobalHotkey_PlayPause, helper.Handle, 0)
                                         HotkeyState(0) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_Next_MOD), My.Settings.GlobalHotkey_Next, helper.Handle, 1)
                                         HotkeyState(1) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_Previous_MOD), My.Settings.GlobalHotkey_Previous, helper.Handle, 2)
                                         HotkeyState(2) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_Skip10_MOD), My.Settings.GlobalHotkey_Skip10, helper.Handle, 3)
                                         HotkeyState(3) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_Back10_MOD), My.Settings.GlobalHotkey_Back10, helper.Handle, 4)
                                         HotkeyState(4) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_VolumeUp_MOD), My.Settings.GlobalHotkey_VolumeUp, helper.Handle, 5)
                                         HotkeyState(5) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_VolumeDown_MOD), My.Settings.GlobalHotkey_VolumeDown, helper.Handle, 6)
                                         HotkeyState(6) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_VolumeMute_MOD), My.Settings.GlobalHotkey_VolumeMute, helper.Handle, 7)
                                         HotkeyState(7) = Hotkey.Register()
                                         Hotkey = New GlobalHotkey(Utils.IntToMod(My.Settings.GlobalHotkey_NowPlaying_MOD), My.Settings.GlobalHotkey_NowPlaying, helper.Handle, 8)
                                         HotkeyState(8) = Hotkey.Register()
                                         If MainLibrary IsNot Nothing Then
                                             Gtracks = Await MainLibrary.GroupTracksAsync
                                             For i As Integer = 0 To Gtracks.Count - 1
                                                 Try
                                                     Dim Tag = TagLib.File.Create(Gtracks(i)).Tag
                                                     libraryItems.Add(New PlaylistItem(i + 1, Tag.Title, Tag.JoinedPerformers, Tag.Album, Tag.Year, Tag.Track, Player.StreamTypes.Local, Gtracks(i), Nothing))
                                                 Catch ex As Exception
                                                     libraryItems.Add(New PlaylistItem(i + 1, IO.Path.GetFileNameWithoutExtension(Gtracks(i)), "Not available", "Not available", 0, 0, Player.StreamTypes.Local, Gtracks(i), Nothing))
                                                 End Try
                                             Next
                                             If My.Settings.CacheLibraryData Then
                                                 GArtists = Await MainLibrary.ReadArtistsCache(Utils.AppDataPath)
                                             Else
                                                 GArtists = Await MainLibrary.GroupArtistsAsync
                                             End If
                                             For i As Integer = 0 To GArtists.Count - 1
                                                 Try
                                                     libraryArtistsItems.Add(New ArtistItem(i + 1, GArtists(i)))
                                                 Catch ex As Exception
                                                 End Try
                                             Next
                                             If My.Settings.CacheLibraryData Then
                                                 GYears = Await MainLibrary.ReadYearsCache(Utils.AppDataPath)
                                             Else
                                                 GYears = Await MainLibrary.GroupYearsAsync
                                             End If
                                             For i As Integer = 0 To GYears.Count - 1
                                                 Try
                                                     libraryYearsItems.Add(New ArtistItem(i + 1, GYears(i)))
                                                 Catch ex As Exception
                                                 End Try
                                             Next
                                             For i As Integer = 0 To My.Settings.FavouriteTracks.Count - 1
                                                 Try
                                                     Dim Tag = TagLib.File.Create(My.Settings.FavouriteTracks(i)).Tag
                                                     libraryFavouritesItems.Add(New PlaylistItem(i + 1, Tag.Title, Tag.JoinedPerformers, Tag.Album, Tag.Year, Tag.Track, Player.StreamTypes.Local, My.Settings.FavouriteTracks(i), Nothing))
                                                 Catch ex As Exception
                                                     libraryFavouritesItems.Add(New PlaylistItem(i + 1, IO.Path.GetFileNameWithoutExtension(My.Settings.FavouriteTracks(i)), "Not available", "Not available", 0, 0, Player.StreamTypes.Local, My.Settings.FavouriteTracks(i), Nothing))
                                                 End Try
                                             Next
                                         End If
                                     End Function)
        If My.Settings.UseAnimations Then
            Home_Loading.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(1000))))
            Await Task.Delay(1000)
        End If
        Home_Loading.Visibility = Visibility.Hidden
        If My.Settings.UseAnimations Then
            Switches_RecChanger.IsChecked = False
        End If
        RefreshRecommended()
        If My.Settings.UseAnimations Then
            Switches_RecChanger.IsChecked = True
        End If
        If wasitfirststart Then
            Overlay(True, False)
            My.Windows.Changelog.Owner = Me
            My.Windows.Changelog.ShowDialog()
            Overlay(False, False)
        End If
    End Function
    Private Async Sub HandleHotkey(id As IntPtr)
        Select Case id
            Case 0 'PlayPause
                media_play_btn_Click(Nothing, New RoutedEventArgs)
            Case 1 'Next
                media_next_btn_Click(Nothing, New RoutedEventArgs)
            Case 2 'Previous
                media_prev_btn_Click(Nothing, New RoutedEventArgs)
            Case 3 '+10
                MainPlayer.SetPosition(MainPlayer.GetPosition + 10)
            Case 4 '-10
                MainPlayer.SetPosition(MainPlayer.GetPosition - 10)
            Case 5 'V +
                MainPlayer.SetVolume(MainPlayer.Volume + 0.02)
            Case 6 'V -
                MainPlayer.SetVolume(MainPlayer.Volume - 0.02)
            Case 7 'V *
                MainPlayer.Mute = Not MainPlayer.Mute
            Case 8 'Now Playing
                If Not String.IsNullOrEmpty(MainPlayer.CurrentMediaTitle) Then
                    Dim oldvol = MainPlayer.Volume
                    Await MainPlayer.FadeVol(0.1)
                    Await Task.Run(Sub()
                                       Synth.Speak("Now playing. " & MainPlayer.CurrentMediaTitle & ",By " & MainPlayer.CurrentMediaArtist)
                                   End Sub)
                    Await MainPlayer.FadeVol(oldvol)
                End If
        End Select
    End Sub
    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try
            If PIPO IsNot Nothing Then
                PIPO.Dispose()
            End If
            If MainPlayer.SourceURL IsNot Nothing Then
                My.Settings.LastMediaType = MainPlayer.CurrentMediaType
                My.Settings.LastMediaTitle = MainPlayer.CurrentMediaTitle
                My.Settings.LastMediaArtist = MainPlayer.CurrentMediaArtist
                My.Settings.LastMedia = MainPlayer.SourceURL
                My.Settings.LastMediaSeek = MainPlayer.GetPosition
                My.Settings.LastMediaDuration = MainPlayer.GetLength
                My.Settings.LastMediaIndex = MainPlaylist.Index
                My.Settings.LastPlaylist.Clear()
                For Each song In MainPlaylist.Playlist
                    My.Settings.LastPlaylist.Add(song)
                Next
                My.Settings.Save()
                MainPlayer.Dispose()
                visualiser_off_Click(Nothing, New RoutedEventArgs)
                If My.Settings.UseDiscordRPC Then
                    RPCClient.Dispose()
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
#Region "Tab Selectors and  related events"
    Private Sub btn_home_Click(sender As Object, e As RoutedEventArgs) Handles btn_home.Click
        MainTabCtrl.SelectedIndex = 0
        DrawerLeft.IsOpen = False
    End Sub

    Private Sub btn_playlist_Click(sender As Object, e As RoutedEventArgs) Handles btn_playlist.Click
        MainTabCtrl.SelectedIndex = 1
        DrawerLeft.IsOpen = False
    End Sub

    Private Sub btn_visualiser_Click(sender As Object, e As RoutedEventArgs) Handles btn_visualiser.Click
        MainTabCtrl.SelectedIndex = 2
        DrawerLeft.IsOpen = False
    End Sub
    Private Sub btn_library_Click(sender As Object, e As RoutedEventArgs) Handles btn_library.Click
        MainTabCtrl.SelectedIndex = 3
        DrawerLeft.IsOpen = False
    End Sub
    Private Sub MainTabCtrl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles MainTabCtrl.SelectionChanged
        'If e.Source Is MainTabCtrl Then
        '    Select Case MainTabCtrl.SelectedIndex
        '        Case 0
        '            Me.Title = "MuPlay - Home"
        '        Case 1
        '            Me.Title = "MuPlay - Playlists"
        '        Case 2
        '            Me.Title = "MuPlay - Favourites"
        '        Case 3
        '            Me.Title = "MuPlay - Visualizer"
        '    End Select
        'End If
    End Sub
#End Region
    Private Sub media_cover_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles media_cover.MouseDown
        If e.ClickCount = 2 Then
            Dim Cover = Utils.BitmapFromImageSource(MainPlayer.CurrentMediaCover)
            Dim TempFile = IO.Path.GetTempFileName
            Cover.Save(TempFile)
            Dim CoverBrower As New HandyControl.Controls.ImageBrowser(TempFile)
            Overlay(True, False)
            CoverBrower.ShowDialog()
            Overlay(False, False)
            Exit Sub
            If media_cover.Source IsNot Nothing Then
                Dim Viewer As New CoverViewer
                Try
                    Viewer.ImageToView = media_cover.Tag
                Catch ex As Exception

                    Viewer = Nothing
                    Exit Sub
                End Try
                Viewer.Owner = Me
                Viewer.ShowDialog()
                Viewer = Nothing
            End If
        End If
    End Sub

    Private Sub media_sidebar_Click(sender As Object, e As RoutedEventArgs) Handles media_sidebar.Click
        DrawerLeft.IsOpen = True
    End Sub

    Public Sub media_play_btn_Click(sender As Object, e As RoutedEventArgs) Handles media_play_btn.Click
        If MainPlayer.PlayerState <> Player.State.Undefined Then
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause()
            Else
                MainPlayer.StreamPlay()
            End If
        End If
    End Sub
    Public Async Sub media_next_btn_Click(sender As Object, e As RoutedEventArgs) Handles media_next_btn.Click
        Select Case MainPlayer.RepeateType
            Case Player.RepeateBehaviour.RepeatAll
                media_prev_btn.IsEnabled = False
                media_next_btn.IsEnabled = False
        End Select
        MainPlayer_MediaEnded()
        Exit Sub
        Try
            Dim Pitem = MainPlaylist.Playlist.Item(MainPlaylist.GetNextSongIndex)
            Select Case Pitem.Substring(Pitem.IndexOf(">>") + 2, 1) 'Pitem.Substring(Pitem.Length - 1)
                Case Player.StreamTypes.Local
                    Try
                        MainPlayer.LoadSong(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), MainPlaylist, False)
                        MainPlayer.StreamPlay()
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                Case Player.StreamTypes.URL
                    Overlay(True, True)
                    'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                    Dim _pitem = playlistItems(MainPlaylist.GetNextSongIndex)
                    MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                    _pitem = Nothing
                    MainPlayer.StreamPlay()
                    Overlay(False, False)
                Case Player.StreamTypes.Youtube
                    Overlay(True, True)
                    Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                    MainPlayer.StreamPlay()
                    Overlay(False, False)
            End Select
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Sub media_next_btn_MouseEnter(sender As Object, e As MouseEventArgs) Handles media_next_btn.MouseEnter
        Media_Next_Border.Visibility = Visibility.Visible
        If My.Settings.UseAnimations Then
            Media_Next_Border.BeginAnimation(Border.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(250))))
        Else
            Media_Next_Border.Opacity = 1
        End If
    End Sub

    Private Async Sub media_next_btn_MouseLeave(sender As Object, e As MouseEventArgs) Handles media_next_btn.MouseLeave
        If My.Settings.UseAnimations Then
            Media_Next_Border.BeginAnimation(Border.OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(250))))
            Await Task.Delay(250)
        Else
            Media_Next_Border.Opacity = 0
        End If
        Media_Next_Border.Visibility = Visibility.Hidden
    End Sub
    Public Async Sub media_prev_btn_Click(sender As Object, e As RoutedEventArgs) Handles media_prev_btn.Click
        If MainPlayer.GetPosition > (MainPlayer.GetLength / 4) Then
            MainPlayer.SetPosition(0)
            Exit Sub
        End If
        Select Case MainPlayer.RepeateType
            Case Player.RepeateBehaviour.RepeatAll
                media_prev_btn.IsEnabled = False
                media_next_btn.IsEnabled = False
        End Select
        Try
            Dim Pitem = MainPlaylist.Playlist.Item(MainPlaylist.GetPreviousSongIndex)
            Select Case Pitem.Substring(Pitem.IndexOf(">>") + 2, 1) 'Pitem.Substring(Pitem.Length - 1)
                Case Player.StreamTypes.Local
                    MainPlayer.LoadSong(MainPlaylist.JumpTo(MainPlaylist.GetPreviousSongIndex), MainPlaylist, False)
                Case Player.StreamTypes.URL
                    Overlay(True, True)
                    'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetPreviousSongIndex), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                    Dim _pitem = playlistItems(MainPlaylist.GetPreviousSongIndex)
                    MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(MainPlaylist.GetPreviousSongIndex), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                    _pitem = Nothing
                    Overlay(False, False)
                Case Player.StreamTypes.Youtube
                    Overlay(True, True)
                    Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetPreviousSongIndex), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                    Overlay(False, False)
            End Select
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Sub media_prev_btn_MouseEnter(sender As Object, e As MouseEventArgs) Handles media_prev_btn.MouseEnter
        Media_Previous_Border.Visibility = Visibility.Visible
        If My.Settings.UseAnimations Then
            Media_Previous_Border.BeginAnimation(Border.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(250))))
        Else
            Media_Previous_Border.Opacity = 1
        End If
    End Sub

    Private Async Sub media_prev_btn_MouseLeave(sender As Object, e As MouseEventArgs) Handles media_prev_btn.MouseLeave
        If My.Settings.UseAnimations Then
            Media_Previous_Border.BeginAnimation(Border.OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(250))))
            Await Task.Delay(250)
        Else
            Media_Previous_Border.Opacity = 0
        End If
        Media_Previous_Border.Visibility = Visibility.Hidden
    End Sub
    Private Sub media_vol_btn_Click(sender As Object, e As RoutedEventArgs) Handles media_vol_btn.Click
        MainPlayer.Mute = Not MainPlayer.Mute
    End Sub
#Region "MainPlayer Events"
    Private Async Sub MainPlayer_PlayerStateChanged(State As Player.State) Handles MainPlayer.PlayerStateChanged
        Try
            Select Case State
                Case Player.State.MediaLoaded
                    TaskbarManager.Instance.SetOverlayIcon(Loadingicon, "Loading")
                    media_play_btn.Visibility = Visibility.Hidden
                    media_loading.Visibility = Visibility.Visible
#If WIN1010240 Then
                    SMTCPlayer.PlaybackStatus = MediaPlaybackStatus.Changing
                    SMTCPlayer.DisplayUpdater.Update()
#End If
                    If My.Settings.UseAnimations Then
                        Await Task.Delay(1000)
                        Switches_TopBarCanvasVisChanger.IsChecked = True
                    End If
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate)
                Case Player.State.Paused
                    media_loading.Visibility = Visibility.Hidden
                    media_play_btn.Visibility = Visibility.Visible
                    Try
                        If My.Settings.UseAnimations Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 1
                            Danim.To = 0
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, media_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            media_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        media_play_btn.Opacity = 1
                    End Try
                    media_play_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/play.png")))
                    TaskBarPlayPause.Icon = Playicon
                    TaskBarPlayPause.Tooltip = "Play"
                    NotifyIcon_Play.Header = "Play"
                    CType(NotifyIcon_Play.Icon, Image).Source = CType(media_play_btn.Background, ImageBrush).ImageSource
                    TaskbarManager.Instance.SetOverlayIcon(Pauseicon, "Paused")
                    Try
                        If My.Settings.UseAnimations Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 0
                            Danim.To = 1
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, media_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            media_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        media_play_btn.Opacity = 1
                    End Try
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused)
#If WIN1010240 Then
                    SMTCPlayer.PlaybackStatus = MediaPlaybackStatus.Paused
                    SMTCPlayer.DisplayUpdater.Update()
#End If
                    Try
                        If RPCClient.IsInitialized Then
                            RPCClient.SetPresence(New DiscordRPC.RichPresence With {.Details = MainPlayer.CurrentMediaTitle & " By: " & MainPlayer.CurrentMediaArtist, .State = "Paused", .Timestamps = New DiscordRPC.Timestamps(New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetPosition), Utils.GetRestSecs(MainPlayer.GetPosition)), New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetLength), Utils.GetRestSecs(MainPlayer.GetLength))), .Assets = New DiscordRPC.Assets With {.LargeImageKey = "muplayerl", .LargeImageText = "MuPlay", .SmallImageKey = "statepaused", .SmallImageText = "Paused"}})
                        End If
                    Catch ex As Exception
                    End Try
                Case Player.State.Playing
                    media_loading.Visibility = Visibility.Hidden
                    media_play_btn.Visibility = Visibility.Visible
                    Try
                        If My.Settings.UseAnimations Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 1
                            Danim.To = 0
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, media_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            media_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        media_play_btn.Opacity = 1
                    End Try
                    media_play_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/pause.png")))
                    TaskBarPlayPause.Icon = Pauseicon
                    NotifyIcon_Play.Header = "Pause"
                    CType(NotifyIcon_Play.Icon, Image).Source = CType(media_play_btn.Background, ImageBrush).ImageSource
                    TaskbarManager.Instance.SetOverlayIcon(Playicon, "Playing")
                    Try
                        If My.Settings.UseAnimations Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 0
                            Danim.To = 1
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, media_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            media_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        media_play_btn.Opacity = 1
                    End Try
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal)
#If WIN1010240 Then
                    SMTCPlayer.PlaybackStatus = MediaPlaybackStatus.Playing
                    SMTCPlayer.DisplayUpdater.Update()
#End If
                    Try
                        If RPCClient.IsInitialized Then
                            RPCClient.SetPresence(New DiscordRPC.RichPresence With {.Details = MainPlayer.CurrentMediaTitle & " By: " & MainPlayer.CurrentMediaArtist, .State = "Playing", .Timestamps = New DiscordRPC.Timestamps(New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetPosition), Utils.GetRestSecs(MainPlayer.GetPosition)), New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetLength), Utils.GetRestSecs(MainPlayer.GetLength))), .Assets = New DiscordRPC.Assets With {.LargeImageKey = "muplayerl", .LargeImageText = "MuPlay", .SmallImageKey = "stateplaying", .SmallImageText = "Listening"}})
                        End If
                    Catch ex As Exception
                    End Try
                Case Player.State.Stopped
                    media_loading.Visibility = Visibility.Hidden
                    media_play_btn.Visibility = Visibility.Visible
#If WIN1010240 Then
                    SMTCPlayer.PlaybackStatus = MediaPlaybackStatus.Closed
                    SMTCPlayer.DisplayUpdater.Update()
#End If
                    Try
                        If RPCClient.IsInitialized Then
                            RPCClient.SetPresence(New DiscordRPC.RichPresence With {.Details = MainPlayer.CurrentMediaTitle & " By: " & MainPlayer.CurrentMediaArtist, .State = "Stopped", .Timestamps = New DiscordRPC.Timestamps(New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetPosition), Utils.GetRestSecs(MainPlayer.GetPosition)), New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetLength), Utils.GetRestSecs(MainPlayer.GetLength))), .Assets = New DiscordRPC.Assets With {.LargeImageKey = "muplayerl", .LargeImageText = "MuPlay", .SmallImageKey = "stateidling", .SmallImageText = "Stopped"}})
                        End If
                    Catch ex As Exception

                    End Try
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress)
                    TaskbarManager.Instance.SetOverlayIcon(Nothing, Nothing)
                Case Player.State.Undefined
                    media_play_btn.Visibility = Visibility.Hidden
                    media_loading.Visibility = Visibility.Visible
                    Try
                        If RPCClient.IsInitialized Then
                            RPCClient.SetPresence(New DiscordRPC.RichPresence With {.Details = "Home", .State = "Idling", .Assets = New DiscordRPC.Assets With {.LargeImageKey = "muplayerl", .LargeImageText = "MuPlay", .SmallImageKey = "stateidling", .SmallImageText = "Idling"}})
                        End If
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate)
                    Catch ex As Exception

                    End Try
                Case Player.State._Error
                    media_play_btn.Visibility = Visibility.Hidden
                    media_loading.Visibility = Visibility.Visible
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error)
#If WIN1010240 Then
                    SMTCPlayer.PlaybackStatus = MediaPlaybackStatus.Closed
                    SMTCPlayer.DisplayUpdater.Update()
#End If
            End Select
            If State <> Player.State.Undefined AndAlso MainUIVisualizerUpdator.Enabled = False Then
                If My.Settings.Home_ShowVisualiser Then
                    MainUIVisualizerUpdator.Start()
                End If
            End If
        Catch ex As Exception
            MainPlayer.ReSendPlayStateChangedEvent(State, 1000)
        End Try
    End Sub
    Private Sub MainPlayer_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles MainPlayer.VolumeChanged
        PreviewPlayer.Volume = NewVal
        media_vol_track.Value = NewVal * 100
        Resources("Vol_Tip") = Math.Round(NewVal * 100)
        If IsMuted Then
            media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mvol.png")))
        Else
            If NewVal * 100 = 0 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mvol.png")))
            ElseIf NewVal * 100 > 0 AndAlso NewVal * 100 < 50 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Lvol.png")))
            ElseIf NewVal * 100 >= 50 AndAlso NewVal * 100 < 100 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mivol.png")))
            ElseIf NewVal * 100 = 100 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Fvol.png")))
            End If
        End If
    End Sub
    Private Async Sub MainPlayer_MediaLoaded(Title As String, Artist As String, Cover As System.Windows.Interop.InteropBitmap, Thumb As System.Windows.Interop.InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles MainPlayer.MediaLoaded
        If MainPlayer.RepeateType <> Player.RepeateBehaviour.Shuffle Then
            media_prev_btn.IsEnabled = True
        End If
        media_next_btn.IsEnabled = True
        AddToJlist(MainPlayer.SourceURL)
        If My.Settings.UseAnimations Then
            Switches_TopBarCanvasVisChanger.IsChecked = False
            Await Task.Delay(200)
        End If
        media_track.Maximum = MainPlayer.GetLength
        If Cover IsNot Nothing Then
            media_cover.Source = Thumb
            media_cover.Tag = Cover
            Home_NowPlaying.Source = Cover
            ''If My.Settings.UseAnimations Then
            ''    Dim Danim As New Animation.DoubleAnimation
            ''    Danim.From = 1
            ''    Danim.To = 0
            ''    Danim.Duration = TimeSpan.FromSeconds(0.5)
            ''    Animation.Storyboard.SetTarget(Danim, Home_Background)
            ''    Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
            ''    Dim sb As New Animation.Storyboard
            ''    sb.Children.Add(Danim)
            ''    sb.AutoReverse = True
            ''    sb.Begin()
            ''End If
            Home_Background.Source = Thumb
            Dim changeColor As New Animation.ColorAnimation
            Try
                If My.Settings.UseAnimations Then
                    Dim avgcolor = Utils.GetAverageColor(Thumb)
                    changeColor.From = CType(BottomCanvas.Background, SolidColorBrush).Color
                    changeColor.To = avgcolor
                    changeColor.Duration = TimeSpan.FromSeconds(1)
                    Animation.Storyboard.SetTarget(changeColor, BottomCanvas)
                    Animation.Storyboard.SetTargetProperty(changeColor, New PropertyPath("Background.Color"))
                    Dim sb As New Animation.Storyboard
                    If My.Settings.BackgroundType = 1 Then
                        FancyBackgroundManager.Color = New SolidColorBrush(avgcolor)
                        FancyBackgroundManager.ApplyColor()
                        'Dim c As New ColorAnimation
                        'c.To = avgcolor
                        'c.Duration = TimeSpan.FromSeconds(3)
                        'Storyboard.SetTarget(c, Home_FancyBackground_el1)
                        'Storyboard.SetTargetProperty(c, New PropertyPath("(Ellipse.Fill).(SolidColorBrush.Color)"))
                        'Dim c2 As New ColorAnimation
                        'c2.To = Utils.GetInverseColor(avgcolor)
                        'c2.Duration = TimeSpan.FromSeconds(3)
                        'Storyboard.SetTarget(c2, Home_FancyBackground_el2)
                        'Storyboard.SetTargetProperty(c2, New PropertyPath("(Ellipse.Fill).(SolidColorBrush.Color)"))
                        'sb.Children.Add(c)
                        'sb.Children.Add(c2)
                    End If
                    sb.Children.Add(changeColor)
                    sb.Begin()
                Else
                    Dim avgcolor = Utils.GetAverageColor(Thumb)
                    FancyBackgroundManager.Color = New SolidColorBrush(avgcolor)
                    FancyBackgroundManager.ApplyColor()
                    BottomCanvas.Background = New SolidColorBrush(avgcolor)
                    'Home_FancyBackground_el1.Fill = New SolidColorBrush(avgcolor)
                    'Home_FancyBackground_el2.Fill = New SolidColorBrush(avgcolor)
                End If
            Catch ex As Exception
                'BottomCanvas.Background = New SolidColorBrush(Utils.GetAverageColor(Thumb))
            End Try
        Else
            media_cover.Source = New BitmapImage(New Uri("pack://application:,,,/Res/song.png")) '"pack://application:,,,/WpfPlayer;component/Res/song.png"))
            Home_NowPlaying.Source = New BitmapImage(New Uri("pack://application:,,,/Res/song.png")) '"pack://application:,,,/WpfPlayer;component/Res/song.png"))
            BottomCanvas.Background = New SolidColorBrush(Color.FromRgb(220, 220, 220))
            Home_Background.Source = Nothing
        End If
        If Not String.IsNullOrEmpty(Title) Then
            MEDIA_TITLE.Content = Title
            Home_Media_Title.Content = Title
        Else
            MEDIA_TITLE.Content = IO.Path.GetFileNameWithoutExtension(MainPlayer.SourceURL)
            Home_Media_Title.Content = IO.Path.GetFileNameWithoutExtension(MainPlayer.SourceURL)
        End If
        If Not String.IsNullOrEmpty(Artist) Then
            MEDIA_ARTIST.Content = Artist
            Home_Media_Artist.Content = Artist
        Else
            MEDIA_ARTIST.Content = "Not Available"
            Home_Media_Artist.Content = "Not Available"
        End If
        Try
            If RPCClient.IsInitialized = True Then
                RPCClient.SetPresence(New DiscordRPC.RichPresence With {.Details = Title & " By: " & Artist, .State = "Listening", .Timestamps = New DiscordRPC.Timestamps(New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, 00), New Date(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute + Utils.GetMins(MainPlayer.GetLength), Utils.GetRestSecs(MainPlayer.GetLength))), .Assets = New DiscordRPC.Assets With {.LargeImageKey = "muplayerl", .LargeImageText = "MuPlay", .SmallImageKey = "stateplaying", .SmallImageText = "Listening"}})
            End If
        Catch ex As Exception

        End Try
        Dim NextItemInfo = playlistItems(MainPlaylist.GetNextSongIndex) 'Utils.GetSongInfo(MainPlaylist.GetNextSong)
        Media_next.Content = "Next: " & NextItemInfo.Title & " By: " & NextItemInfo.Artist & " From: " & NextItemInfo.Album
        Media_Next_Border_Title.Text = NextItemInfo.Title
        Media_Next_Border_Artist.Text = NextItemInfo.Artist
        If NextItemInfo.Cover IsNot Nothing Then
            Media_Next_Border_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(NextItemInfo.Cover))
        Else
            Dim _Cover = Utils.GetAlbumArt(NextItemInfo.Source)
            If _Cover IsNot Nothing Then
                Media_Next_Border_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(_Cover))
            Else
                Media_Next_Border_Cover.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png")))
            End If
        End If
        NextItemInfo = Nothing
        Dim PreviousItemInfo = playlistItems(MainPlaylist.GetPreviousSongIndex) 'Utils.GetSongInfo(MainPlaylist.GetNextSong)        
        Media_Previous_Border_Title.Text = PreviousItemInfo.Title
        Media_Previous_Border_Artist.Text = PreviousItemInfo.Artist
        If PreviousItemInfo.Cover IsNot Nothing Then
            Media_Previous_Border_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(PreviousItemInfo.Cover))
        Else
            Dim _Cover = Utils.GetAlbumArt(PreviousItemInfo.Source)
            If _Cover IsNot Nothing Then
                Media_Previous_Border_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(_Cover))
            Else
                Media_Previous_Border_Cover.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png")))
            End If
        End If
        PreviousItemInfo = Nothing
        If My.Settings.FavouriteTracks.Contains(MainPlayer.SourceURL) Then
            media_like_btn.Foreground = Brushes.Yellow
            Resources("fav_Tip") = "I hate it"
        Else
            media_like_btn.Foreground = Brushes.Black
            Resources("fav_Tip") = "I love it"
        End If
        If LyricsAvailable Then
            home_lyrics_block.Text = Lyrics
        Else
            home_lyrics_block.Text = String.Empty
        End If
#If WIN1010240 Then
        SMTCPlayer.AutoRepeatMode = Windows.Media.MediaPlaybackAutoRepeatMode.Track
        SMTCPlayer.IsPlayEnabled = True
        SMTCPlayer.IsPauseEnabled = True
        SMTCPlayer.IsNextEnabled = True
        SMTCPlayer.IsPreviousEnabled = True
        SMTCPlayer.IsFastForwardEnabled = True
        SMTCPlayer.IsRewindEnabled = True
        SMTCPlayer.IsStopEnabled = True
        SMTCPlayer.PlaybackRate = 1
        SMTCPlayer.ShuffleEnabled = True
        SMTCPlayer.DisplayUpdater.Type = Windows.Media.MediaPlaybackType.Music
        If String.IsNullOrEmpty(Title) Then
            SMTCPlayer.DisplayUpdater.MusicProperties.Title = IO.Path.GetFileNameWithoutExtension(MainPlayer.SourceURL)
        Else
            SMTCPlayer.DisplayUpdater.MusicProperties.Title = Title
        End If        
            SMTCPlayer.DisplayUpdater.MusicProperties.Artist = Artist
        SMTCPlayer.PlaybackStatus = MediaPlaybackStatus.Playing
        If Cover IsNot Nothing Then
            Dim Ccover = Utils.BitmapFromImageSource(Cover)
            Dim TCcover = IO.Path.GetTempFileName
            Ccover.Save(TCcover)
            Dim SCcover = Await Windows.Storage.StorageFile.GetFileFromPathAsync(TCcover)
            Dim STCcover = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(SCcover)
            SMTCPlayer.DisplayUpdater.Thumbnail = STCcover
        End If
        SMTCPlayer.DisplayUpdater.Update()
#End If
    End Sub
#If WIN1010240 Then
    Private Sub SMTCPlayer_ButtonPressed(sender As SystemMediaTransportControls, args As SystemMediaTransportControlsButtonPressedEventArgs) Handles SMTCPlayer.ButtonPressed
        Try
            Me.Dispatcher.Invoke(Sub()
                                     Select Case args.Button
                                         Case SystemMediaTransportControlsButton.Play
                                             MainPlayer.StreamPlay()
                                         Case SystemMediaTransportControlsButton.Pause
                                             MainPlayer.StreamPause()
                                         Case SystemMediaTransportControlsButton.Next
                                             media_next_btn_Click(Nothing, New RoutedEventArgs)
                                         Case SystemMediaTransportControlsButton.Previous
                                             media_prev_btn_Click(Nothing, New RoutedEventArgs)
                                         Case SystemMediaTransportControlsButton.FastForward
                                             MainPlayer.SetPosition(MainPlayer.GetPosition + 10)
                                         Case SystemMediaTransportControlsButton.Rewind
                                             MainPlayer.SetPosition(MainPlayer.GetPosition - 10)
                                     End Select
                                 End Sub)
        Catch ex As Exception
        End Try
    End Sub
#End If
    Private Async Sub MainPlayer_MediaEnded() 'Handles MainPlayer.MediaEnded        
        Select Case MainPlayer.RepeateType
            Case Player.RepeateBehaviour.RepeatOne
            Case Player.RepeateBehaviour.NoRepeat
                If MainPlayer.Shuffle = Player.RepeateBehaviour.NoShuffle Then
                    If MainPlaylist.Index < MainPlaylist.Count - 1 Then
                        Try
                            Dim Pitem = MainPlaylist.Playlist.Item(MainPlaylist.GetNextSongIndex)
                            Select Case Pitem.Substring(Pitem.IndexOf(">>") + 2, 1) 'Pitem.Substring(Pitem.Length - 1)
                                Case Player.StreamTypes.Local
                                    Await Dispatcher.InvokeAsync(Sub()
                                                                     MainPlayer.LoadSong(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), MainPlaylist, False)
                                                                 End Sub)
                                Case Player.StreamTypes.URL
                                    Overlay(True, True)
                                    'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                                    Dim _pitem = playlistItems(MainPlaylist.GetNextSongIndex)
                                    Await Dispatcher.InvokeAsync(Sub()
                                                                     MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                                                                 End Sub)
                                    _pitem = Nothing
                                    Overlay(False, False)
                                Case Player.StreamTypes.Youtube
                                    Overlay(True, True)
                                    Await Dispatcher.InvokeAsync(Async Function()
                                                                     Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                                                                 End Function)
                                    Overlay(False, False)
                            End Select
                        Catch ex As Exception
                            Return
                        End Try
                    End If
                Else
                    Try
                        Dim Rnd As New Random
                        Dim RndSong = Rnd.Next(0, MainPlaylist.Count - 1)
                        Dim Pitem = MainPlaylist.Playlist.Item(RndSong)
                        Select Case Pitem.Substring(Pitem.IndexOf(">>") + 2, 1) 'Pitem.Substring(Pitem.Length - 1)
                            Case Player.StreamTypes.Local
                                Await Dispatcher.InvokeAsync(Sub()
                                                                 MainPlayer.LoadSong(MainPlaylist.JumpTo(RndSong), MainPlaylist, False)
                                                             End Sub)
                            Case Player.StreamTypes.URL
                                Overlay(True, True)
                                'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(RndSong), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                                Dim _pitem = playlistItems(RndSong)
                                Await Dispatcher.InvokeAsync(Sub()
                                                                 MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(RndSong), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                                                             End Sub)
                                _pitem = Nothing
                                Overlay(False, False)
                            Case Player.StreamTypes.Youtube
                                Overlay(True, True)
                                Await Dispatcher.InvokeAsync(Async Function()
                                                                 Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(RndSong), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                                                             End Function)
                                Overlay(False, False)
                        End Select
                        Rnd = Nothing
                    Catch ex As Exception
                        Return
                    End Try
                End If
            Case Player.RepeateBehaviour.RepeatAll
                If MainPlayer.Shuffle = Player.RepeateBehaviour.NoShuffle Then
                    Try
                        Dim Pitem = MainPlaylist.Playlist.Item(MainPlaylist.GetNextSongIndex)
                        Select Case Pitem.Substring(Pitem.IndexOf(">>") + 2, 1) 'Pitem.Substring(Pitem.Length - 1)
                            Case Player.StreamTypes.Local
                                Await Dispatcher.InvokeAsync(Sub()
                                                                 MainPlayer.LoadSong(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), MainPlaylist, False)
                                                             End Sub)
                            Case Player.StreamTypes.URL
                                Overlay(True, True)
                                'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                                Dim _pitem = playlistItems(MainPlaylist.GetNextSongIndex)
                                Await Dispatcher.InvokeAsync(Sub()
                                                                 MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                                                             End Sub)
                                _pitem = Nothing
                                Overlay(False, False)
                            Case Player.StreamTypes.Youtube
                                Overlay(True, True)
                                Await Dispatcher.InvokeAsync(Async Function()
                                                                 Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.GetNextSongIndex), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                                                             End Function)
                                Overlay(False, False)
                        End Select
                    Catch ex As Exception
                        Return
                    End Try
                Else
                    Try
                        Dim Rnd As New Random
                        Dim RndSong = Rnd.Next(0, MainPlaylist.Count - 1)
                        Dim Pitem = MainPlaylist.Playlist.Item(RndSong)
                        Select Case Pitem.Substring(Pitem.IndexOf(">>") + 2, 1) 'Pitem.Substring(Pitem.Length - 1)
                            Case Player.StreamTypes.Local
                                Await Dispatcher.InvokeAsync(Sub()
                                                                 MainPlayer.LoadSong(MainPlaylist.JumpTo(RndSong), MainPlaylist, False)
                                                             End Sub)
                            Case Player.StreamTypes.URL
                                Overlay(True, True)
                                'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(RndSong), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                                Dim _pitem = playlistItems(RndSong)
                                Await Dispatcher.InvokeAsync(Sub()
                                                                 MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(RndSong), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                                                             End Sub)
                                _pitem = Nothing
                                Overlay(False, False)
                            Case Player.StreamTypes.Youtube
                                Overlay(True, True)
                                Await Dispatcher.InvokeAsync(Async Function()
                                                                 Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(RndSong), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                                                             End Function)
                                Overlay(False, False)
                        End Select
                    Catch ex As Exception
                        Return
                    End Try
                End If
        End Select
    End Sub
    Private Sub MainPlayer_OnFxChanged(FX As Player.LinkHandles, State As Boolean) Handles MainPlayer.OnFxChanged
        Select Case FX
            Case Player.LinkHandles.EQ
                If State = True Then
                    TitleBar_Eq.IsChecked = True
                    My.Windows.Equalizer.fx_state.IsChecked = True
                Else
                    TitleBar_Eq.IsChecked = False
                    My.Windows.Equalizer.fx_state.IsChecked = False
                End If
            Case Player.LinkHandles.Reverb
                If State = True Then
                    TitleBar_Reverb.IsChecked = True
                    My.Windows.Reverb.fx_state.IsChecked = True
                Else
                    TitleBar_Reverb.IsChecked = False
                    My.Windows.Reverb.fx_state.IsChecked = False
                End If
            Case Player.LinkHandles.Loudness
                If State = True Then
                    TitleBar_SoundBooster.IsChecked = True
                    TitleBar_SoundBooster_PresSoft.IsEnabled = True
                    TitleBar_SoundBooster_PresMed.IsEnabled = True
                    TitleBar_SoundBooster_PresHard.IsEnabled = True
                Else
                    TitleBar_SoundBooster.IsChecked = False
                    TitleBar_SoundBooster_PresSoft.IsEnabled = False
                    TitleBar_SoundBooster_PresMed.IsEnabled = False
                    TitleBar_SoundBooster_PresHard.IsEnabled = False
                End If
            Case Player.LinkHandles.Balance
                If State = True Then
                    Select Case MainPlayer.GetBalance
                        Case < 0
                            TitleBar_BalanceLeft.IsChecked = True
                            TitleBar_BalanceCenter.IsChecked = False
                            TitleBar_BalanceRight.IsChecked = False
                        Case = 0
                            TitleBar_BalanceLeft.IsChecked = False
                            TitleBar_BalanceCenter.IsChecked = True
                            TitleBar_BalanceRight.IsChecked = False
                        Case > 1
                            TitleBar_BalanceLeft.IsChecked = False
                            TitleBar_BalanceCenter.IsChecked = False
                            TitleBar_BalanceRight.IsChecked = True
                    End Select
                Else
                    TitleBar_BalanceLeft.IsChecked = False
                    TitleBar_BalanceCenter.IsChecked = False
                    TitleBar_BalanceRight.IsChecked = False
                End If
            Case Player.LinkHandles.SampleRate
                If State = True Then
                    TitleBar_SampleRate.IsChecked = True
                Else
                    TitleBar_SampleRate.IsChecked = False
                End If
            Case Player.LinkHandles.StereoMix
                If State = True Then
                    TitleBar_SteroMix.IsChecked = True
                Else
                    TitleBar_SteroMix.IsChecked = False
                End If
            Case Player.LinkHandles.Rotate
                If State = True Then
                    TitleBar_RotateChannel.IsChecked = True
                    TitleBar_8DRotateChannel_Rate_Slow.IsChecked = False
                    TitleBar_8DRotateChannel_Rate_Med.IsChecked = False
                    TitleBar_8DRotateChannel_Rate_Fast.IsChecked = False
                Else
                    TitleBar_RotateChannel.IsChecked = False
                    TitleBar_8DRotateChannel_Rate_Slow.IsChecked = False
                    TitleBar_8DRotateChannel_Rate_Med.IsChecked = False
                    TitleBar_8DRotateChannel_Rate_Fast.IsChecked = False
                End If
        End Select
    End Sub
    Private Async Sub MainPlayer_OnMediaError(ErrorCode As Un4seen.Bass.BASSError) Handles MainPlayer.OnMediaError
        If MainPlaylist.Count > 1 Then
            ShowNotification("MuPlay", "An error occured with code " & ErrorCode.ToString & "." & vbCrLf & "Switching to next song in 5 seconds", HandyControl.Data.NotifyIconInfoType.Error)
            Await Task.Delay(5000)
            media_next_btn_Click(Nothing, New RoutedEventArgs)
        Else
            ShowNotification("MuPlay", "An error occured with code " & ErrorCode.ToString & "." & vbCrLf & "Stopping player...", HandyControl.Data.NotifyIconInfoType.Error)
        End If
    End Sub
    Private Sub MainPlayer_OnRepeatChanged(NewType As Player.RepeateBehaviour) Handles MainPlayer.OnRepeatChanged
        Select Case NewType
            Case Player.RepeateBehaviour.NoRepeat
                media_loop.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/no-loop.png")))
            Case Player.RepeateBehaviour.RepeatAll
                media_loop.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/loop.png")))
            Case Player.RepeateBehaviour.RepeatOne
                media_loop.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/loop-one.png")))
        End Select
    End Sub

    Private Sub MainPlayer_OnShuffleChanged(NewType As Player.RepeateBehaviour) Handles MainPlayer.OnShuffleChanged
        Select Case NewType
            Case Player.RepeateBehaviour.NoShuffle
                media_prev_btn.IsEnabled = True
                media_shuffle.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/no-shuffle.png")))
            Case Player.RepeateBehaviour.Shuffle
                media_prev_btn.IsEnabled = False
                media_shuffle.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/shuffle.png")))
        End Select
    End Sub
#End Region
    Private Sub media_vol_track_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles media_vol_track.ValueChanged
        Try
            MainPlayer.SetVolume(e.NewValue / 100)
            If media_vol_track.Value = 0 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mvol.png")))
            ElseIf media_vol_track.Value > 0 AndAlso media_vol_track.Value < 50 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Lvol.png")))
            ElseIf media_vol_track.Value >= 50 AndAlso media_vol_track.Value < 100 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mivol.png")))
            ElseIf media_vol_track.Value = 100 Then
                media_vol_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Fvol.png")))
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub media_track_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles media_track.ValueChanged
        If media_track.IsMouseOver Then
            Try
                MainPlayer.SetPosition(media_track.Value)
            Catch ex As Exception
            End Try
        End If
    End Sub
    Private Sub Home_Rec1_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec1.MouseDown, Home_Rec1_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec1.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec1.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            PreviewPlayer.StreamPlay()
            If My.Settings.UseAnimations Then
                Dim PAnim As New Animation.ThicknessAnimation
                PAnim.From = Home_Rec1.Margin
                PAnim.To = New Thickness(35, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec1)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec1_Overlay.Visibility = Visibility.Visible
            Home_Rec1_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub Home_Rec1_MouseLeave(sender As Object, e As MouseEventArgs) Handles Home_Rec1_Overlay.MouseLeave, Home_Rec2_Overlay.MouseLeave, Home_Rec3_Overlay.MouseLeave, Home_Rec4_Overlay.MouseLeave, Home_Rec5_Overlay.MouseLeave, Home_Rec6_Overlay.MouseLeave, Home_Rec7_Overlay.MouseLeave
        PreviewPlayer.StreamStop()
        If MainPlayer.PlayerState = Player.State.Playing Then
            MainPlayer.StreamPlay()
        End If
        Home_Rec1_Overlay.Visibility = Visibility.Hidden
        Home_Rec1_RunningBlock.Visibility = Visibility.Hidden
        Home_Rec2_Overlay.Visibility = Visibility.Hidden
        Home_Rec2_RunningBlock.Visibility = Visibility.Hidden
        Home_Rec3_Overlay.Visibility = Visibility.Hidden
        Home_Rec3_RunningBlock.Visibility = Visibility.Hidden
        Home_Rec4_Overlay.Visibility = Visibility.Hidden
        Home_Rec4_RunningBlock.Visibility = Visibility.Hidden
        Home_Rec5_Overlay.Visibility = Visibility.Hidden
        Home_Rec5_RunningBlock.Visibility = Visibility.Hidden
        Home_Rec6_Overlay.Visibility = Visibility.Hidden
        Home_Rec6_RunningBlock.Visibility = Visibility.Hidden
        Home_Rec7_Overlay.Visibility = Visibility.Hidden
        Home_Rec7_RunningBlock.Visibility = Visibility.Hidden
    End Sub
    Private Sub Home_Rec2_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec2.MouseDown, Home_Rec2_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec2.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec2.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            PreviewPlayer.StreamPlay()
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            Dim PAnim As New Animation.ThicknessAnimation
            If My.Settings.UseAnimations Then
                PAnim.From = Home_Rec2.Margin
                PAnim.To = New Thickness(200, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec2)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec2_Overlay.Visibility = Visibility.Visible
            Home_Rec2_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub Home_Rec3_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec3.MouseDown, Home_Rec3_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec3.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec3.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            PreviewPlayer.StreamPlay()
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            If My.Settings.UseAnimations Then
                Dim PAnim As New Animation.ThicknessAnimation
                PAnim.From = Home_Rec3.Margin
                PAnim.To = New Thickness(365, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec3)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec3_Overlay.Visibility = Visibility.Visible
            Home_Rec3_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub Home_Rec4_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec4.MouseDown, Home_Rec4_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec4.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec4.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            PreviewPlayer.StreamPlay()
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            If My.Settings.UseAnimations Then
                Dim PAnim As New Animation.ThicknessAnimation
                PAnim.From = Home_Rec4.Margin
                PAnim.To = New Thickness(530, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec4)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec4_Overlay.Visibility = Visibility.Visible
            Home_Rec4_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub Home_Rec5_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec5.MouseDown, Home_Rec5_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec5.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec5.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            PreviewPlayer.StreamPlay()
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            If My.Settings.UseAnimations Then
                Dim PAnim As New Animation.ThicknessAnimation
                PAnim.From = Home_Rec5.Margin
                PAnim.To = New Thickness(695, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec5)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec5_Overlay.Visibility = Visibility.Visible
            Home_Rec5_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub Home_Rec6_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec6.MouseDown, Home_Rec6_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec6.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec6.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            PreviewPlayer.StreamPlay()
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            If My.Settings.UseAnimations Then
                Dim PAnim As New Animation.ThicknessAnimation
                PAnim.From = Home_Rec6.Margin
                PAnim.To = New Thickness(860, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec6)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec6_Overlay.Visibility = Visibility.Visible
            Home_Rec6_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub Home_Rec7_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_Rec7.MouseDown, Home_Rec7_Overlay.MouseDown
        If e.ClickCount = 2 Then
            MainPlayer.LoadSong(Home_Rec7.Tag, MainPlaylist)
            MainPlayer.StreamPlay()
        ElseIf e.ClickCount = 1 Then
            PreviewPlayer.LoadSong(Home_Rec7.Tag, Nothing, False)
            PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
            PreviewPlayer.StreamPlay()
            If MainPlayer.PlayerState = Player.State.Playing Then
                MainPlayer.StreamPause(False)
            End If
            If My.Settings.UseAnimations Then
                Dim PAnim As New Animation.ThicknessAnimation
                PAnim.From = Home_Rec7.Margin
                PAnim.To = New Thickness(1025, 230, 0, 0)
                PAnim.Duration = TimeSpan.FromMilliseconds(200)
                Animation.Storyboard.SetTarget(PAnim, Home_Rec7)
                Animation.Storyboard.SetTargetProperty(PAnim, New PropertyPath("Margin"))
                Dim sb As New Storyboard
                sb.Children.Add(PAnim)
                sb.AutoReverse = True
                sb.Begin()
                PAnim = Nothing
            End If
            Home_Rec7_Overlay.Visibility = Visibility.Visible
            Home_Rec7_RunningBlock.Visibility = Visibility.Visible
        End If
    End Sub
#Region "Playlist Related"
    Private Sub MainPlaylist_OnSongAdd(Value As String, Type As Player.StreamTypes, IndexUpdated As Boolean, UseURL As Boolean, URL As String, OverrideCurrentMedia As Boolean, OCMTitle As String, OCMArtist As String, OCMYear As Integer, OCMCover As System.Drawing.Bitmap) Handles MainPlaylist.OnSongAdd
        If UseURL = False Then
            Dim Info = Utils.GetSongInfo(Value)
            Try
                playlistItems.Add(New PlaylistItem(Playlist_Main.Items.Count + 1, Info(1), Info(0), Info(2), Info(3), Info(4), Type, Value, Nothing))
            Catch ex As Exception
            End Try
            If IndexUpdated = True Then
                UpdatePlaylist = False
                Playlist_Main.SelectedIndex = Playlist_Main.Items.Count - 1
            End If
            Info = Nothing
        Else
            Try
                playlistItems.Add(New PlaylistItem(Playlist_Main.Items.Count + 1, OCMTitle, OCMArtist, "Not Available", OCMYear, 1, Type, URL, OCMCover))
            Catch ex As Exception
            End Try
            If IndexUpdated = True Then
                UpdatePlaylist = False
                Playlist_Main.SelectedIndex = Playlist_Main.Items.Count - 1
            End If
        End If
    End Sub
    Private Sub MainPlaylist_OnSongInsert(Value As String, Type As Player.StreamTypes, IndexUpdated As Boolean, UseURL As Boolean, URL As String, OverrideCurrentMedia As Boolean, OCMTitle As String, OCMArtist As String, OCMYear As Integer, OCMCover As System.Drawing.Bitmap, Index As Integer) Handles MainPlaylist.OnSongInsert
        If UseURL = False Then
            Dim Info = Utils.GetSongInfo(Value)
            Try
                playlistItems.Insert(Index, New PlaylistItem(Playlist_Main.Items.Count + 1, Info(1), Info(0), Info(2), Info(3), Info(4), Type, Value, Nothing))
            Catch ex As Exception
            End Try
            If IndexUpdated = True Then
                UpdatePlaylist = False
                Playlist_Main.SelectedIndex = Playlist_Main.Items.Count - 1
            End If
            Info = Nothing
        Else
            Try
                playlistItems.Insert(Index, New PlaylistItem(Playlist_Main.Items.Count + 1, OCMTitle, OCMArtist, "Not Available", OCMYear, 1, Type, URL, OCMCover))
            Catch ex As Exception
            End Try
            If IndexUpdated = True Then
                UpdatePlaylist = False
                Playlist_Main.SelectedIndex = Playlist_Main.Items.Count - 1
            End If
        End If
        RefreshPlaylistNums()
    End Sub
    Private Sub MainPlaylist_OnSongRemove(Value As String, Index As Integer) Handles MainPlaylist.OnSongRemove
        Try
            playlistItems.RemoveAt(Index)
        Catch ex As Exception
            Return
        End Try
    End Sub

    Private Sub MainPlaylist_OnPlaylistClear() Handles MainPlaylist.OnPlaylistClear
        Try
            playlistItems.Clear()
        Catch ex As Exception
        End Try
    End Sub
    Private Sub MainPlaylist_OnSongNext() Handles MainPlaylist.OnSongNext
        Try
            UpdatePlaylist = False
            Playlist_Main.SelectedIndex = MainPlaylist.Index
        Catch ex As Exception
            Return
        End Try
    End Sub

    Private Sub MainPlaylist_OnSongPrevious() Handles MainPlaylist.OnSongPrevious
        Try
            UpdatePlaylist = False
            Playlist_Main.SelectedIndex = MainPlaylist.Index
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Sub MainPlaylist_OnIndexChanged(Index As Integer) Handles MainPlaylist.OnIndexChanged
        Try
            UpdatePlaylist = False
            Playlist_Main.SelectedIndex = Index
        Catch ex As Exception
        End Try
    End Sub
    Private Sub MainPlaylist_OnItemChanged(IndexFrom As Integer, IndexTo As Integer) Handles MainPlaylist.OnItemChanged
        Try
            Dim item = playlistItems.Item(IndexFrom)
            item.Num = IndexTo + 1
            playlistItems.RemoveAt(IndexFrom)
            playlistItems.Insert(IndexTo, item)
            RefreshPlaylistNums()
        Catch ex As Exception
            ShowNotification("MuPlay", ex.Message, HandyControl.Data.NotifyIconInfoType.Error)
        End Try
    End Sub
    Private Async Sub Playlist_Main_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Playlist_Main.SelectionChanged
        If IsPlaylistEditMode = False Then
            Try
                If UpdatePlaylist Then
                    Select Case playlistItems(Playlist_Main.SelectedIndex).Type
                        Case Player.StreamTypes.Local
                            MainPlayer.LoadSong(MainPlaylist.JumpTo(Playlist_Main.SelectedIndex), MainPlaylist, False)
                        Case Player.StreamTypes.URL
                            Overlay(True, True)
                            'Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(MainPlaylist.Playlist_Main.SelectedIndex), Player.StreamTypes.URL, MainPlaylist, Nothing, False)
                            Dim _pitem = playlistItems(Playlist_Main.SelectedIndex)
                            MainPlayer.LoadSong(Nothing, MainPlaylist, False, True, True, MainPlaylist.JumpTo(Playlist_Main.SelectedIndex), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                            _pitem = Nothing
                            Overlay(False, False)
                        Case Player.StreamTypes.Youtube
                            Overlay(True, True)
                            Playlist_Main.IsEnabled = False
                            Await MainPlayer.LoadStreamAsync(MainPlaylist.JumpTo(Playlist_Main.SelectedIndex), Player.StreamTypes.Youtube, MainPlaylist, Nothing, False)
                            Playlist_Main.IsEnabled = True
                            Overlay(False, False)
                    End Select
                End If
                UpdatePlaylist = True
            Catch ex As Exception
                UpdatePlaylist = True
            End Try
        End If
    End Sub
    Private Sub Playlistdrawerclosebtn_Click(sender As Object, e As RoutedEventArgs) Handles Playlistdrawerclosebtn.Click
        PlaylistDrawerLeft.IsOpen = False
    End Sub
    Private Sub PlaylistLeftDrawerOpen_Click(sender As Object, e As RoutedEventArgs) Handles PlaylistLeftDrawerOpen.Click
        PlaylistDrawerLeft.IsOpen = True
    End Sub
    Private Sub Playlistadd_Click(sender As Object, e As RoutedEventArgs) Handles Playlistadd.Click
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Filter = Utils.OFDFileFilters, .Multiselect = True, .Title = "Select one or more files"}
        If OFD.ShowDialog Then
            Dispatcher.InvokeAsync(Sub()
                                       Switches_Playlist_Overlay_Changer.IsChecked = True
                                       For i As Integer = 0 To OFD.FileNames.Length - 1
                                           MainPlaylist.Add(OFD.FileNames(i), Player.StreamTypes.Local, False)
                                           Playlist_Overlay_State.Content = "Adding songs ...(" & i + 1 & "/" & OFD.FileNames.Length & ")"
                                       Next
                                       Switches_Playlist_Overlay_Changer.IsChecked = False
                                   End Sub, System.Windows.Threading.DispatcherPriority.Background)
        End If
    End Sub

    Private Sub Playlistclear_Click(sender As Object, e As RoutedEventArgs) Handles Playlistclear.Click
        MainPlaylist.Clear()
    End Sub

    Private Async Sub Playlistmovedown_Click(sender As Object, e As RoutedEventArgs) Handles Playlistmovedown.Click
        Dim Index = Playlist_Main.SelectedIndex
        UpdatePlaylist = False
        Index = MainPlaylist.MoveTo(Index, Index + 1)
        Await Task.Delay(10)
        UpdatePlaylist = False
        Playlist_Main.SelectedIndex = Index
    End Sub
    Private Async Sub Playlistmoveto_Click(sender As Object, e As RoutedEventArgs) Handles Playlistmoveto.Click
        Dim Index = Playlist_Main.SelectedIndex
        Dim IndexIB As New InputDialog("Move To ?") With {.Owner = Me}
        If IndexIB.ShowDialog Then
            Try
                UpdatePlaylist = False
                Index = MainPlaylist.MoveTo(Index, IndexIB.Input)
                Await Task.Delay(10)
                UpdatePlaylist = False
                Playlist_Main.SelectedIndex = Index
            Catch ex As Exception
                ShowNotification("MuPlay", ex.Message, HandyControl.Data.NotifyIconInfoType.Error)
            End Try
        End If
    End Sub

    Private Async Sub Playlistmoveup_Click(sender As Object, e As RoutedEventArgs) Handles Playlistmoveup.Click
        Dim Index = Playlist_Main.SelectedIndex
        UpdatePlaylist = False
        Index = MainPlaylist.MoveTo(Index, Index - 1)
        Await Task.Delay(10)
        UpdatePlaylist = False
        Playlist_Main.SelectedIndex = Index
    End Sub

    Private Sub Playlistremove_Click(sender As Object, e As RoutedEventArgs) Handles Playlistremove.Click
        Dim Index = Playlist_Main.SelectedIndex
        MainPlaylist.RemoveAt(Index)
        Try
            If Index = MainPlaylist.Count Then
                Playlist_Main.SelectedIndex = 0
            Else
                Playlist_Main.SelectedIndex = Index
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Playlisteditmode_Checked(sender As Object, e As RoutedEventArgs) Handles Playlisteditmode.Checked
        IsPlaylistEditMode = True
        Playlisteditmode.Content = "Edit Mode: ON"
    End Sub

    Private Sub Playlisteditmode_Unchecked(sender As Object, e As RoutedEventArgs) Handles Playlisteditmode.Unchecked
        IsPlaylistEditMode = False
        Playlisteditmode.Content = "Edit Mode: OFF"
        UpdatePlaylist = False
        Playlist_Main.SelectedIndex = MainPlaylist.Index
        RefreshPlaylistNums()

    End Sub
#End Region
    Private Sub Quick_Access_AddSong_Click(sender As Object, e As RoutedEventArgs) Handles Quick_Access_AddSong.Click
        'Dim Opf As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.Title = "MuPlayer - Add Song", .Filter = Utils.OFDFileFilters}
        'If Opf.ShowDialog() Then
        '    MainPlayer.LoadSong(Opf.FileName, MainPlaylist)
        '    MainPlayer.StreamPlay()
        '    MainUIManager.IsEnabled = True
        '    If My.Settings.UseAnimations Then
        '        Switches_OverlayChanger.IsChecked = False
        '    Else
        '        QuickAccess_MainCanvas.Visibility = Visibility.Hidden
        '    End If
        '    Overlay(False, False)
        'End If
        'Opf = Nothing
        Dim Opf As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.Multiselect = True, .Title = "MuPlayer - Add songs", .Filter = Utils.OFDFileFilters}
        If Opf.ShowDialog() Then
            For Each song In Opf.FileNames
                MainPlaylist.Add(song, Player.StreamTypes.Local)
            Next
            MainPlayer.LoadSong(Opf.FileNames(Opf.FileNames.Length - 1), MainPlaylist, False)
            MainUIManager.IsEnabled = True
            If My.Settings.UseAnimations Then
                Switches_OverlayChanger.IsChecked = False
            Else
                QuickAccess_MainCanvas.Visibility = Visibility.Hidden
            End If
            Overlay(False, False)
        End If
        Opf = Nothing
    End Sub

    Private Sub Quick_Access_AddAlbum_Click(sender As Object, e As RoutedEventArgs) Handles Quick_Access_AddAlbum.Click
        'Dim Opf As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.Multiselect = True, .Title = "MuPlayer - Add Album", .Filter = Utils.OFDFileFilters}
        'If Opf.ShowDialog() Then
        '    For Each song In Opf.FileNames
        '        MainPlaylist.Add(song, Player.StreamTypes.Local)
        '    Next
        '    MainPlayer.LoadSong(Opf.FileNames(Opf.FileNames.Length - 1), MainPlaylist, False)
        '    MainPlayer.StreamPlay()
        '    MainUIManager.IsEnabled = True
        '    If My.Settings.UseAnimations Then
        '        Switches_OverlayChanger.IsChecked = False
        '    Else
        '        QuickAccess_MainCanvas.Visibility = Visibility.Hidden
        '    End If
        '    Overlay(False, False)
        'End If
        'Opf = Nothing
        Dim Fbd As New Ookii.Dialogs.Wpf.VistaFolderBrowserDialog With {.Description = "Sub folders scaning is currently set to """ & My.Settings.FBD_QuickAcess_SubFolders & """"}
        If Fbd.ShowDialog Then
            If IO.Directory.Exists(Fbd.SelectedPath) Then
                For Each song As String In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(Fbd.SelectedPath, filter, My.Settings.FBD_QuickAcess_SubFolders)).ToArray()
                    MainPlaylist.Add(song, Player.StreamTypes.Local)
                Next
                MainPlayer.LoadSong(MainPlaylist.GetItem(MainPlaylist.Count - 1), MainPlaylist, False)
                MainUIManager.IsEnabled = True
                If My.Settings.UseAnimations Then
                    Switches_OverlayChanger.IsChecked = False
                Else
                    QuickAccess_MainCanvas.Visibility = Visibility.Hidden
                End If
                Overlay(False, False)
            End If
        End If
        Fbd = Nothing
    End Sub

    Private Sub TitleBar_Menu_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Menu.Click
        Switches_OverlayChanger.IsChecked = Not Switches_OverlayChanger.IsChecked
        If Switches_OverlayChanger.IsChecked Then
            Overlay(True, False)
        Else
            Overlay(False, False)
        End If
    End Sub

    Private Sub TitleBar_Theme_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Theme.Click
        If My.Settings.DefaultTheme < 2 Then
            My.Settings.DefaultTheme += 1
        Else
            My.Settings.DefaultTheme = 0
        End If
        My.Settings.Save()
        If My.Settings.DefaultTheme <> 2 Then
            ThemeManager.Current.UsingSystemTheme = False
            UpdateSkin(My.Settings.DefaultTheme)
            Dim CurrentTheme = System.Enum.GetName(GetType(HandyControl.Themes.ApplicationTheme), My.Settings.DefaultTheme)
            ShowNotification("MuPlay", "Current theme: " & CurrentTheme, HandyControl.Data.NotifyIconInfoType.Info)
            CurrentTheme = Nothing
        Else
            ThemeManager.Current.UsingSystemTheme = True
            ShowNotification("MuPlay", "Now using system theme", HandyControl.Data.NotifyIconInfoType.Info)
        End If
    End Sub

    Private Sub TitleBar_Theme_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs) Handles TitleBar_Theme.MouseRightButtonUp
        Select Case My.Settings.DefaultTheme
            Case 0 'Light
                ShowNotification("MuPlay", "Using light theme", HandyControl.Data.NotifyIconInfoType.Info)
            Case 1 'Dark
                ShowNotification("MuPlay", "Using dark theme", HandyControl.Data.NotifyIconInfoType.Info)
            Case 2 'System Theme
                ShowNotification("MuPlay", "Using system theme", HandyControl.Data.NotifyIconInfoType.Info)
        End Select
    End Sub

    Private Sub TitleBar_Settings_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Settings.Click
        Overlay(True, False)
        My.Windows.Settings.Owner = Me
        My.Windows.Settings.ShowDialog()
        Overlay(False, False)
    End Sub
    Private Sub TitleBar_Search_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Search.Click
        Overlay(True, False)
        My.Windows.Search.Owner = Me
        My.Windows.Search.ShowDialog()
        Overlay(False, False)
    End Sub
    Private Sub MainWindow_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.Key = Key.C AndAlso My.Computer.Keyboard.ShiftKeyDown = True Then
            My.Windows.Console.Show()
            Exit Sub
            Dim _Command = InputBox("Write the command", "Command")
            If Not String.IsNullOrEmpty(_Command) Then
                Command.Excute(_Command, Me)
            End If
        ElseIf e.Key = Key.A AndAlso My.Computer.Keyboard.ShiftKeyDown = True Then
            Throw New Exception("NOOB!")
        ElseIf e.Key = Key.D1 AndAlso My.Computer.Keyboard.CtrlKeyDown = True Then
            MainTabCtrl.SelectedIndex = 0
        ElseIf e.Key = Key.D2 AndAlso My.Computer.Keyboard.CtrlKeyDown = True Then
            MainTabCtrl.SelectedIndex = 1
        ElseIf e.Key = Key.D3 AndAlso My.Computer.Keyboard.CtrlKeyDown = True Then
            MainTabCtrl.SelectedIndex = 2
        ElseIf e.Key = Key.D4 AndAlso My.Computer.Keyboard.CtrlKeyDown = True Then
            MainTabCtrl.SelectedIndex = 3
        End If
    End Sub
#Region "TitleBar Buttons"
    Private Sub TitleBar_Eq_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Eq.Click
        Overlay(True, False)
        My.Windows.Equalizer.Owner = Me
        My.Windows.Equalizer.ShowDialog()
        Overlay(False, False)
    End Sub
    Private Sub MainPlayer_OnEqChanged(EQgains As Integer()) Handles MainPlayer.OnEqChanged
        'My.Windows.Equalizer.Eq1.Value = EQgains(0)
        'My.Windows.Equalizer.Eq2.Value = EQgains(1)
        'My.Windows.Equalizer.Eq3.Value = EQgains(2)
        'My.Windows.Equalizer.Eq4.Value = EQgains(3)
        'My.Windows.Equalizer.Eq5.Value = EQgains(4)
        'My.Windows.Equalizer.Eq6.Value = EQgains(5)
        'My.Windows.Equalizer.Eq7.Value = EQgains(6)
        'My.Windows.Equalizer.Eq8.Value = EQgains(7)
        'My.Windows.Equalizer.Eq9.Value = EQgains(8)
        'My.Windows.Equalizer.Eq10.Value = EQgains(9)
        My.Windows.Equalizer.SetPreset(EQgains(0), EQgains(1), EQgains(2), EQgains(3), EQgains(4), EQgains(5), EQgains(6), EQgains(7), EQgains(8), EQgains(9))
    End Sub
    Private Sub TitleBar_Sleep_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Sleep.Click
        Dim ST As New SleepTimer
        Dim DLGIB As New InputDialog("Input the delay in seconds.")
        If DLGIB.ShowDialog Then
            Dim DLG As New Ookii.Dialogs.Wpf.TaskDialog With {.AllowDialogCancellation = True, .ButtonStyle = Ookii.Dialogs.Wpf.TaskDialogButtonStyle.CommandLinks, .MainIcon = Ookii.Dialogs.Wpf.TaskDialogIcon.Information, .MainInstruction = "Select what to do when the timer finishes.", .WindowTitle = "Sleep Timer", .WindowIcon = System.Drawing.SystemIcons.Question, .CenterParent = True}
            Dim DLGBTTTS As New Ookii.Dialogs.Wpf.TaskDialogButton With {.Text = "Use TTS", .CommandLinkNote = "When timer finishes MuPlay will notify you via text to speech."}
            Dim DLGBTPS As New Ookii.Dialogs.Wpf.TaskDialogButton With {.Text = "Pause song", .CommandLinkNote = "When timer finishes MuPlay will pause the current song."}
            Dim DLGBTET As New Ookii.Dialogs.Wpf.TaskDialogButton With {.Text = "Exit", .CommandLinkNote = "When timer finishes MuPlay will close itself."}
            DLG.Buttons.Add(DLGBTTTS)
            DLG.Buttons.Add(DLGBTPS)
            DLG.Buttons.Add(DLGBTET)
            Dim result = DLG.ShowDialog
            If result Is DLGBTTTS Then
                ST.Count(TimeSpan.FromSeconds(DLGIB.Input), Sub()
                                                                Synth.SpeakAsync("Sleep timer finished.")
                                                            End Sub)
            ElseIf result Is DLGBTPS Then
                ST.Count(TimeSpan.FromSeconds(DLGIB.Input), Sub()
                                                                MainPlayer.StreamPause()
                                                            End Sub)
            ElseIf result Is DLGBTET Then
                ST.Count(TimeSpan.FromSeconds(DLGIB.Input), Sub()
                                                                Me.Close()
                                                            End Sub)
            End If
        End If
    End Sub
    Private Sub TitleBar_Exit_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Exit.Click
        Me.Close()
    End Sub
    Private Async Sub TitleBar_SyncScan_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SyncScan.Click
        Overlay(True, True)
        Dim Gtracks = Await MainLibrary.GroupTracksAsync
        Dim files As New List(Of String)
        For Each path In My.Settings.LibrariesPath
            For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, My.Settings.FBD_QuickAcess_SubFolders)).ToArray()
                files.Add(song)
            Next
        Next
        Dim ExcList = files.Except(Gtracks).ToList
        Dim ExcListR = Gtracks.Except(files).ToList
        If ExcList.Count <> 0 Then
            ShowNotification("Scanner", "Found " & ExcList.Count & " songs new,adding to library...", HandyControl.Data.NotifyIconInfoType.Info)
            Await MainLibrary.AddTracksToLibraryAsync(ExcList)
        End If
        If ExcListR.Count <> 0 Then
            ShowNotification("Scanner", "Found " & ExcListR.Count & " songs ,removing from library...", HandyControl.Data.NotifyIconInfoType.Info)
            Await MainLibrary.RemoveTracksFromLibraryAsync(ExcListR)
        End If
        Await MainLibrary.CacheArtists(Utils.AppDataPath)
        Await MainLibrary.CacheYears(Utils.AppDataPath)
        Overlay(False, False)
    End Sub
    Private Sub TitleBar_Tags_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Tags.Click
        Try
            Dim Tags As New Tags(MainPlayer.SourceURL, MainPlayer) With {.Owner = Me}
            Overlay(True, False)
            Tags.ShowDialog()
            Overlay(False, False)
            Tags = Nothing
        Catch ex As Exception
            Overlay(False, False)
        End Try
    End Sub
    Private Sub TitleBar_Reverb_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Reverb.Click
        Overlay(True, False)
        Try
            My.Windows.Reverb.Owner = Me
            My.Windows.Reverb.Update()
            My.Windows.Reverb.ShowDialog()
        Catch ex As Exception
        End Try
        Overlay(False, False)
    End Sub
    Private Sub TitleBar_SoundBooster_OnOff_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SoundBooster_OnOff.Click
        If MainPlayer.IsLoudness Then
            MainPlayer.UpdateLoudness(0, False, True)
        Else
            MainPlayer.UpdateLoudness(0, True, False)
        End If
    End Sub

    Private Sub TitleBar_SoundBooster_PresHard_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SoundBooster_PresHard.Click
        MainPlayer.UpdateLoudness(3)
    End Sub

    Private Sub TitleBar_SoundBooster_PresMed_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SoundBooster_PresMed.Click
        MainPlayer.UpdateLoudness(2)
    End Sub

    Private Sub TitleBar_SoundBooster_PresSoft_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SoundBooster_PresSoft.Click
        MainPlayer.UpdateLoudness(1)
    End Sub
    Private Sub TitleBar_RotateChannel_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_RotateChannel.Click
        If MainPlayer.IsRotate Then
            MainPlayer.SetRotate(False)
        Else
            MainPlayer.SetRotate(True)
        End If
    End Sub

    Private Sub TitleBar_8DRotateChannel_Rate_Fast_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_8DRotateChannel_Rate_Fast.Click
        MainPlayer.UpdateRotate(Player.RotatePreset.Fast)
        TitleBar_8DRotateChannel_Rate_Slow.IsChecked = False
        TitleBar_8DRotateChannel_Rate_Med.IsChecked = False
        TitleBar_8DRotateChannel_Rate_Fast.IsChecked = True
    End Sub

    Private Sub TitleBar_8DRotateChannel_Rate_Med_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_8DRotateChannel_Rate_Med.Click
        MainPlayer.UpdateRotate(Player.RotatePreset.Med)
        TitleBar_8DRotateChannel_Rate_Slow.IsChecked = False
        TitleBar_8DRotateChannel_Rate_Med.IsChecked = True
        TitleBar_8DRotateChannel_Rate_Fast.IsChecked = False
    End Sub

    Private Sub TitleBar_8DRotateChannel_Rate_Slow_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_8DRotateChannel_Rate_Slow.Click
        MainPlayer.UpdateRotate(Player.RotatePreset.Slow)
        TitleBar_8DRotateChannel_Rate_Slow.IsChecked = True
        TitleBar_8DRotateChannel_Rate_Med.IsChecked = False
        TitleBar_8DRotateChannel_Rate_Fast.IsChecked = False
    End Sub
    Private Async Sub TitleBar_UnsyncURL_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_UnsyncURL.Click
        Dim IB As New InputDialog("URL here ...") With {.Owner = Me}
        If IB.ShowDialog Then
            Overlay(True, True)
            Await MainPlayer.LoadStreamAsync(IB.Input, Player.StreamTypes.URL, MainPlaylist)
            Overlay(False, False)
        End If
        IB = Nothing
    End Sub
    Private Async Sub TitleBar_UnsyncSC_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_UnsyncSC.Click
        Dim IB As New InputDialog("URL here ...") With {.Owner = Me}
        If IB.ShowDialog Then
            Overlay(True, True)
            Dim Info = Await SoundCloud.GetInfoAsync(IB.Input)
            MainPlayer.LoadSong(Nothing, MainPlaylist, True, True, True, Info.URI.ToString, True, Info.Title, Info.Artist, Info.Avatar)
            Overlay(False, False)
        End If
        IB = Nothing
    End Sub
    Private Async Sub TitleBar_UnsyncYT_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_UnsyncYT.Click
        Dim IB As New InputDialog("Youtube search query or video URL...") With {.Owner = Me, .Title = "MuPlay"}
        If IB.ShowDialog Then
            Overlay(True, True)
            Try
                Dim urlQueryStrPos As Integer
                'Lets find the index of the query string ?v=
                'right after the equal sign would be the youtube video id which is
                'an 11-character string generated by youtube when a user uploads a video.
                urlQueryStrPos = IB.Input.IndexOf("?v=")
                If urlQueryStrPos < 0 Then
                    Await MainPlayer.LoadStreamAsync(Nothing, Player.StreamTypes.Youtube, MainPlaylist, IB.Input)
                Else
                    Dim youTubeVideoIdStartPos As Integer
                    youTubeVideoIdStartPos = urlQueryStrPos + 3         'locate the start position of the video ID
                    Dim youtubeVideoId As String
                    youtubeVideoId = IB.Input.Substring(youTubeVideoIdStartPos, 11) 'extract the video id from the url string
                    Await MainPlayer.LoadStreamAsync(youtubeVideoId, Player.StreamTypes.Youtube, MainPlaylist)
                End If
            Catch ex As Exception
                media_next_btn_Click(Nothing, New RoutedEventArgs)
                Throw New Exception("An error occured while trying to contact Youtube, try again later or use youtube video id instead of a search query.", ex)
            End Try
            Overlay(False, False)
        End If
        IB = Nothing
    End Sub
    Private Sub TitleBar_15FPS_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles TitleBar_15FPS.Click
        SFXVisualRenderer.Interval = 62 'TimeSpan.FromMilliseconds(62)
    End Sub

    Private Sub TitleBar_30FPS_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles TitleBar_30FPS.Click
        SFXVisualRenderer.Interval = 31 'TimeSpan.FromMilliseconds(31)
    End Sub

    Private Sub TitleBar_60FPS_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles TitleBar_60FPS.Click
        SFXVisualRenderer.Interval = 15 'TimeSpan.FromMilliseconds(15)
    End Sub

    Private Sub TitleBar_Default_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles TitleBar_Default.Click
        LoadVisualizer(Nothing, True)
    End Sub
    Private Sub TitleBar_DefaultMuPlay_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles TitleBar_DefaultMuPlay.Click
        LoadVisualizer(Nothing, False, True)
    End Sub
    Private Sub TitleBar_LoadPlugin_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_LoadPlugin.Click
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True}
        If OFD.ShowDialog Then
            LoadVisualizer(OFD.FileName)
        End If
    End Sub
    Private Async Sub TitleBar_MB_Search_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_MB_Search.Click
        TitleBar_MB_Search.IsEnabled = False
        Dim title As String
        Dim artist As String
        Dim tib As New InputDialog("Song's title...") With {.Owner = Me}
        Dim aib As New InputDialog("Song's artist...") With {.Owner = Me}
        If tib.ShowDialog Then
            title = tib.Input
            If aib.ShowDialog Then
                artist = aib.Input
                MusicBrainz.Song = title
                MusicBrainz.Artist = artist
                Dim Result As MusicBrainz.MusicItem = Await MusicBrainz.Search
                If Result IsNot Nothing Then
                    MessageBox.Show(Me, String.Join(vbCrLf, Result.GetFormattedResult), "Music Brainz", MessageBoxButton.OK, MessageBoxImage.Information)
                End If
            End If
        End If
        tib = Nothing
        aib = Nothing
        title = Nothing
        artist = Nothing
        TitleBar_MB_Search.IsEnabled = True
    End Sub

    Private Async Sub TitleBar_MB_SearchCurrent_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_MB_SearchCurrent.Click
        TitleBar_MB_SearchCurrent.IsEnabled = False
        MusicBrainz.Song = MEDIA_TITLE.Content
        MusicBrainz.Artist = MEDIA_ARTIST.Content
        Dim Result As MusicBrainz.MusicItem = Await MusicBrainz.Search()
        If Result IsNot Nothing Then
            If MessageBox.Show(Me, String.Join(vbCrLf, Result.GetFormattedResult) & vbCrLf & "Do you want to copy tags to current song ?", "Music Brainz", MessageBoxButton.YesNo, MessageBoxImage.Information) = MessageBoxResult.Yes Then
                Dim Tag = TagLib.File.Create(MainPlayer.SourceURL)
                With Tag.Tag
                    .Title = Result.Title
                    .Performers = New String() {Result.Artist}
                    .Album = Result.Album
                    .Year = Result.Year
                End With
                Dim oldvol = MainPlayer.Volume
                Await MainPlayer.FadeVol(0, 1)
                Tag.Save()
                Await MainPlayer.FadeVol(oldvol, 1)
            End If
        End If
        Result = Nothing
        TitleBar_MB_SearchCurrent.IsEnabled = True
    End Sub
    Private Sub MusicBrainz_OnStateChanged(State As MusicBrainz.States) Handles MusicBrainz.OnStateChanged
        ShowNotification("Music Brainz", State.ToString, HandyControl.Data.NotifyIconInfoType.Info)
    End Sub
    Private Async Sub TitleBar_Lyrics_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Lyrics.Click
        If Not String.IsNullOrEmpty(home_lyrics_block.Text) Or Not String.IsNullOrWhiteSpace(home_lyrics_block.Text) Then Exit Sub
        TitleBar_Lyrics.IsEnabled = False
        Overlay(True, True)
        Dim Lrcs As String = Await Lyrics.Beginsearch(TryCast(MEDIA_ARTIST.Content, String).Replace(" ", ""), TryCast(MEDIA_TITLE.Content, String).Replace(" ", ""), MEDIA_TITLE.Content)
        If Lrcs.Trim = "" Then
            If MessageBox.Show(Me, "No lyrics found." & vbCrLf & "Do you want to retry with your own keywords ?", "Lyrics", MessageBoxButton.YesNo, MessageBoxImage.Error) = MessageBoxResult.Yes Then
                Dim title As String
                Dim artist As String
                Dim tib As New InputDialog("Song's title...") With {.Owner = Me}
                Dim aib As New InputDialog("Song's artist...") With {.Owner = Me}
                If tib.ShowDialog Then
                    title = tib.Input
                    If aib.ShowDialog Then
                        artist = aib.Input
                        Lrcs = Await Lyrics.Beginsearch(artist.Replace(" ", ""), title.Replace(" ", ""), title)
                        If Lrcs <> String.Empty Then
                            home_lyrics_block.Text = Lrcs
                        Else
                            MessageBox.Show(Me, "No lyrics found.", "Lyrics", MessageBoxButton.OK, MessageBoxImage.Error)
                        End If
                    End If
                End If
                Lrcs = Nothing
                tib = Nothing
                aib = Nothing
                title = Nothing
                artist = Nothing
            End If
        Else
            home_lyrics_block.Text = Lrcs
            Select Case MessageBox.Show(Me, "Do you want to copy the found lyrics to the song ?,use cancel to clear lyrics." & vbCrLf & "Lyrics: " & vbCrLf & Lrcs.Substring(0, 200) & "...", "MuPlay", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
                Case MessageBoxResult.Yes
                    Try
                        Dim Fi As New IO.FileInfo(MainPlayer.SourceURL)
                        Dim WasItReadOnly As Boolean = Fi.IsReadOnly
                        If Fi.IsReadOnly = True Then
                            If MessageBox.Show(Me, "The file you're trying to write to is read-only." & vbCrLf & "Would you like to temporarily disable write-protection ?", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                                Fi.IsReadOnly = False
                            Else
                                Exit Sub
                            End If
                        End If
                        Dim oldvol = MainPlayer.Volume
                        If MainPlayer.FadeAudio Then
                            Await MainPlayer.FadeVol(0, 1)
                        End If
                        Dim Pos = MainPlayer.GetPosition
                        MainPlayer.StreamStop()
                        MainPlayer.Dispose()
                        Dim tags = TagLib.File.Create(MainPlayer.SourceURL)
                        tags.Tag.Lyrics = Lrcs
                        tags.Save()
                        MainPlayer.Init()
                        MainPlayer.LoadSong(MainPlayer.SourceURL, Nothing, False, False)
                        MainPlayer.SetPosition(Pos)
                        MainPlayer.SetVolume(0, False)
                        MainPlayer.StreamPlay()
                        If MainPlayer.FadeAudio Then
                            Await MainPlayer.FadeVol(oldvol, 1)
                        End If
                        If WasItReadOnly = True Then
                            Fi.IsReadOnly = True
                        End If
                    Catch ex As Exception
                        MessageBox.Show(Me, "An error occured, try again later." & vbCrLf & ex.Message, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
                    End Try
                Case MessageBoxResult.Cancel
                    home_lyrics_block.Text = String.Empty
            End Select
        End If
        Overlay(False, False)
        TitleBar_Lyrics.IsEnabled = True
    End Sub

    Private Sub Lyrics_OnStateChanged(State As Lyrics.States) Handles Lyrics.OnStateChanged
        ShowNotification("Lyrics", State.ToString, HandyControl.Data.NotifyIconInfoType.Info)
    End Sub
    Private Sub TitleBar_Upnp_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Upnp.Click
        My.Windows.LanBrowser.Owner = Me
        My.Windows.LanBrowser.Show()
    End Sub

    Private Sub TitleBar_UnsyncAdd_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_UnsyncAdd.Click
        'Dim Opf As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.Title = "MuPlayer - Add Song", .Filter = Utils.OFDFileFilters}
        'If Opf.ShowDialog() Then
        '    MainPlayer.LoadSong(Opf.FileName, MainPlaylist)
        '    MainPlayer.StreamPlay()
        '    MainUIManager.IsEnabled = True
        '    If My.Settings.UseAnimations Then
        '        Switches_OverlayChanger.IsChecked = False
        '    Else
        '        QuickAccess_MainCanvas.Visibility = Visibility.Hidden
        '    End If
        'End If
        'Opf = Nothing
        Dim Opf As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.Multiselect = True, .Title = "MuPlayer - Add songs", .Filter = Utils.OFDFileFilters}
        If Opf.ShowDialog() Then
            For Each song In Opf.FileNames
                MainPlaylist.Add(song, Player.StreamTypes.Local)
            Next
            MainPlayer.LoadSong(Opf.FileNames(Opf.FileNames.Length - 1), MainPlaylist, False)
            MainUIManager.IsEnabled = True
            If My.Settings.UseAnimations Then
                Switches_OverlayChanger.IsChecked = False
            Else
                QuickAccess_MainCanvas.Visibility = Visibility.Hidden
            End If
        End If
        Opf = Nothing
    End Sub
#End Region
#Region "Visualizer Stuff"
    Dim hSFX3 As Long
    Dim SFXCurrentVisualiserLoc As String
    'Public WithEvents SFXVisualRenderer As New System.Windows.Threading.DispatcherTimer
    Public WithEvents SFXVisualRenderer As New Forms.Timer
    Dim WithEvents SFXFPSReseter As New Timers.Timer With {.Interval = 1000}
    Dim FPS As Integer
    Dim WithEvents fftanalyzer As Analyzer
    Private Sub fftanalyzer_DataArrived(Data As List(Of Byte)) Handles fftanalyzer.DataArrived
        Visualiser_Monstercat_p1.SetSmoothValue(Data(0))
        Visualiser_Monstercat_p2.SetSmoothValue(Data(1))
        Visualiser_Monstercat_p3.SetSmoothValue(Data(2))
        Visualiser_Monstercat_p4.SetSmoothValue(Data(3))
        Visualiser_Monstercat_p5.SetSmoothValue(Data(4))
        Visualiser_Monstercat_p6.SetSmoothValue(Data(5))
        Visualiser_Monstercat_p7.SetSmoothValue(Data(6))
        Visualiser_Monstercat_p8.SetSmoothValue(Data(7))
        Visualiser_Monstercat_p9.SetSmoothValue(Data(8))
        Visualiser_Monstercat_p10.SetSmoothValue(Data(9))
        Visualiser_Monstercat_p11.SetSmoothValue(Data(10))
        Visualiser_Monstercat_p12.SetSmoothValue(Data(11))
        Visualiser_Monstercat_p13.SetSmoothValue(Data(12))
        Visualiser_Monstercat_p14.SetSmoothValue(Data(13))
        Visualiser_Monstercat_p15.SetSmoothValue(Data(14))
        Visualiser_Monstercat_p16.SetSmoothValue(Data(15))
        Visualiser_Monstercat_p17.SetSmoothValue(Data(16))
        Visualiser_Monstercat_p18.SetSmoothValue(Data(17))
        Visualiser_Monstercat_p19.SetSmoothValue(Data(18))
        Visualiser_Monstercat_p20.SetSmoothValue(Data(19))
        Visualiser_Monstercat_p21.SetSmoothValue(Data(20))
        Visualiser_Monstercat_p22.SetSmoothValue(Data(21))
        Visualiser_Monstercat_p23.SetSmoothValue(Data(22))
        Visualiser_Monstercat_p24.SetSmoothValue(Data(23))
        Visualiser_Monstercat_p25.SetSmoothValue(Data(24))
        Visualiser_Monstercat_p26.SetSmoothValue(Data(25))
        Visualiser_Monstercat_p27.SetSmoothValue(Data(26))
        Visualiser_Monstercat_p28.SetSmoothValue(Data(27))
        Visualiser_Monstercat_p29.SetSmoothValue(Data(28))
        Visualiser_Monstercat_p30.SetSmoothValue(Data(29))
        Visualiser_Monstercat_p31.SetSmoothValue(Data(30))
        Visualiser_Monstercat_p32.SetSmoothValue(Data(31))
        Visualiser_Monstercat_p33.SetSmoothValue(Data(32))
        Visualiser_Monstercat_p34.SetSmoothValue(Data(33))
        Visualiser_Monstercat_p35.SetSmoothValue(Data(34))
        Visualiser_Monstercat_p36.SetSmoothValue(Data(35))
        Visualiser_Monstercat_p37.SetSmoothValue(Data(36))
        Visualiser_Monstercat_p38.SetSmoothValue(Data(37))
        Visualiser_Monstercat_p39.SetSmoothValue(Data(38))
        Visualiser_Monstercat_p40.SetSmoothValue(Data(39))
        Visualiser_Monstercat_p41.SetSmoothValue(Data(40))
        Visualiser_Monstercat_p42.SetSmoothValue(Data(41))
        Visualiser_Monstercat_p43.SetSmoothValue(Data(42))
        Visualiser_Monstercat_p44.SetSmoothValue(Data(43))
        Visualiser_Monstercat_p45.SetSmoothValue(Data(44))
        Visualiser_Monstercat_p46.SetSmoothValue(Data(45))
        Visualiser_Monstercat_p47.SetSmoothValue(Data(46))
        Visualiser_Monstercat_p48.SetSmoothValue(Data(47))
        Visualiser_Monstercat_p49.SetSmoothValue(Data(48))
        Visualiser_Monstercat_p50.SetSmoothValue(Data(49))
        Visualiser_Monstercat_p51.SetSmoothValue(Data(50))
        Visualiser_Monstercat_p52.SetSmoothValue(Data(51))
        Visualiser_Monstercat_p53.SetSmoothValue(Data(52))
        Visualiser_Monstercat_p54.SetSmoothValue(Data(53))
        Visualiser_Monstercat_p55.SetSmoothValue(Data(54))
        Visualiser_Monstercat_p56.SetSmoothValue(Data(55))
        Visualiser_Monstercat_p57.SetSmoothValue(Data(56))
        Visualiser_Monstercat_p58.SetSmoothValue(Data(57))
        Visualiser_Monstercat_p59.SetSmoothValue(Data(58))
        Visualiser_Monstercat_p60.SetSmoothValue(Data(59))
        Visualiser_Monstercat_p61.SetSmoothValue(Data(60))
        Visualiser_Monstercat_p62.SetSmoothValue(Data(61))
        Visualiser_Monstercat_p63.SetSmoothValue(Data(62))
        Visualiser_Monstercat_p64.SetSmoothValue(Data(63))
    End Sub
    Public Sub LoadVisualizer(ByVal loc As String, Optional UseDefault As Boolean = False, Optional UseMuPlayDefault As Boolean = False)
        If UseMuPlayDefault Then
            Visualiser_Monstercat_Logo.Source = Home_NowPlaying.Source
            Visualiser_Monstercat_Artist.Text = MEDIA_ARTIST.Content
            Visualiser_Monstercat_Title.Text = MEDIA_TITLE.Content
            fftanalyzer = New Analyzer(New ProgressBar, New ProgressBar, MainPlayer)
            fftanalyzer.Enable = True
            Visualiser_Host.Visibility = Visibility.Hidden
            SFXVisualRenderer.Stop()
            SFXFPSReseter.Stop()
            visualiser_off.Visibility = Visibility.Hidden
            visualiser_resize.Visibility = Visibility.Hidden
            visualiser_fps.Visibility = Visibility.Hidden
            Visualiser_Monstercat.Visibility = Visibility.Visible
            Exit Sub
        End If
        BassSfx.BASS_SFX_PluginStop(hSFX3)
        If (Not isSFXLoaded) Then
            ShowNotification("MuPlay", "There was an error while initializing SFX engine.", HandyControl.Data.NotifyIconInfoType.[Error])
        Else
            If fftanalyzer IsNot Nothing Then
                fftanalyzer.Enable = False
                fftanalyzer = Nothing
            End If
            Visualiser_Host.Visibility = Visibility.Visible
            SFXCurrentVisualiserLoc = loc
            If (Not UseDefault) Then
                hSFX3 = BassSfx.BASS_SFX_PluginCreate(loc, Visualiser_Target.Handle, Visualiser_Target.Width, Visualiser_Target.Height, BASSSFXFlag.BASS_SFX_DEFAULT)
            Else
                hSFX3 = BassSfx.BASS_SFX_PluginCreate("0AA02E8D-F851-4CB0-9F64-BBA9BE7A983D", Visualiser_Target.Handle, Visualiser_Target.Width, Visualiser_Target.Height, BASSSFXFlag.BASS_SFX_DEFAULT)
            End If
            BassSfx.BASS_SFX_PluginSetStream(hSFX3, MainPlayer.Stream)
            BassSfx.BASS_SFX_PluginStart(hSFX3)
            Resources("Visualiser_Tip") = BassSfx.BASS_SFX_PluginGetName(hSFX3)
            SFXVisualRenderer.Start()
            SFXFPSReseter.Start()
            visualiser_off.Visibility = Visibility.Visible
            visualiser_resize.Visibility = Visibility.Visible
            visualiser_fps.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub SFXVisualRenderer_Tick() Handles SFXVisualRenderer.Tick
        If (hSFX3 <> -1) Then
            Try
                'Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromHwnd(Visualiser_Target.Handle)
                BassSfx.BASS_SFX_PluginRender(hSFX3, MainPlayer.Stream, Visualiser_Target.CreateGraphics.GetHdc)
                'g.Dispose()
                FPS += 1
            Catch ex As Exception
            End Try
        End If
    End Sub
    Private Sub SFXFPSReseter_Elapsed() Handles SFXFPSReseter.Elapsed
        Dispatcher.Invoke(Sub()
                              visualiser_fps.Content = FPS & "FPS"
                              FPS = 0
                          End Sub)
    End Sub
    Private Sub visualiser_off_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles visualiser_off.Click
        SFXVisualRenderer.Stop()
        SFXFPSReseter.Stop()
        BassSfx.BASS_SFX_PluginFree(hSFX3)
        visualiser_off.Visibility = Visibility.Hidden
        visualiser_resize.Visibility = Visibility.Hidden
        visualiser_fps.Visibility = Visibility.Hidden
    End Sub

    Private Sub visualiser_resize_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles visualiser_resize.Click
        BassSfx.BASS_SFX_PluginStop(hSFX3)
        If SFXCurrentVisualiserLoc IsNot Nothing Then
            hSFX3 = BassSfx.BASS_SFX_PluginCreate(SFXCurrentVisualiserLoc, Visualiser_Target.Handle, Visualiser_Target.Width, Visualiser_Target.Height, BASSSFXFlag.BASS_SFX_DEFAULT)
        Else
            hSFX3 = BassSfx.BASS_SFX_PluginCreate("0AA02E8D-F851-4CB0-9F64-BBA9BE7A983D", Visualiser_Target.Handle, Visualiser_Target.Width, Visualiser_Target.Height, BASSSFXFlag.BASS_SFX_DEFAULT)
        End If
        BassSfx.BASS_SFX_PluginSetStream(hSFX3, MainPlayer.Stream)
        BassSfx.BASS_SFX_PluginStart(hSFX3)
        SFXVisualRenderer.Start()
        SFXFPSReseter.Start()
    End Sub

#End Region
#Region "Fancy Background(i guess)"
    Public AniCounter As Integer = 1
    Public StopToken As Boolean = False
    Public Sub DoAnim(Count As Integer)
        If StopToken = False Then
            Dim RND As New Random
            Select Case Count
                Case 1
                    Dim Tanim1 As New ThicknessAnimation
                    Tanim1.From = Home_FancyBackground_el1.Margin
                    'Tanim1.To = New Thickness(Width - (Home_FancyBackground_el1.Width + 10), Height - (Home_FancyBackground_el1.Height + 10), 0, 0)
                    Tanim1.To = New Thickness(Width - RND.Next(0, 260), Height - RND.Next(0, 260), 0, 0)
                    Tanim1.Duration = TimeSpan.FromSeconds(7)
                    Storyboard.SetTarget(Tanim1, Home_FancyBackground_el1)
                    Storyboard.SetTargetProperty(Tanim1, New PropertyPath("Margin"))
                    Dim Tanim2 As New ThicknessAnimation
                    Tanim2.From = Home_FancyBackground_el2.Margin
                    'Tanim2.To = New Thickness(0, 0, 0, 0)
                    Tanim2.To = New Thickness(RND.Next(0, 260), RND.Next(0, 260), 0, 0)
                    Tanim2.Duration = TimeSpan.FromSeconds(7)
                    Storyboard.SetTarget(Tanim2, Home_FancyBackground_el2)
                    Storyboard.SetTargetProperty(Tanim2, New PropertyPath("Margin"))
                    Dim sb As New Storyboard
                    sb.Children.Add(Tanim1)
                    sb.Children.Add(Tanim2)
                    AddHandler sb.Completed, AddressOf AnimationCompleted
                    sb.Begin()
                Case 2
                    Dim Tanim1 As New ThicknessAnimation
                    Tanim1.From = Home_FancyBackground_el1.Margin
                    'Tanim1.To = New Thickness(Width - (Home_FancyBackground_el1.Width + 10), 0, 0, 0)
                    Tanim1.To = New Thickness(Width - RND.Next(0, 260), RND.Next(0, 260), 0, 0)
                    Tanim1.Duration = TimeSpan.FromSeconds(7)
                    Storyboard.SetTarget(Tanim1, Home_FancyBackground_el1)
                    Storyboard.SetTargetProperty(Tanim1, New PropertyPath("Margin"))
                    Dim Tanim2 As New ThicknessAnimation
                    Tanim2.From = Home_FancyBackground_el2.Margin
                    'Tanim2.To = New Thickness(0, Height - (Home_FancyBackground_el2.Height + 10), 0, 0)
                    Tanim2.To = New Thickness(RND.Next(0, 260), Height - RND.Next(0, 260), 0, 0)
                    Tanim2.Duration = TimeSpan.FromSeconds(7)
                    Storyboard.SetTarget(Tanim2, Home_FancyBackground_el2)
                    Storyboard.SetTargetProperty(Tanim2, New PropertyPath("Margin"))
                    Dim sb As New Storyboard
                    sb.Children.Add(Tanim1)
                    sb.Children.Add(Tanim2)
                    AddHandler sb.Completed, AddressOf AnimationCompleted
                    sb.Begin()
                Case 3
                    Dim Tanim1 As New ThicknessAnimation
                    Tanim1.From = Home_FancyBackground_el1.Margin
                    'Tanim1.To = New Thickness(0, 0, 0, 0)
                    Tanim1.To = New Thickness(RND.Next(0, 260), RND.Next(0, 260), 0, 0)
                    Tanim1.Duration = TimeSpan.FromSeconds(7)
                    Storyboard.SetTarget(Tanim1, Home_FancyBackground_el1)
                    Storyboard.SetTargetProperty(Tanim1, New PropertyPath("Margin"))
                    Dim Tanim2 As New ThicknessAnimation
                    Tanim2.From = Home_FancyBackground_el2.Margin
                    'Tanim2.To = New Thickness(Width - (Home_FancyBackground_el2.Width + 10), Height - (Home_FancyBackground_el2.Height + 10), 0, 0)
                    Tanim2.To = New Thickness(Width - RND.Next(0, 260), Height - RND.Next(0, 260), 0, 0)
                    Tanim2.Duration = TimeSpan.FromSeconds(7)
                    Storyboard.SetTarget(Tanim2, Home_FancyBackground_el2)
                    Storyboard.SetTargetProperty(Tanim2, New PropertyPath("Margin"))
                    Dim sb As New Storyboard
                    sb.Children.Add(Tanim1)
                    sb.Children.Add(Tanim2)
                    AddHandler sb.Completed, AddressOf AnimationCompleted
                    sb.Begin()
            End Select
        Else
            StopToken = False
        End If
    End Sub
    Private Sub AnimationCompleted()
        Select Case AniCounter
            Case 1
                DoAnim(1)
                AniCounter += 1
            Case 2
                DoAnim(2)
                AniCounter += 1
            Case 3
                DoAnim(3)
                AniCounter = 1
        End Select
    End Sub
#End Region
    Private Sub Home_NowPlaying_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Home_NowPlaying.MouseDown
        If e.ClickCount = 5 Then
            If My.Settings.UseAnimations Then
                Dim Wanim As New Animation.DoubleAnimation
                Wanim.From = 430
                Wanim.To = 0
                Wanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Wanim, vbx)
                Animation.Storyboard.SetTargetProperty(Wanim, New PropertyPath("Width"))
                Dim Hanim As New Animation.DoubleAnimation
                Hanim.From = 200
                Hanim.To = 0
                Hanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Hanim, vbx)
                Animation.Storyboard.SetTargetProperty(Hanim, New PropertyPath("Height"))
                Dim sb As New Animation.Storyboard
                sb.Children.Add(Wanim)
                sb.Children.Add(Hanim)
                sb.AutoReverse = True
                sb.Begin()
            End If
        End If
    End Sub

    Private Sub TaskBarPlayPause_Click(sender As Object, e As ThumbnailButtonClickedEventArgs) Handles TaskBarPlayPause.Click
        If MainPlayer.PlayerState = Player.State.Playing Then
            MainPlayer.StreamPause()
        Else
            MainPlayer.StreamPlay()
        End If
    End Sub

    Private Sub TaskBarNext_Click(sender As Object, e As ThumbnailButtonClickedEventArgs) Handles TaskBarNext.Click
        media_next_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub TaskBarPrev_Click(sender As Object, e As ThumbnailButtonClickedEventArgs) Handles TaskBarPrev.Click
        media_prev_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub NotifyIconMain_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIconMain.Click
        MediaBar.ShowAnim(My.Settings.MediaBar_AnimType)
    End Sub

    Private Sub media_like_btn_Click(sender As Object, e As RoutedEventArgs) Handles media_like_btn.Click
        If My.Settings.UseAnimations Then
            Switches_Favourite.IsChecked = False
        End If
        If My.Settings.FavouriteTracks.Contains(MainPlayer.SourceURL) Then
            My.Settings.FavouriteTracks.Remove(MainPlayer.SourceURL)
            libraryFavouritesItems.Remove(libraryFavouritesItems.FirstOrDefault(Function(k) k.Source = MainPlayer.SourceURL))
            media_like_btn.Foreground = Brushes.Black
            Resources("fav_Tip") = "I love it"
        Else
            My.Settings.FavouriteTracks.Add(MainPlayer.SourceURL)
            Try
                Dim Tag = TagLib.File.Create(MainPlayer.SourceURL).Tag
                libraryFavouritesItems.Add(New PlaylistItem(libraryFavouritesItems.Count, Tag.Title, Tag.JoinedPerformers, Tag.Album, Tag.Year, Tag.Track, Player.StreamTypes.Local, MainPlayer.SourceURL, Nothing))
            Catch ex As Exception
                libraryFavouritesItems.Add(New PlaylistItem(libraryFavouritesItems.Count, IO.Path.GetFileNameWithoutExtension(MainPlayer.SourceURL), "Not available", "Not available", 0, 0, Player.StreamTypes.Local, MainPlayer.SourceURL, Nothing))
            End Try
            media_like_btn.Foreground = Brushes.Yellow
            Resources("fav_Tip") = "I hate it"
        End If
        If My.Settings.UseAnimations Then
            Switches_Favourite.IsChecked = True
        End If
    End Sub

    Private Sub NotifyIcon_Exit_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Exit.Click
        Me.Close()
    End Sub

    Private Async Sub NotifyIcon_MuPlay_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_MuPlay.Click
        Dim s As System.IO.Stream = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("WpfPlayer.MuPlay.wav")
        Dim player As System.Media.SoundPlayer = New System.Media.SoundPlayer(s)
        Dim oldvol = MainPlayer.Volume
        Await MainPlayer.FadeVol(0.1, 1)
        player.Play()
        s.Dispose()
        Await Task.Delay(940)
        Await MainPlayer.FadeVol(oldvol, 1)
    End Sub

    Private Sub NotifyIcon_Next_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Next.Click
        media_next_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub NotifyIcon_Play_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Play.Click
        media_play_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub NotifyIcon_Previous_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Previous.Click
        media_prev_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub NotifyIcon_Volume0_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Volume0.Click
        MainPlayer.SetVolume(0)
    End Sub

    Private Sub NotifyIcon_Volume25_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Volume25.Click
        MainPlayer.SetVolume(0.25)
    End Sub

    Private Sub NotifyIcon_Volume50_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Volume50.Click
        MainPlayer.SetVolume(0.5)
    End Sub

    Private Sub NotifyIcon_Volume75_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Volume75.Click
        MainPlayer.SetVolume(0.75)
    End Sub

    Private Sub NotifyIcon_Volume100_Click(sender As Object, e As RoutedEventArgs) Handles NotifyIcon_Volume100.Click
        MainPlayer.SetVolume(1)
    End Sub

    Private Async Sub TitleBar_Playlist_Favourites_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Playlist_Favourites.Click
        If My.Settings.FavouriteTracks.Count > 1 Then
            MainPlaylist.Clear()
            Overlay(True, True)
            Await Dispatcher.InvokeAsync(Sub()
                                             For i As Integer = 0 To My.Settings.FavouriteTracks.Count - 1
                                                 MainPlaylist.Add(My.Settings.FavouriteTracks(i), Player.StreamTypes.Local, False)
                                                 Home_Overlay_State.Text = "Adding songs ...(" & i + 1 & "/" & My.Settings.FavouriteTracks.Count & ")"
                                             Next
                                         End Sub, System.Windows.Threading.DispatcherPriority.Background)
            Overlay(False, False)
            MainPlayer.LoadSong(MainPlaylist.JumpTo(0), MainPlaylist, False)
        End If
    End Sub

    Private Sub media_loop_Click(sender As Object, e As RoutedEventArgs) Handles media_loop.Click
        Select Case MainPlayer.RepeateType
            Case Player.RepeateBehaviour.NoRepeat
                MainPlayer.RepeateType = Player.RepeateBehaviour.RepeatAll
            Case Player.RepeateBehaviour.RepeatAll
                MainPlayer.RepeateType = Player.RepeateBehaviour.RepeatOne
            Case Player.RepeateBehaviour.RepeatOne
                MainPlayer.RepeateType = Player.RepeateBehaviour.NoRepeat
        End Select
    End Sub

    Private Sub media_shuffle_Click(sender As Object, e As RoutedEventArgs) Handles media_shuffle.Click
        Select Case MainPlayer.Shuffle
            Case Player.RepeateBehaviour.NoShuffle
                MainPlayer.Shuffle = Player.RepeateBehaviour.Shuffle
            Case Player.RepeateBehaviour.Shuffle
                MainPlayer.Shuffle = Player.RepeateBehaviour.NoShuffle
        End Select
    End Sub

    Private Async Sub TitleBar_Playlist_All_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Playlist_All.Click
        Overlay(True, True)
        Home_Overlay_State.Text = "Adding songs..."
        Await Task.Run(Sub()
                           MainPlaylist.Clear()
                           For i As Integer = 0 To My.Settings.LibrariesPath.Count - 1
                               Dim files = Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(My.Settings.LibrariesPath(i), filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                               For _i As Integer = 0 To files.Count - 1
                                   Dim ix = _i
                                   Dispatcher.BeginInvoke(Sub()
                                                              Home_Overlay_State.Text = "From library " & i & " " & ix + 1 & "/" & files.Count & " song added."
                                                          End Sub)
                                   MainPlaylist.Add(files(_i), Player.StreamTypes.Local)
                               Next
                           Next
                       End Sub)
        Overlay(False, False)
        MainPlayer.LoadSong(MainPlaylist.JumpTo(0), MainPlaylist, False)
    End Sub

    Private Async Sub TitleBar_Playlist_Random10_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Playlist_Random10.Click
        Overlay(True, True)
        Await Dispatcher.InvokeAsync(Sub()
                                         Dim Rnd As New Random
                                         Dim RndSongs As New List(Of String)
                                         Dim files As New List(Of String)
                                         Dim _files As String()
                                         For Each path In My.Settings.LibrariesPath
                                             _files = Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                             For Each song In _files
                                                 files.Add(song)
                                             Next
                                         Next
                                         For i As Integer = 0 To 9
                                             RndSongs.Add(files(Rnd.Next(0, files.Count - 1)))
                                         Next
                                         MainPlaylist.Clear()
                                         For Each song In RndSongs
                                             MainPlaylist.Add(song, Player.StreamTypes.Local)
                                         Next
                                         MainPlayer.LoadSong(MainPlaylist.JumpTo(0), MainPlaylist, False)
                                         Rnd = Nothing
                                         RndSongs = Nothing
                                         files = Nothing
                                         _files = Nothing
                                     End Sub, System.Windows.Threading.DispatcherPriority.Background)
        Overlay(False, False)
    End Sub

    Private Async Sub TitleBar_Playlist_ShuffleAll_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Playlist_ShuffleAll.Click
        Overlay(True, True)
        Home_Overlay_State.Text = "Adding and shuffling songs..."
        Await Dispatcher.InvokeAsync(Sub()
                                         Dim Songs As New List(Of String)
                                         For i As Integer = 0 To My.Settings.LibrariesPath.Count - 1
                                             Dim files = Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(My.Settings.LibrariesPath(i), filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                             For _i As Integer = 0 To files.Count - 1
                                                 Home_Overlay_State.Text = "From library " & i & " " & _i + 1 & "/" & files.Count & " song added."
                                                 Songs.Add(files(_i))
                                             Next
                                         Next
                                         Songs = Utils.Shuffle(Songs)
                                         MainPlaylist.Clear()
                                         For Each song In Songs
                                             MainPlaylist.Add(song, Player.StreamTypes.Local)
                                         Next
                                         Songs = Nothing
                                     End Sub, System.Windows.Threading.DispatcherPriority.Background)
        Overlay(False, False)
        MainPlayer.LoadSong(MainPlaylist.JumpTo(0), MainPlaylist, False)
    End Sub

    Private Sub TitleBar_BalanceLeft_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_BalanceLeft.Click
        MainPlayer.SetBalance(-1)
        TitleBar_BalanceLeft.IsChecked = True
        TitleBar_BalanceCenter.IsChecked = False
        TitleBar_BalanceRight.IsChecked = False
    End Sub

    Private Sub TitleBar_BalanceCenter_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_BalanceCenter.Click
        MainPlayer.SetBalance(0)
        TitleBar_BalanceLeft.IsChecked = False
        TitleBar_BalanceCenter.IsChecked = True
        TitleBar_BalanceRight.IsChecked = False
    End Sub

    Private Sub TitleBar_BalanceRight_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_BalanceRight.Click
        MainPlayer.SetBalance(1)
        TitleBar_BalanceLeft.IsChecked = False
        TitleBar_BalanceCenter.IsChecked = False
        TitleBar_BalanceRight.IsChecked = True
    End Sub
    Private Sub TitleBar_Balance_Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        MainPlayer.SetBalance(e.NewValue / 100)
        If e.NewValue = 0 Then
            TitleBar_BalanceCenter.IsChecked = True
            TitleBar_BalanceLeft.IsChecked = False
            TitleBar_BalanceRight.IsChecked = False
        ElseIf e.NewValue = 100 Then
            TitleBar_BalanceCenter.IsChecked = False
            TitleBar_BalanceLeft.IsChecked = False
            TitleBar_BalanceRight.IsChecked = True
        ElseIf e.NewValue = -100 Then
            TitleBar_BalanceCenter.IsChecked = False
            TitleBar_BalanceLeft.IsChecked = True
            TitleBar_BalanceRight.IsChecked = False
        Else
            TitleBar_BalanceCenter.IsChecked = False
            TitleBar_BalanceLeft.IsChecked = False
            TitleBar_BalanceRight.IsChecked = False
        End If
    End Sub

    Private Sub TitleBar_Mono_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Mono.Click
        MainPlayer.Mono = Not MainPlayer.Mono
        If MainPlayer.Mono Then
            TitleBar_Mono.IsChecked = True
        Else
            TitleBar_Mono.IsChecked = False
        End If
    End Sub

    Private Sub TitleBar_SampleRate_Reset_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SampleRate_Reset.Click
        MainPlayer.SetSampleRate(0)
    End Sub

    Private Sub TitleBar_SampleRate_Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        MainPlayer.SetSampleRate(Int(e.NewValue))
    End Sub

    Private Sub TitleBar_StreamDownloader_DownloadCurrent_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_StreamDownloader_DownloadCurrent.Click
        StreamDownloader.DownloadCurrent()
    End Sub
    Private Sub TitleBar_SteroMix_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_SteroMix.Click
        If MainPlayer.IsStereoMix Then
            MainPlayer.SetStereoMix(False)
        Else
            MainPlayer.SetStereoMix(True)
        End If
    End Sub

    Private Sub StreamDownloader_OnStateChanged(State As StreamDownloader.States) Handles StreamDownloader.OnStateChanged
        Dispatcher.Invoke(Sub()
                              If My.Settings.StreamDownloaderNotify Then
                                  Select Case State
                                      Case StreamDownloader.States.Downloading
                                          ShowNotification("Stream downloader", "Downloading...", HandyControl.Data.NotifyIconInfoType.Info)
                                      Case StreamDownloader.States.DownloadCompleted
                                          ShowNotification("Stream downloader", "Download completed.", HandyControl.Data.NotifyIconInfoType.Info)
                                      Case StreamDownloader.States.FatalError
                                          ShowNotification("Stream downloader", "An error occued while downloading the file.", HandyControl.Data.NotifyIconInfoType.Error)
                                      Case StreamDownloader.States.Local
                                          ShowNotification("Stream downloader", "The file you are trying to download is a local file.", HandyControl.Data.NotifyIconInfoType.Error)
                                  End Select
                              End If
                          End Sub, System.Windows.Threading.DispatcherPriority.Background)
    End Sub

    Private Sub TitleBar_StreamDownloader_State_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_StreamDownloader_State.Click
        Dispatcher.Invoke(Sub()
                              If StreamDownloader.State = StreamDownloader.States.Downloading Then
                                  ShowNotification("Stream downloader", "Downloading... " & StreamDownloader.Progress & "%", HandyControl.Data.NotifyIconInfoType.Info)
                              Else
                                  ShowNotification("Stream downloader", "State: " & StreamDownloader.State.ToString, HandyControl.Data.NotifyIconInfoType.Info)
                              End If
                          End Sub, System.Windows.Threading.DispatcherPriority.Background)
    End Sub

    Private Sub SoundCloud_OnStateChanged(State As SoundCloud.State) Handles SoundCloud.OnStateChanged
        Dispatcher.Invoke(Sub()
                              If My.Settings.SoundCloud_Notify Then
                                  Select Case State
                                      Case SoundCloud.State.Free
                                          ShowNotification("SoundCloud", "All tasks completed", HandyControl.Data.NotifyIconInfoType.Info)
                                      Case SoundCloud.State.Connected
                                          ShowNotification("SoundCloud", "Connected to SoundCloud", HandyControl.Data.NotifyIconInfoType.Info)
                                      Case SoundCloud.State.FatalError
                                          ShowNotification("SoundCloud", "Fatal error, try again later.", HandyControl.Data.NotifyIconInfoType.Error)
                                  End Select
                              End If
                          End Sub, System.Windows.Threading.DispatcherPriority.Background)
    End Sub

    Private Async Sub Library_Tracks_Lview_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Library_Tracks_Lview.SelectionChanged
        If My.Settings.UseAnimations Then
            Switches_Library_Tracks.IsChecked = True
        Else
            Library_Tracks_Overlay.Visibility = Visibility.Visible
        End If
        Dim Cover = Utils.GetAlbumArt(libraryItems(Library_Tracks_Lview.SelectedIndex).Source)
        Dim Mitem As MusicItem = Nothing
        If Cover IsNot Nothing Then
            Mitem = New MusicItem(libraryItems(Library_Tracks_Lview.SelectedIndex).Source, libraryItems(Library_Tracks_Lview.SelectedIndex).Title, libraryItems(Library_Tracks_Lview.SelectedIndex).Artist, Cover, MainPlayer, PreviewPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
        Else
            Mitem = New MusicItem(libraryItems(Library_Tracks_Lview.SelectedIndex).Source, IO.Path.GetFileNameWithoutExtension(libraryItems(Library_Tracks_Lview.SelectedIndex).Source), "Not available", New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"), MainPlayer, PreviewPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
        End If
        Library_Track_SPanel.Children.Clear()
        Library_Track_SPanel.Children.Add(Mitem)
        If My.Settings.UseAnimations Then
            Switches_Library_Tracks_Mitem.IsChecked = True
        Else
            Library_Track_SPanel.Visibility = Visibility.Visible
        End If
        Await Mitem.WaitCloseAsync()
        If My.Settings.UseAnimations Then
            Switches_Library_Tracks_Mitem.IsChecked = False
        Else
            Library_Track_SPanel.Visibility = Visibility.Hidden
        End If
        If My.Settings.UseAnimations Then
            Switches_Library_Tracks.IsChecked = False
        Else
            Library_Tracks_Overlay.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Async Sub Library_Artists_Lview_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Library_Artists_Lview.SelectionChanged
        If My.Settings.UseAnimations Then
            Library_Artists_SpanelView.Visibility = Visibility.Visible
            Switches_Library_Artists.IsChecked = True
        Else
            Library_Artists_SpanelView.Visibility = Visibility.Visible
            Library_Artists_Overlay.Visibility = Visibility.Visible
        End If
        Dim Mitem As MusicItem = Nothing
        Dim cover = Utils.GetAlbumArt(libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs(0))
        If cover Is Nothing Then
            Mitem = New MusicItem(libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs, libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Name, New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"), MainPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
        Else
            Mitem = New MusicItem(libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs, libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Name, cover, MainPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
        End If
        libraryArtistsLVItems.Clear()
        For i As Integer = 0 To libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs.Count - 1
            Try
                Dim info = Utils.GetSongInfo(libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs(i))
                libraryArtistsLVItems.Add(New PlaylistItem(i + 1, info(1), info(0), info(2), info(3), info(4), Player.StreamTypes.Local, libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs(i), Utils.GetAlbumArt(libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs(i))))
            Catch ex As Exception
                libraryArtistsLVItems.Add(New PlaylistItem(i + 1, IO.Path.GetFileNameWithoutExtension(libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs(i)), "Not available", "Not available", 0, 0, Player.StreamTypes.Local, libraryArtistsItems(Library_Artists_Lview.SelectedIndex).Songs(i), Nothing))
            End Try
        Next
        Library_Artists_SPanel.Children.Clear()
        Library_Artists_SPanel.Children.Add(Mitem)
        Library_Artists_SPanel.Children.Add(Library_ArtistsLV)
        If My.Settings.UseAnimations Then
            Switches_Library_Artists_Mitem.IsChecked = True
        Else
            Library_Artists_SPanel.Visibility = Visibility.Visible
        End If
        Await Mitem.WaitCloseAsync()
        If My.Settings.UseAnimations Then
            Switches_Library_Artists_Mitem.IsChecked = False
        Else
            Library_Artists_SPanel.Visibility = Visibility.Hidden
        End If
        If My.Settings.UseAnimations Then
            Switches_Library_Artists.IsChecked = False
            Library_Artists_SpanelView.Visibility = Visibility.Hidden
        Else
            Library_Artists_Overlay.Visibility = Visibility.Hidden
            Library_Artists_SpanelView.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Async Sub Library_Years_Lview_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Library_Years_Lview.SelectionChanged
        If My.Settings.UseAnimations Then
            Library_Years_SpanelView.Visibility = Visibility.Visible
            Switches_Library_Years.IsChecked = True
        Else
            Library_Years_SpanelView.Visibility = Visibility.Visible
            Library_Years_Overlay.Visibility = Visibility.Visible
        End If
        Dim Mitem As MusicItem = Nothing
        Dim cover = Utils.GetAlbumArt(libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs(0))
        If cover Is Nothing Then
            Mitem = New MusicItem(libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs, libraryYearsItems(Library_Years_Lview.SelectedIndex).Name, New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"), MainPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
        Else
            Mitem = New MusicItem(libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs, libraryYearsItems(Library_Years_Lview.SelectedIndex).Name, cover, MainPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
        End If
        libraryYearsLVItems.Clear()
        For i As Integer = 0 To libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs.Count - 1
            Try
                Dim info = Utils.GetSongInfo(libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs(i))
                libraryYearsLVItems.Add(New PlaylistItem(i + 1, info(1), info(0), info(2), info(3), info(4), Player.StreamTypes.Local, libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs(i), Utils.GetAlbumArt(libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs(i))))
            Catch ex As Exception
                libraryYearsLVItems.Add(New PlaylistItem(i + 1, IO.Path.GetFileNameWithoutExtension(libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs(i)), "Not available", "Not available", 0, 0, Player.StreamTypes.Local, libraryYearsItems(Library_Years_Lview.SelectedIndex).Songs(i), Nothing))
            End Try
        Next
        Library_Years_SPanel.Children.Clear()
        Library_Years_SPanel.Children.Add(Mitem)
        Library_Years_SPanel.Children.Add(Library_YearsLV)
        If My.Settings.UseAnimations Then
            Switches_Library_Years_Mitem.IsChecked = True
        Else
            Library_Years_SPanel.Visibility = Visibility.Visible
        End If
        Await Mitem.WaitCloseAsync()
        If My.Settings.UseAnimations Then
            Switches_Library_Years_Mitem.IsChecked = False
        Else
            Library_Years_SPanel.Visibility = Visibility.Hidden
        End If
        If My.Settings.UseAnimations Then
            Switches_Library_Years.IsChecked = False
            Library_Years_SpanelView.Visibility = Visibility.Hidden
        Else
            Library_Years_Overlay.Visibility = Visibility.Hidden
            Library_Years_SpanelView.Visibility = Visibility.Hidden
        End If
    End Sub
    Private Async Sub Library_Favourites_Lview_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Library_Favourites_Lview.SelectionChanged
        Try
            If My.Settings.UseAnimations Then
                Switches_Library_Favourites.IsChecked = True
            Else
                Library_Favourites_Overlay.Visibility = Visibility.Visible
            End If
            Dim Cover = Utils.GetAlbumArt(libraryFavouritesItems(Library_Favourites_Lview.SelectedIndex).Source)
            Dim Mitem As MusicItem = Nothing
            If Cover IsNot Nothing Then
                Mitem = New MusicItem(libraryFavouritesItems(Library_Favourites_Lview.SelectedIndex).Source, libraryFavouritesItems(Library_Favourites_Lview.SelectedIndex).Title, libraryFavouritesItems(Library_Favourites_Lview.SelectedIndex).Artist, Cover, MainPlayer, PreviewPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
            Else
                Mitem = New MusicItem(libraryFavouritesItems(Library_Favourites_Lview.SelectedIndex).Source, IO.Path.GetFileNameWithoutExtension(libraryFavouritesItems(Library_Favourites_Lview.SelectedIndex).Source), "Not available", New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png"), MainPlayer, PreviewPlayer, MainPlaylist) With {.Width = Double.NaN, .HorizontalAlignment = HorizontalAlignment.Center}
            End If
            Library_Favourites_SPanel.Children.Clear()
            Library_Favourites_SPanel.Children.Add(Mitem)
            If My.Settings.UseAnimations Then
                Switches_Library_Favourites_Mitem.IsChecked = True
            Else
                Library_Favourites_SPanel.Visibility = Visibility.Visible
            End If
            Await Mitem.WaitCloseAsync()
            If My.Settings.UseAnimations Then
                Switches_Library_Favourites_Mitem.IsChecked = False
            Else
                Library_Favourites_SPanel.Visibility = Visibility.Hidden
            End If
            If My.Settings.UseAnimations Then
                Switches_Library_Favourites.IsChecked = False
            Else
                Library_Favourites_Overlay.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub TitleBar_MiniPlayer_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_MiniPlayer.Click
        Me.Hide()
        My.Windows.MiniPlayer.Show()
    End Sub

    Private Async Sub MainLibrary_OnItemsChanged() Handles MainLibrary.OnItemsChanged
        Await Dispatcher.BeginInvoke(Async Sub()
                                         Gtracks = Await MainLibrary.GroupTracksAsync
                                     End Sub, System.Windows.Threading.DispatcherPriority.Background)
    End Sub

    Private Sub MainWindow_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effects = DragDropEffects.Link
        End If
    End Sub
    Private Sub MainWindow_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For Each file In files
            Try
                If IO.File.GetAttributes(file) = IO.FileAttributes.Directory Then
                    Select Case My.Settings.PlaylistDragDropAction
                        Case Utils.DragDropPlaylistBehaviour.AddToFirst
                            For Each _file In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(file, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                MainPlaylist.Insert(0, _file, Player.StreamTypes.Local)
                            Next
                        Case Utils.DragDropPlaylistBehaviour.AddToLast
                            For Each _file In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(file, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                MainPlaylist.Add(_file, Player.StreamTypes.Local, False)
                            Next
                        Case Utils.DragDropPlaylistBehaviour.AddToLastPlay
                            Dim Cidx = MainPlaylist.Count - 1
                            For Each _file In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(file, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                MainPlaylist.Add(_file, Player.StreamTypes.Local, False)
                            Next
                            MainPlayer.LoadSong(MainPlaylist.JumpTo(Cidx), MainPlaylist, False)
                        Case Utils.DragDropPlaylistBehaviour.AddToNext
                            For Each _file In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(file, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                MainPlaylist.Insert(MainPlaylist.Index + 1, _file, Player.StreamTypes.Local)
                            Next
                        Case Utils.DragDropPlaylistBehaviour.Replace
                            MainPlaylist.Clear()
                            For Each _file In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(file, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                                MainPlaylist.Add(_file, Player.StreamTypes.Local)
                            Next
                    End Select
                Else
                    If IO.File.Exists(file) Then
                        Select Case My.Settings.PlaylistDragDropAction
                            Case Utils.DragDropPlaylistBehaviour.AddToFirst
                                MainPlaylist.Insert(0, file, Player.StreamTypes.Local)
                            Case Utils.DragDropPlaylistBehaviour.AddToLast
                                MainPlaylist.Add(file, Player.StreamTypes.Local, False)
                            Case Utils.DragDropPlaylistBehaviour.AddToLastPlay
                                MainPlaylist.Add(file, Player.StreamTypes.Local, False)
                                MainPlayer.LoadSong(MainPlaylist.JumpTo(MainPlaylist.Count - 1), MainPlaylist, False)
                            Case Utils.DragDropPlaylistBehaviour.AddToNext
                                MainPlaylist.Insert(MainPlaylist.Index + 1, file, Player.StreamTypes.Local)
                            Case Utils.DragDropPlaylistBehaviour.Replace
                                MainPlaylist.Clear()
                                MainPlaylist.Add(file, Player.StreamTypes.Local)
                                MainPlayer.LoadSong(file, MainPlaylist, False)
                        End Select
                    End If
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Sub TitleBar_ABLoop_Set_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_ABLoop_Set.Click
        Dim A As Double
        Dim cA = Double.TryParse(TitleBar_ABLoop_A.Items(0).Text, A)
        Dim B As Double
        Dim cB = Double.TryParse(TitleBar_ABLoop_B.Items(0).Text, B)
        If cA AndAlso cB Then
            MainPlayer.ABLoop = New Player.ABLoopItem(A, B)
        End If
    End Sub

    Private Sub TitleBar_ABLoop_MouseEnter(sender As Object, e As MouseEventArgs) Handles TitleBar_ABLoop.MouseEnter
        PosDurSwitch = True
    End Sub

    Private Sub TitleBar_ABLoop_MouseLeave(sender As Object, e As MouseEventArgs) Handles TitleBar_ABLoop.MouseLeave
        PosDurSwitch = False
    End Sub

    Private Sub TitleBar_DSP_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_DSP.Click
        My.Windows.DSPPlugins.Owner = Me
        My.Windows.DSPPlugins.Show()
    End Sub

    Private Sub TitleBar_Playlist_CustomPlaylistManager_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Playlist_CustomPlaylistManager.Click
        My.Windows.PlaylistManager.Show()
    End Sub
    Dim WithEvents thumb As TabbedThumbnail = Nothing
    Public Sub SetThumb()
        thumb = TaskbarThumbnailManager.AddThumbnail(True, media_cover.Source, System.Drawing.SystemIcons.Error, "Debug", "Debug Tip")
        'TaskbarThumbnailManager.Bind(Home_NowPlaying, thumb)
        TaskbarThumbnailManager.Bind(CustomTaskBarThumb.Binding.FromVisualiser(MainPlayer, thumb, Player.Visualizers.SpectrumPeak, Drawing.Color.Black, Drawing.Color.Red, Drawing.Color.Empty, Drawing.Color.Black, 2, 1, 250, 2))
    End Sub
    Public Sub RemThumb()
        TaskbarThumbnailManager.RemoveThumbnail(0)
    End Sub

End Class
