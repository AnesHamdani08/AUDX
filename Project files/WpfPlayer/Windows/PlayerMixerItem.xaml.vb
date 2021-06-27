Imports System.Windows.Interop

Public Class PlayerMixerItem
    Public WithEvents Player As Player
    Dim WithEvents UIManager As New Forms.Timer With {.Interval = 500}
    Public Sub New(ByRef _Player As Player)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If _Player Is Nothing Then
            Close()
        Else
            Player = _Player
            Player.ReSendPlayStateChangedEvent(Player.PlayerState, 10)
            Main_Slider.Maximum = Player.GetLength
            Main_Title.Text = Player.CurrentMediaTitle
            Main_Artist.Text = Player.CurrentMediaArtist
            Main_Cover.Source = Player.CurrentMediaCover
            Main_Slider_Volume.Value = Player.GetVolume * 100
            Main_Slider_Balance.Value = Player.GetBalance * 100
            UIManager.Start()
        End If
    End Sub

    Private Sub UIManager_Tick(sender As Object, e As EventArgs) Handles UIManager.Tick
        Try
            If Not Main_Slider.IsMouseOver Then
                Main_Slider.Value = Player.GetPosition
            End If
        Catch
        End Try
    End Sub
    Private Sub Player_PlayerStateChanged(State As Player.State) Handles Player.PlayerStateChanged
        Try
            Select Case State
                Case Player.State.MediaLoaded
                    Main_play_btn.Visibility = Visibility.Hidden
                Case Player.State.Paused
                    Main_play_btn.Visibility = Visibility.Visible
                    Try
                        If My.Settings.USEANIMATIONS Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 1
                            Danim.To = 0
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, Main_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            Main_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        Main_play_btn.Opacity = 1
                    End Try
                    Main_play_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/play.png")))
                    Try
                        If My.Settings.USEANIMATIONS Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 0
                            Danim.To = 1
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, Main_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            Main_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        Main_play_btn.Opacity = 1
                    End Try
                Case Player.State.Playing
                    Main_play_btn.Visibility = Visibility.Visible
                    Try
                        If My.Settings.USEANIMATIONS Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 1
                            Danim.To = 0
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, Main_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            Main_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        Main_play_btn.Opacity = 1
                    End Try
                    Main_play_btn.Background = New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/pause.png")))
                    Try
                        If My.Settings.USEANIMATIONS Then
                            Dim Danim As New Animation.DoubleAnimation
                            Danim.From = 0
                            Danim.To = 1
                            Danim.Duration = TimeSpan.FromSeconds(0.5)
                            Animation.Storyboard.SetTarget(Danim, Main_play_btn)
                            Animation.Storyboard.SetTargetProperty(Danim, New PropertyPath("Opacity"))
                            Dim sb As New Animation.Storyboard
                            sb.Children.Add(Danim)
                            sb.Begin()
                        Else
                            Main_play_btn.Opacity = 1
                        End If
                    Catch ex As Exception
                        Main_play_btn.Opacity = 1
                    End Try
                Case Player.State.Stopped
                    Main_play_btn.Visibility = Visibility.Visible
                Case Player.State.Undefined
                    Main_play_btn.Visibility = Visibility.Hidden
                Case Player.State._Error
                    Main_play_btn.Visibility = Visibility.Hidden
            End Select
        Catch ex As Exception
            Player.ReSendPlayStateChangedEvent(State, 1000)
        End Try
    End Sub

    Private Sub Player_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles Player.VolumeChanged
        Main_Slider_Volume.Value = NewVal
    End Sub

    Private Sub Player_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles Player.MediaLoaded
        Main_Title.Text = Title
        Main_Artist.Text = Artist
        Main_Cover.Source = Cover
    End Sub

    Private Sub Main_Cover_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Main_Cover.MouseDown
        If e.ClickCount = 2 Then
            Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Filter = Utils.FileFilters}
            If OFD.ShowDialog Then
                Player.LoadSong(OFD.FileName, Nothing, False)
            End If
        End If
    End Sub

    Private Sub Main_Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Slider.ValueChanged
        If Main_Slider.IsMouseOver Then
            Player.SetPosition(Main_Slider.Value)
        End If
    End Sub

    Private Sub Main_play_btn_Click(sender As Object, e As RoutedEventArgs) Handles Main_play_btn.Click
        If Player.PlayerState = Player.State.Playing Then
            Player.StreamPause()
        Else
            Player.StreamPlay()
        End If
    End Sub

    Private Sub Main_Slider_Volume_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Slider_Volume.ValueChanged
        Player.SetVolume(Main_Slider_Volume.Value)
    End Sub

    Private Sub Main_Slider_Balance_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Slider_Balance.ValueChanged
        Player.SetBalance(Main_Slider_Balance.Value / 100)
    End Sub

    Private Sub Main_prev_btn_Click(sender As Object, e As RoutedEventArgs) Handles Main_prev_btn.Click
        Player.SetPosition(0)
    End Sub

    Private Sub Main_song_btn_Click(sender As Object, e As RoutedEventArgs) Handles Main_song_btn.Click
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Filter = Utils.OFDFileFilters}
        If OFD.ShowDialog Then
            Player.LoadSong(OFD.FileName, Nothing, False)
        End If
    End Sub
End Class
