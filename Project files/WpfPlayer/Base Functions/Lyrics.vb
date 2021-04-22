Imports System.ComponentModel
Imports System.Runtime.InteropServices

Public Class Lyrics
    Public WithEvents WebBrowser1 As Forms.WebBrowser = Nothing
    Dim AsyncWebBrowser1 As New AsyncWebBrowser
    Public Property _Lyrics = ""
    Private _Status As States = States.Free
    Public Property IsBusy As Boolean = False
    Public Property Status As States
        Get
            Return _Status
        End Get
        Set(value As States)
            _Status = value
            If My.Settings.LyricsNotify Then
                RaiseEvent OnStateChanged(value)
            End If
        End Set
    End Property
    Public Property Song = ""
    Public Event OnStateChanged(State As States)
    Public Sub Dispose()
        WebBrowser1 = Nothing
        AsyncWebBrowser1 = Nothing
    End Sub
    Public Sub New()
        WebBrowser1 = New Forms.WebBrowser With {.ScriptErrorsSuppressed = True}
        AsyncWebBrowser1.SetBrowser(WebBrowser1)
    End Sub
    <System.Runtime.InteropServices.DllImport("wininet.dll")>
    Private Shared Function InternetGetConnectedState(ByRef Description As Integer, ByVal ReservedValue As Integer) As Boolean
    End Function
    Public Async Function Beginsearch(artist As String, title As String, rawtitle As String) As Task(Of String)
        IsBusy = True
        Status = States.Initializing
        _Lyrics = String.Empty
        Dim ConnDesc As Integer
        If Not InternetGetConnectedState(ConnDesc, 0) Then Status = States.NoNet : Return Nothing
        Song = rawtitle
        Await AsyncWebBrowser1.NavigateAsync("https://www.azlyrics.com/lyrics/" & artist.ToLower & "/" & title.ToLower & ".html")
        Status = States.Searching
        Dim divs = WebBrowser1.Document.Body.GetElementsByTagName("div")
        Dim A As List(Of String) = New List(Of String)
        For Each d As Forms.HtmlElement In divs
            If d.GetAttribute("classname") = "" Then
                If d.InnerText <> "" Then
                    A.Add(d.InnerText)
                End If
                For Each line In A
                    _Lyrics = line
                Next
                If A.Count = 0 Then
                    Continue For
                Else
                    Status = States.LyricsFound
                    IsBusy = False
                    Return Await Task.FromResult(_Lyrics)
                End If
            End If
        Next
        IsBusy = False
        Return Await Task.FromResult(String.Empty)
    End Function
    Public Sub Cancel()
        WebBrowser1.Stop()
    End Sub
    Public Enum States
        Free = 1
        Initializing = 2
        Searching = 3
        Done = 4
        NoLyrics = 5
        LyricsFound = 6
        NoNet = 7
    End Enum
End Class
