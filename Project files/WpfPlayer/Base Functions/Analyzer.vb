Friend Class Analyzer
    Private _enable As Boolean
    Private _t As Forms.Timer
    Private _fft As Single()
    Private _l, _r As ProgressBar
    Private _lastlevel As Integer
    Private _hanctr As Integer
    Private _spectrumdata As List(Of Byte)
    Private _player As Player
    Private _initialized As Boolean
    Private devindex As Integer
    Private _lines As Integer = 64
    Public Event DataArrived(Data As List(Of Byte))
    Public Sub New(ByVal left As ProgressBar, ByVal right As ProgressBar, Player As Player)
        _fft = New Single(1023) {}
        _lastlevel = 0
        _hanctr = 0
        _t = New Forms.Timer()
        AddHandler _t.Tick, AddressOf _t_Tick
        _t.Interval = 100
        _t.Enabled = False
        _l = left
        _r = right
        _l.Minimum = 0
        _r.Minimum = 0
        _r.Maximum = UShort.MaxValue
        _l.Maximum = UShort.MaxValue
        _spectrumdata = New List(Of Byte)()
        _player = Player
        _initialized = False
    End Sub

    Public Property Enable As Boolean
        Get
            Return _enable
        End Get
        Set(ByVal value As Boolean)
            _enable = value
            _t.Enabled = value
        End Set
    End Property

    Private Sub _t_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Dim ret As Integer = Un4seen.Bass.Bass.BASS_ChannelGetData(_player.Stream, _fft, CInt(Un4seen.Bass.BASSData.BASS_DATA_FFT2048))
        If ret < 0 Then Return
        Dim x, y As Integer
        Dim b0 As Integer = 0

        For x = 0 To _lines - 1
            Dim peak As Single = 0
            Dim b1 As Integer = CInt(Math.Pow(2, x * 10.0 / (_lines - 1)))
            If b1 > 1023 Then b1 = 1023
            If b1 <= b0 Then b1 = b0 + 1

            While b0 < b1
                If peak < _fft(1 + b0) Then peak = _fft(1 + b0)
                b0 += 1
            End While

            y = CInt((Math.Sqrt(peak) * 3 * 255 - 4))
            If y > 255 Then y = 255
            If y < 0 Then y = 0
            _spectrumdata.Add(CByte(y))
        Next

        RaiseEvent DataArrived(_spectrumdata)
        _spectrumdata.Clear()
        Dim _peak = _player.GetPeak
        _l.Value = _peak.Left
        _r.Value = _peak.Right
        'If _peak.Master = _lastlevel AndAlso _peak.Master <> 0 Then _hanctr += 1
        '_lastlevel = _peak.Master

        'If _hanctr > 3 Then
        '    _hanctr = 0
        '    _l.Value = 0
        '    _r.Value = 0
        '    Enable = True
        'End If
    End Sub

    Private Function Process(ByVal buffer As IntPtr, ByVal length As Integer, ByVal user As IntPtr) As Integer
        Return length
    End Function
End Class
