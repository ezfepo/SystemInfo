Imports System.Globalization
Imports System.Management

''' <summary>
''' Readers for inventory data that WMI does not offer as a class: installed programs, the real Windows
''' release and the Secure Boot state. They read the registry through <see cref="StdRegistry"/>, so they work
''' on remote computers too.
''' </summary>
Friend Module InventoryReaders
    Private Const UninstallPath As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
    Private Const UninstallPath32Bit As String = "SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    Private Const CurrentVersionPath As String = "SOFTWARE\Microsoft\Windows NT\CurrentVersion"
    Private Const SecureBootStatePath As String = "SYSTEM\CurrentControlSet\Control\SecureBoot\State"
    Private Const Windows11FirstBuild As Integer = 22000

    ''' <summary>
    ''' Programs listed in Programs and Features: the 64-bit and the 32-bit Uninstall keys of the computer.
    ''' Updates, system components and entries without a name are left out. Win32_Product is deliberately not
    ''' used: it is slow and triggers a repair check of every MSI package. Programs installed only for a user
    ''' who is not logged on are not visible from HKEY_LOCAL_MACHINE.
    ''' </summary>
    Friend Function ReadInstalledSoftware(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oPrograms As New List(Of SyntheticObject)
        Dim oSeen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Using oRegistry As New StdRegistry(pTarget)
            For Each oBasePath As String In {UninstallPath, UninstallPath32Bit}
                For Each oKey As String In oRegistry.EnumKey(oBasePath)
                    If pCancel() Then Return oPrograms

                    Dim oPath As String = oBasePath + "\" + oKey
                    Dim oName As String = oRegistry.GetString(oPath, "DisplayName")
                    If String.IsNullOrWhiteSpace(oName) Then Continue For

                    Dim oSystemComponent As Long? = oRegistry.GetDWord(oPath, "SystemComponent")
                    If oSystemComponent.HasValue AndAlso oSystemComponent.Value = 1 Then Continue For
                    If Not String.IsNullOrEmpty(oRegistry.GetString(oPath, "ParentKeyName")) Then Continue For 'an update of another program

                    Dim oVersion As String = oRegistry.GetString(oPath, "DisplayVersion")
                    If Not oSeen.Add(oName.Trim() + "|" + If(oVersion, String.Empty)) Then Continue For 'listed in both views

                    Dim oProgram As New SyntheticObject(oName.Trim())
                    oProgram.Add("DisplayName", oName)
                    oProgram.Add("DisplayVersion", oVersion)
                    oProgram.Add("Publisher", oRegistry.GetString(oPath, "Publisher"))
                    oProgram.Add("InstallDate", FormatInstallDate(oRegistry.GetString(oPath, "InstallDate")))
                    Dim oSizeKB As Long? = oRegistry.GetDWord(oPath, "EstimatedSize")
                    If oSizeKB.HasValue AndAlso oSizeKB.Value > 0 Then oProgram.Add("EstimatedSizeKB", oSizeKB.Value.ToString(CultureInfo.InvariantCulture))
                    oPrograms.Add(oProgram)
                Next
            Next
        End Using

        oPrograms.Sort(Function(a, b) String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase))
        Return oPrograms
    End Function

    ''' <summary>InstallDate is stored as yyyymmdd text.</summary>
    Private Function FormatInstallDate(ByVal pValue As String) As String
        Dim oDate As Date
        If Not pValue Is Nothing AndAlso Date.TryParseExact(pValue.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oDate) Then
            Return oDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        End If
        Return pValue
    End Function

    ''' <summary>
    ''' The Windows release as the user knows it (for example "Windows 11 Enterprise 25H2, build 26300.9457").
    ''' Win32_OperatingSystem lacks the feature update name, and the ProductName value still says
    ''' "Windows 10" on Windows 11, so it is corrected from the build number.
    ''' </summary>
    Friend Function ReadWindowsRelease(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)

        Using oRegistry As New StdRegistry(pTarget)
            Dim oProductName As String = oRegistry.GetString(CurrentVersionPath, "ProductName")
            Dim oBuildText As String = oRegistry.GetString(CurrentVersionPath, "CurrentBuild")
            Dim oUpdateRevision As Long? = oRegistry.GetDWord(CurrentVersionPath, "UBR")
            If String.IsNullOrWhiteSpace(oProductName) AndAlso String.IsNullOrWhiteSpace(oBuildText) Then Return oResult

            Dim oBuild As Integer
            Integer.TryParse(oBuildText, oBuild)
            If oBuild >= Windows11FirstBuild AndAlso Not oProductName Is Nothing Then oProductName = oProductName.Replace("Windows 10", "Windows 11")

            Dim oFullBuild As String = oBuildText
            If oUpdateRevision.HasValue Then oFullBuild += "." + oUpdateRevision.Value.ToString(CultureInfo.InvariantCulture)

            Dim oRelease As New SyntheticObject("Windows")
            oRelease.Add("Caption", oProductName)
            oRelease.Add("DisplayVersion", oRegistry.GetString(CurrentVersionPath, "DisplayVersion"))
            oRelease.Add("Build", oFullBuild)
            oRelease.Add("Edition", oRegistry.GetString(CurrentVersionPath, "EditionID"))
            oRelease.Add("InstallationType", oRegistry.GetString(CurrentVersionPath, "InstallationType"))
            oResult.Add(oRelease)
        End Using

        Return oResult
    End Function

    ''' <summary>
    ''' Firmware type and Secure Boot. The UEFISecureBootEnabled value only exists on UEFI computers (it is
    ''' 0 when Secure Boot is off), so a missing value means legacy BIOS.
    ''' </summary>
    Friend Function ReadBootSecurity(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)

        Using oRegistry As New StdRegistry(pTarget)
            Dim oSecureBoot As Long? = oRegistry.GetDWord(SecureBootStatePath, "UEFISecureBootEnabled")

            Dim oBoot As New SyntheticObject("Boot")
            If oSecureBoot.HasValue Then
                oBoot.Add("FirmwareType", "UEFI")
                oBoot.Add("SecureBoot", If(oSecureBoot.Value = 1, "Enabled", "Disabled"))
            Else
                oBoot.Add("FirmwareType", "Legacy BIOS (or not reported)")
                oBoot.Add("SecureBoot", "Not supported")
            End If
            oResult.Add(oBoot)
        End Using

        Return oResult
    End Function

#Region "Battery, administrators, Office, services and Windows 11"
    Private Const Windows11MinimumRamBytes As Long = 4L * 1024 * 1024 * 1024
    Private Const Windows11MinimumDiskBytes As Long = 64L * 1024 * 1024 * 1024
    Private Const AdministratorsSid As String = "S-1-5-32-544" 'well-known SID: the group name is localized ("Administradores")

    ''' <summary>
    ''' Runs a WQL query on the target and returns the asked properties of each object, or Nothing when the
    ''' query could not run (namespace missing on this Windows edition, access denied...). An empty list
    ''' means the query ran and found nothing.
    ''' </summary>
    Private Function Query(ByVal pTarget As RemoteTarget, ByVal pNamespace As String, ByVal pQuery As String, ByVal ParamArray pProperties As String()) As List(Of Dictionary(Of String, Object))
        Try
            Dim oScope As ManagementScope = pTarget.CreateScope(pNamespace)
            oScope.Connect()

            Dim oOptions As New EnumerationOptions
            oOptions.Timeout = TimeSpan.FromSeconds(60)

            Dim oRows As New List(Of Dictionary(Of String, Object))
            Using oSearcher As New ManagementObjectSearcher(oScope, New ObjectQuery(pQuery), oOptions)
                For Each oObject As ManagementObject In oSearcher.Get
                    Using oObject
                        Dim oRow As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
                        For Each oProperty As String In pProperties
                            Try
                                oRow(oProperty) = oObject(oProperty)
                            Catch ex As ManagementException
                                oRow(oProperty) = Nothing 'the class does not have that property on this Windows version
                            End Try
                        Next
                        oRows.Add(oRow)
                    End Using
                Next
            End Using
            Return oRows
        Catch ex As Exception
            Console.WriteLine("Query failed: " + pNamespace + " / " + pQuery + Environment.NewLine + ex.Message)
            Return Nothing
        End Try
    End Function

    Private Function AsText(ByVal pValue As Object) As String
        If pValue Is Nothing Then Return String.Empty
        Dim oArray As Array = TryCast(pValue, Array)
        If Not oArray Is Nothing Then
            Dim oItems As New List(Of String)
            For Each oItem As Object In oArray
                oItems.Add(Convert.ToString(oItem, CultureInfo.InvariantCulture))
            Next
            Return String.Join(", ", oItems)
        End If
        Return Convert.ToString(pValue, CultureInfo.InvariantCulture)
    End Function

    Private Function AsLong(ByVal pValue As Object) As Long
        Dim oNumber As Long
        Long.TryParse(AsText(pValue), NumberStyles.Integer, CultureInfo.InvariantCulture, oNumber)
        Return oNumber
    End Function

    ''' <summary>
    ''' Battery health: the capacity the battery was designed for (Win32_PortableBattery) against what it can
    ''' hold now, and the charge cycles. Desktops return nothing. Batteries are matched by position.
    ''' </summary>
    Friend Function ReadBatteryHealth(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)

        Dim oDesign As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2", "SELECT Name, DesignCapacity FROM Win32_PortableBattery", "Name", "DesignCapacity")
        Dim oFull As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\wmi", "SELECT FullChargedCapacity FROM BatteryFullChargedCapacity", "FullChargedCapacity")
        Dim oCycles As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\wmi", "SELECT CycleCount FROM BatteryCycleCount", "CycleCount")
        If oFull Is Nothing OrElse oFull.Count = 0 Then Return oResult 'no battery, or the computer does not report it

        For i As Integer = 0 To oFull.Count - 1
            Dim oBattery As New SyntheticObject("Battery " + (i + 1).ToString())
            Dim oHasDesign As Boolean = Not oDesign Is Nothing AndAlso i < oDesign.Count
            Dim oDesignCapacity As Long = If(oHasDesign, AsLong(oDesign(i)("DesignCapacity")), 0)
            Dim oFullCapacity As Long = AsLong(oFull(i)("FullChargedCapacity"))

            If oHasDesign Then oBattery.Add("Name", AsText(oDesign(i)("Name")))
            If oDesignCapacity > 0 Then oBattery.Add("DesignCapacity", oDesignCapacity.ToString(CultureInfo.InvariantCulture))
            If oFullCapacity > 0 Then oBattery.Add("FullChargedCapacity", oFullCapacity.ToString(CultureInfo.InvariantCulture))
            If oDesignCapacity > 0 AndAlso oFullCapacity > 0 Then
                oBattery.Add("HealthPercent", Math.Min(100, Math.Round(100.0 * oFullCapacity / oDesignCapacity)).ToString(CultureInfo.InvariantCulture) + " %")
            End If
            If Not oCycles Is Nothing AndAlso i < oCycles.Count AndAlso AsLong(oCycles(i)("CycleCount")) > 0 Then
                oBattery.Add("CycleCount", AsText(oCycles(i)("CycleCount")))
            End If
            oResult.Add(oBattery)
        Next

        Return oResult
    End Function

    ''' <summary>
    ''' Wear, temperature and error counters of each physical disk. MSFT_StorageReliabilityCounter returns nothing when it is
    ''' queried directly (Windows only exposes it through the disk it belongs to), so it is read by association from
    ''' MSFT_PhysicalDisk. Needs administrator rights; many drives (HDD, some USB) report no counters.
    ''' </summary>
    Friend Function ReadDiskReliability(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)
        Const oNamespace As String = "root\Microsoft\Windows\Storage"

        Dim oDisks As List(Of Dictionary(Of String, Object)) = Query(pTarget, oNamespace, "SELECT FriendlyName, ObjectId FROM MSFT_PhysicalDisk", "FriendlyName", "ObjectId")
        If oDisks Is Nothing Then Return oResult

        For Each oDisk As Dictionary(Of String, Object) In oDisks
            If pCancel() Then Exit For

            Dim oObjectId As String = AsText(oDisk("ObjectId")).Replace("\", "\\")
            If oObjectId.Length = 0 Then Continue For

            Dim oCounters As List(Of Dictionary(Of String, Object)) = Query(pTarget, oNamespace,
                "ASSOCIATORS OF {MSFT_PhysicalDisk.ObjectId=""" + oObjectId.Replace("""", "\""") + """} WHERE ResultClass = MSFT_StorageReliabilityCounter",
                "Wear", "Temperature", "PowerOnHours", "ReadErrorsUncorrected", "WriteErrorsUncorrected")
            If oCounters Is Nothing Then Continue For

            For Each oCounter As Dictionary(Of String, Object) In oCounters
                Dim oItem As New SyntheticObject(AsText(oDisk("FriendlyName")))
                For Each oName As String In oCounter.Keys
                    oItem.Add(oName, AsText(oCounter(oName)))
                Next
                oResult.Add(oItem)
            Next
        Next

        Return oResult
    End Function

    ''' <summary>
    ''' Members of the local Administrators group (users, groups and accounts), whatever the language of Windows.
    ''' Limitation: WMI only returns the members Windows can resolve to a name. Azure AD accounts of a computer
    ''' joined to Azure AD (S-1-12-1-...) are not returned, so the list can be incomplete there.
    ''' </summary>
    Friend Function ReadLocalAdministrators(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)

        Dim oGroups As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2", "SELECT Name, Domain FROM Win32_Group WHERE SID='" + AdministratorsSid + "'", "Name", "Domain")
        If oGroups Is Nothing OrElse oGroups.Count = 0 Then Return oResult

        Dim oGroupName As String = AsText(oGroups(0)("Name")).Replace("'", "\'")
        Dim oGroupDomain As String = AsText(oGroups(0)("Domain")).Replace("'", "\'")
        Dim oMembers As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2",
            "ASSOCIATORS OF {Win32_Group.Domain='" + oGroupDomain + "',Name='" + oGroupName + "'} WHERE AssocClass=Win32_GroupUser Role=GroupComponent",
            "Name", "Domain", "SID", "__CLASS", "Disabled")
        If oMembers Is Nothing Then Return oResult

        For Each oMember As Dictionary(Of String, Object) In oMembers
            Dim oName As String = AsText(oMember("Domain")) + "\" + AsText(oMember("Name"))
            Dim oEntry As New SyntheticObject(oName)
            oEntry.Add("Name", oName)
            oEntry.Add("Type", AsText(oMember("__CLASS")).Replace("Win32_", String.Empty))
            oEntry.Add("Disabled", AsText(oMember("Disabled")))
            oEntry.Add("SID", AsText(oMember("SID")))
            oResult.Add(oEntry)
        Next

        oResult.Sort(Function(a, b) String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase))
        Return oResult
    End Function

    ''' <summary>Microsoft 365 / Office installed with Click-to-Run: products, version and update channel.</summary>
    Friend Function ReadOfficeClickToRun(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Const ConfigurationPath As String = "SOFTWARE\Microsoft\Office\ClickToRun\Configuration"
        Dim oResult As New List(Of SyntheticObject)

        Using oRegistry As New StdRegistry(pTarget)
            Dim oProducts As String = oRegistry.GetString(ConfigurationPath, "ProductReleaseIds")
            If String.IsNullOrWhiteSpace(oProducts) Then Return oResult 'Office is not installed, or it is the old MSI kind

            Dim oChannel As String = oRegistry.GetString(ConfigurationPath, "UpdateChannel")
            If String.IsNullOrWhiteSpace(oChannel) Then oChannel = oRegistry.GetString(ConfigurationPath, "CDNBaseUrl")

            Dim oOffice As New SyntheticObject("Office")
            oOffice.Add("ProductReleaseIds", oProducts)
            oOffice.Add("Version", oRegistry.GetString(ConfigurationPath, "VersionToReport"))
            oOffice.Add("Platform", oRegistry.GetString(ConfigurationPath, "Platform"))
            oOffice.Add("UpdateChannel", DescribeOfficeChannel(oChannel))
            oResult.Add(oOffice)
        End Using

        Return oResult
    End Function

    ''' <summary>The update channel is stored as the URL of the content delivery network; its last segment is a well-known GUID.</summary>
    Private Function DescribeOfficeChannel(ByVal pUrl As String) As String
        If String.IsNullOrWhiteSpace(pUrl) Then Return String.Empty
        If pUrl.IndexOf("492350f6-3a01-4f97-b9c0-c7c6ddf67d60", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "Current Channel"
        If pUrl.IndexOf("55336b82-a18d-4dd6-b5f6-9e5095c314a6", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "Monthly Enterprise Channel"
        If pUrl.IndexOf("7ffbc6bf-bc32-4f92-8982-f9dd17fd3114", StringComparison.OrdinalIgnoreCase) >= 0 Then Return "Semi-Annual Enterprise Channel"
        Return pUrl
    End Function

    ''' <summary>
    ''' Services that do not run from the Windows folder: remote-access tools, backup and management agents,
    ''' vendor services. The hundreds of Windows services are left out.
    ''' </summary>
    Friend Function ReadThirdPartyServices(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)

        Dim oServices As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2", "SELECT Name, DisplayName, State, StartMode, PathName, StartName FROM Win32_Service",
                                                                         "Name", "DisplayName", "State", "StartMode", "PathName", "StartName")
        If oServices Is Nothing Then Return oResult

        For Each oService As Dictionary(Of String, Object) In oServices
            Dim oPath As String = AsText(oService("PathName"))
            If oPath.Length = 0 Then Continue For
            If oPath.IndexOf("\windows\", StringComparison.OrdinalIgnoreCase) >= 0 Then Continue For
            If oPath.IndexOf("\windows defender\", StringComparison.OrdinalIgnoreCase) >= 0 Then Continue For
            If oPath.IndexOf("\windowsapps\", StringComparison.OrdinalIgnoreCase) >= 0 Then Continue For

            Dim oDisplayName As String = AsText(oService("DisplayName"))
            Dim oEntry As New SyntheticObject(If(oDisplayName.Length > 0, oDisplayName, AsText(oService("Name"))))
            oEntry.Add("ServiceName", AsText(oService("Name")))
            oEntry.Add("DisplayName", oDisplayName)
            oEntry.Add("State", AsText(oService("State")))
            oEntry.Add("StartMode", AsText(oService("StartMode")))
            oEntry.Add("Account", AsText(oService("StartName")))
            oEntry.Add("Path", oPath)
            oResult.Add(oEntry)
        Next

        oResult.Sort(Function(a, b) String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase))
        Return oResult
    End Function

    ''' <summary>
    ''' Whether the computer meets the Windows 11 requirements: memory, system drive size, 64-bit processor
    ''' with 2 cores at 1 GHz, UEFI with Secure Boot, TPM 2.0 and a processor model in Microsoft's list
    ''' (see <see cref="ProcessorSupport"/>). Items that can't be checked (TPM without administrator rights,
    ''' Xeon/Celeron/Pentium models) are reported as still to check, never as a pass.
    ''' </summary>
    Friend Function ReadWindows11Readiness(ByVal pTarget As RemoteTarget, ByVal pCancel As Func(Of Boolean)) As List(Of SyntheticObject)
        Dim oResult As New List(Of SyntheticObject)

        Dim oSystem As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2", "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem", "TotalPhysicalMemory")
        Dim oOperatingSystem As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2", "SELECT SystemDrive, BuildNumber FROM Win32_OperatingSystem", "SystemDrive", "BuildNumber")
        Dim oProcessor As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2", "SELECT Name, AddressWidth, NumberOfCores, MaxClockSpeed FROM Win32_Processor", "Name", "AddressWidth", "NumberOfCores", "MaxClockSpeed")
        If oSystem Is Nothing OrElse oSystem.Count = 0 OrElse oOperatingSystem Is Nothing OrElse oOperatingSystem.Count = 0 OrElse oProcessor Is Nothing OrElse oProcessor.Count = 0 Then Return oResult

        Dim oChecks As New List(Of KeyValuePair(Of String, String)) 'the details, shown after the verdict
        Dim oProblems As New List(Of String)
        Dim oPending As New List(Of String)

        Dim oRam As Long = AsLong(oSystem(0)("TotalPhysicalMemory"))
        oChecks.Add(New KeyValuePair(Of String, String)("Memory", FormatGigabytes(oRam) + " (minimum 4 GB)"))
        If oRam < Windows11MinimumRamBytes Then oProblems.Add("memory")

        Dim oSystemDrive As String = AsText(oOperatingSystem(0)("SystemDrive"))
        Dim oDisks As List(Of Dictionary(Of String, Object)) = Nothing
        If oSystemDrive.Length > 0 Then oDisks = Query(pTarget, "root\CIMV2", "SELECT Size FROM Win32_LogicalDisk WHERE DeviceID='" + oSystemDrive + "'", "Size")
        If Not oDisks Is Nothing AndAlso oDisks.Count > 0 Then
            Dim oSize As Long = AsLong(oDisks(0)("Size"))
            oChecks.Add(New KeyValuePair(Of String, String)("Storage", FormatGigabytes(oSize) + " on " + oSystemDrive + " (minimum 64 GB)"))
            If oSize < Windows11MinimumDiskBytes Then oProblems.Add("storage")
        Else
            oPending.Add("storage")
        End If

        Dim oIs64Bit As Boolean = AsLong(oProcessor(0)("AddressWidth")) = 64
        Dim oCores As Long = AsLong(oProcessor(0)("NumberOfCores"))
        Dim oMegahertz As Long = AsLong(oProcessor(0)("MaxClockSpeed"))
        oChecks.Add(New KeyValuePair(Of String, String)("Processor", If(oIs64Bit, "64-bit", "32-bit") + ", " + oCores.ToString() + " cores, " + (oMegahertz / 1000.0).ToString("0.0", CultureInfo.InvariantCulture) + " GHz (minimum 64-bit, 2 cores, 1 GHz)"))
        If Not oIs64Bit OrElse oCores < 2 OrElse oMegahertz < 1000 Then oProblems.Add("processor")

        'The model is checked against Microsoft's list of supported processors, embedded in the executable
        Dim oModelCheck As ProcessorSupport.Result = ProcessorSupport.Check(AsText(oProcessor(0)("Name")))
        Select Case oModelCheck.Verdict
            Case ProcessorSupport.VerdictEnum.Supported
                oChecks.Add(New KeyValuePair(Of String, String)("ProcessorModel", "Supported: " + oModelCheck.Detail))
            Case ProcessorSupport.VerdictEnum.SupportedByGeneration
                oChecks.Add(New KeyValuePair(Of String, String)("ProcessorModel", "Supported (newer than the list): " + oModelCheck.Detail))
            Case ProcessorSupport.VerdictEnum.NotSupported
                oChecks.Add(New KeyValuePair(Of String, String)("ProcessorModel", "Not supported: " + oModelCheck.Detail))
                oProblems.Add("processor model")
            Case Else
                oChecks.Add(New KeyValuePair(Of String, String)("ProcessorModel", oModelCheck.Detail))
                oPending.Add("processor model")
        End Select

        Using oRegistry As New StdRegistry(pTarget)
            Dim oSecureBoot As Long? = oRegistry.GetDWord(SecureBootStatePath, "UEFISecureBootEnabled")
            If oSecureBoot.HasValue Then
                oChecks.Add(New KeyValuePair(Of String, String)("Firmware", "UEFI, Secure Boot " + If(oSecureBoot.Value = 1, "enabled", "off (the firmware supports it)")))
            Else
                oChecks.Add(New KeyValuePair(Of String, String)("Firmware", "Legacy BIOS: UEFI with Secure Boot is required"))
                oProblems.Add("firmware")
            End If
        End Using

        Dim oTpm As List(Of Dictionary(Of String, Object)) = Query(pTarget, "root\CIMV2\Security\MicrosoftTpm", "SELECT SpecVersion FROM Win32_Tpm", "SpecVersion")
        If oTpm Is Nothing Then
            oChecks.Add(New KeyValuePair(Of String, String)("TPM", "Unknown (reading it needs administrator rights)"))
            oPending.Add("TPM 2.0")
        ElseIf oTpm.Count = 0 Then
            oChecks.Add(New KeyValuePair(Of String, String)("TPM", "Not found (TPM 2.0 is required)"))
            oProblems.Add("TPM")
        ElseIf AsText(oTpm(0)("SpecVersion")).StartsWith("2.0", StringComparison.Ordinal) Then
            oChecks.Add(New KeyValuePair(Of String, String)("TPM", "2.0"))
        Else
            oChecks.Add(New KeyValuePair(Of String, String)("TPM", AsText(oTpm(0)("SpecVersion")) + " (TPM 2.0 is required)"))
            oProblems.Add("TPM")
        End If

        Dim oVerdict As String
        If AsLong(oOperatingSystem(0)("BuildNumber")) >= Windows11FirstBuild Then
            oVerdict = "Already running Windows 11"
        ElseIf oProblems.Count > 0 Then
            oVerdict = "Does not meet: " + String.Join(", ", oProblems)
        ElseIf oPending.Count > 0 Then
            oVerdict = "No blocker found; still to check: " + String.Join(", ", oPending)
        Else
            oVerdict = "Meets the requirements"
        End If

        Dim oReadiness As New SyntheticObject("Windows 11")
        oReadiness.Add("Result", oVerdict)
        For Each oCheck As KeyValuePair(Of String, String) In oChecks
            oReadiness.Add(oCheck.Key, oCheck.Value)
        Next
        oResult.Add(oReadiness)
        Return oResult
    End Function

    Private Function FormatGigabytes(ByVal pBytes As Long) As String
        Return (pBytes / 1024.0 / 1024.0 / 1024.0).ToString("0.#", CultureInfo.InvariantCulture) + " GB"
    End Function
#End Region

End Module
