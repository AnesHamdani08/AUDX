Public Class SleepTimer
    Private WithEvents TMR As New Forms.Timer With {.Interval = 500}
    Private SW As New Stopwatch
    Private Property CountTo As TimeSpan
    Private OnFinishDo As Action = Nothing
    Public Event OnFinish()
    Public Property IsCounting As Boolean = False
    Public Sub New()
    End Sub
    Public Sub Count(_To As TimeSpan, Optional FinishAction As Action = Nothing)
        If _To <> TimeSpan.Zero Then
            CountTo = _To
            If FinishAction IsNot Nothing Then
                OnFinishDo = FinishAction
            End If
            IsCounting = True
            SW.Start()
            TMR.Start()
        End If
    End Sub
    Private Sub TMR_Tick(sender As Object, e As EventArgs) Handles TMR.Tick
        If IsCounting = False Then
            TMR.Stop()
        Else
            If CountTo.Seconds = SW.Elapsed.Seconds Then
                RaiseEvent OnFinish()
                If OnFinishDo IsNot Nothing Then
                    OnFinishDo.Invoke
                End If
                IsCounting = False
                TMR.Stop()
            End If
        End If
    End Sub
End Class
