Public Class Playlist
#Region "Properties"
    Private _Playlist As List(Of String)
    Private Property _Index As Integer = 0
    Private Property Idx As Integer
        Get
            Return _Index
        End Get
        Set(value As Integer)
            RaiseEvent OnIndexChanged(value)
            _Index = value
        End Set
    End Property
    Public ReadOnly Property Index As Integer
        Get
            Return Idx
        End Get
    End Property
    Public ReadOnly Property Count As Integer
        Get
            Return _Playlist.Count
        End Get
    End Property
    Public ReadOnly Property Playlist As List(Of String)
        Get
            Return _Playlist
        End Get
    End Property
    Public ReadOnly Property Current As String
        Get
            Return GetItem(Index)
        End Get
    End Property
    Public ReadOnly Property NextItem As String
        Get
            Try
                Dim Pitem = _Playlist.Item(Idx + 1)
                Return Pitem.Substring(0, Pitem.IndexOf(">>"))
            Catch ex As Exception
                Dim Pitem = _Playlist.Item(0)
                Return Pitem.Substring(0, Pitem.IndexOf(">>"))
            End Try
        End Get
    End Property
    Public ReadOnly Property PreviousItem As String
        Get
            Try
                Dim Pitem = _Playlist.Item(Idx - 1)
                Return Pitem.Substring(0, Pitem.IndexOf(">>"))
            Catch ex As Exception
                Dim Pitem = _Playlist.Item(Count - 1)
                Return Pitem.Substring(0, Pitem.IndexOf(">>"))
            End Try
        End Get
    End Property
#End Region
#Region "Events"
    Public Event OnSongAdd(Value As String, Type As Player.StreamTypes, IndexUpdated As Boolean, UseURL As Boolean, URL As String, OverrideCurrentMedia As Boolean, OCMTitle As String, OCMArtist As String, OCMYear As Integer, OCMCover As System.Drawing.Bitmap)
    Public Event OnSongRemove(Value As String, Index As Integer)
    Public Event OnPlaylistClear()
    Public Event OnSongNext()
    Public Event OnSongPrevious()
    Public Event OnIndexChanged(Index As Integer)
    Public Event OnItemChanged(IndexFrom As Integer, IndexTo As Integer)
    Public Event OnSongInsert(Value As String, Type As Player.StreamTypes, IndexUpdated As Boolean, UseURL As Boolean, URL As String, OverrideCurrentMedia As Boolean, OCMTitle As String, OCMArtist As String, OCMYear As Integer, OCMCover As System.Drawing.Bitmap, Index As Integer)
#End Region
    Public Sub New()
        _Playlist = New List(Of String)
    End Sub
    Public Sub Add(Loc As String, Type As Player.StreamTypes, Optional UpdateIndex As Boolean = True, Optional UseURL As Boolean = False, Optional URL As String = Nothing, Optional OverrideCurrentMedia As Boolean = False, Optional OCMTitle As String = Nothing, Optional OCMArtist As String = Nothing, Optional OCMYear As Integer = 0, Optional OCMCover As System.Drawing.Bitmap = Nothing)
        Try
            If Loc IsNot Nothing Then
                _Playlist.Add(Loc & ">>" & Type)
                If UpdateIndex = True Then
                    RaiseEvent OnSongAdd(Loc, Type, True, UseURL, URL, OverrideCurrentMedia, OCMTitle, OCMArtist, OCMYear, OCMCover)
                    Idx = Count - 1
                    RaiseEvent OnIndexChanged(Idx)
                Else
                    RaiseEvent OnSongAdd(Loc, Type, False, UseURL, URL, OverrideCurrentMedia, OCMTitle, OCMArtist, OCMYear, OCMCover)
                End If
            ElseIf UseURL = True Then
                _Playlist.Add(URL & ">>" & Type & ">>" & OCMTitle & ">>" & OCMArtist & ">>" & OCMYear)
                If UpdateIndex = True Then
                    RaiseEvent OnSongAdd(Loc, Type, True, UseURL, URL, OverrideCurrentMedia, OCMTitle, OCMArtist, OCMYear, OCMCover)
                    Idx = Count - 1
                    RaiseEvent OnIndexChanged(Idx)
                Else
                    RaiseEvent OnSongAdd(Loc, Type, False, UseURL, URL, OverrideCurrentMedia, OCMTitle, OCMArtist, OCMYear, OCMCover)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Insert(Index As Integer, Loc As String, Type As Player.StreamTypes, Optional UpdateIndex As Boolean = True, Optional UseURL As Boolean = False, Optional URL As String = Nothing, Optional OverrideCurrentMedia As Boolean = False, Optional OCMTitle As String = Nothing, Optional OCMArtist As String = Nothing, Optional OCMYear As Integer = 0, Optional OCMCover As System.Drawing.Bitmap = Nothing)
        Try
            If Loc IsNot Nothing Then
                _Playlist.Insert(Index, Loc & ">>" & Type)
                RaiseEvent OnSongInsert(Loc, Type, False, UseURL, URL, OverrideCurrentMedia, OCMTitle, OCMArtist, OCMYear, OCMCover, Index)
            ElseIf UseURL = True Then
                _Playlist.Insert(Index, URL & ">>" & Type & ">>" & OCMTitle & ">>" & OCMArtist & ">>" & OCMYear)
                RaiseEvent OnSongInsert(Loc, Type, False, UseURL, URL, OverrideCurrentMedia, OCMTitle, OCMArtist, OCMYear, OCMCover, Index)
            End If
            If UpdateIndex = True Then
                If Index < Idx Then
                    Idx += 1
                    RaiseEvent OnIndexChanged(Idx)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Remove(Str As String)
        Try
            _Playlist.Remove(Str)
            RaiseEvent OnSongRemove(Str, Nothing)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub RemoveAt(Index As Integer)
        Try
            _Playlist.RemoveAt(Index)
            RaiseEvent OnSongRemove(Nothing, Index)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Clear()
        Try
            _Playlist.Clear()
            RaiseEvent OnPlaylistClear()
        Catch ex As Exception
        End Try
    End Sub
    Public Function NextSong() As String
        Try
            Idx += 1
            Dim Pitem = _Playlist.Item(Idx)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        Catch ex As Exception
            Idx = 0
            Dim Pitem = _Playlist.Item(Idx)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        End Try
    End Function
    Public Function GetNextSong() As String
        Try
            Dim Pitem = _Playlist.Item(Idx + 1)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        Catch ex As Exception
            Dim Pitem = _Playlist.Item(0)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        End Try
    End Function
    Public Function PreviousSong() As String
        Try
            Idx -= 1
            Dim Pitem = _Playlist.Item(Idx)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        Catch ex As Exception
            Idx = _Playlist.Count - 1
            Dim Pitem = _Playlist.Item(Idx)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        End Try
    End Function
    Public Function GetNextSongIndex() As Integer
        Try
            If Index + 1 = Count Then
                Return 0
            Else
                Return Index + 1
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Public Function GetPreviousSong() As String
        Try
            Dim Pitem = _Playlist.Item(Idx - 1)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        Catch ex As Exception
            Dim Pitem = _Playlist.Item(Count - 1)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        End Try
    End Function
    Public Function GetPreviousSongIndex() As Integer
        Try
            If Index - 1 < 0 Then
                Return Count - 1
            Else
                Return Index - 1
            End If
        Catch ex As Exception
            Return Count - 1
        End Try
    End Function
    Public Function GetItem(Index As Integer) As String
        Try
            Dim Pitem = _Playlist.Item(Index)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function JumpTo(Index As Integer) As String
        Try
            Idx = Index
            Dim Pitem = _Playlist.Item(Index)
            Return Pitem.Substring(0, Pitem.IndexOf(">>"))
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Sub SetIndex(Index As Integer)
        Try
            Idx = Index
        Catch ex As Exception
            Return
        End Try
    End Sub
    Public Function MoveTo(ByVal IndexFrom As Integer, ByVal IndexTo As Integer) As Integer
        Try
            If _Playlist.Item(IndexFrom) = Current Then
                If IndexTo >= Count Then
                    Dim Item = _Playlist.Item(IndexFrom)
                    _Playlist.RemoveAt(IndexFrom)
                    _Playlist.Insert(0, Item)
                    Idx = 0
                    RaiseEvent OnItemChanged(IndexFrom, 0)
                    Return 0
                ElseIf IndexTo < 0 Then
                    Dim Item = _Playlist.Item(IndexFrom)
                    _Playlist.RemoveAt(IndexFrom)
                    _Playlist.Insert(Count - 1, Item)
                    Idx = Count - 1
                    RaiseEvent OnItemChanged(IndexFrom, Count - 1)
                    Return Count - 1
                Else
                    Dim Item = _Playlist.Item(IndexFrom)
                    _Playlist.RemoveAt(IndexFrom)
                    _Playlist.Insert(IndexTo, Item)
                    Idx = IndexTo
                    RaiseEvent OnItemChanged(IndexFrom, IndexTo)
                    Return IndexTo
                End If
            Else
                If IndexTo >= Count Then
                    Dim Item = _Playlist.Item(IndexFrom)
                    _Playlist.RemoveAt(IndexFrom)
                    _Playlist.Insert(0, Item)
                    RaiseEvent OnItemChanged(IndexFrom, 0)
                    Return 0
                ElseIf IndexTo < 0 Then
                    Dim Item = _Playlist.Item(IndexFrom)
                    _Playlist.RemoveAt(IndexFrom)
                    _Playlist.Insert(Count - 1, Item)
                    RaiseEvent OnItemChanged(IndexFrom, Count - 1)
                    Return Count - 1
                Else
                    Dim Item = _Playlist.Item(IndexFrom)
                    _Playlist.RemoveAt(IndexFrom)
                    _Playlist.Insert(IndexTo, Item)
                    RaiseEvent OnItemChanged(IndexFrom, IndexTo)
                    Return IndexTo
                End If
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
