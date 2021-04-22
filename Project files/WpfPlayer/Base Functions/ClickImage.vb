Public Class ClickImage
    Inherits Image
    Public Property Player As Player
    Public Property PreviewPlayer As Player
    Public Property Playlist As Playlist
    Public Sub New(_Player As Player, _PreviewPlayer As Player, _Playlist As Playlist)
        Player = _Player
        PreviewPlayer = _PreviewPlayer
        Playlist = _Playlist
    End Sub
    Private Sub ClickImage_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseDown
        Select Case e.ClickCount
            Case 1
                PreviewPlayer.LoadSong(Tag, Nothing, False)
                PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
                If Player.PlayerState = Player.State.Playing Then
                    Player.StreamPause(False)
                End If
                PreviewPlayer.StreamPlay()
            Case 2
                Player.LoadSong(Tag, Playlist)
                Player.StreamPlay()
        End Select
    End Sub

    Private Sub ClickImage_MouseLeave(sender As Object, e As MouseEventArgs) Handles Me.MouseLeave
        PreviewPlayer.StreamStop()
        If Player.PlayerState = Player.State.Playing Then
            Player.StreamPlay()
        End If
    End Sub

End Class
