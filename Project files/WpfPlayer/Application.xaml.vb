Imports System.Windows.Threading

Class Application
    Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        If My.Settings.SUPRESSERRORS = False Then
            Select Case My.Settings.ONERRORSHOW
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
        My.Windows.Console.Log_Debug(e.Exception.ToString)
    End Sub
    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        If My.Settings.ALLOW_MULTIPLEINSTANCES = False Then
            If Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName).Count > 1 Then
                My.Windows.MainWindow.WhyAreWeHere = "JustToSuffer"
                If e.Args.Count > 0 Then
                    Dim manager = New NamedPipeManager("MuPlayPipe")
                    manager.Write(String.Join(">", e.Args), 1000)
                End If
                Process.GetCurrentProcess.Kill()
                'Application.Current.Shutdown(0)
                'Threading.Thread.Sleep(5000)
            Else
                Dim SP As New SplashScreen("Res/MuPlayLogo.png")
                SP.Show(True)
            End If
        Else
            Dim SP As New SplashScreen("Res/MuPlayLogo.png")
            SP.Show(True)
        End If
    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    'TODO    
    'Complete library manager:artists:saving :artists :year , complete years 
    'Pin current playing track To top of playlist

End Class
