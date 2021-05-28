Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class PlaylistManager
    Public Class CustomPlaylistItem
        Implements INotifyPropertyChanged

        Private _num As Integer
        Private _name As String
        Private _datecreated As String
        Private _description As String
        Private _count As Integer
        Public Sub New(pnum As String, pname As String, pdatecreated As String, pdescription As String, pcount As Integer, file As String, <CallerMemberName> ByVal Optional caller As String = Nothing)
            Num = pnum
            Name = pname
            Description = pdescription
            DateCreated = pdatecreated
            Source = file
            count = pcount
        End Sub

        Public Property Num As Integer
            Get
                Return _num
            End Get
            Set(ByVal value As Integer)
                If _num = value Then Return
                _num = value
                OnPropertyChanged()
            End Set
        End Property

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                If _name = value Then Return
                _name = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property DateCreated As String
            Get
                Return _datecreated
            End Get
            Set(ByVal value As String)
                If _datecreated = value Then Return
                _datecreated = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property Description As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                If _description = value Then Return
                _description = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property Count As Integer
            Get
                Return _count
            End Get
            Set(ByVal value As Integer)
                If _count = value Then Return
                _count = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property Source As String
        Public Property SongCount As List(Of String)
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
    Public Function ReadPlaylist(source As String) As CustomPlaylistItem
        If IO.File.Exists(source) Then
            Try
                Dim Doc As New Xml.XmlDocument()
                Doc.Load(source)
                Dim node = Doc.SelectSingleNode("/MuPlay/Playlist")
                Dim _name As String = node.Attributes(0).InnerText
                Dim _dc As String = node.Attributes(1).InnerText
                Dim _count As String = node.Attributes(2).InnerText
                Dim _desc As String = node.Attributes(3).InnerText
                Dim _source As String = source
                Dim _snglist As New List(Of String)
                For Each song As Xml.XmlNode In Doc.SelectNodes("/MuPlay/Songs/Song")
                    _snglist.Add(song.Attributes(5).InnerText)
                Next
                Return New CustomPlaylistItem(Playlists_Source.Count + 1, _name, _dc, _desc, _count, _source) With {.SongCount = _snglist}
            Catch ex As Exception
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function
    Dim Playlists_Source As New ObjectModel.ObservableCollection(Of CustomPlaylistItem)

    Private Sub PlaylistManager_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Main_CustomPlaylist_View.ItemsSource = Playlists_Source
    End Sub
    Private Sub PlaylistManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If IO.Directory.Exists(IO.Path.Combine(Utils.AppDataPath, "Playlists")) Then
            Dim _filter = "*.xml"
            For Each playlist In _filter.Split("|"c).SelectMany(Function(filter) System.IO.Directory.GetFiles(IO.Path.Combine(Utils.AppDataPath, "Playlists"), filter, IO.SearchOption.TopDirectoryOnly)).ToArray()
                Playlists_Source.Add(ReadPlaylist(playlist))
            Next
        End If
    End Sub
    Private Sub PlaylistManager_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub
    Private Sub TitleBar_Add_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Add.Click
        Dim pb As New PlaylistBuilder
        'Utils.UpdateSkin(My.Settings.DefaultTheme, pb)
        If pb.ShowDialog Then
            If pb.Result IsNot Nothing Then
                With pb.Result
                    Playlists_Source.Add(New CustomPlaylistItem(Playlists_Source.Count + 1, .Name, .DateCreated, .Description, .Count, .Source))
                End With
            End If
        End If
    End Sub

    Private Sub TitleBar_Remove_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Remove.Click
        If Main_CustomPlaylist_View.SelectedIndex <> -1 Then
            Dim idx = Main_CustomPlaylist_View.SelectedIndex
            IO.File.Delete(Playlists_Source(idx).Source)
            Playlists_Source.RemoveAt(idx)
        End If
    End Sub

    Private Sub Main_CustomPlaylist_View_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Main_CustomPlaylist_View.MouseDown
        If e.ClickCount = 2 AndAlso Main_CustomPlaylist_View.SelectedIndex <> -1 Then
            Dim csp = Playlists_Source(Main_CustomPlaylist_View.SelectedIndex)
            If MessageBox.Show(Me, "Are you sure you want to load this playlist." & vbCrLf & csp.Name & " with " & csp.Count & " song(s), that was created on " & csp.DateCreated, "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                With TryCast(Application.Current.MainWindow, MainWindow)
                    .MainPlaylist.Clear()
                    For Each song In ReadPlaylist(csp.Source).SongCount
                        .MainPlaylist.Add(song, Player.StreamTypes.Local, False)
                    Next
                    .MainPlayer.LoadSong(.MainPlaylist.JumpTo(0), .MainPlaylist, False)
                    .MainPlayer.StreamPlay()
                End With
            End If
        End If
    End Sub
End Class
