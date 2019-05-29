Imports System.Management
Imports System.Math
Imports Microsoft.Win32
Imports System.Text

Public Class WMINamespace
    Inherits List(Of WMIClass)

    Private _Name As String
    Private _XmlNamespace As XmlDocument
    Private _ProcessDate As DateTime
    Private _FileName As String

    Public Event DoCancel(ByRef pCancel As Boolean)

    Public Sub New(ByVal pXmlNamespace As Object)
        _ProcessDate = Now
        _Name = "root\CIMV2"

        _XmlNamespace = TryCast(pXmlNamespace, XmlDocument)
        If Not _XmlNamespace Is Nothing Then Console.WriteLine(_Name + " Classes not loaded")
    End Sub
    Public ReadOnly Property Cancel() As Boolean
        Get
            Dim oDoCancel As Boolean
            RaiseEvent DoCancel(oDoCancel)
            Return oDoCancel
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _Name
        End Get
    End Property
    Friend ReadOnly Property XmlNamespace() As XmlDocument
        Get
            Return _XmlNamespace
        End Get
    End Property
    Public ReadOnly Property FileName() As String
        Get
            Dim oFriendlyName As String = AppDomain.CurrentDomain.FriendlyName
            oFriendlyName = oFriendlyName.Replace(".exe", String.Empty)
            Dim oFolder As String = AppConfiguration.OutputFolder
            If Not oFolder.EndsWith(IO.Path.DirectorySeparatorChar) Then oFolder += IO.Path.DirectorySeparatorChar
            Return oFolder + oFriendlyName + "." + System.Environment.MachineName + "." + _ProcessDate.ToString("yyyyMMdd hhmmss") + ".xml"
        End Get
    End Property

    Public Sub Load()
        For Each oChild As Object In _XmlNamespace.DocumentElement.ChildNodes
            If Cancel Then Exit For

            Dim oXmlClass As XmlElement = TryCast(oChild, XmlElement)
            Try
                If Not oXmlClass Is Nothing Then MyBase.Add(New WMIClass(Me, oXmlClass))
            Catch ex As Exception
                Console.WriteLine("Class: " + oXmlClass.Name + Environment.NewLine + ex.ToString)
            End Try
        Next
    End Sub
    Public Sub Save()
        If Not _XmlNamespace Is Nothing Then
            Try
                _XmlNamespace.Save(Me.FileName)
            Catch ex As Exception
                Console.WriteLine("FileName: " + Me.FileName + Environment.NewLine + ex.ToString)
            End Try
        End If
    End Sub
End Class

Public Class WMIClass
    Inherits List(Of WMIManagementObject)

    Private _WMINamespace As WMINamespace
    Private _XmlClass As XmlElement

    Public Sub New(ByRef pWMINamespace As WMINamespace, ByRef pXmlClass As XmlElement)
        _WMINamespace = pWMINamespace
        _XmlClass = pXmlClass
    End Sub
    Public ReadOnly Property Cancel() As Boolean
        Get
            Return _WMINamespace.Cancel
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _XmlClass.Name
        End Get
    End Property
    Friend ReadOnly Property XmlClass() As XmlElement
        Get
            Return _XmlClass
        End Get
    End Property

    Public Sub Load()
        If Me.Name = "Win32_Monitor" Then

            Try
                Dim oMonitors As List(Of WMIExtMonitor) = WMIExtMonitor.GetMonitors
                For Each oMonitor As WMIExtMonitor In oMonitors
                    If _WMINamespace.Cancel Then Exit For

                    Try
                        MyBase.Add(New WMIManagementObject(Me, oMonitor))
                    Catch ex As Exception
                        Console.WriteLine("ManagementObject: " + oMonitor.ID + "\" + oMonitor.PNPDeviceID + Environment.NewLine + ex.ToString)
                    End Try
                Next

            Catch ex As Exception
                Console.WriteLine("WMIExtMonitor.GetMonitors: " + Environment.NewLine + ex.ToString)
            End Try

        Else
            Dim oManagementObjectSearcher As ManagementObjectSearcher
            Dim oManagementScope As ManagementScope
            Dim oobjectquery As ObjectQuery
            Dim queryString As String = "SELECT * FROM " + Me.Name

            Try
                oManagementScope = New ManagementScope(_WMINamespace.Name)
                oManagementScope.Connect()
                oobjectquery = New ObjectQuery(queryString)
                oManagementObjectSearcher = New ManagementObjectSearcher(oManagementScope, oobjectquery)

                For Each oManagementObject As ManagementObject In oManagementObjectSearcher.Get
                    If _WMINamespace.Cancel Then Exit For

                    Try
                        MyBase.Add(New WMIManagementObject(Me, oManagementObject))
                    Catch ex As Exception
                        Console.WriteLine("ManagementObject: " + oManagementObject.Path.RelativePath + Environment.NewLine + ex.ToString)
                    End Try
                Next

            Catch ex As Exception
                Console.WriteLine("QueryString: " + queryString + Environment.NewLine + ex.ToString)
            End Try
        End If
    End Sub
End Class

Public Class WMIManagementObject
    Inherits List(Of WMIPropertyData)

    Private _WMIClass As WMIClass
    Private _ManagementObject As ManagementObject
    Private _Monitor As WMIExtMonitor
    Private _XmlManagementObject As XmlElement

    Public Sub New(ByRef pWMIClass As WMIClass, ByVal pMonitor As WMIExtMonitor)
        _WMIClass = pWMIClass
        _Monitor = pMonitor

        _XmlManagementObject = _WMIClass.XmlClass.OwnerDocument.CreateElement("Object")
        _WMIClass.XmlClass.AppendChild(_XmlManagementObject)
    End Sub
    Public Sub New(ByRef pWMIClass As WMIClass, ByVal pManagementObject As ManagementObject)
        _WMIClass = pWMIClass
        _ManagementObject = pManagementObject

        _XmlManagementObject = _WMIClass.XmlClass.OwnerDocument.CreateElement("Object")
        _WMIClass.XmlClass.AppendChild(_XmlManagementObject)
    End Sub
    Public ReadOnly Property Cancel() As Boolean
        Get
            Return _WMIClass.Cancel
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            If Not _Monitor Is Nothing Then
                Return _Monitor.FullName
            ElseIf Not _ManagementObject Is Nothing Then
                Return _ManagementObject.Path.RelativePath
            Else
                Throw New NotImplementedException
            End If
        End Get
    End Property
    Friend ReadOnly Property XmlManagementObject() As XmlElement
        Get
            Return _XmlManagementObject
        End Get
    End Property

    Public Sub Load()
        If Not _Monitor Is Nothing Then

            MyBase.Add(New WMIPropertyData(Me, "ID", _Monitor.ID))
            MyBase.Add(New WMIPropertyData(Me, "Device", _Monitor.Device))
            MyBase.Add(New WMIPropertyData(Me, "ManufacturerDate", _Monitor.ManufacturerDate))
            MyBase.Add(New WMIPropertyData(Me, "ManufacturerID", _Monitor.ManufacturerID))
            MyBase.Add(New WMIPropertyData(Me, "Name", _Monitor.Name))
            MyBase.Add(New WMIPropertyData(Me, "PNPDeviceID", _Monitor.PNPDeviceID))
            MyBase.Add(New WMIPropertyData(Me, "ProductID", _Monitor.ProductID))
            MyBase.Add(New WMIPropertyData(Me, "Serial", _Monitor.Serial))
            MyBase.Add(New WMIPropertyData(Me, "SerialNumber", _Monitor.SerialNumber))

        ElseIf Not _ManagementObject Is Nothing Then

            For Each oPropertyData As PropertyData In _ManagementObject.Properties
                If _WMIClass.Cancel Then Exit For

                If oPropertyData.Name <> "SystemCreationClassName" And oPropertyData.Name <> "SystemName" Then

                    Try
                        MyBase.Add(New WMIPropertyData(Me, oPropertyData))
                    Catch ex As Exception
                        Console.WriteLine("PropertyData: " + oPropertyData.Name + Environment.NewLine + ex.ToString)
                    End Try

                End If
            Next

        Else
            Throw New NotImplementedException
        End If
    End Sub
End Class

Public Class WMIPropertyData
    Private _WMIManagementObject As WMIManagementObject
    Private _PropertyData As PropertyData
    Private _Name As String
    Private _Value As String

    Public Sub New(ByRef pWMIManagementObject As WMIManagementObject, ByVal pName As String, ByVal pValue As String)
        _WMIManagementObject = pWMIManagementObject
        _Name = pName
        _Value = pValue
    End Sub
    Public Sub New(ByRef pWMIManagementObject As WMIManagementObject, ByVal pPropertyData As PropertyData)
        _WMIManagementObject = pWMIManagementObject
        _PropertyData = pPropertyData
    End Sub
    Public ReadOnly Property Cancel() As Boolean
        Get
            Return _WMIManagementObject.Cancel
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            If _PropertyData Is Nothing Then
                Return _Name
            Else
                Return _PropertyData.Name
            End If
        End Get
    End Property
    Public ReadOnly Property Value() As String
        Get
            Dim oPropertyDataValue As String = String.Empty

            If _PropertyData Is Nothing Then
                oPropertyDataValue = _Value
            Else
                If _PropertyData.IsArray Then
                    oPropertyDataValue = PropertyDataToString()
                Else
                    oPropertyDataValue = CStr(_PropertyData.Value)
                End If
            End If

            Return oPropertyDataValue
        End Get
    End Property

    Public Sub Load()
        Try
            _WMIManagementObject.XmlManagementObject.SetAttribute(Me.Name, Me.Value)
        Catch ex As Exception
            Console.WriteLine("PropertyData: " + Me.Name + Environment.NewLine + ex.ToString)
        End Try
    End Sub

    Private Function PropertyDataToString() As String
        Dim pValueArray As System.Array = CType(_PropertyData.Value, System.Array)
        Dim pValueString As String = String.Empty

        If Not pValueArray Is Nothing Then
            For Each pData As Object In pValueArray
                pValueString += pData.ToString + ", "
            Next

            If pValueString.Length > 0 Then pValueString = pValueString.Substring(0, pValueString.Length - 2)
        End If

        Return pValueString
    End Function
End Class

Public Class WMIViewer
    Private _XmlSystemInfo As XmlDocument
    Private _XmlConfigurationType As XmlDocument
    Private _FileName As String
    Private _ConfigurationType As ConfigurationTypeEnum

    Public Property ConfigurationType() As ConfigurationTypeEnum
        Get
            Return _ConfigurationType
        End Get
        Set(ByVal value As ConfigurationTypeEnum)
            _ConfigurationType = value

            Select Case _ConfigurationType
                Case ConfigurationTypeEnum.Short : _XmlConfigurationType = modConfigurations.Short
                Case ConfigurationTypeEnum.Resume : _XmlConfigurationType = modConfigurations.Resume
                Case ConfigurationTypeEnum.Complete : _XmlConfigurationType = modConfigurations.Complete
            End Select
        End Set
    End Property

    Public Function Load(ByVal pFileName As String) As Boolean
        Dim pLoaded As Boolean
        Try
            Dim oFileInfo As New IO.FileInfo(pFileName)
            If oFileInfo.Exists Then
                _XmlSystemInfo = New XmlDocument
                _XmlSystemInfo.Load(pFileName)
                pLoaded = _XmlSystemInfo.DocumentElement.Name = "SystemInfo"
                If pLoaded Then _FileName = oFileInfo.Name
            End If
        Catch ex As Exception
        End Try
        Return pLoaded
    End Function
    Public Sub Fill(ByRef tvw As TreeView)
        tvw.Nodes.Clear()
        Dim oNodeRoot As TreeNode = tvw.Nodes.Add("root", _FileName)

        For Each oChild As Object In _XmlConfigurationType.DocumentElement.ChildNodes
            Dim oXmlClass As XmlElement = TryCast(oChild, XmlElement)
            If Not oXmlClass Is Nothing Then
                Dim oXmlSystemInfoNode As XmlNode = _XmlSystemInfo.DocumentElement.SelectSingleNode(oXmlClass.Name)
                If Not oXmlSystemInfoNode Is Nothing Then
                    If oXmlSystemInfoNode.HasChildNodes Then
                        Dim oTreeNode As TreeNode = oNodeRoot.Nodes.Add(oXmlClass.Name, oXmlClass.Name.Replace("Win32_", "") + " (" + oXmlSystemInfoNode.ChildNodes.Count.ToString + ")")
                        oTreeNode.Tag = oXmlSystemInfoNode
                    End If
                End If
            End If
        Next

        'For Each oChild As Object In _XmlSystemInfo.DocumentElement.ChildNodes
        '    Dim oXmlElement As XmlElement = TryCast(oChild, XmlElement)
        '    If Not oXmlElement Is Nothing Then
        '        If oXmlElement.HasChildNodes Then oNodeRoot.Nodes.Add(oXmlElement.Name, oXmlElement.Name)
        '    End If
        'Next

        oNodeRoot.ExpandAll()
    End Sub
    Public Sub Fill(ByRef lvw As ListView, ByRef oTreeNode As TreeNode)
        If oTreeNode.Tag Is Nothing Then Exit Sub

        lvw.Items.Clear()
        lvw.Groups.Clear()
        lvw.ShowGroups = True

        Dim oXmlSystemInfoElement As XmlElement = TryCast(oTreeNode.Tag, XmlElement)
        Dim oXmlClass As XmlNode = _XmlConfigurationType.DocumentElement.SelectSingleNode(oXmlSystemInfoElement.Name)

        If Not oXmlClass Is Nothing Then
            For Each oChild As Object In oXmlSystemInfoElement.ChildNodes
                Dim oXmlChild As XmlElement = TryCast(oChild, XmlElement)
                If Not oXmlChild Is Nothing Then

                    Dim oListViewGroup As ListViewGroup = lvw.Groups.Add(oXmlChild.Name + " " + lvw.Groups.Count.ToString, oXmlChild.Name + " " + lvw.Groups.Count.ToString)

                    If oXmlClass.Attributes.Count = 0 Then
                        For Each oXmlAttribute As XmlAttribute In oXmlChild.Attributes
                            Fill(lvw, oXmlAttribute, oListViewGroup, oXmlClass.Name)
                        Next
                    Else
                        For Each oXmlAttribute As XmlAttribute In oXmlClass.Attributes
                            Dim oXmlChildAttribute As XmlAttribute = oXmlChild.GetAttributeNode(oXmlAttribute.Name)
                            If Not oXmlChildAttribute Is Nothing Then Fill(lvw, oXmlChildAttribute, oListViewGroup, oXmlClass.Name)
                        Next
                    End If
                End If
            Next
        End If

        lvw.ShowGroups = True
    End Sub
    Public Function [Resume]() As String
        Dim oResume As String = String.Empty

        oResume += GetXMLClassResume("SystemEnclosure", "ChassisTypes") + " " + GetXMLClassResume("ComputerSystem", "Name") + " " + GetXMLClassResume("ComputerSystem", "SystemType") + Environment.NewLine
        oResume += GetXMLClassResume("ComputerSystem", "Manufacturer", "SystemFamily", "SystemSKUNumber") + Environment.NewLine
        oResume += "Motherboard: " + GetXMLClassResume("BaseBoard", "Manufacturer", "Product", "Version") + Environment.NewLine
        oResume += "CPU: " + GetXMLClassResume("Processor", "Manufacturer", "Architecture", "Name", "Description", "NunberOfCores", "MaxClockSpeed", "NumberOfLogicalProcessors", "SocketDesignation") + Environment.NewLine
        oResume += "Cache: " + GetXMLClassResume("CacheMemory", "Level", "CacheType", "Purpose", "InstalledSize") + Environment.NewLine
        oResume += "RAM: " + GetXMLClassResume("PhysicalMemory", "Manufacturer", "FormFactor", "Capacity", "Speed", "MinVoltage", "PartNumber") + Environment.NewLine
        oResume += "Video: " + GetXMLClassResume("VideoController", "AdapterRAM", "Name", "AdapterDACType", "VideoArcuitecture") + Environment.NewLine
        oResume += "HD: " + GetXMLClassResume("DiskDrive", "InterfaceType", "Model", "Size") + Environment.NewLine
        oResume += "CD/DVD: " + GetXMLClassResume("CDROMDrive", "Name", "MediaType") + Environment.NewLine

        Return oResume
    End Function
    Public Function GetXMLClassResume(ByVal pClass As String, ByVal ParamArray pAttributes As String()) As String
        Dim oResume As String = String.Empty

        Dim oXmlElement As XmlElement = _XmlSystemInfo.DocumentElement.SelectSingleNode("Win32_" + pClass)
        If Not oXmlElement Is Nothing Then
            Dim oXmlNodeList As XmlNodeList = oXmlElement.SelectNodes("Object")
            For Each oXmlNode As XmlNode In oXmlNodeList
                Dim oItem As String = String.Empty
                For Each pAttribute In pAttributes
                    Dim oValue As String = GetXmlNodeAttribute(oXmlNode, pAttribute)
                    If Not String.IsNullOrEmpty(oValue) Then oItem += GetValue(pAttribute, oValue, "Win32_" + pClass).Trim + " "
                Next
                If Not String.IsNullOrEmpty(oResume.Trim) Then oResume += Environment.NewLine +vbTab 
                If Not String.IsNullOrEmpty(oItem.Trim) Then oResume += oItem.Trim
            Next
        End If

        Return oResume
    End Function
    Private Function GetXmlNodeAttribute(ByRef pXml As XmlNode, ByVal pName As String) As String
        Dim oValue As String = String.Empty

        If Not pXml Is Nothing Then
            Dim oXmlAttribute As XmlAttribute = pXml.Attributes(pName)
            If Not oXmlAttribute Is Nothing Then
                oValue = oXmlAttribute.Value
            End If
        End If

        Return oValue
    End Function
    Private Function GetXmlNodeValue(ByRef pXml As XmlNode, ByVal xpath As String) As String
        Dim oValue As String = String.Empty

        If Not pXml Is Nothing Then
            Dim oXmlNodeList As XmlNodeList = pXml.SelectNodes(xpath)
            For Each oXmlNode As XmlNode In oXmlNodeList
                oValue = oXmlNode.InnerText
                Exit For
            Next
        End If

        Return oValue
    End Function
    Private Sub Fill(ByRef lvw As ListView, ByRef oXmlAttribute As XmlAttribute, ByRef oListViewGroup As ListViewGroup, ByVal pClassName As String)
        Dim pValue As String = GetValues(oXmlAttribute.Name, oXmlAttribute.Value, pClassName)

        If pValue.Length > 0 Then
            Dim oListViewItem As New ListViewItem(oXmlAttribute.Name)
            oListViewItem.SubItems.Add(pValue)
            oListViewItem.Group = oListViewGroup
            lvw.Items.Add(oListViewItem)
        End If
    End Sub
    Private Function GetValues(ByVal pName As String, ByVal pValue As String, ByVal pClassName As String) As String
        Dim aValues As String() = pValue.Split(","c)
        Dim pValues As String = String.Empty

        For Each oValue In aValues
            pValues += GetValue(pName, oValue.Trim, pClassName) + ", "
        Next

        If pValues.Length > 0 Then pValues = pValues.Substring(0, pValues.Length - 2)

        Return pValues
    End Function
    Private Function GetValue(ByVal pName As String, ByVal pValue As String, ByVal pClassName As String) As String

        Select Case pName
            Case "CurrentNumberOfColors"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue = Get1024(pSize, " colors")
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "CurrentRefreshRate", "MaxRefreshRate", "MinRefreshRate"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " Hertz"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "CurrentClockSpeed", "ExtClock", "L2CacheSpeed", "L3CacheSpeed", "MaxClockSpeed"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " MegaHertz"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "MaxBaudRate", "BaudRate", "MaxBaudRateToSerialPort"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue = Get1024(pSize, "Bits por second")
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "Speed"
                Select Case pClassName
                    Case "Win32_NetworkAdapter"
                        Dim pSize As Long
                        If Long.TryParse(pValue, pSize) Then
                            If pSize > 0 Then
                                pValue = Get1024(pSize, "Bits por second")
                            Else
                                pValue = String.Empty
                            End If
                        End If

                    Case "Win32_PhysicalMemory"
                        Dim pSize As Long
                        If Long.TryParse(pValue, pSize) Then
                            If pSize > 0 Then
                                pValue += " nanoseconds"
                            Else
                                pValue = String.Empty
                            End If
                        End If
                End Select

            Case "DesignCapacity"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " milliwatt-hours"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "DesignVoltage", "ConfiguredVoltage", "MinVoltage"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " millivolts"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "PixelsPerXLogicalInch", "PixelsPerYLogicalInch", "LogPixels", "HorizontalResolution", "VerticalResolution"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " DPI"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "ScreenHeight", "ScreenWidth", "PelsHeight", "PelsWidth", "CurrentHorizontalResolution", "CurrentVerticalResolution"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " Pixels"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "SystemStartupDelay"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " Seconds"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "EstimatedChargeRemaining"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue += " %"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "NunberOfCores"
                Dim pCores As Integer
                If Integer.TryParse(pValue, pCores) Then
                    If pCores > 0 Then
                        pValue += " Cores"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "SocketDesignation"
                Dim pCores As Integer
                If Integer.TryParse(pValue, pCores) Then
                    If pCores > 0 Then
                        pValue = +"Socket "
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "PauseAfterReset"
                Dim pMilliseconds As Long
                If Long.TryParse(pValue, pMilliseconds) Then
                    If pMilliseconds > 0 Then
                        pValue += " Milliseconds"
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "BitsPerPel", "EncryptionLevel", "DataWidth", "TotalWidth", "AddressWidth", "CurrentBitsPerPixel"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pSize > 0 Then
                        pValue = Get1024(pSize, "Bits")
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "TotalPhysicalMemory", "BlockSize", "InstalledSize", "MaxCacheSize", "BytesPerSector", "Size", "FreeSpace", "EndingAddress", "StartingAddress", "Capacity", "TransferRate", "FreePhysicalMemory", "FreeSpaceInPagingFiles", "FreeVirtualMemory", "MaxProcessMemorySize", "SizeStoredInPagingFiles", "TotalVirtualMemorySize", "TotalVisibleMemorySize", "L2CacheSize", "L3CacheSize", "AdapterRAM"
                Dim pSize As Long
                If Long.TryParse(pValue, pSize) Then
                    If pName = "InstalledSize" Or pName = "MaxCacheSize" Or pName = "EndingAddress" Or pName = "StartingAddress" Or pName = "TransferRate" Or pName = "FreePhysicalMemory" Or pName = "FreeSpaceInPagingFiles" Or pName = "FreeVirtualMemory" Or pName = "MaxProcessMemorySize" Or pName = "SizeStoredInPagingFiles" Or pName = "TotalVirtualMemorySize" Or pName = "TotalVisibleMemorySize" Or pName = "L2CacheSize" Or pName = "L3CacheSize" Then pSize = pSize * 1024

                    pValue = Get1024(pSize, "Bytes")
                End If

            Case "ProtocolSupported"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "10" : pValue = "SCSI Fibre Channel Protocol"
                    Case "11" : pValue = "SCSI Serial Bus Protocol"
                    Case "12" : pValue = "SCSI Serial Bus Protocol-2 (1394)"
                    Case "13" : pValue = "SCSI Serial Storage Architecture"
                    Case "14" : pValue = "VESA"
                    Case "15" : pValue = "PCMCIA"
                    Case "16" : pValue = "Universal Serial Bus"
                    Case "17" : pValue = "Parallel Protocol"
                    Case "18" : pValue = "ESCON"
                    Case "19" : pValue = "Diagnostic"
                    Case "2" : pValue = "Unknown"
                    Case "20" : pValue = "I2C"
                    Case "21" : pValue = "Power"
                    Case "22" : pValue = "HIPPI"
                    Case "23" : pValue = "MultiBus"
                    Case "24" : pValue = "VME"
                    Case "25" : pValue = "IPI"
                    Case "26" : pValue = "IEEE-488"
                    Case "27" : pValue = "RS232"
                    Case "28" : pValue = "IEEE 802.3 10BASE5"
                    Case "29" : pValue = "IEEE 802.3 10BASE2"
                    Case "3" : pValue = "EISA"
                    Case "30" : pValue = "IEEE 802.3 1BASE5"
                    Case "31" : pValue = "IEEE 802.3 10BROAD36"
                    Case "32" : pValue = "IEEE 802.3 100BASEVG"
                    Case "33" : pValue = "IEEE 802.5 Token-Ring"
                    Case "34" : pValue = "ANSI X3T9.5 FDDI"
                    Case "35" : pValue = "MCA"
                    Case "36" : pValue = "ESDI"
                    Case "37" : pValue = "IDE"
                    Case "38" : pValue = "CMD"
                    Case "39" : pValue = "ST506"
                    Case "4" : pValue = "ISA"
                    Case "40" : pValue = "DSSI"
                    Case "41" : pValue = "QIC2"
                    Case "42" : pValue = "Enhanced ATA/IDE"
                    Case "43" : pValue = "AGP"
                    Case "44" : pValue = "TWIRP (two-way infrared)"
                    Case "45" : pValue = "FIR (fast infrared)"
                    Case "46" : pValue = "SIR (serial infrared)"
                    Case "47" : pValue = "IrBus"
                    Case "5" : pValue = "PCI"
                    Case "6" : pValue = "ATA/ATAPI"
                    Case "7" : pValue = "Flexible Diskette"
                    Case "8" : pValue = "1496"
                    Case "9" : pValue = "SCSI Parallel Interface"
                End Select

            Case "Availability"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "10" : pValue = "Degraded"
                    Case "11" : pValue = "Not Installed"
                    Case "12" : pValue = "Install Error"
                    Case "13" : pValue = "Power Save - Unknown"
                    Case "14" : pValue = "Power Save - Low Power Mode"
                    Case "15" : pValue = "Power Save - Standby"
                    Case "16" : pValue = "Power Cycle"
                    Case "17" : pValue = "Power Save - Warning"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Running or Full Power"
                    Case "4" : pValue = "Warning"
                    Case "5" : pValue = "In Test"
                    Case "6" : pValue = "Not Applicable"
                    Case "7" : pValue = "Power Off"
                    Case "8" : pValue = "Off Line"
                    Case "9" : pValue = "Off Duty"
                End Select

            Case "BatteryStatus"
                Select Case pValue
                    Case "1" : pValue = "The battery is discharging."
                    Case "10" : pValue = "Undefined"
                    Case "11" : pValue = "Partially Charged"
                    Case "2" : pValue = "The system has access to AC so no battery is being discharged. However, the battery is not necessarily charging."
                    Case "3" : pValue = "Fully Charged"
                    Case "4" : pValue = "Low"
                    Case "5" : pValue = "Critical"
                    Case "6" : pValue = "Charging"
                    Case "7" : pValue = "Charging and High"
                    Case "8" : pValue = "Charging and Low"
                    Case "9" : pValue = "Charging and Critical"
                End Select

            Case "Chemistry"
                Select Case pValue
                    Case "3" : pValue = "Lead Acid"
                    Case "8" : pValue = "Lithium Polymer"
                    Case "6" : pValue = "Lithium-ion"
                    Case "4" : pValue = "Nickel Cadmium"
                    Case "5" : pValue = "Nickel Metal Hydride"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "7" : pValue = "Zinc air"
                End Select

            Case "Associativity"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Direct Mapped"
                    Case "4" : pValue = "2-way Set-Associative"
                    Case "5" : pValue = "4-way Set-Associative"
                    Case "6" : pValue = "Fully Associative"
                    Case "7" : pValue = "Windows Server 2003 and Windows XP:  8-way Set-Associative"
                    Case "8" : pValue = "Windows Server 2003 and Windows XP:  16-way Set-Associative"
                End Select

            Case "CacheType"
                Select Case pValue
                    Case "4" : pValue = "Data"
                    Case "3" : pValue = "Instruction"
                    Case "1" : pValue = "Other"
                    Case "5" : pValue = "Unified"
                    Case "2" : pValue = "Unknown"
                End Select

            Case "CurrentSRAM", "SupportedSRAM"
                Select Case pValue
                    Case "0" : pValue = "Other"
                    Case "1" : pValue = "Unknown"
                    Case "2" : pValue = "Non-Burst"
                    Case "3" : pValue = "Burst"
                    Case "4" : pValue = "Pipeline Burst"
                    Case "5" : pValue = "Synchronous"
                    Case "6" : pValue = "Asynchronous"
                End Select

            Case "ErrorCorrectType"
                Select Case pValue
                    Case "0" : pValue = "Reserved"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "None"
                    Case "4" : pValue = "Parity"
                    Case "5" : pValue = "Single-Bit ECC"
                    Case "6" : pValue = "Multi-Bit ECC"
                End Select

            Case "Level"
                If pClassName = "Win32_CacheMemory" Then
                    Select Case pValue
                        Case "1" : pValue = "Other"
                        Case "2" : pValue = "Unknown"
                        Case "3" : pValue = "Primary"
                        Case "4" : pValue = "Secondary"
                        Case "5" : pValue = "Tertiary"
                        Case "6" : pValue = "Windows Server 2003 and Windows XP:  Not Applicable"
                    End Select
                End If

            Case "Location"
                Select Case pValue
                    Case "0" : pValue = "Internal"
                    Case "1" : pValue = "External"
                    Case "2" : pValue = "Reserved"
                    Case "3" : pValue = "Unknown"
                End Select

            Case "WritePolicy"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Write Back"
                    Case "4" : pValue = "Write Through"
                    Case "5" : pValue = "Varies with Address"
                    Case "6" : pValue = "Windows Server 2003 and Windows XP:  Determination Per I/O"
                End Select

            Case "Capabilities"
                Select Case pClassName
                    Case "Win32_CDROMDrive", "Win32_DiskDrive", "Win32_FloppyDrive"
                        Select Case pValue
                            Case "0" : pValue = "Unknown"
                            Case "1" : pValue = "Other"
                            Case "2" : pValue = "Sequential Access"
                            Case "3" : pValue = "Random Access"
                            Case "4" : pValue = "Supports Writing"
                            Case "5" : pValue = "Encryption"
                            Case "6" : pValue = "Compression"
                            Case "7" : pValue = "Supports Removable Media"
                            Case "8" : pValue = "Manual Cleaning"
                            Case "9" : pValue = "Automatic Cleaning"
                            Case "10" : pValue = "SMART Notification"
                            Case "11" : pValue = "Supports Dual-Sided Media"
                            Case "12" : pValue = "Predismount Eject Not Required"
                        End Select

                    Case "Win32_Printer"
                        Select Case pValue
                            Case "0" : pValue = "Unknown"
                            Case "1" : pValue = "Other"
                            Case "2" : pValue = "Color Printing"
                            Case "3" : pValue = "Duplex Printing"
                            Case "4" : pValue = "Copies"
                            Case "5" : pValue = "Collation"
                            Case "6" : pValue = "Stapling"
                            Case "7" : pValue = "Transparency Printing"
                            Case "8" : pValue = "Punch"
                            Case "9" : pValue = "Cover"
                            Case "10" : pValue = "Bind"
                            Case "11" : pValue = "Black and White Printing"
                            Case "12" : pValue = "One-Sided"
                            Case "13" : pValue = "Two-Sided Long Edge"
                            Case "14" : pValue = "Two-Sided Short Edge"
                            Case "15" : pValue = "Portrait"
                            Case "16" : pValue = "Landscape"
                            Case "17" : pValue = "Reverse Portrait"
                            Case "18" : pValue = "Reverse Landscape"
                            Case "19" : pValue = "Quality High"
                            Case "20" : pValue = "Quality Normal"
                            Case "21" : pValue = "Quality Low"
                        End Select

                    Case "Win32_SerialPort"
                        Select Case pValue
                            Case "1" : pValue = "Other"
                            Case "2" : pValue = "Unknown"
                            Case "3" : pValue = "XT/AT Compatible"
                            Case "4" : pValue = "16450 Compatible"
                            Case "5" : pValue = "16550 Compatible"
                            Case "6" : pValue = "16550A Compatible"
                            Case "160" : pValue = "8251 Compatible"
                            Case "161" : pValue = "8251FIFO Compatible"
                        End Select


                End Select

            Case "PCSystemType"
                Select Case pValue
                    Case "0" : pValue = "Unspecified"
                    Case "1" : pValue = "Desktop"
                    Case "2" : pValue = "Mobile"
                    Case "3" : pValue = "Workstation"
                    Case "4" : pValue = "Enterprise Server"
                    Case "5" : pValue = "Small Office and Home Office (SOHO) Server"
                    Case "6" : pValue = "Appliance PC"
                    Case "7" : pValue = "Performance Server"
                    Case "8" : pValue = "Maximum"
                End Select

            Case "ThermalState", "PowerSupplyState", "ChassisBootupState"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Safe"
                    Case "4" : pValue = "Warning"
                    Case "5" : pValue = "Critical"
                    Case "6" : pValue = "Nonrecoverable"
                End Select

            Case "ResetCapability"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Disabled"
                    Case "4" : pValue = "Enabled"
                    Case "5" : pValue = "Nonrecoverable"
                End Select

            Case "Layout"
                Select Case pValue
                    Case "00000402" : pValue = "bg"
                    Case "00000404" : pValue = "ch"
                    Case "00000405" : pValue = "cz"
                    Case "00000406" : pValue = "dk"
                    Case "00000407" : pValue = "gr"
                    Case "00000408" : pValue = "gk"
                    Case "00000409" : pValue = "us"
                    Case "0000040A" : pValue = "sp"
                    Case "0000040B" : pValue = "su"
                    Case "0000040C" : pValue = "fr"
                    Case "0000040E" : pValue = "hu"
                    Case "0000040F" : pValue = "is"
                    Case "00000410" : pValue = "it"
                    Case "00000411" : pValue = "jp"
                    Case "00000412" : pValue = "ko"
                    Case "00000413" : pValue = "nl"
                    Case "00000414" : pValue = "no"
                    Case "00000415" : pValue = "pl"
                    Case "00000416" : pValue = "br"
                    Case "00000418" : pValue = "ro"
                    Case "00000419" : pValue = "ru"
                    Case "0000041A" : pValue = "yu"
                    Case "0000041B" : pValue = "sl"
                    Case "0000041C" : pValue = "us"
                    Case "0000041D" : pValue = "sv"
                    Case "0000041F" : pValue = "tr"
                    Case "00000422" : pValue = "us"
                    Case "00000423" : pValue = "us"
                    Case "00000424" : pValue = "yu"
                    Case "00000425" : pValue = "et"
                    Case "00000426" : pValue = "us"
                    Case "00000427" : pValue = "us"
                    Case "00000804" : pValue = "ch"
                    Case "00000809" : pValue = "uk"
                    Case "0000080A" : pValue = "la"
                    Case "0000080C" : pValue = "be"
                    Case "00000813" : pValue = "be"
                    Case "00000816" : pValue = "po"
                    Case "00000C0C" : pValue = "cf"
                    Case "00000C1A" : pValue = "us"
                    Case "00001009" : pValue = "us"
                    Case "0000100C" : pValue = "sf"
                    Case "00001809" : pValue = "us"
                    Case "00010402" : pValue = "us"
                    Case "00010405" : pValue = "cz"
                    Case "00010407" : pValue = "gr"
                    Case "00010408" : pValue = "gk"
                    Case "00010409" : pValue = "dv"
                    Case "0001040A" : pValue = "sp"
                    Case "0001040E" : pValue = "hu"
                    Case "00010410" : pValue = "it"
                    Case "00010415" : pValue = "pl"
                    Case "00010419" : pValue = "ru"
                    Case "0001041B" : pValue = "sl"
                    Case "0001041F" : pValue = "tr"
                    Case "00010426" : pValue = "us"
                    Case "00010C0C" : pValue = "cf"
                    Case "00010C1A" : pValue = "us"
                    Case "00020408" : pValue = "gk"
                    Case "00020409" : pValue = "us"
                    Case "00030409" : pValue = "usl"
                    Case "00040409" : pValue = "usr"
                    Case "00050408" : pValue = "gk"
                End Select

            Case "MediaType"
                Select Case pValue
                    Case "0" : pValue = "Format is unknown"
                    Case "1" : pValue = "5 1/4-Inch Floppy Disk - 1.2 MB - 512 bytes/sector"
                    Case "2" : pValue = "3 1/2-Inch Floppy Disk - 1.44 MB -512 bytes/sector"
                    Case "3" : pValue = "3 1/2-Inch Floppy Disk - 2.88 MB - 512 bytes/sector"
                    Case "4" : pValue = "3 1/2-Inch Floppy Disk - 20.8 MB - 512 bytes/sector"
                    Case "5" : pValue = "3 1/2-Inch Floppy Disk - 720 KB - 512 bytes/sector"
                    Case "6" : pValue = "5 1/4-Inch Floppy Disk - 360 KB - 512 bytes/sector"
                    Case "7" : pValue = "5 1/4-Inch Floppy Disk - 320 KB - 512 bytes/sector"
                    Case "8" : pValue = "5 1/4-Inch Floppy Disk - 320 KB - 1024 bytes/sector"
                    Case "9" : pValue = "5 1/4-Inch Floppy Disk - 180 KB - 512 bytes/sector"
                    Case "10" : pValue = "5 1/4-Inch Floppy Disk - 160 KB - 512 bytes/sector"
                    Case "11" : pValue = "Removable media other than floppy"
                    Case "12" : pValue = "Fixed hard disk media"
                    Case "13" : pValue = "3 1/2-Inch Floppy Disk - 120 MB - 512 bytes/sector"
                    Case "14" : pValue = "3 1/2-Inch Floppy Disk - 640 KB - 512 bytes/sector"
                    Case "15" : pValue = "5 1/4-Inch Floppy Disk - 640 KB - 512 bytes/sector"
                    Case "16" : pValue = "5 1/4-Inch Floppy Disk - 720 KB - 512 bytes/sector"
                    Case "17" : pValue = "3 1/2-Inch Floppy Disk - 1.2 MB - 512 bytes/sector"
                    Case "18" : pValue = "3 1/2-Inch Floppy Disk - 1.23 MB - 1024 bytes/sector"
                    Case "19" : pValue = "5 1/4-Inch Floppy Disk - 1.23 MB - 1024 bytes/sector"
                    Case "20" : pValue = "3 1/2-Inch Floppy Disk - 128 MB - 512 bytes/sector"
                    Case "21" : pValue = "3 1/2-Inch Floppy Disk - 230 MB - 512 bytes/sector"
                    Case "22" : pValue = "8-Inch Floppy Disk - 256 KB - 128 bytes/sector"
                End Select

            Case "DriveType"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "No Root Directory"
                    Case "2" : pValue = "Removable Disk"
                    Case "3" : pValue = "Local Disk"
                    Case "4" : pValue = "Network Drive"
                    Case "5" : pValue = "Compact Disc"
                    Case "6" : pValue = "RAM Disk"
                End Select

            Case "StatusInfo"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Enabled"
                    Case "4" : pValue = "Disabled"
                    Case "5" : pValue = "Not Applicable"
                End Select

            Case "SoftwareElementState"
                Select Case pValue
                    Case "0" : pValue = "Deployable"
                    Case "1" : pValue = "Installable"
                    Case "2" : pValue = "Executable"
                    Case "3" : pValue = "Running"
                End Select

            Case "PowerManagementCapabilities"
                Select Case pValue
                    Case "1" : pValue = "Not Supported"
                    Case "2" : pValue = "Disabled"
                    Case "3" : pValue = "Enabled"
                    Case "4" : pValue = "Power Saving Modes Entered Automatically"
                    Case "5" : pValue = "Power State Settable"
                    Case "6" : pValue = "Power Cycling Supported"
                End Select

            Case "AdminPasswordStatus", "FrontPanelResetStatus", "KeyboardPasswordStatus", "PowerOnPasswordStatus"
                Select Case pValue
                    Case "1" : pValue = "Disabled"
                    Case "2" : pValue = "Enabled"
                    Case "3" : pValue = "Not Implemented"
                    Case "4" : pValue = "Unknown"
                End Select

            Case "DomainRole"
                Select Case pValue
                    Case "0" : pValue = "Standalone Workstation"
                    Case "1" : pValue = "Member Workstation"
                    Case "2" : pValue = "Standalone Server"
                    Case "3" : pValue = "Member Server"
                    Case "4" : pValue = "Backup Domain Controller"
                    Case "5" : pValue = "Primary Domain Controller"
                End Select

            Case "ConfigManagerErrorCode"
                Select Case pValue
                    Case "0" : pValue = "Device is working properly."
                    Case "1" : pValue = "Device is not configured correctly."
                    Case "2" : pValue = "Windows cannot load the driver for this device."
                    Case "3" : pValue = "Driver for this device might be corrupted, or the system may be low on memory or other resources."
                    Case "4" : pValue = "Device is not working properly. One of its drivers or the registry might be corrupted."
                    Case "5" : pValue = "Driver for the device requires a resource that Windows cannot manage."
                    Case "6" : pValue = "Boot configuration for the device conflicts with other devices."
                    Case "7" : pValue = "Cannot filter."
                    Case "8" : pValue = "Driver loader for the device is missing."
                    Case "9" : pValue = "Device is not working properly. The controlling firmware is incorrectly reporting the resources for the device."
                    Case "10" : pValue = "Device cannot start."
                    Case "11" : pValue = "Device failed."
                    Case "12" : pValue = "Device cannot find enough free resources to use."
                    Case "13" : pValue = "Windows cannot verify the device's resources."
                    Case "14" : pValue = "Device cannot work properly until the computer is restarted."
                    Case "15" : pValue = "Device is not working properly due to a possible re-enumeration problem."
                    Case "16" : pValue = "Windows cannot identify all of the resources that the device uses."
                    Case "17" : pValue = "Device is requesting an unknown resource type."
                    Case "18" : pValue = "Device drivers must be reinstalled."
                    Case "19" : pValue = "Failure using the VxD loader."
                    Case "20" : pValue = "Registry might be corrupted."
                    Case "21" : pValue = "System failure. If changing the device driver is ineffective, see the hardware documentation. Windows is removing the device."
                    Case "22" : pValue = "Device is disabled."
                    Case "23" : pValue = "System failure. If changing the device driver is ineffective, see the hardware documentation."
                    Case "24" : pValue = "Device is not present, not working properly, or does not have all of its drivers installed."
                    Case "25" : pValue = "Windows is still setting up the device."
                    Case "26" : pValue = "Windows is still setting up the device."
                    Case "27" : pValue = "Device does not have valid log configuration."
                    Case "28" : pValue = "Device drivers are not installed."
                    Case "29" : pValue = "Device is disabled. The device firmware did not provide the required resources."
                    Case "30" : pValue = "Device is using an IRQ resource that another device is using."
                    Case "31" : pValue = "Device is not working properly. Windows cannot load the required device drivers."
                End Select

            Case "TargetOperatingSystem"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "MACOS"
                    Case "3" : pValue = "ATTUNIX"
                    Case "4" : pValue = "DGUX"
                    Case "5" : pValue = "DECNT"
                    Case "6" : pValue = "Digital Unix"
                    Case "7" : pValue = "OpenVMS"
                    Case "8" : pValue = "HPUX"
                    Case "9" : pValue = "AIX"
                    Case "10" : pValue = "MVS"
                    Case "11" : pValue = "OS400"
                    Case "12" : pValue = "OS/2"
                    Case "13" : pValue = "JavaVM"
                    Case "14" : pValue = "MSDOS"
                    Case "15" : pValue = "WIN3x"
                    Case "16" : pValue = "WIN95"
                    Case "17" : pValue = "WIN98"
                    Case "18" : pValue = "WINNT"
                    Case "19" : pValue = "WINCE"
                    Case "20" : pValue = "NCR3000"
                    Case "21" : pValue = "NetWare"
                    Case "22" : pValue = "OSF"
                    Case "23" : pValue = "DC/OS"
                    Case "24" : pValue = "Reliant UNIX"
                    Case "25" : pValue = "SCO UnixWare"
                    Case "26" : pValue = "SCO OpenServer"
                    Case "27" : pValue = "Sequent"
                    Case "28" : pValue = "IRIX"
                    Case "29" : pValue = "Solaris"
                    Case "30" : pValue = "SunOS"
                    Case "31" : pValue = "U6000"
                    Case "32" : pValue = "ASERIES"
                    Case "33" : pValue = "TandemNSK"
                    Case "34" : pValue = "TandemNT"
                    Case "35" : pValue = "BS2000"
                    Case "36" : pValue = "LINUX"
                    Case "37" : pValue = "Lynx"
                    Case "38" : pValue = "XENIX"
                    Case "39" : pValue = "VM/ESA"
                    Case "40" : pValue = "Interactive UNIX"
                    Case "41" : pValue = "BSDUNIX"
                    Case "42" : pValue = "FreeBSD"
                    Case "43" : pValue = "NetBSD"
                    Case "44" : pValue = "GNU Hurd"
                    Case "45" : pValue = "OS9"
                    Case "46" : pValue = "MACH Kernel"
                    Case "47" : pValue = "Inferno"
                    Case "48" : pValue = "QNX"
                    Case "49" : pValue = "EPOC"
                    Case "50" : pValue = "IxWorks"
                    Case "51" : pValue = "VxWorks"
                    Case "52" : pValue = "MiNT"
                    Case "53" : pValue = "BeOS"
                    Case "54" : pValue = "HP MPE"
                    Case "55" : pValue = "NextStep"
                    Case "56" : pValue = "PalmPilot"
                    Case "57" : pValue = "Rhapsody"
                    Case "58" : pValue = "Windows 2000"
                    Case "59" : pValue = "Dedicated"
                    Case "60" : pValue = "VSE"
                    Case "61" : pValue = "TPF"
                End Select

            Case "PowerState"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Full Power"
                    Case "2" : pValue = "Power Save - Low Power Mode"
                    Case "3" : pValue = "Power Save - Standby"
                    Case "4" : pValue = "Power Save - Unknown"
                    Case "5" : pValue = "Power Cycle"
                    Case "6" : pValue = "Power Off"
                    Case "7" : pValue = "Power Save - Warning"
                End Select

            Case "WakeUpType"
                Select Case pValue
                    Case "0" : pValue = "Reserved"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "APM Timer"
                    Case "4" : pValue = "Modem Ring"
                    Case "5" : pValue = "LAN Remote"
                    Case "6" : pValue = "Power Switch"
                    Case "7" : pValue = "PCI PME#"
                    Case "8" : pValue = "AC Power Restored"
                End Select

            Case "AdapterTypeId"
                Select Case pValue
                    Case "0" : pValue = "Ethernet 802.3"
                    Case "1" : pValue = "Token Ring 802.5"
                    Case "2" : pValue = "Fiber Distributed Data Interface (FDDI)"
                    Case "3" : pValue = "Wide Area Network (WAN)"
                    Case "4" : pValue = "LocalTalk"
                    Case "5" : pValue = "Ethernet using DIX header format"
                    Case "6" : pValue = "ARCNET"
                    Case "7" : pValue = "ARCNET (878.2)"
                    Case "8" : pValue = "ATM"
                    Case "9" : pValue = "Wireless"
                    Case "10" : pValue = "Infrared Wireless"
                    Case "11" : pValue = "Bpc"
                    Case "12" : pValue = "CoWan"
                    Case "13" : pValue = "1394"
                End Select

            Case "NetConnectionStatus"
                Select Case pValue
                    Case "0" : pValue = "Disconnected"
                    Case "1" : pValue = "Connecting"
                    Case "2" : pValue = "Connected"
                    Case "3" : pValue = "Disconnecting"
                    Case "4" : pValue = "Hardware not present"
                    Case "5" : pValue = "Hardware disabled"
                    Case "6" : pValue = "Hardware malfunction"
                    Case "7" : pValue = "Media disconnected"
                    Case "8" : pValue = "Authenticating"
                    Case "9" : pValue = "Authentication succeeded"
                    Case "10" : pValue = "Authentication failed"
                    Case "11" : pValue = "Invalid address"
                    Case "12" : pValue = "Credentials required"
                End Select

            Case "TcpipNetbiosOptions"
                Select Case pValue
                    Case "0" : pValue = "EnableNetbiosViaDhcp"
                    Case "1" : pValue = "EnableNetbios"
                    Case "2" : pValue = "DisableNetbios"
                End Select

            Case "DataExecutionPrevention_SupportPolicy"
                Select Case pValue
                    Case "0" : pValue = "Always Off: DEP is turned off for all 32-bit applications on the computer with no exceptions. This setting is not available for the user interface."
                    Case "1" : pValue = "Always On: DEP is enabled for all 32-bit applications on the computer. This setting is not available for the user interface."
                    Case "2" : pValue = "Opt In: DEP is enabled for a limited number of binaries, the kernel, and all Windows-based services. However, it is off by default for all 32-bit applications. A user or administrator must explicitly choose either the AlwaysOn or the OptOut setting before DEP can be applied to 32-bit applications."
                    Case "3" : pValue = "Opt Out: DEP is enabled by default for all 32-bit applications. A user or administrator can explicitly remove support for a 32-bit application by adding the application to an exceptions list."
                End Select

            Case "ForegroundApplicationBoost"
                Select Case pValue
                    Case "0" : pValue = "None"
                    Case "1" : pValue = "Minimum"
                    Case "2" : pValue = "(Default) Maximum"
                End Select

            Case "OperatingSystemSKU"
                Select Case pValue
                    Case "0" : pValue = "Undefined"
                    Case "1" : pValue = "Ultimate Edition"
                    Case "2" : pValue = "Home Basic Edition"
                    Case "3" : pValue = "Home Premium Edition"
                    Case "4" : pValue = "Enterprise Edition"
                    Case "5" : pValue = "Home Basic N Edition"
                    Case "6" : pValue = "Business Edition"
                    Case "7" : pValue = "Standard Server Edition"
                    Case "8" : pValue = "Datacenter Server Edition"
                    Case "9" : pValue = "Small Business Server Edition"
                    Case "10" : pValue = "Enterprise Server Edition"
                    Case "11" : pValue = "Starter Edition"
                    Case "12" : pValue = "Datacenter Server Core Edition"
                    Case "13" : pValue = "Standard Server Core Edition"
                    Case "14" : pValue = "Enterprise Server Core Edition"
                    Case "15" : pValue = "Enterprise Server Edition for Itanium-Based Systems"
                    Case "16" : pValue = "Business N Edition"
                    Case "17" : pValue = "Web Server Edition"
                    Case "18" : pValue = "Cluster Server Edition"
                    Case "19" : pValue = "Home Server Edition"
                    Case "20" : pValue = "Storage Express Server Edition"
                    Case "21" : pValue = "Storage Standard Server Edition"
                    Case "22" : pValue = "Storage Workgroup Server Edition"
                    Case "23" : pValue = "Storage Enterprise Server Edition"
                    Case "24" : pValue = "Server For Small Business Edition"
                    Case "25" : pValue = "Small Business Server Premium Edition"
                End Select

            Case "OSLanguage"
                Select Case pValue
                    Case "1" : pValue = "Arabic"
                    Case "4" : pValue = "Arabic – Syria"
                    Case "9" : pValue = "English – Belize"
                    Case "1025" : pValue = "Arabic – Saudi Arabia"
                    Case "1026" : pValue = "Spanish – Peru"
                    Case "1027" : pValue = "Bulgarian"
                    Case "1028" : pValue = "Catalan"
                    Case "1029" : pValue = "Chinese (Traditional) – Taiwan"
                    Case "1030" : pValue = "Czech"
                    Case "1031" : pValue = "Danish"
                    Case "1032" : pValue = "German – Germany"
                    Case "1033" : pValue = "Greek"
                    Case "1034" : pValue = "English – United States"
                    Case "1035" : pValue = "Spanish – Traditional Sort"
                    Case "1036" : pValue = "Finnish"
                    Case "1037" : pValue = "French – France"
                    Case "1038" : pValue = "Hebrew"
                    Case "1039" : pValue = "Hungarian"
                    Case "1040" : pValue = "Icelandic"
                    Case "1041" : pValue = "Italian – Italy"
                    Case "1042" : pValue = "Japanese"
                    Case "1043" : pValue = "Korean"
                    Case "1044" : pValue = "Dutch – Netherlands"
                    Case "1045" : pValue = "Norwegian – Bokmal"
                    Case "1046" : pValue = "Polish"
                    Case "1047" : pValue = "Portuguese – Brazil"
                    Case "1048" : pValue = "Rhaeto-Romanic"
                    Case "1049" : pValue = "Romanian"
                    Case "1050" : pValue = "Russian"
                    Case "1051" : pValue = "Croatian"
                    Case "1052" : pValue = "Slovak"
                    Case "1053" : pValue = "Albanian"
                    Case "1054" : pValue = "Swedish"
                    Case "1055" : pValue = "Thai"
                    Case "1056" : pValue = "Turkish"
                    Case "1057" : pValue = "Urdu"
                    Case "1058" : pValue = "Indonesian"
                    Case "1059" : pValue = "Ukrainian"
                    Case "1060" : pValue = "Belarusian"
                    Case "1061" : pValue = "Slovenian"
                    Case "1062" : pValue = "Estonian"
                    Case "1063" : pValue = "Latvian"
                    Case "1065" : pValue = "Lithuanian"
                    Case "1066" : pValue = "Persian"
                    Case "1069" : pValue = "Vietnamese"
                    Case "1070" : pValue = "Basque"
                    Case "1071" : pValue = "Serbian"
                    Case "1072" : pValue = "Macedonian (F.Y.R.O. Macedonia)"
                    Case "1073" : pValue = "Sutu"
                    Case "1074" : pValue = "Tsonga"
                    Case "1076" : pValue = "Tswana"
                    Case "1077" : pValue = "Xhosa"
                    Case "1078" : pValue = "Zulu"
                    Case "1080" : pValue = "Afrikaans"
                    Case "1081" : pValue = "Faeroese"
                    Case "1082" : pValue = "Hindi"
                    Case "1084" : pValue = "Maltese"
                    Case "1085" : pValue = "Scottish Gaelic"
                    Case "1086" : pValue = "Yiddish"
                    Case "2049" : pValue = "Malay – Malaysia"
                    Case "2052" : pValue = "Arabic – Jordan"
                    Case "2055" : pValue = "English – Trinidad"
                    Case "2057" : pValue = "Spanish – Argentina"
                    Case "2058" : pValue = "Arabic – Lebanon"
                    Case "2060" : pValue = "Spanish – Ecuador"
                    Case "2064" : pValue = "Arabic – Kuwait"
                    Case "2067" : pValue = "Spanish – Chile"
                    Case "2068" : pValue = "Arabic – U.A.E."
                    Case "2070" : pValue = "Spanish – Uruguay"
                    Case "2072" : pValue = "Arabic – Bahrain"
                    Case "2073" : pValue = "Spanish – Paraguay"
                    Case "2074" : pValue = "Arabic – Qatar"
                    Case "2077" : pValue = "Spanish – Bolivia"
                    Case "3073" : pValue = "Spanish – El Salvador"
                    Case "3076" : pValue = "Spanish – Honduras"
                    Case "3079" : pValue = "Spanish – Nicaragua"
                    Case "3081" : pValue = "Arabic – Iraq"
                    Case "3082" : pValue = "Spanish – Puerto Rico"
                    Case "3084" : pValue = "Chinese (Simplified) – PRC"
                    Case "3098" : pValue = "German – Switzerland"
                    Case "4097" : pValue = "English – United Kingdom"
                    Case "4100" : pValue = "Spanish – Mexico"
                    Case "4103" : pValue = "French – Belgium"
                    Case "4105" : pValue = "Italian – Switzerland"
                    Case "4106" : pValue = "Dutch – Belgium"
                    Case "4108" : pValue = "Norwegian – Nynorsk"
                    Case "5121" : pValue = "Portuguese – Portugal"
                    Case "5127" : pValue = "Romanian – Moldova"
                    Case "5129" : pValue = "Russian – Moldova"
                    Case "5130" : pValue = "Serbian – Latin"
                    Case "5132" : pValue = "Swedish – Finland"
                    Case "6145" : pValue = "Arabic – Egypt"
                    Case "6153" : pValue = "Chinese (Traditional) – Hong Kong SAR"
                    Case "6154" : pValue = "German – Austria"
                    Case "7169" : pValue = "English – Australia"
                    Case "7177" : pValue = "Spanish – International Sort"
                    Case "7178" : pValue = "French – Canada"
                    Case "8193" : pValue = "Serbian – Cyrillic"
                    Case "8201" : pValue = "Chinese (Simplified)– China"
                    Case "8202" : pValue = "Arabic – Libya"
                    Case "9217" : pValue = "Chinese (Simplified) – Singapore"
                    Case "9226" : pValue = "German – Luxembourg"
                    Case "10241" : pValue = "English – Canada"
                    Case "10249" : pValue = "Spanish – Guatemala"
                    Case "10250" : pValue = "French – Switzerland"
                    Case "11265" : pValue = "Arabic – Algeria"
                    Case "11273" : pValue = "German – Liechtenstein"
                    Case "11274" : pValue = "English – New Zealand"
                    Case "12289" : pValue = "Spanish – Costa Rica"
                    Case "12298" : pValue = "French – Luxembourg"
                    Case "13313" : pValue = "Arabic – Morocco"
                    Case "13322" : pValue = "English – Ireland"
                    Case "14337" : pValue = "Spanish – Panama"
                    Case "14346" : pValue = "Arabic – Tunisia"
                    Case "15361" : pValue = "English – South Africa"
                    Case "15370" : pValue = "Spanish – Dominican Republic"
                    Case "16385" : pValue = "Arabic – Oman"
                    Case "16394" : pValue = "English – Jamaica"
                    Case "17418" : pValue = "Spanish – Venezuela"
                    Case "18442" : pValue = "English"
                    Case "19466" : pValue = "Arabic – Yemen"
                    Case "20490" : pValue = "Spanish – Colombia"
                End Select

            Case "OSType"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "MACROS"
                    Case "3" : pValue = "ATTUNIX"
                    Case "4" : pValue = "DGUX"
                    Case "5" : pValue = "DECNT"
                    Case "6" : pValue = "Digital UNIX"
                    Case "7" : pValue = "OpenVMS"
                    Case "8" : pValue = "HPUX"
                    Case "9" : pValue = "AIX"
                    Case "10" : pValue = "MVS"
                    Case "11" : pValue = "OS400"
                    Case "12" : pValue = "OS/2"
                    Case "13" : pValue = "JavaVM"
                    Case "14" : pValue = "MSDOS"
                    Case "15" : pValue = "WIN3x"
                    Case "16" : pValue = "WIN95"
                    Case "17" : pValue = "WIN98"
                    Case "18" : pValue = "WINNT"
                    Case "19" : pValue = "WINCE"
                    Case "20" : pValue = "NCR3000"
                    Case "21" : pValue = "NetWare"
                    Case "22" : pValue = "OSF"
                    Case "23" : pValue = "DC/OS"
                    Case "24" : pValue = "Reliant UNIX"
                    Case "25" : pValue = "SCO UnixWare"
                    Case "26" : pValue = "SCO OpenServer"
                    Case "27" : pValue = "Sequent"
                    Case "28" : pValue = "IRIX"
                    Case "29" : pValue = "Solaris"
                    Case "30" : pValue = "SunOS"
                    Case "31" : pValue = "U6000"
                    Case "32" : pValue = "ASERIES"
                    Case "33" : pValue = "TandemNSK"
                    Case "34" : pValue = "TandemNT"
                    Case "35" : pValue = "BS2000"
                    Case "36" : pValue = "LINUX"
                    Case "37" : pValue = "Lynx"
                    Case "38" : pValue = "XENIX"
                    Case "39" : pValue = "VM/ESA"
                    Case "40" : pValue = "Interactive UNIX"
                    Case "41" : pValue = "BSDUNIX"
                    Case "42" : pValue = "FreeBSD"
                    Case "43" : pValue = "NetBSD"
                    Case "44" : pValue = "GNU Hurd"
                    Case "45" : pValue = "OS9"
                    Case "46" : pValue = "MACH Kernel"
                    Case "47" : pValue = "Inferno"
                    Case "48" : pValue = "QNX"
                    Case "49" : pValue = "EPOC"
                    Case "50" : pValue = "IxWorks"
                    Case "51" : pValue = "VxWorks"
                    Case "52" : pValue = "MiNT"
                    Case "53" : pValue = "BeOS"
                    Case "54" : pValue = "HP MPE"
                    Case "55" : pValue = "NextStep"
                    Case "56" : pValue = "PalmPilot"
                    Case "57" : pValue = "Rhapsody"
                End Select

            Case "ProductType"
                Select Case pValue
                    Case "1" : pValue = "Work Station"
                    Case "2" : pValue = "Domain Controller"
                    Case "3" : pValue = "Server"
                End Select

            Case "SuiteMask"
                Dim pSuite As Integer
                If Integer.TryParse(pValue, pSuite) Then
                    If pSuite > 0 Then
                        pValue = String.Empty

                        If (pSuite And 1) = 1 Then pValue += "Small Business Server, "
                        If (pSuite And 2) = 2 Then pValue += "Server Enterprise, "
                        If (pSuite And 4) = 4 Then pValue += "BackOffice, "
                        If (pSuite And 8) = 8 Then pValue += "Communications Server, "
                        If (pSuite And 16) = 16 Then pValue += "Terminal Services, "
                        If (pSuite And 32) = 32 Then pValue += "Small Business Server (Restricted), "
                        If (pSuite And 64) = 64 Then pValue += "Embedded, "
                        If (pSuite And 128) = 128 Then pValue += "Data Center Server, "
                        If (pSuite And 256) = 256 Then pValue += "Terminal Services (Single User), "
                        If (pSuite And 512) = 512 Then pValue += "Home, "
                        If (pSuite And 1024) = 1024 Then pValue += "Blade, "

                        If pValue.Length > 0 Then pValue = pValue.Substring(0, pValue.Length - 2)
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "OSProductSuite"
                Dim pSuite As Integer
                If Integer.TryParse(pValue, pSuite) Then
                    If pSuite > 0 Then
                        pValue = String.Empty

                        If (pSuite And 1) = 1 Then pValue += "Small Business Server, "
                        If (pSuite And 2) = 2 Then pValue += "Server Enterprise, "
                        If (pSuite And 4) = 4 Then pValue += "BackOffice, "
                        If (pSuite And 8) = 8 Then pValue += "Communications Server, "
                        If (pSuite And 16) = 16 Then pValue += "Terminal Services, "
                        If (pSuite And 32) = 32 Then pValue += "Small Business Server (Restricted), "
                        If (pSuite And 64) = 64 Then pValue += "Embedded, "
                        If (pSuite And 128) = 128 Then pValue += "Data Center Server, "
                        If (pSuite And 256) = 256 Then pValue += "Terminal Services (Single User), "
                        If (pSuite And 512) = 512 Then pValue += "Home, "
                        If (pSuite And 1024) = 1024 Then pValue += "Web Server, "
                        If (pSuite And 8192) = 8192 Then pValue += "Storage Server, "
                        If (pSuite And 16384) = 16384 Then pValue += "Cluster Server, "

                        If pValue.Length > 0 Then pValue = pValue.Substring(0, pValue.Length - 2)
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "FormFactor"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "SIP"
                    Case "3" : pValue = "DIP"
                    Case "4" : pValue = "ZIP"
                    Case "5" : pValue = "SOJ"
                    Case "6" : pValue = "Proprietary"
                    Case "7" : pValue = "SIMM"
                    Case "8" : pValue = "DIMM"
                    Case "9" : pValue = "TSOP"
                    Case "10" : pValue = "PGA"
                    Case "11" : pValue = "RIMM"
                    Case "12" : pValue = "SODIMM"
                    Case "13" : pValue = "SRIMM"
                    Case "14" : pValue = "SMD"
                    Case "15" : pValue = "SSMP"
                    Case "16" : pValue = "QFP"
                    Case "17" : pValue = "TQFP"
                    Case "18" : pValue = "SOIC"
                    Case "19" : pValue = "LCC"
                    Case "20" : pValue = "PLCC"
                    Case "21" : pValue = "BGA"
                    Case "22" : pValue = "FPBGA"
                    Case "23" : pValue = "LGA"
                End Select

            Case "InterleavePosition"
                Select Case pValue
                    Case "0" : pValue = "Noninterleaved"
                    Case "1" : pValue = "First position"
                    Case "2" : pValue = "Second position"
                End Select

            Case "MemoryType"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "DRAM"
                    Case "3" : pValue = "Synchronous DRAM"
                    Case "4" : pValue = "Cache DRAM"
                    Case "5" : pValue = "EDO"
                    Case "6" : pValue = "EDRAM"
                    Case "7" : pValue = "VRAM"
                    Case "8" : pValue = "SRAM"
                    Case "9" : pValue = "RAM"
                    Case "10" : pValue = "ROM"
                    Case "11" : pValue = "Flash"
                    Case "12" : pValue = "EEPROM"
                    Case "13" : pValue = "FEPROM"
                    Case "14" : pValue = "EPROM"
                    Case "15" : pValue = "CDRAM"
                    Case "16" : pValue = "3DRAM"
                    Case "17" : pValue = "SDRAM"
                    Case "18" : pValue = "SGRAM"
                    Case "19" : pValue = "RDRAM"
                    Case "20" : pValue = "DDR"
                    Case "21" : pValue = "DDR-2"
                End Select

            Case "TypeDetail"
                Select Case pValue
                    Case "1" : pValue = "Reserved"
                    Case "2" : pValue = "Other"
                    Case "4" : pValue = "Unknown"
                    Case "8" : pValue = "Fast-paged"
                    Case "16" : pValue = "Static column"
                    Case "32" : pValue = "Pseudo-static"
                    Case "64" : pValue = "RAMBUS"
                    Case "128" : pValue = "Synchronous"
                    Case "256" : pValue = "CMOS"
                    Case "512" : pValue = "EDO"
                    Case "1024" : pValue = "Window DRAM"
                    Case "2048" : pValue = "Cache DRAM"
                    Case "4096" : pValue = "Nonvolatile"
                End Select

            Case "DeviceInterface"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Serial"
                    Case "4" : pValue = "PS/2"
                    Case "5" : pValue = "Infrared"
                    Case "6" : pValue = "HP-HIL"
                    Case "7" : pValue = "Bus Mouse"
                    Case "8" : pValue = "ADB (Apple Desktop Bus)"
                    Case "160" : pValue = "Bus Mouse DB-9"
                    Case "161" : pValue = "Bus Mouse Micro-DIN"
                    Case "162" : pValue = "USB"
                End Select

            Case "Handedness"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Not Applicable"
                    Case "2" : pValue = "Right-Handed Operation"
                    Case "3" : pValue = "Left-Handed Operation"
                End Select

            Case "PointingType"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Mouse"
                    Case "4" : pValue = "Track Ball"
                    Case "5" : pValue = "Track Point"
                    Case "6" : pValue = "Glide Point"
                    Case "7" : pValue = "Touch Pad"
                    Case "8" : pValue = "Touch Screen"
                    Case "9" : pValue = "Mouse - Optical Sensor"
                End Select

            Case "ConnectorType"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Male"
                    Case "3" : pValue = "Female"
                    Case "4" : pValue = "Shielded"
                    Case "5" : pValue = "Unshielded"
                    Case "6" : pValue = "SCSI (A) High-Density (50 pins)"
                    Case "7" : pValue = "SCSI (A) Low-Density (50 pins)"
                    Case "8" : pValue = "SCSI (P) High-Density (68 pins)"
                    Case "9" : pValue = "SCSI SCA-I (80 pins)"
                    Case "10" : pValue = "SCSI SCA-II (80 pins)"
                    Case "11" : pValue = "SCSI Fibre Channel (DB-9, Copper)"
                    Case "12" : pValue = "SCSI Fibre Channel (Fibre)"
                    Case "13" : pValue = "SCSI Fibre Channel SCA-II (40 pins)"
                    Case "14" : pValue = "SCSI Fibre Channel SCA-II (20 pins)"
                    Case "15" : pValue = "SCSI Fibre Channel BNC"
                    Case "16" : pValue = "ATA 3-1/2 Inch (40 pins)"
                    Case "17" : pValue = "ATA 2-1/2 Inch (44 pins)"
                    Case "18" : pValue = "ATA-2"
                    Case "19" : pValue = "ATA-3"
                    Case "20" : pValue = "ATA/66"
                    Case "21" : pValue = "DB-9"
                    Case "22" : pValue = "DB-15"
                    Case "23" : pValue = "DB-25"
                    Case "24" : pValue = "DB-36"
                    Case "25" : pValue = "RS-232C"
                    Case "26" : pValue = "RS-422"
                    Case "27" : pValue = "RS-423"
                    Case "28" : pValue = "RS-485"
                    Case "29" : pValue = "RS-449"
                    Case "30" : pValue = "V.35"
                    Case "31" : pValue = "X.21"
                    Case "32" : pValue = "IEEE-488"
                    Case "33" : pValue = "AUI"
                    Case "34" : pValue = "UTP Category 3"
                    Case "35" : pValue = "UTP Category 4"
                    Case "36" : pValue = "UTP Category 5"
                    Case "37" : pValue = "BNC"
                    Case "38" : pValue = "RJ11"
                    Case "39" : pValue = "RJ45"
                    Case "40" : pValue = "Fiber MIC"
                    Case "41" : pValue = "Apple AUI"
                    Case "42" : pValue = "Apple GeoPort"
                    Case "43" : pValue = "PCI"
                    Case "44" : pValue = "ISA"
                    Case "45" : pValue = "EISA"
                    Case "46" : pValue = "VESA"
                    Case "47" : pValue = "PCMCIA"
                    Case "48" : pValue = "PCMCIA Type I"
                    Case "49" : pValue = "PCMCIA Type II"
                    Case "50" : pValue = "PCMCIA Type III"
                    Case "51" : pValue = "ZV Port"
                    Case "52" : pValue = "CardBus"
                    Case "53" : pValue = "USB"
                    Case "54" : pValue = "IEEE 1394"
                    Case "55" : pValue = "HIPPI"
                    Case "56" : pValue = "HSSDC (6 pins)"
                    Case "57" : pValue = "GBIC"
                    Case "58" : pValue = "DIN"
                    Case "59" : pValue = "Mini-DIN"
                    Case "60" : pValue = "Micro-DIN"
                    Case "61" : pValue = "PS/2"
                    Case "62" : pValue = "Infrared"
                    Case "63" : pValue = "HP-HIL"
                    Case "64" : pValue = "Access.bus"
                    Case "65" : pValue = "NuBus"
                    Case "66" : pValue = "Centronics"
                    Case "67" : pValue = "Mini-Centronics"
                    Case "68" : pValue = "Mini-Centronics Type-14"
                    Case "69" : pValue = "Mini-Centronics Type-20"
                    Case "70" : pValue = "Mini-Centronics Type-26"
                    Case "71" : pValue = "Bus Mouse"
                    Case "72" : pValue = "ADB"
                    Case "73" : pValue = "AGP"
                    Case "74" : pValue = "VME Bus"
                    Case "75" : pValue = "VME64"
                    Case "76" : pValue = "Proprietary"
                    Case "77" : pValue = "Proprietary Processor Card Slot"
                    Case "78" : pValue = "Proprietary Memory Card Slot"
                    Case "79" : pValue = "Proprietary I/O Riser Slot"

                    Case "80" : pValue = "PCI-66MHZ"
                    Case "81" : pValue = "AGP2X"
                    Case "82" : pValue = "AGP4X"
                    Case "83" : pValue = "PC-98"
                    Case "84" : pValue = "PC-98-Hireso"
                    Case "85" : pValue = "PC-H98"
                    Case "86" : pValue = "PC-98Note"
                    Case "87" : pValue = "PC-98Full"
                    Case "88" : pValue = "PCI-X"
                    Case "89" : pValue = "SSA SCSI"
                    Case "90" : pValue = "Circular"
                    Case "91" : pValue = "On-Board IDE Connector"
                    Case "92" : pValue = "On-Board Floppy Connector"
                    Case "93" : pValue = "9 Pin Dual Inline"
                    Case "94" : pValue = "25 Pin Dual Inline"
                    Case "95" : pValue = "50 Pin Dual Inline"
                    Case "96" : pValue = "68 Pin Dual Inline"
                    Case "97" : pValue = "On-Board Sound Connector"
                    Case "98" : pValue = "Mini-Jack"
                    Case "99" : pValue = "PCI-X"
                    Case "100" : pValue = "Sbus IEEE 1396-1993 32 Bit"
                    Case "101" : pValue = "Sbus IEEE 1396-1993 64 Bit"
                    Case "102" : pValue = "MCA"
                    Case "103" : pValue = "GIO"
                    Case "104" : pValue = "XIO"
                    Case "105" : pValue = "HIO"
                    Case "106" : pValue = "NGIO"
                    Case "107" : pValue = "PMC"
                    Case "108" : pValue = "MTRJ"
                    Case "109" : pValue = "VF-45"
                    Case "110" : pValue = "Future I/O"
                    Case "111" : pValue = "SC"
                    Case "112" : pValue = "SG"
                    Case "113" : pValue = "Electrical"
                    Case "114" : pValue = "Optical"
                    Case "115" : pValue = "Ribbon"
                    Case "116" : pValue = "GLM"
                    Case "117" : pValue = "1x9"
                    Case "118" : pValue = "Mini SG"
                    Case "119" : pValue = "LC"
                    Case "120" : pValue = "HSSC"
                    Case "121" : pValue = "VHDCI Shielded (68 pins)"
                    Case "122" : pValue = "InfiniBand"
                    Case "123" : pValue = "AGP8X"
                    Case "124" : pValue = "PCI-E"
                End Select

            Case "PortType"
                Select Case pValue
                    Case "0" : pValue = "None"
                    Case "1" : pValue = "Parallel Port XT/AT Compatible"
                    Case "2" : pValue = "Parallel Port PS/2"
                    Case "3" : pValue = "Parallel Port ECP"
                    Case "4" : pValue = "Parallel Port EPP"
                    Case "5" : pValue = "Parallel Port ECP/EPP"
                    Case "6" : pValue = "Serial Port XT/AT Compatible"
                    Case "7" : pValue = "Serial Port 16450 Compatible"
                    Case "8" : pValue = "Serial Port 16550 Compatible"
                    Case "9" : pValue = "Serial Port 16550A Compatible"
                    Case "10" : pValue = "SCSI Port"
                    Case "11" : pValue = "MIDI Port"
                    Case "12" : pValue = "Joy Stick Port"
                    Case "13" : pValue = "Keyboard Port"
                    Case "14" : pValue = "Mouse Port"
                    Case "15" : pValue = "SSA SCSI"
                    Case "16" : pValue = "USB"
                    Case "17" : pValue = "FireWire (IEEE P1394)"
                    Case "18" : pValue = "PCMCIA Type II"
                    Case "19" : pValue = "PCMCIA Type II"
                    Case "20" : pValue = "PCMCIA Type III"
                    Case "21" : pValue = "CardBus"
                    Case "22" : pValue = "Access Bus Port"
                    Case "23" : pValue = "SCSI II"
                    Case "24" : pValue = "SCSI Wide"
                    Case "25" : pValue = "PC-98"
                    Case "26" : pValue = "PC-98-Hireso"
                    Case "27" : pValue = "PC-H98"
                    Case "28" : pValue = "Video Port"
                    Case "29" : pValue = "Audio Port"
                    Case "30" : pValue = "Modem Port"
                    Case "31" : pValue = "Network Port"
                    Case "32" : pValue = "8251 Compatible"
                    Case "33" : pValue = "8251 FIFO Compatible"
                End Select

            Case "PortSubClass"
                Select Case pValue
                    Case """00""" : pValue = "Parallel Port"
                    Case """01""" : pValue = "Serial Port"
                    Case """02""" : pValue = "Modem"
                End Select

            Case "Attributes"
                Dim pSuite As Integer
                If Integer.TryParse(pValue, pSuite) Then
                    If pSuite > 0 Then
                        pValue = String.Empty

                        If (pSuite And 1) = 1 Then pValue += "Queued: Print jobs are buffered and queued., "
                        If (pSuite And 2) = 2 Then pValue += "Direct: Document to be sent directly to the printer. This value is used if print jobs are not queued correctly., "
                        If (pSuite And 4) = 4 Then pValue += "Queued: Print jobs are buffered and queued., "
                        If (pSuite And 8) = 8 Then pValue += "Shared: Available as a shared network resource., "
                        If (pSuite And 16) = 16 Then pValue += "Network: Attached to a network. If both Local and Network bits are set, this indicates a network printer., "
                        If (pSuite And 32) = 32 Then pValue += "Hidden: Hidden from some users on the network., "
                        If (pSuite And 64) = 64 Then pValue += "Local: Directly connected to a computer. If both Local and Network bits are set, this indicates a network printer., "
                        If (pSuite And 128) = 128 Then pValue += "EnableDevQ: Enable the queue on the printer if available., "
                        If (pSuite And 256) = 256 Then pValue += "KeepPrintedJobs: Spooler should not delete documents after they are printed., "
                        If (pSuite And 512) = 512 Then pValue += "DoCompleteFirst: Start jobs that are finished spooling first., "
                        If (pSuite And 1024) = 1024 Then pValue += "WorkOffline: Queue print jobs when a printer is not available., "
                        If (pSuite And 2048) = 2048 Then pValue += "EnableBIDI: Enable bidirectional printing., "
                        If (pSuite And 4096) = 4096 Then pValue += "RawData: Allow only raw data type jobs to be spooled., "
                        If (pSuite And 8192) = 8192 Then pValue += "Published: Published in the network directory service., "

                        If pValue.Length > 0 Then pValue = pValue.Substring(0, pValue.Length - 2)
                    Else
                        pValue = String.Empty
                    End If
                End If

            Case "PaperSizesSupported"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "A"
                    Case "3" : pValue = "B"
                    Case "4" : pValue = "C"
                    Case "5" : pValue = "D"
                    Case "6" : pValue = "E"
                    Case "7" : pValue = "Letter"
                    Case "8" : pValue = "Legal"
                    Case "9" : pValue = "NA-10x13-Envelope"
                    Case "10" : pValue = "NA-9x12-Envelope"
                    Case "11" : pValue = "NA-Number-10-Envelope"
                    Case "12" : pValue = "NA-7x9-Envelope"
                    Case "13" : pValue = "NA-9x11-Envelope"
                    Case "14" : pValue = "NA-10x14-Envelope"
                    Case "15" : pValue = "NA-Number-9-Envelope"
                    Case "16" : pValue = "NA-6x9-Envelope"
                    Case "17" : pValue = "NA-10x15-Envelope"
                    Case "18" : pValue = "A0"
                    Case "19" : pValue = "A1"
                    Case "20" : pValue = "A2"
                    Case "21" : pValue = "A3"
                    Case "22" : pValue = "A4"
                    Case "23" : pValue = "A5"
                    Case "24" : pValue = "A6"
                    Case "25" : pValue = "A7"
                    Case "26" : pValue = "A8"
                    Case "27" : pValue = "A9A10"
                    Case "28" : pValue = "B0"
                    Case "29" : pValue = "B1"
                    Case "30" : pValue = "B2"
                    Case "31" : pValue = "B3"
                    Case "32" : pValue = "B4"
                    Case "33" : pValue = "B5"
                    Case "34" : pValue = "B6"
                    Case "35" : pValue = "B7"
                    Case "36" : pValue = "B8"
                    Case "37" : pValue = "B9"
                    Case "38" : pValue = "B10"
                    Case "39" : pValue = "C0"
                    Case "40" : pValue = "C1"
                    Case "41" : pValue = "C2"
                    Case "42" : pValue = "C3"
                    Case "43" : pValue = "C4"
                    Case "44" : pValue = "C5"
                    Case "45" : pValue = "C6"
                    Case "46" : pValue = "C7"
                    Case "47" : pValue = "C8"
                    Case "48" : pValue = "ISO-Designated"
                    Case "49" : pValue = "JIS B0"
                    Case "50" : pValue = "JIS B1"
                    Case "51" : pValue = "JIS B2"
                    Case "52" : pValue = "JIS B3"
                    Case "53" : pValue = "JIS B4"
                    Case "54" : pValue = "JIS B5"
                    Case "55" : pValue = "JIS B6"
                    Case "56" : pValue = "JIS B7"
                    Case "57" : pValue = "JIS B8"
                    Case "58" : pValue = "JIS B9"
                    Case "59" : pValue = "JIS B10"
                End Select

            Case "Architecture"
                Select Case pValue
                    Case "0" : pValue = "x86"
                    Case "1" : pValue = "MIPS"
                    Case "2" : pValue = "Alpha"
                    Case "3" : pValue = "PowerPC"
                    Case "6" : pValue = "Intel Itanium Processor Family (IPF)"
                    Case "9" : pValue = "x64"
                End Select

            Case "CpuStatus"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "CPU Enabled"
                    Case "2" : pValue = "CPU Disabled by User via BIOS Setup"
                    Case "3" : pValue = "CPU Disabled by BIOS (POST Error)"
                    Case "4" : pValue = "CPU Is Idle"
                    Case "5" : pValue = "Reserved"
                    Case "6" : pValue = "Reserved"
                    Case "7" : pValue = "Other"
                End Select

            Case "Family"
                Select Case pValue
                    Case "1" : pValue = "(" + pValue + ") Other"
                    Case "2" : pValue = "(" + pValue + ") Unknown"
                    Case "3" : pValue = "(" + pValue + ") 8086"
                    Case "4" : pValue = "(" + pValue + ") 80286"
                    Case "5" : pValue = "(" + pValue + ") Intel386™ Processor"
                    Case "6" : pValue = "(" + pValue + ") Intel486™ Processor"
                    Case "7" : pValue = "(" + pValue + ") 8087"
                    Case "8" : pValue = "(" + pValue + ") 80287"
                    Case "9" : pValue = "(" + pValue + ") 80387"
                    Case "10" : pValue = "(" + pValue + ") 80487"
                    Case "11" : pValue = "(" + pValue + ") Pentium Brand"
                    Case "12" : pValue = "(" + pValue + ") Pentium Pro"
                    Case "13" : pValue = "(" + pValue + ") Pentium II"
                    Case "14" : pValue = "(" + pValue + ") Pentium Processor with MMX™ Technology"
                    Case "15" : pValue = "(" + pValue + ") Celeron™"
                    Case "16" : pValue = "(" + pValue + ") Pentium II Xeon™"
                    Case "17" : pValue = "(" + pValue + ") Pentium III"
                    Case "18" : pValue = "(" + pValue + ") M1 Family"
                    Case "19" : pValue = "(" + pValue + ") M2 Family"
                    Case "24" : pValue = "(" + pValue + ") AMD Duron™ Processor Family"
                    Case "25" : pValue = "(" + pValue + ") K5 Family"
                    Case "26" : pValue = "(" + pValue + ") K6 Family"
                    Case "27" : pValue = "(" + pValue + ") K6-2"
                    Case "28" : pValue = "(" + pValue + ") K6-3"
                    Case "29" : pValue = "(" + pValue + ") AMD Athlon™ Processor Family"
                    Case "30" : pValue = "(" + pValue + ") AMD2900 Family"
                    Case "31" : pValue = "(" + pValue + ") K6-2+"
                    Case "32" : pValue = "(" + pValue + ") Power PC Family"
                    Case "33" : pValue = "(" + pValue + ") Power PC 601"
                    Case "34" : pValue = "(" + pValue + ") Power PC 603"
                    Case "35" : pValue = "(" + pValue + ") Power PC 603+"
                    Case "36" : pValue = "(" + pValue + ") Power PC 604"
                    Case "37" : pValue = "(" + pValue + ") Power PC 620"
                    Case "38" : pValue = "(" + pValue + ") Power PC X704"
                    Case "39" : pValue = "(" + pValue + ") Power PC 750"
                    Case "48" : pValue = "(" + pValue + ") Alpha Family"
                    Case "49" : pValue = "(" + pValue + ") Alpha 21064"
                    Case "50" : pValue = "(" + pValue + ") Alpha 21066"
                    Case "51" : pValue = "(" + pValue + ") Alpha 21164"
                    Case "52" : pValue = "(" + pValue + ") Alpha 21164PC"
                    Case "53" : pValue = "(" + pValue + ") Alpha 21164a"
                    Case "54" : pValue = "(" + pValue + ") Alpha 21264"
                    Case "55" : pValue = "(" + pValue + ") Alpha 21364"
                    Case "64" : pValue = "(" + pValue + ") MIPS Family"
                    Case "65" : pValue = "(" + pValue + ") MIPS R4000"
                    Case "66" : pValue = "(" + pValue + ") MIPS R4200"
                    Case "67" : pValue = "(" + pValue + ") MIPS R4400"
                    Case "68" : pValue = "(" + pValue + ") MIPS R4600"
                    Case "69" : pValue = "(" + pValue + ") MIPS R10000"
                    Case "80" : pValue = "(" + pValue + ") SPARC Family"
                    Case "81" : pValue = "(" + pValue + ") SuperSPARC"
                    Case "82" : pValue = "(" + pValue + ") microSPARC II"
                    Case "83" : pValue = "(" + pValue + ") microSPARC IIep"
                    Case "84" : pValue = "(" + pValue + ") UltraSPARC"
                    Case "85" : pValue = "(" + pValue + ") UltraSPARC II"
                    Case "86" : pValue = "(" + pValue + ") UltraSPARC IIi"
                    Case "87" : pValue = "(" + pValue + ") UltraSPARC III"
                    Case "88" : pValue = "(" + pValue + ") UltraSPARC IIIi"
                    Case "96" : pValue = "(" + pValue + ") 68040"
                    Case "97" : pValue = "(" + pValue + ") 68xxx Family"
                    Case "98" : pValue = "(" + pValue + ") 68000"
                    Case "99" : pValue = "(" + pValue + ") 68010"
                    Case "100" : pValue = "(" + pValue + ") 68020"
                    Case "101" : pValue = "(" + pValue + ") 68030"
                    Case "112" : pValue = "(" + pValue + ") Hobbit Family"
                    Case "120" : pValue = "(" + pValue + ") Crusoe™ TM5000 Family"
                    Case "121" : pValue = "(" + pValue + ") Crusoe™ TM3000 Family"
                    Case "122" : pValue = "(" + pValue + ") Efficeon™ TM8000 Family"
                    Case "128" : pValue = "(" + pValue + ") Weitek"
                    Case "130" : pValue = "(" + pValue + ") Itanium™ Processor"
                    Case "131" : pValue = "(" + pValue + ") AMD Athlon™ 64 Processor Famiily"
                    Case "132" : pValue = "(" + pValue + ") AMD Opteron™ Processor Family"
                    Case "144" : pValue = "(" + pValue + ") PA-RISC Family"
                    Case "145" : pValue = "(" + pValue + ") PA-RISC 8500"
                    Case "146" : pValue = "(" + pValue + ") PA-RISC 8000"
                    Case "147" : pValue = "(" + pValue + ") PA-RISC 7300LC"
                    Case "148" : pValue = "(" + pValue + ") PA-RISC 7200"
                    Case "149" : pValue = "(" + pValue + ") PA-RISC 7100LC"
                    Case "150" : pValue = "(" + pValue + ") PA-RISC 7100"
                    Case "160" : pValue = "(" + pValue + ") V30 Family"
                    Case "176" : pValue = "(" + pValue + ") Pentium III Xeon™ Processor"
                    Case "177" : pValue = "(" + pValue + ") Pentium III Processor with Intel SpeedStep™ Technology"
                    Case "178" : pValue = "(" + pValue + ") Pentium 4"
                    Case "179" : pValue = "(" + pValue + ") Intel Xeon™"
                    Case "180" : pValue = "(" + pValue + ") AS400 Family"
                    Case "181" : pValue = "(" + pValue + ") Intel Xeon™ Processor MP"
                    Case "182" : pValue = "(" + pValue + ") AMD Athlon™ XP Family"
                    Case "183" : pValue = "(" + pValue + ") AMD Athlon™ MP Family"
                    Case "184" : pValue = "(" + pValue + ") Intel Itanium 2"
                    Case "185" : pValue = "(" + pValue + ") Intel Pentium M Processor"
                    Case "190" : pValue = "(" + pValue + ") K7"
                    Case "200" : pValue = "(" + pValue + ") IBM390 Family"
                    Case "201" : pValue = "(" + pValue + ") G4"
                    Case "202" : pValue = "(" + pValue + ") G5"
                    Case "203" : pValue = "(" + pValue + ") G6"
                    Case "204" : pValue = "(" + pValue + ") z/Architecture Base"
                    Case "250" : pValue = "(" + pValue + ") i860"
                    Case "251" : pValue = "(" + pValue + ") i960"
                    Case "260" : pValue = "(" + pValue + ") SH-3"
                    Case "261" : pValue = "(" + pValue + ") SH-4"
                    Case "280" : pValue = "(" + pValue + ") ARM"
                    Case "281" : pValue = "(" + pValue + ") StrongARM"
                    Case "300" : pValue = "(" + pValue + ") 6x86"
                    Case "301" : pValue = "(" + pValue + ") MediaGX"
                    Case "302" : pValue = "(" + pValue + ") MII"
                    Case "320" : pValue = "(" + pValue + ") WinChip"
                    Case "350" : pValue = "(" + pValue + ") DSP"
                    Case "500" : pValue = "(" + pValue + ") Video Processor"
                End Select

            Case "ProcessorType"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Central Processor"
                    Case "4" : pValue = "Math Processor"
                    Case "5" : pValue = "DSP Processor"
                    Case "6" : pValue = "Video Processor"
                End Select

            Case "UpgradeMethod"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Daughter Board"
                    Case "4" : pValue = "ZIF Socket"
                    Case "5" : pValue = "Replacement or Piggy Back"
                    Case "6" : pValue = "None"
                    Case "7" : pValue = "LIF Socket"
                    Case "8" : pValue = "Slot 1"
                    Case "9" : pValue = "Slot 2"
                    Case "10" : pValue = "370 Pin Socket"
                    Case "11" : pValue = "Slot A"
                    Case "12" : pValue = "Slot M"
                    Case "13" : pValue = "Socket 423"
                    Case "14" : pValue = "Socket A (Socket 462)"
                    Case "15" : pValue = "Socket 478"
                    Case "16" : pValue = "Socket 754"
                    Case "17" : pValue = "Socket 940"
                    Case "18" : pValue = "Socket 939"
                End Select

            Case "DetectedErrorState"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "No Error"
                    Case "3" : pValue = "Low Paper"
                    Case "4" : pValue = "No Paper"
                    Case "5" : pValue = "Low Toner"
                    Case "6" : pValue = "No Toner"
                    Case "7" : pValue = "Door Open"
                    Case "8" : pValue = "Jammed"
                    Case "9" : pValue = "Offline"
                    Case "10" : pValue = "Service Requested"
                    Case "11" : pValue = "Output Bin Full"
                End Select

            Case "ExtendedDetectedErrorState"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "No Error"
                    Case "3" : pValue = "Low Paper"
                    Case "4" : pValue = "No Paper"
                    Case "5" : pValue = "Low Toner"
                    Case "6" : pValue = "No Toner"
                    Case "7" : pValue = "Door Open"
                    Case "8" : pValue = "Jammed"
                    Case "9" : pValue = "Service Requested"
                    Case "10" : pValue = "Output Bin Full"
                    Case "11" : pValue = "Paper Problem"
                    Case "12" : pValue = "Cannot Print Page"
                    Case "13" : pValue = "User Intervention Required"
                    Case "14" : pValue = "Out of Memory"
                    Case "15" : pValue = "Server Unknown"
                End Select

            Case "ExtendedPrinterStatus"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Idle"
                    Case "4" : pValue = "Printing"
                    Case "5" : pValue = "Warming Up"
                    Case "6" : pValue = "Stopped Printing"
                    Case "7" : pValue = "Offline"
                    Case "8" : pValue = "Paused"
                    Case "9" : pValue = "Error"
                    Case "10" : pValue = "Busy"
                    Case "11" : pValue = "Not Available"
                    Case "12" : pValue = "Waiting"
                    Case "13" : pValue = "Processing"
                    Case "14" : pValue = "Initialization"
                    Case "15" : pValue = "Power Save"
                    Case "16" : pValue = "Pending Deletion"
                    Case "17" : pValue = "I/O Active"
                    Case "18" : pValue = "Manual Feed"
                End Select

            Case "PrinterState"
                Select Case pValue
                    Case "1" : pValue = "Paused"
                    Case "2" : pValue = "Error"
                    Case "3" : pValue = "Pending Deletion"
                    Case "4" : pValue = "Paper Jam"
                    Case "5" : pValue = "Paper Out"
                    Case "6" : pValue = "Manual Feed"
                    Case "7" : pValue = "Paper Problem"
                    Case "8" : pValue = "Offline"
                    Case "9" : pValue = "I/O Active"
                    Case "10" : pValue = "Busy"
                    Case "11" : pValue = "Printing"
                    Case "12" : pValue = "Output Bin Full"
                    Case "13" : pValue = "Not Available"
                    Case "14" : pValue = "Waiting"
                    Case "15" : pValue = "Processing"
                    Case "16" : pValue = "Initialization"
                    Case "17" : pValue = "Warming Up"
                    Case "18" : pValue = "Toner Low"
                    Case "19" : pValue = "No Toner"
                    Case "20" : pValue = "Page Punt"
                    Case "21" : pValue = "User Intervention Required"
                    Case "22" : pValue = "Out of Memory"
                    Case "23" : pValue = "Door Open"
                    Case "24" : pValue = "Server_Unknown"
                    Case "25" : pValue = "Power Save"
                End Select

            Case "PrinterStatus"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Idle"
                    Case "4" : pValue = "Printing"
                    Case "5" : pValue = "Warming Up"
                    Case "6" : pValue = "Stopped printing"
                    Case "7" : pValue = "Offline"
                End Select

            Case "LanguagesSupported"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "PCL"
                    Case "4" : pValue = "HPGL"
                    Case "5" : pValue = "PJL"
                    Case "6" : pValue = "PS"
                    Case "7" : pValue = "PSPrinter"
                    Case "8" : pValue = "IPDS"
                    Case "9" : pValue = "PPDS"
                    Case "10" : pValue = "EscapeP"
                    Case "11" : pValue = "Epson"
                    Case "12" : pValue = "DDIF"
                    Case "13" : pValue = "Interpress"
                    Case "14" : pValue = "ISO6429"
                    Case "15" : pValue = "LineData"
                    Case "16" : pValue = "DODCA"
                    Case "17" : pValue = "REGIS"
                    Case "18" : pValue = "SCS"
                    Case "19" : pValue = "SPDL"
                    Case "20" : pValue = "TEK4014"
                    Case "21" : pValue = "PDS"
                    Case "22" : pValue = "IGP"
                    Case "23" : pValue = "CodeV"
                    Case "24" : pValue = "DSCDSE"
                    Case "25" : pValue = "WPS"
                    Case "26" : pValue = "LN03"
                    Case "27" : pValue = "CCITT"
                    Case "28" : pValue = "QUIC"
                    Case "29" : pValue = "CPAP"
                    Case "30" : pValue = "DecPPL"
                    Case "31" : pValue = "SimpleText"
                    Case "32" : pValue = "NPAP"
                    Case "33" : pValue = "DOC"
                    Case "34" : pValue = "imPress"
                    Case "35" : pValue = "Pinwriter"
                    Case "36" : pValue = "NPDL"
                    Case "37" : pValue = "NEC201PL"
                    Case "38" : pValue = "Automatic"
                    Case "39" : pValue = "Pages"
                    Case "40" : pValue = "LIPS"
                    Case "41" : pValue = "TIFF"
                    Case "42" : pValue = "Diagnostic"
                    Case "43" : pValue = "CaPSL"
                    Case "44" : pValue = "EXCL"
                    Case "45" : pValue = "LCDS"
                    Case "46" : pValue = "XES"
                    Case "47" : pValue = "MIME"
                    Case "48" : pValue = "XPS"
                    Case "49" : pValue = "HPGL2"
                    Case "50" : pValue = "PCLXL"
                End Select

            Case "ChassisTypes"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Desktop"
                    Case "4" : pValue = "Low Profile Desktop"
                    Case "5" : pValue = "Pizza Box"
                    Case "6" : pValue = "Mini Tower"
                    Case "7" : pValue = "Tower"
                    Case "8" : pValue = "Portable"
                    Case "9" : pValue = "Laptop"
                    Case "10" : pValue = "Notebook"
                    Case "11" : pValue = "Hand Held"
                    Case "12" : pValue = "Docking Station"
                    Case "13" : pValue = "All in One"
                    Case "14" : pValue = "Sub Notebook"
                    Case "15" : pValue = "Space-Saving"
                    Case "16" : pValue = "Lunch Box"
                    Case "17" : pValue = "Main System Chassis"
                    Case "18" : pValue = "Expansion Chassis"
                    Case "19" : pValue = "SubChassis"
                    Case "20" : pValue = "Bus Expansion Chassis"
                    Case "21" : pValue = "Peripheral Chassis"
                    Case "22" : pValue = "Storage Chassis"
                    Case "23" : pValue = "Rack Mount Chassis"
                    Case "24" : pValue = "Sealed-Case PC"
                End Select

            Case "SecurityStatus"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "None"
                    Case "4" : pValue = "External Interface Locked Out"
                    Case "5" : pValue = "External Interface Enabled"
                End Select

            Case "CurrentUsage"
                Select Case pValue
                    Case "0" : pValue = "Reserved"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Available"
                    Case "4" : pValue = "In Use"
                End Select

            Case "MaxDataWidth"
                Select Case pValue
                    Case "0" : pValue = "8 Bits"
                    Case "1" : pValue = "16 Bits"
                    Case "2" : pValue = "32 Bits"
                    Case "3" : pValue = "64 Bits"
                    Case "4" : pValue = "128 Bits"
                    Case "5" : pValue = "256 Bits"
                    Case "6" : pValue = "512 Bits"
                    Case "7" : pValue = "1 KBits"
                    Case "8" : pValue = "2 KBits"
                    Case "9" : pValue = "4 KBits"
                    Case "10" : pValue = "8 KBits"
                End Select

            Case "VccMixedVoltageSupport"
                Select Case pValue
                    Case "0" : pValue = "Unknown"
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "3.3V"
                    Case "3" : pValue = "5V"
                End Select

            Case "CurrentScanMode"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "Interlaced"
                    Case "4" : pValue = "Noninterlaced"
                End Select

            Case "VideoArchitecture"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "CGA"
                    Case "4" : pValue = "EGA"
                    Case "5" : pValue = "VGA"
                    Case "6" : pValue = "SVGA"
                    Case "7" : pValue = "MDA"
                    Case "8" : pValue = "HGC"
                    Case "9" : pValue = "MCGA"
                    Case "10" : pValue = "8514A"
                    Case "11" : pValue = "XGA"
                    Case "12" : pValue = "Linear Frame Buffer"
                    Case "160" : pValue = "PC-98"
                End Select

            Case "VideoMemoryType"
                Select Case pValue
                    Case "1" : pValue = "Other"
                    Case "2" : pValue = "Unknown"
                    Case "3" : pValue = "VRAM"
                    Case "4" : pValue = "DRAM"
                    Case "5" : pValue = "SRAM"
                    Case "6" : pValue = "WRAM"
                    Case "7" : pValue = "EDO RAM"
                    Case "8" : pValue = "Burst Synchronous DRAM"
                    Case "9" : pValue = "Pipelined Burst SRAM"
                    Case "10" : pValue = "CDRAM"
                    Case "11" : pValue = "3DRAM"
                    Case "12" : pValue = "SDRAM"
                    Case "13" : pValue = "SGRAM"
                End Select

            Case "ManufacturerID"
                Select Case pValue
                    Case "27934" : pValue = "LG"
                    Case "11596" : pValue = "Samsung"
                End Select

        End Select

        Return pValue
    End Function

    Private Function Get1024(ByVal pSize As Long, ByVal pMeasure As String) As String
        Dim pValue As String

        If Round(pSize / 1024 / 1024 / 1024 / 1024) > 0 Then
            pValue = Round(pSize / 1024 / 1024 / 1024 / 1024).ToString("0") + " T"
        ElseIf Round(pSize / 1024 / 1024 / 1024) > 0 Then
            pValue = Round(pSize / 1024 / 1024 / 1024).ToString("0") + " G"
        ElseIf Round(pSize / 1024 / 1024) > 0 Then
            pValue = Round(pSize / 1024 / 1024).ToString("0") + " M"
        ElseIf Round(pSize / 1024) > 0 Then
            pValue = Round(pSize / 1024).ToString("0") + " K"
        Else
            pValue = Round(pSize).ToString("0") + " "
        End If
        pValue += pMeasure

        Return pValue
    End Function
End Class

Public Class WMIExtMonitor
    Public ID As String
    Public Device As String
    Public PNPDeviceID As String
    Public Name As String
    Public Serial As String
    Public SerialNumber As String
    Public ManufacturerDate As String
    Public ManufacturerID As String
    Public ProductID As String
    Public EDID() As Byte

    Public ReadOnly Property FullName() As String
        Get
            Return Me.ID + "\" + Me.PNPDeviceID
        End Get
    End Property

    Public Shared Function GetMonitors() As List(Of WMIExtMonitor)
        Dim oMonitors As New List(Of WMIExtMonitor)

        Dim oLocalMachineKey As RegistryKey = Registry.LocalMachine

        Dim oDisplaysKey As RegistryKey = oLocalMachineKey.OpenSubKey("System\CurrentControlSet\Enum\Display")
        Dim oDisplays() As String = oDisplaysKey.GetSubKeyNames
        For Each oDisplay As String In oDisplays

            Dim oDisplayKey As RegistryKey = oDisplaysKey.OpenSubKey(oDisplay)
            Dim oSubDisplays() As String = oDisplayKey.GetSubKeyNames
            For Each oSubDisplay As String In oSubDisplays

                Dim oSubDisplayKey As RegistryKey = oDisplayKey.OpenSubKey(oSubDisplay)
                Dim oDeviceDesc As String = oSubDisplayKey.GetValue("DeviceDesc")

                Dim oControlKey As RegistryKey = oSubDisplayKey.OpenSubKey("Control")
                If Not oControlKey Is Nothing Then

                    Dim oMonitor As New WMIExtMonitor
                    oMonitor.ID = oDisplay
                    oMonitor.Device = oDeviceDesc
                    oMonitor.PNPDeviceID = oSubDisplay

                    Dim oDeviceParametersKey As RegistryKey = oSubDisplayKey.OpenSubKey("Device Parameters")
                    oMonitor.EDID = oDeviceParametersKey.GetValue("EDID")
                    If Not oMonitor.EDID Is Nothing Then
                        GetDescriptor(oMonitor, 54, 71)
                        GetDescriptor(oMonitor, 72, 89)
                        GetDescriptor(oMonitor, 90, 107)
                        GetDescriptor(oMonitor, 108, 125)
                        GetManufacturerDate(oMonitor)
                        oMonitor.ManufacturerID = oMonitor.EDID(9) * 256 + oMonitor.EDID(8)
                        oMonitor.ProductID = oMonitor.EDID(11) * 256 + oMonitor.EDID(10)
                        oMonitor.SerialNumber = oMonitor.EDID(15) * 256 * 256 * 256 + oMonitor.EDID(14) * 256 * 256 + oMonitor.EDID(13) * 256 + oMonitor.EDID(12)
                    End If

                    oMonitors.Add(oMonitor)
                End If

            Next
        Next

        Return oMonitors
    End Function
    Private Shared Sub GetDescriptor(ByRef pMonitor As WMIExtMonitor, ByVal pFrom As Int32, ByVal pTo As Int32)
        '54 0
        '55 0
        '56 0
        '57 FFh=Monitor Serial Number, FEh=ASCII string, FDh=Monitor Range Limits, FCh=Monitor name, FBh=Colour Point Data, FAh, Standard Timing Data, F9h=Currently undefined, 0Fh=defined by manufacturer
        '58 0

        If pMonitor.EDID(pFrom + 0) = 0 And
           pMonitor.EDID(pFrom + 1) = 0 And
           pMonitor.EDID(pFrom + 2) = 0 And
           pMonitor.EDID(pFrom + 4) = 0 Then

            Select Case pMonitor.EDID(pFrom + 3)
                Case CInt(Val("&hFF")) 'Monitor Serial Number"
                    pMonitor.Serial = Encoding.Default.GetString(pMonitor.EDID, pFrom + 5, 12)
                Case CInt(Val("&hFC")) 'Monitor name"
                    pMonitor.Name = Encoding.Default.GetString(pMonitor.EDID, pFrom + 5, 12)

                    'Case CInt(Val("&hF9")) 'Currently undefined"
                    'Case CInt(Val("&h0F")) 'defined by manufacturer"
                    'Case CInt(Val("&hFE")) 'ASCII string"
                    'Case CInt(Val("&hFD")) 'Monitor Range Limits
                    'Case CInt(Val("&hFB")) 'Colour Point Data, 
                    'Case CInt(Val("&hFA")) 'Standard Timing Data, 
            End Select
        End If

    End Sub
    Private Shared Sub GetManufacturerDate(ByRef pMonitor As WMIExtMonitor)
        Dim oYear As String = pMonitor.EDID(17) + 1990
        Dim oWeek As String = pMonitor.EDID(16)

        Dim oDate As Date = (New Date(oYear, 1, 1))
        oDate = oDate.AddDays((oWeek * 7) - 1)

        pMonitor.ManufacturerDate = oYear + " week " + oWeek + " (" + oDate.ToString("MMMM") + ")"
    End Sub
End Class
