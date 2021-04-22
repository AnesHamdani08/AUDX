Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class Plugins
    Public Class PluginItem
        Implements INotifyPropertyChanged

        Private _num As Integer
        Private _name As String
        Private _author As String
        Private _version As String
        Public Sub New(pnum As String, pname As String, pauthor As String, pversion As String, <CallerMemberName> ByVal Optional caller As String = Nothing)
            Num = pnum
            Name = pname
            Author = pauthor
            Version = pversion
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

        Public Property Author As String
            Get
                Return _author
            End Get
            Set(ByVal value As String)
                If _author = value Then Return
                _author = value
                OnPropertyChanged()
            End Set
        End Property

        Public Property Version As String
            Get
                Return _version
            End Get
            Set(ByVal value As String)
                If _version = value Then Return
                _version = value
                OnPropertyChanged()
            End Set
        End Property
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    Private Plugins_Source As New ObjectModel.ObservableCollection(Of PluginItem)

    Private Sub Plugins_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.Hide()
        e.Cancel = True
    End Sub

        Private Sub Plugins_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Main_Plugins_View.ItemsSource = Plugins_Source
        For Each assembly In Reflection.Assembly.GetExecutingAssembly.GetReferencedAssemblies
            If assembly.Name.IndexOf(".") <> -1 Then
                Plugins_Source.Add(New PluginItem(Plugins_Source.Count + 1, assembly.Name, assembly.Name.Substring(0, assembly.Name.IndexOf(".")), assembly.Version.ToString))
            Else
                Plugins_Source.Add(New PluginItem(Plugins_Source.Count + 1, assembly.Name, assembly.Name, assembly.Version.ToString))
            End If
        Next
    End Sub
    End Class
