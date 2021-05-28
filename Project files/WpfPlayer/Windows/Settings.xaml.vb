Imports System.ComponentModel

Public Class Settings
    Private WithEvents Library As Library
    Private Sub Window_Activated(sender As Object, e As EventArgs)
    End Sub
    Private Sub Settings_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Hide()
        e.Cancel = True
    End Sub
    Private Sub Settings_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Lib_Loc_Cbox.Items.Clear()
        For Each Locations In My.Settings.LibrariesPath
            Lib_Loc_Cbox.Items.Add(Locations)
        Next
        'Setting
        set_discordrpc.IsChecked = My.Settings.UseDiscordRPC
        set_animations.IsChecked = My.Settings.UseAnimations
        Select Case My.Settings.BackgroundType
            Case 0
                set_background_cover.IsChecked = True
            Case 1
                set_background_fancy.IsChecked = True
        End Select
        set_mediabaranimationtype.SelectedIndex = My.Settings.MediaBar_AnimType
        set_home_visualiser.IsChecked = My.Settings.Home_ShowVisualiser
        set_onmediachange_fadeaudio.IsChecked = My.Settings.OnMediaChange_FadeAudio
        set_ttsspeaknotification.IsChecked = My.Settings.Notificationtts
        set_suppresserrors.IsChecked = My.Settings.SuppressErrors
        set_onerror.SelectedIndex = My.Settings.OnErrorShow
        set_output.Items.Clear()
        Dim n = 0
        For Each device In TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.GetOutputDevices
            n += 1
            set_output.Items.Add(n & "-" & device)
        Next
        Library = TryCast(Application.Current.MainWindow, MainWindow).MainLibrary
        If Library IsNot Nothing Then
            get_libcount.Text = Library.Count
            get_libdate.Text = Library.DateCreated
        Else
            get_libcount.Text = "N/A"
            get_libdate.Text = "N/A"
        End If
        set_cachelibrarydata.IsChecked = My.Settings.CacheLibraryData
        Set_UpdatesServer.Text = My.Settings.UpdatesServer
        set_skipsilences.IsChecked = My.Settings.SkipSilences
        Select Case My.Settings.FBD_QuickAcess_SubFolders
            Case IO.SearchOption.AllDirectories
                set_fbd_subfolder.IsChecked = False
            Case IO.SearchOption.TopDirectoryOnly
                set_fbd_subfolder.IsChecked = True
        End Select
        set_player_autoplay.IsChecked = My.Settings.Player_AutoPlay
        set_dragdropbehaviour.SelectedIndex = My.Settings.PlaylistDragDropAction
        set_api.IsChecked = My.Settings.API
        set_api_gcalls.IsChecked = My.Settings.API_ALLOWGET
        set_api_log.IsChecked = My.Settings.API_LOG
        set_api_recalls.IsChecked = My.Settings.API_ALLOWEVENTS
        set_api_scalls.IsChecked = My.Settings.API_ALLOWSET
    End Sub
    Private Sub Lib_Loc_Remove_Click(sender As Object, e As RoutedEventArgs) Handles Lib_Loc_Remove.Click
        If Lib_Loc_Cbox.SelectedIndex <> -1 Then
            Try
                Dim Index = Lib_Loc_Cbox.SelectedIndex
                My.Settings.LibrariesPath.RemoveAt(Index)
                My.Settings.Save()
                Lib_Loc_Cbox.Items.RemoveAt(Index)
            Catch ex As Exception
                MessageBox.Show("An unknown error occured" & vbCrLf & "Stack trace: " & vbCrLf & ex.StackTrace, "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Private Sub Lib_Loc_Add_Click(sender As Object, e As RoutedEventArgs) Handles Lib_Loc_Add.Click
        Dim FBD As New Ookii.Dialogs.Wpf.VistaFolderBrowserDialog With {.Description = "Choose a music folder."}
        If FBD.ShowDialog = True Then
            My.Settings.LibrariesPath.Add(FBD.SelectedPath)
            My.Settings.Save()
            Lib_Loc_Cbox.Items.Add(FBD.SelectedPath)
        End If
    End Sub

    Private Sub set_discordrpc_Checked(sender As Object, e As RoutedEventArgs) Handles set_discordrpc.Checked
        My.Settings.UseDiscordRPC = set_discordrpc.IsChecked
        My.Settings.Save()
    End Sub

    Private Sub set_discordrpc_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_discordrpc.Unchecked
        My.Settings.UseDiscordRPC = set_discordrpc.IsChecked
        My.Settings.Save()
    End Sub
    Private Sub set_animations_Checked(sender As Object, e As RoutedEventArgs) Handles set_animations.Checked
        My.Settings.UseAnimations = set_animations.IsChecked
        My.Settings.Save()
    End Sub

    Private Sub set_animations_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_animations.Unchecked
        My.Settings.UseAnimations = set_animations.IsChecked
        My.Settings.Save()
    End Sub

    Private Sub set_background_cover_Checked(sender As Object, e As RoutedEventArgs) Handles set_background_cover.Checked
        My.Settings.BackgroundType = 0
        My.Settings.Save()
        set_background_fancy.IsChecked = False
        With TryCast(Application.Current.MainWindow, MainWindow)
            .StopToken = True
            .Home_FancyBackground.Visibility = Visibility.Hidden
            .Home_Background.Visibility = Visibility.Visible
            .FancyBackgroundManager = Nothing
        End With
    End Sub

    Private Sub set_background_fancy_Checked(sender As Object, e As RoutedEventArgs) Handles set_background_fancy.Checked
        My.Settings.BackgroundType = 1
        My.Settings.Save()
        set_background_cover.IsChecked = False
        With TryCast(Application.Current.MainWindow, MainWindow)
            .Home_FancyBackground.Visibility = Visibility.Visible
            .Home_Background.Visibility = Visibility.Hidden
            .FancyBackgroundManager = New ParticleManager(.FancyCanvas, Me, 500, 5)
            Exit Sub
            .AniCounter = 1
            .StopToken = False
            .DoAnim(.AniCounter)
        End With
    End Sub

    Private Async Sub set_mediabaranimationtype_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_mediabaranimationtype.SelectionChanged
        Select Case CType(set_mediabaranimationtype.SelectedIndex, Mediabar.AnimType)
            Case Mediabar.AnimType.Bottom
                My.Settings.MediaBar_AnimType = Mediabar.AnimType.Bottom
                My.Settings.Save()
            Case Mediabar.AnimType.BottomRight
                My.Settings.MediaBar_AnimType = Mediabar.AnimType.BottomRight
                My.Settings.Save()
            Case Mediabar.AnimType.Top
                My.Settings.MediaBar_AnimType = Mediabar.AnimType.Top
                My.Settings.Save()
            Case Mediabar.AnimType.TopRight
                My.Settings.MediaBar_AnimType = Mediabar.AnimType.TopRight
                My.Settings.Save()
        End Select
        'IDK why but blinking the eye would be awesome !
        test_mediabaranimationtype.Visibility = Visibility.Hidden
        Await Task.Delay(100)
        test_mediabaranimationtype.Visibility = Visibility.Visible
        Await Task.Delay(100)
        test_mediabaranimationtype.Visibility = Visibility.Hidden
        Await Task.Delay(100)
        test_mediabaranimationtype.Visibility = Visibility.Visible
        Await Task.Delay(100)
        test_mediabaranimationtype.Visibility = Visibility.Hidden
        Await Task.Delay(100)
        test_mediabaranimationtype.Visibility = Visibility.Visible
    End Sub

    Private Sub test_mediabaranimationtype_Click(sender As Object, e As RoutedEventArgs) Handles test_mediabaranimationtype.Click
        CType(Application.Current.MainWindow, MainWindow).MediaBar.ShowAnim(My.Settings.MediaBar_AnimType)
    End Sub

    Private Sub set_home_visualiser_Checked(sender As Object, e As RoutedEventArgs) Handles set_home_visualiser.Checked
        My.Settings.Home_ShowVisualiser = True
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).MainUIVisualizerUpdator.Start()
    End Sub

    Private Sub set_home_visualiser_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_home_visualiser.Unchecked
        My.Settings.Home_ShowVisualiser = False
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).MainUIVisualizerUpdator.Stop()
        TryCast(Application.Current.MainWindow, MainWindow).Home_Visualizer.Source = Nothing
    End Sub

    Private Sub set_onmediachange_fadeaudio_Checked(sender As Object, e As RoutedEventArgs) Handles set_onmediachange_fadeaudio.Checked
        My.Settings.OnMediaChange_FadeAudio = True
        My.Settings.Save()
    End Sub

    Private Sub set_onmediachange_fadeaudio_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_onmediachange_fadeaudio.Unchecked
        TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.FadeAudio = False
        My.Settings.OnMediaChange_FadeAudio = False
        My.Settings.Save()
    End Sub

    Private Sub set_ttsspeaknotification_Checked(sender As Object, e As RoutedEventArgs) Handles set_ttsspeaknotification.Checked
        My.Settings.Notificationtts = True
        My.Settings.Save()
    End Sub

    Private Sub set_ttsspeaknotification_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_ttsspeaknotification.Unchecked
        My.Settings.Notificationtts = False
        My.Settings.Save()
    End Sub

    Private Sub set_suppresserrors_Checked(sender As Object, e As RoutedEventArgs) Handles set_suppresserrors.Checked
        My.Settings.SuppressErrors = True
        My.Settings.Save()
    End Sub

    Private Sub set_suppresserrors_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_suppresserrors.Unchecked
        My.Settings.SuppressErrors = False
        My.Settings.Save()
    End Sub

    Private Sub set_onerror_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_onerror.SelectionChanged
        Select Case set_onerror.SelectedIndex
            Case 0 'msg
                My.Settings.OnErrorShow = 0
            Case 1 'inner msg            
                My.Settings.OnErrorShow = 1
            Case 2 'stack trace
                My.Settings.OnErrorShow = 2
            Case 3 'auto
                My.Settings.OnErrorShow = 3
        End Select
        My.Settings.Save()
    End Sub

    Private Sub open_globalhotkeymanager_Click(sender As Object, e As RoutedEventArgs) Handles open_globalhotkeymanager.Click
        My.Windows.GlobalHotkeysManager.ShowDialog()
    End Sub

    Private Sub set_output_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_output.SelectionChanged
        TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.SetOutputDevice(set_output.SelectedIndex)
    End Sub

    Private Sub Refresh_btn_Click(sender As Object, e As RoutedEventArgs) Handles Refresh_btn.Click
        set_output.Items.Clear()
        Dim n = 0
        For Each device In TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.GetOutputDevices
            n += 1
            set_output.Items.Add(n & "-" & device)
        Next
    End Sub
    Private Async Sub Library_OnCountChanged(Count As Integer) Handles Library.OnCountChanged
        Await Dispatcher.InvokeAsync(Sub()
                                         get_libcount.Text = Count
                                         get_libdate.Text = Library.DateCreated
                                     End Sub)
    End Sub

    Private Async Sub set_addtolibray_Click(sender As Object, e As RoutedEventArgs) Handles set_addtolibray.Click
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Multiselect = True, .Filter = Utils.OFDFileFilters}
        If OFD.ShowDialog Then
            overlay_state.Text = "Adding to library..."
            Overlay.Visibility = Visibility.Visible
            Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.AddTracksToLibraryAsync(OFD.FileNames)
            Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheArtists(Utils.AppDataPath)
            Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheYears(Utils.AppDataPath)
            Overlay.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Async Sub set_scanlibrary_Click(sender As Object, e As RoutedEventArgs) Handles set_scanlibrary.Click
        'overlay_state.Text = "Scanning..."
        'Overlay.Visibility = Visibility.Visible
        'Dim Gtracks = Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.GroupTracksAsync
        'Dim files As New List(Of String)
        'For Each path In My.Settings.LibrariesPath
        '    For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, My.Settings.FBD_QuickAcess_SubFolders)).ToArray()
        '        files.Add(song)
        '    Next
        'Next
        'overlay_state.Text = "Adding songs ..."
        'Dim ExcList = files.Except(Gtracks).ToList
        'Dim ExcListR = Gtracks.Except(files).ToList
        'If ExcList.Count <> 0 Then
        '    overlay_state.Text = "Found " & ExcList.Count & " songs new,adding to library..."
        '    Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.AddTracksToLibraryAsync(ExcList)
        'End If
        'If ExcListR.Count <> 0 Then
        '    overlay_state.Text = "Found " & ExcListR.Count & " songs ,removing from library..."
        '    Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.RemoveTracksFromLibraryAsync(ExcListR)
        'End If
        'Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheArtists(Utils.AppDataPath)
        'Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheYears(Utils.AppDataPath)
        'Overlay.Visibility = Visibility.Hidden
        Dim Gtracks = Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.GroupTracksAsync
        Dim files As New List(Of String)
        For Each path In My.Settings.LibrariesPath
            For Each song In Utils.FileFilters.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(path, filter, My.Settings.FBD_QuickAcess_SubFolders)).ToArray()
                files.Add(song)
            Next
        Next
        Dim ExcList = files.Except(Gtracks).ToList
        Dim ExcListR = Gtracks.Except(files).ToList
        If ExcList.Count <> 0 Then
            TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("Scanner", "Found " & ExcList.Count & " songs new,adding to library...", HandyControl.Data.NotifyIconInfoType.Info)
            Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.AddTracksToLibraryAsync(ExcList)
        End If
        If ExcListR.Count <> 0 Then
            TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("Scanner", "Found " & ExcListR.Count & " songs ,removing from library...", HandyControl.Data.NotifyIconInfoType.Info)
            Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.RemoveTracksFromLibraryAsync(ExcListR)
        End If
        Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheArtists(Utils.AppDataPath)
        Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.CacheYears(Utils.AppDataPath)
    End Sub

    Private Async Sub Lib_Refresh_btn_Click(sender As Object, e As RoutedEventArgs) Handles Lib_Refresh_btn.Click
        overlay_state.Text = "Refreshing..."
        Overlay.Visibility = Visibility.Visible
        Await Library.RefreshStats()
        Overlay.Visibility = Visibility.Hidden
    End Sub

    Private Sub set_miniplayersmartcolors_Checked(sender As Object, e As RoutedEventArgs) Handles set_miniplayersmartcolors.Checked
        My.Settings.MiniPlayer_SmartColors = True
        My.Settings.Save()
    End Sub

    Private Sub set_miniplayersmartcolors_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_miniplayersmartcolors.Unchecked
        My.Settings.MiniPlayer_SmartColors = False
        My.Settings.Save()
    End Sub

    Private Sub set_cachelibrarydata_Checked(sender As Object, e As RoutedEventArgs) Handles set_cachelibrarydata.Checked
        My.Settings.CacheLibraryData = True
        My.Settings.Save()
    End Sub

    Private Sub set_cachelibrarydata_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_cachelibrarydata.Unchecked
        My.Settings.CacheLibraryData = False
        My.Settings.Save()
    End Sub

    Private Sub TitleBar_Save_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Save.Click
        My.Settings.Save()
    End Sub

    Private Sub Settings_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Lib_Loc_Cbox.Items.Clear()
        For Each Locations In My.Settings.LibrariesPath
            Lib_Loc_Cbox.Items.Add(Locations)
        Next
        'Setting
        set_discordrpc.IsChecked = My.Settings.UseDiscordRPC
        set_animations.IsChecked = My.Settings.UseAnimations
        Select Case My.Settings.BackgroundType
            Case 0
                set_background_cover.IsChecked = True
            Case 1
                set_background_fancy.IsChecked = True
        End Select
        set_mediabaranimationtype.SelectedIndex = My.Settings.MediaBar_AnimType
        set_home_visualiser.IsChecked = My.Settings.Home_ShowVisualiser
        set_onmediachange_fadeaudio.IsChecked = My.Settings.OnMediaChange_FadeAudio
        set_ttsspeaknotification.IsChecked = My.Settings.Notificationtts
        set_suppresserrors.IsChecked = My.Settings.SuppressErrors
        set_onerror.SelectedIndex = My.Settings.OnErrorShow
        set_output.Items.Clear()
        Dim n = 0
        For Each device In TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.GetOutputDevices
            n += 1
            set_output.Items.Add(n & "-" & device)
        Next
        Library = TryCast(Application.Current.MainWindow, MainWindow).MainLibrary
        If Library IsNot Nothing Then
            get_libcount.Text = Library.Count
            get_libdate.Text = Library.DateCreated
        Else
            get_libcount.Text = "N/A"
            get_libdate.Text = "N/A"
        End If
        set_cachelibrarydata.IsChecked = My.Settings.CacheLibraryData
        Set_UpdatesServer.Text = My.Settings.UpdatesServer
    End Sub
    Private Sub CheckForUpdates_btn_Click(sender As Object, e As RoutedEventArgs) Handles CheckForUpdates_btn.Click
        'Dim Updator As New Updator(My.Settings.UpdatesServer)
        'MsgBox("Current Version: " & Updator.Version.ToString & vbCrLf & "Latest Version: " & Await Updator.CheckForUpdates & vbCrLf & Updator.ChangeLog)
        My.Windows.WUpdator.Show()
    End Sub
    Private Sub Set_UpdatesServer_btn_Click(sender As Object, e As RoutedEventArgs) Handles Set_UpdatesServer_btn.Click
        My.Settings.UpdatesServer = Set_UpdatesServer.Text
        My.Settings.Save()
    End Sub
    Private Sub set_skipsilences_Checked(sender As Object, e As RoutedEventArgs) Handles set_skipsilences.Checked
        My.Settings.SkipSilences = True
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.SkipSilences = True
    End Sub

    Private Sub set_skipsilences_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_skipsilences.Unchecked
        My.Settings.SkipSilences = False
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.SkipSilences = False
    End Sub

    Private Sub set_fbd_subfolder_Checked(sender As Object, e As RoutedEventArgs) Handles set_fbd_subfolder.Checked
        My.Settings.FBD_QuickAcess_SubFolders = IO.SearchOption.TopDirectoryOnly
        My.Settings.Save()
    End Sub

    Private Sub set_fbd_subfolder_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_fbd_subfolder.Unchecked
        My.Settings.FBD_QuickAcess_SubFolders = IO.SearchOption.AllDirectories
        My.Settings.Save()
    End Sub

    Private Sub set_player_autoplay_Checked(sender As Object, e As RoutedEventArgs) Handles set_player_autoplay.Checked
        My.Settings.Player_AutoPlay = True
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.AutoPlay = True
    End Sub

    Private Sub set_player_autoplay_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_player_autoplay.Unchecked
        My.Settings.Player_AutoPlay = False
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).MainPlayer.AutoPlay = False
    End Sub

    Private Sub set_dragdropbehaviour_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_dragdropbehaviour.SelectionChanged
        My.Settings.PlaylistDragDropAction = set_dragdropbehaviour.SelectedIndex
        My.Settings.Save()
    End Sub

    Private Sub set_api_Checked(sender As Object, e As RoutedEventArgs) Handles set_api.Checked
        My.Settings.API = True
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).PIPO = New API
    End Sub

    Private Sub set_api_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_api.Unchecked
        My.Settings.API = False
        My.Settings.Save()
        TryCast(Application.Current.MainWindow, MainWindow).PIPO.Dispose()
        TryCast(Application.Current.MainWindow, MainWindow).PIPO = Nothing
    End Sub

    Private Sub set_api_gcalls_Checked(sender As Object, e As RoutedEventArgs) Handles set_api_gcalls.Checked
        My.Settings.API_ALLOWGET = True
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.AllowGetCalls = True
        End If
    End Sub

    Private Sub set_api_gcalls_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_api_gcalls.Unchecked
        My.Settings.API_ALLOWGET = False
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.AllowGetCalls = False
        End If
    End Sub

    Private Sub set_api_log_Checked(sender As Object, e As RoutedEventArgs) Handles set_api_log.Checked
        My.Settings.API_LOG = True
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.Log = True
        End If
    End Sub

    Private Sub set_api_log_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_api_log.Unchecked
        My.Settings.API_LOG = False
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.Log = False
            End If
    End Sub

    Private Sub set_api_recalls_Checked(sender As Object, e As RoutedEventArgs) Handles set_api_recalls.Checked
        My.Settings.API_ALLOWEVENTS = True
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.AllowRaisingEvents = True
        End If
    End Sub

    Private Sub set_api_recalls_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_api_recalls.Unchecked
        My.Settings.API_ALLOWEVENTS = False
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.AllowRaisingEvents = False
        End If
    End Sub

    Private Sub set_api_scalls_Checked(sender As Object, e As RoutedEventArgs) Handles set_api_scalls.Checked
        My.Settings.API_ALLOWSET = True
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.AllowSetCalls = True
        End If
    End Sub

    Private Sub set_api_scalls_Unchecked(sender As Object, e As RoutedEventArgs) Handles set_api_scalls.Unchecked
        My.Settings.API_ALLOWSET = False
        My.Settings.Save()
        If TryCast(Application.Current.MainWindow, MainWindow).PIPO IsNot Nothing Then
            TryCast(Application.Current.MainWindow, MainWindow).PIPO.AllowSetCalls = False
            End If
    End Sub
End Class
