Imports System.IO
Imports System.IO.Pipes
Imports System.Threading

Public Class NamedPipeManager
    Public NamedPipeName As String = "MuPlayAPI"
    Public Event ReceiveString As Action(Of String)
    Private Const EXIT_STRING As String = "__EXIT__"
    Private _isRunning As Boolean = False
    Private Thread As Thread

    Public Sub New(ByVal name As String)
        NamedPipeName = name
    End Sub

    Public Sub StartServer()
        Thread = New Thread(Sub(pipeName)
                                _isRunning = True

                                While True
                                    Dim text As String

                                    Using server = New NamedPipeServerStream(TryCast(pipeName, String))
                                        server.WaitForConnection()

                                        Using reader As StreamReader = New StreamReader(server)
                                            text = reader.ReadToEnd()
                                        End Using
                                    End Using

                                    If text = EXIT_STRING Then Exit While
                                    OnReceiveString(text)
                                    If _isRunning = False Then Exit While
                                End While
                            End Sub)
        Thread.Start(NamedPipeName)
    End Sub

    Protected Overridable Sub OnReceiveString(ByVal text As String)
        RaiseEvent ReceiveString(text)
    End Sub

    Public Sub StopServer()
        _isRunning = False
        Write(EXIT_STRING)
        Thread.Sleep(30)
    End Sub

    Public Function Write(ByVal text As String, ByVal Optional connectTimeout As Integer = 300) As Boolean
        Using client = New NamedPipeClientStream(NamedPipeName)

            Try
                client.Connect(connectTimeout)
            Catch
                Return False
            End Try

            If Not client.IsConnected Then Return False

            Using writer As StreamWriter = New StreamWriter(client)
                writer.Write(text)
                writer.Flush()
            End Using
        End Using

        Return True
    End Function
End Class
