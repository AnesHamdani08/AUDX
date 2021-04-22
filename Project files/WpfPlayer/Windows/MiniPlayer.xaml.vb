Imports System.ComponentModel
Imports System.Windows.Interop
Imports Microsoft.WindowsAPICodePack.Taskbar
Imports WpfPlayer

Public Class MiniPlayer
    Private WithEvents Player As Player
    Private WithEvents Playlist As Playlist
    Private PlaylistItems As ObjectModel.ObservableCollection(Of PlaylistItem)
    Private WithEvents UIManager As New Threading.DispatcherTimer With {.IsEnabled = True, .Interval = TimeSpan.FromMilliseconds(100)}
    Private UpdatePlaylist As Boolean = True
    Private WithEvents TaskBarPrev As ThumbnailToolBarButton
    Private WithEvents TaskBarPlayPause As ThumbnailToolBarButton
    Private WithEvents TaskBarNext As ThumbnailToolBarButton
    Private PlayIcon = TryCast(Application.Current.MainWindow, MainWindow).Playicon
    Private PauseIcon = TryCast(Application.Current.MainWindow, MainWindow).Pauseicon
    Public Sub UpdateSkin(ByVal skin As HandyControl.Data.SkinType)
        HandyControl.Themes.SharedResourceDictionary.SharedDictionaries.Clear()
        Resources.MergedDictionaries.Add(HandyControl.Tools.ResourceHelper.GetSkin(skin))
        Resources.MergedDictionaries.Add(New ResourceDictionary With {.Source = New Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")})
        OnApplyTemplate()
    End Sub
    Private Sub MiniPlayer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Media_Title.Text = Player.CurrentMediaTitle
        Media_Artist.Text = Player.CurrentMediaArtist
        Media_Cover.Source = Player.CurrentMediaCover
        Media_Seek.Maximum = Player.GetLength
        Media_Length.Text = Utils.SecsToMins(Player.GetLength)
        Dim helper = New WindowInteropHelper(Me)
        TaskBarPrev = New ThumbnailToolBarButton(TryCast(Application.Current.MainWindow, MainWindow).Previcon, "Previous")
        TaskBarPlayPause = New ThumbnailToolBarButton(TryCast(Application.Current.MainWindow, MainWindow).Playicon, "Play")
        TaskBarNext = New ThumbnailToolBarButton(TryCast(Application.Current.MainWindow, MainWindow).Nexticon, "Next")
        TaskbarManager.Instance.ThumbnailToolBars.AddButtons(helper.Handle, {TaskBarPrev, TaskBarPlayPause, TaskBarNext})
        TaskbarManager.Instance.TabbedThumbnail.SetThumbnailClip(helper.Handle, New System.Drawing.Rectangle(Media_Cover.Margin.Left + 5, Media_Cover.Margin.Top + 30, Media_Cover.Width, Media_Cover.Height))
        UpdateSkin(My.Settings.DefaultTheme)
        If My.Settings.MiniPlayer_SmartColors Then
            Dim Clr = Utils.GetAverageColor(Player.CurrentMediaCover)
            Dim iClr = Utils.GetInverseColor(Clr)
            Dim ClrBrush = New SolidColorBrush(Clr)
            Dim iClrBrush = New SolidColorBrush(iClr)
            Background = ClrBrush
            NonClientAreaBackground = ClrBrush
            NonClientAreaForeground = iClrBrush
            Media_Title.Foreground = iClrBrush
            Media_Artist.Foreground = iClrBrush
            Media_Pos.Foreground = iClrBrush
            Media_Length.Foreground = iClrBrush
        End If
    End Sub
    Private Sub MiniPlayer_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Player = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
        Playlist = TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist
        PlaylistItems = TryCast(Application.Current.MainWindow, MainWindow).playlistItems
        Playlist_Main.ItemsSource = PlaylistItems
        Player_PlayerStateChanged(Player.PlayerState)
    End Sub
    Private Sub MiniPlayer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Media_Title.Visibility = Visibility.Hidden
        Media_Artist.Visibility = Visibility.Hidden
        Media_PlayPause.Visibility = Visibility.Visible
        Media_Next.Visibility = Visibility.Visible
        Media_Previous.Visibility = Visibility.Visible
        Player.ReSendPlayStateChangedEvent(Player.PlayerState, 10)
    End Sub

    Private Sub MiniPlayer_Deactivated(sender As Object, e As EventArgs) Handles Me.Deactivated
        Media_Title.Visibility = Visibility.Visible
        Media_Artist.Visibility = Visibility.Visible
        Media_PlayPause.Visibility = Visibility.Hidden
        Media_Next.Visibility = Visibility.Hidden
        Media_Previous.Visibility = Visibility.Hidden
    End Sub
    Private Sub MiniPlayer_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered

    End Sub
    Private Sub Player_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles Player.MediaLoaded
        Media_Title.Text = Title
        Media_Artist.Text = Artist
        Media_Cover.Source = Cover
        Media_Length.Text = Utils.SecsToMins(Player.GetLength)
        Media_Seek.Maximum = Player.GetLength
        If My.Settings.MiniPlayer_SmartColors Then
            Dim Clr = Utils.GetAverageColor(Thumb)
            Dim iClr = Utils.GetInverseColor(Clr)
            Dim ClrBrush = New SolidColorBrush(Clr)
            Dim iClrBrush = New SolidColorBrush(iClr)
            Background = ClrBrush
            NonClientAreaBackground = ClrBrush
            NonClientAreaForeground = iClrBrush
            Media_Title.Foreground = iClrBrush
            Media_Artist.Foreground = iClrBrush
            Media_Pos.Foreground = iClrBrush
            Media_Length.Foreground = iClrBrush
        End If
    End Sub
    Private Sub Player_PlayerStateChanged(State As Player.State) Handles Player.PlayerStateChanged
        If Visibility = Visibility.Visible Then
            Try
                Select Case State
                    Case Player.State.MediaLoaded
                        TaskbarManager.Instance.SetOverlayIcon(Me, TryCast(Application.Current.MainWindow, MainWindow).Loadingicon, "Loading")
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Me)
                    Case Player.State.Paused
                        Media_PlayPause.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/play.png")))
                        If TaskBarPlayPause IsNot Nothing Then
                            TaskBarPlayPause.Icon = PlayIcon
                        End If
                        TaskbarManager.Instance.SetOverlayIcon(Me, TryCast(Application.Current.MainWindow, MainWindow).Pauseicon, "Paused")
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused, Me)
                    Case Player.State.Playing
                        Media_PlayPause.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/pause.png")))
                        If TaskBarPlayPause IsNot Nothing Then
                            TaskBarPlayPause.Icon = PauseIcon
                        End If
                        TaskbarManager.Instance.SetOverlayIcon(Me, TryCast(Application.Current.MainWindow, MainWindow).Playicon, "Playing")
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal, Me)
                    Case Player.State.Stopped
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, Me)
                        TaskbarManager.Instance.SetOverlayIcon(Me, Nothing, Nothing)
                    Case Player.State.Undefined
                        Try
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, Me)
                        Catch ex As Exception
                        End Try
                    Case Player.State._Error
                        Media_PlayPause.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/cross-flat.png")))
                End Select
            Catch ex As Exception
                Player.ReSendPlayStateChangedEvent(State, 1000)
            End Try
        End If
    End Sub

    Private Sub UIManager_Tick(sender As Object, e As EventArgs) Handles UIManager.Tick
        Media_Pos.Text = Utils.SecsToMins(Player.GetPosition)
        If Not Media_Seek.IsMouseOver Then
            Media_Seek.Value = Player.GetPosition
        End If
        If Player.PlayerState <> Player.State.Undefined Then
            Try
                TaskbarManager.Instance.SetProgressValue(Player.GetPosition, Player.GetLength, me)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub Media_PlayPause_Click(sender As Object, e As RoutedEventArgs) Handles Media_PlayPause.Click
        TryCast(Application.Current.MainWindow, MainWindow).media_play_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Media_Previous_Click(sender As Object, e As RoutedEventArgs) Handles Media_Previous.Click
        TryCast(Application.Current.MainWindow, MainWindow).media_prev_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Media_Next_Click(sender As Object, e As RoutedEventArgs) Handles Media_Next.Click
        TryCast(Application.Current.MainWindow, MainWindow).media_next_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub MiniPlayer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try
            Me.Hide()
            TryCast(Application.Current.MainWindow, MainWindow).Show()
            e.Cancel = True
        Catch ex As Exception
            e.Cancel = False
        End Try
    End Sub

    Private Sub Media_Volume_Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Media_Volume_Slider.ValueChanged
        Try
            Player.SetVolume(Media_Volume_Slider.Value / 100)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Player_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles Player.VolumeChanged
        If IsMuted Then
            Media_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mvol.png")))
        Else
            If NewVal * 100 = 0 Then
                Media_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mvol.png")))
            ElseIf NewVal * 100 > 0 AndAlso NewVal * 100 < 50 Then
                Media_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Lvol.png")))
            ElseIf NewVal * 100 >= 50 AndAlso NewVal * 100 < 100 Then
                Media_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Mivol.png")))
            ElseIf NewVal * 100 = 100 Then
                Media_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/Fvol.png")))
            End If
        End If
    End Sub

    Private Sub Media_Volume_Click(sender As Object, e As RoutedEventArgs) Handles Media_Volume.Click
        Player.Mute = True
    End Sub

    Private Sub Media_Seek_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Media_Seek.ValueChanged
        If Media_Seek.IsMouseOver Then
            Try
                Player.SetPosition(Media_Seek.Value)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Async Sub Playlist_Main_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Playlist_Main.SelectionChanged
        Try
            If UpdatePlaylist Then
                Select Case PlaylistItems(Playlist_Main.SelectedIndex).Type
                    Case Player.StreamTypes.Local
                        Player.LoadSong(Playlist.JumpTo(Playlist_Main.SelectedIndex), Playlist, False)
                        Player.StreamPlay()
                    Case Player.StreamTypes.URL
                        Playlist_Main.IsEnabled = False
                        'Await Player.LoadStreamAsync(Playlist.JumpTo(Playlist.Playlist_Main.SelectedIndex), Player.StreamTypes.URL, Playlist, Nothing, False)
                        Dim _pitem = PlaylistItems(Playlist_Main.SelectedIndex)
                        Player.LoadSong(Nothing, Playlist, False, True, True, Playlist.JumpTo(Playlist_Main.SelectedIndex), True, _pitem.Title, _pitem.Artist, _pitem.Cover, _pitem.Year, Nothing)
                        _pitem = Nothing
                        Player.StreamPlay()
                        Playlist_Main.IsEnabled = True
                    Case Player.StreamTypes.Youtube
                        Playlist_Main.IsEnabled = False
                        Await Player.LoadStreamAsync(Playlist.JumpTo(Playlist_Main.SelectedIndex), Player.StreamTypes.Youtube, Playlist, Nothing, False)
                        Player.StreamPlay()
                        Playlist_Main.IsEnabled = True
                End Select
            Else
                UpdatePlaylist = True
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Media_Cover_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Media_Cover.MouseDown
        If e.ClickCount = 2 Then
            Anim_Switch.IsChecked = True
        End If
    End Sub

    Private Sub Playlist_Hide_Click(sender As Object, e As RoutedEventArgs) Handles Playlist_Hide.Click
        Anim_Switch.IsChecked = False

    End Sub

    Private Sub Playlist_OnIndexChanged(Index As Integer) Handles Playlist.OnIndexChanged
        UpdatePlaylist = False
        Playlist_Main.SelectedIndex = Index
        UpdatePlaylist = True
    End Sub

    Private Sub TaskBarPlayPause_Click(sender As Object, e As ThumbnailButtonClickedEventArgs) Handles TaskBarPlayPause.Click
        Media_PlayPause_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub TaskBarNext_Click(sender As Object, e As ThumbnailButtonClickedEventArgs) Handles TaskBarNext.Click
        Media_Next_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub TaskBarPrev_Click(sender As Object, e As ThumbnailButtonClickedEventArgs) Handles TaskBarPrev.Click
        Media_Previous_Click(Nothing, New RoutedEventArgs)
    End Sub
End Class
