Imports System.ComponentModel

Public Class About
    Dim LinkPlayer As Player = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
    Dim WithEvents AniTmr As New Forms.Timer With {.Interval = 100, .Enabled = True}

    Private Sub AniTmr_Tick(sender As Object, e As EventArgs) Handles AniTmr.Tick
        Try
            visualizer.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(Player.Visualizers.SpectrumPeak, visualizer.Width, 64, System.Drawing.Color.Black, System.Drawing.Color.Red, System.Drawing.Color.Empty, System.Drawing.Color.Empty, 4, 2, 2, 500, False, False, False))
        Catch ex As Exception
            AniTmr.Stop()
            visualizer.Source = Nothing
        End Try
    End Sub

    Private Sub About_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        AniTmr.Enabled = False
        e.Cancel = True
    End Sub

    Private Sub About_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        AniTmr.Enabled = True
    End Sub
End Class
