Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class YearItem
    Implements INotifyPropertyChanged

    Private _num As Integer
    Private _year As String
    Private _songs As New List(Of String)
    Private _songsc As Integer

    Public Sub New(ByVal pnum As Integer, ByVal YearElement As Library.YearElement, <CallerMemberName> ByVal Optional caller As String = Nothing)
        Num = pnum
        Year = YearElement.Year
        Songs = YearElement.Songs
        SongsC = Songs.Count
    End Sub

    Public Property Num As Integer
        Get
            Return _num
        End Get
        Set(ByVal value As Integer)
            If _num = value Then Return
            _num = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Year As String
        Get
            Return _year
        End Get
        Set(ByVal value As String)
            If _year = value Then Return
            _year = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Songs As List(Of String)
        Get
            Return _songs
        End Get
        Set(ByVal value As List(Of String))
            If _songs Is value Then Return
            _songs = value
            OnPropertyChanged()
        End Set
    End Property
    Public Property SongsC As Integer
        Get
            Return _songsc
        End Get
        Set(value As Integer)
            _songsc = value
            OnPropertyChanged()
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Overridable Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
