Imports System.Windows.Threading

Class Application
    Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        If My.Settings.SuppressErrors = False Then
            Select Case My.Settings.OnErrorShow
                Case 0 'msg
                    Dim Err = New ErrorDialog(e.Exception.Message)
                    Err.ShowDialog()
                    e.Handled = True
                Case 1 'inner msg       
                    If e.Exception.InnerException IsNot Nothing Then
                        Dim Err = New ErrorDialog(e.Exception.InnerException.Message)
                        Err.ShowDialog()
                    Else
                        Dim Err = New ErrorDialog("NO_INNER_MESSAGE")
                        Err.ShowDialog()
                    End If
                    e.Handled = True
                Case 2 'stack trace
                    If e.Exception.InnerException IsNot Nothing Then
                        Dim Err = New ErrorDialog(e.Exception.StackTrace & vbCrLf & "----------------------INNER EXCEPTION-----------------" & vbCrLf & e.Exception.InnerException.StackTrace)
                        Err.ShowDialog()
                    Else
                        Dim Err = New ErrorDialog(e.Exception.StackTrace & vbCrLf & "----------------------NO_INNER_EXCEPTION-----------------")
                        Err.ShowDialog()
                    End If
                    e.Handled = True
                Case 3 'auto
                    If e.Exception.InnerException IsNot Nothing Then
                        Dim Err = New ErrorDialog(e.Exception.ToString & vbCrLf & "-----------------------INNER EXCEPTION-------------------" & vbCrLf & e.Exception.InnerException.ToString)
                        Err.ShowDialog()
                    Else
                        Dim Err = New ErrorDialog(e.Exception.ToString & vbCrLf & "-----------------------NO_INNER_EXCEPTION-------------------")
                        Err.ShowDialog()
                    End If
                    e.Handled = True
            End Select
        Else
            e.Handled = True
        End If
        My.Windows.Console.Log(e.Exception.ToString)
    End Sub
    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup 'complete mat
        If Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName).Count > 1 Then
            If e.Args.Count > 0 Then
                '    For Each prcs In Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)
                '        If prcs.Id <> Process.GetCurrentProcess.Id Then
                '            Dim BS As New BuildString
                '            If e.Args(0) = "-api" Then
                '                BS.PostString(prcs.MainWindowHandle, &H500, 0, e.Args(0) & ">>" & e.Args(1) & ">>" & e.Args(2) & ">>" & e.Args(3))
                '            Else
                '                BS.PostString(prcs.MainWindowHandle, &H400, 0, String.Join(">>", e.Args))
                '            End If
                '            Exit For
                '        End If
                '    Next
                Dim manager = New NamedPipeManager("MuPlayPipe")
                manager.Write(String.Join(">", e.Args))
            End If
            Application.Current.Shutdown(0)
        Else
            Dim SP As New SplashScreen("Res/MuPlayLogo.png")
            SP.Show(True)
        End If
    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

End Class
