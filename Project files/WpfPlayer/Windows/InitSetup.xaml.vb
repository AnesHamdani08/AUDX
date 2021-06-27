Public Class InitSetup
    Dim sb As New Animation.Storyboard
    Private Async Sub InitSetup_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Logo.BeginAnimation(Image.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        Await Task.Delay(1000)
        Welcome_Message.BeginAnimation(TextBlock.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        Await Task.Delay(1000)
        Start_Btn.BeginAnimation(Button.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(250))))
        Dim MAnim As New Animation.ThicknessAnimation(New Thickness(Start_Btn.Margin.Left + 20, Start_Btn.Margin.Top, Start_Btn.Margin.Right, Start_Btn.Margin.Bottom), New Duration(TimeSpan.FromMilliseconds(500)))
        Animation.Storyboard.SetTarget(MAnim, Start_Btn)
        Animation.Storyboard.SetTargetProperty(MAnim, New PropertyPath("Margin"))
        sb.Children.Add(MAnim)
        sb.Duration = New Duration(TimeSpan.FromMilliseconds(500))
        sb.RepeatBehavior = Animation.RepeatBehavior.Forever
        sb.AutoReverse = True
        sb.Begin()
    End Sub

    Private Async Sub Start_Btn_Click(sender As Object, e As RoutedEventArgs) Handles Start_Btn.Click
        sb.Stop()
        Start_Btn.BeginAnimation(Button.MarginProperty, New Animation.ThicknessAnimation(New Thickness(555, Start_Btn.Margin.Top, Start_Btn.Margin.Right, Start_Btn.Margin.Bottom), New Duration(TimeSpan.FromMilliseconds(500))))
        Await Task.Delay(500)
        Logo.BeginAnimation(Image.WidthProperty, New Animation.DoubleAnimation(75, New Duration(TimeSpan.FromMilliseconds(500))))
        Await Task.Delay(250)
        Welcome_Message.BeginAnimation(TextBlock.MarginProperty, New Animation.ThicknessAnimation(New Thickness(-Welcome_Message.Width * 3, Welcome_Message.Margin.Top, Welcome_Message.Margin.Right, Welcome_Message.Margin.Bottom), New Duration(TimeSpan.FromMilliseconds(500))))
        Setup_ST.BeginAnimation(Grid.MarginProperty, New Animation.ThicknessAnimation(New Thickness(0, Setup_ST.Margin.Top, Setup_ST.Margin.Right, Setup_ST.Margin.Bottom), New Duration(TimeSpan.FromMilliseconds(500))))
        Dim MAnim As New Animation.ThicknessAnimation(New Thickness(ST_Start_Btn.Margin.Left + 20, ST_Start_Btn.Margin.Top, ST_Start_Btn.Margin.Right, ST_Start_Btn.Margin.Bottom), New Duration(TimeSpan.FromMilliseconds(500)))
        Animation.Storyboard.SetTarget(MAnim, ST_Start_Btn)
        Animation.Storyboard.SetTargetProperty(MAnim, New PropertyPath("Margin"))
        sb.Children.Clear()
        sb.Children.Add(MAnim)
        sb.Begin()
    End Sub

    Private Sub ST_Setup_Click(sender As Object, e As RoutedEventArgs) Handles ST_Setup.Click
        My.Windows.Settings.ShowDialog()
    End Sub

    Private Sub ST_Import_Click(sender As Object, e As RoutedEventArgs) Handles ST_Import.Click
        Try
            My.Settings.Upgrade()
            My.Settings.ISFIRSTSTART = False
            My.Settings.Save()
        Catch ex As Exception
            My.Settings.ISFIRSTSTART = True
            My.Settings.Save()
        End Try
        MessageBox.Show(Me, "All settings have been restored from the old version.", "MuPlay", MessageBoxButton.OK, MessageBoxImage.Information)
        'System.Windows.Forms.Application.Restart()
        'System.Windows.Application.Current.Shutdown()
        Exit Sub
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog() With {.CheckFileExists = True, .Title = "MuPlay"}
        Do
            If OFD.ShowDialog Then
                Dim TEMP_LIB As New Library(OFD.FileName)
                If TEMP_LIB.IsLoaded Then
                    Select Case MessageBox.Show(Me, "Does this seems correct ?" & vbCrLf & "Songs count: " & TEMP_LIB.Count & vbCrLf & "Date created: " & TEMP_LIB.DateCreated, "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question)
                        Case MessageBoxResult.Yes
                            For Each path In TEMP_LIB.Paths
                                My.Settings.LIBRARIESPATH.Add(path)
                            Next
                            My.Settings.LIBRARY_PATH = TEMP_LIB.LibraryPath
                            My.Settings.Save()
                            Exit Do
                        Case MessageBoxResult.No
                    End Select
                End If
            Else
                Exit Do
            End If
        Loop
    End Sub

    Private Async Sub ST_Start_Btn_Click(sender As Object, e As RoutedEventArgs) Handles ST_Start_Btn.Click
        ST_Start_Btn.BeginAnimation(Button.MarginProperty, New Animation.ThicknessAnimation(New Thickness(555, ST_Start_Btn.Margin.Top, ST_Start_Btn.Margin.Right, ST_Start_Btn.Margin.Bottom), New Duration(TimeSpan.FromMilliseconds(500))))
        Await Task.Delay(500)
        DialogResult = True
        Me.Close()
    End Sub
End Class
