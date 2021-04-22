Public Class PluginControl
    Public Property PlugName As String
    Public Property PlugDesc As String
    Public Property PlugIcon As ImageSource
    Public Sub New(Name As String, Description As String, Icon As BitmapImage, version As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _name.Content = Name : PlugName = Name
        _description.Text = Description : PlugDesc = Description
        _icon.Source = Icon : PlugIcon = Icon
        _version.Text = version
    End Sub
End Class
