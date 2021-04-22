Imports System.ComponentModel

Public Class GlobalHotkeysManager
    Dim Hwnd As Integer
    Private Sub GlobalHotkeysManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim Helper As New Interop.WindowInteropHelper(Application.Current.MainWindow)
        Hwnd = Helper.Handle
        Helper = Nothing
        set_ghkeycb.Items.Clear()
        Dim KeyID As Int32 = 0
        For i As Integer = 0 To System.Enum.GetNames(GetType(Forms.Keys)).Count
            KeyID = i
            set_ghkeycb.Items.Add(System.Enum.ToObject(GetType(Forms.Keys), KeyID).ToString & "(" & KeyID & ")")
        Next
    End Sub

    Private Sub GlobalHotkeysManager_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Hide()
        e.Cancel = True
    End Sub
    Private Sub set_globalhotkey_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_globalhotkey.SelectionChanged
        Select Case set_globalhotkey.SelectedIndex
            Case 0 'Play Pause
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_PlayPause_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_PlayPause
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(0)
            Case 1 'Next
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_Next_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_Next
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(1)
            Case 2 'Previous
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_Previous_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_Previous
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(2)
            Case 3 '+10
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_Skip10_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_Skip10
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(3)
            Case 4 '-10
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_Back10_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_Back10
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(4)
            Case 5 'V +
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_VolumeUp_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_VolumeUp
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(5)
            Case 6 'V -
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_VolumeDown_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_VolumeDown
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(6)
            Case 7 'V *
                set_ghmodcb.SelectedIndex = My.Settings.GlobalHotkey_VolumeMute_MOD
                set_ghkeycb.SelectedIndex = My.Settings.GlobalHotkey_VolumeMute
                set_ghstate.Text = TryCast(Application.Current.MainWindow, MainWindow).HotkeyState(7)
        End Select
    End Sub

    Private Sub set_ghmodcb_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_ghmodcb.SelectionChanged

    End Sub

    Private Sub set_ghkeycb_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles set_ghkeycb.SelectionChanged

    End Sub

    Private Sub set_apply_Click(sender As Object, e As RoutedEventArgs) Handles set_apply.Click
        Select Case set_globalhotkey.SelectedIndex
            Case 0 'Play Pause
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_PlayPause_MOD), My.Settings.GlobalHotkey_PlayPause, Hwnd, 0)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_PlayPause_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_PlayPause = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 0)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Play Pause hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 1 'Next
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Next_MOD), My.Settings.GlobalHotkey_Next, Hwnd, 1)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_Next_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_Next = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 1)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Next hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 2 'Previous
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Previous_MOD), My.Settings.GlobalHotkey_Previous, Hwnd, 2)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_Previous_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_Previous = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 2)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Previous hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 3 '+10                
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Skip10_MOD), My.Settings.GlobalHotkey_Skip10, Hwnd, 3)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_Skip10_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_Skip10 = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 3)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Skip 10 seconds hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 4 '-10
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Back10_MOD), My.Settings.GlobalHotkey_Back10, Hwnd, 4)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_Back10_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_Back10 = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 4)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Skip 10 seconds hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 5 'V+
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Back10_MOD), My.Settings.GlobalHotkey_Back10, Hwnd, 4)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_VolumeUp_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_VolumeUp = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 5)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Volume up seconds hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 6 'V-
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Back10_MOD), My.Settings.GlobalHotkey_Back10, Hwnd, 4)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_VolumeDown_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_VolumeDown = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 6)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Volume down hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
            Case 7 'V *
                Dim GlobalHotkey As New GlobalHotkey(Utils.IntToMod(index:=My.Settings.GlobalHotkey_Back10_MOD), My.Settings.GlobalHotkey_Back10, Hwnd, 4)
                GlobalHotkey.Unregister()
                My.Settings.GlobalHotkey_VolumeMute_MOD = set_ghmodcb.SelectedIndex
                My.Settings.GlobalHotkey_VolumeMute = set_ghkeycb.SelectedIndex
                My.Settings.Save()
                GlobalHotkey = New GlobalHotkey(Utils.IntToMod(index:=set_ghmodcb.SelectedIndex), set_ghkeycb.SelectedIndex, Hwnd, 7)
                If Not GlobalHotkey.Register() Then
                    TryCast(Application.Current.MainWindow, MainWindow).ShowNotification("MuPlay", "Volume mute hook couldn't be registered.", HandyControl.Data.NotifyIconInfoType.Error)
                End If
        End Select
    End Sub

End Class
