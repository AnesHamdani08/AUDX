Imports UPNPLib

Namespace UPnP_AV
    Public Class UPnPMediaRenderer_v1
        Public MediaRendererDevice As UPnPDevice = Nothing

        Public AVTransport As AVTransportService = Nothing
        Public RendererControl As RendererControlService = Nothing

        Public Sub New(ByVal MediaRendererDevice As UPnPDevice)
            Me.MediaRendererDevice = MediaRendererDevice

            For Each myService As UPnPService In MediaRendererDevice.Services
                If myService.ServiceTypeIdentifier = "urn:schemas-upnp-org:service:AVTransport:1" Then
                    AVTransport = New AVTransportService(myService)
                    Exit For 'end the for-loop
                End If
            Next
        End Sub

        'ServiceTypeIdentifier = urn:schemas-upnp-org:service:AVTransport:1
        Public Class AVTransportService
            Public Structure PositionInfo
                Public Track As UInt32
                Public TrackDuration As String
                Public TrackMetaData As String
                Public TrackURI As String
                Public RelativeTime As String
                Public AbsoluteTime As String
                Public RelativeCount As Integer
                Public AbsoluteCount As Integer
            End Structure

            Public Enum TransportStatusEnum
                ERROR_OCCURRED
                STOPPED
                OK
            End Enum

            Public Enum TransportStateEnum
                STOPPED 'always supported
                PLAYING 'always supported
                TRANSITIONING
                PAUSED_PLAYBACK
                PAUSED_RECORDING
                RECORDING
                NO_MEDIA_PRESENT
            End Enum

            Public Structure TransportInfo
                Public CurrentTransportState As TransportStateEnum
                Public CurrentTransportStatus As TransportStatusEnum
                Public CurrentSpeed As String
            End Structure

            Dim AVTransportService As UPnPService = Nothing

            Public Sub New(ByRef AVTransportService As UPnPService)
                Me.AVTransportService = AVTransportService
            End Sub

            Public Sub SetAVTransportURI(ByVal InstanceID As UInt32, ByVal CurrentURI As String, ByVal CurrentURIMetaData As String)
                If AVTransportService Is Nothing Then Exit Sub

                Dim inObject(2) As Object
                Dim outObject As Object = Nothing

                inObject.SetValue(0, 0) 'Instance ID
                inObject.SetValue(CurrentURI, 1) 'CurrentURI
                inObject.SetValue(CurrentURIMetaData, 2) 'CurrentURIMetadata

                Try
                    Me.AVTransportService.InvokeAction("SetAVTransportURI", inObject, outObject)
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:SetAVTransportURI :" & ex.ToString)
                End Try
            End Sub

            Public Function GetPositionInfo(ByVal InstanceID As UInt32) As PositionInfo
                If AVTransportService Is Nothing Then Return Nothing

                Dim tmpPosInfo As New PositionInfo
                Dim inObject(0) As Object
                Dim outObject(7) As Object

                inObject.SetValue(InstanceID, 0) 'Instance ID

                Try
                    Me.AVTransportService.InvokeAction("GetPositionInfo", inObject, outObject)
                    tmpPosInfo.Track = CUInt(outObject(0))
                    tmpPosInfo.TrackDuration = CStr(outObject(1))
                    tmpPosInfo.TrackMetaData = CStr(outObject(2))
                    tmpPosInfo.TrackURI = CStr(outObject(3))
                    tmpPosInfo.RelativeTime = CStr(outObject(4))
                    tmpPosInfo.AbsoluteTime = CStr(outObject(5))
                    tmpPosInfo.RelativeCount = CInt(outObject(6))
                    tmpPosInfo.AbsoluteCount = CInt(outObject(7))
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:GetPositionInfo :" & ex.ToString)
                End Try

                Return tmpPosInfo
            End Function

            Public Function GetTransportInfo(ByVal InstanceID As UInt32) As TransportInfo
                If AVTransportService Is Nothing Then Return Nothing

                Dim tmpTransInfo As New TransportInfo
                Dim inObject(0) As Object
                Dim outObject(2) As Object

                inObject.SetValue(InstanceID, 0) 'Instance ID

                Try
                    Me.AVTransportService.InvokeAction("GetTransportInfo", inObject, outObject)

                    tmpTransInfo.CurrentTransportState = [Enum].Parse(GetType(TransportStateEnum), CStr(outObject(0)).ToUpper)
                    tmpTransInfo.CurrentTransportStatus = [Enum].Parse(GetType(TransportStatusEnum), CStr(outObject(1)).ToUpper)
                    tmpTransInfo.CurrentSpeed = CStr(outObject(2))
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:GetTransportInfo :" & ex.ToString)
                End Try

                Return tmpTransInfo
            End Function

            Public Sub [Stop](ByVal InstanceID As UInt32)
                If AVTransportService Is Nothing Then Exit Sub

                Dim inObject(0) As Object
                Dim outObject As Object = Nothing

                inObject.SetValue(InstanceID, 0) 'Instance ID

                Try
                    Me.AVTransportService.InvokeAction("Stop", inObject, outObject)
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:Stop :" & ex.ToString)
                End Try
            End Sub

            Public Sub [Next](ByVal InstanceID As UInt32)
                If AVTransportService Is Nothing Then Exit Sub

                Dim inObject(0) As Object
                Dim outObject As Object = Nothing

                inObject.SetValue(InstanceID, 0) 'Instance ID

                Try
                    Me.AVTransportService.InvokeAction("Next", inObject, outObject)
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:Next :" & ex.ToString)
                End Try
            End Sub

            Public Sub [Previous](ByVal InstanceID As UInt32)
                If AVTransportService Is Nothing Then Exit Sub

                Dim inObject(0) As Object
                Dim outObject As Object = Nothing

                inObject.SetValue(InstanceID, 0) 'Instance ID

                Try
                    Me.AVTransportService.InvokeAction("Previous", inObject, outObject)
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:Previous :" & ex.ToString)
                End Try
            End Sub

            ' ''<summary>
            ' ''Starts playing the CurrentURI
            ' ''</summary>
            ' ''<value>Age of the claimant.</value>
            ' ''<param name="InstanceID">InstanceID, 0 by default</param>
            ' ''<param name="TransportPlaySpeed">String representing the playbackspeed. "1" is normal playback (always supported), "1/10" is slow playback (not always supported, check allowed values), "-1" is reverse playback (not always supported, check allowed values),...</param>
            Public Sub Play(ByVal InstanceID As UInt32, ByVal TransportPlaySpeed As String)
                If AVTransportService Is Nothing Then Exit Sub

                Dim inObject(1) As Object
                Dim outObject As Object = Nothing

                inObject.SetValue(InstanceID, 0) 'Instance ID
                inObject.SetValue(TransportPlaySpeed, 1) 'TransportPlaySpeed: "1", "1/10", "-1"

                Try
                    Me.AVTransportService.InvokeAction("Play", inObject, outObject)
                Catch ex As Exception
                    Debug.WriteLine("Error in AVTransportService:Play :" & ex.ToString)
                End Try
            End Sub

            

            'sub for convenience
            Public Sub Play()
                Me.Play(CUInt(0), "1")
            End Sub

            'sub for convenience
            Public Sub Play(ByVal urlToPlay As String)
                Me.SetAVTransportURI(urlToPlay)
                Me.Play(CUInt(0), "1")
            End Sub

            'sub for convenience
            Public Sub SetAVTransportURI(ByVal CurrentURI As String)
                Me.SetAVTransportURI(CUInt(0), CurrentURI, "")
            End Sub

            'sub for convenience
            Public Sub [Stop]()
                Me.Stop(CUInt(0))
            End Sub

            'sub for convenience
            Public Sub [Next]()
                Me.Next(CUInt(0))
            End Sub

            'sub for convenience
            Public Sub [Previous]()
                Me.Previous(CUInt(0))
            End Sub

            'sub for convenience
            Public Function GetTransportInfo() As TransportInfo
                Return Me.GetTransportInfo(CUInt(0))
            End Function

            'sub for convenience
            Public Function GetPositionInfo() As PositionInfo
                Return GetPositionInfo(CUInt(0))
            End Function

            'TransportPlaySpeed Info
            '-----------------------
            'String representation of a rational fraction, indicates the speed relative to normal speed. 
            'Example values are ‘1’, ‘1/2’, ‘2’, ‘-1’, ‘1/10’, etc. 
            'Actually supported speeds can be retrieved from the AllowedValueList of this state variable in the AVTransport service description. 
            'Value ‘1’ is required, value ‘0’ is not allowed.
            Public ReadOnly Property TransportPlaySpeed() As String
                Get
                    Return Me.AVTransportService.QueryStateVariable("TransportPlaySpeed")
                End Get
            End Property

            Public ReadOnly Property TransportState() As TransportStateEnum
                Get
                    Try
                        Return [Enum].Parse(GetType(TransportStateEnum), CStr(Me.AVTransportService.QueryStateVariable("TransportState")))

                    Catch ex As Exception
                        Return TransportStateEnum.PLAYING
                    End Try
                    End Get
            End Property

            Public ReadOnly Property TransportStatus() As TransportStatusEnum
                Get
                    Return [Enum].Parse(GetType(TransportStatusEnum), CStr(Me.AVTransportService.QueryStateVariable("TransportStatus")))
                End Get
            End Property

            Public ReadOnly Property CurrentPlayMode() As String
                Get
                    Return Me.AVTransportService.QueryStateVariable("CurrentPlayMode")
                End Get
            End Property

            Public ReadOnly Property RelativeTimePosition() As String
                Get
                    Return CStr(Me.AVTransportService.QueryStateVariable("RelativeTimePosition"))
                End Get
            End Property
        End Class

        'ServiceID = urn:upnp-org:serviceId:RenderingControlServiceID
        Public Class RendererControlService
            Dim RendererControlService As UPnPService = Nothing

            Public Sub New(ByRef RendererControlService As UPnPService)
                Me.RendererControlService = RendererControlService
            End Sub
        End Class
    End Class
End Namespace
