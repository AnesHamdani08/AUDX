Imports UPNPLib
Imports WpfPlayer.UPnP_AV.ContentDirectory_v1
Imports WpfPlayer.UPnP_AV
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.ComponentModel

Public Class LanBrowser
#Region "DLNA"
    Public MediaServer As UPnPDevice = Nothing 'The device in which we browse or search for music
    Public ContentDirectory As UPnPService = Nothing 'The service that handles the browsing and searching actions
    Public AllDevices As UPnPDevices = Nothing 'A list with all discovered upnp-devices
    Public DeviceFinder As New UPnPDeviceFinder
    Public WithEvents myDeviceFinderCallback As New myUPnPDeviceFinderCallback
    Dim soMediaServers As Integer = Nothing
    Dim soMediaRenderers As Integer = Nothing
    Dim soRootDevice As Integer = Nothing
    Dim TotalItemDuration As DateTime
    Dim SelectedItemInPlaylist As Integer = -1
    Dim CurrentTreeview As TreeView = Nothing
    Dim myMediaRenderer As UPnPMediaRenderer_v1 = Nothing
    Dim WithEvents DLNAtreeview As New Forms.TreeView With {.BorderStyle = Forms.BorderStyle.None}
    Public Class UPnPDeviceListItemWrapper
        Public UPnPDevice As UPnPDevice = Nothing

        Public Sub New(ByVal UPnPDevice As UPnPDevice)
            Me.UPnPDevice = UPnPDevice
        End Sub

        Public Overrides Function ToString() As String
            If Not Me.UPnPDevice Is Nothing Then Return Me.UPnPDevice.FriendlyName Else Return "Not Set!"
        End Function
    End Class
    <ComVisible(True), ComImport(), Guid("415A984A-88B3-49F3-92AF-0508BEDF0D6C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> Interface IUPnPDeviceFinderCallback
        <PreserveSig()> Function DeviceAdded(ByVal lFindData As Integer, ByVal pDevice As UPNPLib.IUPnPDevice) As Integer
        <PreserveSig()> Function DeviceRemoved(ByVal lFindData As Integer, ByVal bstrUDN As String) As Integer
        <PreserveSig()> Function SearchComplete(ByVal lFindData As Integer) As Integer
    End Interface
    Public Class myUPnPDeviceFinderCallback
        Implements IUPnPDeviceFinderCallback

        Public Event DeviceFound(ByVal lFindData As Integer, ByVal pDevice As UPNPLib.IUPnPDevice)
        Public Event DeviceLost(ByVal lFindData As Integer, ByVal bstrUDN As String)
        Public Event SearchOperationCompleted(ByVal lFindData As Integer)

        Public Function DeviceAdded(ByVal lFindData As Integer, ByVal pDevice As UPNPLib.IUPnPDevice) As Integer Implements IUPnPDeviceFinderCallback.DeviceAdded
            RaiseEvent DeviceFound(lFindData, pDevice)
        End Function

        Public Function DeviceRemoved(ByVal lFindData As Integer, ByVal bstrUDN As String) As Integer Implements IUPnPDeviceFinderCallback.DeviceRemoved
            RaiseEvent DeviceLost(lFindData, bstrUDN)
        End Function

        Public Function SearchComplete(ByVal lFindData As Integer) As Integer Implements IUPnPDeviceFinderCallback.SearchComplete
            RaiseEvent SearchOperationCompleted(lFindData)
        End Function
    End Class
    Private Sub LanBrowser_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        winformsHost.Child = DLNAtreeview
    End Sub

    Private Sub LanBrowser_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Hide()
        e.Cancel = True
    End Sub
    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Refresh_btn.Click
        Refresh_btn.IsEnabled = False
        DLNAServers.Items.Clear()
        soMediaServers = DeviceFinder.CreateAsyncFind("urn:schemas-upnp-org:device:MediaServer:1", 0, myDeviceFinderCallback)
        DeviceFinder.StartAsyncFind(soMediaServers)
        MediaRendererList.Items.Clear()
        soMediaRenderers = DeviceFinder.CreateAsyncFind("urn:schemas-upnp-org:device:MediaRenderer:1", 0, myDeviceFinderCallback)
        DeviceFinder.StartAsyncFind(soMediaRenderers)
        Refresh_btn.IsEnabled = True
    End Sub
    Private Sub myDeviceFinderCallback_DeviceFound(ByVal lFindData As Integer, ByVal pDevice As UPNPLib.IUPnPDevice) Handles myDeviceFinderCallback.DeviceFound
        Select Case lFindData
            Case soMediaServers 'add new device to MediaServer Listbox
                DLNAServers.Items.Add((New UPnPDeviceListItemWrapper(pDevice)))
            Case soMediaRenderers
                MediaRendererList.Items.Add(New UPnPDeviceListItemWrapper(pDevice))
            Case soRootDevice
                '  Me.lstAllUPnPDevices.Items.Add(New UPnPDeviceListItemWrapper(pDevice))
        End Select
    End Sub
    Private Sub DLNAServers_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DLNAServers.SelectionChanged
        If DLNAServers.SelectedIndex >= 0 Then
            Me.MediaServer = DirectCast(DLNAServers.SelectedItem, UPnPDeviceListItemWrapper).UPnPDevice

            For Each myService As UPnPService In Me.MediaServer.Services
                If myService.ServiceTypeIdentifier = "urn:schemas-upnp-org:service:ContentDirectory:1" Then
                    Me.ContentDirectory = myService
                    Exit For 'end the for-loop
                End If
            Next
            Dim rootNode As Forms.TreeNode = DLNAtreeview.Nodes.Add(Me.MediaServer.FriendlyName)
            LoadContainerChildren(rootNode)
        Else
            Me.MediaServer = Nothing
        End If
    End Sub
    Private Sub dlnaservers_SelectedIndexChanged(sender As Object, e As SelectionChangedEventArgs) Handles DLNAServers.SelectionChanged
        If DLNAServers.SelectedIndex >= 0 Then
            Me.MediaServer = DirectCast(DLNAServers.SelectedItem, UPnPDeviceListItemWrapper).UPnPDevice

            For Each myService As UPnPService In Me.MediaServer.Services
                If myService.ServiceTypeIdentifier = "urn:schemas-upnp-org:service:ContentDirectory:1" Then
                    Me.ContentDirectory = myService
                    Exit For 'end the for-loop
                End If
            Next

            DLNAtreeview.Nodes.Clear()

            Dim rootNode As Forms.TreeNode = DLNAtreeview.Nodes.Add(Me.MediaServer.FriendlyName)
            LoadContainerChildren(rootNode)
        Else
            Me.MediaServer = Nothing
        End If
    End Sub
    Private Sub LoadContainerChildren(ByRef parentTreeNode As Forms.TreeNode)
        If TypeOf parentTreeNode.Tag Is CD_Item Then Exit Sub
        Dim myObjects() As CD_Object = Nothing

        If parentTreeNode.Tag Is Nothing Then '=the root-node of the treeview
            myObjects = BrowseContainer("0")
        ElseIf TypeOf parentTreeNode.Tag Is CD_Object Then
            myObjects = BrowseContainer(DirectCast(parentTreeNode.Tag, CD_Object).ID)
        End If

        If myObjects Is Nothing Then Exit Sub

        parentTreeNode.Nodes.Clear()

        For Each myObject As CD_Object In myObjects
            Dim myTreeNode As New Forms.TreeNode
            myTreeNode.Text = myObject.Title
            myTreeNode.Tag = myObject

            parentTreeNode.Nodes.Add(myTreeNode)

            If TypeOf myObject Is CD_Container Then
                myTreeNode.SelectedImageKey = "container"
                myTreeNode.ImageKey = "container"
                If Not myObject.ClassName = "object.container.playlistContainer" Then myTreeNode.Nodes.Add("Dummy")
            Else
                myTreeNode.SelectedImageKey = "item"
                myTreeNode.ImageKey = "item"
            End If
        Next
    End Sub
    Private Function GetCDObjectInformation(ByVal objectID As String) As CD_Object
        If ContentDirectory Is Nothing Then Return Nothing

        Dim myItem As CD_Object = Nothing
        Dim myInObject(5) As Object
        Dim myOutObject(3) As Object

        myInObject.SetValue(objectID, 0) '; //UPnP Object to browse, check SDK for values of the WMV hierarchy
        myInObject.SetValue("BrowseMetadata", 1) '; //SortOfInformation
        myInObject.SetValue("upnp:album,upnp:artist,upnp:genre,upnp:title,res@size,res@duration,res@bitrate,res@sampleFrequency,res@bitsPerSample,res@nrAudioChannels,res@protocolInfo,res@protection,res@importUri", 2) ' Filter
        myInObject.SetValue(0, 3) ' Start Index
        myInObject.SetValue(1, 4) ' Max result count
        myInObject.SetValue("", 5) ' sort Criteria

        Try
            ContentDirectory.InvokeAction("Browse", myInObject, myOutObject)
        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try


        Dim myXMLDoc As New XmlDocument

        Try
            myXMLDoc.LoadXml(CStr(myOutObject(0)))
        Catch ex As Exception
            Try
                myXMLDoc.LoadXml(CStr(myOutObject(0)).Replace("&", "&amp;"))
            Catch ex2 As Exception
                Throw New Exception(ex.ToString, ex2)
            End Try
        End Try

        If myXMLDoc.DocumentElement.ChildNodes.Count = 1 Then
            myItem = CD_Object.Create_CD_Object(myXMLDoc.DocumentElement.ChildNodes(0))
        End If

        Return myItem
    End Function
    Private Function BrowseContainer(ByVal objectID As String) As CD_Object()
        If ContentDirectory Is Nothing Then Return Nothing
        Debug.WriteLine("BrowseContainer: Start")
        Dim myObjects() As CD_Object = Nothing
        Dim myInObject(5) As Object
        Dim myOutObject(3) As Object

        myInObject.SetValue(objectID, 0) '; //UPnP Object to browse, check SDK for values of the WMV hierarchy
        myInObject.SetValue("BrowseDirectChildren", 1) '; //SortOfInformation
        myInObject.SetValue("", 2) ' Filter
        myInObject.SetValue(0, 3) ' Start Index
        myInObject.SetValue(0, 4) ' Max result count
        myInObject.SetValue("", 5) ' sort Criteria

        Try
            Dim myResponse As Object = ContentDirectory.InvokeAction("Browse", myInObject, myOutObject)
            'ContentDirectory.
            Debug.WriteLine(myResponse.ToString)
        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try
        If myOutObject(0) Is Nothing Then Return Nothing
        Dim myXMLDoc As New XmlDocument()

        Try
            myXMLDoc.LoadXml(CStr(myOutObject(0)))
        Catch ex As Exception
            Try
                myXMLDoc.LoadXml(CStr(myOutObject(0)).Replace("&", "&amp;"))
            Catch ex2 As Exception
                Throw New Exception(ex.ToString, ex2)
            End Try
        End Try

        For Each xmlNode As XmlNode In myXMLDoc.DocumentElement.ChildNodes
            If myObjects Is Nothing Then
                ReDim Preserve myObjects(0) 'first time we use the myObjects array, it is undefined, so we make it the size of 1 element
            Else
                ReDim Preserve myObjects(myObjects.Length) 'each time we increase the size of the array and remember the elements that were already in it
            End If
            myObjects(myObjects.Length - 1) = CD_Object.Create_CD_Object(xmlNode)
            'Debug.WriteLine("Create Object:" & myObjects(myObjects.Length - 1).ClassName & " " & myObjects(myObjects.Length - 1).ID)
        Next
        'Debug.WriteLine("BrowseContainer: Stop")
        Return myObjects
    End Function
    Private Function BrowseContainer(ByRef parentContainer As CD_Container) As CD_Object()
        Return BrowseContainer(parentContainer.ID)
    End Function
    Private Sub ContentBrowser_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles DLNAtreeview.AfterSelect
        LoadContainerChildren(e.Node)
    End Sub
    Private Sub ContentBrowser_BeforeExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles DLNAtreeview.BeforeExpand
        LoadContainerChildren(e.Node)
    End Sub
    Private Sub AddNodeToPlayList(ByRef ParentNode As Forms.TreeNode, Optional ByVal LoadAsSeperate As Boolean = False)
        If TypeOf ParentNode.Tag Is CD_Container And LoadAsSeperate = True Then
            If ParentNode.Nodes.Count > 0 Then
                If ParentNode.Nodes.Count = 1 And ParentNode.Nodes(0).Text = "Dummy" Then
                    LoadContainerChildren(ParentNode)
                End If
                For Each tmpChild As Forms.TreeNode In ParentNode.Nodes
                    AddNodeToPlayList(tmpChild, LoadAsSeperate)
                Next
            End If
        ElseIf TypeOf ParentNode.Tag Is CD_Object Then
            Dim myObject As CD_Object = GetCDObjectInformation(DirectCast(ParentNode.Tag, CD_Object).ID)
            If myObject.Resource Is Nothing Then Exit Sub
            Dim myLVItem As New Forms.ListViewItem(myObject.Title)
            myLVItem.Tag = myObject

            If TypeOf myObject Is CD_Item Then
                myLVItem.ImageKey = "item"
                Dim myItem As CD_Item = DirectCast(myObject, CD_Item)
                myLVItem.SubItems.Add(myItem.Artist)
                myLVItem.SubItems.Add(myItem.Album)
                myLVItem.SubItems.Add(myItem.Genre)
            Else
                myLVItem.ImageKey = "container"
                myLVItem.SubItems.Add("")
                myLVItem.SubItems.Add("")
                myLVItem.SubItems.Add("")
            End If

            Try
                myLVItem.SubItems.Add(myObject.Resource(0).ToString)
            Catch ex As Exception
                Debug.WriteLine(ex.ToString)
            End Try
        End If
    End Sub
    Public Sub SendToPlayer(Player As Player, ParentNode As Forms.TreeNode)
        If ParentNode Is Nothing Then
            Exit Sub
        End If
        If TypeOf ParentNode.Tag Is CD_Object Then
            Dim myObject As CD_Object = GetCDObjectInformation(DirectCast(ParentNode.Tag, CD_Object).ID)
            If myObject.Resource Is Nothing Then Exit Sub
            If TypeOf myObject Is CD_Item Then
                Dim myItem As CD_Item = DirectCast(myObject, CD_Item)
                Dim Artist As String = myItem.Artist
                Dim Album As String = myItem.Album
                Dim Title As String = myItem.Title
                Dim Genre As String = myItem.Genre
                Dim Duration = myItem.Resource(0).duration
                Dim Url = myItem.Resource(0).URI.ToString
                Dim songlist As New List(Of String())
                Dim XMLDC As New XmlDocument
                XMLDC.LoadXml(myItem.XMLDump)
                Dim Cover As System.Drawing.Image = Nothing
                Dim Info As String() = {"", "", "", "", "", "", "", ""} '0 title1 class,2 ALbum,3 genre,4 artist,5 uri,6 cover uri
                For Each nn As XmlNode In XMLDC.ChildNodes(0)
                    Select Case nn.Name
                        Case "dc:title"
                            Info(0) = nn.InnerText
                        Case "upnp:album"
                            Info(2) = nn.InnerText
                        Case "upnp:genre"
                            Info(3) = nn.InnerText
                        Case "upnp:artist"
                            Info(4) = nn.InnerText
                        Case "upnp:class"
                            Info(1) = nn.InnerText
                        Case "res"
                            Dim SngInfo As String()
                            For i As Integer = 0 To nn.Attributes.Count - 1
                                ReDim Preserve SngInfo(i)
                                Try
                                    SngInfo(i) = nn.Attributes(i).Name & "//" & nn.Attributes(i).InnerText
                                Catch ex As Exception
                                End Try
                                If i = nn.Attributes.Count - 1 Then
                                    SngInfo(i) = nn.InnerText
                                    If nn.Attributes("protocolInfo").InnerText.Contains("audio/") Then
                                        Info(5) = nn.InnerText
                                    End If
                                    If Cover Is Nothing AndAlso nn.Attributes("protocolInfo").InnerText.Contains("image/") Then
                                        Info(6) = nn.InnerText
                                    End If
                                End If
                            Next
                            songlist.Add(SngInfo)
                    End Select
                Next
                For Each n As XmlElement In XMLDC.ChildNodes
                    Try
                        Using WC As New WebClient
                            Cover = System.Drawing.Image.FromStream(New MemoryStream(WC.DownloadData((Info(6)))))
                        End Using
                    Catch ex As Exception
                        Console.WriteLine(TimeOfDay.ToShortTimeString & ": " & "Error on getting Album Cover from UPNP" & ex.Message)
                    End Try
                Next
                '0 title1 class,2 ALbum,3 genre,4 artist,5 uri,6 cover uri
                CType(Application.Current.MainWindow, MainWindow).UpdatePlaylist = False
                Player.LoadSong(Nothing, CType(Application.Current.MainWindow, MainWindow).MainPlaylist, True, True, True, Info(5), True, Info(0), Info(4), Cover, 0, Nothing)
                Player.StreamPlay()
            Else
                Exit Sub
            End If
        End If
    End Sub
    ''' <summary>
    ''' 0.title ,1.artist ,2.url
    ''' </summary>
    ''' <param name="ParentNode"></param>
    ''' <returns></returns>
    Public Function GetSelFileURL(ParentNode As Forms.TreeNode) As String()
        If ParentNode Is Nothing Then
            Return Nothing
        End If
        If TypeOf ParentNode.Tag Is CD_Object Then
            Dim myObject As CD_Object = GetCDObjectInformation(DirectCast(ParentNode.Tag, CD_Object).ID)
            If myObject.Resource Is Nothing Then Return Nothing
            If TypeOf myObject Is CD_Item Then
                Dim myItem As CD_Item = DirectCast(myObject, CD_Item)
                Dim Artist As String = myItem.Artist
                Dim Album As String = myItem.Album
                Dim Title As String = myItem.Title
                Dim Genre As String = myItem.Genre
                Dim Duration = myItem.Resource(0).duration
                Dim Url = myItem.Resource(0).URI.ToString
                Dim songlist As New List(Of String())
                Dim XMLDC As New XmlDocument
                XMLDC.LoadXml(myItem.XMLDump)
                Dim Cover As System.Drawing.Image = Nothing
                Dim Info As String() = {"", "", "", "", "", "", "", ""} '0 title1 class,2 ALbum,3 genre,4 artist,5 uri,6 cover uri
                For Each nn As XmlNode In XMLDC.ChildNodes(0)
                    Select Case nn.Name
                        Case "dc:title"
                            Info(0) = nn.InnerText
                        Case "upnp:album"
                            Info(2) = nn.InnerText
                        Case "upnp:genre"
                            Info(3) = nn.InnerText
                        Case "upnp:artist"
                            Info(4) = nn.InnerText
                        Case "upnp:class"
                            Info(1) = nn.InnerText
                        Case "res"
                            Dim SngInfo As String()
                            For i As Integer = 0 To nn.Attributes.Count - 1
                                ReDim Preserve SngInfo(i)
                                Try
                                    SngInfo(i) = nn.Attributes(i).Name & "//" & nn.Attributes(i).InnerText
                                Catch ex As Exception
                                End Try
                                If i = nn.Attributes.Count - 1 Then
                                    SngInfo(i) = nn.InnerText
                                    If nn.Attributes("protocolInfo").InnerText.Contains("audio/") Then
                                        Info(5) = nn.InnerText
                                    End If
                                    If Cover Is Nothing AndAlso nn.Attributes("protocolInfo").InnerText.Contains("image/") Then
                                        Info(6) = nn.InnerText
                                    End If
                                End If
                            Next
                            songlist.Add(SngInfo)
                    End Select
                Next
                Return New String() {Info(0), Info(4), Info(5)}
            Else
                Return Nothing
            End If
        End If
    End Function
    Private Sub DLNAtreeview_DoubleClick(sender As Object, e As EventArgs) Handles DLNAtreeview.DoubleClick
        ' AddNodeToPlayList(DLNAtreeview.SelectedNode, False)
        SendToPlayer(CType(Application.Current.MainWindow, MainWindow).MainPlayer, DLNAtreeview.SelectedNode)
    End Sub
    Private Sub MediaRendererList_SelectedIndexChanged(sender As Object, e As SelectionChangedEventArgs) Handles MediaRendererList.SelectionChanged
        If MediaRendererList.SelectedIndex >= 0 Then
            myMediaRenderer = New UPnPMediaRenderer_v1(DirectCast(MediaRendererList.SelectedItem, UPnPDeviceListItemWrapper).UPnPDevice)
        End If
    End Sub
    Private Sub playto_btn_Click(sender As Object, e As EventArgs) Handles playto_btn.Click
        If myMediaRenderer IsNot Nothing Then
            'If AVTransport Is Nothing Then Exit Sub
            'If lvPlaylist.SelectedItems.Count = 1 Then
            'Building cd_object
            Dim ParentNode = DLNAtreeview.SelectedNode
            Dim myObject As CD_Object
            If TypeOf ParentNode.Tag Is CD_Object Then
                myObject = GetCDObjectInformation(DirectCast(ParentNode.Tag, CD_Object).ID)
                If myObject.Resource Is Nothing Then Exit Sub
                Dim myLVItem As New Forms.ListViewItem(myObject.Title)

                myLVItem.Tag = myObject
                If TypeOf myObject Is CD_Item Then
                    myLVItem.ImageKey = "item"
                    Dim myItem As CD_Item = DirectCast(myObject, CD_Item)
                    myLVItem.SubItems.Add(myItem.Artist)
                    myLVItem.SubItems.Add(myItem.Album)
                    myLVItem.SubItems.Add(myItem.Genre)
                Else
                    myLVItem.ImageKey = "container"
                    myLVItem.SubItems.Add("")
                    myLVItem.SubItems.Add("")
                    myLVItem.SubItems.Add("")
                End If

                Try
                    myLVItem.SubItems.Add(myObject.Resource(0).ToString)
                Catch ex As Exception
                    Debug.WriteLine(ex.ToString)
                End Try
            End If
            'end of build cd object
            myMediaRenderer.AVTransport.SetAVTransportURI(CUInt(0), myObject.Resource(0).URI.ToString, myObject.XMLDump)
            myMediaRenderer.AVTransport.Play()
            If TypeOf DLNAtreeview.SelectedNode.Tag Is CD_Item Then
                Dim myItem As CD_Item = DirectCast(DLNAtreeview.SelectedNode.Tag, CD_Item)
            End If

        Else
        End If
    End Sub
#End Region
End Class
