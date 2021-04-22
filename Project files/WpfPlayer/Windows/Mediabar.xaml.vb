Imports System.Windows.Interop

Public Class Mediabar
    Private WithEvents Player As Player
    Dim WithEvents UIManager As New Threading.DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(100), .IsEnabled = True}
    Dim WithEvents TimeOutTimer As New Threading.DispatcherTimer With {.Interval = TimeSpan.FromSeconds(5), .IsEnabled = False}
    Dim WithEvents sb As New Animation.Storyboard
    Public Sub New(MainPlayer As Player)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Player = MainPlayer
    End Sub
    Private Sub mediabar_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        media_cover.Fill = New ImageBrush(Player.CurrentMediaCover)
        media_title.Text = Player.CurrentMediaTitle
        media_artist.Text = Player.CurrentMediaArtist
        media_posdur.Text = Utils.SecsToMins(Player.GetPosition) & "/" & Utils.SecsToMins(Player.GetLength)
        Dim AvgColor = Utils.GetAverageColor(Player.CurrentMediaCover, 192)
        main_border.Background = New SolidColorBrush(AvgColor)
        media_title.Foreground = New SolidColorBrush(Utils.GetInverseColor(AvgColor))
        media_artist.Foreground = New SolidColorBrush(Utils.GetInverseColor(AvgColor))
        media_posdur.Foreground = New SolidColorBrush(Utils.GetInverseColor(AvgColor))
    End Sub
    Public Sub ShowAnim(Type As AnimType)
        Opacity = 0
        Show()
        sb.Stop()
        sb.Children.Clear()
        Select Case Type
            Case AnimType.Bottom
                Left = SystemParameters.WorkArea.Right - Width
                Top = SystemParameters.WorkArea.Bottom + Height
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 0
                Oanim.To = 1
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Lanim As New Animation.DoubleAnimation
                Lanim.From = Top
                Lanim.To = SystemParameters.WorkArea.Bottom - Height
                Lanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Lanim, Me)
                Animation.Storyboard.SetTargetProperty(Lanim, New PropertyPath("Top"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Lanim)
                sb.Begin()
                TimeOutTimer.Start()
            Case AnimType.BottomRight
                Left = SystemParameters.WorkArea.Right + Width
                Top = SystemParameters.WorkArea.Bottom - Height
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 0
                Oanim.To = 1
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Tanim As New Animation.DoubleAnimation
                Tanim.From = SystemParameters.WorkArea.Right + Width
                Tanim.To = SystemParameters.WorkArea.Right - Width
                Tanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Tanim, Me)
                Animation.Storyboard.SetTargetProperty(Tanim, New PropertyPath("Left"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Tanim)
                sb.Begin()
            Case AnimType.Top
                Left = SystemParameters.WorkArea.Right - Width
                Top = -Height
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 0
                Oanim.To = 1
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Tanim As New Animation.DoubleAnimation
                Tanim.From = -Height
                Tanim.To = 0
                Tanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Tanim, Me)
                Animation.Storyboard.SetTargetProperty(Tanim, New PropertyPath("Top"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Tanim)
                sb.Begin()
            Case AnimType.TopRight
                Left = SystemParameters.WorkArea.Right + Width
                Top = 0
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 0
                Oanim.To = 1
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Tanim As New Animation.DoubleAnimation
                Tanim.From = SystemParameters.WorkArea.Right + Width
                Tanim.To = SystemParameters.WorkArea.Right - Width
                Tanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Tanim, Me)
                Animation.Storyboard.SetTargetProperty(Tanim, New PropertyPath("Left"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Tanim)
                sb.Begin()
        End Select
    End Sub
    Public Sub CloseAnim(Type As AnimType)
        sb.Children.Clear()
        Select Case Type
            Case AnimType.Bottom
                Left = SystemParameters.WorkArea.Right - Width
                Top = SystemParameters.WorkArea.Bottom - Height
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 1
                Oanim.To = 0
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Lanim As New Animation.DoubleAnimation
                Lanim.From = Top
                Lanim.To = SystemParameters.WorkArea.Bottom + Height
                Lanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Lanim, Me)
                Animation.Storyboard.SetTargetProperty(Lanim, New PropertyPath("Top"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Lanim)
                sb.Begin()
            Case AnimType.BottomRight
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 1
                Oanim.To = 0
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Tanim As New Animation.DoubleAnimation
                Tanim.From = SystemParameters.WorkArea.Right - Width
                Tanim.To = SystemParameters.WorkArea.Right + Width
                Tanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Tanim, Me)
                Animation.Storyboard.SetTargetProperty(Tanim, New PropertyPath("Left"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Tanim)
                sb.Begin()
            Case AnimType.Top
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 1
                Oanim.To = 0
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Tanim As New Animation.DoubleAnimation
                Tanim.From = 0
                Tanim.To = -Height
                Tanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Tanim, Me)
                Animation.Storyboard.SetTargetProperty(Tanim, New PropertyPath("Top"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Tanim)
                sb.Begin()
            Case AnimType.TopRight
                Dim Oanim As New Animation.DoubleAnimation
                Oanim.From = 1
                Oanim.To = 0
                Oanim.Duration = TimeSpan.FromSeconds(0.3)
                Animation.Storyboard.SetTarget(Oanim, Me)
                Animation.Storyboard.SetTargetProperty(Oanim, New PropertyPath("Opacity"))
                Dim Tanim As New Animation.DoubleAnimation
                Tanim.From = SystemParameters.WorkArea.Right - Width
                Tanim.To = SystemParameters.WorkArea.Right + Width
                Tanim.Duration = TimeSpan.FromSeconds(0.5)
                Animation.Storyboard.SetTarget(Tanim, Me)
                Animation.Storyboard.SetTargetProperty(Tanim, New PropertyPath("Left"))
                sb.Children.Add(Oanim)
                sb.Children.Add(Tanim)
                sb.Begin()
        End Select
    End Sub

    Private Sub UIManager_Tick(sender As Object, e As EventArgs) Handles UIManager.Tick
        media_posdur.Text = Utils.SecsToMins(Player.GetPosition) & "/" & Utils.SecsToMins(Player.GetLength)
    End Sub

    Private Sub Player_MediaLoaded(Title As String, Artist As String, Cover As InteropBitmap, Thumb As InteropBitmap, LyricsAvailable As Boolean, Lyrics As String) Handles Player.MediaLoaded
        media_cover.Fill = New ImageBrush(Player.CurrentMediaCover)
        media_title.Text = Player.CurrentMediaTitle
        media_artist.Text = Player.CurrentMediaArtist
        Try
            Dim AvgColor = Utils.GetAverageColor(Player.CurrentMediaCover, 192)
            main_border.Background = New SolidColorBrush(AvgColor)
            media_title.Foreground = New SolidColorBrush(Utils.GetInverseColor(AvgColor))
            media_artist.Foreground = New SolidColorBrush(Utils.GetInverseColor(AvgColor))
            media_posdur.Foreground = New SolidColorBrush(Utils.GetInverseColor(AvgColor))
        Catch ex As Exception
            main_border.Background = New SolidColorBrush(Color.FromArgb(192, 0, 0, 0))
            media_title.Foreground = Brushes.White
            media_artist.Foreground = Brushes.White
            media_posdur.Foreground = Brushes.White
        End Try
    End Sub
    Public Sub Dispose()
        Player = Nothing
        UIManager.Stop()
    End Sub

    Private Sub TimeOutTimer_Tick(sender As Object, e As EventArgs) Handles TimeOutTimer.Tick
        If Opacity = 1 Then
            TimeOutTimer.Stop()
            CloseAnim(My.Settings.MediaBar_AnimType)
        ElseIf Opacity = 0 Then
            Hide()
        End If
    End Sub

    Private Sub Mediabar_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        TimeOutTimer.Stop()
        TimeOutTimer.Start()
    End Sub

    Private Sub sb_Completed(sender As Object, e As EventArgs) Handles sb.Completed
        TimeOutTimer.Start()
    End Sub

    Public Enum AnimType
        Bottom = 0
        BottomRight = 1
        Top = 2
        TopRight = 3
    End Enum
End Class
