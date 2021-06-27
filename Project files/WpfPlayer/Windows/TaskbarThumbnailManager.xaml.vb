Imports Microsoft.WindowsAPICodePack.Taskbar
Imports System
Imports HandyControl.Data
Imports System.ComponentModel

Public Class TaskbarThumbnailManager
    Public TaskbarThumbnailManager As New CustomTaskBarThumb()
    Dim MainWindow As MainWindow = TryCast(Application.Current.MainWindow, MainWindow)
    Dim LinkPlayer As Player = MainWindow.MainPlayer
    'Full size : 546 ; Add only size : 260 ; Normal size : 112
#Region "Debug Only"
    Dim WithEvents thumb As TabbedThumbnail = Nothing
    Public Sub SetThumb(i As Integer)
        thumb = TaskbarThumbnailManager.AddThumbnail(True, New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png")), System.Drawing.SystemIcons.Error, "Debug", "Debug Tip", i)
        Dim rnd As New Random
        Dim bnd = CustomTaskBarThumb.Binding.FromVisualiser(LinkPlayer, thumb, rnd.Next(0, 5), Drawing.Color.Black, Drawing.Color.Red, Drawing.Color.Empty, Drawing.Color.Black, 2, 1, 250, 2) 'rnd.Next(0, 5) = 0 <= i < 5
        bnd.Owner = Me
        bnd.OwnerIndex = i
        TaskbarThumbnailManager.Bind(bnd)
    End Sub
#End Region
    Private Sub TaskbarThumbnailManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'COLOR 1
        Dim picker1 = HandyControl.Tools.SingleOpenHelper.CreateControl(Of HandyControl.Controls.ColorPicker)()
        Dim window1 As New HandyControl.Controls.PopupWindow With {.PopupElement = picker1}
        AddHandler picker1.Canceled, Sub()
                                         window1.Hide()
                                     End Sub
        AddHandler picker1.Confirmed, Sub()
                                          VIS_CLR1_WO = System.Drawing.Color.FromArgb(picker1.SelectedBrush.Color.A, picker1.SelectedBrush.Color.R, picker1.SelectedBrush.Color.G, picker1.SelectedBrush.Color.B)
                                          window1.Hide()
                                      End Sub
        AddHandler VA_CLR1.Click, Sub()
                                      window1.ShowDialog()
                                  End Sub
        'COLOR 2
        Dim picker2 = HandyControl.Tools.SingleOpenHelper.CreateControl(Of HandyControl.Controls.ColorPicker)()
        Dim window2 As New HandyControl.Controls.PopupWindow With {.PopupElement = picker2}
        AddHandler picker2.Canceled, Sub()
                                         window2.Hide()
                                     End Sub
        AddHandler picker2.Confirmed, Sub()
                                          VIS_CLR2_WO = System.Drawing.Color.FromArgb(picker2.SelectedBrush.Color.A, picker2.SelectedBrush.Color.R, picker2.SelectedBrush.Color.G, picker2.SelectedBrush.Color.B)
                                          window2.Hide()
                                      End Sub
        AddHandler VA_CLR2.Click, Sub()
                                      window2.ShowDialog()
                                  End Sub
        'COLOR 3
        Dim picker3 = HandyControl.Tools.SingleOpenHelper.CreateControl(Of HandyControl.Controls.ColorPicker)()
        Dim window3 As New HandyControl.Controls.PopupWindow With {.PopupElement = picker3}
        AddHandler picker3.Canceled, Sub()
                                         window3.Hide()
                                     End Sub
        AddHandler picker3.Confirmed, Sub()
                                          VIS_CLR3_WO = System.Drawing.Color.FromArgb(picker3.SelectedBrush.Color.A, picker3.SelectedBrush.Color.R, picker3.SelectedBrush.Color.G, picker3.SelectedBrush.Color.B)
                                          window3.Hide()
                                      End Sub
        AddHandler VA_CLR3.Click, Sub()
                                      window3.ShowDialog()
                                  End Sub
        'COLOR BG
        Dim pickerBG = HandyControl.Tools.SingleOpenHelper.CreateControl(Of HandyControl.Controls.ColorPicker)()
        Dim windowBG As New HandyControl.Controls.PopupWindow With {.PopupElement = pickerBG}
        AddHandler pickerBG.Canceled, Sub()
                                          VIS_CLRBG_WO = System.Drawing.Color.FromArgb(pickerBG.SelectedBrush.Color.A, pickerBG.SelectedBrush.Color.R, pickerBG.SelectedBrush.Color.G, pickerBG.SelectedBrush.Color.B)
                                          windowBG.Hide()
                                      End Sub
        AddHandler pickerBG.Confirmed, Sub()
                                           windowBG.Hide()
                                       End Sub
        AddHandler VA_CLRBG.Click, Sub()
                                       windowBG.ShowDialog()
                                   End Sub
    End Sub
    Private Sub TaskbarThumbnailManager_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub

    Private Sub Main_ThumbRefresh_Click(sender As Object, e As RoutedEventArgs) Handles Main_ThumbRefresh.Click
        Main_ThumbList.Items.Clear()
        Dim i = 0
        For Each wnd In TaskbarThumbnailManager.Wnds
            If wnd.Thumbnail IsNot Nothing Then
                Main_ThumbList.Items.Add(i & "-" & wnd.Thumbnail.Title)
            Else
                Main_ThumbList.Items.Add(i & "-Nothing")
            End If
            i += 1
        Next
    End Sub

    Private Sub Main_ThumbManage_Click(sender As Object, e As RoutedEventArgs) Handles Main_ThumbManage.Click
        MessageBox.Show(Me, "N/A ATM", "MuPlay", MessageBoxButton.OK, MessageBoxImage.Warning)
    End Sub

    Private Sub Main_ThumbAdd_Click(sender As Object, e As RoutedEventArgs) Handles Main_ThumbAdd.Click
        BeginAnimation(HeightProperty, New Animation.DoubleAnimation(260, New Duration(TimeSpan.FromMilliseconds(500))))
    End Sub

    Private Sub Main_ThumbRemove_Click(sender As Object, e As RoutedEventArgs) Handles Main_ThumbRemove.Click
        Try
            If Main_ThumbList.SelectedIndex <> -1 Then
                TaskbarThumbnailManager.RemoveThumbnail(Main_ThumbList.SelectedIndex)
                Main_ThumbList.Items.RemoveAt(Main_ThumbList.SelectedIndex)
            End If
        Catch
        End Try
    End Sub
#Region "Custom VIS Builder"
    Private VIS_Type As Player.Visualizers
    Private VIS_HIT As Integer = 85
    Private VIS_WDT As Integer = 167
    Private VIS_CLR1 As System.Drawing.Color = System.Drawing.Color.Red
    Private VIS_CLR2 As System.Drawing.Color = System.Drawing.Color.Black
    Private VIS_CLR3 As System.Drawing.Color = System.Drawing.Color.Green
    Private VIS_CLRBG As System.Drawing.Color = System.Drawing.Color.Empty
    Private VIS_LN_WDT As Integer = 2
    Private VIS_PK_WDT As Integer = 2
    Private VIS_DST As Integer = 1
    Private VIS_PKDLY As Integer = 200
    WriteOnly Property VIS_Type_WO As Player.Visualizers
        Set(value As Player.Visualizers)
            VIS_Type = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_HIT_WO As Integer
        Set(value As Integer)
            VIS_HIT = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_WDT_WO As Integer
        Set(value As Integer)
            VIS_WDT = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_CLR1_WO As System.Drawing.Color
        Set(value As System.Drawing.Color)
            VIS_CLR1 = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_CLR2_WO As System.Drawing.Color
        Set(value As System.Drawing.Color)
            VIS_CLR2 = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_CLR3_WO As System.Drawing.Color
        Set(value As System.Drawing.Color)
            VIS_CLR3 = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_CLRBG_WO As System.Drawing.Color
        Set(value As System.Drawing.Color)
            VIS_CLRBG = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_LN_WDT_WO As Integer
        Set(value As Integer)
            VIS_LN_WDT = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_PK_WDT_WO As Integer
        Set(value As Integer)
            VIS_PK_WDT = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_DIST_WO As Integer
        Set(value As Integer)
            VIS_DST = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    WriteOnly Property VIS_PKDLY_WO As Integer
        Set(value As Integer)
            VIS_PKDLY = value
            Try
                VA_PRVW.Source = Utils.ImageSourceFromBitmap(LinkPlayer.CreateVisualizer(VIS_Type, VIS_WDT, VIS_HIT, VIS_CLR1, VIS_CLR2, VIS_CLR3, VIS_CLRBG, VIS_LN_WDT, VIS_PK_WDT, VIS_DST, VIS_PKDLY, False, False, False))
            Catch
                'VA_PRVW.Source = New BitmapImage(New Uri("pack://application:,,,/WpfPlayer;component/Res/MuPlayLogoF.png"))
            End Try
        End Set
    End Property
    Private Sub VA_CB_TYPE_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles VA_CB_TYPE.SelectionChanged
        Select Case VA_CB_TYPE.SelectedIndex
            Case 4 'Plugin
            Case Else
                VIS_Type_WO = VA_CB_TYPE.SelectedIndex
        End Select
    End Sub

    Private Sub VA_HIT_TextChanged(sender As Object, e As TextChangedEventArgs) Handles VA_HIT.TextChanged
        VIS_HIT_WO = Utils.NumFromSTR(VA_HIT.Text)
    End Sub

    Private Sub VA_WDT_TextChanged(sender As Object, e As TextChangedEventArgs) Handles VA_WDT.TextChanged
        VIS_WDT_WO = Utils.NumFromSTR(VA_WDT.Text)
    End Sub

    Private Sub VA_LNWDT_TextChanged(sender As Object, e As TextChangedEventArgs) Handles VA_LNWDT.TextChanged
        VIS_LN_WDT_WO = Utils.NumFromSTR(VA_LNWDT.Text)
    End Sub

    Private Sub VA_PKWDT_TextChanged(sender As Object, e As TextChangedEventArgs) Handles VA_PKWDT.TextChanged
        VIS_PKDLY_WO = Utils.NumFromSTR(VA_PKWDT.Text)
    End Sub

    Private Sub VA_DST_TextChanged(sender As Object, e As TextChangedEventArgs) Handles VA_DST.TextChanged
        VIS_DIST_WO = Utils.NumFromSTR(VA_DST.Text)
    End Sub

    Private Sub VA_PKDLY_TextChanged(sender As Object, e As TextChangedEventArgs) Handles VA_PKDLY.TextChanged
        VIS_PKDLY_WO = Utils.NumFromSTR(VA_PKDLY.Text)
    End Sub

#End Region
    Private Sub THUMB_ADD_Click(sender As Object, e As RoutedEventArgs) Handles THUMB_ADD.Click
        Select Case THUMB_BNGTYP.SelectedIndex
            Case 0 'Static Image
                Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Filter = Utils.ImageFilters}
                If OFD.ShowDialog Then
                    Dim idx = TaskbarThumbnailManager.AddWindow
                    TaskbarThumbnailManager.AddThumbnail(False, System.Drawing.Image.FromFile(OFD.FileName), System.Drawing.SystemIcons.Shield, THUMB_TITLE.Text, THUMB_TIP.Text, idx)
                    TaskbarThumbnailManager.Bind(System.Drawing.Image.FromFile(OFD.FileName), idx)
                    BeginAnimation(HeightProperty, New Animation.DoubleAnimation(112, New Duration(TimeSpan.FromMilliseconds(500))))
                End If
            Case 1 'Media Cover
                Dim idx = TaskbarThumbnailManager.AddWindow
                TaskbarThumbnailManager.AddThumbnail(False, MainWindow.Home_NowPlaying.Source, System.Drawing.SystemIcons.Shield, THUMB_TITLE.Text, THUMB_TIP.Text, idx)
                TaskbarThumbnailManager.Bind(MainWindow.Home_NowPlaying, idx)
                BeginAnimation(HeightProperty, New Animation.DoubleAnimation(112, New Duration(TimeSpan.FromMilliseconds(500))))
            Case 2 'Visualiser
                ThumbVisualiserMaker.Visibility = Visibility.Visible
                BeginAnimation(HeightProperty, New Animation.DoubleAnimation(546, New Duration(TimeSpan.FromMilliseconds(500))))
            Case 3 'Media Info
                Dim idx = TaskbarThumbnailManager.AddWindow
                TaskbarThumbnailManager.Bind(New CustomTaskBarThumb.Binding(LinkPlayer, TaskbarThumbnailManager.AddThumbnail(False, MainWindow.Home_NowPlaying.Source, System.Drawing.SystemIcons.Shield, THUMB_TITLE.Text, THUMB_TIP.Text, idx), CustomTaskBarThumb.Binding.BindingType.MediaInfo))
                BeginAnimation(HeightProperty, New Animation.DoubleAnimation(112, New Duration(TimeSpan.FromMilliseconds(500))))
        End Select
    End Sub

    Private Async Sub VA_ADD_Click(sender As Object, e As RoutedEventArgs) Handles VA_ADD.Click
        Dim idx = TaskbarThumbnailManager.AddWindow
        TaskbarThumbnailManager.Bind(New CustomTaskBarThumb.Binding(Nothing, TaskbarThumbnailManager.AddThumbnail(False, MainWindow.Home_NowPlaying.Source, System.Drawing.SystemIcons.Shield, THUMB_TITLE.Text, THUMB_TIP.Text, idx), CustomTaskBarThumb.Binding.BindingType.Player) With {.Color1 = VIS_CLR1, .Color2 = VIS_CLR2, .Color3 = VIS_CLR3, .ColorBackground = VIS_CLRBG, .Distance = VIS_DST, .LineWidth = VIS_LN_WDT, .Owner = TaskbarThumbnailManager.Wnds(idx).Window, .OwnerIndex = idx, .PeakDelay = VIS_PKDLY, .PeakWidth = VIS_PK_WDT, .VisualizerType = VIS_Type})
        BeginAnimation(HeightProperty, New Animation.DoubleAnimation(112, New Duration(TimeSpan.FromMilliseconds(500))))
        Await Task.Delay(500)
        ThumbVisualiserMaker.Visibility = Visibility.Hidden
    End Sub
End Class
