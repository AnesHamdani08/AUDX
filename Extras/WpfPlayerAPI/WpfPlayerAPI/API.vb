Imports System.Threading
Imports System.Drawing
Imports System.ComponentModel

Namespace Anes08.MuPlay
    ''' <summary>
    ''' MuPlay integration module , allows you to get information about the current player state and also set some player properties.
    ''' Copyright Anes08 ©2021 All rights reserved , MuPlay API is open source and availble at GitHub.
    ''' </summary>
    Public Class API
#Region "Sub Classes"
        Public Sub RemoveAt(Of T)(ByRef arr As T(), ByVal index As Integer)
            Dim uBound = arr.GetUpperBound(0)
            Dim lBound = arr.GetLowerBound(0)
            Dim arrLen = uBound - lBound

            If index < lBound OrElse index > uBound Then
                Throw New ArgumentOutOfRangeException(
        String.Format("Index must be from {0} to {1}.", lBound, uBound))

            Else
                'create an array 1 element less than the input array
                Dim outArr(arrLen - 1) As T
                'copy the first part of the input array
                Array.Copy(arr, 0, outArr, 0, index)
                'then copy the second part of the input array
                Array.Copy(arr, index + 1, outArr, index, uBound - index)

                arr = outArr
            End If
        End Sub
        Public Enum MediaType
            Local
            Youtube
            URL
            SoundCloud
        End Enum
        Public Enum RepeatBehaviour
            NoRepeat = 0
            RepeatOne = 1
            RepeatAll = 2
            Shuffle = 3
            NoShuffle = 4
        End Enum
        Public Enum State
            Undefined = 0
            MediaLoaded = 1
            Playing = 2
            Paused = 3
            Stopped = 4
            _Error = 5
        End Enum
        Public Class MediaItem
            Public Property Title As String
            Public Property Artist As String
            Public Property Cover As Bitmap
            Public Property Source As String
            Public Property Type As MediaType
            Public Sub New(_Title As String, _Artist As String, _Cover As Bitmap, _Source As String, _Type As MediaType)
                Title = _Title
                Artist = _Artist
                Cover = _Cover
                Source = _Source
                Type = _Type
            End Sub
            Public Function Format() As String
                Return Title & vbCrLf & Artist & vbCrLf & Source & vbCrLf & "[" & Type & "]" & vbCrLf & Type.ToString
            End Function
        End Class
        Public Class MediaTag
            Public Property Artist As String
            Public Property Title As String
            Public Property Album As String
            Public Property Year As String
            Public Property TrackNum As Integer
            Public Property Genres As String
            Public Property Lyrics As String
            Public Property Bitrates As Integer
            Public Sub New(_Artist As String, _Title As String, _Album As String, _Year As String, _TrackNum As String, _Genres As String, _Lyrics As String, _Bitrates As String)
                Artist = _Artist
                Title = _Title
                Album = _Album
                Year = _Year
                TrackNum = _TrackNum
                Genres = _Genres
                Lyrics = _Lyrics
                Bitrates = _Bitrates
            End Sub
            Public Overrides Function ToString() As String
                Return "Title: " & Title & vbCrLf & "Artist: " & Artist & vbCrLf & "Album: " & Album & vbCrLf & "Year: " & Year & vbCrLf & "Track Number: " & TrackNum & vbCrLf & "Genres: " & Genres & vbCrLf & "Lyrics: " & Lyrics & vbCrLf & "Bitrates: " & Bitrates
            End Function
        End Class
        Public Class PeakItem
            Public Property Left As Single
            Public Property Right As Single
            Public Property Master As Single
            Public Sub New(L As Single, R As Single, M As Single)
                Left = L
                Right = R
                Master = M
            End Sub
        End Class
        Public Shared Function SecsToMins(sec As Double, Optional LongText As Boolean = False) As String
            Try
                If LongText = False Then
                    Dim Pos = sec / 60
                    Dim Mins As String
                    Try
                        Mins = Pos.ToString.Substring(0, Pos.ToString.IndexOf("."))
                    Catch ex As Exception
                        Mins = Pos.ToString
                    End Try
                    Dim secs As String
                    Try
                        secs = Pos.ToString.Substring(Pos.ToString.IndexOf("."), Pos.ToString.Length - Pos.ToString.IndexOf("."))
                    Catch ex As Exception
                        secs = 0
                    End Try
                    Dim ConvSecs = secs * 60
                    If Mins >= 10 AndAlso Math.Round(ConvSecs) >= 10 Then
                        Return New String(Mins & ":" & Math.Round(ConvSecs))
                    ElseIf Mins < 10 AndAlso Math.Round(ConvSecs) >= 10 Then
                        Return New String("0" & Mins & ":" & Math.Round(ConvSecs))
                    ElseIf Mins >= 10 AndAlso Math.Round(ConvSecs) < 10 Then
                        Return New String(Mins & ":" & "0" & Math.Round(ConvSecs))
                    ElseIf Mins < 10 Or Mins = 0 AndAlso Math.Round(ConvSecs) < 10 Then
                        Return New String("0" & Mins & ":" & "0" & Math.Round(ConvSecs))
                    ElseIf Mins < 10 AndAlso Math.Round(ConvSecs) > 59 Then
                        Return New String("0" & Mins & ":" & "00")
                    ElseIf Mins > 10 AndAlso Math.Round(ConvSecs) > 59 Then
                        Return New String(Mins & ":" & "00")
                    End If
                ElseIf LongText = True Then
                    Dim DeciMins = sec / 60
                    Dim IntMins = DeciMins.ToString.Substring(0, DeciMins.ToString.IndexOf("."))
                    Dim DeciSecs = DeciMins.ToString.Substring(DeciMins.ToString.IndexOf("."), DeciMins.ToString.Length - DeciMins.ToString.IndexOf("."))
                    Dim IntSecs = DeciSecs * 60
                    If IntSecs < 10 Then
                        Return New String(IntMins & " min 0" & Math.Round(IntSecs) & " sec")
                    Else
                        Return New String(IntMins & " min " & Math.Round(IntSecs) & " sec")
                    End If
                End If
            Catch ex As Exception
                Return "ERROR"
            End Try
        End Function
        Public Event MediaLoaded()
        Public Event RepeatChanged(NewType As RepeatBehaviour)
        Public Event ShuffleChanged(NewType As RepeatBehaviour)
        Public Event StateChanged(State As State)
        Public Event VolumeChanged(NewVal As Single, IsMuted As Boolean)
#End Region
        Private WithEvents SNamedPipeServer As New NamedPipeManager("MuPlayAPI_MuPlay") 'Send only , running from MuPlay
        Private WithEvents RNamedPipeServer As New NamedPipeManager("MuPlayAPI_API") 'Recieve only , running from here
        Private Spacer As String = ">"
        Public ReadOnly Property IsMuPlayAvailable As Boolean
            Get
                Dim MuPlay = Process.GetProcessesByName("WpfPlayer").FirstOrDefault(Function(k) k.ProcessName = "WpfPlayer")
                If MuPlay IsNot Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public Sub New()
            RNamedPipeServer.StartServer()
        End Sub
        Public Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                RNamedPipeServer.StopServer()
            End If
            RNamedPipeServer.StopServer()
            ' TODO: free unmanaged resources (unmanaged objects).
        End Sub
        Private Sub NamedPipeServer_ReceiveString(obj As String) Handles RNamedPipeServer.ReceiveString
            Dim CMDARGS = obj.Split(Spacer)
            'Dim Rargs As New List(Of String)
            'Dim Args = CMDARGS
            'For Each arg In Args
            '    If Not String.IsNullOrEmpty(arg) AndAlso Not String.IsNullOrWhiteSpace(arg) Then
            '        Rargs.Add(arg)
            '    End If
            'Next
            Dim ID = CMDARGS(0)
            RemoveAt(CMDARGS, 0)
            'Rargs.RemoveAt(0)
            HandleMsg(ID, CMDARGS)
        End Sub
        Private Sub HandleMsg(ID As CMDType, Msg As String())
            Dim Info = Msg
            Select Case ID
                Case CMDType.GetCurrent 'Get current                
                    Select Case Info(3).ToLower 'Type
                        Case 0 'URL
                            SGetCurrentIT = New MediaItem(Info(0), Info(1), Nothing, Info(2), MediaType.URL)
                            SGetCurrentMRE.Set()
                        Case 1 'YT
                            SGetCurrentIT = New MediaItem(Info(0), Info(1), Nothing, Info(2), MediaType.Youtube)
                            SGetCurrentMRE.Set()
                        Case 2 'SC
                            SGetCurrentIT = New MediaItem(Info(0), Info(1), Nothing, Info(2), MediaType.SoundCloud)
                            SGetCurrentMRE.Set()
                        Case 3 'Local
                            SGetCurrentIT = New MediaItem(Info(0), Info(1), Nothing, Info(2), MediaType.Local)
                            SGetCurrentMRE.Set()
                    End Select
                    SGetCurrentMRE.Set()
                Case CMDType.GetCover 'Get cover
                    Dim MEMCover As New IO.MemoryStream(Convert.FromBase64String(Info(0)))
                    Dim Cover = Bitmap.FromStream(MEMCover)
                    SGetCoverIT = Cover
                    SGetCoverMRE.Set()
                Case CMDType.GetRepeatShuffleType 'Get Repeat Type
                    Dim RepeatType As RepeatBehaviour = Info(0)
                    Dim ShuffleType As RepeatBehaviour = Info(1)
                    Dim CombinedType As RepeatBehaviour() = {RepeatType, ShuffleType}
                    SGetRepeatShuffleTypeIT = CombinedType
                    SGetRepeatShuffleTypeMRE.Set()
                Case CMDType.GetIsMono
                    If Msg(0) = 0 Then
                        SGetMonoIT = False
                    ElseIf Msg(0) = 1 Then
                        SGetMonoIT = True
                    End If
                    SGetMonoMRE.Set()
                Case CMDType.GetVolume
                    SGetVolumeIT = Msg(0) * 100
                    SGetVolumeMRE.Set()
                Case CMDType.GetPosition
                    SGetPositionIT = Msg(0)
                    SGetPositionMRE.Set()
                Case CMDType.GetLength
                    SGetLengthIT = Msg(0)
                    SGetLengthMRE.Set()
                Case CMDType.GetTags
                    SGetTagsIT = New MediaTag(Info(0), Info(1), Info(2), Info(3), Info(4), Info(5), Info(6), Info(7))
                    SGetTagsMRE.Set()
                Case CMDType.GetPeak
                    SGetPeakIT = New PeakItem(Info(0), Info(1), Info(2))
                    SGetPeakMRE.Set()
                Case CMDType.EventOnMediaLoaded
                    RaiseEvent MediaLoaded()
                Case CMDType.EventOnRepeatChanged
                    RaiseEvent RepeatChanged(Info(0))
                Case CMDType.EventOnShuffleChanged
                    RaiseEvent ShuffleChanged(Info(0))
                Case CMDType.EventOnStateChanged
                    RaiseEvent StateChanged(Info(0))
                Case CMDType.EventOnVolumeChanged
                    RaiseEvent VolumeChanged(Info(0), Info(1))
            End Select
        End Sub
        Public Enum CMDType
            GetCurrent = 0
            GetCover = 1
            GetRepeatShuffleType = 2
            GetIsMono = 3
            GetVolume = 4
            GetPosition = 5
            GetLength = 6
            GetTags = 7
            GetPeak = 8
            SetPlay = 1000
            SetPause = 1001
            SetNext = 1002
            SetPrevious = 1003
            SetPosition = 1004
            SetVolume = 1005
            EventOnMediaLoaded = 2000
            EventOnRepeatChanged = 2001
            EventOnShuffleChanged = 2002
            EventOnStateChanged = 2003
            EventOnVolumeChanged = 2004
        End Enum
#Region "GET/SET"
        Public Sub SPlay()
            SNamedPipeServer.Write(CMDType.SetPlay)
        End Sub
        Public Sub SPause()
            SNamedPipeServer.Write(CMDType.SetPause)
        End Sub
        Public Sub SNext()
            SNamedPipeServer.Write(CMDType.SetNext)
        End Sub
        Public Sub SPrevious()
            SNamedPipeServer.Write(CMDType.SetPrevious)
        End Sub
        Public Sub SVolume(Vol As Integer)
            SNamedPipeServer.Write(CMDType.SetVolume & Spacer & Vol)
        End Sub
        Public Sub SPosition(Pos As Double)
            SNamedPipeServer.Write(CMDType.SetPosition & Spacer & Pos)
        End Sub
        Dim SGetCurrentMRE As New ManualResetEvent(False)
        Dim SGetCurrentIT As MediaItem
        Public Async Function GetCurrent() As Task(Of MediaItem)
            SNamedPipeServer.Write(CMDType.GetCurrent)
            Await Task.Run(Sub()
                               SGetCurrentMRE.WaitOne()
                               SGetCurrentMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetCurrentIT)
        End Function
        Dim SGetCoverMRE As New ManualResetEvent(False)
        Dim SGetCoverIT As Bitmap
        Public Async Function GetCover() As Task(Of Bitmap)
            SNamedPipeServer.Write(CMDType.GetCover)
            Await Task.Run(Sub()
                               SGetCoverMRE.WaitOne()
                               SGetCoverMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetCoverIT)
        End Function
        Dim SGetRepeatShuffleTypeMRE As New ManualResetEvent(False)
        Dim SGetRepeatShuffleTypeIT As RepeatBehaviour()
        Public Async Function GetRepeatShuffleType() As Task(Of RepeatBehaviour())
            SNamedPipeServer.Write(CMDType.GetRepeatShuffleType)
            Await Task.Run(Sub()
                               SGetRepeatShuffleTypeMRE.WaitOne()
                               SGetRepeatShuffleTypeMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetRepeatShuffleTypeIT)
        End Function
        Dim SGetMonoMRE As New ManualResetEvent(False)
        Dim SGetMonoIT As Boolean
        Public Async Function GetMono() As Task(Of Boolean)
            SNamedPipeServer.Write(CMDType.GetIsMono)
            Await Task.Run(Sub()
                               SGetMonoMRE.WaitOne()
                               SGetMonoMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetMonoIT)
        End Function
        Dim SGetVolumeMRE As New ManualResetEvent(False)
        Dim SGetVolumeIT As Integer
        Public Async Function GetVolume() As Task(Of Integer)
            SNamedPipeServer.Write(CMDType.GetVolume)
            Await Task.Run(Sub()
                               SGetVolumeMRE.WaitOne()
                               SGetVolumeMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetVolumeIT)
        End Function
        Dim SGetPositionMRE As New ManualResetEvent(False)
        Dim SGetPositionIT As Integer
        Public Async Function GetPosition() As Task(Of Long)
            SNamedPipeServer.Write(CMDType.GetPosition)
            Await Task.Run(Sub()
                               SGetPositionMRE.WaitOne()
                               SGetPositionMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetPositionIT)
        End Function
        Dim SGetLengthMRE As New ManualResetEvent(False)
        Dim SGetLengthIT As Integer
        Public Async Function GetLength() As Task(Of Long)
            SNamedPipeServer.Write(CMDType.GetLength)
            Await Task.Run(Sub()
                               SGetLengthMRE.WaitOne()
                               SGetLengthMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetLengthIT)
        End Function
        Dim SGetTagsMRE As New ManualResetEvent(False)
        Dim SGetTagsIT As MediaTag
        Public Async Function GetTags() As Task(Of MediaTag)
            SNamedPipeServer.Write(CMDType.GetTags)
            Await Task.Run(Sub()
                               SGetTagsMRE.WaitOne()
                               SGetTagsMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetTagsIT)
        End Function
        Dim SGetPeakMRE As New ManualResetEvent(False)
        Dim SGetPeakIT As PeakItem
        Public Async Function GetPeak() As Task(Of PeakItem)
            SNamedPipeServer.Write(CMDType.GetPeak)
            Await Task.Run(Sub()
                               SGetPeakMRE.WaitOne()
                               SGetPeakMRE.Reset()
                           End Sub)
            Return Await Task.FromResult(SGetPeakIT)
        End Function
#End Region
    End Class
End Namespace