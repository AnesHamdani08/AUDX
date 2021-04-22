Imports System.ComponentModel
Imports System.Net

Public Class WUpdator
    '200=>240
    Dim _Updator As Updator
    Dim WithEvents wc As Net.WebClient
    Dim updloc As String = Nothing
    Private Sub WUpdator_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Upd_Cpb.BeginAnimation(HandyControl.Controls.CircleProgressBar.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        Upd_State.BeginAnimation(TextBlock.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        '_Updator = New Updator(My.Settings.UpdatesServer)
        'Await _Updator.CheckForUpdates
        'If _Updator.LatestVersion = _Updator.Version Then
        '    Resources("Res_State") = "No updates found."
        'Else
        '    Resources("Res_State") = "Updates found."
        '    Upd_Cpb.IsIndeterminate = False
        '    Upd_Cpb.Value = 100
        '    BeginAnimation(Window.HeightProperty, New Animation.DoubleAnimation(240, New Duration(TimeSpan.FromSeconds(1))))
        '    Upd_Btn.BeginAnimation(Button.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        'End If
    End Sub
    Private Sub Upd_Btn_Click(sender As Object, e As RoutedEventArgs) Handles Upd_Btn.Click
        Upd_Cpb.BeginAnimation(HandyControl.Controls.CircleProgressBar.OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromSeconds(1))))
        wc = New Net.WebClient
        Upd_Cpb.IsIndeterminate = True
        updloc = IO.Path.Combine(Utils.AppDataPath, "Downloads", "update" & _Updator.LatestVersion.Major & _Updator.LatestVersion.Minor & _Updator.LatestVersion.Build & _Updator.LatestVersion.Revision & ".exe")
        wc.DownloadFileAsync(New Uri(_Updator.UpdatesLink), updloc)
        BeginAnimation(Window.HeightProperty, New Animation.DoubleAnimation(200, New Duration(TimeSpan.FromSeconds(1))))
        Upd_Wpb.BeginAnimation(HandyControl.Controls.WaveProgressBar.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
    End Sub
    Private Sub wc_DownloadFileCompleted(sender As Object, e As AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        BeginAnimation(Window.HeightProperty, New Animation.DoubleAnimation(200, New Duration(TimeSpan.FromSeconds(1))))
        Upd_Btn.BeginAnimation(Button.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        Upd_Wpb.BeginAnimation(HandyControl.Controls.WaveProgressBar.OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromSeconds(1))))
        Upd_Cpb.BeginAnimation(HandyControl.Controls.CircleProgressBar.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        If e.Error IsNot Nothing Then
            If Not String.IsNullOrEmpty(updloc) Then
                Process.Start(updloc)
                Hide()
            End If
        End If
    End Sub

    Private Sub wc_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        Upd_Wpb.Value = e.ProgressPercentage
    End Sub
    Private Async Sub Upd_Cpb_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles Upd_Cpb.MouseLeftButtonUp
        Upd_Btn.BeginAnimation(Button.OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(500))))
        BeginAnimation(Window.HeightProperty, New Animation.DoubleAnimation(200, New Duration(TimeSpan.FromSeconds(1))))
        Upd_Cpb.IsIndeterminate = True
        _Updator = New Updator(My.Settings.UpdatesServer)
        Await _Updator.CheckForUpdates
        If _Updator.LatestVersion = _Updator.Version Then
            Upd_Cpb.IsIndeterminate = False
            Upd_Cpb.Value = 100
            Resources("Res_State") = "No updates found."
        Else
            Resources("Res_State") = "Updates found. " & _Updator.Version.ToString & " > " & _Updator.LatestVersion.ToString
            Upd_Cpb.IsIndeterminate = False
            Upd_Cpb.Value = 100
            BeginAnimation(Window.HeightProperty, New Animation.DoubleAnimation(240, New Duration(TimeSpan.FromSeconds(1))))
            Upd_Btn.BeginAnimation(Button.OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromSeconds(1))))
        End If
    End Sub
End Class
