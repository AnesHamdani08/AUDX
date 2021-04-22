Public Class ErrorDialog
    Private Property _ErrorMsg As String
    Public Sub New(Msg As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _ErrorMsg = Msg
    End Sub

    Private Sub ErrorDialog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If String.IsNullOrEmpty(_ErrorMsg) Then
            Me.Close()
        Else
            error_msg.Text = _ErrorMsg
        End If
    End Sub

    Private Sub exit_btn_Click(sender As Object, e As RoutedEventArgs) Handles exit_btn.Click
        Me.Close()
    End Sub
End Class
