Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class PlaylistItem
    Implements INotifyPropertyChanged

    Private _num As Integer
    Private _title As String
    Private _artist As String
    Private _album As String
    Private _year As Integer
    Private _track As Integer
    Private _type As Player.StreamTypes
    Private _source As String
    Private _cover As System.Drawing.Bitmap

    Public Sub New(ByVal pnum As Integer, ByVal ptitle As String, ByVal partist As String, ByVal palbum As String, ByVal pyear As Integer, ByVal ptrack As Integer, ByVal ptype As Player.StreamTypes, ByVal psource As String, pcover As System.Drawing.Bitmap, <CallerMemberName> ByVal Optional caller As String = Nothing)
        Num = pnum
        Title = ptitle
        Artist = partist
        Album = palbum
        Year = pyear
        Track = ptrack
        Type = ptype
        _source = psource
        _cover = pcover
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

    Public Property Title As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            If _title = value Then Return
            _title = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Artist As String
        Get
            Return _artist
        End Get
        Set(ByVal value As String)
            If _artist = value Then Return
            _artist = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Album As String
        Get
            Return _album
        End Get
        Set(ByVal value As String)
            If _album = value Then Return
            _album = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Year As Integer
        Get
            Return _year
        End Get
        Set(ByVal value As Integer)
            If _year = value Then Return
            _year = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Track As Integer
        Get
            Return _track
        End Get
        Set(ByVal value As Integer)
            If _track = value Then Return
            _track = value
            OnPropertyChanged()
        End Set
    End Property
    Public Property Source As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            If _source = value Then Return
            _source = value
            OnPropertyChanged()
        End Set
    End Property

    Public Property Type As Player.StreamTypes
        Get
            Return _type
        End Get
        Set(value As Player.StreamTypes)
            If _type = value Then Return
            _type = value
            OnPropertyChanged()
        End Set
    End Property
    Public Property Cover As System.Drawing.Bitmap
        Get
            Return _cover
        End Get
        Set(value As System.Drawing.Bitmap)
            _cover = value
        End Set
    End Property
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Overridable Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
