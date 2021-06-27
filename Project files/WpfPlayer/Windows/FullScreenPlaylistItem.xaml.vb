Public Class FullScreenPlaylistItem
#Region "Confidental Stuff"
    Dim PreviewPlayer As New Player(Nothing, Nothing) With {.AutoPlay = False}
    Dim LinkPlayer As Player = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
    Private ResLike As New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/heart_filled.png"))
    Private ResLikeEmpty As New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/heart.png"))
    Public OnClick As Action = Nothing
    Public Property Source As String
    Private _Num As Integer
    Public Property Num As Integer
        Get
            Return _Num
        End Get
        Set(value As Integer)
            _Num = value
            FSPI_Num.Text = value
        End Set
    End Property
    Private _Title As String
    Public Property Title As String
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
            FSPI_Title.Text = value
        End Set
    End Property
    Private _Artist As String
    Public Property Artist As String
        Get
            Return _Artist
        End Get
        Set(value As String)
            _Artist = value
            FSPI_Artist.Text = value
        End Set
    End Property
    Public Property Cover As System.Drawing.Bitmap
#End Region
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler TryCast(Application.Current.MainWindow, MainWindow).media_like_btn.Click, AddressOf Main_Favourite_Click
    End Sub
    Public Sub New(iNum As Integer, iTitle As String, iArtist As String, iSource As String, Optional _Cover As System.Drawing.Bitmap = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Num = iNum
        Title = iTitle
        Artist = iArtist
        Source = iSource
        If _Cover Is Nothing Then
            If IO.File.Exists(iSource) Then
                Cover = Utils.GetAlbumArt(iSource)
                FSPI_Cover.Source = Utils.ImageSourceFromBitmap(Cover, True, 100, 100)
            End If
        Else
            Cover = _Cover
            FSPI_Cover.Source = Utils.ImageSourceFromBitmap(Cover, True, 100, 100)
        End If
        If My.Settings.FAVOURITETRACKS.Contains(iSource) Then
            FSPI_Favourite.Background = New ImageBrush(ResLike)
        Else
            FSPI_Favourite.Background = New ImageBrush(ResLikeEmpty)
        End If
        PreviewPlayer.LoadSong(iSource, Nothing, False, False)
        AddHandler TryCast(Application.Current.MainWindow, MainWindow).media_like_btn.Click, AddressOf Main_Favourite_Click
    End Sub
    Public Sub SelectItem()
        BorderThickness = New Thickness(1, 1, 1, 1)
        FSPI_Num.Text = "◉"
    End Sub
    Public Sub UnSelectItem()
        BorderThickness = New Thickness(0, 0, 0, 0)
        FSPI_Num.Text = Num
    End Sub
    Private Sub Main_Favourite_Click()
        If My.Settings.FAVOURITETRACKS.Contains(Source) Then
            'My.Settings.FavouriteTracks.Remove(Source)
            FSPI_Favourite.Background = New ImageBrush(ResLike)
        Else
            'My.Settings.FavouriteTracks.Add(Source)
            FSPI_Favourite.Background = New ImageBrush(ResLikeEmpty)
        End If
    End Sub
    Private Sub FSPI_Favourite_Click(sender As Object, e As RoutedEventArgs) Handles FSPI_Favourite.Click
        If My.Settings.FAVOURITETRACKS.Contains(Source) Then
            My.Settings.FAVOURITETRACKS.Remove(Source)
            FSPI_Favourite.Background = New ImageBrush(ResLikeEmpty)
        Else
            My.Settings.FAVOURITETRACKS.Add(Source)
            FSPI_Favourite.Background = New ImageBrush(ResLike)
        End If
        TryCast(Application.Current.MainWindow, MainWindow).UpdateFavourite()
    End Sub

    Private Sub FSPI_Preview_Click(sender As Object, e As RoutedEventArgs) Handles FSPI_Preview.Click
        PreviewPlayer.LoadSong(_Source, Nothing, False)
        PreviewPlayer.SetPosition(PreviewPlayer.GetLength / 4)
        If LinkPlayer.PlayerState = Player.State.Playing Then
            LinkPlayer.StreamPause(False)
        End If
        PreviewPlayer.StreamPlay()
    End Sub

    Private Sub FSPI_Preview_MouseLeave(sender As Object, e As MouseEventArgs) Handles FSPI_Preview.MouseLeave
        PreviewPlayer.StreamStop()
        If LinkPlayer.PlayerState = Player.State.Playing Then
            LinkPlayer.StreamPlay()
        End If
    End Sub

    Private Sub FullScreenPlaylistItem_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        If OnClick IsNot Nothing Then OnClick.Invoke
    End Sub
End Class
