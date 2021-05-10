Public Class SmoothProgressBar
    Inherits ProgressBar
    Public Property Smoothness As Double = 100
    Public Sub SetSmoothValue(val As Double)
        BeginAnimation(ValueProperty, New Animation.DoubleAnimation(val, New Duration(TimeSpan.FromMilliseconds(Smoothness))))
    End Sub
End Class
