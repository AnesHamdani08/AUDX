Imports System.ComponentModel

Public Class Console
    Private _LogDebug As Boolean = False
    Private _Log As String
    Public Property LogDebug As Boolean
        Get
            Return _LogDebug
        End Get
        Set(value As Boolean)
            _LogDebug = value
            If value Then
                TB_Log.Foreground = Brushes.Red
                Log_Debug("Previous Stack Trace")
                Log_Debug(_Log)
            End If
        End Set
    End Property
    Dim AutoCommandSource As New ObjectModel.ObservableCollection(Of String)
    Private Sub Console_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        CB_AutoCommand.ItemsSource = AutoCommandSource
    End Sub
    Private Sub Console_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub
    Public Async Sub Log(Message As String)
        Await Dispatcher.BeginInvoke(Sub()
                                         TB_Log.Text += Now.ToString("[HH:mm:ss]") & Message & Environment.NewLine
                                     End Sub)
    End Sub
    Public Sub Log_Debug(Message As String)
        If LogDebug Then
            TB_Log.Text += Now.ToString("[HH:mm:ss][DEBUG]") & Message & Environment.NewLine
        Else
            _Log += Now.ToString("[HH:mm:ss][DEBUG]") & Message & Environment.NewLine
        End If
    End Sub
    Private Sub TB_Command_KeyUp(sender As Object, e As KeyEventArgs) Handles TB_Command.KeyUp
        If e.Key = Key.Enter Then
            Command.Excute(TB_Command.Text, Application.Current.MainWindow)
            Log("Excuted command: " & TB_Command.Text)
            TB_Command.Text = String.Empty
        End If
    End Sub
    Public Sub FindCommand(Message As String)
        AutoCommandSource.Clear()
        For Each cmd In Command.Commands
            If cmd.ToLower.Contains(Message.ToLower) Then AutoCommandSource.Add(cmd)
        Next
    End Sub
    Private Sub TB_Command_TextChanged(sender As Object, e As TextChangedEventArgs) Handles TB_Command.TextChanged
        FindCommand(TB_Command.Text)
    End Sub

    Private Sub CB_AutoCommand_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles CB_AutoCommand.SelectionChanged
        If CB_AutoCommand.SelectedIndex <> -1 Then
            Command.Excute(AutoCommandSource(CB_AutoCommand.SelectedIndex), Application.Current.MainWindow)
            Log("Excuted command: " & AutoCommandSource(CB_AutoCommand.SelectedIndex))
            TB_Command.Text = String.Empty
        End If
    End Sub

End Class
