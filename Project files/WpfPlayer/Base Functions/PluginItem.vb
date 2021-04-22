Public Class PluginItem
    Private Property pname As String
    Private Property pdescription As String
    Private Property picon As BitmapImage
    Private Property pver As String
    Public ReadOnly Property Name As String
        Get
            Return pname
        End Get
    End Property
    Public ReadOnly Property Description As String
        Get
            Return pdescription
        End Get
    End Property
    Public ReadOnly Property Icon As BitmapImage
        Get
            Return picon
        End Get
    End Property
    Public ReadOnly Property Version As String
        Get
            Return pver
        End Get
    End Property
    Public ReadOnly Property PluginControl As PluginControl
        Get
            Return New PluginControl(pname, pdescription, picon, pver)
        End Get
    End Property
    Public Sub New(PlugName As String, PlugDesc As String, PlugIcon As BitmapImage, PlugVersion As String)
        pname = PlugName
        pdescription = PlugDesc
        picon = PlugIcon
        pver = PlugVersion
    End Sub
End Class
