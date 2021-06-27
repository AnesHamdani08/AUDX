Imports System.Drawing
Imports Microsoft.WindowsAPICodePack.Taskbar
''' <summary>
''' A helper class to show custom taskbar thumbnails.
''' </summary>
Public Class CustomTaskBarThumb
    Public Class Binding
        Public Property BoundFrom As Object
        Public Property Owner As Window
        Public Property OwnerIndex As Integer
        Public Property BindTo As TabbedThumbnail
        Public Property Type As BindingType
        Public Property VisualizerType As Player.Visualizers
        Public Property Color1 As System.Drawing.Color
        Public Property Color2 As System.Drawing.Color
        Public Property Color3 As System.Drawing.Color
        Public Property ColorBackground As System.Drawing.Color
        Public Property LineWidth As Integer
        Public Property PeakWidth As Integer
        Public Property PeakDelay As Integer
        Public Property Distance As Integer
        Public Sub New(_From As Object, _To As TabbedThumbnail, _Type As BindingType)
            BoundFrom = _From
            BindTo = _To
            Type = _Type
        End Sub
        Public Shared Function FromVisualiser(_From As Player, _To As TabbedThumbnail, VisType As Player.Visualizers, Clr1 As System.Drawing.Color, Clr2 As System.Drawing.Color, Clr3 As System.Drawing.Color, ClrBackground As System.Drawing.Color, LWidth As Integer, Dist As Integer, PkDelay As Integer, PkWidth As Integer) As Binding
            Return New Binding(_From, _To, BindingType.Player) With {.VisualizerType = VisType, .Color1 = Clr1, .Color2 = Clr2, .Color3 = Clr3, .Distance = Dist, .LineWidth = LWidth, .PeakDelay = PkDelay, .PeakWidth = PkWidth, .ColorBackground = ClrBackground}
        End Function
        Public Enum BindingType
            Image
            Bitmap
            Source
            Player
            MediaInfo
        End Enum
    End Class
    Public Class ThumbWindow
        Public Property Window As Window
        Public Property Hwnd As IntPtr
        Public Property OHwnd As IntPtr
        Public Property Thumbnail As TabbedThumbnail
    End Class
    Public Property Wnds As New List(Of ThumbWindow)
    Public Property BoundThumbnails As New List(Of Binding)
    Dim WithEvents BoundThumbnailsUpdator As New Forms.Timer With {.Interval = 16, .Enabled = True}
#Region "Events"
    Private _Activated As Boolean = True
    Private Sub Window_StateChanged(sender As Object, e As EventArgs)
        For Each window In Wnds
            window.Window.WindowState = WindowState.Minimized
        Next
    End Sub
#End Region
    Public Sub New()
    End Sub
    Public Function AddWindow() As Integer
        Dim Twnd As New ThumbWindow
        Dim Wnd As New Window With {.ShowInTaskbar = True, .Owner = Application.Current.MainWindow}
        Wnd.Show()
        Wnd.WindowState = WindowState.Minimized
        Twnd.Window = Wnd
        Dim Hlper As New Interop.WindowInteropHelper(Wnd)
        If Hlper.Handle = IntPtr.Zero Then
            Twnd.Hwnd = Hlper.EnsureHandle
            'Twnd.OHwnd = Hlper.Owner
            Twnd.OHwnd = Hlper.Handle
        Else
            Twnd.Hwnd = Hlper.Handle
            'Twnd.OHwnd = Hlper.Owner
            Twnd.OHwnd = Hlper.Handle
        End If
        AddHandler Wnd.StateChanged, AddressOf Window_StateChanged
        Wnds.Add(Twnd)
        Return Wnds.Count - 1
    End Function
    Public Sub AddThumbnail(Thumb As TabbedThumbnail, WindowIndex As Integer)
        Try
            TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(Thumb)
            Wnds.Item(WindowIndex).Thumbnail = Thumb
        Catch ex As Exception
        End Try
    End Sub
    Public Function AddThumbnail(Frame As Boolean, Image As System.Drawing.Bitmap, Icon As System.Drawing.Icon, Title As String, ToolTip As String, WindowIndex As Integer) As TabbedThumbnail
        Dim Thumb As New TabbedThumbnail(Wnds.Item(WindowIndex).OHwnd, Wnds.Item(WindowIndex).Hwnd)
        Thumb.DisplayFrameAroundBitmap = Frame
        Thumb.SetImage(Image)
        Thumb.SetWindowIcon(Icon)
        Thumb.Title = Title
        Thumb.Tooltip = ToolTip
        Try
            TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(Thumb)
            Wnds.Item(WindowIndex).Thumbnail = Thumb
            Return Thumb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function AddThumbnail(Frame As Boolean, Image As BitmapSource, Icon As System.Drawing.Icon, Title As String, ToolTip As String, WindowIndex As Integer) As TabbedThumbnail
        Dim Thumb As New TabbedThumbnail(Wnds.Item(WindowIndex).OHwnd, Wnds.Item(WindowIndex).Hwnd)
        Thumb.DisplayFrameAroundBitmap = Frame
        Thumb.SetImage(Image)
        Thumb.SetWindowIcon(Icon)
        Thumb.Title = Title
        Thumb.Tooltip = ToolTip
        Try
            TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(Thumb)
            Wnds.Item(WindowIndex).Thumbnail = Thumb
            Return Thumb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Sub RemoveThumbnail(Thumb As TabbedThumbnail)
        BoundThumbnails.Remove(BoundThumbnails.FirstOrDefault(Function(k) k.BindTo Is Thumb))
        TaskbarManager.Instance.TabbedThumbnail.RemoveThumbnailPreview(Thumb)
    End Sub
    Public Sub RemoveThumbnail(Index As Integer)
        Try
            BoundThumbnails.Remove(BoundThumbnails.FirstOrDefault(Function(k) k.BindTo Is Wnds.Item(Index).Thumbnail))
            TaskbarManager.Instance.TabbedThumbnail.RemoveThumbnailPreview(Wnds.Item(Index).Thumbnail)
            Wnds.Item(Index).Window.Close()
            'Wnds.RemoveAt(Index) --Removing from list requires re-assigning every item index
        Catch
        End Try
    End Sub
    Public Sub Bind(ByRef Image As Controls.Image, Index As Integer)
        BoundThumbnails.Add(New Binding(Image, Wnds.Item(Index).Thumbnail, Binding.BindingType.Image) With {.Owner = Wnds.Item(Index).Window, .OwnerIndex = Index})
    End Sub
    Public Sub Bind(ByRef Bitmap As System.Drawing.Bitmap, Index As Integer)
        BoundThumbnails.Add(New Binding(Bitmap, Wnds.Item(Index).Thumbnail, Binding.BindingType.Bitmap) With {.Owner = Wnds.Item(Index).Window, .OwnerIndex = Index})
    End Sub
    Public Sub Bind(ByRef Source As BitmapSource, Index As Integer)
        BoundThumbnails.Add(New Binding(Source, Wnds.Item(Index).Thumbnail, Binding.BindingType.Source) With {.Owner = Wnds.Item(Index).Window, .OwnerIndex = Index})
    End Sub
    Public Sub Bind(Binding As Binding)
        BoundThumbnails.Add(Binding)
    End Sub

    Private Sub BoundThumbnailsUpdator_Tick(sender As Object, e As EventArgs) Handles BoundThumbnailsUpdator.Tick 'fix image size        
        Try
            For Each _bind In BoundThumbnails
                Select Case _bind.Type
                    Case Binding.BindingType.Image
                        With CType(_bind.BoundFrom, Controls.Image).Source
                            Wnds(_bind.OwnerIndex).Window.Width = .Width
                            Wnds(_bind.OwnerIndex).Window.Height = .Height
                        End With
                        _bind.BindTo.InvalidatePreview()
                        _bind.BindTo.SetImage(CType(_bind.BoundFrom, Controls.Image).Source)
                    Case Binding.BindingType.Bitmap
                        With CType(_bind.BoundFrom, System.Drawing.Bitmap)
                            Wnds(_bind.OwnerIndex).Window.Width = .Width
                            Wnds(_bind.OwnerIndex).Window.Height = .Height
                        End With
                        _bind.BindTo.InvalidatePreview()
                        _bind.BindTo.SetImage(CType(_bind.BoundFrom, System.Drawing.Bitmap))
                    Case Binding.BindingType.Source
                        With CType(_bind.BoundFrom, BitmapSource)
                            Wnds(_bind.OwnerIndex).Window.Width = .PixelWidth
                            Wnds(_bind.OwnerIndex).Window.Height = .PixelHeight
                        End With
                        _bind.BindTo.InvalidatePreview()
                        _bind.BindTo.SetImage(CType(_bind.BoundFrom, BitmapSource))
                    Case Binding.BindingType.Player
                        Wnds(_bind.OwnerIndex).Window.Width = 200
                        Wnds(_bind.OwnerIndex).Window.Height = 100
                        _bind.BindTo.InvalidatePreview()
                        _bind.BindTo.SetImage(CType(_bind.BoundFrom, Player).CreateVisualizer(_bind.VisualizerType, 200, 100, _bind.Color1, _bind.Color2, _bind.Color3, _bind.ColorBackground, _bind.LineWidth, _bind.PeakWidth, _bind.Distance, _bind.PeakDelay, False, False, False))
                    Case Binding.BindingType.MediaInfo
                        Dim Player As Player = CType(_bind.BoundFrom, Player)
                        Dim mi As New Bitmap(500, 150)
                        Dim gr = Graphics.FromImage(mi)
                        gr.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        If Player.CurrentMediaCover IsNot Nothing Then
                            gr.DrawImage(Utils.BitmapFromImageSource(Player.CurrentMediaCover), 0, 0, 150, 150)
                        Else
                            gr.DrawImage(Utils.BitmapFromImageSource(New BitmapImage(New Uri("pack://application:,,,/Wpfplayer;component/Res/song.png"))), 0, 0, 150, 150)
                        End If
                        Dim size = gr.MeasureString(Player.CurrentMediaTitle, New Font("Arial", 16, FontStyle.Bold))
                        gr.DrawString(Player.CurrentMediaTitle, New Font("Arial", 16, FontStyle.Bold), Brushes.Black, 150, 75 - (size.Height / 2))
                        Dim size2 = gr.MeasureString(Player.CurrentMediaArtist, New Font("Arial", 14))
                        gr.DrawString(Player.CurrentMediaArtist, New Font("Arial", 14), Brushes.Black, 150, 100 - (size2.Height / 2))
                        gr.DrawString(Utils.SecsToMins(Player.GetPosition) & "/" & Utils.SecsToMins(Player.GetLength), New Font("Arial", 10, FontStyle.Italic), Brushes.Black, 420, 125)
                        'Min=150 Max=500
                        gr.DrawLine(New Pen(Brushes.Black, 5), New Point(150, 145), New Point(500, 145))
                        gr.DrawLine(New Pen(Brushes.DarkGray, 5), New Point(150, 145), New Point(Utils.PercentageToVal(Utils.ValToPercentage(Player.GetPosition, 0, Player.GetLength), 150, 500), 145))
                        _bind.BindTo.InvalidatePreview()
                        _bind.BindTo.SetImage(mi)
                End Select
            Next
        Catch
        End Try
    End Sub
End Class
