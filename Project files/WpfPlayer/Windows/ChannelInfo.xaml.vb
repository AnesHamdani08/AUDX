﻿Imports System.ComponentModel

Public Class ChannelInfo
    Dim WithEvents UIManager As New Threading.DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(33), .IsEnabled = True}
    Dim WithEvents UICounterUpdator As New Threading.DispatcherTimer With {.Interval = TimeSpan.FromSeconds(1), .IsEnabled = True}
    Private CPUCounterAll As PerformanceCounter = New PerformanceCounter("Processor", "% Processor Time", "_Total")
    Private MemCounterAll As PerformanceCounter = New PerformanceCounter("Memory", "Available MBytes")
    Private CPUCounterMP As PerformanceCounter = New PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName)
    Private MemCounterMP As PerformanceCounter = New PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName)
    Private MaxMem As Integer

    Private Sub ChannelInfo_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Hide()
        e.Cancel = True
    End Sub

    Private Sub ChannelInfo_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        MaxMem = (My.Computer.Info.TotalPhysicalMemory / 1024 / 1024)
        UIManager.Start()
    End Sub

    Private Sub UIManager_Tick() Handles UIManager.Tick
        Try
            With CType(Application.Current.MainWindow, MainWindow).MainPlayer.GetPeak
                Peak_Master.Value = .Master
                Peak_Left.Value = .Left
                Peak_Right.Value = .Right
            End With
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Dim CpuCAV As Integer
    Dim CpuCMV As Integer
    Dim MemCAV As Integer
    Dim MemCMV As Integer
    Private Sub UICounterUpdator_Tick(sender As Object, e As EventArgs) Handles UICounterUpdator.Tick
        CpuCAV = CPUCounterAll.NextValue
        CpuCMV = CPUCounterMP.NextValue
        MemCAV = MemCounterAll.NextValue
        MemCMV = MemCounterMP.NextValue / 1024 / 1024
        TB_Cpu_Usage.Text = "Cpu Usage: " & Math.Abs(CpuCMV - CpuCAV) & "%/" & CpuCAV & "%"
        TB_Mem_Usage.Text = "Memory Usage: " & MemCMV & "MB/" & MemCAV & "MB/" & Math.Round(Mem_Usage_All.Maximum) & "MB"
        Try
            If My.Settings.UseAnimations Then
                SetPercent(Cpu_Usage_All, CpuCAV)
                SetPercent(Mem_Usage_MP, Math.Round((MemCMV / MaxMem) * 100))
                SetPercent(Mem_Usage_All, Math.Round(((MaxMem - MemCAV) / MaxMem) * 100))
                SetPercent(Cpu_Usage_MP, Math.Abs(CpuCMV - CpuCAV))
            Else
                Cpu_Usage_All.Value = CpuCAV
                Cpu_Usage_MP.Value = CpuCMV
                Mem_Usage_All.Value = Math.Round(((MaxMem - MemCAV) / MaxMem) * 100)
                Mem_Usage_MP.Value = Math.Round((MemCMV / MaxMem) * 100)
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private duration As New System.Windows.Duration(TimeSpan.FromSeconds(1))
    Private Sub SetPercent(ByVal progressBar As HandyControl.Controls.WaveProgressBar, ByVal percentage As Double)
        Dim panim As New Animation.DoubleAnimation(percentage, duration)
        progressBar.BeginAnimation(HandyControl.Controls.WaveProgressBar.ValueProperty, panim)
    End Sub

    Private Sub textBlock_l_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles textBlock_l.MouseUp
        Dim s As System.IO.Stream = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("WpfPlayer.left.wav")
        Dim player As System.Media.SoundPlayer = New System.Media.SoundPlayer(s)
        player.Play()
        s.Dispose()
    End Sub

    Private Sub textBlock_r_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles textBlock_r.MouseUp
        Dim s As System.IO.Stream = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("WpfPlayer.right.wav")
        Dim player As System.Media.SoundPlayer = New System.Media.SoundPlayer(s)
        player.Play()
        s.Dispose()
    End Sub

    Private Sub textBlock_lr_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles textBlock_lr.MouseUp
        Dim s As System.IO.Stream = System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("WpfPlayer.center.wav")
        Dim player As System.Media.SoundPlayer = New System.Media.SoundPlayer(s)
        player.Play()
        s.Dispose()
    End Sub
End Class
