Imports System.ComponentModel

Public Class Reverb
    Private Sub Reverb_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub ExitBtn_Click(sender As Object, e As RoutedEventArgs) Handles ExitBtn.Click
        Me.Close()
    End Sub

    Private Sub ingain_val_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles ingain_val.ValueChanged
        Try
            CType(Owner, MainWindow).MainPlayer.UpdateReverb(ingain_val.Value, 0, 0, 0, True, False, False, False)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub reverbmix_val_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles reverbmix_val.ValueChanged
        Try
            CType(Owner, MainWindow).MainPlayer.UpdateReverb(0, reverbmix_val.Value, 0, 0, False, True, False, False)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub reverbtime_val_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles reverbtime_val.ValueChanged
        Try
            CType(Owner, MainWindow).MainPlayer.UpdateReverb(0, 0, reverbtime_val.Value / 1000, 0, False, False, True, False)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub highfreqrtratio_val_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles highfreqrtratio_val.ValueChanged
        Try
            CType(Owner, MainWindow).MainPlayer.UpdateReverb(0, 0, 0, highfreqrtratio_val.Value / 1000, False, False, False, True)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub fx_state_Checked(sender As Object, e As RoutedEventArgs) Handles fx_state.Checked
        Try
            CType(Owner, MainWindow).MainPlayer.UpdateReverb(0, 0, 0, 0, False, False, False, False, False, True)
        Catch ex As Exception
            fx_state.IsChecked = False
        End Try
    End Sub

    Private Sub fx_state_Unchecked(sender As Object, e As RoutedEventArgs) Handles fx_state.Unchecked
        Try
            CType(Owner, MainWindow).MainPlayer.UpdateReverb(0, 0, 0, 0, False, False, False, False, True)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Update()
        With CType(Owner, MainWindow).MainPlayer.Reverb
            fx_state.IsChecked = CType(Owner, MainWindow).MainPlayer.IsReverb
            If fx_state.IsChecked Then
                ingain_val.Value = .fInGain
                reverbmix_val.Value = .fReverbMix
                reverbtime_val.Value = .fReverbTime * 1000
                highfreqrtratio_val.Value = .fHighFreqRTRatio * 1000
            End If
        End With
    End Sub
End Class
