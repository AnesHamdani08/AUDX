Imports System.ComponentModel

Public Class LibraryManager
    Public Enum LibraryNodeType
        Stats
        Paths
        Tracks
        Artists
        Years
    End Enum
    Public Enum LibraryNodeCaller
        LibStats
        LibDate
        LibCount
        LibPaths
        LibPath
        LibTracks
        LibTrack
        LibArtists
        LibArtist
        LibYears
        LibYear
    End Enum
    Public Class ItemIdentifier
        Public Property Type As LibraryNodeType
        Public Property Element As XElement
        Public Property Caller As LibraryNodeCaller
        Public Property Index As Integer
        Public Property IndexGrade As ItemGrade
        Public Sub New(NodeType As LibraryNodeType, Xelem As XElement, CalledBy As LibraryNodeCaller)
            Type = NodeType
            Element = Xelem
            Caller = CalledBy
        End Sub
        Public Sub New(NodeType As LibraryNodeType, Xelem As XElement, CalledBy As LibraryNodeCaller, i As Integer, Grade As ItemGrade)
            Type = NodeType
            Element = Xelem
            Caller = CalledBy
            Index = i
            IndexGrade = Grade
        End Sub
        Public Enum ItemGrade
            Parent
            Child
        End Enum
    End Class
    Dim Library As New Library(My.Settings.LIBRARY_PATH)
    Private Async Sub ShowOverlay(Show As Boolean, Optional Loading As Boolean = False)
        If Show Then
            Overlay.Visibility = Visibility.Visible
            If Loading Then
                Overlay_LoadingLine.IsRunning = True
            End If
            Overlay_LoadingLine.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(250))))
            Overlay.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(1, New Duration(TimeSpan.FromMilliseconds(250))))
        Else
            Overlay_LoadingLine.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(250))))
            Overlay_LoadingLine.IsRunning = False
            Overlay.BeginAnimation(OpacityProperty, New Animation.DoubleAnimation(0, New Duration(TimeSpan.FromMilliseconds(250))))
            Await Task.Delay(250)
            Overlay.Visibility = Visibility.Hidden
        End If
    End Sub
    Private Sub LibraryManager_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub

    Private Sub LibraryManager_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Dim s As Style = New Style()
        s.Setters.Add(New Setter(VisibilityProperty, Visibility.Collapsed))
        Tracks_TabControl.ItemContainerStyle = s
        Artists_TabControl.ItemContainerStyle = s
        MakeTracksTree()
        MakeArtistsTree()
        Artists_Tracks.ItemsSource = ArtistsTracks_Source
    End Sub
    Private Async Sub Btn_Save_Click(sender As Object, e As RoutedEventArgs) Handles Btn_Save.Click
        If MessageBox.Show("Are you sure you want to overwrite the current library ?", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.Yes Then
            ShowOverlay(True, True)
            Try
                Await Application.Current.Dispatcher.BeginInvoke(Sub()
                                                                     'Building the XDocument
                                                                     Dim Doc As New XDocument()
                                                                     Dim Root As New XElement("MuPlay")
                                                                     Dim LibDate = CType(CType(CType(Tracks_TreeView.Items.Item(0), TreeViewItem).Items.Item(0), TreeViewItem).Tag, ItemIdentifier).Element.Attribute("date").Value
                                                                     Dim LibCount = CType(CType(CType(Tracks_TreeView.Items.Item(0), TreeViewItem).Items.Item(1), TreeViewItem).Tag, ItemIdentifier).Element.Attribute("count").Value
                                                                     Dim LibraryParent As New XElement("Library", {New XAttribute("date", LibDate), New XAttribute("count", LibCount)})
                                                                     Dim PathsParent As New XElement("Paths")
                                                                     For Each Path As TreeViewItem In CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Items
                                                                         Dim IID As ItemIdentifier = Path.Tag
                                                                         PathsParent.Add(IID.Element)
                                                                     Next
                                                                     Dim TracksParent As New XElement("Tracks")
                                                                     For Each Track As TreeViewItem In CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items
                                                                         Dim IID As ItemIdentifier = Track.Tag
                                                                         TracksParent.Add(IID.Element)
                                                                     Next
                                                                     Root.Add({LibraryParent, PathsParent, TracksParent})
                                                                     Doc.Add(Root)
                                                                     Dim FSD As New Ookii.Dialogs.Wpf.VistaSaveFileDialog With {.CheckFileExists = True, .Filter = "XML|*.xml", .Title = "Where to save the library", .AddExtension = True}
                                                                     If FSD.ShowDialog Then
                                                                         'Saving the XDocument
                                                                         Doc.Save(My.Settings.LIBRARY_PATH)
                                                                     End If
                                                                 End Sub)
            Catch ex As Exception
                ShowOverlay(False)
                Throw ex
            End Try
            ShowOverlay(False)
        End If
    End Sub

    Private Async Sub Btn_SaveAs_Click(sender As Object, e As RoutedEventArgs) Handles Btn_SaveAs.Click
        ShowOverlay(True, True)
        Try
            Await Task.Run(Sub()
                               'Building the XDocument
                               Dim Doc As New XDocument()
                               Dim Root As New XElement("MuPlay")
                               Dim LibDate = CType(CType(CType(Tracks_TreeView.Items.Item(0), TreeViewItem).Items.Item(0), TreeViewItem).Tag, ItemIdentifier).Element.Attribute("date").Value
                               Dim LibCount = CType(CType(CType(Tracks_TreeView.Items.Item(0), TreeViewItem).Items.Item(1), TreeViewItem).Tag, ItemIdentifier).Element.Attribute("count").Value
                               Dim LibraryParent As New XElement("Library", {New XAttribute("date", LibDate), New XAttribute("count", LibCount)})
                               Dim PathsParent As New XElement("Paths")
                               For Each Path As TreeViewItem In CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Items
                                   Dim IID As ItemIdentifier = Path.Tag
                                   PathsParent.Add(IID.Element)
                               Next
                               Dim TracksParent As New XElement("Tracks")
                               For Each Track As TreeViewItem In CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items
                                   Dim IID As ItemIdentifier = Track.Tag
                                   TracksParent.Add(IID.Element)
                               Next
                               Root.Add({LibraryParent, PathsParent, TracksParent})
                               Doc.Add(Root)
                               Dim FSD As New Ookii.Dialogs.Wpf.VistaSaveFileDialog With {.CheckFileExists = True, .Filter = "XML|*.xml", .Title = "Where to save the library", .AddExtension = True}
                               If FSD.ShowDialog Then
                                   'Saving the XDocument
                                   Doc.Save(FSD.FileName)
                               End If
                           End Sub)
        Catch ex As Exception
            ShowOverlay(False)
            Throw ex
        End Try
        ShowOverlay(False)
    End Sub

    Private Async Sub Btn_SyncArtistsFromTracks_Click(sender As Object, e As RoutedEventArgs) Handles Btn_SyncArtistsFromTracks.Click
        ShowOverlay(True, True)
        Library.LoadLibrary(My.Settings.LIBRARY_PATH)
        Await Library.CacheArtists(Utils.AppDataPath)
        ShowOverlay(False)
    End Sub

    Private Async Sub Btn_SyncYearsFromTracks_Click(sender As Object, e As RoutedEventArgs) Handles Btn_SyncYearsFromTracks.Click
        ShowOverlay(True, True)
        Library.LoadLibrary(My.Settings.LIBRARY_PATH)
        Await Library.CacheYears(Utils.AppDataPath)
        ShowOverlay(False)
    End Sub
#Region "Tracks"
    Dim TracksXMLLib As XDocument
    Private Sub MakeTracksTree()
        TracksXMLLib = XDocument.Load(My.Settings.LIBRARY_PATH)
        Dim Root = TracksXMLLib.Root
        Dim LibStatistics = Root.Element("Library")
        Dim LibStatisticsTreeItem As New TreeViewItem() With {.Header = "Library", .Tag = New ItemIdentifier(LibraryNodeType.Stats, LibStatistics, LibraryNodeCaller.LibStats, 0, ItemIdentifier.ItemGrade.Parent)}
        LibStatisticsTreeItem.Items.Add(New TreeViewItem With {.Header = "Date: " & LibStatistics.Attribute("date").Value, .IsEnabled = False, .Tag = New ItemIdentifier(LibraryNodeType.Stats, LibStatistics, LibraryNodeCaller.LibDate, 0, ItemIdentifier.ItemGrade.Child)})
        LibStatisticsTreeItem.Items.Add(New TreeViewItem With {.Header = "Count: " & LibStatistics.Attribute("count").Value, .IsEnabled = False, .Tag = New ItemIdentifier(LibraryNodeType.Stats, LibStatistics, LibraryNodeCaller.LibCount, 1, ItemIdentifier.ItemGrade.Child)})
        Dim LibPaths = Root.Element("Paths")
        Dim LibPathsTreeItem As New TreeViewItem With {.Header = "Paths", .Tag = New ItemIdentifier(LibraryNodeType.Paths, LibPaths, LibraryNodeCaller.LibPaths, 1, ItemIdentifier.ItemGrade.Parent)}
        Dim i As Integer = 0
        For Each Path In LibPaths.Elements
            LibPathsTreeItem.Items.Add(New TreeViewItem With {.Header = Path.Value, .Tag = New ItemIdentifier(LibraryNodeType.Paths, Path, LibraryNodeCaller.LibPath, i, ItemIdentifier.ItemGrade.Child)})
            i += 1
        Next
        Dim LibTracks = Root.Element("Tracks")
        Dim LibTracksTreeItem As New TreeViewItem With {.Header = "Tracks", .Tag = New ItemIdentifier(LibraryNodeType.Tracks, LibTracks, LibraryNodeCaller.LibTracks, 2, ItemIdentifier.ItemGrade.Parent)}
        i = 0
        For Each Track In LibTracks.Elements
            LibTracksTreeItem.Items.Add(New TreeViewItem With {.Header = Track.Attribute("source").Value, .Tag = New ItemIdentifier(LibraryNodeType.Tracks, Track, LibraryNodeCaller.LibTrack, i, ItemIdentifier.ItemGrade.Child)})
            i += 1
        Next
        Tracks_TreeView.Items.Add(LibStatisticsTreeItem)
        Tracks_TreeView.Items.Add(LibPathsTreeItem)
        Tracks_TreeView.Items.Add(LibTracksTreeItem)
    End Sub

    Private Sub Tracks_TreeView_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles Tracks_TreeView.SelectedItemChanged
        Dim IID = CType(CType(Tracks_TreeView.SelectedItem, TreeViewItem).Tag, ItemIdentifier)
        Select Case IID.Type
            Case LibraryNodeType.Stats
                Select Case IID.Caller
                    Case LibraryNodeCaller.LibStats
                        RefreshTracksStats(1)
                End Select
                Tracks_TabControl.SelectedIndex = 1
            Case LibraryNodeType.Paths
                Select Case IID.Caller
                    Case LibraryNodeCaller.LibPaths
                        RefreshTracksStats(2)
                End Select
                Tracks_TabControl.SelectedIndex = 2
            Case LibraryNodeType.Tracks
                Select Case IID.Caller
                    Case LibraryNodeCaller.LibTracks
                        RefreshTracksStats(3)
                    Case LibraryNodeCaller.LibTrack
                        FillTrackInfo(IID.Index)
                End Select
                Tracks_TabControl.SelectedIndex = 3
            Case Else
        End Select
    End Sub
    Private Sub RefreshTracksStats(n As Integer)
        Select Case n
            Case 1
                Dim IID = CType(CType(Tracks_TreeView.Items.Item(0), TreeViewItem).Tag, ItemIdentifier)
                If IID.Element.HasElements Then
                    Tracks_Stats_DateCreated.SelectedDateTime = Date.ParseExact(IID.Element.Attribute("date").Value, "dd/MM/yyyy HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                    Tracks_Stats_TrackCount.Value = IID.Element.Attribute("count").Value
                End If
            Case 2
                Dim IID = CType(CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Tag, ItemIdentifier)
                If IID.Element.HasElements Then
                    Tracks_Paths_PathCount.Value = IID.Element.Elements.Count
                End If
            Case 3
                Dim IID = CType(CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Tag, ItemIdentifier)
                If IID.Element.HasElements Then
                    Tracks_Tracks_Count.Value = IID.Element.Elements.Count
                End If
        End Select
    End Sub

    Private Sub Tracks_Paths_Add_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Paths_Add.Click
        Dim FBD As New Ookii.Dialogs.Wpf.VistaFolderBrowserDialog With {.Description = "Select or create a directory", .ShowNewFolderButton = True}
        If FBD.ShowDialog Then
            Dim Xelem As New XElement("Path") With {.Value = FBD.SelectedPath}
            CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Items.Add(New TreeViewItem With {.Header = FBD.SelectedPath, .Tag = New ItemIdentifier(LibraryNodeType.Paths, Xelem, LibraryNodeCaller.LibPath)})
            RefreshTracksPathsIndex()
        End If
    End Sub

    Private Sub Tracks_Paths_Remove_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Paths_Remove.Click
        Dim IID = CType(CType(Tracks_TreeView.SelectedItem, TreeViewItem).Tag, ItemIdentifier)
        If IID.Type = LibraryNodeType.Paths AndAlso IID.Caller = LibraryNodeCaller.LibPath Then
            CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Items.RemoveAt(CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Items.IndexOf(Tracks_TreeView.SelectedItem))
            RefreshTracksPathsIndex()
        End If
    End Sub

    Private Sub Tracks_Paths_RemoveAll_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Paths_RemoveAll.Click
        Dim IID = CType(CType(Tracks_TreeView.SelectedItem, TreeViewItem).Tag, ItemIdentifier)
        If IID.Type = LibraryNodeType.Paths AndAlso IID.Caller = LibraryNodeCaller.LibPath Then
            Tracks_TreeView.Items.Clear()
            RefreshTracksPathsIndex()
        End If
    End Sub
    Private Sub RefreshTracksPathsIndex()
        Dim i = 0
        For Each Path As TreeViewItem In CType(Tracks_TreeView.Items.Item(1), TreeViewItem).Items
            Dim IID = CType(Path.Tag, ItemIdentifier)
            IID.Index = i
            i += 1
        Next
    End Sub
    Private Sub Tracks_Paths_Edit_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Paths_Edit.Click
        Dim IID = CType(CType(Tracks_TreeView.SelectedItem, TreeViewItem).Tag, ItemIdentifier)
        If IID.Type = LibraryNodeType.Paths AndAlso IID.Caller = LibraryNodeCaller.LibPath Then
            Dim IB As New InputDialog("The new path...") With {.Owner = Me}
            If IB.ShowDialog Then
                IID.Element.Value = IB.Input
            End If
        End If
    End Sub
    Private Sub FillTrackInfo(n As Integer)
        Dim IID = CType(CType(CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items.Item(n), TreeViewItem).Tag, ItemIdentifier)
        If IID.Type = LibraryNodeType.Tracks AndAlso IID.Caller = LibraryNodeCaller.LibTrack Then
            Tracks_Tracks_Title.Text = If(IID.Element.Attribute("title").Value, String.Empty)
            Tracks_Tracks_Artist.Text = If(IID.Element.Attribute("artist").Value, String.Empty)
            Tracks_Tracks_Year.Text = If(IID.Element.Attribute("year").Value, String.Empty)
            Tracks_Tracks_Source.Text = If(IID.Element.Attribute("source").Value, String.Empty)
        End If
    End Sub
    Private Sub RefreshTracksTracksIndex()
        Dim i = 0
        For Each Path As TreeViewItem In CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items
            Dim IID = CType(Path.Tag, ItemIdentifier)
            IID.Index = i
            i += 1
        Next
    End Sub
    Private Sub Tracks_Tracks_Add_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Tracks_Add.Click
        Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Filter = Utils.OFDFileFilters, .Multiselect = True, .Title = "Select songs to add..."}
        If OFD.ShowDialog Then
            For Each file In OFD.FileNames
                Dim info = Utils.GetSongInfo(file)
                Dim Xelem As New XElement("Track", {New XAttribute("title", info(1)), New XAttribute("artist", info(0)), New XAttribute("year", info(3)), New XAttribute("source", file)})
                CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items.Add(New TreeViewItem() With {.Header = file, .Tag = New ItemIdentifier(LibraryNodeType.Tracks, Xelem, LibraryNodeCaller.LibTrack)})
            Next
        End If
    End Sub

    Private Sub Tracks_Tracks_Remove_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Tracks_Remove.Click
        Dim IID = CType(CType(Tracks_TreeView.SelectedItem, TreeViewItem).Tag, ItemIdentifier)
        If IID.Type = LibraryNodeType.Tracks AndAlso IID.Caller = LibraryNodeCaller.LibTrack Then
            CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items.RemoveAt(IID.Index)
        End If
    End Sub

    Private Sub Tracks_Tracks_RemoveAll_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Tracks_RemoveAll.Click
        CType(Tracks_TreeView.Items.Item(2), TreeViewItem).Items.Clear()
    End Sub

    Private Sub Tracks_Tracks_Edit_Click(sender As Object, e As RoutedEventArgs) Handles Tracks_Tracks_Edit.Click
        Dim IID = CType(CType(Tracks_TreeView.SelectedItem, TreeViewItem).Tag, ItemIdentifier)
        If IID.Type = LibraryNodeType.Tracks AndAlso IID.Caller = LibraryNodeCaller.LibTrack Then
            If MessageBox.Show("Are you sure you want to edit the track ?", "MuPlay", MessageBoxButton.YesNo, MessageBoxImage.Question) = MessageBoxResult.Yes Then
                IID.Element.Attribute("title").Value = Tracks_Tracks_Title.Text
                IID.Element.Attribute("artist").Value = Tracks_Tracks_Artist.Text
                IID.Element.Attribute("year").Value = Tracks_Tracks_Year.Text
                IID.Element.Attribute("source").Value = Tracks_Tracks_Source.Text
            End If
        End If
    End Sub

#End Region
#Region "Artists"
    Dim ArtistsXMLLib As XDocument
    Dim ArtistsTracks_Source As New ObjectModel.ObservableCollection(Of ArtistViewItem)
    Public Class ArtistViewItem
        Implements INotifyPropertyChanged

        Private _num As Integer
        Private _source As String

        Public Sub New(ByVal pnum As Integer, ByVal psource As String, <Runtime.CompilerServices.CallerMemberName> ByVal Optional caller As String = Nothing)
            Num = pnum
            Source = psource
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
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(<Runtime.CompilerServices.CallerMemberName> ByVal Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
    Private Sub MakeArtistsTree()
        ArtistsXMLLib = XDocument.Load(IO.Path.Combine(Utils.AppDataPath, "Library", "artists.xml"))
        Dim Root = ArtistsXMLLib.Root
        Dim LibArtistsParent = Root.Element("Artists")
        Dim LibArtistsRoot As New TreeViewItem With {.Header = "Artists", .Tag = New ItemIdentifier(LibraryNodeType.Artists, LibArtistsParent, LibraryNodeCaller.LibArtists, 0, ItemIdentifier.ItemGrade.Parent)}
        Dim i As Integer = 0
        For Each Artist As XElement In LibArtistsParent.Elements
            Dim ArtistChild As New TreeViewItem() With {.Header = Artist.Attribute("Name").Value, .Tag = New ItemIdentifier(LibraryNodeType.Artists, Artist, LibraryNodeCaller.LibArtist, i, ItemIdentifier.ItemGrade.Child)}
            Dim _i As Integer = 0
            For Each Track As XElement In Artist.Elements
                ArtistChild.Items.Add(New TreeViewItem With {.Header = Track.Value, .IsEnabled = False, .Tag = New ItemIdentifier(LibraryNodeType.Artists, Track, LibraryNodeCaller.LibTrack, _i, ItemIdentifier.ItemGrade.Child)})
                _i += 1
            Next
            LibArtistsRoot.Items.Add(ArtistChild)
            i += 1
        Next
        Artists_TreeView.Items.Add(LibArtistsRoot)
    End Sub

    Private Sub Artists_TreeView_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles Artists_TreeView.SelectedItemChanged
        Artists_Artists_Count.Value = CType(Artists_TreeView.Items.Item(0), TreeViewItem)?.Items.Count
        Dim IID As ItemIdentifier = CType(Artists_TreeView.SelectedItem, TreeViewItem).Tag
        If IID.Type = LibraryNodeType.Artists AndAlso IID.Caller = LibraryNodeCaller.LibArtist Then
            ArtistsTracks_Source.Clear()
            For Each Song As TreeViewItem In CType(Artists_TreeView.SelectedItem, TreeViewItem).Items
                Dim _IID As ItemIdentifier = Song.Tag
                ArtistsTracks_Source.Add(New ArtistViewItem(ArtistsTracks_Source.Count + 1, _IID.Element.Value))
            Next
            Artists_TabControl.SelectedIndex = 1
        Else
            Artists_TabControl.SelectedIndex = 0
        End If
    End Sub

    Private Sub Artists_Tracks_Add_Click(sender As Object, e As RoutedEventArgs) Handles Artists_Tracks_Add.Click
        Dim IID As ItemIdentifier = CType(Artists_TreeView.SelectedItem, TreeViewItem).Tag
        If IID.Type = LibraryNodeType.Artists AndAlso IID.Caller = LibraryNodeCaller.LibArtist Then
            Dim OFD As New Ookii.Dialogs.Wpf.VistaOpenFileDialog With {.CheckFileExists = True, .Filter = Utils.OFDFileFilters}
            If OFD.ShowDialog Then
                IID.Element.Add(New XElement("Song") With {.Value = OFD.FileName})
                ArtistsTracks_Source.Add(New ArtistViewItem(ArtistsTracks_Source.Count + 1, OFD.FileName))
            End If
        End If
    End Sub

    Private Sub Artists_Tracks_Remove_Click(sender As Object, e As RoutedEventArgs) Handles Artists_Tracks_Remove.Click
        Dim IID As ItemIdentifier = CType(Artists_TreeView.SelectedItem, TreeViewItem).Tag
        If IID.Type = LibraryNodeType.Artists AndAlso IID.Caller = LibraryNodeCaller.LibArtist Then
            If Artists_Tracks.SelectedIndex <> -1 Then
                Dim _IID = CType(CType(CType(Artists_TreeView.SelectedItem, TreeViewItem).Items.Item(Artists_Tracks.SelectedIndex), TreeViewItem).Tag, ItemIdentifier)
                _IID.Element.Remove()
                ArtistsTracks_Source.RemoveAt(_IID.Index)
            End If
        End If
    End Sub

    Private Sub Artists_Tracks_RemoveAll_Click(sender As Object, e As RoutedEventArgs) Handles Artists_Tracks_RemoveAll.Click
        Dim IID As ItemIdentifier = CType(Artists_TreeView.SelectedItem, TreeViewItem).Tag
        If IID.Type = LibraryNodeType.Artists AndAlso IID.Caller = LibraryNodeCaller.LibArtist Then
            IID.Element.RemoveAll()
        End If
    End Sub

    Private Sub Artists_Tracks_Edit_Click(sender As Object, e As RoutedEventArgs) Handles Artists_Tracks_Edit.Click
        Dim IID As ItemIdentifier = CType(Artists_TreeView.SelectedItem, TreeViewItem).Tag
        If IID.Type = LibraryNodeType.Artists AndAlso IID.Caller = LibraryNodeCaller.LibArtist Then
            If Artists_Tracks.SelectedIndex <> -1 Then
                Dim _IID = CType(CType(CType(Artists_TreeView.SelectedItem, TreeViewItem).Items.Item(Artists_Tracks.SelectedIndex), TreeViewItem).Tag, ItemIdentifier)
                Dim IB As New InputDialog("Change " & _IID.Element.Value & " To ...")
                If IB.ShowDialog Then
                    If IO.File.Exists(IB.Input) Then
                        _IID.Element.Value = IB.Input
                        ArtistsTracks_Source.Item(Artists_Tracks.SelectedIndex).Source = IB.Input
                    End If
                End If
                End If
        End If
    End Sub

#End Region
End Class
