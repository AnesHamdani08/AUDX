Public Class Updator
    Public ReadOnly Property Version As Version = My.Application.Info.Version
    Private UpdatesServer As String = Nothing
    Private _LastVersion As Version = Nothing
    Private _UpdatesLink As String = Nothing
    Private _ChangeLog As String = Nothing
    Public ReadOnly Property LatestVersion As Version
        Get
            Return _LastVersion
        End Get
    End Property
    Public ReadOnly Property UpdatesLink As String
        Get
            Return _UpdatesLink
        End Get
    End Property
    Public ReadOnly Property ChangeLog As String
        Get
            Return _ChangeLog
        End Get
    End Property
    Public Sub New(Server As String)
        If Not String.IsNullOrEmpty(Server) AndAlso Not String.IsNullOrWhiteSpace(Server) Then
            UpdatesServer = Server
            chkupd()
        End If
    End Sub
    Public Async Function CheckForUpdates() As Task(Of String)
        Return Await Task.FromResult(Await Task.Run(Async Function() As Task(Of String)
                                                        Try
                                                            Using WC As New Net.WebClient
                                                                Dim UpdatesData = Await WC.DownloadStringTaskAsync(UpdatesServer)
                                                                Dim UpdatesDoc As New Xml.XmlDocument
                                                                UpdatesDoc.LoadXml(UpdatesData)
                                                                Dim LastVersion As String = UpdatesDoc.SelectSingleNode("/MuPlay/LastVersion").InnerText
                                                                Dim UpdateLink As String = UpdatesDoc.SelectSingleNode("/MuPlay/UpdateLink").InnerText
                                                                Dim ChangeLog As String = UpdatesDoc.SelectSingleNode("/MuPlay/ChangeLog").InnerText
                                                                _LastVersion = New Version(LastVersion)
                                                                _UpdatesLink = UpdateLink
                                                                _ChangeLog = ChangeLog.Replace("&", Environment.NewLine)
                                                                Return Await Task.FromResult(LastVersion)
                                                            End Using
                                                        Catch ex As Exception
                                                            Return Nothing
                                                        End Try
                                                    End Function))
    End Function
    Private Sub chkupd()
        Using WC As New Net.WebClient
            Dim UpdatesData = WC.DownloadString(New Uri(UpdatesServer))
            Dim UpdatesDoc As New Xml.XmlDocument
            UpdatesDoc.LoadXml(UpdatesData)
            Dim LastVersion As String = UpdatesDoc.SelectSingleNode("/MuPlay/LastVersion").InnerText
            Dim UpdateLink As String = UpdatesDoc.SelectSingleNode("/MuPlay/UpdateLink").InnerText
            _LastVersion = New Version(LastVersion)
            _UpdatesLink = UpdateLink
        End Using
    End Sub
End Class
