Public Class Utils
    Public Class CoverItem
        Public Property Type As TagLib.PictureType
        Public Property Cover As System.Drawing.Image
        Public Sub New(_Cover As System.Drawing.Image, _Type As TagLib.PictureType)
            Cover = _Cover
            Type = _Type
        End Sub
    End Class
    Public Class PeakItem
        Public Property Master As Single
        Public Property Left As Single
        Public Property Right As Single
        Public Sub New(_Master As Single, _Left As Single, _Right As Single)
            Master = _Master
            Left = _Left
            Right = _Right
        End Sub
    End Class
    Public Shared ReadOnly Property FileFilters As String = "*.mp3|*.m4a|*.wav|*.aiff|*.mp2|*.mp1|*.ogg|*.wma|*.flac|*.alac|*.webm"
    Public Shared ReadOnly Property OFDFileFilters As String = "Supported Files|*.mp3;*.m4a;*.wav;*.aiff;*.mp2;*.mp1;*.ogg;*.wma;*.flac;*.alac;*.webm"
    Public Shared ReadOnly Property ImageFilters As String = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|All Files(*.*)|*.*"
    Public Shared ReadOnly Property brush_heart As New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/heart.png")))
    Public Shared ReadOnly Property brush_heart_filled As New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/heart_filled.png")))
    Public Shared ReadOnly Property AppDataPath As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MuPlay")
    Public Shared ReadOnly Property AppPath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, My.Application.Info.AssemblyName & ".exe")

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
    Public Shared Function SecsToMins(sec As Double, Optional LongText As Boolean = False) As String
        Try
            If LongText = False Then
                Dim Pos = sec / 60
                Dim Mins As String
                Try
                    Mins = Pos.ToString.Substring(0, Pos.ToString.IndexOf("."))
                Catch ex As Exception
                    Mins = Pos.ToString
                End Try
                Dim secs As String
                Try
                    secs = Pos.ToString.Substring(Pos.ToString.IndexOf("."), Pos.ToString.Length - Pos.ToString.IndexOf("."))
                Catch ex As Exception
                    secs = 0
                End Try
                Dim ConvSecs = secs * 60
                If Mins >= 10 AndAlso Math.Round(ConvSecs) >= 10 Then
                    Return New String(Mins & ":" & Math.Round(ConvSecs))
                ElseIf Mins < 10 AndAlso Math.Round(ConvSecs) >= 10 Then
                    Return New String("0" & Mins & ":" & Math.Round(ConvSecs))
                ElseIf Mins >= 10 AndAlso Math.Round(ConvSecs) < 10 Then
                    Return New String(Mins & ":" & "0" & Math.Round(ConvSecs))
                ElseIf Mins < 10 Or Mins = 0 AndAlso Math.Round(ConvSecs) < 10 Then
                    Return New String("0" & Mins & ":" & "0" & Math.Round(ConvSecs))
                ElseIf Mins < 10 AndAlso Math.Round(ConvSecs) > 59 Then
                    Return New String("0" & Mins & ":" & "00")
                ElseIf Mins > 10 AndAlso Math.Round(ConvSecs) > 59 Then
                    Return New String(Mins & ":" & "00")
                End If
            ElseIf LongText = True Then
                Dim DeciMins = sec / 60
                Dim IntMins = DeciMins.ToString.Substring(0, DeciMins.ToString.IndexOf("."))
                Dim DeciSecs = DeciMins.ToString.Substring(DeciMins.ToString.IndexOf("."), DeciMins.ToString.Length - DeciMins.ToString.IndexOf("."))
                Dim IntSecs = DeciSecs * 60
                If IntSecs < 10 Then
                    Return New String(IntMins & " min 0" & Math.Round(IntSecs) & " sec")
                Else
                    Return New String(IntMins & " min " & Math.Round(IntSecs) & " sec")
                End If
            End If
        Catch ex As Exception
            Return "ERROR"
        End Try
    End Function
    Public Shared Function GetMins(sec As Double)
        Dim mins = sec / 60
        Try
            Return mins.ToString.Substring(0, mins.ToString.IndexOf("."))
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Public Shared Function GetRestSecs(sec As Double)
        Dim mins = sec / 60
        Dim secs = mins.ToString.Substring(mins.ToString.IndexOf(".")) * 60
        Try
            Return secs
        Catch ex As Exception
            Return 0
        End Try
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
    Public Shared Sub ShowError(ex As Exception, Window As Window)
        Dim Ed
        Try
            Ed = New ErrorDialog(ex.StackTrace) With {.Owner = Window}
        Catch _ex As Exception
            Ed = New ErrorDialog(ex.StackTrace) With {.WindowStartupLocation = WindowStartupLocation.CenterScreen}
        End Try
        Ed.ShowDialog()
        Ed = Nothing
    End Sub
    Public Shared Function Shuffle(list As String()) As String()
        Dim Rnd As New Random()
        Dim j As Int32
        Dim temp As String
        Dim items As String() = list
        For n As Int32 = items.Length - 1 To 0 Step -1
            j = Rnd.Next(0, n + 1)
            ' Swap them.
            temp = items(n)
            items(n) = items(j)
            items(j) = temp
        Next n
        Return items
    End Function
    Public Shared Function Shuffle(list As List(Of String)) As List(Of String)
        Dim Rnd As New Random()
        Dim j As Int32
        Dim temp As String
        Dim items As List(Of String) = list
        For n As Int32 = items.Count - 1 To 0 Step -1
            j = Rnd.Next(0, n + 1)
            ' Swap them.
            temp = items(n)
            items(n) = items(j)
            items(j) = temp
        Next n
        Return items
    End Function
    Public Shared Function IntToMod(index As Integer) As Integer
        Select Case index
            Case 0
                Return Constants.NOMOD
            Case 1
                Return Constants.SHIFT
            Case 2
                Return Constants.CTRL
            Case 3
                Return Constants.ALT
            Case 4
                Return Constants.WIN
            Case Else
                Return 0
        End Select
    End Function
    Public Shared Function IntToModSTR(idx As Integer) As String
        Select Case idx
            Case 0
                Return "NOMOD"
            Case 1
                Return "SHIFT"
            Case 2
                Return "CTRL"
            Case 3
                Return "ALT"
            Case 4
                Return "WIN"
            Case Else
                Return 0
        End Select
    End Function
    Public Shared Sub UpdateSkin(ByVal skin As HandyControl.Data.SkinType, Window As HandyControl.Controls.Window)
        'HandyControl.Themes.SharedResourceDictionary.SharedDictionaries.Clear()
        Window.Resources.MergedDictionaries.Add(HandyControl.Tools.ResourceHelper.GetSkin(skin))
        Window.Resources.MergedDictionaries.Add(New ResourceDictionary With {.Source = New Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")})
        Window.OnApplyTemplate()
    End Sub
#Region "Enums"
    Public Enum DragDropPlaylistBehaviour
        Replace = 0
        AddToFirst = 1
        AddToLast = 2
        AddToNext = 3
        AddToLastPlay = 4
    End Enum
#End Region
End Class