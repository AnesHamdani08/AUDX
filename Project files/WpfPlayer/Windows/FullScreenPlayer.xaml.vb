Imports System.ComponentModel
Imports System.Windows.Interop
Imports WpfPlayer

Public Class FullScreenPlayer
    Dim WithEvents LinkPlayer As Player = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
    Dim WithEvents LinkPlaylist As Playlist = TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist
    Dim WithEvents UIManager As New Threading.DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(500), .IsEnabled = True}
    Private Async Sub FullScreenPlayer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Select Case LinkPlayer.RepeateType
            Case Player.RepeateBehaviour.Shuffle
                Media_Next_Cover.IsEnabled = False
                Media_Next_Cover.Opacity = 0.5
                Media_Previous_Cover.IsEnabled = False
                Media_Previous_Cover.Opacity = 0.5
            Case Else
                Media_Next_Cover.IsEnabled = True
                Media_Next_Cover.Opacity = 1
                Media_Previous_Cover.IsEnabled = True
                Media_Previous_Cover.Opacity = 1
        End Select
        Media_TrackBar.Maximum = LinkPlayer.GetLength
        Background_Image.Source = LinkPlayer.CurrentMediaCover
        Media_Current_Cover.Background = New ImageBrush(LinkPlayer.CurrentMediaCover)
        Media_Next_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(Utils.GetAlbumArt(LinkPlaylist.NextItem)))
        Media_Previous_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(Utils.GetAlbumArt(LinkPlaylist.PreviousItem)))
        Dim BackColor = Utils.GetAverageColor(LinkPlayer.CurrentMediaCover)
        Control_Panel.Background = New SolidColorBrush(Utils.GetInverseColor(BackColor, 150))
        'UI Stuff
        Logo2.Effect = New Effects.DropShadowEffect With {.BlurRadius = 15, .Color = Utils.GetInverseColor(BackColor), .ShadowDepth = 0}
        If My.Settings.UseAnimations Then
            Logo.BeginAnimation(Image.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(2))))
            Dim WAnim As New Animation.DoubleAnimation
            WAnim.To = Logo.Height / 2
            WAnim.Duration = New Duration(TimeSpan.FromSeconds(2))
            Dim DAnim As New Animation.DoubleAnimation
            DAnim.To = Logo.Width / 2
            DAnim.Duration = New Duration(TimeSpan.FromSeconds(2))
            Animation.Storyboard.SetTarget(WAnim, Logo)
            Animation.Storyboard.SetTarget(DAnim, Logo)
            Animation.Storyboard.SetTargetProperty(WAnim, New PropertyPath("Height"))
            Animation.Storyboard.SetTargetProperty(DAnim, New PropertyPath("Width"))
            Dim sb As New Animation.Storyboard
            AddHandler sb.Completed, AddressOf sb_completed
            sb.Children.Add(WAnim)
            sb.Children.Add(DAnim)
            sb.Begin()
        Else
            Logo.Opacity = 1
            Await Task.Delay(1000)
        End If
    End Sub
    Private Sub FullScreenPlayer_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        Dim BackColor = Utils.GetAverageColor(LinkPlayer.CurrentMediaCover)
        Dim InvColor = Utils.GetInverseColor(BackColor)
        TryCast(Logo2.Effect, Effects.DropShadowEffect).Color = InvColor
    End Sub
    Private Sub LinkPlayer_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles LinkPlayer.MediaLoaded
        Media_TrackBar.Maximum = LinkPlayer.GetLength
        Background_Image.Source = LinkPlayer.CurrentMediaCover
        Media_Current_Cover.Background = New ImageBrush(LinkPlayer.CurrentMediaCover)
        Media_Next_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(Utils.GetAlbumArt(LinkPlaylist.NextItem)))
        Media_Previous_Cover.Background = New ImageBrush(Utils.ImageSourceFromBitmap(Utils.GetAlbumArt(LinkPlaylist.PreviousItem)))
        Dim BackColor = Utils.GetAverageColor(LinkPlayer.CurrentMediaCover)
        If Logo2.Effect IsNot Nothing Then
            TryCast(Logo2.Effect, Effects.DropShadowEffect).Color = Utils.GetInverseColor(BackColor)
        Else
            Logo2.Effect = New Effects.DropShadowEffect With {.ShadowDepth = 0, .BlurRadius = 20}
        End If
        Control_Panel.Background = New SolidColorBrush(Utils.GetInverseColor(BackColor, 150))
        If My.Settings.UseAnimations Then
            Opacity_Switch.IsChecked = True
        Else
            Media_Previous_Cover.Opacity = 1
            Media_Previous_Cover.Visibility = Visibility.Visible
            Media_Next_Cover.Opacity = 1
            Media_Next_Cover.Visibility = Visibility.Visible
        End If
    End Sub
    Private Async Sub sb_completed()
        Logo.BeginAnimation(Image.OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromSeconds(2))))
        Await Task.Delay(2000)
        Logo2.BeginAnimation(Image.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(2))))
        Background_Image.BeginAnimation(Image.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        Media_Current_Cover.BeginAnimation(Image.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        Opacity_Switch.IsChecked = True
    End Sub

    Private Sub Media_Previous_Click(sender As Object, e As MouseButtonEventArgs) Handles Media_Previous_Cover.MouseDown
        If My.Settings.UseAnimations Then
            Opacity_Switch.IsChecked = False
        End If
        TryCast(Application.Current.MainWindow, MainWindow).media_prev_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Media_PlayPause_Click(sender As Object, e As MouseButtonEventArgs) Handles Media_Current_Cover.MouseDown
        TryCast(Application.Current.MainWindow, MainWindow).media_play_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Media_Next_Click(sender As Object, e As MouseButtonEventArgs) Handles Media_Next_Cover.MouseDown
        If My.Settings.UseAnimations Then
            Opacity_Switch.IsChecked = False
        End If
        TryCast(Application.Current.MainWindow, MainWindow).media_next_btn_Click(Nothing, New RoutedEventArgs)
    End Sub

    Private Sub Media_TrackBar_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Media_TrackBar.ValueChanged
        If Media_TrackBar.IsMouseOver Then
            Try
                LinkPlayer.SetPosition(Media_TrackBar.Value)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub UIManager_Tick(sender As Object, e As EventArgs) Handles UIManager.Tick
        If Not Media_TrackBar.IsMouseOver Then
            Media_TrackBar.Value = LinkPlayer.GetPosition
        End If
    End Sub

    Private Sub Control_Panel_MouseEnter(sender As Object, e As MouseEventArgs) Handles Control_Panel.MouseEnter
        Control_Panel.Opacity = 1
    End Sub

    Private Sub Control_Panel_MouseLeave(sender As Object, e As MouseEventArgs) Handles Control_Panel.MouseLeave
        Control_Panel.Opacity = 0.25
    End Sub
    Private Sub FullScreenPlayer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Hide()
        e.Cancel = True
    End Sub

    Private Sub LinkPlayer_OnShuffleChanged(NewType As Player.RepeateBehaviour) Handles LinkPlayer.OnShuffleChanged
        Select Case NewType
            Case Player.RepeateBehaviour.Shuffle
                Media_Next_Cover.IsEnabled = False
                Media_Next_Cover.Opacity = 0.5
                Media_Previous_Cover.IsEnabled = False
                Media_Previous_Cover.Opacity = 0.5
            Case Else
                Media_Next_Cover.IsEnabled = True
                Media_Next_Cover.Opacity = 1
                Media_Previous_Cover.IsEnabled = True
                Media_Previous_Cover.Opacity = 1
        End Select
    End Sub
End Class
