Class Changelog
    Dim sb As New Animation.Storyboard
    Private Sub Changelog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim MAnim As New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(2000)))
        Animation.Storyboard.SetTarget(MAnim, done_btn)
        Animation.Storyboard.SetTargetProperty(MAnim, New PropertyPath("Opacity"))
        sb.Children.Add(MAnim)
        sb.Duration = New Duration(TimeSpan.FromMilliseconds(2000))
        sb.RepeatBehavior = Animation.RepeatBehavior.Forever
        sb.AutoReverse = True
        sb.Begin()
        Changelog_TB.Text = String.Join(vbCrLf, Utils.Changelog)
    End Sub

    Private Sub done_btn_Click(sender As Object, e As RoutedEventArgs) Handles done_btn.Click
        Close()
    End Sub
End Class
