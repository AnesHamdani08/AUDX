﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On



<Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
 Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"),  _
 Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
Partial Friend NotInheritable Class MySettings
    Inherits Global.System.Configuration.ApplicationSettingsBase
    
    Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
    
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
    
    Public Shared ReadOnly Property [Default]() As MySettings
        Get
            
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
            Return defaultInstance
        End Get
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property LastMediaTitle() As String
        Get
            Return CType(Me("LastMediaTitle"),String)
        End Get
        Set
            Me("LastMediaTitle") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("<?xml version=""1.0"" encoding=""utf-16""?>"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"<ArrayOfString xmlns:xsi=""http://www.w3."& _ 
        "org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />")>  _
    Public Property LastPlaylist() As Global.System.Collections.Specialized.StringCollection
        Get
            Return CType(Me("LastPlaylist"),Global.System.Collections.Specialized.StringCollection)
        End Get
        Set
            Me("LastPlaylist") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property LastMediaSeek() As Double
        Get
            Return CType(Me("LastMediaSeek"),Double)
        End Get
        Set
            Me("LastMediaSeek") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property LastMediaDuration() As Double
        Get
            Return CType(Me("LastMediaDuration"),Double)
        End Get
        Set
            Me("LastMediaDuration") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property LastMediaIndex() As Integer
        Get
            Return CType(Me("LastMediaIndex"),Integer)
        End Get
        Set
            Me("LastMediaIndex") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property DefaultTheme() As Integer
        Get
            Return CType(Me("DefaultTheme"),Integer)
        End Get
        Set
            Me("DefaultTheme") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("<?xml version=""1.0"" encoding=""utf-16""?>"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"<ArrayOfString xmlns:xsi=""http://www.w3."& _ 
        "org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />")>  _
    Public Property LibrariesPath() As Global.System.Collections.Specialized.StringCollection
        Get
            Return CType(Me("LibrariesPath"),Global.System.Collections.Specialized.StringCollection)
        End Get
        Set
            Me("LibrariesPath") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property UseAnimations() As Boolean
        Get
            Return CType(Me("UseAnimations"),Boolean)
        End Get
        Set
            Me("UseAnimations") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Property FX_EQ() As Global.Un4seen.Bass.BASS_DX8_PARAMEQ
        Get
            Return CType(Me("FX_EQ"),Global.Un4seen.Bass.BASS_DX8_PARAMEQ)
        End Get
        Set
            Me("FX_EQ") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Property FX_REVERB() As Global.Un4seen.Bass.BASS_DX8_REVERB
        Get
            Return CType(Me("FX_REVERB"),Global.Un4seen.Bass.BASS_DX8_REVERB)
        End Get
        Set
            Me("FX_REVERB") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
    Public Property UseDiscordRPC() As Boolean
        Get
            Return CType(Me("UseDiscordRPC"),Boolean)
        End Get
        Set
            Me("UseDiscordRPC") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Property FX_DAMP() As Global.Un4seen.Bass.AddOn.Fx.BASS_BFX_DAMP
        Get
            Return CType(Me("FX_DAMP"),Global.Un4seen.Bass.AddOn.Fx.BASS_BFX_DAMP)
        End Get
        Set
            Me("FX_DAMP") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property LastMediaType() As Integer
        Get
            Return CType(Me("LastMediaType"),Integer)
        End Get
        Set
            Me("LastMediaType") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property LastMediaArtist() As String
        Get
            Return CType(Me("LastMediaArtist"),String)
        End Get
        Set
            Me("LastMediaArtist") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property LastMedia() As String
        Get
            Return CType(Me("LastMedia"),String)
        End Get
        Set
            Me("LastMedia") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property LastMediaYear() As Integer
        Get
            Return CType(Me("LastMediaYear"),Integer)
        End Get
        Set
            Me("LastMediaYear") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property MusicBrainzNotify() As Boolean
        Get
            Return CType(Me("MusicBrainzNotify"),Boolean)
        End Get
        Set
            Me("MusicBrainzNotify") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property LyricsNotify() As Boolean
        Get
            Return CType(Me("LyricsNotify"),Boolean)
        End Get
        Set
            Me("LyricsNotify") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property BackgroundType() As Integer
        Get
            Return CType(Me("BackgroundType"),Integer)
        End Get
        Set
            Me("BackgroundType") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property MediaBar_AnimType() As Integer
        Get
            Return CType(Me("MediaBar_AnimType"),Integer)
        End Get
        Set
            Me("MediaBar_AnimType") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("<?xml version=""1.0"" encoding=""utf-16""?>"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"<ArrayOfString xmlns:xsi=""http://www.w3."& _ 
        "org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />")>  _
    Public Property FavouriteTracks() As Global.System.Collections.Specialized.StringCollection
        Get
            Return CType(Me("FavouriteTracks"),Global.System.Collections.Specialized.StringCollection)
        End Get
        Set
            Me("FavouriteTracks") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property Home_ShowVisualiser() As Boolean
        Get
            Return CType(Me("Home_ShowVisualiser"),Boolean)
        End Get
        Set
            Me("Home_ShowVisualiser") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property OnMediaChange_FadeAudio() As Boolean
        Get
            Return CType(Me("OnMediaChange_FadeAudio"),Boolean)
        End Get
        Set
            Me("OnMediaChange_FadeAudio") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property StreamDownloaderNotify() As Boolean
        Get
            Return CType(Me("StreamDownloaderNotify"),Boolean)
        End Get
        Set
            Me("StreamDownloaderNotify") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
    Public Property Notificationtts() As Boolean
        Get
            Return CType(Me("Notificationtts"),Boolean)
        End Get
        Set
            Me("Notificationtts") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property FX_BALANCE() As Single
        Get
            Return CType(Me("FX_BALANCE"),Single)
        End Get
        Set
            Me("FX_BALANCE") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property FX_SAMPLERATE() As Single
        Get
            Return CType(Me("FX_SAMPLERATE"),Single)
        End Get
        Set
            Me("FX_SAMPLERATE") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property OnErrorShow() As Integer
        Get
            Return CType(Me("OnErrorShow"),Integer)
        End Get
        Set
            Me("OnErrorShow") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
    Public Property SuppressErrors() As Boolean
        Get
            Return CType(Me("SuppressErrors"),Boolean)
        End Get
        Set
            Me("SuppressErrors") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property AllowGlobalHotkeys() As Boolean
        Get
            Return CType(Me("AllowGlobalHotkeys"),Boolean)
        End Get
        Set
            Me("AllowGlobalHotkeys") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_PlayPause() As Integer
        Get
            Return CType(Me("GlobalHotkey_PlayPause"),Integer)
        End Get
        Set
            Me("GlobalHotkey_PlayPause") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_PlayPause_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_PlayPause_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_PlayPause_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Next() As Integer
        Get
            Return CType(Me("GlobalHotkey_Next"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Next") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Next_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_Next_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Next_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Previous() As Integer
        Get
            Return CType(Me("GlobalHotkey_Previous"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Previous") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Previous_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_Previous_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Previous_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Skip10() As Integer
        Get
            Return CType(Me("GlobalHotkey_Skip10"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Skip10") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Skip10_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_Skip10_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Skip10_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Back10() As Integer
        Get
            Return CType(Me("GlobalHotkey_Back10"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Back10") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_Back10_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_Back10_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_Back10_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_VolumeUp() As Integer
        Get
            Return CType(Me("GlobalHotkey_VolumeUp"),Integer)
        End Get
        Set
            Me("GlobalHotkey_VolumeUp") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_VolumeUp_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_VolumeUp_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_VolumeUp_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_VolumeDown() As Integer
        Get
            Return CType(Me("GlobalHotkey_VolumeDown"),Integer)
        End Get
        Set
            Me("GlobalHotkey_VolumeDown") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_VolumeDown_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_VolumeDown_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_VolumeDown_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_VolumeMute() As Integer
        Get
            Return CType(Me("GlobalHotkey_VolumeMute"),Integer)
        End Get
        Set
            Me("GlobalHotkey_VolumeMute") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
    Public Property GlobalHotkey_VolumeMute_MOD() As Integer
        Get
            Return CType(Me("GlobalHotkey_VolumeMute_MOD"),Integer)
        End Get
        Set
            Me("GlobalHotkey_VolumeMute_MOD") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property Library_Path() As String
        Get
            Return CType(Me("Library_Path"),String)
        End Get
        Set
            Me("Library_Path") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property SoundCloud_Notify() As Boolean
        Get
            Return CType(Me("SoundCloud_Notify"),Boolean)
        End Get
        Set
            Me("SoundCloud_Notify") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Property FX_STEREOMIX() As Object
        Get
            Return CType(Me("FX_STEREOMIX"),Object)
        End Get
        Set
            Me("FX_STEREOMIX") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
    Public Property MiniPlayer_SmartColors() As Boolean
        Get
            Return CType(Me("MiniPlayer_SmartColors"),Boolean)
        End Get
        Set
            Me("MiniPlayer_SmartColors") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property CacheLibraryData() As Boolean
        Get
            Return CType(Me("CacheLibraryData"),Boolean)
        End Get
        Set
            Me("CacheLibraryData") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
    Public Property IsFirstStart() As Boolean
        Get
            Return CType(Me("IsFirstStart"),Boolean)
        End Get
        Set
            Me("IsFirstStart") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("")>  _
    Public Property UpdatesServer() As String
        Get
            Return CType(Me("UpdatesServer"),String)
        End Get
        Set
            Me("UpdatesServer") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
    Public Property SkipSilences() As Boolean
        Get
            Return CType(Me("SkipSilences"),Boolean)
        End Get
        Set
            Me("SkipSilences") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("4")>  _
    Public Property PlaylistDragDropAction() As Integer
        Get
            Return CType(Me("PlaylistDragDropAction"),Integer)
        End Get
        Set
            Me("PlaylistDragDropAction") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("TopDirectoryOnly")>  _
    Public Property FBD_QuickAcess_SubFolders() As Global.System.IO.SearchOption
        Get
            Return CType(Me("FBD_QuickAcess_SubFolders"),Global.System.IO.SearchOption)
        End Get
        Set
            Me("FBD_QuickAcess_SubFolders") = value
        End Set
    End Property
    
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("<?xml version=""1.0"" encoding=""utf-16""?>"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"<ArrayOfString xmlns:xsi=""http://www.w3."& _ 
        "org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" />")>  _
    Public Property DSP_Plugins() As Global.System.Collections.Specialized.StringCollection
        Get
            Return CType(Me("DSP_Plugins"),Global.System.Collections.Specialized.StringCollection)
        End Get
        Set
            Me("DSP_Plugins") = value
        End Set
    End Property
End Class

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.WpfPlayer.MySettings
            Get
                Return Global.WpfPlayer.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
