Public Class Command
#Const WIN1010240 = True
    Public Shared Property Commands As String() = {"home hide overlay", "playlist show overlay", "playlist hide overlay", "home show reflection", "home hide reflection", "playlist index", "playlist count", "playlist items", "playlist items -s", "playlist items -n", "playlist items -u", "playlist add random", "exit", "exit n", "clear", "next", "previous", "stop", "pause", "play", "init", "free", "fade down", "fade down db", "fade up", "fade up db", "fade to", "fade to db", "eq enable", "eq disable", "reverb enable", "update reverb ingain", "update reverb mix", "update reverb time", "update reverb hfrtr", "reverb disable", "handle", "plugin fx", "plugin sfx", "set sfx fps", "source url", "discord disconnect", "discord connect", "discord set presence", "save", "channelinfo", "tags", "plugins", "notification", "debug", "error", "hooks", "set custom state", "library stats", "search", "refresh library stats", "library make", "library make artists", "library make years", "fullscreen", "fullscreen -0", "library update artists", "library update years", "about", "hotkeys", "home show overlay", "home refresh recommended", "home show recommended", "home topmost off", "home topmost on", "home set visualizer refresh speed", "home hide recommended", "version", "taskbar add thumbnail", "taskbar remove thumbnail", "taskbar clear thumbnail", "taskbar thumbnail manager", "test plugin system", "set api timeout", "enable double output", "disable double output", "set double output", "val to percentage", "percentage to val", "home visualiser", "settings browse", "engine state", "library manager", "library manager new", "playlist get previous", "playlist get next", "playlist play at", "playlist play at force"}
    Public Shared Async Sub Excute(command As String, Window As Window)
        Select Case command.ToLower
            Case "home topmost off"
                CType(Window, MainWindow).Topmost = False
                Exit Sub
            Case "home topmost on"
                CType(Window, MainWindow).Topmost = True
                Exit Sub
            Case "home set visualizer refresh speed"
                CType(Window, MainWindow).MainUIVisualizerUpdator.Interval = InputBox("Interval in Milliseconds")
                Exit Sub
            Case "home hide recommended"
                With CType(Window, MainWindow)
                    .Home_Rec1.Visibility = Visibility.Hidden
                    .Home_Rec2.Visibility = Visibility.Hidden
                    .Home_Rec3.Visibility = Visibility.Hidden
                    .Home_Rec4.Visibility = Visibility.Hidden
                    .Home_Rec5.Visibility = Visibility.Hidden
                    .Home_Rec6.Visibility = Visibility.Hidden
                    .Home_Rec7.Visibility = Visibility.Hidden
                End With
                Exit Sub
            Case "home show recommended"
                With CType(Window, MainWindow)
                    .Home_Rec1.Visibility = Visibility.Visible
                    .Home_Rec2.Visibility = Visibility.Visible
                    .Home_Rec3.Visibility = Visibility.Visible
                    .Home_Rec4.Visibility = Visibility.Visible
                    .Home_Rec5.Visibility = Visibility.Visible
                    .Home_Rec6.Visibility = Visibility.Visible
                    .Home_Rec7.Visibility = Visibility.Visible
                End With
                Exit Sub
            Case "home refresh recommended"
                With CType(Window, MainWindow)
                    .Switches_RecChanger.IsChecked = False
                    .RefreshRecommended()
                    .Switches_RecChanger.IsChecked = True
                End With
                Exit Sub
            Case "home show overlay"
                CType(Window, MainWindow).Switches_OverlayChanger.IsChecked = True
                Exit Sub
            Case "home hide overlay"
                CType(Window, MainWindow).Switches_OverlayChanger.IsChecked = False
                Exit Sub
            Case "playlist show overlay"
                CType(Window, MainWindow).Switches_Playlist_Overlay_Changer.IsChecked = True
                Exit Sub
            Case "playlist hide overlay"
                CType(Window, MainWindow).Switches_Playlist_Overlay_Changer.IsChecked = False
                Exit Sub
            Case "home show reflection"
                CType(Window, MainWindow).Home_TopBarCanvas_Reflection.Visibility = Visibility.Visible
                Exit Sub
            Case "home hide reflection"
                CType(Window, MainWindow).Home_TopBarCanvas_Reflection.Visibility = Visibility.Hidden
                Exit Sub
            Case "playlist index"
                MsgBox("Index: " & CType(Window, MainWindow).MainPlaylist.Index & vbCrLf & "Actual Index: " & CType(Window, MainWindow).MainPlaylist.Index + 1)
                Exit Sub
            Case "playlist count"
                MsgBox("Count: " & CType(Window, MainWindow).MainPlaylist.Count)
                Exit Sub
            Case "playlist items"
                Dim limit = InputBox("Return limit")
                Dim SB As New Text.StringBuilder
                If limit = -1 Then
                    For i As Integer = 0 To CType(Window, MainWindow).MainPlaylist.Count - 1
                        SB.AppendLine(i + 1 & "-" & CType(Window, MainWindow).MainPlaylist.GetItem(i))
                    Next
                Else
                    For i As Integer = 0 To limit
                        SB.AppendLine(i + 1 & "-" & CType(Window, MainWindow).MainPlaylist.GetItem(i))
                    Next
                End If
                MsgBox(SB.ToString)
                SB = Nothing
                Exit Sub
            Case "playlist items -s"
                Dim limit = InputBox("Return limit")
                Dim SB As New Text.StringBuilder
                If limit = -1 Then
                    For i As Integer = 0 To My.Settings.LASTPLAYLIST.Count - 1
                        SB.AppendLine(My.Settings.LASTPLAYLIST(i))
                    Next
                Else
                    For i As Integer = 0 To limit
                        SB.AppendLine(My.Settings.LASTPLAYLIST(i))
                    Next
                End If
                MsgBox(SB.ToString)
                SB = Nothing
                Exit Sub
            Case "playlist items -n"
                Dim limit = InputBox("Return limit")
                Dim SB As New Text.StringBuilder
                If limit = -1 Then
                    For i As Integer = 0 To CType(Window, MainWindow).MainPlaylist.Count - 1
                        SB.AppendLine(i + 1 & "-" & CType(Window, MainWindow).MainPlaylist.Playlist.Item(i))
                    Next
                Else
                    For i As Integer = 0 To limit
                        SB.AppendLine(i + 1 & "-" & CType(Window, MainWindow).MainPlaylist.Playlist.Item(i))
                    Next
                End If
                MsgBox(SB.ToString)
                SB = Nothing
                Exit Sub
            Case "playlist items -u"
                Dim limit = InputBox("Return limit")
                Dim SB As New Text.StringBuilder
                If limit = -1 Then
                    For i As Integer = 0 To CType(Window, MainWindow).playlistItems.Count - 1
                        SB.AppendLine(i + 1 & "-" & CType(Window, MainWindow).playlistItems(i).Source)
                    Next
                Else
                    For i As Integer = 0 To limit
                        SB.AppendLine(i + 1 & "-" & CType(Window, MainWindow).playlistItems(i).Source)
                    Next
                End If
                MsgBox(SB.ToString)
                SB = Nothing
                Exit Sub
            Case "playlist add random"
                Dim RND As New Random
                CType(Window, MainWindow).playlistItems.Add(New PlaylistItem(RND.Next, RND.Next, RND.Next, RND.Next, RND.Next, RND.Next, RND.Next(0, 3), RND.Next, Nothing))
                RND = Nothing
                Exit Sub
            Case "exit"
                CType(Window, MainWindow).Close()
                Exit Sub
            Case "exit n"
                Application.Current.Shutdown()
                Exit Sub
            Case "clear"
                CType(Window, MainWindow).MainPlayer.StreamStop()
                CType(Window, MainWindow).MainPlaylist.Clear()
                Exit Sub
            Case "next"
                CType(Window, MainWindow).MainPlayer.LoadSong(CType(Window, MainWindow).MainPlaylist.NextSong, Nothing, False)
                CType(Window, MainWindow).MainPlayer.StreamPlay()
                Exit Sub
            Case "previous"
                CType(Window, MainWindow).MainPlayer.LoadSong(CType(Window, MainWindow).MainPlaylist.PreviousSong, Nothing, False)
                CType(Window, MainWindow).MainPlayer.StreamPlay()
                Exit Sub
            Case "stop"
                CType(Window, MainWindow).MainPlayer.StreamStop()
                Exit Sub
            Case "pause"
                CType(Window, MainWindow).MainPlayer.StreamPause()
                Exit Sub
            Case "play"
                CType(Window, MainWindow).MainPlayer.StreamPlay()
                Exit Sub
            Case "init"
                CType(Window, MainWindow).MainPlayer.Init()
                Exit Sub
            Case "free"
                CType(Window, MainWindow).MainPlayer.Dispose()
                Exit Sub
            Case "fade down"
                Await CType(Window, MainWindow).MainPlayer.FadeVol(0, 1, False)
                Exit Sub
            Case "fade down db"
                Await CType(Window, MainWindow).MainPlayer.FadeVol(0, 1, True)
                Exit Sub
            Case "fade up"
                Await CType(Window, MainWindow).MainPlayer.FadeVol(1, 1, False)
                Exit Sub
            Case "fade up db"
                Await CType(Window, MainWindow).MainPlayer.FadeVol(1, 1, True)
                Exit Sub
            Case "fade to"
                Dim _to = InputBox("To")
                Await CType(Window, MainWindow).MainPlayer.FadeVol(_to, 1, False)
                Exit Sub
            Case "fade to db"
                Dim _to = InputBox("To")
                Await CType(Window, MainWindow).MainPlayer.FadeVol(_to, 1, True)
                Exit Sub
            Case "eq enable"
                Exit Sub
            Case "eq disable"
                CType(Window, MainWindow).MainPlayer.UpdateEQ(0, 0, True)
                Exit Sub
            Case "reverb enable"
                CType(Window, MainWindow).MainPlayer.UpdateReverb(0, 0, 0, 0, False, False, False, False, False, True)
                Exit Sub
            Case "update reverb ingain"
                CType(Window, MainWindow).MainPlayer.UpdateReverb(InputBox("Ingain"), 0, 0, 0, True, False, False, False)
                Exit Sub
            Case "update reverb mix"
                CType(Window, MainWindow).MainPlayer.UpdateReverb(0, InputBox("Mix"), 0, 0, False, True, False, False)
                Exit Sub
            Case "update reverb time"
                CType(Window, MainWindow).MainPlayer.UpdateReverb(0, 0, InputBox("Time"), 0, False, False, True, False)
                Exit Sub
            Case "update reverb hfrtr"
                CType(Window, MainWindow).MainPlayer.UpdateReverb(0, 0, 0, InputBox("HighFreq RT Ratio"), False, False, False, True)
                Exit Sub
            Case "reverb disable"
                CType(Window, MainWindow).MainPlayer.UpdateReverb(0, 0, 0, 0, False, False, False, False, True)
                Exit Sub
            Case "handle"
                MsgBox(CType(Window, MainWindow).MainPlayer.GetHandle(InputBox(String.Join(vbCrLf, System.Enum.GetNames(GetType(Player.LinkHandles))))))
                Exit Sub
            Case "plugin fx"
                MsgBox("Bass_FX: " & CType(Window, MainWindow).MainPlayer.IsFXLoaded)
                Exit Sub
            Case "plugin sfx"
                MsgBox("Bass_SFX: " & CType(Window, MainWindow).MainPlayer.IsSFXLoaded)
                Exit Sub
            Case "set sfx fps"
                CType(Window, MainWindow).SFXVisualRenderer.Interval = InputBox("Interval in MS")
                Exit Sub
            Case "source url"
                MsgBox(CType(Window, MainWindow).MainPlayer.SourceURL)
                Exit Sub
            Case "discord disconnect"
                Try
                    CType(Window, MainWindow).RPCClient.Deinitialize()
                    Exit Sub
                Catch ex As Exception
                    Throw ex
                    Exit Sub
                End Try
            Case "discord connect"
                Try
                    CType(Window, MainWindow).RPCClient = New DiscordRPC.DiscordRpcClient("779393129366683719")
                    CType(Window, MainWindow).RPCClient.Initialize()
                    Exit Sub
                Catch ex As Exception
                    Throw ex
                    Exit Sub
                End Try
            Case "discord set presence"
                Dim _command = InputBox("use %d for details and %s for state", "Command")
                Try
                    If Not String.IsNullOrEmpty(_command) Then
                        Dim _det As String = _command.Substring(_command.IndexOf("%d") + 2, _command.IndexOf("%s") - 2)
                        Dim _stt As String = _command.Substring(_command.IndexOf("%s") + 2)
                        CType(Window, MainWindow).RPCClient.SetPresence(New DiscordRPC.RichPresence With {.Details = _det, .State = _stt, .Assets = New DiscordRPC.Assets With {.LargeImageKey = "muplayerl", .LargeImageText = "MuPlay", .SmallImageKey = "statepaused", .SmallImageText = "Paused"}})
                    End If
                    Exit Sub
                Catch ex As Exception
                    Throw ex
                    Exit Sub
                End Try
            Case "save"
                With CType(Window, MainWindow).MainPlayer
                    If .SourceURL IsNot Nothing Then
                        My.Settings.LASTMEDIATYPE = .CurrentMediaType
                        My.Settings.LASTMEDIATITLE = .CurrentMediaTitle
                        My.Settings.LASTMEDIAARTIST = .CurrentMediaArtist
                        My.Settings.LASTMEDIA = .SourceURL
                        My.Settings.LASTMEDIASEEK = .GetPosition
                        My.Settings.LASTMEDIADURATION = .GetLength
                        My.Settings.LASTMEDIAINDEX = CType(Window, MainWindow).MainPlaylist.Index
                        My.Settings.LASTPLAYLIST.Clear()
                        For Each song In CType(Window, MainWindow).MainPlaylist.Playlist
                            My.Settings.LASTPLAYLIST.Add(song)
                        Next
                        'For i As Integer = 0 To MainPlaylist.Count - 1
                        'My.Settings.LastPlaylist.Add(MainPlaylist.GetItem(i) & playlistItems(i).Type)
                        'Next
                    End If
                End With
                My.Settings.Save()
                Exit Sub
            Case "channelinfo"
                My.Windows.ChannelInfo.Show()
                Exit Sub
            Case "tags"
                Try
                    Dim Tags = CType(Window, MainWindow).MainPlayer.GetTags
                    Dim Artist = Tags.artist
                    Dim Title = Tags.title
                    Dim Album = Tags.album
                    Dim Year = Tags.year
                    Dim TrackNum = Tags.track
                    Dim Lyrics = Tags.lyricist
                    Dim Genres = Tags.genre
                    Dim Bitrates = Tags.bitrate
                    Dim CompressedInfo As String()
                    Dim Channelinfo = Un4seen.Bass.Bass.BASS_ChannelGetInfo(CType(Window, MainWindow).MainPlayer.Stream)
                    CompressedInfo = {Artist, Title, Album, Year, TrackNum, Genres, Lyrics, Channelinfo.chans, Channelinfo.freq, Channelinfo.origres, Channelinfo.filename, Bitrates}
                    MsgBox(String.Join(vbCrLf, CompressedInfo))
                    Exit Sub
                Catch ex As Exception
                    Exit Sub
                End Try
            Case "plugins"
                My.Windows.Plugins.Show()
                Exit Sub
            Case "notification"
                TryCast(Window, MainWindow).ShowNotification("Debug", InputBox("Message"), HandyControl.Data.NotifyIconInfoType.Info)
                Exit Sub
            Case "debug"
                Dim Password As String = "AneS2002" & DateTime.UtcNow.Hour & DateTime.UtcNow.Minute & "//" & DateTime.UtcNow.Day & DateTime.UtcNow.Month & DateTime.UtcNow.Year
                If InputBox("Password ?", "Debug", "Hello admin !") = Password Then
                    TryCast(Window, MainWindow).ShowNotification("Welcome To Debug Mode", "All Devlopper options are now availble.", HandyControl.Data.NotifyIconInfoType.Info)
                    With My.Windows.Settings
                        .onerror_stacktrace.IsEnabled = True
                        .onerror_tostring.IsEnabled = True
                    End With
                    My.Windows.Console.LogDebug = True
                    Exit Sub
                End If
            Case "error"
                Throw New Exception("error")
                Exit Sub
            Case "hooks"
                Dim SB As New Text.StringBuilder
                SB.AppendLine("PlayPause: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_PLAYPAUSE_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_PLAYPAUSE))
                SB.AppendLine("Next: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_NEXT_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_NEXT))
                SB.AppendLine("Previous: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_PREVIOUS_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_PREVIOUS))
                SB.AppendLine("Skip 10: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_SKIP10_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_SKIP10))
                SB.AppendLine("Back 10: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_BACK10_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_BACK10))
                SB.AppendLine("Volume Up: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_VOLUMEUP_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_VOLUMEUP))
                SB.AppendLine("Volume Down: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_VOLUMEDOWN_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_VOLUMEDOWN))
                SB.AppendLine("Volume Mute: " & Utils.IntToModSTR(My.Settings.GLOBALHOTKEY_VOLUMEMUTE_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_VOLUMEMUTE))
                MsgBox(SB.ToString)
                SB = Nothing
                Exit Sub
            Case "set custom state"
                Dim sb As New Text.StringBuilder
                Dim states = System.Enum.GetNames(GetType(Player.State))
                For i As Integer = 0 To states.Count - 1
                    sb.AppendLine(i & "-" & states(i))
                Next
                CType(Window, MainWindow).MainPlayer.SetCustomState(InputBox(sb.ToString))
                Exit Sub
            Case "library stats"
                MsgBox(TryCast(Window, MainWindow).MainLibrary.DateCreated & "//" & TryCast(Window, MainWindow).MainLibrary.Count)
                Exit Sub
            Case "search"
                My.Windows.Search.Show()
                Exit Sub
            Case "refresh library stats"
                Await TryCast(Window, MainWindow).MainLibrary.RefreshStats
                Exit Sub
            Case "library make"
                Dim Temp_List As New List(Of String)
                For Each path In My.Settings.LIBRARIESPATH
                    Temp_List.Add(path)
                Next
                Dim Temp_Library As New Library(Await Library.MakeLibrary(Utils.AppDataPath, Temp_List))
                Dim files As New List(Of String)
                For Each path In My.Settings.LIBRARIESPATH
                    For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                        files.Add(song)
                    Next
                Next
                Await Temp_Library.AddTracksToLibraryAsync(files)
                Temp_List = Nothing
                Exit Sub
            Case "library make artists"
                Await TryCast(Window, MainWindow).MainLibrary.CacheArtists(Utils.AppDataPath)
                Exit Sub
            Case "library make years"
                Await TryCast(Window, MainWindow).MainLibrary.CacheYears(Utils.AppDataPath)
                Exit Sub
            Case "fullscreen"
                My.Windows.FullScreenPlayer.Owner = Window
                My.Windows.FullScreenPlayer.Main_BG.Opacity = 1
                My.Windows.FullScreenPlayer.ShowDialog()
                Exit Sub
            Case "fullscreen -0"
                My.Windows.FullScreenPlayer.Owner = Window
                My.Windows.FullScreenPlayer.Main_BG.Opacity = 0
                My.Windows.FullScreenPlayer.ShowDialog()
                Exit Sub
            Case "library update artists"
                With TryCast(Window, MainWindow)
                    .GArtists = Await .MainLibrary.ReadArtistsCache(Utils.AppDataPath)
                    For i As Integer = 0 To .GArtists.Count - 1
                        Try
                            .libraryArtistsItems.Add(New ArtistItem(i + 1, .GArtists(i)))
                        Catch ex As Exception
                        End Try
                    Next
                End With
            Case "library update years"
                With TryCast(Window, MainWindow)
                    .GYears = Await .MainLibrary.ReadYearsCache(Utils.AppDataPath)
                    For i As Integer = 0 To .GYears.Count - 1
                        Try
                            .libraryYearsItems.Add(New YearItem(i + 1, .GYears(i)))
                        Catch ex As Exception
                        End Try
                    Next
                End With
            Case "about"
                My.Windows.About.Show()
                Exit Sub
            Case "version"
                With My.Application.Info
                    Dim sb As New Text.StringBuilder
                    sb.AppendLine("Assembly Name: " & .AssemblyName)
                    sb.AppendLine("Company Name: " & .CompanyName)
                    sb.AppendLine("Copyright: " & .Copyright)
                    sb.AppendLine("Version: " & .Version.ToString)
                    sb.AppendLine(" Build: " & .Version.Build)
                    sb.AppendLine(" Major: " & .Version.Major)
                    sb.AppendLine(" Major Revision: " & .Version.MajorRevision)
                    sb.AppendLine(" Minor: " & .Version.Minor)
                    sb.AppendLine(" Minor Revision: " & .Version.MinorRevision)
                    sb.AppendLine(" Revision: " & .Version.Revision)
                    sb.AppendLine("Working Set: " & .WorkingSet)
#If WIN1010240 Then
                    sb.AppendLine("Compatiblity: WIN10 10240+")
#Else
                    sb.AppendLine("Compatiblity: WIN7+")
#End If
                    MessageBox.Show(Window, sb.ToString, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Information)
                End With
                Exit Sub
            Case "hotkeys"
                Dim names = System.Enum.GetNames(GetType(Forms.Keys))
                Dim sb As New Text.StringBuilder
                sb.AppendLine("PlayPause: " & My.Settings.GLOBALHOTKEY_PLAYPAUSE_MOD & "//" & My.Settings.GLOBALHOTKEY_PLAYPAUSE & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_PLAYPAUSE).ToString & "]")
                sb.AppendLine("Next: " & My.Settings.GLOBALHOTKEY_NEXT_MOD & "//" & My.Settings.GLOBALHOTKEY_NEXT & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_NEXT).ToString & "]")
                sb.AppendLine("Previous: " & My.Settings.GLOBALHOTKEY_PREVIOUS_MOD & "//" & My.Settings.GLOBALHOTKEY_PREVIOUS & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_PREVIOUS).ToString & "]")
                sb.AppendLine("+10s: " & My.Settings.GLOBALHOTKEY_SKIP10_MOD & "//" & My.Settings.GLOBALHOTKEY_SKIP10 & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_SKIP10).ToString & "]")
                sb.AppendLine("-10s: " & My.Settings.GLOBALHOTKEY_BACK10_MOD & "//" & My.Settings.GLOBALHOTKEY_BACK10 & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_BACK10).ToString & "]")
                sb.AppendLine("Vol Up: " & My.Settings.GLOBALHOTKEY_VOLUMEUP_MOD & "//" & My.Settings.GLOBALHOTKEY_VOLUMEUP & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_VOLUMEUP).ToString & "]")
                sb.AppendLine("Vol Down: " & My.Settings.GLOBALHOTKEY_VOLUMEDOWN_MOD & "//" & My.Settings.GLOBALHOTKEY_VOLUMEDOWN & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_VOLUMEDOWN).ToString & "]")
                sb.AppendLine("Vol Mute: " & My.Settings.GLOBALHOTKEY_VOLUMEMUTE_MOD & "//" & My.Settings.GLOBALHOTKEY_VOLUMEMUTE & "[" & System.Enum.ToObject(GetType(Forms.Keys), My.Settings.GLOBALHOTKEY_VOLUMEMUTE).ToString & "]")
                MsgBox(sb.ToString, MsgBoxStyle.MsgBoxHelp)
                Exit Sub
            Case "taskbar add thumbnail"
                My.Windows.TaskbarThumbnailManager.SetThumb(My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.AddWindow())
                Exit Sub
            Case "taskbar remove thumbnail"
                Dim sb As New Text.StringBuilder
                For i As Integer = 0 To My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.Wnds.Count - 1
                    Try
                        sb.AppendLine(i & My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.Wnds(i).Thumbnail.Title & ";" & My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.Wnds(i).Thumbnail.Tooltip)
                    Catch ex As Exception
                        sb.AppendLine(i & ": Nothing")
                    End Try
                Next
                My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.RemoveThumbnail(InputBox(sb.ToString, "Debug"))
                Exit Sub
            Case "taskbar clear thumbnail"
                For i As Integer = 0 To My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.Wnds.Count - 1
                    My.Windows.TaskbarThumbnailManager.TaskbarThumbnailManager.RemoveThumbnail(i)
                Next
                Exit Sub
            Case "taskbar thumbnail manager"
                My.Windows.TaskbarThumbnailManager.Show()
                Exit Sub
            Case "test plugin system"
                Dim Plugitem As New Utils.PluginItem("Debug Plugin", "MuPlay///DebugPlugin") With {.ID = 1234}
                Plugitem.Entries.Add(New Utils.PluginItem.Entry(5678, "Entry 1"))
                Plugitem.Entries.Add(New Utils.PluginItem.Entry(9012, "Entry 2"))
                Plugitem.Entries.Add(New Utils.PluginItem.Entry(3456, "Entry 3"))
                Dim STRPlugin As String = Plugitem.ToString(">")
                Dim Plugin = Utils.PluginItem.FromString(">", STRPlugin)
                Dim Result As New List(Of String)
                If Plugitem.Name = Plugin.Name Then Result.Add("Name: " & True) Else Result.Add("Name: " & False)
                If Plugitem.ID = Plugin.ID Then Result.Add("ID: " & True) Else Result.Add("ID: " & False)
                If Plugitem.Source = Plugin.Source Then Result.Add("Source: " & True) Else Result.Add("Source: " & False)
                If Plugitem.Entries.Count = Plugin.Entries.Count Then
                    Result.Add("Entries.Count: " & True)
                    For i As Integer = 0 To Plugitem.Entries.Count - 1
                        Result.Add("Entries.Item(" & i & ") = " & Plugitem.Entries.Item(i).ToString("|") & " >|< " & "Entries.Item(" & i & ") = " & Plugin.Entries.Item(i).ToString("|"))
                    Next
                Else
                    Result.Add("Entries.Count: " & False)
                    Result.Add("Entries.Items: Obviously failed.")
                End If
                MessageBox.Show("Result: " & Environment.NewLine & String.Join(Environment.NewLine, Result), "Debug", MessageBoxButton.OK, MessageBoxImage.Information)
                Exit Sub
            Case "set api timeout"
                Dim timeout As Integer
                If Integer.TryParse(InputBox("Time out in ms:"), timeout) Then
                    My.Settings.API_TIMEOUT = timeout
                    My.Settings.Save()
                End If
                Exit Sub
            Case "enable double output"
                TryCast(Window, MainWindow).MainPlayer.DoubleOutput = True
                Exit Sub
            Case "disable double output"
                TryCast(Window, MainWindow).MainPlayer.DoubleOutput = False
                Exit Sub
            Case "set double output"
                Dim Device As Integer = 0
                If Integer.TryParse(InputBox("Device index :" & String.Join(vbCrLf, TryCast(Window, MainWindow).MainPlayer.GetOutputDevices)), Device) Then
                    TryCast(Window, MainWindow).MainPlayer.SetDoubleOutputDevice(Device)
                End If
                Exit Sub
            Case "val to percentage"
                Dim args = InputBox("Val;Min;Max").Split(";")
                My.Windows.Console.Log(Utils.ValToPercentage(args(0), args(1), args(2)) & "%")
                Exit Sub
            Case "percentage to val"
                Dim args = InputBox("Percentage;Min;Max").Split(";")
                My.Windows.Console.Log(Utils.PercentageToVal(args(0), args(1), args(2)))
                Exit Sub
            Case "mixer"
                'Under devlopment
                My.Windows.PlayerMixer.Owner = Window
                My.Windows.PlayerMixer.Show()
                Exit Sub
            Case "home visualiser"
                My.Settings.HOMEVISUALISERTYPE = InputBox(String.Join(Environment.NewLine, System.Enum.GetNames(GetType(Player.Visualizers))))
                My.Settings.Save()
                Exit Sub
            Case "settings browse"
                My.Windows.SettingsBrowser.Show()
                Exit Sub
            Case "engine state"
                '{IsInitialized,IsFXLoaded, IsSFXLoaded, IsDSPLoaded, IsMidiLoaded}
                Dim EngineState = TryCast(Window, MainWindow).MainPlayer.GetEngineState()
                Dim SB As New Text.StringBuilder
                SB.AppendLine("IsInitialized: " & EngineState(0))
                SB.AppendLine("IsFXLoaded :  " & EngineState(1))
                SB.AppendLine("IsSFXLoaded: " & EngineState(2))
                SB.AppendLine("IsDSPLoaded: " & EngineState(3))
                SB.AppendLine("IsMidiLoaded: " & EngineState(4))
                MessageBox.Show(SB.ToString, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Information)
                Exit Sub
            Case "library manager"
                My.Windows.LibraryManager.Show()
                Exit Sub
            Case "library manager new"
                Dim libmgr As New LibraryManager
                libmgr.Show()
                Exit Sub
            Case "playlist get previous"
                Dim Playlist = TryCast(Window, MainWindow).MainPlaylist
                My.Windows.Console.Log(Playlist.GetPreviousSong & " At " & Playlist.GetPreviousSongIndex)
                Exit Sub
            Case "playlist get next"
                Dim Playlist = TryCast(Window, MainWindow).MainPlaylist
                My.Windows.Console.Log(Playlist.GetNextSong & " At " & Playlist.GetNextSongIndex)
                Exit Sub
            Case "playlist play at"
                Dim i As Integer = 0
                If Integer.TryParse(InputBox("Index..."), i) Then
                    Dim Playlist = TryCast(Window, MainWindow).MainPlaylist
                    If i < Playlist.Count Then
                        TryCast(Window, MainWindow).MainPlayer.LoadSong(Playlist.JumpTo(i), Nothing, False)
                    Else
                        My.Windows.Console.Log("Index cannot be less then the playlist count.")
                    End If
                End If
                Exit Sub
            Case "playlist play at force"
                Dim i As Integer = 0
                If Integer.TryParse(InputBox("Index..."), i) Then
                    Dim Playlist = TryCast(Window, MainWindow).MainPlaylist
                    TryCast(Window, MainWindow).MainPlayer.LoadSong(Playlist.JumpTo(i), Nothing, False)
                    End If
                Exit Sub
        End Select
        Throw New Exception("No such command!")
    End Sub
End Class
