Imports System.ComponentModel
Imports System.Timers
Imports Un4seen.Bass
Imports Un4seen.Bass.Misc
Imports Un4seen.Bass.AddOn.Fx
Imports Un4seen.Bass.AddOn.Sfx
Imports Un4seen.Bass.AddOn.Tags
Imports Un4seen.Bass.AddOn.WaDsp
Public Class Player
#Region "Events"
        Public Event PlayerStateChanged(State As State)
        Public Event VolumeChanged(NewVal As Single, IsMuted As Boolean)
    Public Event MediaLoaded(Title As String, Artist As String, Cover As System.Windows.Interop.InteropBitmap, Thumb As System.Windows.Interop.InteropBitmap, LyricsAvailable As Boolean, Lyrics As String)
    Public Event MediaEnded()
        Private CurrentMediaEndedCALLBACK As SYNCPROC
        Public Event OnFxChanged(FX As LinkHandles, State As Boolean)
        Public Event OnMediaError(ErrorCode As BASSError)
        Public Event OnRepeatChanged(NewType As RepeateBehaviour)
    Public Event OnShuffleChanged(NewType As RepeateBehaviour)
    Public Event OnDspAdded(DSP As DSPPlugin)
    Public Event OnDspRemoved(DSP As DSPPlugin)
#End Region
#Region "Properties"
    Private IsFirstStream As Boolean = True
    Private Owner As Window = Nothing
        Public Property PlayerState As State
        Public Property SourceURL As String
        Private Property Visualizer As Visuals
        Public Property Volume As Single = 1
        Public Property IsFXLoaded As Boolean = False
    Public Property IsSFXLoaded As Boolean = False
    Public Property IsDSPLoaded As Boolean = False
    Private Property IsMute As Boolean = False
        Public Property IsInitialized As Boolean = False
    Public Property IsInitializedReason As Exception = Nothing
    Public WithEvents LoadedDSP As New List(Of DSPPlugin)
    Public ToBeLoadedDSP As New List(Of String)
    Public Property Mute As Boolean
            Get
                Return IsMute
            End Get
            Set(value As Boolean)
                If value Then
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 0)
                    IsMute = True
                    RaiseEvent VolumeChanged(Volume, True)
                Else
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, Volume)
                    IsMute = False
                    RaiseEvent VolumeChanged(Volume, False)
                End If
            End Set
        End Property
        Private _RepeateType As RepeateBehaviour = RepeateBehaviour.NoRepeat
        Public Property RepeateType As RepeateBehaviour
            Get
                Return _RepeateType
            End Get
            Set(value As RepeateBehaviour)
                If value = RepeateBehaviour.RepeatOne Then
                    Bass.BASS_ChannelFlags(Stream, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP)
                Else
                    Bass.BASS_ChannelFlags(Stream, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP)
                End If
                _RepeateType = value
                RaiseEvent OnRepeatChanged(value)
            End Set
        End Property
        Private _ShuffleType As RepeateBehaviour = RepeateBehaviour.NoShuffle
        Public Property Shuffle As RepeateBehaviour
            Get
                Return _ShuffleType
            End Get
            Set(value As RepeateBehaviour)
                _ShuffleType = value
                RaiseEvent OnShuffleChanged(value)
            End Set
        End Property
        Private _Mono As Boolean = False
    Public Property Mono As Boolean
        Get
            Return _Mono
        End Get
        Set(value As Boolean)
            If value = True Then
                If CurrentMediaType = StreamTypes.Local Then
                    Dim pos = GetPosition()
                    StreamStop()
                    Stream = Bass.BASS_StreamCreateFile(SourceURL, 0, 0, BASSFlag.BASS_SAMPLE_MONO Or BASSFlag.BASS_STREAM_AUTOFREE)
                    SetPosition(pos)
                    StreamPlay()
                    pos = Nothing
                    _Mono = value
                End If
            Else
                If CurrentMediaType = StreamTypes.Local Then
                    Dim pos = GetPosition()
                    StreamStop()
                    Stream = Bass.BASS_StreamCreateFile(SourceURL, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT Or BASSFlag.BASS_STREAM_AUTOFREE)
                    SetPosition(pos)
                    StreamPlay()
                    pos = Nothing
                    _Mono = value
                End If
            End If
        End Set
    End Property
    Private Property _SkipSilencesInterval As Double = 500
    Private WithEvents _SkipSilencesTimer As New Timer With {.Interval = SkipSilencesInterval}
    Public Property SkipSilencesInterval As Double
        Get
            Return _SkipSilencesInterval
        End Get
        Set(value As Double)
            _SkipSilencesInterval = value
            _SkipSilencesTimer.Interval = value
        End Set
    End Property
    Private _SkipSilences As Boolean = False
    Public Property SkipSilences As Boolean
        Get
            Return _SkipSilences
        End Get
        Set(value As Boolean)
            _SkipSilences = value
            If value Then
                _SkipSilencesCuePoints = DetectSilence(SourceURL)
                _SkipSilencesTimer.Start()
            Else
                _SkipSilencesCuePoints = {0, 0}
                _SkipSilencesTimer.Stop()
            End If
        End Set
    End Property
    Private _SkipSilencesCuePoints As Double() = {0, 0}
    Private _ABLoop As ABLoopItem
    Private WithEvents _ABLoopTimer As New Timer With {.Interval = 100}
    Public WriteOnly Property ABLoop As ABLoopItem
        Set(value As ABLoopItem)
            _ABLoop = value
            SetABLoop()
        End Set
    End Property
#Region "Handles"
    Public Property Stream As Integer = 0
        Private Property BassFX_Handle As Integer = 0
        Private Property BassSFX_Handle As Integer = 0
    Private Property BassFlac_Handle As Integer = 0
    Private Property BassWaDSP_Handle As Integer = 0
#End Region
#Region "FX"
    Private Property _fxEQ As Integer() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Private Property _fxEQgains As Integer() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Public Property IsReverb As Boolean = False
        Public Property Reverb As BASS_DX8_REVERB = Nothing
        Private Property ReverbHandle As Integer
        Public Property IsLoudness As Boolean = False
        Private Property Loudness As BASS_BFX_DAMP
        Private Property LoudnessHandle As Integer
        Private Property IsBalance As Boolean = False
        Private Property Balance As Single
        Private Property IsSampleRate As Boolean = False
    Private Property SampleRate As Single
    Private Property StereoMix As BASS_BFX_MIX
    Private Property StereoMixHandle As Integer
    Public Property IsStereoMix As Boolean = False
    Private Property Rotate As BASS_BFX_ROTATE
    Private Property RotateHandle As Integer
    Public Property IsRotate As Boolean
#End Region
#Region "CurrentMedia"
    Public Property CurrentMediaTitle As String
        Public Property CurrentMediaArtist As String
        Public Property CurrentMediaCover As System.Windows.Interop.InteropBitmap
        Public Property CurrentMediaType As StreamTypes
#End Region
#End Region
#Region "Boot"
        Public Sub New(WndOwner As Window, OnMediaEndedCALLBACK As SYNCPROC)
            Try
            If Not Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero) Then
                If Bass.BASS_ErrorGetCode <> BASSError.BASS_ERROR_ALREADY Then
                    IsInitialized = False
                    MessageBox.Show("Error: " & Bass.BASS_ErrorGetCode.ToString, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            Else
                IsInitialized = True
                Owner = WndOwner
                CurrentMediaEndedCALLBACK = OnMediaEndedCALLBACK
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_DEV_DEFAULT, 1)
                BassFlac_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "bassflac.dll"))
                Try
                    BassFX_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "bass_fx.dll"))
                    If BassFx.BASS_FX_GetVersion() = 0 Then
                        IsFXLoaded = False
                    Else
                        IsFXLoaded = True
                    End If
                Catch ex As Exception
                    IsFXLoaded = False
                End Try
                Try
                    BassSFX_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "BASS_SFX.dll"))
                    If BassSfx.BASS_SFX_GetVersion() = 0 Then
                        IsSFXLoaded = False
                    Else
                        IsSFXLoaded = True
                    End If
                Catch ex As Exception
                    IsSFXLoaded = False
                End Try
                Try
                    BassWaDSP_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "bass_wadsp.dll"))
                    If BassWaDsp.BASS_WADSP_GetVersion = 0 Then
                        IsDSPLoaded = False
                    Else
                        Dim Hwnd = New Interop.WindowInteropHelper(Application.Current.MainWindow)
                        If BassWaDsp.BASS_WADSP_Init(Hwnd.Handle) Then
                            IsDSPLoaded = True
                        Else
                            IsDSPLoaded = False
                        End If
                    End If
                Catch ex As Exception
                    IsDSPLoaded = False
                End Try
                If My.Settings.FX_REVERB IsNot Nothing Then
                    Reverb = My.Settings.FX_REVERB
                    IsReverb = True
                End If
                If My.Settings.FX_DAMP IsNot Nothing Then
                    Loudness = My.Settings.FX_DAMP
                    IsLoudness = True
                End If
                If My.Settings.FX_BALANCE <> 0 Then
                    Balance = My.Settings.FX_BALANCE
                    IsBalance = True
                End If
                If My.Settings.FX_SAMPLERATE <> 0 Then
                    SampleRate = My.Settings.FX_SAMPLERATE
                    IsSampleRate = True
                End If
                If TryCast(My.Settings.FX_STEREOMIX, BASS_BFX_MIX) IsNot Nothing Then
                    StereoMix = CType(My.Settings.FX_STEREOMIX, BASS_BFX_MIX)
                    IsStereoMix = True
                End If
                Visualizer = New Visuals
                PlayerState = State.Undefined
                End If
        Catch ex As Exception
                IsInitialized = False
                IsInitializedReason = ex
            End Try
        End Sub
    Public Sub Dispose()
        BassFx.FreeMe()
        BassSfx.FreeMe()
        BassWaDsp.FreeMe()
        Bass.BASS_Free()
        If IsReverb Then
            My.Settings.FX_REVERB = Reverb
        Else
            My.Settings.FX_REVERB = Nothing
        End If
        If IsLoudness Then
            My.Settings.FX_DAMP = Loudness
        Else
            My.Settings.FX_DAMP = Nothing
        End If
        If IsBalance Then
            My.Settings.FX_BALANCE = Balance
        Else
            My.Settings.FX_BALANCE = 0
        End If
        If IsSampleRate Then
            My.Settings.FX_SAMPLERATE = SampleRate
        Else
            My.Settings.FX_SAMPLERATE = 0
        End If
        My.Settings.Save()
    End Sub
    Public Sub Init()
            If Not Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero) Then
                MessageBox.Show("Error: " & Bass.BASS_ErrorGetCode.ToString, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
            Else
                BassFlac_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "bassflac.dll"))
                Try
                    BassFX_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "bass_fx.dll"))
                    If BassFx.BASS_FX_GetVersion() = 0 Then
                        IsFXLoaded = False
                    Else
                        IsFXLoaded = True
                    End If
                Catch ex As Exception
                    IsFXLoaded = False
                End Try
                Try
                    BassSFX_Handle = Bass.BASS_PluginLoad(IO.Path.Combine(My.Application.Info.DirectoryPath, "BASS_SFX.dll"))
                    If BassSfx.BASS_SFX_GetVersion() = 0 Then
                        IsSFXLoaded = False
                    Else
                        IsSFXLoaded = True
                    End If
                Catch ex As Exception
                    IsSFXLoaded = False
                End Try
                Visualizer = New Visuals
                PlayerState = State.Undefined
            End If
        End Sub
#End Region
#Region "Navigation"
        Public Sub LoadSong(Loc As String, ByRef Playlist As Playlist, Optional UpdatePlaylist As Boolean = True, Optional RaiseEvents As Boolean = True, Optional UseURL As Boolean = False, Optional URL As String = Nothing, Optional OverrideCurrentMedia As Boolean = False, Optional OCMTitle As String = Nothing, Optional OCMArtist As String = Nothing, Optional OCMCover As System.Drawing.Bitmap = Nothing, Optional OCMYear As Integer = 0, Optional YTURL As String = Nothing, <Runtime.CompilerServices.CallerMemberName> ByVal Optional propertyName As String = Nothing, <Runtime.CompilerServices.CallerLineNumber> ByVal Optional propertyline As String = Nothing)
        StreamStop()
        Bass.BASS_StreamFree(Stream)
        If RaiseEvents = True Then
                RaiseEvent PlayerStateChanged(State.Stopped)
            End If
            PlayerState = State.Stopped
            If Not UseURL Then
                If _RepeateType <> RepeateBehaviour.RepeatOne Then
                    If Mono Then
                        Stream = Bass.BASS_StreamCreateFile(Loc, 0, 0, BASSFlag.BASS_SAMPLE_MONO Or BASSFlag.BASS_STREAM_AUTOFREE)
                    Else
                    Stream = Bass.BASS_StreamCreateFile(Loc, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT Or BASSFlag.BASS_STREAM_AUTOFREE)
                End If
                Else
                    If Mono Then
                        Stream = Bass.BASS_StreamCreateFile(Loc, 0, 0, BASSFlag.BASS_SAMPLE_MONO Or BASSFlag.BASS_STREAM_AUTOFREE Or BASSFlag.BASS_SAMPLE_LOOP)
                    Else
                    Stream = Bass.BASS_StreamCreateFile(Loc, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT Or BASSFlag.BASS_STREAM_AUTOFREE Or BASSFlag.BASS_SAMPLE_LOOP)
                End If
                End If
            If Stream <> 0 Then
                TryCast(Application.Current.MainWindow, MainWindow).AddToJlist(Loc)
                Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_END, 0, CurrentMediaEndedCALLBACK, IntPtr.Zero)
                SetVolume(Volume)
                SourceURL = Loc
                SetFx()
                For Each dsp In LoadedDSP
                    dsp.HDSP = BassWaDsp.BASS_WADSP_ChannelSetDSP(dsp.Handle, Stream, 1)
                Next
                Dim info = Utils.GetSongInfo(Loc)
                CurrentMediaType = StreamTypes.Local
                CurrentMediaTitle = info(1)
                CurrentMediaArtist = info(0)
                Dim Cover = Utils.GetAlbumArt(SourceURL)
                CurrentMediaCover = Utils.ImageSourceFromBitmap(Cover)
                PlayerState = State.MediaLoaded
                If RaiseEvents = True Then
                    If String.IsNullOrEmpty(info(6)) Then
                        RaiseEvent MediaLoaded(CurrentMediaTitle, CurrentMediaArtist, CurrentMediaCover, Utils.ImageSourceFromBitmap(Cover, True, 50, 50), False, Nothing)
                    Else
                        RaiseEvent MediaLoaded(CurrentMediaTitle, CurrentMediaArtist, CurrentMediaCover, Utils.ImageSourceFromBitmap(Cover, True, 50, 50), True, info(6))
                    End If
                    RaiseEvent PlayerStateChanged(State.MediaLoaded)
                End If
                info = Nothing
                Cover = Nothing
                If UpdatePlaylist = True Then
                    Playlist.Add(Loc, StreamTypes.Local)
                End If
                If SkipSilences Then
                    _SkipSilencesCuePoints = DetectSilence(SourceURL)
                    SetPosition(_SkipSilencesCuePoints(0))
                    _SkipSilencesTimer.Start()
                End If
            Else
                    RaiseEvent OnMediaError(Bass.BASS_ErrorGetCode)
                End If
            Else
                If URL IsNot Nothing Then
                    If _RepeateType <> RepeateBehaviour.RepeatOne Then
                        Stream = Bass.BASS_StreamCreateURL(URL, 0, BASSFlag.BASS_SAMPLE_FLOAT Or BASSFlag.BASS_STREAM_AUTOFREE Or BASSFlag.BASS_STREAM_STATUS, Nothing, Nothing)
                    Else
                    Stream = Bass.BASS_StreamCreateURL(URL, 0, BASSFlag.BASS_SAMPLE_FLOAT Or BASSFlag.BASS_STREAM_AUTOFREE Or BASSFlag.BASS_STREAM_STATUS Or BASSFlag.BASS_SAMPLE_LOOP, Nothing, Nothing)
                End If
                    If Stream <> 0 Then
                        Bass.BASS_ChannelSetSync(Stream, BASSSync.BASS_SYNC_END, 0, CurrentMediaEndedCALLBACK, IntPtr.Zero)
                        SetVolume(Volume)
                        SetFx()
                        If OverrideCurrentMedia = False Then
                            SourceURL = URL
                            CurrentMediaType = StreamTypes.URL
                            CurrentMediaTitle = "Not available"
                            CurrentMediaArtist = "Not available"
                            CurrentMediaCover = Nothing
                        Else
                            If YTURL <> Nothing Then
                                CurrentMediaType = StreamTypes.Youtube
                                CurrentMediaTitle = OCMTitle
                                CurrentMediaArtist = OCMArtist
                                CurrentMediaCover = Utils.ImageSourceFromBitmap(OCMCover)
                            Else
                                SourceURL = URL
                                CurrentMediaType = StreamTypes.URL
                                CurrentMediaTitle = OCMTitle
                                CurrentMediaArtist = OCMArtist
                                CurrentMediaCover = Utils.ImageSourceFromBitmap(OCMCover)
                            End If
                        End If
                        PlayerState = State.MediaLoaded
                        If RaiseEvents = True Then
                            If Not OverrideCurrentMedia = False Then
                            RaiseEvent MediaLoaded(CurrentMediaTitle, CurrentMediaArtist, CurrentMediaCover, Utils.ImageSourceFromBitmap(OCMCover, True, 50, 50), False, Nothing)
                        Else
                            RaiseEvent MediaLoaded(CurrentMediaTitle, CurrentMediaArtist, CurrentMediaCover, Nothing, False, Nothing)
                        End If
                            RaiseEvent PlayerStateChanged(State.MediaLoaded)
                        End If
                        If UpdatePlaylist = True Then
                            If OverrideCurrentMedia Then
                                If YTURL <> Nothing Then
                                    Playlist.Add(Nothing, StreamTypes.Youtube, True, True, YTURL, True, OCMTitle, OCMArtist, OCMYear)
                                Else
                                    Playlist.Add(Nothing, StreamTypes.URL, True, True, URL, True, OCMTitle, OCMArtist, OCMYear, OCMCover)
                                End If
                            Else
                                Playlist.Add(Nothing, StreamTypes.URL, True, True, URL, True, "Not Available", "Not Available", 0)
                            End If
                        End If
                    Else
                        RaiseEvent OnMediaError(Bass.BASS_ErrorGetCode)
                    End If
                Else
                    Exit Sub
                End If
            End If
        End Sub
        Public Async Function LoadStreamAsync(URL As String, Type As StreamTypes, Playlist As Playlist, Optional YTquery As String = Nothing, Optional UpdatePlaylist As Boolean = True) As Task(Of Boolean)
            Select Case Type
                Case StreamTypes.URL
                    LoadSong(Nothing, Playlist, UpdatePlaylist, True, True, URL)
                Case StreamTypes.Youtube
                If URL IsNot Nothing Then
                    Dim Yt As New YoutubeExplode.YoutubeClient
                    Dim Video = Await Yt.Videos.GetAsync(URL)
                    Dim CoverData As Byte() = Nothing
                    Using WC As New System.Net.WebClient
                        CoverData = WC.DownloadData(Video.Thumbnails.MediumResUrl)
                    End Using
                    Dim YTTrackManifest = Await Yt.Videos.Streams.GetManifestAsync(Video.Id)
                    Dim YTTrackStream = YTTrackManifest.GetMuxed()
                    SourceURL = Video.Url
                    LoadSong(Nothing, Playlist, UpdatePlaylist, True, True, YTTrackStream(0).Url, True, Video.Title, Video.Author, System.Drawing.Image.FromStream(New IO.MemoryStream(CoverData)))
                    Yt = Nothing
                    Video = Nothing
                    CoverData = Nothing
                    YTTrackManifest = Nothing
                    YTTrackStream = Nothing
                    Return Await Task.FromResult(True)
                Else
                    Dim Yt As New YoutubeExplode.YoutubeClient
                    Dim VideoId = Yt.Search.GetVideosAsync(YTquery, 1, 1).GetAsyncEnumerator
                    Await VideoId.MoveNextAsync()
                    Dim Video = VideoId.Current
                    Do While MessageBox.Show(My.Windows.MainWindow, "Do you want to play: " & vbCrLf & Video.Title & "By: " & Video.Author, "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No
                        Dim z As Boolean = Await VideoId.MoveNextAsync()
                        If z Then
                            If Video.Title = VideoId.Current.Title AndAlso Video.Author = VideoId.Current.Author Then
                                Exit Function
                            Else
                                Video = VideoId.Current
                            End If
                        Else
                            Exit Do
                        End If
                    Loop
                    Dim CoverData As Byte() = Nothing
                    Using WC As New System.Net.WebClient
                        CoverData = WC.DownloadData(Video.Thumbnails.MediumResUrl)
                    End Using
                    Dim YTTrackManifest = Await Yt.Videos.Streams.GetManifestAsync(Video.Id)
                    Dim YTTrackStream = YTTrackManifest.GetMuxed()
                    SourceURL = Video.Url
                    LoadSong(Nothing, Playlist, UpdatePlaylist, True, True, YTTrackStream(0).Url, True, Video.Title, Video.Author, System.Drawing.Image.FromStream(New IO.MemoryStream(CoverData)), 0, Video.Url)
                    Yt = Nothing
                    VideoId = Nothing
                    Video = Nothing
                    CoverData = Nothing
                    YTTrackManifest = Nothing
                    YTTrackStream = Nothing
                    Return Await Task.FromResult(True)
                    End If
                Case StreamTypes.Soundcloud
            End Select
        End Function
        Public Async Sub LoadStream(URL As String, Type As StreamTypes, Playlist As Playlist, Optional YTquery As String = Nothing, Optional UpdatePlaylist As Boolean = True)
            Select Case Type
                Case StreamTypes.URL
                    LoadSong(Nothing, Playlist, UpdatePlaylist, True, True, URL)
                Case StreamTypes.Youtube
                    If URL IsNot Nothing Then
                        Dim Yt As New YoutubeExplode.YoutubeClient
                        Dim Video = Await Yt.Videos.GetAsync(URL)
                        Dim CoverData As Byte() = Nothing
                        Using WC As New System.Net.WebClient
                            CoverData = WC.DownloadData(Video.Thumbnails.MediumResUrl)
                        End Using
                        Dim YTTrackManifest = Await Yt.Videos.Streams.GetManifestAsync(Video.Id)
                        Dim YTTrackStream = YTTrackManifest.GetMuxed()
                        SourceURL = Video.Url
                        LoadSong(Nothing, Playlist, UpdatePlaylist, True, True, YTTrackStream(0).Url, True, Video.Title, Video.Author, System.Drawing.Image.FromStream(New IO.MemoryStream(CoverData)))
                        Yt = Nothing
                        Video = Nothing
                        CoverData = Nothing
                        YTTrackManifest = Nothing
                        YTTrackStream = Nothing
                    Else
                        Dim Yt As New YoutubeExplode.YoutubeClient
                        Dim VideoId = Yt.Search.GetVideosAsync(YTquery, 1, 1).GetAsyncEnumerator
                        Await VideoId.MoveNextAsync()
                        Dim Video = VideoId.Current
                        Do While MessageBox.Show(My.Windows.MainWindow, "Do you want to play: " & vbCrLf & Video.Title & "By: " & Video.Author, "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.No
                            Dim z As Boolean = Await VideoId.MoveNextAsync()
                            If Video.Title = VideoId.Current.Title AndAlso Video.Author = VideoId.Current.Author Then
                                Exit Sub
                            Else
                                Video = VideoId.Current
                            End If
                        Loop
                        Dim CoverData As Byte() = Nothing
                        Using WC As New System.Net.WebClient
                            CoverData = WC.DownloadData(Video.Thumbnails.MediumResUrl)
                        End Using
                        Dim YTTrackManifest = Await Yt.Videos.Streams.GetManifestAsync(Video.Id)
                        Dim YTTrackStream = YTTrackManifest.GetMuxed()
                        SourceURL = Video.Url
                    LoadSong(Nothing, Playlist, UpdatePlaylist, True, True, YTTrackStream(0).Url, True, Video.Title, Video.Author, System.Drawing.Image.FromStream(New IO.MemoryStream(CoverData)), 0, Video.Url)
                    Yt = Nothing
                        VideoId = Nothing
                        Video = Nothing
                        CoverData = Nothing
                        YTTrackManifest = Nothing
                        YTTrackStream = Nothing
                    End If
                Case StreamTypes.Soundcloud
            End Select
        End Sub
        Public Sub StreamPlay(Optional RaiseEvents As Boolean = True)
            If Bass.BASS_ChannelPlay(Stream, False) Then
                If RaiseEvents Then
                    RaiseEvent PlayerStateChanged(State.Playing)
                    PlayerState = State.Playing
                End If
            End If
        End Sub
        Public Sub StreamPause(Optional RaiseEvents As Boolean = True)
            If Bass.BASS_ChannelPause(Stream) Then
                If RaiseEvents Then
                    RaiseEvent PlayerStateChanged(State.Paused)
                    PlayerState = State.Paused
                End If
            End If
        End Sub
        Public Sub StreamStop()
            If Bass.BASS_ChannelStop(Stream) Then
                RaiseEvent PlayerStateChanged(State.Stopped)
                PlayerState = State.Stopped
            Stream = 0
        End If
        End Sub
#End Region
#Region "Settings"
    Public Sub SetVolume(Vol As Single, Optional RaiseEvents As Boolean = True)
        Select Case Vol
            Case < 0
                If Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 0) Then
                    Volume = 0
                    If RaiseEvents = True Then
                        RaiseEvent VolumeChanged(0, False)
                    End If
                End If
            Case > 1
                If Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 1) Then
                    Volume = 1
                    If RaiseEvents = True Then
                        RaiseEvent VolumeChanged(1, False)
                    End If
                End If
            Case Else
                If Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, Vol) Then
                    Volume = Vol
                    If RaiseEvents = True Then
                        RaiseEvent VolumeChanged(Vol, False)
                    End If
                End If
        End Select
    End Sub
    Public Function GetVolume() As Single
            Dim Vol As Single = 0
            If Bass.BASS_ChannelGetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, Vol) Then
                Volume = Vol
                Return Vol
            End If
            Vol = Nothing
        End Function
        Public Sub SetPosition(Seconds As Double)
            Bass.BASS_ChannelSetPosition(Stream, Seconds)
        End Sub
        Public Function GetPosition() As Long
            Try
                Return Bass.BASS_ChannelBytes2Seconds(Stream, Bass.BASS_ChannelGetPosition(Stream, BASSMode.BASS_POS_BYTES))
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Function GetLength() As Long
            Try
                Return Bass.BASS_ChannelBytes2Seconds(Stream, Bass.BASS_ChannelGetLength(Stream, BASSMode.BASS_POS_BYTES))
            Catch ex As Exception
                Return 0
            End Try
        End Function
        Public Sub UpdateEQ(band As Integer, gain As Single, Optional DisableForAll As Boolean = False)
            If DisableForAll = False Then
                Dim eq As New BASS_DX8_PARAMEQ()
                If Bass.BASS_FXGetParameters(_fxEQ(band), eq) Then
                    eq.fGain = gain
                    Bass.BASS_FXSetParameters(_fxEQ(band), eq)
                    _fxEQgains(band) = gain
                    RaiseEvent OnFxChanged(LinkHandles.EQ, True)
                End If
            Else
                SetEq(True)
                RaiseEvent OnFxChanged(LinkHandles.EQ, False)
            End If
        End Sub
        Private Sub SetEq(Optional Reset As Boolean = False)
            If Reset = False Then
            ' 10-band EQ
            Dim eq As New BASS_DX8_PARAMEQ()
            _fxEQ(0) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(1) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(2) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(3) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(4) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(5) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(6) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(7) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(8) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            _fxEQ(9) = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0)
            eq.fBandwidth = 18.0F
            eq.fCenter = 80.0F
            eq.fGain = _fxEQgains(0)
            Bass.BASS_FXSetParameters(_fxEQ(0), eq)
            eq.fCenter = 100.0F
            eq.fGain = _fxEQgains(1)
            Bass.BASS_FXSetParameters(_fxEQ(1), eq)
            eq.fCenter = 125.0F
            eq.fGain = _fxEQgains(2)
            Bass.BASS_FXSetParameters(_fxEQ(2), eq)
            eq.fCenter = 250.0F
            eq.fGain = _fxEQgains(3)
            Bass.BASS_FXSetParameters(_fxEQ(3), eq)
            eq.fCenter = 500.0F
            eq.fGain = _fxEQgains(4)
            Bass.BASS_FXSetParameters(_fxEQ(4), eq)
            eq.fCenter = 1000.0F
            eq.fGain = _fxEQgains(5)
            Bass.BASS_FXSetParameters(_fxEQ(5), eq)
            eq.fCenter = 2000.0F
            eq.fGain = _fxEQgains(6)
            Bass.BASS_FXSetParameters(_fxEQ(6), eq)
            eq.fCenter = 4000.0F
            eq.fGain = _fxEQgains(7)
            Bass.BASS_FXSetParameters(_fxEQ(7), eq)
            eq.fCenter = 8000.0F
            eq.fGain = _fxEQgains(8)
            Bass.BASS_FXSetParameters(_fxEQ(8), eq)
            eq.fCenter = 16000.0F
            eq.fGain = _fxEQgains(9)
            Bass.BASS_FXSetParameters(_fxEQ(9), eq)
        Else
                _fxEQgains = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                ' 10-band EQ
                Dim eq As New BASS_DX8_PARAMEQ()
                eq.fBandwidth = 18.0F
                eq.fCenter = 80.0F
                eq.fGain = _fxEQgains(0)
                Bass.BASS_FXSetParameters(_fxEQ(0), eq)
                eq.fCenter = 100.0F
                eq.fGain = _fxEQgains(1)
                Bass.BASS_FXSetParameters(_fxEQ(1), eq)
                eq.fCenter = 125.0F
                eq.fGain = _fxEQgains(2)
                Bass.BASS_FXSetParameters(_fxEQ(2), eq)
                eq.fCenter = 250.0F
                eq.fGain = _fxEQgains(3)
                Bass.BASS_FXSetParameters(_fxEQ(3), eq)
                eq.fCenter = 500.0F
                eq.fGain = _fxEQgains(4)
                Bass.BASS_FXSetParameters(_fxEQ(4), eq)
                eq.fCenter = 1000.0F
                eq.fGain = _fxEQgains(5)
                Bass.BASS_FXSetParameters(_fxEQ(5), eq)
                eq.fCenter = 2000.0F
                eq.fGain = _fxEQgains(6)
                Bass.BASS_FXSetParameters(_fxEQ(6), eq)
                eq.fCenter = 4000.0F
                eq.fGain = _fxEQgains(7)
                Bass.BASS_FXSetParameters(_fxEQ(7), eq)
                eq.fCenter = 8000.0F
                eq.fGain = _fxEQgains(8)
                Bass.BASS_FXSetParameters(_fxEQ(8), eq)
                eq.fCenter = 16000.0F
                eq.fGain = _fxEQgains(9)
                Bass.BASS_FXSetParameters(_fxEQ(9), eq)
            End If
        End Sub
        Public Sub UpdateReverb(InGain As Single, ReverbMix As Single, ReverbTime As Single, HighFreqRTRatio As Single, UpInGain As Boolean, UpReverbMix As Boolean, UpReverbTime As Boolean, UpHighFreqRTRatio As Boolean, Optional DisableFX As Boolean = False, Optional EnableFX As Boolean = False)
            If EnableFX = True AndAlso IsReverb = False Then
                Dim Rvrb As New BASS_DX8_REVERB
                Rvrb.Preset_Default()
                ReverbHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_REVERB, 1)
                If ReverbHandle <> 0 Then
                    IsReverb = True
                    Reverb = Rvrb
                    RaiseEvent OnFxChanged(LinkHandles.Reverb, True)
                End If
                Exit Sub
            End If
            If DisableFX = True AndAlso IsReverb = True Then
                If Bass.BASS_ChannelRemoveFX(Stream, ReverbHandle) Then
                    ReverbHandle = 0
                    IsReverb = False
                    RaiseEvent OnFxChanged(LinkHandles.Reverb, False)
                End If
            End If
            If IsReverb = True Then
                Dim Rvrb As New BASS_DX8_REVERB
                Bass.BASS_FXGetParameters(ReverbHandle, Rvrb)
                If UpInGain Then
                    Rvrb.fInGain = InGain
                End If
                If UpReverbMix Then
                    Rvrb.fReverbMix = ReverbMix
                End If
                If UpReverbTime Then
                    Rvrb.fReverbTime = ReverbTime
                End If
                If UpHighFreqRTRatio Then
                    Rvrb.fHighFreqRTRatio = HighFreqRTRatio
                End If
                If Bass.BASS_FXSetParameters(ReverbHandle, Rvrb) Then
                    Reverb = Rvrb
                End If
            End If
        End Sub
        Private Sub SetReverb()
            If Reverb IsNot Nothing Then
                ReverbHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_DX8_REVERB, 1)
                If ReverbHandle <> 0 Then
                    Bass.BASS_FXSetParameters(ReverbHandle, Reverb)
                End If
            End If
        End Sub
        Public Sub UpdateLoudness(Preset As Integer, Optional EnableFX As Boolean = False, Optional DisableFX As Boolean = False)
            If EnableFX = True AndAlso IsLoudness = False Then
                Dim Ldns As New BASS_BFX_DAMP
                Ldns.Preset_Soft()
                LoudnessHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_BFX_DAMP, 1)
                If LoudnessHandle <> 0 Then
                    IsLoudness = True
                    Loudness = Ldns
                    RaiseEvent OnFxChanged(LinkHandles.Loudness, True)
                End If
                Exit Sub
            End If
            If DisableFX = True AndAlso IsLoudness = True Then
                If Bass.BASS_ChannelRemoveFX(Stream, LoudnessHandle) Then
                    LoudnessHandle = 0
                    IsLoudness = False
                    RaiseEvent OnFxChanged(LinkHandles.Loudness, False)
                End If
            End If
            If IsLoudness = True Then
                Dim Ldns As New BASS_BFX_DAMP
                Select Case Preset
                    Case 1
                        Ldns.Preset_Soft()
                    Case 2
                        Ldns.Preset_Medium()
                    Case 3
                        Ldns.Preset_Hard()
                End Select
                If Bass.BASS_FXSetParameters(LoudnessHandle, Ldns) Then
                    Loudness = Ldns
                End If
            End If
        End Sub
        Private Sub SetLoudness()
            If Loudness IsNot Nothing Then
                LoudnessHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_BFX_DAMP, 1)
                If LoudnessHandle <> 0 Then
                    Bass.BASS_FXSetParameters(LoudnessHandle, Loudness)
                End If
            End If
        End Sub
    Private Sub SetFx()
        SetEq() 'Equalizer
        If IsReverb Then
            SetReverb() 'Reverb
            RaiseEvent OnFxChanged(LinkHandles.Reverb, True)
        Else
            RaiseEvent OnFxChanged(LinkHandles.Reverb, False)
        End If
        If IsLoudness Then 'Loudness
            SetLoudness()
            RaiseEvent OnFxChanged(LinkHandles.Loudness, True)
        Else
            RaiseEvent OnFxChanged(LinkHandles.Loudness, False)
        End If
        If IsBalance Then 'Balance
            SetBalance(Balance)
            RaiseEvent OnFxChanged(LinkHandles.Balance, True)
        Else
            RaiseEvent OnFxChanged(LinkHandles.Balance, False)
        End If
        If IsSampleRate Then 'Sample Rate
            SetSampleRate(SampleRate)
            RaiseEvent OnFxChanged(LinkHandles.SampleRate, True)
        Else
            RaiseEvent OnFxChanged(LinkHandles.SampleRate, False)
        End If
        If IsStereoMix Then 'StereoMix
            SetStereoMix(True)
            RaiseEvent OnFxChanged(LinkHandles.StereoMix, True)
        Else
            RaiseEvent OnFxChanged(LinkHandles.StereoMix, False)
        End If
        If IsFirstStream = False Then
            For Each DSP In LoadedDSP
                DSP.HDSP = BassWaDsp.BASS_WADSP_ChannelSetDSP(DSP.Handle, Stream, 0)
            Next
        Else
            For Each DSP In ToBeLoadedDSP
                ImportDSP(DSP)
            Next
            ToBeLoadedDSP.Clear()
            IsFirstStream = False
        End If
    End Sub
    Public Function GetHandle(FX As LinkHandles) As String
            Select Case FX
                Case LinkHandles.Stream
                    Return Stream
                Case LinkHandles.EQ
                    Return String.Join(vbCrLf, _fxEQ)
                Case LinkHandles.Reverb
                    Return ReverbHandle
                Case LinkHandles.Loudness
                    Return LoudnessHandle
                Case LinkHandles.FX
                    Return BassFx.BASS_FX_GetVersion
                Case LinkHandles.SFX
                    Return BassSfx.BASS_SFX_GetVersion
                Case LinkHandles.BASS
                    Return Bass.BASS_GetVersion
            End Select
            Return "Null"
        End Function
        Public Function GetTags() As TAG_INFO
            If CurrentMediaType = StreamTypes.Local Then
                Return BassTags.BASS_TAG_GetFromFile(SourceURL)
            ElseIf CurrentMediaType = StreamTypes.URL Then
                Dim tags As New TAG_INFO
                If BassTags.BASS_TAG_GetFromURL(Stream, tags) Then
                    Return tags
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function
        Public Sub SetBalance(_Balance As Single)
            Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_PAN, _Balance)
            If _Balance = 0 Then
                IsBalance = False
                Balance = 0
                RaiseEvent OnFxChanged(LinkHandles.Balance, False)
            Else
                IsBalance = True
                Balance = _Balance
                RaiseEvent OnFxChanged(LinkHandles.Balance, True)
            End If
        End Sub
        Public Function GetBalance() As Single
            Return Balance
        End Function
        Public Function GetPeak() As Utils.PeakItem
            Dim Level = Bass.BASS_ChannelGetLevel(Stream)
            Dim LeftLevel = Un4seen.Bass.Utils.LowWord32(Level) / 1000
            Dim RightLevel = Un4seen.Bass.Utils.HighWord32(Level) / 1000
            Return New Utils.PeakItem((LeftLevel + RightLevel) / 2, LeftLevel, RightLevel)
        End Function
    Public Sub SetSampleRate(Rate As Single)
        Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_FREQ, Rate)
        If Rate = 0 Then
            IsSampleRate = False
            SampleRate = 0
            RaiseEvent OnFxChanged(LinkHandles.SampleRate, False)
        Else
            IsSampleRate = True
            SampleRate = Rate
            RaiseEvent OnFxChanged(LinkHandles.SampleRate, True)
        End If
    End Sub
    Public Sub SetStereoMix(ReverseStereo As Boolean)
        If ReverseStereo = True Then
            If IsStereoMix Then
                If Bass.BASS_ChannelRemoveFX(Stream, StereoMixHandle) Then
                    StereoMixHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_BFX_MIX, 1)
                    If StereoMixHandle <> 0 Then
                        Dim MIX As New BASS_BFX_MIX({BASSFXChan.BASS_BFX_CHAN1, BASSFXChan.BASS_BFX_CHAN2})
                        MIX.lChannel(0) = BASSFXChan.BASS_BFX_CHAN2
                        MIX.lChannel(1) = BASSFXChan.BASS_BFX_CHAN1
                        If Bass.BASS_FXSetParameters(StereoMixHandle, MIX) Then
                            StereoMix = MIX
                            IsStereoMix = True
                            RaiseEvent OnFxChanged(LinkHandles.StereoMix, True)
                        End If
                    End If
                End If
            Else
                StereoMixHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_BFX_MIX, 1)
                If StereoMixHandle <> 0 Then
                    Dim MIX As New BASS_BFX_MIX({BASSFXChan.BASS_BFX_CHAN1, BASSFXChan.BASS_BFX_CHAN2})
                    MIX.lChannel(0) = BASSFXChan.BASS_BFX_CHAN2
                    MIX.lChannel(1) = BASSFXChan.BASS_BFX_CHAN1
                    If Bass.BASS_FXSetParameters(StereoMixHandle, MIX) Then
                        StereoMix = MIX
                        IsStereoMix = True
                        RaiseEvent OnFxChanged(LinkHandles.StereoMix, True)
                    End If
                End If
            End If
        Else
            If Bass.BASS_ChannelRemoveFX(Stream, StereoMixHandle) Then
                StereoMix = Nothing
                IsStereoMix = False
                RaiseEvent OnFxChanged(LinkHandles.StereoMix, False)
            End If
        End If
    End Sub
    Public Sub SetCustomState(state As State)
        RaiseEvent PlayerStateChanged(state)
    End Sub
    Public Sub SetRotate(Enable As Boolean)
        If Enable Then
            RotateHandle = Bass.BASS_ChannelSetFX(Stream, BASSFXType.BASS_FX_BFX_ROTATE, 1)
            If RotateHandle <> 0 Then
                Rotate = New BASS_BFX_ROTATE(1, BASSFXChan.BASS_BFX_CHANALL)
                Bass.BASS_FXSetParameters(RotateHandle, Rotate)
                IsRotate = True
                RaiseEvent OnFxChanged(LinkHandles.Rotate, True)
            Else
                Rotate = Nothing
                IsRotate = False
                RaiseEvent OnFxChanged(LinkHandles.Rotate, False)
            End If
        Else
            If Bass.BASS_ChannelRemoveFX(Stream, RotateHandle) Then
                Rotate = Nothing
                IsRotate = False
                RaiseEvent OnFxChanged(LinkHandles.Rotate, False)
            End If
        End If
    End Sub
    Public Sub UpdateRotate(Preset As RotatePreset)
        If IsRotate Then
            Select Case Preset
                Case RotatePreset.Slow
                    Rotate.fRate = 0.05
                    Bass.BASS_FXSetParameters(RotateHandle, Rotate)
                Case RotatePreset.Med
                    Rotate.fRate = 0.1
                    Bass.BASS_FXSetParameters(RotateHandle, Rotate)
                Case RotatePreset.Fast
                    Rotate.fRate = 0.3
                    Bass.BASS_FXSetParameters(RotateHandle, Rotate)
            End Select
        End If
    End Sub
    Public Function DetectSilence(Filename As String) As Double()
        Dim cueInPos As Double
        Dim cueOutPos As Double
        Un4seen.Bass.Utils.DetectCuePoints(Filename, 10, cueInPos, cueOutPos, -25, -42, 0)
        Dim ComInfo As Double() = {cueInPos, cueOutPos}
        Return ComInfo
    End Function
    Private Sub _SkipSilencesTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _SkipSilencesTimer.Elapsed
        If SkipSilences Then
            If GetPosition() >= _SkipSilencesCuePoints(1) Then
                _SkipSilencesTimer.Stop()
                CurrentMediaEndedCALLBACK(Stream, 0, Nothing, IntPtr.Zero)
            End If
        Else
            _SkipSilencesTimer.Stop()
        End If
    End Sub
#Region "Sound Fading"
    Dim WithEvents FaderUpTimer As New Timer With {.Interval = 1}
        Dim WithEvents FaderDownTimer As New Timer With {.Interval = 1}
        Dim WithEvents PartialFaderDownTimer As New Timer With {.Interval = 1}
        Dim PartialDownTo As Single = 0
        Dim FadingVol As Boolean = False
    Public ReadOnly Property IsFadingVol As Boolean
        Get
            Return FadingVol
        End Get
    End Property
    <Obsolete("FadeVolume Is Obsolete Use FadeVol for better performance")>
    Public Async Function FadeVolume(Up As Boolean, Down As Boolean, Optional PartialFade As Boolean = False, Optional PartialTo As Single = 0) As Task(Of Boolean)
            If PartialFade = True Then
                FadingVol = True
                PartialDownTo = PartialTo
                FaderUpTimer.Stop()
                FaderDownTimer.Stop()
                PartialFaderDownTimer.Start()
                Await Task.Delay(650)
                Return Await Task.FromResult(True)
            Else
                If Up Then
                    FadingVol = True
                    PartialFaderDownTimer.Stop()
                    FaderDownTimer.Stop()
                    FaderUpTimer.Start()
                    Await Task.Delay(650)
                    Return Await Task.FromResult(True)
                ElseIf Down Then
                    FadingVol = True
                    FaderUpTimer.Stop()
                    PartialFaderDownTimer.Stop()
                    FaderDownTimer.Start()
                    Await Task.Delay(650)
                    Return Await Task.FromResult(True)
                End If
            End If
        End Function
        Private Sub FaderUpTimer_Tick() Handles FaderUpTimer.Elapsed
            Dim vol As Single = 0F
            Bass.BASS_ChannelGetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, vol)
            If vol >= Volume Then
                FaderUpTimer.Stop()
                FadingVol = False
                Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, Volume)
            Else
                FadingVol = True
                If Not Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, vol + 0.025) Then
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 1)
                End If
            End If
        End Sub
        Private Sub FaderDownTimer_Tick() Handles FaderDownTimer.Elapsed
            Dim vol As Single = 0F
            Bass.BASS_ChannelGetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, vol)
            If vol <= 0 Then
                FaderDownTimer.Stop()
                FadingVol = False
                Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 0)
            Else
                FadingVol = True
                If Not Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, vol - 0.025) Then
                    Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 0)
                End If
            End If
        End Sub
    Private Sub PartialFaderDownTimer_Tick() Handles PartialFaderDownTimer.Elapsed
        Dim vol As Single = 0F
        Bass.BASS_ChannelGetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, vol)
        If vol <= PartialDownTo Then
            FaderDownTimer.Stop()
            FadingVol = False
            Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, PartialDownTo)
        Else
            FadingVol = True
            If Not Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, vol - 0.025) Then
                Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, 0)
            End If
        End If
    End Sub
    Private OldVol As Single
    Public Async Function FadeVol(Up As Boolean, Down As Boolean, ToVol As Single) As Task
        If Not Single.IsNaN(ToVol) Then
            OldVol = Volume
        End If
        Await Task.Run(Sub()
                           If Not Single.IsNaN(ToVol) Then
                               If Up = True Then
                                   Do While Volume <= ToVol
                                       If Volume = ToVol Then Exit Do
                                       SetVolume(Volume + 0.001, False)
                                   Loop
                               Else
                                   Do While Volume >= ToVol
                                       If Volume = ToVol Then Exit Do
                                       SetVolume(Volume - 0.001, False)
                                   Loop
                               End If
                           Else
                               If Up = True Then
                                   Do While Volume <= OldVol
                                       If Volume = OldVol Then Exit Do
                                       SetVolume(Volume + 0.001, False)
                                   Loop
                               Else
                                   Do While Volume >= OldVol
                                       If Volume = OldVol Then Exit Do
                                       SetVolume(Volume - 0.001, False)
                                   Loop
                               End If
                           End If
                       End Sub)
    End Function
#End Region
    Public Function GetOutputDevices()
        Dim n As Integer = 0
        Dim DevicesList As New List(Of String)
        Dim info As New BASS_DEVICEINFO()
        While (Bass.BASS_GetDeviceInfo(n, info))
            DevicesList.Add(info.ToString)
            n += 1
        End While
        Return DevicesList
    End Function
    Public Sub SetOutputDevice(index As Integer)
        Bass.BASS_Init(index, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)
        Bass.BASS_SetDevice(index)
        Bass.BASS_ChannelSetDevice(Stream, index)
    End Sub
    Private Sub SetABLoop()
        If _ABLoop IsNot Nothing Then
            _ABLoopTimer.Start()
        Else
            _ABLoopTimer.Stop()
        End If
    End Sub
    Private Sub _ABLoopTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _ABLoopTimer.Elapsed
        If GetPosition() >= _ABLoop.B Then
            SetPosition(_ABLoop.A)
            'ElseIf GetPosition() < _ABLoop.A Then
            '    SetPosition(_ABLoop.A)
        End If
    End Sub
    Public Sub ImportDSP(file As String)
        If IsDSPLoaded = False Then Throw New Exception("basswadsp is not loaded")
        Dim dspPluginA As Integer = BassWaDsp.BASS_WADSP_Load(file, 5, 5, 500, 500, Nothing)
        If dspPluginA <> 0 Then
            If Not BassWaDsp.BASS_WADSP_Start(dspPluginA, 0, 0) Then Throw New Exception("Couldn't start the DSP plugin. The DSP plugin is probably already running." & Bass.BASS_ErrorGetCode.ToString)
            Dim hdsp = BassWaDsp.BASS_WADSP_ChannelSetDSP(dspPluginA, Stream, 1)
            If hdsp <> 0 Then
                Dim dsp = New DSPPlugin(file, BassWaDsp.BASS_WADSP_GetName(dspPluginA), dspPluginA, hdsp)
                LoadedDSP.Add(dsp)
                RaiseEvent OnDspAdded(dsp)
            Else
                Throw New Exception(Bass.BASS_ErrorGetCode.ToString)
            End If
        End If
    End Sub
    Public Sub RemoveDSP(DSP As DSPPlugin)
        If IsDSPLoaded = False Then Throw New Exception("basswadsp is not loaded")
        If Not BassWaDsp.BASS_WADSP_ChannelRemoveDSP(DSP.Handle) Then
            Throw New Exception("Couldn't remove the DSP plugin " & Bass.BASS_ErrorGetCode.ToString)
        Else
            If Not BassWaDsp.BASS_WADSP_FreeDSP(DSP.Handle) Then
                Throw New Exception("Couldn't remove the DSP plugin " & Bass.BASS_ErrorGetCode.ToString)
            Else
                LoadedDSP.Remove(LoadedDSP.FirstOrDefault(Function(k) k.Handle = DSP.Handle))
                RaiseEvent OnDspRemoved(DSP)
            End If
        End If
    End Sub
#End Region
#Region "Visuals"
    Public Function CreateVisualizer(type As Visualizers, width As Integer, height As Integer, color1 As System.Drawing.Color, color2 As System.Drawing.Color, color3 As System.Drawing.Color, background As System.Drawing.Color, linewidth As Integer, peakwidth As Integer, distance As Integer, peakdelay As Integer, linear As Boolean, fullspectrum As Boolean, highquality As Boolean) As System.Drawing.Bitmap
            If type = Visualizers.Line Then
                Return Visualizer.CreateSpectrumLine(Stream, width, height, color1, color2, background, linewidth, distance, True, False, False)
            ElseIf type = Visualizers.Wave Then
                Return Visualizer.CreateWaveForm(Stream, width, height, color1, color2, System.Drawing.Color.Empty, background, linewidth, False, True, False)
            ElseIf type = Visualizers.Spectrum Then
                Return Visualizer.CreateSpectrum(Stream, width, height, color1, color2, background, linear, fullspectrum, highquality)
            ElseIf type = Visualizers.SpectumLine Then
                Return Visualizer.CreateSpectrumLine(Stream, width, height, color1, color2, background, linewidth, distance, linear, fullspectrum, highquality)
            ElseIf type = Visualizers.SpectrumPeak Then
                Return Visualizer.CreateSpectrumLinePeak(Stream, width, height, color1, color2, color3, background, linewidth, peakwidth, distance, peakdelay, linear, fullspectrum, highquality)
            Else
                Return Nothing
            End If
        End Function
#End Region
#Region "Error Resolvers"
    Public Async Sub ReSendPlayStateChangedEvent(state As State, Wait As Integer, <Runtime.CompilerServices.CallerMemberName> ByVal Optional propertyName As String = Nothing, <Runtime.CompilerServices.CallerLineNumber> ByVal Optional propertyLine As String = Nothing)
        Try
            Await Task.Delay(Wait)
            RaiseEvent PlayerStateChanged(state)
        Catch ex As Exception
        End Try
    End Sub
#End Region
#Region "Enums"
    Public Enum State
        Undefined = 0
        MediaLoaded = 1
        Playing = 2
        Paused = 3
        Stopped = 4
        _Error = 5
    End Enum
    Public Enum Visualizers
            Line = 0
            Wave = 1
            Spectrum = 2
            SpectumLine = 3
            SpectrumPeak = 4
        End Enum
    Public Enum LinkHandles
        Stream = 0
        EQ = 1
        Reverb = 2
        Loudness = 3
        FX = 4
        SFX = 5
        BASS = 6
        Balance = 7
        SampleRate = 8
        StereoMix = 9
        Rotate = 10
    End Enum
    Public Enum StreamTypes
            URL = 0
            Youtube = 1
            Soundcloud = 2
            Local = 3
        End Enum

    Public Enum RepeateBehaviour
        NoRepeat = 0
        RepeatOne = 1
        RepeatAll = 2
        Shuffle = 3
        NoShuffle = 4
    End Enum
    Public Enum RotatePreset
        Slow
        Med
        Fast
    End Enum
#End Region
    Public Class ABLoopItem
        Private Property _A As Double
        Private Property _B As Double
        Public ReadOnly Property A As Double
            Get
                Return _A
            End Get
        End Property
        Public ReadOnly Property B As Double
            Get
                Return _B
            End Get
        End Property
        Public Sub New(A As Double, B As Double)
            _A = A
            _B = B
        End Sub
    End Class
    Public Class DSPPlugin
        Public Property Name As String
        Public Property Handle As Integer
        Public Property HDSP As Integer
        Public Property Source As String
        Public Sub New(_Source As String, _name As String, Hndl As Integer, _HDSP As Integer)
            Source = _Source
            Name = _name
            Handle = Hndl
        End Sub
        Public Sub Config()
            If Not BassWaDsp.BASS_WADSP_Config(Handle) Then Throw New InvalidOperationException
        End Sub
    End Class
End Class