Imports System.ComponentModel

Public Class About
#Region "Matrix Helper"
    Private Async Sub About_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim Rnd As New Random
        Select Case Rnd.Next(0, 4)
            Case 1
                Select Case e.Key
                    Case Key.Left
                        MatLeftSlideAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatLeftSlideAnimation(Brushes.Black, 100)
                    Case Key.Right
                        MatRightSlideAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatRightSlideAnimation(Brushes.Black, 100)
                    Case Key.Up
                        MatUpSlideAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatUpSlideAnimation(Brushes.Black, 100)
                    Case Key.Down
                        MatDownSlideAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatDownSlideAnimation(Brushes.Black, 100)
                End Select
            Case 2
                Select Case e.Key
                    Case Key.Left
                        MatLeftArrowAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatLeftArrowAnimation(Brushes.Black, 100)
                    Case Key.Right
                        MatRightArrowAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatRightArrowAnimation(Brushes.Black, 100)
                    Case Key.Up
                        MatUpArrowAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatUpArrowAnimation(Brushes.Black, 100)
                    Case Key.Down
                        MatDownArrowAnimation(GetRGB(Rnd.Next(3)), 100)
                        Await Task.Delay(100)
                        MatDownArrowAnimation(Brushes.Black, 100)
                End Select
            Case 3
                Select Case e.Key
                    Case Key.Left
                        MatLeftArrowRGBAnimation(100)
                        Await Task.Delay(100)
                        MatLeftArrowAnimation(Brushes.Black, 100)
                    Case Key.Right
                        MatRightArrowRGBAnimation(100)
                        Await Task.Delay(100)
                        MatRightArrowAnimation(Brushes.Black, 100)
                    Case Key.Up
                        MatUpArrowRGBAnimation(100)
                        Await Task.Delay(100)
                        MatUpArrowAnimation(Brushes.Black, 100)
                    Case Key.Down
                        MatDownArrowRGBAnimation(100)
                        Await Task.Delay(100)
                        MatDownArrowAnimation(Brushes.Black, 100)
                End Select
        End Select
    End Sub
    Public Function GetRGB(i As Integer) As Brush
        Select Case i
            Case 0
                Return Brushes.Red
            Case 1
                Return Brushes.Green
            Case 2
                Return Brushes.Blue
            Case Else
                Return Brushes.Black
        End Select
    End Function
    Public Sub MatFillLine(Line As Integer, Color As Brush)
        Select Case Line
            Case 0
                R00.Fill = Color
                R01.Fill = Color
                R02.Fill = Color
                R03.Fill = Color
                R04.Fill = Color
            Case 1
                R10.Fill = Color
                R11.Fill = Color
                R12.Fill = Color
                R13.Fill = Color
                R14.Fill = Color
            Case 2
                R20.Fill = Color
                R21.Fill = Color
                R22.Fill = Color
                R23.Fill = Color
                R24.Fill = Color
            Case 3
                R30.Fill = Color
                R31.Fill = Color
                R32.Fill = Color
                R33.Fill = Color
                R34.Fill = Color
            Case 4
                R40.Fill = Color
                R41.Fill = Color
                R42.Fill = Color
                R43.Fill = Color
                R44.Fill = Color
        End Select
    End Sub
    Public Sub MatFillCollumn(Col As Integer, Color As Brush)
        Select Case Col
            Case 0
                R00.Fill = Color
                R10.Fill = Color
                R20.Fill = Color
                R30.Fill = Color
                R40.Fill = Color
            Case 1
                R01.Fill = Color
                R11.Fill = Color
                R21.Fill = Color
                R31.Fill = Color
                R41.Fill = Color
            Case 2
                R02.Fill = Color
                R12.Fill = Color
                R22.Fill = Color
                R32.Fill = Color
                R42.Fill = Color
            Case 3
                R03.Fill = Color
                R13.Fill = Color
                R23.Fill = Color
                R33.Fill = Color
                R43.Fill = Color
            Case 4
                R04.Fill = Color
                R14.Fill = Color
                R24.Fill = Color
                R34.Fill = Color
                R44.Fill = Color
        End Select
    End Sub
    Public Sub FillCube(Line As Integer, Col As Integer, Color As Brush)
        Select Case Line
            Case 0
                Select Case Col
                    Case 0
                        R00.Fill = Color
                    Case 1
                        R01.Fill = Color
                    Case 2
                        R02.Fill = Color
                    Case 3
                        R03.Fill = Color
                    Case 4
                        R04.Fill = Color
                End Select
            Case 1
                Select Case Col
                    Case 0
                        R10.Fill = Color
                    Case 1
                        R11.Fill = Color
                    Case 2
                        R12.Fill = Color
                    Case 3
                        R13.Fill = Color
                    Case 4
                        R14.Fill = Color
                End Select
            Case 2
                Select Case Col
                    Case 0
                        R20.Fill = Color
                    Case 1
                        R21.Fill = Color
                    Case 2
                        R22.Fill = Color
                    Case 3
                        R23.Fill = Color
                    Case 4
                        R24.Fill = Color
                End Select
            Case 3
                Select Case Col
                    Case 0
                        R30.Fill = Color
                    Case 1
                        R31.Fill = Color
                    Case 2
                        R32.Fill = Color
                    Case 3
                        R33.Fill = Color
                    Case 4
                        R34.Fill = Color
                End Select
            Case 4
                Select Case Col
                    Case 0
                        R40.Fill = Color
                    Case 1
                        R41.Fill = Color
                    Case 2
                        R42.Fill = Color
                    Case 3
                        R43.Fill = Color
                    Case 4
                        R44.Fill = Color
                End Select
        End Select
    End Sub
    Public Sub MatFillRandomCubes()
        Dim Rnd As New Random
        R00.Fill = GetRGB(Rnd.Next(0, 3))
        R01.Fill = GetRGB(Rnd.Next(0, 3))
        R02.Fill = GetRGB(Rnd.Next(0, 3))
        R03.Fill = GetRGB(Rnd.Next(0, 3))
        R04.Fill = GetRGB(Rnd.Next(0, 3))

        R10.Fill = GetRGB(Rnd.Next(0, 3))
        R11.Fill = GetRGB(Rnd.Next(0, 3))
        R12.Fill = GetRGB(Rnd.Next(0, 3))
        R13.Fill = GetRGB(Rnd.Next(0, 3))
        R14.Fill = GetRGB(Rnd.Next(0, 3))

        R20.Fill = GetRGB(Rnd.Next(0, 3))
        R21.Fill = GetRGB(Rnd.Next(0, 3))
        R22.Fill = GetRGB(Rnd.Next(0, 3))
        R23.Fill = GetRGB(Rnd.Next(0, 3))
        R24.Fill = GetRGB(Rnd.Next(0, 3))

        R30.Fill = GetRGB(Rnd.Next(0, 3))
        R31.Fill = GetRGB(Rnd.Next(0, 3))
        R32.Fill = GetRGB(Rnd.Next(0, 3))
        R33.Fill = GetRGB(Rnd.Next(0, 3))
        R34.Fill = GetRGB(Rnd.Next(0, 3))

        R40.Fill = GetRGB(Rnd.Next(0, 3))
        R41.Fill = GetRGB(Rnd.Next(0, 3))
        R42.Fill = GetRGB(Rnd.Next(0, 3))
        R43.Fill = GetRGB(Rnd.Next(0, 3))
        R44.Fill = GetRGB(Rnd.Next(0, 3))
    End Sub
    Public Sub MatClearCubes()
        MatFillLine(0, Brushes.Black)
        MatFillLine(1, Brushes.Black)
        MatFillLine(2, Brushes.Black)
        MatFillLine(3, Brushes.Black)
        MatFillLine(4, Brushes.Black)
    End Sub
    Public Async Sub MatDownSlideAnimation(Color As Brush, Delay As Integer)
        MatFillLine(0, Color)
        Await Task.Delay(Delay)
        MatFillLine(1, Color)
        Await Task.Delay(Delay)
        MatFillLine(2, Color)
        Await Task.Delay(Delay)
        MatFillLine(3, Color)
        Await Task.Delay(Delay)
        MatFillLine(4, Color)
    End Sub
    Public Async Sub MatUpSlideAnimation(Color As Brush, Delay As Integer)
        MatFillLine(4, Color)
        Await Task.Delay(Delay)
        MatFillLine(3, Color)
        Await Task.Delay(Delay)
        MatFillLine(2, Color)
        Await Task.Delay(Delay)
        MatFillLine(1, Color)
        Await Task.Delay(Delay)
        MatFillLine(0, Color)
    End Sub
    Public Async Sub MatRightSlideAnimation(Color As Brush, Delay As Integer)
        MatFillCollumn(0, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(1, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(2, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(3, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(4, Color)
    End Sub
    Public Async Sub MatLeftSlideAnimation(Color As Brush, Delay As Integer)
        MatFillCollumn(4, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(3, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(2, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(1, Color)
        Await Task.Delay(Delay)
        MatFillCollumn(0, Color)
    End Sub
    Public Async Sub MatDownArrowAnimation(Color As Brush, Delay As Integer)
        FillCube(0, 2, Color)
        Await Task.Delay(Delay)
        FillCube(0, 1, Color)
        FillCube(0, 3, Color)
        FillCube(1, 2, Color)
        Await Task.Delay(Delay)
        FillCube(0, 0, Color)
        FillCube(0, 4, Color)
        FillCube(1, 1, Color)
        FillCube(1, 3, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        FillCube(1, 0, Color)
        FillCube(1, 4, Color)
        FillCube(2, 1, Color)
        FillCube(2, 3, Color)
        FillCube(3, 2, Color)
        Await Task.Delay(Delay)
        FillCube(2, 0, Color)
        FillCube(2, 4, Color)
        FillCube(3, 1, Color)
        FillCube(3, 3, Color)
        FillCube(4, 2, Color)
        Await Task.Delay(Delay)
        FillCube(3, 0, Color)
        FillCube(3, 4, Color)
        FillCube(4, 1, Color)
        FillCube(4, 3, Color)
        Await Task.Delay(Delay)
        FillCube(4, 0, Color)
        FillCube(4, 4, Color)
    End Sub
    Public Async Sub MatUpArrowAnimation(Color As Brush, Delay As Integer)
        FillCube(4, 2, Color)
        Await Task.Delay(Delay)
        FillCube(4, 1, Color)
        FillCube(4, 3, Color)
        FillCube(3, 2, Color)
        Await Task.Delay(Delay)
        FillCube(4, 0, Color)
        FillCube(4, 4, Color)
        FillCube(3, 1, Color)
        FillCube(3, 3, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        FillCube(3, 0, Color)
        FillCube(3, 4, Color)
        FillCube(2, 1, Color)
        FillCube(2, 3, Color)
        FillCube(1, 2, Color)
        Await Task.Delay(Delay)
        FillCube(2, 0, Color)
        FillCube(2, 4, Color)
        FillCube(1, 1, Color)
        FillCube(1, 3, Color)
        FillCube(0, 2, Color)
        Await Task.Delay(Delay)
        FillCube(1, 0, Color)
        FillCube(1, 4, Color)
        FillCube(0, 1, Color)
        FillCube(0, 3, Color)
        Await Task.Delay(Delay)
        FillCube(0, 0, Color)
        FillCube(0, 4, Color)
    End Sub
    Public Async Sub MatDownArrowRGBAnimation(Delay As Integer)
        Dim color As Brush = Brushes.Red
        FillCube(0, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Green
        FillCube(0, 1, Color)
        FillCube(0, 3, Color)
        FillCube(1, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Blue
        FillCube(0, 0, Color)
        FillCube(0, 4, Color)
        FillCube(1, 1, Color)
        FillCube(1, 3, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Red
        FillCube(1, 0, Color)
        FillCube(1, 4, Color)
        FillCube(2, 1, Color)
        FillCube(2, 3, Color)
        FillCube(3, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Green
        FillCube(2, 0, Color)
        FillCube(2, 4, Color)
        FillCube(3, 1, Color)
        FillCube(3, 3, Color)
        FillCube(4, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Blue
        FillCube(3, 0, Color)
        FillCube(3, 4, Color)
        FillCube(4, 1, Color)
        FillCube(4, 3, Color)
        Await Task.Delay(Delay)
        color = Brushes.Red
        FillCube(4, 0, Color)
        FillCube(4, 4, Color)
    End Sub
    Public Async Sub MatUpArrowRGBAnimation(Delay As Integer)
        Dim color As Brush = Brushes.Red
        FillCube(4, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Green
        FillCube(4, 1, Color)
        FillCube(4, 3, Color)
        FillCube(3, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Blue
        FillCube(4, 0, Color)
        FillCube(4, 4, Color)
        FillCube(3, 1, Color)
        FillCube(3, 3, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Red
        FillCube(3, 0, Color)
        FillCube(3, 4, Color)
        FillCube(2, 1, Color)
        FillCube(2, 3, Color)
        FillCube(1, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Green
        FillCube(2, 0, Color)
        FillCube(2, 4, Color)
        FillCube(1, 1, Color)
        FillCube(1, 3, Color)
        FillCube(0, 2, Color)
        Await Task.Delay(Delay)
        color = Brushes.Blue
        FillCube(1, 0, Color)
        FillCube(1, 4, Color)
        FillCube(0, 1, Color)
        FillCube(0, 3, Color)
        Await Task.Delay(Delay)
        color = Brushes.Red
        FillCube(0, 0, Color)
        FillCube(0, 4, Color)
    End Sub
    Public Async Sub MatLeftArrowAnimation(Color As Brush, Delay As Integer)
        FillCube(2, 4, Color)
        Await Task.Delay(Delay)
        FillCube(1, 4, Color)
        FillCube(3, 4, Color)
        FillCube(2, 3, Color)
        Await Task.Delay(Delay)
        FillCube(0, 4, Color)
        FillCube(4, 4, Color)
        FillCube(1, 3, Color)
        FillCube(3, 3, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        FillCube(0, 3, Color)
        FillCube(4, 3, Color)
        FillCube(1, 2, Color)
        FillCube(3, 2, Color)
        FillCube(2, 1, Color)
        Await Task.Delay(Delay)
        FillCube(0, 2, Color)
        FillCube(4, 2, Color)
        FillCube(1, 1, Color)
        FillCube(3, 1, Color)
        FillCube(2, 0, Color)
        Await Task.Delay(Delay)
        FillCube(0, 1, Color)
        FillCube(4, 1, Color)
        FillCube(1, 0, Color)
        FillCube(3, 0, Color)
        Await Task.Delay(Delay)
        FillCube(0, 0, Color)
        FillCube(4, 0, Color)
    End Sub
    Public Async Sub MatRightArrowAnimation(Color As Brush, Delay As Integer)
        FillCube(2, 0, Color)
        Await Task.Delay(Delay)
        FillCube(1, 0, Color)
        FillCube(3, 0, Color)
        FillCube(2, 1, Color)
        Await Task.Delay(Delay)
        FillCube(0, 0, Color)
        FillCube(4, 0, Color)
        FillCube(1, 1, Color)
        FillCube(3, 1, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        FillCube(0, 1, Color)
        FillCube(4, 1, Color)
        FillCube(1, 2, Color)
        FillCube(3, 2, Color)
        FillCube(2, 3, Color)
        Await Task.Delay(Delay)
        FillCube(0, 2, Color)
        FillCube(4, 2, Color)
        FillCube(1, 3, Color)
        FillCube(3, 3, Color)
        FillCube(2, 4, Color)
        Await Task.Delay(Delay)
        FillCube(0, 3, Color)
        FillCube(4, 3, Color)
        FillCube(1, 4, Color)
        FillCube(3, 4, Color)
        Await Task.Delay(Delay)
        FillCube(0, 4, Color)
        FillCube(4, 4, Color)
    End Sub
    Public Async Sub MatLeftArrowRGBAnimation(Delay As Integer)
        Dim Color As Brush = Brushes.Red
        FillCube(2, 4, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Green
        FillCube(1, 4, Color)
        FillCube(3, 4, Color)
        FillCube(2, 3, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Blue
        FillCube(0, 4, Color)
        FillCube(4, 4, Color)
        FillCube(1, 3, Color)
        FillCube(3, 3, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Red
        FillCube(0, 3, Color)
        FillCube(4, 3, Color)
        FillCube(1, 2, Color)
        FillCube(3, 2, Color)
        FillCube(2, 1, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Green
        FillCube(0, 2, Color)
        FillCube(4, 2, Color)
        FillCube(1, 1, Color)
        FillCube(3, 1, Color)
        FillCube(2, 0, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Blue
        FillCube(0, 1, Color)
        FillCube(4, 1, Color)
        FillCube(1, 0, Color)
        FillCube(3, 0, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Red
        FillCube(0, 0, Color)
        FillCube(4, 0, Color)
    End Sub
    Public Async Sub MatRightArrowRGBAnimation(Delay As Integer)
        Dim Color As Brush = Brushes.Red
        FillCube(2, 0, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Green
        FillCube(1, 0, Color)
        FillCube(3, 0, Color)
        FillCube(2, 1, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Blue
        FillCube(0, 0, Color)
        FillCube(4, 0, Color)
        FillCube(1, 1, Color)
        FillCube(3, 1, Color)
        FillCube(2, 2, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Red
        FillCube(0, 1, Color)
        FillCube(4, 1, Color)
        FillCube(1, 2, Color)
        FillCube(3, 2, Color)
        FillCube(2, 3, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Green
        FillCube(0, 2, Color)
        FillCube(4, 2, Color)
        FillCube(1, 3, Color)
        FillCube(3, 3, Color)
        FillCube(2, 4, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Blue
        FillCube(0, 3, Color)
        FillCube(4, 3, Color)
        FillCube(1, 4, Color)
        FillCube(3, 4, Color)
        Await Task.Delay(Delay)
        Color = Brushes.Red
        FillCube(0, 4, Color)
        FillCube(4, 4, Color)
    End Sub
#End Region
    Dim LinkPlayer As Player = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
    Dim WithEvents AniTmr As New Forms.Timer With {.Interval = 100, .Enabled = True}
    Dim WithEvents OpAniTmr As New Forms.Timer With {.Interval = 2000, .Enabled = True}

    Private Sub AniTmr_Tick(sender As Object, e As EventArgs) Handles AniTmr.Tick
        Dim Peak = LinkPlayer.GetPeak
        LProgBar.SetSmoothValue(Peak.Left)
        LProgBar1.SetSmoothValue(Peak.Left)
        RProgBar.SetSmoothValue(Peak.Right)
        RProgBar1.SetSmoothValue(Peak.Right)
        Logo.BeginAnimation(MarginProperty, New Animation.ThicknessAnimation(New Thickness(100 - (Peak.Master * 2), 20, 100 - (Peak.Master * 2), 0), New Duration(TimeSpan.FromMilliseconds(100))))
    End Sub

    Private Sub About_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        AniTmr.Enabled = False
        e.Cancel = True
    End Sub

    Private Sub About_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        AniTmr.Enabled = True
    End Sub

    Private Sub About_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        MilkAndMochaGIF.BeginAnimation(MarginProperty, New Animation.ThicknessAnimation(New Thickness(0, 0, 0, 0), New Duration(TimeSpan.FromMilliseconds(150))))
    End Sub

    Private Async Sub OpAniTmr_Tick(sender As Object, e As EventArgs) Handles OpAniTmr.Tick
        SubText.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(100))))
        Await Task.Delay(200)
        SubText.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(100))))
    End Sub
End Class
