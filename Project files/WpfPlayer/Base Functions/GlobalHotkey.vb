Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class GlobalHotkey
    Private modifier As Integer
    Private key As Integer
    Private hWnd As IntPtr
    Private id As Integer

    Public Sub New(ByVal modifier As Integer, ByVal key As Keys, ByVal form As Form, id As Integer)
        Me.modifier = modifier
        Me.key = CInt(key)
        Me.hWnd = form.Handle
        Me.id = id
    End Sub
    Public Sub New(ByVal modifier As Integer, ByVal key As Keys, ByVal handle As IntPtr, id As Integer)
        Me.modifier = modifier
        Me.key = CInt(key)
        Me.hWnd = handle
        Me.id = id
    End Sub
    Public Function Register() As Boolean
        Return RegisterHotKey(hWnd, id, modifier, key)
    End Function

    Public Function Unregister() As Boolean
        Return UnregisterHotKey(hWnd, id)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return modifier Xor key Xor hWnd.ToInt32()
    End Function

    <DllImport("user32.dll")>
    Private Shared Function RegisterHotKey(ByVal hWnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function UnregisterHotKey(ByVal hWnd As IntPtr, ByVal id As Integer) As Boolean
    End Function
End Class
