Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Interop
Imports WpfPlayer

Public Class FullScreenPlayer
    Dim MainWND As MainWindow = TryCast(Application.Current.MainWindow, MainWindow)
    Dim WithEvents LinkPlayer As Player = MainWND.MainPlayer
    Dim WithEvents LinkPlaylist As Playlist = MainWND.MainPlaylist
    Dim WithEvents UiManager As New Forms.Timer With {.Interval = 500}
    Private ResPlay As New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/wplay.png"))
    Private ResPause As New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/wpause.png"))
    Private Sub FullScreenPlayer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub

    Private Sub FullScreenPlayer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Main_Track.Maximum = LinkPlayer.GetLength
        Main_Len.Text = Utils.SecsToMins(LinkPlayer.GetLength)
        If LinkPlayer.PlayerState = Player.State.Playing Then
            Main_BT_PlayPause.Background = New ImageBrush(ResPause)
        Else
            Main_BT_PlayPause.Background = New ImageBrush(ResPlay)
        End If
        Main_Cover.Source = LinkPlayer.CurrentMediaCover
        For i As Integer = 0 To LinkPlaylist.Playlist.Count - 1
            Dim _i = i
            Dim pinfo = Utils.GetSongInfo(LinkPlaylist.GetItem(i))
            Main_Playlist.Children.Add(New FullScreenPlaylistItem(i + 1, pinfo(1), pinfo(0), LinkPlaylist.GetItem(i), Nothing) With {.Margin = New Thickness(0, 10, 0, 0), .OnClick = Sub()
                                                                                                                                                                                          LinkPlayer.LoadSong(MainWND.MainPlaylist.JumpTo(_i), Nothing, False)
                                                                                                                                                                                      End Sub})
        Next
        UiManager.Start()
    End Sub
#Region "Player Stuff"
    Private Async Sub LinkPlayer_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles LinkPlayer.MediaLoaded
        Main_Cover.Source = Cover
        Main_BG.Source = Cover
        If IsActive Then
            Await Task.Delay(100)
            Main_Cover.BeginAnimation(MarginProperty, New Animation.ThicknessAnimation(New Thickness(0, 0, -100, 0), New Duration(TimeSpan.FromMilliseconds(10))))
            Await Task.Delay(10)
            Main_Cover.BeginAnimation(MarginProperty, New Animation.ThicknessAnimation(New Thickness(0, 0, 100, 0), New Duration(TimeSpan.FromMilliseconds(100))))
            Main_Cover.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(100))))
        Else
            Main_Cover.Margin = New Thickness(0, 0, 100, 0)
            Main_Cover.Opacity = 1
        End If
    End Sub
    Public Sub LinkPlayer_MediaEnded()
        If IsActive Then
            Main_Cover.BeginAnimation(MarginProperty, New Animation.ThicknessAnimation(New Thickness(0, 0, 300, 0), New Duration(TimeSpan.FromMilliseconds(100))))
            Main_Cover.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(100))))
        End If
    End Sub
#Region "Playlist Stuff -_-"
    Private Sub LinkPlaylist_OnSongAdd(Value As String, Type As Player.StreamTypes, IndexUpdated As Boolean, UseURL As Boolean, URL As String, OverrideCurrentMedia As Boolean, OCMTitle As String, OCMArtist As String, OCMYear As Integer, OCMCover As Bitmap) Handles LinkPlaylist.OnSongAdd
        Dim pinfo = Utils.GetSongInfo(LinkPlaylist.GetItem(Value))
        Main_Playlist.Children.Add(New FullScreenPlaylistItem(Main_Playlist.Children.Count + 1, pinfo(1), pinfo(0), Value, Nothing) With {.Margin = New Thickness(0, 10, 0, 0), .OnClick = Sub()
                                                                                                                                                                                               LinkPlayer.LoadSong(MainWND.MainPlaylist.JumpTo(Main_Playlist.Children.Count), Nothing, False)
                                                                                                                                                                                           End Sub})
    End Sub

    Private Sub LinkPlaylist_OnSongRemove(Value As String, Index As Integer) Handles LinkPlaylist.OnSongRemove
        Main_Playlist.Children.RemoveAt(Index)
    End Sub

    Private Sub LinkPlaylist_OnSongInsert(Value As String, Type As Player.StreamTypes, IndexUpdated As Boolean, UseURL As Boolean, URL As String, OverrideCurrentMedia As Boolean, OCMTitle As String, OCMArtist As String, OCMYear As Integer, OCMCover As Bitmap, Index As Integer) Handles LinkPlaylist.OnSongInsert
        Dim pinfo = Utils.GetSongInfo(LinkPlaylist.GetItem(Value))
        Main_Playlist.Children.Insert(Index, New FullScreenPlaylistItem(Main_Playlist.Children.Count + 1, pinfo(1), pinfo(0), Value, Nothing) With {.Margin = New Thickness(0, 10, 0, 0), .OnClick = Sub()
                                                                                                                                                                                                         LinkPlayer.LoadSong(MainWND.MainPlaylist.JumpTo(Main_Playlist.Children.Count), Nothing, False)
                                                                                                                                                                                                     End Sub})
    End Sub
    Private Sub LinkPlaylist_OnPlaylistClear() Handles LinkPlaylist.OnPlaylistClear
        Main_Playlist.Children.Clear()
    End Sub

    Private Sub LinkPlaylist_OnIndexChanged(Index As Integer) Handles LinkPlaylist.OnIndexChanged
        For Each fspi As FullScreenPlaylistItem In Main_Playlist.Children
            fspi.UnSelectItem()
        Next
        Try
            TryCast(Main_Playlist.Children.Item(Index), FullScreenPlaylistItem).SelectItem()
        Catch ex As Exception
        End Try
    End Sub
#End Region
    Private Sub LinkPlayer_OnRepeatChanged(NewType As Player.RepeateBehaviour) Handles LinkPlayer.OnRepeatChanged
        Select Case NewType
            Case Player.RepeateBehaviour.NoRepeat
                Main_BT_Loop.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wno-loop.png")))
            Case Player.RepeateBehaviour.RepeatOne
                Main_BT_Loop.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wloop-one.png")))
            Case Player.RepeateBehaviour.RepeatAll
                Main_BT_Loop.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wloop.png")))
        End Select
    End Sub

    Private Sub LinkPlayer_OnShuffleChanged(NewType As Player.RepeateBehaviour) Handles LinkPlayer.OnShuffleChanged
        Select Case NewType
            Case Player.RepeateBehaviour.NoShuffle
                Main_BT_Shuffle.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wno-shuffle.png")))
            Case Player.RepeateBehaviour.Shuffle
                Main_BT_Shuffle.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wshuffle.png")))
        End Select
    End Sub

    Private Sub LinkPlayer_PlayerStateChanged(State As Player.State) Handles LinkPlayer.PlayerStateChanged
        Select Case State
            Case Player.State.Playing
                Main_BT_PlayPause.Background = New ImageBrush(ResPause)
            Case Else
                Main_BT_PlayPause.Background = New ImageBrush(ResPlay)
        End Select
    End Sub

    Private Sub LinkPlayer_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles LinkPlayer.VolumeChanged
        If IsMuted Then
            Main_BT_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wNvol.png")))
        Else
            If NewVal * 100 = 0 Then
                Main_BT_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wNvol.png")))
            ElseIf NewVal * 100 > 0 AndAlso NewVal * 100 < 50 Then
                Main_BT_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wLvol.png")))
            ElseIf NewVal * 100 >= 50 AndAlso NewVal * 100 < 100 Then
                Main_BT_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wMivol.png")))
            ElseIf NewVal * 100 = 100 Then
                Main_BT_Volume.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/Res/wFvol.png")))
            End If
        End If
    End Sub
#End Region
    Private Sub UiManager_Tick(sender As Object, e As EventArgs) Handles UiManager.Tick
        If Main_Track.IsMouseOver = False Then
            Main_Track.Value = LinkPlayer.GetPosition
            Main_Pos.Text = Utils.SecsToMins(Main_Track.Value)
        End If
        Main_Pos.Text = Utils.SecsToMins(LinkPlayer.GetPosition)
    End Sub

    Private Sub Main_BT_PlayPause_Click(sender As Object, e As RoutedEventArgs) Handles Main_BT_PlayPause.Click
        Select Case LinkPlayer.PlayerState
            Case Player.State.Playing
                LinkPlayer.StreamPause()
            Case Else
                LinkPlayer.StreamPlay()
        End Select
    End Sub

    Private Sub Main_BT_Next_Click(sender As Object, e As RoutedEventArgs) Handles Main_BT_Next.Click
        MainWND.media_next_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Main_BT_Prev_Click(sender As Object, e As RoutedEventArgs) Handles Main_BT_Prev.Click
        MainWND.media_prev_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Main_Logo_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles Main_Logo.MouseLeftButtonUp
        Close()
    End Sub

    Private Sub Main_Volume_G_MouseEnter(sender As Object, e As MouseEventArgs) Handles Main_Volume_G.MouseEnter
        Main_Volume_G.Opacity = 1
    End Sub

    Private Sub Main_Volume_G_MouseLeave(sender As Object, e As MouseEventArgs) Handles Main_Volume_G.MouseLeave
        Main_Volume_G.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(100))))
    End Sub

    Private Sub Main_BT_Volume_MouseEnter(sender As Object, e As MouseEventArgs) Handles Main_BT_Volume.MouseEnter
        Main_Volume_G.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(100))))
    End Sub

    Private Sub Main_Volume_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Volume.ValueChanged
        LinkPlayer.SetVolume(Main_Volume.Value / 100)
    End Sub
    Private Sub Main_BT_Shuffle_Click(sender As Object, e As RoutedEventArgs) Handles Main_BT_Shuffle.Click
        Select Case LinkPlayer.Shuffle
            Case Player.RepeateBehaviour.NoShuffle
                LinkPlayer.Shuffle = Player.RepeateBehaviour.Shuffle
            Case Player.RepeateBehaviour.Shuffle
                LinkPlayer.Shuffle = Player.RepeateBehaviour.NoShuffle
        End Select
    End Sub

    Private Sub Main_BT_Loop_Click(sender As Object, e As RoutedEventArgs) Handles Main_BT_Loop.Click
        Select Case LinkPlayer.RepeateType
            Case Player.RepeateBehaviour.NoRepeat
                LinkPlayer.RepeateType = Player.RepeateBehaviour.RepeatOne
            Case Player.RepeateBehaviour.RepeatOne
                LinkPlayer.RepeateType = Player.RepeateBehaviour.RepeatAll
            Case Player.RepeateBehaviour.RepeatAll
                LinkPlayer.RepeateType = Player.RepeateBehaviour.NoRepeat
        End Select
    End Sub

    Private Sub Main_Track_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Track.ValueChanged
        If Main_Track.IsMouseOver Then
            LinkPlayer.SetPosition(Main_Track.Value)
        End If
    End Sub


End Class
