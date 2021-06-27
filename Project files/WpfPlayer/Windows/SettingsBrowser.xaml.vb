Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class SettingsBrowser
    Public Class SettingItem
        Implements INotifyPropertyChanged

        Private _num As Integer
        Private _name As String
        Private _value As String
        Private _type As String
        Public Sub New(pnum As String, pname As String, pvalue As String, ptype As String, <CallerMemberName> ByVal Optional caller As String = Nothing)
            Num = pnum
            Name = pname
            Value = pvalue
            Type = ptype
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

        Public Property Value As String
            Get
                Return _value
            End Get
            Set(ByVal value As String)
                If _value = value Then Return
                _value = value
                OnPropertyChanged()
            End Set
        End Property

        Public Property Type As String
            Get
                Return _type
            End Get
            Set(ByVal value As String)
                If _type = value Then Return
                _type = value
                OnPropertyChanged()
            End Set
        End Property
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(<CallerMemberName> ByVal Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    Private Settings_Source As New ObjectModel.ObservableCollection(Of SettingItem)

    Private Sub SettingsBrowser_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Main_SettingsView.ItemsSource = Settings_Source
        Dim Dummy = My.Settings.DUMMY
        Dim i = 0
        For Each value As System.Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            If value.PropertyValue IsNot Nothing Then
                If value.PropertyValue.GetType IsNot GetType(Specialized.StringCollection) Then
                    Settings_Source.Add(New SettingItem(i + 1, value.Name, value.PropertyValue.ToString, value.PropertyValue.GetType.Name))
                Else
                    Settings_Source.Add(New SettingItem(i + 1, value.Name, TryCast(value.PropertyValue, Specialized.StringCollection).Count & " Item(s)", value.PropertyValue.GetType.Name))
                End If
                i += 1
            End If
        Next
    End Sub

    Private Sub SettingsBrowser_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Hide()
        e.Cancel = True
    End Sub
End Class
