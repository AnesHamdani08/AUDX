Public Class CoverViewer
    Public ImageToView As ImageSource
    Private Sub Window_Initialized(sender As Object, e As EventArgs)

    End Sub

    Private Sub Window_Activated(sender As Object, e As EventArgs)
        IMG_VIEWER.Source = ImageToView
        With ImageToView
            Me.Title += "-" & .Width & "x" & .Height
        End With
    End Sub

    Private Sub TitleBar_Save_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Save.Click
        Dim SFD As New Ookii.Dialogs.Wpf.VistaSaveFileDialog With {.AddExtension = True, .CheckPathExists = True, .DefaultExt = ".png", .Title = "MuPlay"}
        If SFD.ShowDialog Then
            Utils.SaveToPng(ImageToView, SFD.FileName)
        End If
    End Sub
End Class
