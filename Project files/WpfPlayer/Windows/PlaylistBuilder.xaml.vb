Imports System.Xml

Public Class PlaylistBuilder
    Public Class PlaylistInfo
        Public Property Name As String
        Public Property Description As String
        Public Property Songlist As ObjectModel.ObservableCollection(Of PlaylistItem)
        Public Property Count As Integer
        Public Property DateCreated As String
        Public Property Source As String
    End Class
    Dim PB_View_Source As New ObjectModel.ObservableCollection(Of PlaylistItem)
    Public Property Result As PlaylistInfo
    Private Sub PlaylistBuilder_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        PB_View.ItemsSource = PB_View_Source
    End Sub

    Private Sub TitleBar_Add_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Add.Click
        Dim ofd As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.Multiselect = True, .CheckFileExists = True, .Filter = Utils.OFDFileFilters, .Title = "MuPlay"}
        If ofd.ShowDialog Then
            For Each song In ofd.FileNames
                Dim tag = TagLib.File.Create(song).Tag
                PB_View_Source.Add(New PlaylistItem(PB_View_Source.Count + 1, tag.Title, tag.JoinedPerformers, tag.Album, tag.Year, tag.Track, Player.StreamTypes.Local, song, Nothing))
            Next
        End If
    End Sub

    Private Sub TitleBar_LoadCurrentPlaylist_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_LoadCurrentPlaylist.Click
        For Each item In TryCast(Application.Current.MainWindow, MainWindow).playlistItems
            PB_View_Source.Add(item)
        Next
    End Sub

    Private Sub TitleBar_Remove_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Remove.Click
        If PB_View.SelectedIndex <> -1 Then
            PB_View_Source.RemoveAt(PB_View.SelectedIndex)
        End If
    End Sub

    Private Async Sub TitleBar_Save_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Save.Click
        If String.IsNullOrEmpty(PB_Name.Text) = False AndAlso String.IsNullOrEmpty(PB_Desc.Text) = False AndAlso PB_View_Source.Count > 0 Then
            If Not IO.Directory.Exists(IO.Path.Combine(Utils.AppDataPath, "Playlists")) Then IO.Directory.CreateDirectory(IO.Path.Combine(Utils.AppDataPath, "Playlists"))
            Dim rnd As New Random
            Dim DC As String = Now.ToString("dd/MM/yyyy HH:mm:ss")
            Dim _src As String = IO.Path.Combine(Utils.AppDataPath, "Playlists", PB_Name.Text & rnd.Next & ".xml")
            Dim Writer As XmlWriter = XmlWriter.Create(_src, New XmlWriterSettings With {.Indent = True, .Async = True})
            ' Begin writing.        
            Await Writer.WriteStartDocumentAsync()
            Writer.WriteStartElement("MuPlay")
            Writer.WriteStartElement("Playlist")
            Writer.WriteAttributeString("name", PB_Name.Text)
            Writer.WriteAttributeString("date", DC)
            Writer.WriteAttributeString("count", PB_View_Source.Count)
            Writer.WriteAttributeString("description", PB_Desc.Text)
            Await Writer.WriteEndElementAsync
            Writer.WriteStartElement("Songs")
            For Each item In PB_View_Source
                Writer.WriteStartElement("Song")
                Writer.WriteAttributeString("title", item.Title)
                Writer.WriteAttributeString("artist", item.Artist)
                Writer.WriteAttributeString("album", item.Album)
                Writer.WriteAttributeString("track", item.Track)
                Writer.WriteAttributeString("year", item.Year)
                Writer.WriteAttributeString("source", item.Source)
                Await Writer.WriteEndElementAsync()
            Next
            Await Writer.WriteEndElementAsync
            Await Writer.WriteEndElementAsync
            Await Writer.WriteEndDocumentAsync
            Writer.Close()
            Result = New PlaylistInfo With {.Name = PB_Name.Text, .DateCreated = DC, .Count = PB_View_Source.Count, .Description = PB_Desc.Text, .Songlist = PB_View_Source, .Source = _src}
            DialogResult = True
            Close()
        Else
            TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("Playlist builder", "The playlist couldn't be saved.", HandyControl.Data.NotifyIconInfoType.Error)
            DialogResult = False
            Close()
        End If
    End Sub
End Class
