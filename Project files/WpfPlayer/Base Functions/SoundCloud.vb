Imports System.ComponentModel
Imports System.Drawing
Imports System.Net
Imports System.Text.RegularExpressions
Public Class SoundCloud
    Public Event OnStateChanged(State As State)
    Dim client_id As String = "client_id=a549b13e0494aaec74fed484d567153b"
    Private Property _status As State = State.Free
    Public Property Status As State
        Get
            Return _status
        End Get
        Set(value As State)
            _status = value
            RaiseEvent OnStateChanged(value)
        End Set
    End Property
    Public Class SoundCloudMusicItem
        Public Property Title As String
        Public Property Artist As String
        Public Property Avatar As Bitmap
        Public Property UploadDate As String
        Public Property URI As Uri
        Public Property Size As String
        Public Sub New(_Title As String, _Artist As String, _Avatar As Bitmap, _Date As String, _Uri As Uri)
            Title = _Title
            Artist = _Artist
            Avatar = _Avatar
            UploadDate = _Date
            URI = _Uri
        End Sub
        Public Overrides Function ToString() As String
            Return "Artist/" & Artist & "/Title/" & Title & "/Date/" & UploadDate & "/Size/" & Size
        End Function
    End Class
    Public Enum State
        Free = 0
        Connected = 1
        FatalError = 2
    End Enum
    Public Sub New()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
    End Sub
    Public Function GetInfo(URL As String) As SoundCloudMusicItem
        Try
            Using I As New WebClient
                Dim result As String = I.DownloadString(New Uri("https://api.soundcloud.com/resolve.json?url=" & URL & "&" & client_id))
                Status = State.Connected
                Dim Title As String = Regex.Match(result, ",""title"":""(.*?)"",").Groups.Item(1).Value
                Dim UploadDate As String = Regex.Match(result, ",""created_at"":""(.*?)"",").Groups.Item(1).Value
                Dim artist As String = Regex.Match(result, ",""username"":""(.*?)"",").Groups.Item(1).Value
                Dim SongSize = F(Regex.Match(result, ",""original_content_size"":(.*?),").Groups.Item(1).Value)
                Dim URI As Uri = New Uri("https://api.soundcloud.com/tracks/" & Regex.Match(result, ",""id"":(.*?),").Groups.Item(1).Value & "/stream?" & client_id)
                ' Create a 'WebRequest' object with the specified url. 
                Dim myWebRequest As WebRequest = WebRequest.Create(URI)
                ' Send the 'WebRequest' and wait for response.
                Dim myWebResponse As WebResponse = myWebRequest.GetResponse()
                ' "ResponseUri" property is used to get the actual Uri from where the response was attained.
                Status = State.Free
                URI = myWebResponse.ResponseUri
                Dim avatar = Bitmap.FromStream(New WebClient() With {.Proxy = Nothing}.OpenRead(Regex.Match(result, ",""avatar_url"":""(.*?)""}").Groups.Item(1).Value)).Clone
                Return New SoundCloudMusicItem(Title, artist, avatar, UploadDate, URI) With {.Size = SongSize}
            End Using
        Catch ex As Exception
            Status = State.FatalError
            Return Nothing
        End Try
    End Function
    Public Async Function GetInfoAsync(URL As String) As Task(Of SoundCloudMusicItem)
        Try
            Return Await Task.FromResult(Await Task.Run(Function()
                                                            Try
                                                                Using I As New WebClient
                                                                    Dim result As String = I.DownloadString(New Uri("https://api.soundcloud.com/resolve.json?url=" & URL & "&" & client_id))
                                                                    Status = State.Connected
                                                                    Dim Title As String = Regex.Match(result, ",""title"":""(.*?)"",").Groups.Item(1).Value
                                                                    Dim UploadDate As String = Regex.Match(result, ",""created_at"":""(.*?)"",").Groups.Item(1).Value
                                                                    Dim artist As String = Regex.Match(result, ",""username"":""(.*?)"",").Groups.Item(1).Value
                                                                    Dim SongSize = F(Regex.Match(result, ",""original_content_size"":(.*?),").Groups.Item(1).Value)
                                                                    Dim URI As Uri = New Uri("https://api.soundcloud.com/tracks/" & Regex.Match(result, ",""id"":(.*?),").Groups.Item(1).Value & "/stream?" & client_id)
                                                                    ' Create a 'WebRequest' object with the specified url. 
                                                                    Dim myWebRequest As WebRequest = WebRequest.Create(URI)
                                                                    ' Send the 'WebRequest' and wait for response.
                                                                    Dim myWebResponse As WebResponse = myWebRequest.GetResponse()
                                                                    ' "ResponseUri" property is used to get the actual Uri from where the response was attained.
                                                                    Status = State.Free
                                                                    URI = myWebResponse.ResponseUri
                                                                    Dim avatar = Bitmap.FromStream(New WebClient() With {.Proxy = Nothing}.OpenRead(Regex.Match(result, ",""avatar_url"":""(.*?)""}").Groups.Item(1).Value)).Clone
                                                                    Return New SoundCloudMusicItem(Title, artist, avatar, UploadDate, URI) With {.Size = SongSize}
                                                                End Using
                                                            Catch ex As Exception
                                                                Return Nothing
                                                            End Try
                                                        End Function))
        Catch ex As Exception
            Status = State.FatalError
            Return Nothing
        End Try
    End Function
    Private Function F(ByVal type As Double) As String
        Dim types As String() = {"B", "KB", "MB", "GB"}
        Dim typees As Double = type
        Dim CSA As Integer = 0
        While typees >= 1024 AndAlso CSA + 1 < types.Length
            CSA += 1
            typees = typees / 1024
        End While
        Return [String].Format("{0:0.##} {1}", typees, types(CSA))
    End Function
    Private Function ModifyName(ByVal URL As String) As String
        Return URL.Replace("\", Nothing).Replace("/", Nothing).Replace(":", Nothing).Replace("*", Nothing).Replace("?", Nothing).Replace("""", Nothing).Replace("<", Nothing).Replace(">", Nothing).Replace("|", Nothing)
    End Function
End Class
