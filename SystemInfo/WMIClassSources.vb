''' <summary>An object that does not come from a WMI query but from code (for example the registry).</summary>
Friend Class SyntheticObject
    Public ReadOnly Property Name As String
    Public ReadOnly Property Properties As New List(Of KeyValuePair(Of String, String))

    Public Sub New(ByVal pName As String)
        Name = pName
    End Sub

    ''' <summary>Adds a property; empty values are left out.</summary>
    Public Sub Add(ByVal pName As String, ByVal pValue As String)
        If Not String.IsNullOrWhiteSpace(pValue) Then Properties.Add(New KeyValuePair(Of String, String)(pName, pValue.Trim()))
    End Sub
End Class

''' <summary>Where the objects of a class of the report come from.</summary>
Friend Class WMIClassSource
    ''' <summary>WMI namespace the class lives in.</summary>
    Public ReadOnly Property NamespacePath As String
    ''' <summary>Optional WQL filter (the part after WHERE); empty = every object.</summary>
    Public ReadOnly Property Filter As String
    ''' <summary>When set, the objects come from this code instead of a WMI query. The Func tells the reader to stop (scan cancelled).</summary>
    Public ReadOnly Property Reader As Func(Of RemoteTarget, Func(Of Boolean), List(Of SyntheticObject))

    Public Sub New(ByVal pNamespacePath As String, ByVal pFilter As String, ByVal pReader As Func(Of RemoteTarget, Func(Of Boolean), List(Of SyntheticObject)))
        NamespacePath = pNamespacePath
        Filter = pFilter
        Reader = pReader
    End Sub
End Class

''' <summary>
''' Describes the classes of the report that are not plain "SELECT * FROM class" queries on root\CIMV2: they
''' live in another namespace, need a filter, or are read by code. Anything not listed here is queried
''' on root\CIMV2 without a filter.
''' </summary>
Friend Module WMIClassSources
    Private Const Cimv2 As String = "root\CIMV2"

    Private ReadOnly _Default As New WMIClassSource(Cimv2, String.Empty, Nothing)

    Private ReadOnly _Sources As New Dictionary(Of String, WMIClassSource)(StringComparer.OrdinalIgnoreCase) From {
        {"SoftwareLicensingProduct", New WMIClassSource(Cimv2, "ApplicationID='55c92734-d682-4d71-983e-d6ec3f16059f' AND PartialProductKey IS NOT NULL", Nothing)},
        {"Win32_PnPEntity", New WMIClassSource(Cimv2, "ConfigManagerErrorCode <> 0", Nothing)},
        {"Win32_NetworkAdapterConfiguration", New WMIClassSource(Cimv2, "IPEnabled = TRUE", Nothing)},
        {"Win32_UserAccount", New WMIClassSource(Cimv2, "LocalAccount = TRUE", Nothing)},
        {"Win32_UserProfile", New WMIClassSource(Cimv2, "Special = FALSE", Nothing)},
        {"Win32_Share", New WMIClassSource(Cimv2, "Type = 0", Nothing)},
        {"Win32_PnPSignedDriver", New WMIClassSource(Cimv2, "DeviceClass = 'DISPLAY' OR DeviceClass = 'NET'", Nothing)},
        {"Win32_DeviceGuard", New WMIClassSource("root\Microsoft\Windows\DeviceGuard", String.Empty, Nothing)},
        {"MSFT_StorageReliabilityCounter", New WMIClassSource("root\Microsoft\Windows\Storage", String.Empty, AddressOf InventoryReaders.ReadDiskReliability)},
        {"BatteryHealth", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadBatteryHealth)},
        {"LocalAdministrators", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadLocalAdministrators)},
        {"OfficeClickToRun", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadOfficeClickToRun)},
        {"ThirdPartyServices", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadThirdPartyServices)},
        {"Windows11Readiness", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadWindows11Readiness)},
        {"MSFT_PhysicalDisk", New WMIClassSource("root\Microsoft\Windows\Storage", String.Empty, Nothing)},
        {"MSFT_MpComputerStatus", New WMIClassSource("root\Microsoft\Windows\Defender", String.Empty, Nothing)},
        {"AntiVirusProduct", New WMIClassSource("root\SecurityCenter2", String.Empty, Nothing)},
        {"Win32_Tpm", New WMIClassSource("root\CIMV2\Security\MicrosoftTpm", String.Empty, Nothing)},
        {"Win32_EncryptableVolume", New WMIClassSource("root\CIMV2\Security\MicrosoftVolumeEncryption", String.Empty, Nothing)},
        {"InstalledSoftware", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadInstalledSoftware)},
        {"WindowsRelease", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadWindowsRelease)},
        {"BootSecurity", New WMIClassSource(Cimv2, String.Empty, AddressOf InventoryReaders.ReadBootSecurity)}
    }

    'Names shown in the viewer for the classes whose WMI name says little to a person
    Private ReadOnly _DisplayNames As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
        {"SoftwareLicensingProduct", "WindowsActivation"},
        {"Win32_PnPEntity", "ProblemDevices"},
        {"Win32_QuickFixEngineering", "Updates"},
        {"MSFT_PhysicalDisk", "PhysicalDisk"},
        {"MSFT_MpComputerStatus", "DefenderStatus"},
        {"AntiVirusProduct", "AntiVirus"},
        {"Win32_Tpm", "TPM"},
        {"Win32_EncryptableVolume", "BitLocker"},
        {"Win32_NetworkAdapterConfiguration", "NetworkConfiguration"},
        {"Win32_UserAccount", "LocalUsers"},
        {"Win32_UserProfile", "UserProfiles"},
        {"Win32_Share", "SharedFolders"},
        {"Win32_StartupCommand", "StartupPrograms"},
        {"Win32_PnPSignedDriver", "Drivers"},
        {"Win32_DeviceGuard", "VirtualizationSecurity"},
        {"Win32_PageFileUsage", "PageFile"},
        {"MSFT_StorageReliabilityCounter", "DiskReliability"}
    }

    Friend Function Find(ByVal pClassName As String) As WMIClassSource
        Dim oSource As WMIClassSource = Nothing
        Return If(_Sources.TryGetValue(pClassName, oSource), oSource, _Default)
    End Function

    ''' <summary>Name of a class as the viewer shows it (without the Win32_ / MSFT_ prefix, or a friendlier name).</summary>
    Friend Function DisplayName(ByVal pClassName As String) As String
        Dim oName As String = Nothing
        If _DisplayNames.TryGetValue(pClassName, oName) Then Return oName
        Return pClassName.Replace("Win32_", String.Empty).Replace("MSFT_", String.Empty)
    End Function
End Module
