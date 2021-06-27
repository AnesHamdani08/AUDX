Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports WpfPlayer

Public Class DSPPlugins
    Public Class DSPPluginItem
        Implements INotifyPropertyChanged

        Private _num As Integer
        Private _name As String
        Public Sub New(pnum As String, pname As String, DSP As Player.DSPPlugin, <CallerMemberName> ByVal Optional caller As String = Nothing)
            Num = pnum
            Name = pname
            Source = DSP
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

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                If _name = value Then Return
                _name = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property Source As Player.DSPPlugin

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
    Private DSP_Source As New ObjectModel.ObservableCollection(Of DSPPluginItem)
    Private WithEvents LinkPlayer As Player
    Private Sub DSPPlugins_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Main_DSP_View.ItemsSource = DSP_Source
        LinkPlayer = TryCast(Application.Current.MainWindow, MainWindow).MainPlayer
        For Each plugin In LinkPlayer.LoadedDSP
            DSP_Source.Add(New DSPPluginItem(DSP_Source.Count + 1, plugin.Name, plugin))
        Next
    End Sub
    Private Sub DSPPlugins_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub

    Private Sub Main_DSP_View_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Main_DSP_View.MouseDown
        If e.ClickCount = 2 Then
            Try
                If Main_DSP_View.SelectedIndex <> -1 Then
                    DSP_Source.Item(Main_DSP_View.SelectedIndex).Source.Config()
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub LinkPlayer_OnDspAdded(DSP As Player.DSPPlugin) Handles LinkPlayer.OnDspAdded
        DSP_Source.Add(New DSPPluginItem(DSP_Source.Count + 1, DSP.Name, DSP))
        My.Settings.DSP_PLUGINS.Add(DSP.Source)
        My.Settings.Save()
    End Sub

    Private Sub LinkPlayer_OnDspRemoved(DSP As Player.DSPPlugin) Handles LinkPlayer.OnDspRemoved
        DSP_Source.Remove(DSP_Source.FirstOrDefault(Function(k) k.Source.Handle = DSP.Handle))
        My.Settings.DSP_PLUGINS.Remove(DSP.Source)
        My.Settings.Save()
    End Sub

    Private Sub TitleBar_Add_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Add.Click
        Dim ofd As New Ookii.Dialogs.Wpf.VistaOpenFileDialog
        If ofd.ShowDialog() Then
            LinkPlayer.ImportDSP(ofd.FileName)
        End If
    End Sub

    Private Sub TitleBar_Remove_Click(sender As Object, e As RoutedEventArgs) Handles TitleBar_Remove.Click
        Try
            LinkPlayer.RemoveDSP(DSP_Source(Main_DSP_View.SelectedIndex).Source)
        Catch ex As Exception
        End Try
    End Sub
End Class
