Imports System.ComponentModel
Imports WpfPlayerAPI.Anes08.MuPlay

Class MainWindow
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
    Dim WithEvents API As New WpfPlayerAPI.Anes08.MuPlay.API
    Dim WithEvents UI_MANAGER As New Forms.Timer With {.Interval = 500}
    Dim WithEvents UI_PEAK_MANAGER As New Forms.Timer With {.Interval = 33}
    Private SET_FROM_INSIDE As Boolean = False

    Private Async Sub API_MediaLoaded() Handles API.MediaLoaded
        UI_MANAGER.Stop()
        Await Dispatcher.BeginInvoke(Async Sub()
                                         Main_Cover.Source = ImageSourceFromBitmap(Await API.GetCover)
                                         Main_Background.Source = Main_Cover.Source
                                         Main_Track.Maximum = Await API.GetLength
                                         Main_Len.Text = API.SecsToMins(Main_Track.Maximum)
                                         Dim Tags = Await API.GetTags
                                         Main_Title.Text = Tags.Title
                                         Main_Artist.Text = Tags.Artist
                                         UI_MANAGER.Start()
                                         UI_PEAK_MANAGER.Start()
                                     End Sub)
    End Sub

    Private Sub API_RepeatChanged(NewType As API.RepeatBehaviour) Handles API.RepeatChanged

    End Sub

    Private Sub API_ShuffleChanged(NewType As API.RepeatBehaviour) Handles API.ShuffleChanged

    End Sub

    Private Async Sub API_StateChanged(State As API.State) Handles API.StateChanged
        Await Dispatcher.BeginInvoke(Sub()
                                         Select Case State
                                             Case API.State.MediaLoaded
                                                 Main_PlayPause.Content = "Loaded"
                                             Case API.State.Paused
                                                 Main_PlayPause.Content = "Play"
                                             Case API.State.Playing
                                                 Main_PlayPause.Content = "Pause"
                                         End Select
                                     End Sub)
    End Sub

    Private Async Sub API_VolumeChanged(NewVal As Single, IsMuted As Boolean) Handles API.VolumeChanged
        SET_FROM_INSIDE = True
        Await Dispatcher.BeginInvoke(Sub()
                                         Main_Volume.Value = Math.Round(NewVal * 100)
                                     End Sub)
        SET_FROM_INSIDE = False
    End Sub

    Private Async Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If API.IsMuPlayAvailable Then
            API_MediaLoaded()
            Dim Tit = Title
            Title = "MuPlay Is Available"
            Await Task.Delay(1000)
            Title = Tit
            Await Task.Delay(1000)
            Title = "MuPlay Is Available"
            Await Task.Delay(1000)
            Title = Tit
            Await Task.Delay(1000)
            Title = "MuPlay Is Available"
            Await Task.Delay(1000)
            Title = Tit
            Await Task.Delay(1000)
        Else
            Dim Tit = Title
            Title = "MuPlay Is Not Available"
            Await Task.Delay(1000)
            Title = Tit
            Await Task.Delay(1000)
            Title = "MuPlay Is Not Available"
            Await Task.Delay(1000)
            Title = Tit
            Await Task.Delay(1000)
            Title = "MuPlay Is Not Available"
            Await Task.Delay(1000)
            Title = Tit
            Await Task.Delay(1000)
        End If
    End Sub

    Private Async Sub UI_MANAGER_Tick(sender As Object, e As EventArgs) Handles UI_MANAGER.Tick
        If Main_Track.IsMouseOver = False Then
            Main_Track.Value = Await API.GetPosition
        End If
        Main_Pos.Text = API.SecsToMins(Main_Track.Value)
    End Sub

    Private Async Sub UI_PEAK_MANAGER_Tick(sender As Object, e As EventArgs) Handles UI_PEAK_MANAGER.Tick
        Dim Peak = Await API.GetPeak
        Main_Peak_L.Value = Peak.Left
        Main_Peak_R.Value = Peak.Right
    End Sub

    Private Sub Main_Previous_Click(sender As Object, e As RoutedEventArgs) Handles Main_Previous.Click
        API.SPrevious()
    End Sub

    Private Sub Main_PlayPause_Click(sender As Object, e As RoutedEventArgs) Handles Main_PlayPause.Click
        If Main_PlayPause.Content = "Play" Then
            API.SPlay()
        ElseIf Main_PlayPause.Content = "Pause" Then
            API.SPause()
        End If
    End Sub

    Private Sub Main_Next_Click(sender As Object, e As RoutedEventArgs) Handles Main_Next.Click
        API.SNext()
    End Sub

    Private Sub Main_Volume_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Volume.ValueChanged
        If SET_FROM_INSIDE = False Then
            API.SVolume(e.NewValue)
        End If
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        API.Dispose(True)
    End Sub

    Private Sub Main_Track_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Main_Track.ValueChanged
        If Main_Track.IsMouseOver Then
            API.SPosition(e.NewValue)
        End If
    End Sub
End Class
