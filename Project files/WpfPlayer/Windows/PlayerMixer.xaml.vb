Imports System.ComponentModel
Imports System.Windows.Interop

Public Class PlayerMixer
    Dim WithEvents UiManager As New Forms.Timer With {.Interval = 33}
    Dim WithEvents PlayerOne As Player = Nothing
    Dim WithEvents PlayerTwo As Player = Nothing
    Private Sub DummyMediaEnded()
        'MsgBox("done")
    End Sub
    Private Sub PlayerMixer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Player1_Cover.RenderTransform = New RotateTransform
        Player2_Cover.RenderTransform = New RotateTransform
        UiManager.Start()
    End Sub
    Private Sub Main_CB_Players_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Main_CB_Players.SelectionChanged
        Dim MixerItem As New PlayerMixerItem(TryCast(Owner, MainWindow).MixerPlayers.Item(Main_CB_Players.SelectedIndex)) With {.Title = "Player[" & Main_CB_Players.SelectedIndex & "]"}
        MixerItem.Show()
    End Sub

    Private Sub Main_PlayersAdd_Click(sender As Object, e As RoutedEventArgs) Handles Main_PlayersAdd.Click
        TryCast(Owner, MainWindow).MixerPlayers.Add(New Player(Owner, AddressOf DummyMediaEnded))
        Main_CB_Players.Items.Add("N/A [" & Main_CB_Players.Items.Count & "]")
    End Sub

    Private Sub Main_PlayersRemove_Click(sender As Object, e As RoutedEventArgs) Handles Main_PlayersRemove.Click
        TryCast(Owner, MainWindow).MixerPlayers.Item(Main_CB_Players.SelectedIndex).Dispose()
        TryCast(Owner, MainWindow).MixerPlayers.RemoveAt(Main_CB_Players.SelectedIndex)
        Main_CB_Players.Items.RemoveAt(Main_CB_Players.SelectedIndex)
    End Sub

    Private Sub Main_PlayersRefresh_Click(sender As Object, e As RoutedEventArgs) Handles Main_PlayersRefresh.Click
        Dim i = 0
        For Each Player In TryCast(Owner, MainWindow).MixerPlayers
            Main_CB_Players.Items.Add(Player.CurrentMediaTitle & " [" & i & "]")
            i += 1
        Next
    End Sub

    Private Sub PlayerMixer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub

    Private Sub UiManager_Tick(sender As Object, e As EventArgs) Handles UiManager.Tick
        Try
            'Player1_Pos.Text = New Date(TimeSpan.FromSeconds(PlayerOne.GetPosition).Ticks).ToString("HH:mm:ss.fff")
            Player1_Pos.Text = New Date(Utils.SecsToMs(PlayerOne.GetPrecisePosition).Ticks).ToString("HH:mm:ss.fff")
        Catch ex As Exception
        End Try
        Try
            'Player2_Pos.Text = New Date(TimeSpan.FromSeconds(PlayerTwo.GetPosition).Ticks).ToString("hh:mm:ss.ffff")
            Player2_Pos.Text = New Date(Utils.SecsToMs(PlayerTwo.GetPrecisePosition).Ticks).ToString("HH:mm:ss.fff")
        Catch ex As Exception
        End Try
        'Vol1.Fill = Lime 0-255-0
        'Vol4.Fill = Orange 255-139-0
        'Vol5.Fill = Red 255-0-0
        Try
            Select Case Utils.PercentageToFiveMax(Utils.ValToPercentage(PlayerOne.GetPeak.Master, 0, 32))
                Case 0
                    Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 1
                    Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 2
                    Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 3
                    Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 4
                    Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(255, 139, 0))
                    Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 5
                    Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(255, 139, 0))
                    Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(255, 0, 0))
            End Select
        Catch ex As Exception
            Player1_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
            Player1_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
            Player1_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
            Player1_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
            Player1_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
        End Try
        Try
            Select Case Utils.PercentageToFiveMax(Utils.ValToPercentage(PlayerTwo.GetPeak.Master, 0, 32))
                Case 0
                    Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 1
                    Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 2
                    Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
                    Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 3
                    Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
                    Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 4
                    Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(255, 139, 0))
                    Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
                Case 5
                    Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
                    Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(255, 139, 0))
                    Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(255, 0, 0))
            End Select
        Catch ex As Exception
            Player2_Vol_1.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
            Player2_Vol_2.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
            Player2_Vol_3.Fill = New SolidColorBrush(Color.FromRgb(0, 155, 0))
            Player2_Vol_4.Fill = New SolidColorBrush(Color.FromRgb(155, 39, 0))
            Player2_Vol_5.Fill = New SolidColorBrush(Color.FromRgb(155, 0, 0))
        End Try
    End Sub

    Private Sub Main_PlayerSendToOne_Click(sender As Object, e As RoutedEventArgs) Handles Main_PlayerSendToOne.Click
        PlayerOne = TryCast(Owner, MainWindow).MixerPlayers(Main_CB_Players.SelectedIndex)
        Player1_Cover.Fill = New ImageBrush(PlayerOne.CurrentMediaCover)
        Player1_Vol.Value = PlayerOne.GetPosition * 100
    End Sub

    Private Sub Main_PlayerSendToTwo_Click(sender As Object, e As RoutedEventArgs) Handles Main_PlayerSendToTwo.Click
        PlayerTwo = TryCast(Owner, MainWindow).MixerPlayers(Main_CB_Players.SelectedIndex)
        Player2_Cover.Fill = New ImageBrush(PlayerTwo.CurrentMediaCover)
        Player2_Vol.Value = PlayerTwo.GetPosition * 100
    End Sub

    Private Sub PlayerOne_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles PlayerOne.MediaLoaded
        Player1_Cover.Fill = New ImageBrush(Cover)
    End Sub

    Private Sub PlayerOne_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles PlayerOne.VolumeChanged
        Player1_Vol.Value = NewVal * 100
    End Sub
    Private Sub PlayerOne_PlayerStateChanged(State As Player.State) Handles PlayerOne.PlayerStateChanged
        If State = Player.State.Playing Then
            Player1_Cover.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, New Animation.DoubleAnimation(360, Duration.Forever))
        Else
            Player1_Cover.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, Nothing)
        End If
    End Sub
    Private Sub PlayerTwo_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles PlayerTwo.MediaLoaded
        Player2_Cover.Fill = New ImageBrush(Cover)
    End Sub

    Private Sub PlayerTwo_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles PlayerTwo.VolumeChanged
        Player2_Vol.Value = NewVal * 100
    End Sub
    Private Sub PlayerTwo_PlayerStateChanged(State As Player.State) Handles PlayerTwo.PlayerStateChanged
        If State = Player.State.Playing Then
            Player2_Cover.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, New Animation.DoubleAnimation(360, Duration.Forever))
        Else
            Player2_Cover.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, Nothing)
        End If
    End Sub

    Private Sub Player1_Pause_Click(sender As Object, e As RoutedEventArgs) Handles Player1_Pause.Click
        PlayerOne.StreamPause()
    End Sub

    Private Sub Player1_Play_Click(sender As Object, e As RoutedEventArgs) Handles Player1_Play.Click
        PlayerOne.StreamPlay()
    End Sub

    Private Sub Player1_SyncToTwo_Click(sender As Object, e As RoutedEventArgs) Handles Player1_SyncToTwo.Click
        PlayerOne.SetPosition(PlayerTwo.GetPosition)
    End Sub

    Private Sub Player1_Block_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Player1_Block.MouseDown
        PlayerOne.StreamPause()
    End Sub

    Private Sub Player1_Block_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Player1_Block.MouseUp
        PlayerOne.StreamPlay()
    End Sub

    Private Sub Player1_Vol_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Player1_Vol.ValueChanged
        PlayerOne.SetVolume(Player1_Vol.Value / 100)
    End Sub

    Private Sub Player2_Pause_Click(sender As Object, e As RoutedEventArgs) Handles Player2_Pause.Click
        PlayerTwo.StreamPause()
    End Sub

    Private Sub Player2_Play_Click(sender As Object, e As RoutedEventArgs) Handles Player2_Play.Click
        PlayerTwo.StreamPlay()
    End Sub

    Private Sub Player2_SyncToOne_Click(sender As Object, e As RoutedEventArgs) Handles Player2_SyncToOne.Click
        PlayerTwo.SetPosition(PlayerOne.GetPosition)
    End Sub

    Private Sub Player2_Block_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Player2_Block.MouseDown
        PlayerTwo.StreamPause()
    End Sub

    Private Sub Player2_Block_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Player2_Block.MouseUp
        PlayerOne.StreamPlay()
    End Sub

    Private Sub Player2_Vol_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Player2_Vol.ValueChanged
        PlayerTwo.SetVolume(Player2_Vol.Value / 100)
    End Sub

    Private Sub Player_Mix_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Player_Mix.ValueChanged
        '>0    --> Mix -1-Right-2-Left
        '=100  --> Mix -1-Center-2-Center
        '<0    --> Mix -1-Left-2-Right
        '=-100 --> Mix -1-Left-2-Right
        Try
            If Utils.IsInRange(Player_Mix.Value, 0, 100, False, True) Then

            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
