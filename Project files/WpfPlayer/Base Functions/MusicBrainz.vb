Imports System.ComponentModel

Public Class MusicBrainz
    Public Property Song As String
    Public Property Artist As String
    Private _Status As States = States.Free
    Public Event OnStateChanged(State As States)
    Public Property IsBusy As Boolean = False
    Public Property Status As States
        Get
            Return _Status
        End Get
        Set(value As States)
            _Status = value
            If My.Settings.MUSICBRAINZNOTIFY Then
                RaiseEvent OnStateChanged(value)
            End If
        End Set
    End Property
    Private sSong As String
    Private sSongLength As Double
    Private sArtist As String
    Private sAlbum As String
    Private sDate As String
    Private sScore As String

    Public Sub New()
        Status = States.Free
    End Sub
    <System.Runtime.InteropServices.DllImport("wininet.dll")>
    Private Shared Function InternetGetConnectedState(ByRef Description As Integer, ByVal ReservedValue As Integer) As Boolean
    End Function
    Public Async Function Search() As Task(Of MusicItem)
        Try
            IsBusy = True
            Status = States.Searching
            Dim ConnDesc As Integer
            If Not InternetGetConnectedState(ConnDesc, 0) Then Status = States.NoNet : Return Nothing
            Return Await Task.Run(Async Function()
                                      Dim rec = Global.MusicBrainz.Search.Recording(query:=Song, artist:=Artist)
                                      Try
                                          For Each wSong In rec.Data
                                              For Each album In wSong.Releaselist
                                                  If album.Id = String.Empty Then Continue For
                                                  If wSong.Score = 100 Then
                                                      For Each wArtist In wSong.Artistcredit
                                                          If wArtist.Artist.Id <> String.Empty Then
                                                              sSong = wSong.Title
                                                              sSongLength = wSong.Length
                                                              sArtist = wArtist.Artist.Name
                                                              sAlbum = album.Title
                                                              sDate = album.Date
                                                              sScore = wSong.Score
                                                              IsBusy = False
                                                              Return Await Task.FromResult(New MusicItem(sSong, sArtist, sAlbum, sDate, sScore))
                                                          End If
                                                      Next
                                                  End If
                                              Next
                                          Next
                                          Return Nothing
                                      Catch ex As Exception
                                          Return Nothing
                                      End Try
                                  End Function)
            Status = States.Free
            IsBusy = False
            Return Nothing
        Catch ex As Exception
            IsBusy = False
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' 0_Song , 1_Length , 2_Artist , 3_Album , 4_Date , 5_Score
    ''' </summary>
    ''' <returns></returns>
    Public Function GetRawResult() As String()
        Try
            Dim RLength As String = Utils.SecsToMins(sSongLength / 1000)
            Dim Result As String() = {sSong, RLength, sArtist, sAlbum, sDate, sScore}
            Return Result
        Catch ex As Exception
            Return New String() {"No information found"}
        End Try
    End Function
    Public Enum States
        Free = 0
        Searching = 1
        Canceling = 2
        Canceled = 3
        Done = 5
        NoNet = 6
    End Enum
    Public Class MusicItem
        Public Property Title As String
        Public Property Artist As String
        Public Property Album As String
        Public Property Year As String
        Public Property Score As Integer
        Public Sub New(_Title As String, _Artist As String, _Album As String, _Year As String, _Score As Integer)
            Title = _Title
            Artist = _Artist
            Album = _Album
            Year = _Year
            Score = _Score
        End Sub
        Public Function GetFormattedResult() As String()
            Try
                Dim Result As String() = {"Title: " & Title, "Aritst: " & Artist, "Album title: " & Album, "Release date: " & Year, "Search score: " & Score}
                Return Result
            Catch ex As Exception
                Return New String() {"No information found"}
            End Try
        End Function
    End Class
End Class
