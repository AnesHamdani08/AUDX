Imports System.Threading
Imports System.Windows.Forms

Public Class AsyncWebBrowser
    Protected WithEvents m_WebBrowser As Forms.WebBrowser
    Private m_MRE As ManualResetEvent = New ManualResetEvent(False)

    Public Sub SetBrowser(ByVal browser As Forms.WebBrowser)
        Me.m_WebBrowser = browser
    End Sub

    Public Function NavigateAsync(ByVal url As String) As Task
        Navigate(url)
        Return Task.Factory.StartNew(CType((Function()
                                                m_MRE.WaitOne()
                                                m_MRE.Reset()
                                            End Function), Action))
    End Function

    Public Sub Navigate(ByVal url As String)
        m_WebBrowser.Navigate(New Uri(url))
    End Sub
    Private Sub m_WebBrowser_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles m_WebBrowser.DocumentCompleted
        m_MRE.[Set]()
    End Sub
End Class
