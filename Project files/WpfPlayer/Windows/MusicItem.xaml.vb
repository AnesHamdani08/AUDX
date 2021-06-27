Public Class MusicItem
    Public Property Title As String
    Public Property Artist As String
    Public Property Cover As System.Drawing.Bitmap
    Public Property CoverSource As ImageSource
    Private _player As Player
    Private _pplayer As Player
    Private _playlist As Playlist
    Private _source
    Private IsArtistMode As Boolean = False
    Private _sources As List(Of String)
    Private MV As New System.Threading.ManualResetEvent(False)
    Public Event OnPlaySingle()
    Public Sub New(Source As String, _Title As String, _Artist As String, _Cover As System.Drawing.Bitmap, Player As Player, PreviewPlayer As Player, Playlist As Playlist)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Title = _Title
        Song_Title.Text = _Title
        Artist = _Artist
        Song_Artist.Text = _Artist
        Cover = _Cover
        CoverSource = Utils.ImageSourceFromBitmap(_Cover)
        Song_Cover.Source = CoverSource
        _player = Player
        _pplayer = PreviewPlayer
        _playlist = Playlist
        _source = Source
        btn_add_playall.IsEnabled = False
        btn_add_playnow.IsEnabled = False
        btn_add_playsinglenow.IsEnabled = True
        btn_add_queuefirst.IsEnabled = True
        btn_add_queuelast.IsEnabled = True
        btn_add_queuenext.IsEnabled = True
    End Sub

    Public Sub New(Source As String, _Title As String, _Artist As String, _Cover As Uri, Player As Player, PreviewPlayer As Player, Playlist As Playlist)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Title = _Title
        Song_Title.Text = _Title
        Artist = _Artist
        Song_Artist.Text = _Artist
        Cover = Nothing
        CoverSource = New BitmapImage(_Cover)
        Song_Cover.Source = CoverSource
        _player = Player
        _pplayer = PreviewPlayer
        _playlist = Playlist
        _source = Source
        btn_add_playall.IsEnabled = False
        btn_add_playnow.IsEnabled = False
        btn_add_playsinglenow.IsEnabled = True
        btn_add_queuefirst.IsEnabled = True
        btn_add_queuelast.IsEnabled = True
        btn_add_queuenext.IsEnabled = True
    End Sub
    Public Sub New(Sources As List(Of String), _Name As String, _Cover As Uri, Player As Player, Playlist As Playlist)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Title = _Name
        Song_Title.Text = _Name
        Song_Artist.Text = Nothing
        _player = Player
        _playlist = Playlist
        IsArtistMode = True
        Song_Cover.Source = New BitmapImage(_Cover)
        _sources = Sources
        btn_preview.IsEnabled = False
        btn_add_playall.IsEnabled = True
        btn_add_playnow.IsEnabled = True
        btn_add_playsinglenow.IsEnabled = True
        btn_add_queuefirst.IsEnabled = True
        btn_add_queuelast.IsEnabled = True
        btn_add_queuenext.IsEnabled = True
    End Sub
    Public Sub New(Sources As List(Of String), _Name As String, _Cover As System.Drawing.Bitmap, Player As Player, Playlist As Playlist)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Title = _Name
        Song_Title.Text = _Name
        Song_Artist.Text = Nothing
        _player = Player
        _playlist = Playlist
        IsArtistMode = True
        Song_Cover.Source = Utils.ImageSourceFromBitmap(_Cover)
        _sources = Sources
        btn_preview.IsEnabled = False
        btn_add_playall.IsEnabled = True
        btn_add_playnow.IsEnabled = True
        btn_add_playsinglenow.IsEnabled = True
        btn_add_queuefirst.IsEnabled = True
        btn_add_queuelast.IsEnabled = True
        btn_add_queuenext.IsEnabled = True
    End Sub
    Private Sub btn_preview_Click(sender As Object, e As RoutedEventArgs) Handles btn_preview.Click
        _pplayer.LoadSong(_source, Nothing, False)
        _pplayer.SetPosition(_pplayer.GetLength / 4)
        If _player.PlayerState = Player.State.Playing Then
            _player.StreamPause(False)
        End If
        _pplayer.StreamPlay()
    End Sub
    Private Sub btn_preview_MouseLeave(sender As Object, e As MouseEventArgs) Handles btn_preview.MouseLeave
        _pplayer.StreamStop()
        If _player.PlayerState = Player.State.Playing Then
            _player.StreamPlay()
        End If
    End Sub
    Private Sub btn_add_Click(sender As Object, e As RoutedEventArgs) Handles btn_add.Click
        btn_add.ContextMenu.IsOpen = True
    End Sub

    Private Sub btn_add_queuefirst_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_queuefirst.Click
        If IsArtistMode Then
            _sources.Reverse()
            For Each song In _sources
                _playlist.Insert(0, song, Player.StreamTypes.Local)
            Next
            _sources.Reverse()
        Else
            _playlist.Insert(0, _source, Player.StreamTypes.Local)
        End If
    End Sub

    Private Sub btn_add_queuelast_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_queuelast.Click
        If IsArtistMode Then
            For Each song In _sources
                _playlist.Add(song, Player.StreamTypes.Local, False)
            Next
        Else
            _playlist.Add(_source, Player.StreamTypes.Local, False)
        End If
    End Sub

    Private Sub btn_add_queuenext_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_queuenext.Click
        If IsArtistMode Then
            If _playlist.Index = _playlist.Count - 1 Then
                For Each song In _sources
                    _playlist.Add(song, Player.StreamTypes.Local, False)
                Next
            Else
                For Each song In _sources
                    _playlist.Insert(_playlist.Index + 1, song, Player.StreamTypes.Local)
                Next
            End If
        Else
            If _playlist.Index = _playlist.Count - 1 Then
                _playlist.Add(_source, Player.StreamTypes.Local, False)
            Else
                _playlist.Insert(_playlist.Index + 1, _source, Player.StreamTypes.Local)
            End If
        End If
    End Sub

    Private Sub btn_add_playnow_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_playnow.Click
        If IsArtistMode Then
            Dim count = _playlist.Count
            For Each song In _sources
                _playlist.Add(song, Player.StreamTypes.Local)
            Next
            _player.LoadSong(_playlist.JumpTo(count), _playlist, False)
            _player.StreamPlay()
        Else
        _player.LoadSong(_source, _playlist)
        _player.StreamPlay()
        End If
    End Sub

    Private Sub btn_add_playall_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_playall.Click
        _playlist.Clear()
        For Each song In _sources
            _playlist.Add(song, Player.StreamTypes.Local)
        Next
        _player.LoadSong(_playlist.JumpTo(0), _playlist, False)
        _player.StreamPlay()
    End Sub

    Private Sub btn_add_playsinglenow_Click(sender As Object, e As RoutedEventArgs) Handles btn_add_playsinglenow.Click
        If IsArtistMode Then
            RaiseEvent OnPlaySingle()
        Else
            _player.LoadSong(_source, _playlist)
            _player.StreamPlay()
        End If
    End Sub
    Public Function WaitCloseAsync() As Task
        Return Task.Run(Sub()
                            MV.WaitOne()
                            MV.Reset()
                        End Sub)
    End Function

    Private Sub btn_close_Click(sender As Object, e As RoutedEventArgs) Handles btn_close.Click
        MV.Set()
    End Sub

End Class
