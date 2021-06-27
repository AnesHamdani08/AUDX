Imports Microsoft.WindowsAPICodePack
Public Class TaskbarJumpListManager
    Public Shared Property JumpList As Taskbar.JumpList = Nothing
    Private Shared AppPath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, My.Application.Info.AssemblyName & ".exe")
    Public Shared Sub Prepare()
        JumpList = Taskbar.JumpList.CreateJumpList
        JumpList.KnownCategoryToDisplay = Taskbar.JumpListKnownCategoryType.Recent
        Dim CAT_Controls As New Taskbar.JumpListCustomCategory("Controls")
        Dim CAT_PlayPause As New Taskbar.JumpListLink(AppPath, "Play/Pause") With {.IconReference = New Shell.IconReference(AppPath, 0), .Arguments = "-j -pp"}
        Dim CAT_Previous As New Taskbar.JumpListLink(AppPath, "Previous") With {.IconReference = New Shell.IconReference(AppPath, 1), .Arguments = "-j -p"}
        Dim CAT_Next As New Taskbar.JumpListLink(AppPath, "Next") With {.IconReference = New Shell.IconReference(AppPath, 2), .Arguments = "-j -n"}
        Dim CAT_Playlist As New Taskbar.JumpListCustomCategory("Playlist")
        Dim CAT_PlaylistAll As New Taskbar.JumpListLink(AppPath, "All") With {.IconReference = New Shell.IconReference(AppPath, 3), .Arguments = "-j -pa"}
        Dim CAT_PlaylistAllShuffled As New Taskbar.JumpListLink(AppPath, "All Shuffled") With {.IconReference = New Shell.IconReference(AppPath, 1), .Arguments = "-j -pas"}
        Dim CAT_Random10 As New Taskbar.JumpListLink(AppPath, "Random 10") With {.IconReference = New Shell.IconReference(AppPath, 1), .Arguments = "-j -prt"}
        CAT_Controls.AddJumpListItems({CAT_Previous, CAT_PlayPause, CAT_Next})
        CAT_Playlist.AddJumpListItems({CAT_PlaylistAll, CAT_PlaylistAllShuffled, CAT_Random10})
        JumpList.AddCustomCategories(CAT_Controls)
        JumpList.AddUserTasks(New Taskbar.JumpListSeparator)
        JumpList.AddCustomCategories(CAT_Playlist)
        JumpList.Refresh()
    End Sub
End Class
