Public Class Utils
    Public Class CoverItem
        Public Property Type As TagLib.PictureType
        Public Property Cover As System.Drawing.Image
        Public Sub New(_Cover As System.Drawing.Image, _Type As TagLib.PictureType)
            Cover = _Cover
            Type = _Type
        End Sub
    End Class
    Public Shared ReadOnly Property ImageFilters As String = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|All Files(*.*)|*.*"
    <Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint:="DeleteObject")>
    Public Shared Function DeleteObject(
<Runtime.InteropServices.[In]> ByVal hObject As IntPtr) As Boolean
    End Function
    Public Shared Function ImageSourceFromBitmap(ByVal bmp As System.Drawing.Bitmap, Optional ChangeRes As Boolean = False, Optional ResX As Integer = 0, Optional ResY As Integer = 0) As ImageSource
        If ChangeRes = False Then
            Try
                Dim handle = bmp.GetHbitmap()
                Try
                    Return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                Finally
                    DeleteObject(handle)
                End Try
            Catch ex As Exception
                Return Nothing
            End Try
        Else
            Try
                Dim ResBmp As New System.Drawing.Bitmap(bmp, ResX, ResY)
                Dim handle = ResBmp.GetHbitmap()
                Try
                    Return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                Finally
                    DeleteObject(handle)
                End Try
            Catch ex As Exception
                Return Nothing
            End Try
        End If
    End Function
    Public Shared Function BitmapFromImageSource(ByVal bitmap As BitmapSource) As System.Drawing.Bitmap
        Dim bmp As System.Drawing.Bitmap = New System.Drawing.Bitmap(bitmap.PixelWidth, bitmap.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        Dim data As System.Drawing.Imaging.BitmapData = bmp.LockBits(New System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.[WriteOnly], System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        bitmap.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride)
        bmp.UnlockBits(data)
        Return bmp
    End Function
    Public Shared Function GetAverageColor(ByVal bitmap As BitmapSource, Optional Opacity As Integer = 255) As System.Windows.Media.Color
        If bitmap Is Nothing Then Return Colors.Black
        Dim format = bitmap.Format
        If format <> PixelFormats.Bgr24 AndAlso format <> PixelFormats.Bgr32 AndAlso format <> PixelFormats.Bgra32 AndAlso format <> PixelFormats.Pbgra32 Then
            Throw New InvalidOperationException("BitmapSource must have Bgr24, Bgr32, Bgra32 or Pbgra32 format")
            Return Nothing
        End If

        Dim width = bitmap.PixelWidth
        Dim height = bitmap.PixelHeight
        Dim numPixels = width * height
        Dim bytesPerPixel = format.BitsPerPixel / 8
        Dim pixelBuffer = New Byte(numPixels * bytesPerPixel - 1) {}
        bitmap.CopyPixels(pixelBuffer, width * bytesPerPixel, 0)
        Dim blue As Long = 0
        Dim green As Long = 0
        Dim red As Long = 0
        Dim i As Integer = 0

        While i < pixelBuffer.Length
            blue += pixelBuffer(i)
            green += pixelBuffer(i + 1)
            red += pixelBuffer(i + 2)
            i += bytesPerPixel
        End While

        Return System.Windows.Media.Color.FromArgb(CByte(Opacity), CByte((red / numPixels)), CByte((green / numPixels)), CByte((blue / numPixels)))
    End Function
    Public Shared Function GetInverseColor(Color As System.Windows.Media.Color, Optional Opacity As Integer = 255) As System.Windows.Media.Color
        Dim R = Color.R
        Dim G = Color.G
        Dim B = Color.B
        Dim newR = 255 - R
        Dim newG = 255 - G
        Dim newB = 255 - B
        Return System.Windows.Media.Color.FromArgb(Opacity, newR, newG, newB)
    End Function
    ''' <summary>
    ''' Artist, Title, Album, Year, TrackNum, Genres, Lyrics, Bitrates
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Shared Function GetSongInfo(path As String) As String()
        Try
            Dim Tag = TagLib.File.Create(path)
            Dim Artist = Tag.Tag.JoinedPerformers
            Dim Title = Tag.Tag.Title
            Dim Album = Tag.Tag.Album
            Dim Year = Tag.Tag.Year
            Dim TrackNum = Tag.Tag.TrackCount
            Dim Lyrics = Tag.Tag.Lyrics
            Dim Genres = Tag.Tag.Genres
            Dim Bitrates = Tag.Properties.AudioBitrate
            Dim CompressedInfo As String()
            CompressedInfo = {Artist, Title, Album, Year, TrackNum, String.Join(";", Genres), Lyrics, Bitrates}
            Return CompressedInfo
        Catch ex As Exception
            Return {"Not Available", "Not Available", "Not Available", 0, 0, "Not Available", "Not Available", 0}
        End Try
    End Function
    Public Shared Function GetAlbumArt(path As String) As System.Drawing.Image
        Try
            Dim Tag = TagLib.File.Create(path)
            If Tag.Tag.Pictures.Length >= 1 Then
                Dim bin = DirectCast(Tag.Tag.Pictures(0).Data.Data, Byte())
                Dim Cover = System.Drawing.Image.FromStream(New IO.MemoryStream(bin))
                Return Cover
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Function GetAlbumArts(path As String) As List(Of CoverItem)
        Dim Arts As New List(Of CoverItem)
        Try
            Dim Tag = TagLib.File.Create(path)
            If Tag.Tag.Pictures.Length >= 1 Then
                For Each art In Tag.Tag.Pictures
                    Dim bin = DirectCast(art.Data.Data, Byte())
                    Arts.Add(New CoverItem(System.Drawing.Image.FromStream(New IO.MemoryStream(bin)), art.Type))
                    bin = Nothing
                Next
                Return Arts
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Sub SaveToPng(ByVal visual As ImageSource, ByVal fileName As String)
        Dim encoder = New PngBitmapEncoder()
        SaveUsingEncoder(visual, fileName, encoder)
    End Sub
    Private Shared Sub SaveUsingEncoder(ByVal source As ImageSource, ByVal fileName As String, ByVal encoder As BitmapEncoder)
        Dim frame As BitmapFrame = BitmapFrame.Create(source)
        encoder.Frames.Add(frame)
        Using stream = IO.File.Create(fileName)
            encoder.Save(stream)
        End Using
    End Sub
#Region "Enums"
    Public Enum DragDropPlaylistBehaviour
        Replace = 0
        AddToFirst = 1
        AddToLast = 2
        AddToNext = 3
        AddToLastPlay = 4
    End Enum
    Public Enum FileSize
        TB
        GB
        MB
        KB
        B
    End Enum
#End Region
End Class