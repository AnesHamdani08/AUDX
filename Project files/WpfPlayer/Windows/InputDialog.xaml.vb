Imports System.ComponentModel

Public Class InputDialog
    Public Property Input As String
    Public Sub New(msg As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ib_msg.Text = msg
    End Sub

    Private Sub ib_done_Click(sender As Object, e As RoutedEventArgs) Handles ib_done.Click
        If Not String.IsNullOrEmpty(ib_input.Text) AndAlso Not String.IsNullOrWhiteSpace(ib_input.Text) Then
            Input = ib_input.Text
            DialogResult = True
        End If
    End Sub
End Class
