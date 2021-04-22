Imports System.ComponentModel

Public Class Search
    Dim FoundItems As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Dim Player As Player = Nothing
    Private Sub Search_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Player = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
        LV_Main.ItemsSource = FoundItems
    End Sub
    Private Sub Search_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub

    Private Async Sub search_btn_Click(sender As Object, e As RoutedEventArgs) Handles search_btn.Click
        search_btn.IsEnabled = False
        FoundItems.Clear()
        If Set_PlaylistOnly.IsChecked = False Then
            Dim Ltracks = TryCast(Application.Current.MainWindow, MainWindow).Gtracks
            Dim Lartists = Await TryCast(Application.Current.MainWindow, MainWindow).MainLibrary.ReadArtistsCache(Utils.AppDataPath)
            For Each song In Ltracks
                If song.ToLower.Contains(search_box.Text.ToLower) Then
                    Dim info = Utils.GetSongInfo(song)
                    FoundItems.Add(New PlaylistItem(FoundItems.Count + 1, info(1), info(0), info(2), info(3), info(4), Player.StreamTypes.Local, song, Nothing))
                End If
            Next
            For Each artist In Lartists
                If artist.Name.ToLower.Contains(search_box.Text.ToLower) Then
                    For Each song In artist.Songs
                        Dim info = Utils.GetSongInfo(song)
                        FoundItems.Add(New PlaylistItem(FoundItems.Count + 1, info(1), info(0), info(2), info(3), info(4), Player.StreamTypes.Local, song, Nothing))
                    Next
                End If
            Next
        Else
            Dim PlaylistEnum = TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist.Playlist.GetEnumerator
            Dim Idx = -1
            Do While PlaylistEnum.MoveNext
                Idx += 1
                If PlaylistEnum.Current.ToLower.Substring(0, PlaylistEnum.Current.IndexOf(">>")).Contains(search_box.Text.ToLower) Then
                    Dim info = Utils.GetSongInfo(PlaylistEnum.Current.Substring(0, PlaylistEnum.Current.IndexOf(">>")))
                    FoundItems.Add(New PlaylistItem(Idx, info(1), info(0), info(2), info(3), info(4), Player.StreamTypes.Local, PlaylistEnum.Current.Substring(0, PlaylistEnum.Current.IndexOf(">>")), Nothing))
                End If
            Loop
        End If
        search_btn.IsEnabled = True
    End Sub

    Private Sub LV_Main_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles LV_Main.SelectionChanged
        If LV_Main.SelectedIndex <> -1 Then
            If Set_PlaylistOnly.IsChecked = False Then
                Player.LoadSong(FoundItems.Item(LV_Main.SelectedIndex).Source, TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist)
                Player.StreamPlay()
            Else
                Player.LoadSong(TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist.GetItem(FoundItems.Item(LV_Main.SelectedIndex).Num), TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist, False)
                TryCast(Application.Current.MainWindow, MainWindow).MainPlaylist.SetIndex(FoundItems.Item(LV_Main.SelectedIndex).Num)
                Player.StreamPlay()
            End If
        End If
    End Sub
    Private Sub search_box_KeyDown(sender As Object, e As KeyEventArgs) Handles search_box.KeyDown
        If e.Key = Key.Enter Then
            search_btn_Click(Nothing, New RoutedEventArgs)
        End If
    End Sub
End Class
