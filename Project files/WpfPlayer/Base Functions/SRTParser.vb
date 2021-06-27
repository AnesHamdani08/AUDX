Public Class SRTParser
#Region "Stuff"
    Public Class SRTItem
        Public Property Num As Integer
        Public Property Text As String
        Public Property StartTime As TimeSpan
        Public Property EndTime As TimeSpan
        Public Shadows Function ToString() As String
            Return Num & Environment.NewLine & "Start: " & StartTime.ToString & Environment.NewLine & "End: " & EndTime.ToString & Environment.NewLine & "Text: " & Text
        End Function
    End Class
    Public PushSrtCallback As Action(Of SRTItem)
    Public PushSrtCancellationToken As Boolean = False
    Private _PushSrtCurrent As SRTItem = Nothing
    Public Property PushSrtCurrent As SRTItem
        Get
            Return _PushSrtCurrent
        End Get
        Set(value As SRTItem)
            _PushSrtCurrent = value
            RaiseEvent OnPushSrtCurrentChanged(value)
        End Set
    End Property
    Private PushSrtSyncWatch As New Stopwatch
    Public IsPushSrt As Boolean = False
    Public Property Subtitles As New List(Of SRTItem)
    Public Property Source As String
    Public Event OnSRTLoaded()
    Public Event OnPushStarted()
    Public Event OnPushFinished()
    Public Event OnPushSrtCurrentChanged(CurrentSrt As SRTItem)
    Public Event OnSRTDisposed()
#End Region
    Public Sub Load(SRT As String)
        If IO.File.Exists(SRT) Then
            Source = SRT
            'Let's begin the work
            Dim Text = IO.File.ReadAllLines(SRT) 'First we read all the lines
            For i As Integer = 0 To Text.Length - 1 'Then we replace empty lines with a char that will be used later for spliting
                If String.IsNullOrEmpty(Text(i).Trim) Then Text(i) = "$"
            Next
            Dim sb As New Text.StringBuilder
            For Each line In Text
                sb.AppendLine(line)
            Next
            Dim Parts = sb.ToString.Split("$") 'Now we get every subtitle block
            For i As Integer = 0 To Parts.Length - 1
                If Not String.IsNullOrEmpty(Parts(i)) Then
                    'We get our required information
                    Try 'Just in case
                        Dim Index As Integer = 0
                        Dim StartTime As TimeSpan = TimeSpan.Zero
                        Dim EndTime As TimeSpan = TimeSpan.Zero
                        Dim Value As String = String.Empty
                        Dim Ssb As New Text.StringBuilder
                        Dim SplitPart = Parts(i).Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                        Index = SplitPart(0) 'Ref:1
                        Dim Times = SplitPart(1).Split(" --> ") 'Ref: 00:00:19,834 ; 00:00:22,498
                        Dim STime = Times(0).Split(":") 'Ref: 00 ; 00 ; 19,834
                        Dim ETime = Times(2).Split(":") 'Ref: 00 ; 00 , 22,498
                        Dim STimeMS = Times(0).Split(",")(1) 'Ref: 834
                        Dim ETimeMS = Times(2).Split(",")(1) 'Ref: 498
                        StartTime = New TimeSpan(0, hours:=STime(0), minutes:=STime(1), seconds:=STime(2).Split(",")(0), milliseconds:=STimeMS)
                        EndTime = New TimeSpan(0, hours:=ETime(0), minutes:=ETime(1), seconds:=ETime(2).Split(",")(0), milliseconds:=ETimeMS)
                        For _i As Integer = 2 To SplitPart.Length - 1
                            Value += SplitPart(_i)
                        Next
                        Subtitles.Add(New SRTItem With {.Num = Index, .StartTime = StartTime, .EndTime = EndTime, .Text = Value})
                    Catch
                    End Try
                End If
            Next
            RaiseEvent OnSRTLoaded()
        End If
        'Reference :
        '1
        '00:00:19,834 --> 00:00:22,498
        'I Think I'm Losing Hope"
        '"Can you hear me?
    End Sub
    Public Sub DisposeSRT()
        Subtitles.Clear()
        Source = String.Empty
        RaiseEvent OnSRTDisposed()
    End Sub
    Public Sub Offset(ts As TimeSpan, Add As Boolean)
        For Each srt In Subtitles
            If Add Then
                srt.StartTime = srt.StartTime.Add(ts)
                srt.EndTime = srt.EndTime.Add(ts)
            Else
                srt.StartTime = srt.StartTime.Subtract(ts)
                srt.EndTime = srt.EndTime.Subtract(ts)
            End If
        Next
    End Sub
    Public Function GetSubAt(ts As TimeSpan) As SRTItem
        Dim result As SRTItem = Nothing
        For Each srt In Subtitles
            If srt.StartTime <= ts AndAlso srt.EndTime >= ts Then
                result = srt
                Exit For
            End If
        Next
        Return result
    End Function
    ''' <summary>
    ''' Takes a TextBlock as parameter
    ''' Note: setting the PushSrtCancellationToken will stop both PushSubtitles methods
    ''' </summary>
    ''' <param name="TB"></param>
    <Obsolete("This Method is obsolete. Use PushSubtitlesAsync instead.")>
    Public Async Sub PushSubtitles(TB As TextBlock)
        PushSrtCancellationToken = False
        IsPushSrt = True
        If PushSrtCallback IsNot Nothing Then
            Dim PrevEndTime As TimeSpan = TimeSpan.Zero
            For Each srt In Subtitles
                If Not PushSrtCancellationToken Then
                    Await Task.Delay(srt.StartTime.Subtract(PrevEndTime))
                    TB.Text = srt.Text
                    Await Task.Delay(srt.EndTime.Subtract(srt.StartTime))
                    TB.Text = String.Empty
                    PrevEndTime = srt.EndTime
                Else
                    IsPushSrt = False
                    Exit Sub
                End If
            Next
        End If
        IsPushSrt = False
    End Sub
    ''' <summary>
    ''' Set PushSrtCallback(Subtitle As String) before calling this.
    ''' Note: setting the PushSrtCancellationToken will stop both PushSubtitles methods
    ''' </summary>
    <Obsolete("This Method is obsolete. Use PushSubtitlesAsync instead.")>
    Public Async Sub PushSubtitles()
        IsPushSrt = True
        PushSrtCancellationToken = False
        If PushSrtCallback IsNot Nothing Then
            Dim PrevEndTime As TimeSpan = TimeSpan.Zero
            For Each srt In Subtitles
                If Not PushSrtCancellationToken Then
                    Await Task.Delay(srt.StartTime.Subtract(PrevEndTime))
                    PushSrtCurrent = srt
                    PushSrtCallback.Invoke(srt)
                    Await Task.Delay(srt.EndTime.Subtract(srt.StartTime))
                    PushSrtCurrent = Nothing
                    PushSrtCallback.Invoke(srt)
                    PrevEndTime = srt.EndTime
                Else
                    IsPushSrt = False
                    Exit Sub
                End If
            Next
        End If
        IsPushSrt = False
    End Sub
    <Obsolete("This Method is obsolete. Use PushSubtitlesAsync instead.")>
    Public Async Sub PushSubtitles(StartTime As TimeSpan)
        PushSrtCancellationToken = False
        IsPushSrt = True
        If PushSrtCallback IsNot Nothing Then
            Dim PrevEndTime As TimeSpan = TimeSpan.Zero
            For i As Integer = GetSubAt(StartTime).Num - 1 To Subtitles.Count - 1
                If Not PushSrtCancellationToken Then
                    Dim srt = Subtitles(i)
                    Await Task.Delay(srt.StartTime.Subtract(PrevEndTime))
                    PushSrtCurrent = srt
                    PushSrtCallback.Invoke(srt)
                    Await Task.Delay(srt.EndTime.Subtract(srt.StartTime))
                    PushSrtCurrent = srt
                    PushSrtCallback.Invoke(srt)
                    PrevEndTime = srt.EndTime
                Else
                    IsPushSrt = False
                    Exit Sub
                End If
            Next
        End If
        IsPushSrt = False
    End Sub
    Public Async Function PushSubtitles(Callback As Action(Of SRTItem), CancellationToken As Threading.CancellationToken, StartTime As TimeSpan) As Task
        Dim PrevEndTime As TimeSpan = TimeSpan.Zero
        IsPushSrt = True
        RaiseEvent OnPushStarted()
        PushSrtSyncWatch.Restart()
        Await Task.Run(Async Function()
                           Dim StartSub = GetSubAt(StartTime)
                           If StartSub IsNot Nothing Then
                               Dim StartNum = StartSub.Num - 1
                               If StartNum < 0 Then StartNum = 0
                               Try
                                   For i As Integer = StartNum To Subtitles.Count - 1
                                       Dim srt = Subtitles(i)
                                       Await Task.Delay(srt.StartTime.Subtract(PrevEndTime))
                                       PushSrtCurrent = srt
                                       Callback.Invoke(srt)
                                       Await Task.Delay(srt.EndTime.Subtract(srt.StartTime))
                                       PushSrtCurrent = srt
                                       Callback.Invoke(srt)
                                       PrevEndTime = srt.EndTime
                                   Next
                               Catch
                               End Try
                           Else
                               Try
                                   For i As Integer = 0 To Subtitles.Count - 1
                                       Dim srt = Subtitles(i)
                                       Await Task.Delay(srt.StartTime.Subtract(PrevEndTime))
                                       PushSrtCurrent = srt
                                       Callback.Invoke(srt)
                                       Await Task.Delay(srt.EndTime.Subtract(srt.StartTime))
                                       PushSrtCurrent = srt
                                       Callback.Invoke(srt)
                                       PrevEndTime = srt.EndTime
                                   Next
                               Catch
                               End Try
                           End If
                       End Function, CancellationToken)
        PushSrtSyncWatch.Stop()
        IsPushSrt = False
        RaiseEvent OnPushFinished()
    End Function
    Public Function GetSyncTime() As TimeSpan
        If PushSrtSyncWatch IsNot Nothing Then
            Return PushSrtSyncWatch.Elapsed
        Else
            Return TimeSpan.Zero
        End If
    End Function
End Class
