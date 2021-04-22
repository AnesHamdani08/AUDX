Imports Un4seen.Bass

Public Class Command
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
                    For i As Integer = 0 To My.Settings.LastPlaylist.Count - 1
                        SB.AppendLine(My.Settings.LastPlaylist(i))
                    Next
                Else
                    For i As Integer = 0 To limit
                        SB.AppendLine(My.Settings.LastPlaylist(i))
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
                CType(Window, MainWindow).MainPlayer.FadeVol(False, True, 0)
                Exit Sub
            Case "fade up"
                CType(Window, MainWindow).MainPlayer.FadeVol(True, False, 1)
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
                        My.Settings.LastMediaType = .CurrentMediaType
                        My.Settings.LastMediaTitle = .CurrentMediaTitle
                        My.Settings.LastMediaArtist = .CurrentMediaArtist
                        My.Settings.LastMedia = .SourceURL
                        My.Settings.LastMediaSeek = .GetPosition
                        My.Settings.LastMediaDuration = .GetLength
                        My.Settings.LastMediaIndex = CType(Window, MainWindow).MainPlaylist.Index
                        My.Settings.LastPlaylist.Clear()
                        For Each song In CType(Window, MainWindow).MainPlaylist.Playlist
                            My.Settings.LastPlaylist.Add(song)
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
                    Dim Tags = CType(Application.Current.MainWindow, MainWindow).MainPlayer.GetTags
                    Dim Artist = Tags.artist
                    Dim Title = Tags.title
                    Dim Album = Tags.album
                    Dim Year = Tags.year
                    Dim TrackNum = Tags.track
                    Dim Lyrics = Tags.lyricist
                    Dim Genres = Tags.genre
                    Dim Bitrates = Tags.bitrate
                    Dim CompressedInfo As String()
                    Dim Channelinfo = Un4seen.Bass.Bass.BASS_ChannelGetInfo(CType(Application.Current.MainWindow, MainWindow).MainPlayer.Stream)
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
                TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("Debug", InputBox("Message"), HandyControl.Data.NotifyIconInfoType.Info)
                Exit Sub
            Case "debug"
                If InputBox("Password ?", "Debug", "Hello admin !") = "anes2002" Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("Hello admin !", "Debug mode: enabled", HandyControl.Data.NotifyIconInfoType.Info)
                    With My.Windows.Settings
                        .onerror_stacktrace.IsEnabled = True
                        .onerror_tostring.IsEnabled = True
                    End With
                End If
                Exit Sub
            Case "error"
                Throw New Exception("error")
                Exit Sub
            Case "hooks"
                Dim SB As New Text.StringBuilder
                SB.AppendLine("PlayPause: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_PlayPause_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_PlayPause))
                SB.AppendLine("Next: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_Next_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_Next))
                SB.AppendLine("Previous: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_Previous_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_Previous))
                SB.AppendLine("Skip 10: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_Skip10_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_Skip10))
                SB.AppendLine("Back 10: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_Back10_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_Back10))
                SB.AppendLine("Volume Up: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_VolumeUp_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_VolumeUp))
                SB.AppendLine("Volume Down: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_VolumeDown_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_VolumeDown))
                SB.AppendLine("Volume Mute: " & Utils.IntToModSTR(My.Settings.GlobalHotkey_VolumeMute_MOD) & "+" & System.Enum.GetName(GetType(Forms.Keys), My.Settings.GlobalHotkey_VolumeMute))
                MsgBox(SB.ToString)
                SB = Nothing
                Exit Sub
            Case "set custom state"
                Dim sb As New Text.StringBuilder
                Dim states = System.Enum.GetNames(GetType(Player.State))
                For i As Integer = 0 To states.Count - 1
                    sb.AppendLine(i & "-" & states(i))
                Next
                CType(Application.Current.MainWindow, MainWindow).MainPlayer.SetCustomState(InputBox(sb.ToString))
                Exit Sub
            Case "library stats"
                MsgBox(TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.DateCreated & "//" & TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.Count)
                Exit Sub
            Case "search"
                My.Windows.Search.Show()
                Exit Sub
            Case "refresh library stats"
                Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.RefreshStats
                Exit Sub
            Case "library make"
                Dim Temp_List As New List(Of String)
                For Each path In My.Settings.LibrariesPath
                    Temp_List.Add(path)
                Next
                Dim Temp_Library As New Library(Await Library.MakeLibrary(Utils.AppDataPath, Temp_List))
                Dim files As New List(Of String)
                For Each path In My.Settings.LibrariesPath
                    For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                        files.Add(song)
                    Next
                Next
                Await Temp_Library.AddTracksToLibraryAsync(files)
                Temp_List = Nothing
                Exit Sub
            Case "library make artists"
                         Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheArtists(Utils.AppDataPath)
                Exit Sub
                         Case "library make years"
                         Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheYears(Utils.AppDataPath)
                Exit Sub
                         Case "fullscreen"
                         My.Windows.FullScreenPlayer.Owner = Application.Current.MainWindow
                         My.Windows.FullScreenPlayer.ShowDialog()
                         Exit Sub
                         Case "library update artists"
                         With TryCast(Application.Current.MainWindow, MainWindow)
                             .GArtists = Await.MainLibrary.ReadArtistsCache(Utils.AppDataPath)
                             For i As Integer = 0 To .GArtists.Count - 1
                                 Try
                                     .libraryArtistsItems.Add(New ArtistItem(i + 1, .GArtists(i)))
                                 Catch ex As Exception
                                 End Try
                             Next
                         End With
                         Case "library update years"
                         With TryCast(Application.Current.MainWindow, MainWindow)
                             .GYears = Await.MainLibrary.ReadYearsCache(Utils.AppDataPath)
                             For i As Integer = 0 To .GYears.Count - 1
                                 Try
                                     .libraryYearsItems.Add(New ArtistItem(i + 1, .GYears(i)))
                                 Catch ex As Exception
                                 End Try
                             Next
                         End With
            Case "about"
                My.Windows.About.Show()
                Exit Sub
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
                    'sb.AppendLine("Compatiblity: WIN10 10240+")
                    sb.AppendLine("Compatiblity: WIN7+")
                    MessageBox.Show(Application.Current.MainWindow, sb.ToString, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Information)
                End With
                Exit Sub
        End Select
        Throw New Exception("No such command!")
    End Sub
End Class
