Imports System.Xml
Public Class Library
    Public LibraryPath As String = Nothing
    Private ReadOnly WriterSettings As New XmlWriterSettings With {.Indent = True, .Async = True}
    Private Property _Count As Integer
    Public Property Count As Integer
        Get
            Return _Count
        End Get
        Set(value As Integer)
            _Count = value
            RaiseEvent OnCountChanged(value)
        End Set
    End Property
    Public Property DateCreated As String
    Public Property IsLoaded As Boolean = False
    Public Property Paths As New List(Of String)
    Public Event OnCountChanged(Count As Integer)
    Public Event OnItemsChanged()
    Private CTracks As List(Of String) = Nothing
    Private CArtists As List(Of ArtistElement) = Nothing
    Private CYears As List(Of YearElement) = Nothing
    Public ReadOnly Property Cache As LibraryCache
        Get
            Return New LibraryCache With {.Tracks = CTracks, .Artists = CArtists, .Years = CYears}
        End Get
    End Property
    Public Class LibraryCache
        Public Property Tracks As List(Of String)
        Public Property Artists As List(Of ArtistElement)
        Public Property Years As List(Of YearElement)
    End Class
    Public Class ArtistElement
        Public Property Name As String
        Public Property Songs As New List(Of String)
        Public Sub New(_name As String, files As String())
            Name = _name
            Songs.AddRange(files)
        End Sub
        Public Sub New(_name As String, listfiles As List(Of String))
            Name = _name
            Songs.AddRange(listfiles)
        End Sub
        Public Sub New(_name As String)
            Name = _name
        End Sub
    End Class
    Public Class YearElement
        Public Property Year As String
        Public Property Songs As New List(Of String)
        Public Sub New(_year As String, files As String())
            Year = _year
            Songs.AddRange(files)
        End Sub
        Public Sub New(_year As String, listfiles As List(Of String))
            Year = _year
            Songs.AddRange(listfiles)
        End Sub
        Public Sub New(_year As String)
            Year = _year
        End Sub
    End Class
    Public Sub New(path As String)
        If LoadLibrary(path) Then
            IsLoaded = True
        Else
            IsLoaded = False
        End If
    End Sub
    Public Sub New()
    End Sub
    Public Shared Async Function MakeLibrary(DirectoryPath As String, LibraryPaths As List(Of String)) As Task(Of String)
        If Not IO.Directory.Exists(IO.Path.Combine(DirectoryPath, "Library")) Then IO.Directory.CreateDirectory(IO.Path.Combine(DirectoryPath, "Library"))
        Dim Writer As XmlWriter = XmlWriter.Create(IO.Path.Combine(DirectoryPath, "Library", "library.xml"), New XmlWriterSettings With {.Indent = True, .Async = True})
        ' Begin writing.        
        Await Writer.WriteStartDocumentAsync()
        Writer.WriteStartElement("MuPlay")
        Writer.WriteStartElement("Library")
        Writer.WriteAttributeString("date", Now.ToString("dd/MM/yyyy HH:mm:ss"))
        Writer.WriteAttributeString("count", 0)
        Await Writer.WriteEndElementAsync
        Writer.WriteStartElement("Paths")
        For Each path In LibraryPaths
            Writer.WriteElementString("Path", path)
        Next
        Await Writer.WriteEndElementAsync
        Writer.WriteStartElement("Tracks")
        Await Writer.WriteEndElementAsync
        Await Writer.WriteEndElementAsync
        Await Writer.WriteEndDocumentAsync
        Writer.Close()
        Return Await Task.FromResult(IO.Path.Combine(DirectoryPath, "Library", "library.xml"))
    End Function
    Public Function LoadLibrary(path As String) As Boolean
        If IO.File.Exists(path) Then
            LibraryPath = path
            Try
                Dim LibDocument As New XmlDocument()
                LibDocument.Load(path)
                Dim attr = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes
                Count = attr(1).Value
                DateCreated = attr(0).Value
                Dim lpaths = LibDocument.SelectNodes("/MuPlay/Paths/Path")
                For Each lpath As XmlNode In lpaths
                    Paths.Add(lpath.InnerText)
                Next
                IsLoaded = True
                LibraryPath = path
                Return True
            Catch ex As Exception
                Count = 0
                DateCreated = Nothing
                IsLoaded = False
                Return False
            End Try
        Else
            IsLoaded = False
            Return False
        End If
    End Function
    Public Sub AddTracksToLibrary(files As String())
        Dim LibDocument As New XmlDocument
        LibDocument.Load(LibraryPath)
        Dim Cnt = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value
        Dim iCnt As Integer
        If Integer.TryParse(Cnt, iCnt) Then
            Cnt = iCnt
        Else
            Cnt = 0
        End If
        Cnt += files.Count
        Dim TrackNode = LibDocument.SelectSingleNode("/MuPlay/Tracks")
        For Each song In files
            Try
                Dim tag = TagLib.File.Create(song).Tag
                Dim xmlsong = LibDocument.CreateElement("Track")
                xmlsong.SetAttribute("title", tag.Title)
                xmlsong.SetAttribute("artist", tag.JoinedPerformers)
                xmlsong.SetAttribute("year", tag.Year)
                xmlsong.SetAttribute("source", song)
                TrackNode.AppendChild(xmlsong)
            Catch ex As Exception
                Continue For
            End Try
        Next
        LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value = Cnt
        Count = Cnt
        LibDocument.Save(LibraryPath)
        RaiseEvent OnItemsChanged()
    End Sub
    Public Async Function RefreshStatsFromLibrary() As Task
        Await Task.Run(Sub()
                           Dim LibDocument As New XmlDocument
                           LibDocument.Load(LibraryPath)
                           Dim Cnt = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value
                           Dim Dte = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(0).Value
                           DateCreated = Dte
                           Count = Cnt
                           LibDocument = Nothing
                       End Sub)
    End Function
    Public Async Function RefreshStats() As Task
        Await Task.Run(Async Function()
                           Dim LibDocument As New XmlDocument
                           LibDocument.Load(LibraryPath)
                           Dim Gtracks = Await GroupTracksAsync()
                           LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value = Gtracks.Count
                           Count = Gtracks.Count
                           LibDocument.Save(LibraryPath)
                           LibDocument = Nothing
                       End Function)
    End Function
    Public Async Function AddTracksToLibraryAsync(files As String()) As Task
        Await Task.Run(Sub()
                           Dim LibDocument As New XmlDocument
                           LibDocument.Load(LibraryPath)
                           Dim Cnt = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value
                           Dim iCnt As Integer
                           If Integer.TryParse(Cnt, iCnt) Then
                               Cnt = iCnt
                           Else
                               Cnt = 0
                           End If
                           Cnt += files.Count
                           Dim TrackNode = LibDocument.SelectSingleNode("/MuPlay/Tracks")
                           For Each song In files
                               Try
                                   Dim tag = TagLib.File.Create(song).Tag
                                   Dim xmlsong = LibDocument.CreateElement("Track")
                                   xmlsong.SetAttribute("title", tag.Title)
                                   xmlsong.SetAttribute("artist", tag.JoinedPerformers)
                                   xmlsong.SetAttribute("year", tag.Year)
                                   xmlsong.SetAttribute("source", song)
                                   TrackNode.AppendChild(xmlsong)
                               Catch ex As Exception
                                   Continue For
                               End Try
                           Next
                           LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value = Cnt
                           Count = Cnt
                           LibDocument.Save(LibraryPath)
                       End Sub)
    End Function
    Public Async Function AddTracksToLibraryAsync(files As List(Of String)) As Task
        Await Task.Run(Sub()
                           Dim LibDocument As New XmlDocument
                           LibDocument.Load(LibraryPath)
                           Dim Cnt = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value
                           Dim iCnt As Integer
                           If Integer.TryParse(Cnt, iCnt) Then
                               Cnt = iCnt
                           Else
                               Cnt = 0
                           End If
                           Cnt += files.Count
                           Dim TrackNode = LibDocument.SelectSingleNode("/MuPlay/Tracks")
                           For Each song In files
                               Try
                                   Dim tag = TagLib.File.Create(song).Tag
                                   Dim xmlsong = LibDocument.CreateElement("Track")
                                   xmlsong.SetAttribute("title", tag.Title)
                                   xmlsong.SetAttribute("artist", tag.JoinedPerformers)
                                   xmlsong.SetAttribute("year", tag.Year)
                                   xmlsong.SetAttribute("source", song)
                                   TrackNode.AppendChild(xmlsong)
                               Catch ex As Exception
                                   Continue For
                               End Try
                           Next
                           LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value = Cnt
                           Count = Cnt
                           LibDocument.Save(LibraryPath)
                       End Sub)
    End Function
    Public Async Function RemoveTracksFromLibraryAsync(files As String()) As Task
        Await Task.Run(Sub()
                           Dim LibDocument As New XmlDocument
                           LibDocument.Load(LibraryPath)
                           Dim Cnt = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value
                           Dim iCnt As Integer
                           If Integer.TryParse(Cnt, iCnt) Then
                               Cnt = iCnt
                           Else
                               Cnt = 0
                           End If
                           Cnt += files.Count
                           Dim TrackNode = LibDocument.SelectSingleNode("/MuPlay/Tracks")
                           Dim TBRNodes As New List(Of XmlNode)
                           For Each songn As XmlNode In LibDocument.SelectNodes("/MuPlay/Tracks/Track")
                               If files.Contains(songn.Attributes.Item(3).Value) Then
                                   TBRNodes.Add(songn)
                               End If
                           Next
                           For Each node In TBRNodes
                               TrackNode.RemoveChild(node)
                           Next
                           LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value = Cnt - TBRNodes.Count
                           Count = Cnt - TBRNodes.Count
                           LibDocument.Save(LibraryPath)
                       End Sub)
    End Function
    Public Async Function RemoveTracksFromLibraryAsync(files As List(Of String)) As Task
        Await Task.Run(Sub()
                           Dim LibDocument As New XmlDocument
                           LibDocument.Load(LibraryPath)
                           Dim Cnt = LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value
                           Dim iCnt As Integer
                           If Integer.TryParse(Cnt, iCnt) Then
                               Cnt = iCnt
                           Else
                               Cnt = 0
                           End If
                           Cnt += files.Count
                           Dim TrackNode = LibDocument.SelectSingleNode("/MuPlay/Tracks")
                           Dim TBRNodes As New List(Of XmlNode)
                           For Each songn As XmlNode In LibDocument.SelectNodes("/MuPlay/Tracks/Track")
                               If files.Contains(songn.Attributes.Item(3).Value) Then
                                   TBRNodes.Add(songn)
                               End If
                           Next
                           For Each node In TBRNodes
                               TrackNode.RemoveChild(node)
                           Next
                           LibDocument.SelectSingleNode("/MuPlay/Library").Attributes(1).Value = Cnt - TBRNodes.Count
                           Count = Cnt - TBRNodes.Count
                           LibDocument.Save(LibraryPath)
                       End Sub)
    End Function
    Public Function GroupArtists() As List(Of ArtistElement)
        Dim Artists As New List(Of ArtistElement)
        Dim LibDocument As New XmlDocument
        LibDocument.Load(LibraryPath)
        Dim ArtistNode = LibDocument.SelectNodes("/MuPlay/Tracks/Track")
        For Each song As XmlNode In ArtistNode
            Dim _artist = Artists.FirstOrDefault(Function(artist) artist.Name = song.Attributes.Item(1).Value)
            If _artist IsNot Nothing Then
                _artist.Songs.Add(song.Attributes.Item(3).Value)
            Else
                Artists.Add(New ArtistElement(song.Attributes.Item(1).Value, {song.Attributes.Item(3).Value}))
            End If
        Next
        Artists = Artists.OrderBy(Function(k) k.Name).ToList
        CArtists = Artists
        Return Artists
    End Function
    Public Async Function GroupArtistsAsync() As Task(Of List(Of ArtistElement))
        Dim GArtists = Await Task.Run(Function()
                                          Dim Artists As New List(Of ArtistElement)
                                          Dim LibDocument As New XmlDocument
                                          LibDocument.Load(LibraryPath)
                                          Dim ArtistNode = LibDocument.SelectNodes("/MuPlay/Tracks/Track")
                                          For Each song As XmlNode In ArtistNode
                                              Dim _artist = Artists.FirstOrDefault(Function(artist) artist.Name = song.Attributes.Item(1).Value)
                                              If _artist IsNot Nothing Then
                                                  _artist.Songs.Add(song.Attributes.Item(3).Value)
                                              Else
                                                  Artists.Add(New ArtistElement(song.Attributes.Item(1).Value, {song.Attributes.Item(3).Value}))
                                              End If
                                          Next
                                          Artists = Artists.OrderBy(Function(k) k.Name).ToList
                                          CArtists = Artists
                                          Return Artists
                                      End Function)
        Return Await Task.FromResult(GArtists)
    End Function
    Public Function GroupYears() As List(Of YearElement)
        Dim Years As New List(Of YearElement)
        Dim LibDocument As New XmlDocument
        LibDocument.Load(LibraryPath)
        Dim ArtistNode = LibDocument.SelectNodes("/MuPlay/Tracks/Track")
        For Each song As XmlNode In ArtistNode
            Dim _artist = Years.FirstOrDefault(Function(artist) artist.Year = song.Attributes.Item(2).Value)
            If _artist IsNot Nothing Then
                _artist.Songs.Add(song.Attributes.Item(3).Value)
            Else
                Years.Add(New YearElement(song.Attributes.Item(2).Value, {song.Attributes.Item(3).Value}))
            End If
        Next
        Years = Years.OrderByDescending(Function(k) CInt(k.Year)).ToList
        CYears = Years
        Return Years
    End Function
    Public Async Function GroupYearsAsync() As Task(Of List(Of YearElement))
        Dim GYears = Await Task.Run(Function()
                                        Dim Years As New List(Of YearElement)
                                        Dim LibDocument As New XmlDocument
                                        LibDocument.Load(LibraryPath)
                                        Dim ArtistNode = LibDocument.SelectNodes("/MuPlay/Tracks/Track")
                                        For Each song As XmlNode In ArtistNode
                                            Dim _artist = Years.FirstOrDefault(Function(artist) artist.Year = song.Attributes.Item(2).Value)
                                            If _artist IsNot Nothing Then
                                                _artist.Songs.Add(song.Attributes.Item(3).Value)
                                            Else
                                                Years.Add(New YearElement(song.Attributes.Item(2).Value, {song.Attributes.Item(3).Value}))
                                            End If
                                        Next
                                        Years = Years.OrderByDescending(Function(k) CInt(k.Year)).ToList
                                        CYears = Years
                                        Return Years
                                    End Function)
        Return Await Task.FromResult(GYears)
    End Function
    Public Function GroupTracks() As List(Of String)
        Dim Tracks As New List(Of String)
        Dim LibDocument As New XmlDocument
        LibDocument.Load(LibraryPath)
        Dim SongNode = LibDocument.SelectNodes("/MuPlay/Tracks/Track")
        For Each song As XmlNode In SongNode
            Tracks.Add(song.Attributes.Item(3).Value)
        Next
        Tracks.Sort()
        CTracks = Tracks
        Return Tracks
    End Function
    Public Async Function GroupTracksAsync() As Task(Of List(Of String))
        Dim GTracks = Await Task.Run(Function()
                                         Dim Tracks As New List(Of String)
                                         Dim LibDocument As New XmlDocument
                                         LibDocument.Load(LibraryPath)
                                         Dim SongNode = LibDocument.SelectNodes("/MuPlay/Tracks/Track")
                                         For Each song As XmlNode In SongNode
                                             Tracks.Add(song.Attributes.Item(3).Value)
                                         Next
                                         CTracks = Tracks
                                         Return Tracks
                                     End Function)
        Return Await Task.FromResult(GTracks)
    End Function
    Public Async Function CacheArtists(DirectoryPath As String) As Task
        Await Task.Run(Async Function()
                           Dim Gartists = Await GroupArtistsAsync()
                           If Not IO.Directory.Exists(IO.Path.Combine(DirectoryPath, "Library")) Then IO.Directory.CreateDirectory(IO.Path.Combine(DirectoryPath, "Library"))
                           Dim Writer As XmlWriter = XmlWriter.Create(IO.Path.Combine(DirectoryPath, "Library", "artists.xml"), New XmlWriterSettings With {.Indent = True, .Async = True})
                           ' Begin writing.
                           Await Writer.WriteStartDocumentAsync()
                           Writer.WriteStartElement("MuPlay")
                           Writer.WriteStartElement("Artists")
                           For Each artist In Gartists
                               Writer.WriteStartElement("Artist")
                               Writer.WriteAttributeString("Name", artist.Name)
                               For Each song In artist.Songs
                                   Writer.WriteElementString("Song", song)
                               Next
                               Writer.WriteEndElement()
                           Next
                           Await Writer.WriteEndElementAsync
                           Await Writer.WriteEndElementAsync
                           Await Writer.WriteEndDocumentAsync
                           Writer.Close()
                       End Function)
    End Function
    Public Async Function ReadArtistsCache(DirectoryPath) As Task(Of List(Of ArtistElement))
        Dim GArtists = Await Task.Run(Function()
                                          Dim Artists As New List(Of ArtistElement)
                                          Dim LibDocument As New XmlDocument
                                          LibDocument.Load(IO.Path.Combine(DirectoryPath, "Library", "artists.xml"))
                                          Dim _Artists = LibDocument.SelectNodes("/MuPlay/Artists/Artist")
                                          For Each __Artist As XmlNode In _Artists
                                              Dim _artist = Artists.FirstOrDefault(Function(artist) artist.Name = __Artist.Attributes.Item(0).Value)
                                              If _artist IsNot Nothing Then
                                                  For Each song As XmlNode In __Artist.SelectNodes("Song")
                                                      _artist.Songs.Add(song.InnerText)
                                                  Next
                                              Else
                                                  Dim Songs As New List(Of String)
                                                  For Each song As XmlNode In __Artist.SelectNodes("Song")
                                                      Songs.Add(song.InnerText)
                                                  Next
                                                  Artists.Add(New ArtistElement(__Artist.Attributes.Item(0).Value, Songs))
                                              End If
                                          Next
                                          CArtists = Artists
                                          Return Artists
                                      End Function)
        Return Await Task.FromResult(GArtists)
    End Function
    Public Async Function CacheYears(DirectoryPath As String) As Task
        Await Task.Run(Async Function()
                           Dim Gyears = Await GroupYearsAsync()
                           If Not IO.Directory.Exists(IO.Path.Combine(DirectoryPath, "Library")) Then IO.Directory.CreateDirectory(IO.Path.Combine(DirectoryPath, "Library"))
                           Dim Writer As XmlWriter = XmlWriter.Create(IO.Path.Combine(DirectoryPath, "Library", "years.xml"), New XmlWriterSettings With {.Indent = True, .Async = True})
                           ' Begin writing.
                           Await Writer.WriteStartDocumentAsync()
                           Writer.WriteStartElement("MuPlay")
                           Writer.WriteStartElement("Years")
                           For Each _Year In Gyears
                               Writer.WriteStartElement("Year")
                               Writer.WriteAttributeString("Name", _Year.Year)
                               For Each song In _Year.Songs
                                   Writer.WriteElementString("Song", song)
                               Next
                               Writer.WriteEndElement()
                           Next
                           Await Writer.WriteEndElementAsync
                           Await Writer.WriteEndElementAsync
                           Await Writer.WriteEndDocumentAsync
                           Writer.Close()
                       End Function)
    End Function
    Public Async Function ReadYearsCache(DirectoryPath As String) As Task(Of List(Of YearElement))
        Dim GYears = Await Task.Run(Function()
                                        Dim Years As New List(Of YearElement)
                                        Dim LibDocument As New XmlDocument
                                        LibDocument.Load(IO.Path.Combine(DirectoryPath, "Library", "years.xml"))
                                        Dim _Years = LibDocument.SelectNodes("/MuPlay/Years/Year")
                                        For Each __Year As XmlNode In _Years
                                            Dim _year = Years.FirstOrDefault(Function(year) year.Year = __Year.Attributes.Item(0).Value)
                                            If _year IsNot Nothing Then
                                                For Each song As XmlNode In __Year.SelectNodes("Song")
                                                    _year.Songs.Add(song.InnerText)
                                                Next
                                            Else
                                                Dim Songs As New List(Of String)
                                                For Each song As XmlNode In __Year.SelectNodes("Song")
                                                    Songs.Add(song.InnerText)
                                                Next
                                                Years.Add(New YearElement(__Year.Attributes.Item(0).Value, Songs))
                                            End If
                                        Next
                                        CYears = Years
                                        Return Years
                                    End Function)
        Return Await Task.FromResult(GYears)
    End Function
    Public Async Function IsSongExists(file As String) As Task(Of Boolean)
        If CTracks IsNot Nothing Then
            Return CTracks.Contains(file)
        Else
            Await GroupTracksAsync()
            Return CTracks.Contains(file)
        End If
    End Function
End Class
