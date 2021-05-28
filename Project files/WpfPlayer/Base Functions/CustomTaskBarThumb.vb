Imports Microsoft.WindowsAPICodePack.Taskbar
''' <summary>
''' A helper class to show custom taskbar thumbnails.
''' </summary>
Public Class CustomTaskBarThumb
    Public Class Binding
        Public Property BoundFrom As Object
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
        End Enum
    End Class
    Public Property Window As Window
    Public Property Hwnd As IntPtr
    Public Property OHwnd As IntPtr
    Public Property Thumbnails As New List(Of TabbedThumbnail)
    Public WriteOnly Property ActiveTab As Integer
        Set(value As Integer)
            TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(Thumbnails(value))
        End Set
    End Property
    Public ReadOnly Property Preview As TabbedThumbnail
        Get
            Return TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(Hwnd)
        End Get
    End Property
    Public Property AutoHandleMinimizeMaximizeEvents As Boolean = False
    Public Property BoundThumbnails As New List(Of Binding)
    Dim WithEvents BoundThumbnailsUpdator As New Forms.Timer With {.Interval = 16, .Enabled = True}
#Region "Events"
    Private _Activated As Boolean = True
    Private Sub Window_StateChanged(sender As Object, e As EventArgs)
        If Window.WindowState = WindowState.Minimized Then
            _Activated = False
        Else
            _Activated = True
        End If
    End Sub
    Private Sub TabbedThumbnailActivated(sender As Object, e As TabbedThumbnailEventArgs)
        If Not _Activated Then
            _Activated = True
            Window.WindowState = WindowState.Normal
        End If
    End Sub

    Private Sub TabbedThumbnailMaximized(sender As Object, e As TabbedThumbnailEventArgs)
        _Activated = True
        Window.WindowState = WindowState.Maximized
    End Sub

    Private Sub TabbedThumbnailMinimized(sender As Object, e As TabbedThumbnailEventArgs)
        _Activated = False
        Window.WindowState = WindowState.Minimized
    End Sub
#End Region
    Public Sub New(Wnd As Window)
        Window = Wnd
        Dim Hlper As New Interop.WindowInteropHelper(Wnd)
        If Hlper.Handle = IntPtr.Zero Then
            Hwnd = Hlper.EnsureHandle
            OHwnd = Hlper.Owner
        Else
            Hwnd = Hlper.Handle
            OHwnd = Hlper.Owner
        End If
        AddHandler Window.StateChanged, AddressOf Window_StateChanged
    End Sub
    Public Sub AddThumbnail(Thumb As TabbedThumbnail)
        Try
            TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(Thumb)
            Thumbnails.Add(Thumb)
            If AutoHandleMinimizeMaximizeEvents Then
                AddHandler Thumb.TabbedThumbnailActivated, AddressOf TabbedThumbnailActivated
                AddHandler Thumb.TabbedThumbnailMaximized, AddressOf TabbedThumbnailMaximized
                AddHandler Thumb.TabbedThumbnailMinimized, AddressOf TabbedThumbnailMinimized
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Function AddThumbnail(Frame As Boolean, Image As System.Drawing.Bitmap, Icon As System.Drawing.Icon, Title As String, ToolTip As String) As TabbedThumbnail
        Dim Thumb As New TabbedThumbnail(OHwnd, Hwnd)
        Thumb.DisplayFrameAroundBitmap = Frame
        Thumb.SetImage(Image)
        Thumb.SetWindowIcon(Icon)
        Thumb.Title = Title
        Thumb.Tooltip = ToolTip
        Try
            TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(Thumb)
            Thumbnails.Add(Thumb)
            If AutoHandleMinimizeMaximizeEvents Then
                AddHandler Thumb.TabbedThumbnailActivated, AddressOf TabbedThumbnailActivated
                AddHandler Thumb.TabbedThumbnailMaximized, AddressOf TabbedThumbnailMaximized
                AddHandler Thumb.TabbedThumbnailMinimized, AddressOf TabbedThumbnailMinimized
            End If
            Return Thumb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function AddThumbnail(Frame As Boolean, Image As BitmapSource, Icon As System.Drawing.Icon, Title As String, ToolTip As String) As TabbedThumbnail
        Dim Thumb As New TabbedThumbnail(Hwnd, Hwnd)
        Thumb.DisplayFrameAroundBitmap = Frame
        Thumb.SetImage(Image)
        Thumb.SetWindowIcon(Icon)
        Thumb.Title = Title
        Thumb.Tooltip = ToolTip
        Try
            TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(Thumb)
            Thumbnails.Add(Thumb)
            If AutoHandleMinimizeMaximizeEvents Then
                AddHandler Thumb.TabbedThumbnailActivated, AddressOf TabbedThumbnailActivated
                AddHandler Thumb.TabbedThumbnailMaximized, AddressOf TabbedThumbnailMaximized
                AddHandler Thumb.TabbedThumbnailMinimized, AddressOf TabbedThumbnailMinimized
            End If
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
        BoundThumbnails.Remove(BoundThumbnails.FirstOrDefault(Function(k) k.BindTo Is Thumbnails(Index)))
        TaskbarManager.Instance.TabbedThumbnail.RemoveThumbnailPreview(Thumbnails(Index))
    End Sub
    Public Sub Bind(ByRef Image As Image, Thumb As TabbedThumbnail)
        BoundThumbnails.Add(New Binding(Image, Thumb, Binding.BindingType.Image))
    End Sub
    Public Sub Bind(ByRef Bitmap As System.Drawing.Bitmap, Thumb As TabbedThumbnail)
        BoundThumbnails.Add(New Binding(Bitmap, Thumb, Binding.BindingType.Bitmap))
    End Sub
    Public Sub Bind(ByRef Source As BitmapSource, Thumb As TabbedThumbnail)
        BoundThumbnails.Add(New Binding(Source, Thumb, Binding.BindingType.Source))
    End Sub
    Public Sub Bind(Binding As Binding)
        BoundThumbnails.Add(Binding)
    End Sub

    Private Sub BoundThumbnailsUpdator_Tick(sender As Object, e As EventArgs) Handles BoundThumbnailsUpdator.Tick 'fix image size , complete single instance
        Dim UpdatedThumbs As New List(Of TabbedThumbnail)
        For Each _bind In BoundThumbnails
            Select Case _bind.Type
                Case Binding.BindingType.Image
                    With CType(_bind.BoundFrom, Image).Source
                        Window.Width = .Width
                        Window.Height = .Height
                    End With
                    _bind.BindTo.InvalidatePreview()
                    _bind.BindTo.SetImage(CType(_bind.BoundFrom, Image).Source)
                    UpdatedThumbs.Add(_bind.BindTo)
                Case Binding.BindingType.Bitmap
                    With CType(_bind.BoundFrom, System.Drawing.Bitmap)
                        Window.Width = .Width
                        Window.Height = .Height
                    End With
                    _bind.BindTo.InvalidatePreview()
                    _bind.BindTo.SetImage(CType(_bind.BoundFrom, System.Drawing.Bitmap))
                    UpdatedThumbs.Add(_bind.BindTo)
                Case Binding.BindingType.Source
                    With CType(_bind.BoundFrom, BitmapSource)
                        Window.Width = .Width
                        Window.Height = .Height
                    End With
                    _bind.BindTo.InvalidatePreview()
                    _bind.BindTo.SetImage(CType(_bind.BoundFrom, BitmapSource))
                    UpdatedThumbs.Add(_bind.BindTo)
                Case Binding.BindingType.Player
                    Window.Width = 200
                    Window.Height = 100
                    _bind.BindTo.InvalidatePreview()
                    _bind.BindTo.SetImage(CType(_bind.BoundFrom, Player).CreateVisualizer(_bind.VisualizerType, 200, 100, _bind.Color1, _bind.Color2, _bind.Color3, _bind.ColorBackground, _bind.LineWidth, _bind.PeakWidth, _bind.Distance, _bind.PeakDelay, False, False, False))
                    UpdatedThumbs.Add(_bind.BindTo)
            End Select
        Next
        For Each thumb In Thumbnails
            If Not UpdatedThumbs.Contains(thumb) Then
                thumb.InvalidatePreview()
            End If
        Next
    End Sub
End Class
