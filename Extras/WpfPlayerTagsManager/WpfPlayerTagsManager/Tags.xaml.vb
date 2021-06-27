Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Windows.Interop
Imports MetaBrainz.MusicBrainz

Public Class Tags

    Private Path As String
    Private WithEvents CoverList As New BindingList(Of Utils.CoverItem) 'List(Of Utils.CoverItem)
    Private Async Sub Overlay(IsVisible As Boolean)
        If IsVisible Then
            overlay_backgroudn.Visibility = Visibility.Visible
            overlay_backgroudn.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(200))))
            overlay_label.Visibility = Visibility.Visible
            overlay_label.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(200))))
            overlay_coverbg_cancel.Visibility = Visibility.Visible
            overlay_coverbg_cancel.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(200))))
            overlay_loadingline.Visibility = Visibility.Visible
            overlay_loadingline.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(200))))
        Else
            overlay_loadingline.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(200))))
            overlay_coverbg_cancel.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(200))))
            overlay_label.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(200))))
            overlay_backgroudn.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(200))))
            Await Task.Delay(200)
            overlay_loadingline.Visibility = Visibility.Hidden
            overlay_coverbg_cancel.Visibility = Visibility.Hidden
            overlay_label.Visibility = Visibility.Hidden
            overlay_backgroudn.Visibility = Visibility.Hidden
        End If
    End Sub
    Private Sub Tags_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        If Environment.GetCommandLineArgs.Count >= 2 Then
            If IO.File.Exists(Environment.GetCommandLineArgs(1)) Then
                Path = Environment.GetCommandLineArgs(1)
            End If
        End If
    End Sub
    Private Sub Tags_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If String.IsNullOrEmpty(Path) Then
            Me.Close()
        Else
            Prepare()
        End If
    End Sub

    Private Sub Exitbtn_Click(sender As Object, e As RoutedEventArgs) Handles Exitbtn.Click
        Me.Close()
    End Sub
    Private Sub Prepare()
        cover_stack.Children.Clear()
        CoverList.Clear()
        Dim Cover = Utils.GetAlbumArt(Path)
        If Cover IsNot Nothing Then
            TopBlurImage.Source = Utils.ImageSourceFromBitmap(Cover)
            TopImage.Source = TopBlurImage.Source
        Else
            TopBlurImage.Source = Nothing
            TopImage.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayerTagsManager;component/icon_n.ico"))
        End If
        Cover = Nothing
        Dim Covers = Utils.GetAlbumArts(Path)
        If Covers IsNot Nothing Then
            Dim IDX As Integer = 0
            For Each _Cover In Covers
                Try
                    cover_stack.Children.Add(New Image With {.Source = Utils.ImageSourceFromBitmap(_Cover.Cover), .Width = 100, .Height = 100, .ToolTip = "Index: " & IDX})
                    CoverList.Add(_Cover)
                    IDX += 1
                Catch ex As Exception
                    cover_stack.Children.Add(New Image With {.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayerTagsManager;component/icon_n.ico")), .Width = 100, .Height = 100, .ToolTip = "Index: " & IDX})
                    IDX += 1
                End Try
            Next
            IDX = Nothing
        End If
        Dim Tag As TagLib.File
        Try
            Tag = TagLib.File.Create(Path)
        Catch ex As Exception
            MessageBox.Show(Me, "This file type is unsupported or the file has been externally modified." & vbCrLf & "File: " & Path, "MuPlay External Tags Manager", MessageBoxButton.OK, MessageBoxImage.Error)
            Exitbtn_Click(Nothing, New RoutedEventArgs)
            Exit Sub
        End Try
        With Tag.Tag
            tag_album.Text = .Album
            tag_albumsort.Text = .AlbumSort
            tag_amazonid.Text = .AmazonId
            tag_artist.Text = .JoinedPerformers
            Top_Artist.Content = .JoinedPerformers
            tag_artists.Text = .JoinedAlbumArtists
            tag_audiobitrate.Text = Tag.Properties.AudioBitrate
            tag_audiochannels.Text = Tag.Properties.AudioChannels
            tag_bitspersample.Text = Tag.Properties.BitsPerSample
            tag_bpm.Text = .BeatsPerMinute
            For Each codec In (CType(Tag.Properties.Codecs, TagLib.ICodec()))
                tag_codecs.Text += codec.Description & ","
            Next
            tag_comment.Text = .Comment
            tag_composers.Text = .JoinedComposers
            tag_conductor.Text = .Conductor
            tag_copyright.Text = .Copyright
            tag_corrupted.Text = Tag.PossiblyCorrupt
            Try
                tag_corruptedreason.Text = String.Join(",", Tag.CorruptionReasons)
            Catch ex As Exception
            End Try
            tag_country.Text = .MusicBrainzReleaseCountry
            tag_description.Text = Tag.Properties.Description
            tag_duration.Text = Tag.Properties.Duration.Minutes & ":" & Tag.Properties.Duration.Seconds
            tag_genres.Text = .JoinedGenres
            tag_mbartistid.Text = .MusicBrainzArtistId
            tag_mbdiscid.Text = .MusicBrainzDiscId
            tag_mbid.Text = .MusicBrainzTrackId
            tag_mode.Text = Tag.MimeType
            tag_mode1.Text = Tag.Properties.MediaTypes.ToString
            tag_num.Text = .Track
            tag_path.Text = Path
            tag_releaseid.Text = .MusicBrainzReleaseId
            tag_samplerate.Text = Tag.Properties.AudioSampleRate
            tag_status.Text = .MusicBrainzReleaseStatus
            tag_tagver.Text = .TagTypes.ToString
            tag_title.Text = .Title
            Top_Title.Content = .Title
            tag_type.Text = Tag.Writeable
            tag_year.Text = .Year
            tag_lyrics.Text = .Lyrics
        End With
        Dim FI As New IO.FileInfo(Path)
        If FI.IsReadOnly Then
            lockfilebtn.Content = "Unlock"
        Else
            lockfilebtn.Content = "Lock"
        End If
    End Sub
    Private Sub Save()
        Dim Tag = TagLib.File.Create(Path)
        With Tag.Tag
            .Album = tag_album.Text
            .AlbumSort = tag_albumsort.Text
            .AmazonId = tag_amazonid.Text
            .Performers = tag_artist.Text.Split(";")
            .AlbumArtists = tag_artists.Text.Split(";")
            .BeatsPerMinute = tag_bpm.Text
            .Comment = tag_comment.Text
            .Composers = tag_composers.Text.Split(";")
            .Conductor = tag_conductor.Text
            .Copyright = tag_copyright.Text
            .MusicBrainzReleaseCountry = tag_country.Text
            .Genres = tag_genres.Text.Split(";")
            .MusicBrainzArtistId = tag_mbartistid.Text
            .MusicBrainzDiscId = tag_mbdiscid.Text
            .MusicBrainzTrackId = tag_mbid.Text
            .Track = tag_num.Text
            .MusicBrainzReleaseId = tag_releaseid.Text
            .MusicBrainzReleaseStatus = tag_status.Text
            .Title = tag_title.Text
            .Lyrics = tag_lyrics.Text
            Try
                .Year = tag_year.Text
            Catch ex As Exception
            End Try
            Dim CoverArray As TagLib.IPicture()
            ReDim CoverArray(CoverList.Count - 1)
            Dim pic As TagLib.Id3v2.AttachedPictureFrame = New TagLib.Id3v2.AttachedPictureFrame()
            pic.TextEncoding = TagLib.StringType.UTF8
            pic.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
            pic.Type = TagLib.PictureType.FrontCover
            Dim path As String
            For i As Integer = 0 To CoverList.Count - 1
                pic = New TagLib.Id3v2.AttachedPictureFrame With {.TextEncoding = TagLib.StringType.Latin1, .MimeType = Net.Mime.MediaTypeNames.Image.Jpeg, .Type = CoverList(i).Type}
                path = System.IO.Path.GetTempFileName()
                CoverList(i).Cover.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg)
                pic.Data = TagLib.ByteVector.FromPath(path)
                CoverArray(i) = pic
                pic = Nothing
            Next
            .Pictures = CoverArray
        End With
        Tag.Save()
    End Sub
    Private Sub Cover_Add()
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Title = "MuPlay", .FileName = "Pick a cover", .Filter = Utils.ImageFilters}
        If OFD.ShowDialog(Me) Then
            CoverList.Add(New Utils.CoverItem(System.Drawing.Image.FromFile(OFD.FileName), TagLib.PictureType.FrontCover))
        End If
    End Sub
    Private Sub Cover_Remove()
        Dim ib = InputBox("Which cover do you want to remove ?" & vbCrLf & "Enter the cover number below." & vbCrLf & "Note: the cover number starts from zero.")
        Try
            If Regex.Replace(ib, "[^0-9]", "") <= CoverList.Count - 1 Then
                CoverList.RemoveAt(Regex.Replace(ib, "[^0-9]", ""))
            End If
        Catch ex As Exception
            Throw New Exception("Check your input!")
        End Try
    End Sub
    Private Sub Cover_MoveTo()
        If CoverList.Count >= 2 Then
            Dim ib As New InputDialog("Move from;to , separate using "";""")
            If ib.ShowDialog Then
                Dim ibsplit = ib.Input.Split(";")
                If ibsplit(0) < CoverList.Count AndAlso ibsplit(0) >= 0 Then
                    If ibsplit(1) < CoverList.Count AndAlso ibsplit(1) >= 0 Then
                        Dim item = CoverList.Item(ibsplit(0))
                        CoverList.RemoveAt(ibsplit(0))
                        CoverList.Insert(ibsplit(1), item)
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub Cover_Paste()
        If System.Windows.Forms.Clipboard.ContainsImage Then
            CoverList.Add(New Utils.CoverItem(System.Windows.Forms.Clipboard.GetImage, TagLib.PictureType.FrontCover))
        Else
            MessageBox.Show(Me, "The clipboard is empty", "MuPlay", MessageBoxButton.OK, MessageBoxImage.Error)
        End If
    End Sub
    Private Sub Cover_Copy()
        Dim Inputd As New InputDialog("Which cover ? Hover on cover to see the index.") With {.Owner = Me}
        If Inputd.ShowDialog = True Then
            Try
                System.Windows.Forms.Clipboard.SetImage(CoverList(Inputd.Input).Cover)
            Catch ex As Exception
                Throw New Exception("Check your input!")
            End Try
        End If
    End Sub
    Private Sub Cover_SetType()
        Dim Inputd As New InputDialog("Which cover ? Hover on cover to see the index.") With {.Owner = Me}
        Dim CoverTypeSelector As New InputDialog("Please select Album Cover type" & vbCrLf & "01-File Icon" & vbCrLf & "02-Other File Icon" & vbCrLf & "03-Front Cover" & vbCrLf & "04-Back Cover" & vbCrLf & "05-Leaflet Page" & vbCrLf & "06-Media" & vbCrLf & "07-Lead Aritst" & vbCrLf & "08-Artist" & vbCrLf & "09-Conductor" & vbCrLf & "10-Band" & vbCrLf & "11-Composer" & vbCrLf & "12-Lyricist" & vbCrLf & "13-Recording Location" & vbCrLf & "14-During Recording" & vbCrLf & "15-During Performance" & vbCrLf & "16-Movie Screen Capture" & vbCrLf & "17-Colored Fish" & vbCrLf & "18-Illustration" & vbCrLf & "19-Band Logo" & vbCrLf & "20-Publisher Logo" & vbCrLf & "Note: Cancelling will select Type as Front Cover") With {.Owner = Me, .Height = 300}
        If Inputd.ShowDialog = True Then
            If CoverTypeSelector.ShowDialog = True Then
                Try
                    If Regex.Replace(Inputd.Input, "[^0-9]", "") <= CoverList.Count - 1 AndAlso Regex.Replace(CoverTypeSelector.Input, "[^0-9]", "") <= 20 Then
                        CoverList(Regex.Replace(Inputd.Input, "[^0-9]", "")).Type = Regex.Replace(CoverTypeSelector.Input, "[^0-9]", "")
                    End If
                Catch ex As Exception
                    Throw New Exception("Check your input!")
                End Try
            End If
        End If
    End Sub

    Private Async Sub savebtn_Click(sender As Object, e As RoutedEventArgs) Handles savebtn.Click
        Dim Fi As New IO.FileInfo(Path)
        Dim WasItReadOnly As Boolean = Fi.IsReadOnly
        If Fi.IsReadOnly = True Then
            If MessageBox.Show(Me, "The file you're trying to write to is read-only." & vbCrLf & "Would you like to temporarily disable write-protection ?", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                Fi.IsReadOnly = False
            Else
                Exit Sub
            End If
        End If
        Save()
        If WasItReadOnly = True Then
            Fi.IsReadOnly = True
        End If
    End Sub

    Private Sub lockfilebtn_Click(sender As Object, e As RoutedEventArgs) Handles lockfilebtn.Click
        Dim FI As New IO.FileInfo(Path)
        If FI.IsReadOnly = True Then
            FI.IsReadOnly = False
            lockfilebtn.Content = "Lock"
        Else
            FI.IsReadOnly = True
            lockfilebtn.Content = "Unlock"
        End If
    End Sub
    Dim WithEvents CoverBG As New BackgroundWorkerWithTag With {.WorkerSupportsCancellation = True}
    Private Sub fingerprintingbtn_Click(sender As Object, e As RoutedEventArgs) Handles fingerprintingbtn.Click
        Dim TitleNArtist As New InputDialog("Input the song's title and artist separated by "";""") With {.Owner = Me}
        If TitleNArtist.ShowDialog Then
            Dim Result = MusicBrainz.Search.Recording(TitleNArtist.Input.Split(";")(0), artist:=TitleNArtist.Input.Split(";")(1))
            If MessageBox.Show(Me, "Found " & Result.Count & " Match." & vbCrLf & "Do you want to browse them ?", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                For i As Integer = 0 To Result.Count - 1
                    Dim SB As New Text.StringBuilder
                    Dim artists As String
                    For _i As Integer = 0 To Result.Data(i).Artistcredit.Count - 1
                        If _i >= 1 Then
                            SB.Append(";" & Result.Data(i).Artistcredit(_i).Artist.Name)
                        Else
                            SB.Append(Result.Data(i).Artistcredit(_i).Artist.Name)
                        End If
                    Next
                    artists = SB.ToString
                    Dim _result = MessageBox.Show(Me, Result.Data(i).Title & " By: " & artists & vbCrLf & "Yes: select album." & vbCrLf & "No: next song" & vbCrLf & "Cancel: return to tag manager", "MuPlay", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
                    If _result = MessageBoxResult.Yes Then
                        SB.Clear()
                        For __i As Integer = 0 To Result.Data(i).Releaselist.Count - 1
                            SB.AppendLine(__i & "-" & Result.Data(i).Releaselist(__i).Title)
                        Next
                        If SB.ToString.Split(vbCrLf).Count > 10 Then
                            Dim idlg As New InputDialog(SB.ToString) With {.Owner = Me, .Height = 300}
                            Dim idlgrs = idlg.ShowDialog
                            idlg = Nothing
                            If idlgrs Then
                                tag_title.Text = Result.Data(i).Title
                                tag_mbid.Text = Result.Data(i).Id
                                Dim album = Result.Data(i).Releaselist(idlg.Input)
                                tag_album.Text = album.Title
                                tag_country.Text = album.Country
                                Try
                                    tag_year.Text = album.Date.Substring(0, 4)
                                Catch
                                    tag_year.Text = album.Date
                                End Try
                                tag_mbdiscid.Text = album.Id
                                tag_releaseid.Text = album.Releasegroup.Id
                                tag_status.Text = album.Status
                                tag_artists.Text = artists
                                tag_artist.Text = artists
                                CoverBG.Tag = album.Id
                                Overlay(True)
                                CoverBG.RunWorkerAsync()
                                Exit For
                            End If
                        Else
                            Dim idlg As New InputDialog(SB.ToString) With {.Owner = Me}
                            If idlg.ShowDialog Then
                                tag_title.Text = Result.Data(i).Title
                                tag_mbid.Text = Result.Data(i).Id
                                Dim album = Result.Data(i).Releaselist(idlg.Input)
                                tag_album.Text = album.Title
                                tag_country.Text = album.Country
                                Try
                                    tag_year.Text = album.Date.Substring(0, 4)
                                Catch
                                    tag_year.Text = album.Date
                                End Try
                                tag_mbdiscid.Text = album.Id
                                tag_releaseid.Text = album.Releasegroup.Id
                                tag_status.Text = album.Status
                                tag_artists.Text = artists
                                tag_artist.Text = artists
                                CoverBG.Tag = album.Id
                                Overlay(True)
                                CoverBG.RunWorkerAsync()
                                Top_Title.Content = tag_title.Text
                                Top_Artist.Content = artists
                                Exit For
                            End If
                        End If
                    ElseIf _result = MessageBoxResult.Cancel Then
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub
    Private Sub CoverBG_DoWork(sender As Object, e As DoWorkEventArgs) Handles CoverBG.DoWork
        Try
            Dim Cover As CoverArt.CoverArt = New CoverArt.CoverArt("firefox")
            Cover.WebSite = "coverartarchive.org"
            Cover.Port = -1
            Dim RawCover = Cover.FetchFront(New Guid(CoverBG.Tag.ToString)).Decode 'Replace the Guid String with your own
            e.Result = RawCover
        Catch ex As Exception
        End Try
    End Sub

    Private Sub CoverBG_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles CoverBG.RunWorkerCompleted
        If e.Result IsNot Nothing Then
            CoverList.Add(New Utils.CoverItem(CType(e.Result, System.Drawing.Image), TagLib.PictureType.FrontCover))
            Overlay(False)
        Else
            Overlay(False)
        End If
    End Sub
    Private Sub CoverList_ListChanged(sender As Object, e As ListChangedEventArgs) Handles CoverList.ListChanged
        cover_stack.Children.Clear()
        Dim IDX As Integer = 0
        For Each cover In CoverList
            Try
                cover_stack.Children.Add(New Image With {.Source = Utils.ImageSourceFromBitmap(cover.Cover), .Width = 100, .Height = 100, .ToolTip = "Index: " & IDX})
                IDX += 1
            Catch ex As Exception
                cover_stack.Children.Add(New Image With {.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/song.png")), .Width = 100, .Height = 100, .ToolTip = "Index: " & IDX})
                IDX += 1
            End Try
        Next
        IDX = Nothing
        Try
            TopImage.Source = Utils.ImageSourceFromBitmap(CoverList(0).Cover)
        Catch ex As Exception
            TopImage.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayerTagsManager;component/icon_n.ico"))
        End Try
    End Sub

    Private Sub overlay_coverbg_cancel_Click(sender As Object, e As RoutedEventArgs) Handles overlay_coverbg_cancel.Click
        CoverBG.CancelAsync()
    End Sub

    Private Sub clearfilebtn_Click(sender As Object, e As RoutedEventArgs) Handles clearfilebtn.Click
        tag_album.Clear()
        tag_albumsort.Clear()
        tag_amazonid.Clear()
        tag_artist.Clear()
        tag_artists.Clear()
        tag_bpm.Clear()
        tag_comment.Clear()
        tag_composers.Clear()
        tag_conductor.Clear()
        tag_copyright.Clear()
        tag_country.Clear()
        tag_genres.Clear()
        tag_mbartistid.Clear()
        tag_mbdiscid.Clear()
        tag_mbid.Clear()
        tag_num.Clear()
        tag_releaseid.Clear()
        tag_status.Clear()
        tag_title.Clear()
        tag_year.Clear()
        CoverList.Clear()
    End Sub
End Class
