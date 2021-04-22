Imports System.ComponentModel
Imports System.Net

Public Class StreamDownloader
    Private Property Player As Player
    Private WithEvents WC As New System.Net.WebClient
    Private Property _State As States = States.Free
    'Private Property CMTitle As String
    'Private Property CMArtist As String
    'Private Property CMCover As Interop.InteropBitmap
    'Private Property CMDest As String
    Public Property State As States
        Get
            Return _State
        End Get
        Set(value As States)
            _State = value
            RaiseEvent OnStateChanged(value)
        End Set
    End Property
    Public Property Progress As Integer
    Public Event OnStateChanged(State As States)
    Public Sub New(MainPlayer As Player)
        Player = MainPlayer
    End Sub
    Public Sub DownloadCurrent()
        Select Case Player.CurrentMediaType
            Case Player.StreamTypes.Local
                State = States.Local
            Case Player.StreamTypes.URL
                'CMTitle = Player.CurrentMediaTitle
                'CMArtist = Player.CurrentMediaArtist
                'CMCover = Player.CurrentMediaCover
                Try
                    If IO.Directory.Exists(IO.Path.Combine(Utils.AppDataPath, "Downloads")) Then
                        Dim FileType As String = Un4seen.Bass.Bass.BASS_ChannelGetInfo(Player.Stream).ctype.ToString
                        Dim FilePath As String = IO.Path.Combine(Utils.AppDataPath, "Downloads", Player.CurrentMediaTitle.Replace(" ", "") & "." & FileType.Substring(FileType.LastIndexOf("_") + 1))
                        If IO.File.Exists(FilePath) Then
                            Select Case MessageBox.Show("There is a file with the same name in the downloads folder." & vbCrLf & "Yes: overwrite the file." & vbCrLf & "No: download with another name." & vbCrLf & "Cancel: abort operation", "Stream downloader", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
                                Case MessageBoxResult.Yes
                                    'CMDest = FilePath
                                    WC.DownloadFileAsync(New Uri(Player.SourceURL), FilePath)
                                    State = States.Downloading
                                Case MessageBoxResult.No
                                    Dim Rnd As New Random
                                    'CMDest = IO.Path.Combine(Utils.AppDataPath, "Downloads", Player.CurrentMediaTitle.Replace(" ", "") & Rnd.Next & "." & FileType.Substring(FileType.LastIndexOf("_") + 1))
                                    WC.DownloadFileAsync(New Uri(Player.SourceURL), IO.Path.Combine(Utils.AppDataPath, "Downloads", Player.CurrentMediaTitle.Replace(" ", "") & Rnd.Next & "." & FileType.Substring(FileType.LastIndexOf("_") + 1)))
                                    Rnd = Nothing
                                    State = States.Downloading
                                Case MessageBoxResult.Cancel
                                    Exit Sub
                            End Select
                        Else
                            'CMDest = FilePath
                            WC.DownloadFileAsync(New Uri(Player.SourceURL), FilePath)
                            State = States.Downloading
                        End If
                    Else
                        IO.Directory.CreateDirectory(IO.Path.Combine(Utils.AppDataPath, "Downloads"))
                        Dim FileType As String = Un4seen.Bass.Bass.BASS_ChannelGetInfo(Player.Stream).ctype.ToString
                        Dim FilePath As String = IO.Path.Combine(Utils.AppDataPath, "Downloads", Player.CurrentMediaTitle.Replace(" ", "") & "." & FileType.Substring(FileType.LastIndexOf("_") + 1))
                        'CMDest = FilePath
                        WC.DownloadFileAsync(New Uri(Player.SourceURL), FilePath)
                        State = States.Downloading
                    End If
                Catch ex As Exception
                    State = States.FatalError
                End Try
            Case Player.StreamTypes.Youtube

        End Select
    End Sub

    Private Sub WC_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles WC.DownloadProgressChanged
        Progress = e.ProgressPercentage
    End Sub

    Private Sub WC_DownloadFileCompleted(sender As Object, e As AsyncCompletedEventArgs) Handles WC.DownloadFileCompleted
        'Try
        '    Dim Tags = TagLib.File.Create(CMDest)
        '    With Tags
        '        .Tag.Title = CMTitle
        '        .Tag.Performers = {CMArtist}
        '        Dim CoverArray(1) As TagLib.IPicture
        '        Dim pic As TagLib.Id3v2.AttachedPictureFrame = New TagLib.Id3v2.AttachedPictureFrame()
        '        pic.TextEncoding = TagLib.StringType.UTF8
        '        pic.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
        '        pic.Type = TagLib.PictureType.FrontCover
        '        Dim path As String
        '        pic = New TagLib.Id3v2.AttachedPictureFrame With {.TextEncoding = TagLib.StringType.Latin1, .MimeType = Net.Mime.MediaTypeNames.Image.Jpeg, .Type = TagLib.PictureType.FrontCover}
        '        path = System.IO.Path.GetTempFileName()
        '        Utils.BitmapFromImageSource(CMCover).Save(path, System.Drawing.Imaging.ImageFormat.Jpeg)
        '        pic.Data = TagLib.ByteVector.FromPath(path)
        '        CoverArray(0) = pic
        '        pic = Nothing
        '        .Tag.Pictures = CoverArray
        '        .Save()
        '    End With
        'Catch ex As Exception
        'End Try        
        If e.Error IsNot Nothing Then
            'CMTitle = Nothing
            'CMArtist = Nothing
            'CMCover = Nothing
            State = States.FatalError
        Else
            State = States.DownloadCompleted
        End If
    End Sub

    Public Enum States
        Free = 0
        Downloading = 1
        FatalError = 2
        DownloadCompleted = 3
        Local = 4
    End Enum
End Class
