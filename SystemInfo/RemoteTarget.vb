Imports System.Management
Imports System.Net
Imports System.Security

''' <summary>
''' Computer to collect information from: the local machine or a remote one reached through WMI/DCOM.
''' </summary>
Public Class RemoteTarget
    Private Const ConnectTimeoutSeconds As Integer = 15
    Private Const ReachabilityTimeoutSeconds As Integer = 3
    Private Const RpcEndpointMapperPort As Integer = 135

    Public Property Host As String = String.Empty
    Public Property UserName As String = String.Empty
    Public Property Password As SecureString

    Public Shared ReadOnly Property Local As RemoteTarget
        Get
            Return New RemoteTarget
        End Get
    End Property

    ''' <summary>True when the target is this computer. WMI rejects explicit credentials for local connections.</summary>
    Public ReadOnly Property IsLocal As Boolean
        Get
            Dim oHost As String = If(Host, String.Empty).Trim()
            If oHost.Length = 0 OrElse oHost = "." Then Return True
            If oHost.Equals("localhost", StringComparison.OrdinalIgnoreCase) Then Return True
            If oHost.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase) Then Return True

            Try
                Dim oAddress As IPAddress = Nothing
                If IPAddress.TryParse(oHost, oAddress) Then
                    If IPAddress.IsLoopback(oAddress) Then Return True
                    For Each oLocal As IPAddress In Dns.GetHostAddresses(Dns.GetHostName())
                        If oLocal.Equals(oAddress) Then Return True
                    Next
                End If
            Catch ex As Exception
                Console.WriteLine("RemoteTarget.IsLocal: " + ex.Message)
            End Try

            Return False
        End Get
    End Property

    Public ReadOnly Property DisplayName As String
        Get
            Return If(IsLocal, Environment.MachineName, Host.Trim())
        End Get
    End Property

    Public Function BuildConnectionOptions() As ConnectionOptions
        Dim oOptions As New ConnectionOptions
        oOptions.Impersonation = ImpersonationLevel.Impersonate
        oOptions.Authentication = AuthenticationLevel.PacketPrivacy 'required by the DCOM hardening (KB5004442)
        oOptions.EnablePrivileges = True
        oOptions.Timeout = TimeSpan.FromSeconds(ConnectTimeoutSeconds)

        If Not IsLocal AndAlso Not String.IsNullOrEmpty(UserName) Then
            oOptions.Username = UserName
            oOptions.SecurePassword = Password
        End If

        Return oOptions
    End Function

    ''' <summary>
    ''' Connects and returns a one-line description ("NAME · Windows 11 Pro · Dell Inc. OptiPlex 7090").
    ''' Throws the WMI/DCOM exception when the computer can't be queried (see <see cref="ConnectionErrors"/>).
    ''' </summary>
    Public Function Probe() As String
        EnsureReachable()
        Dim oScope As ManagementScope = CreateScope("root\CIMV2")
        oScope.Connect()

        Dim oParts As New List(Of String)
        Dim oComputer As ManagementBaseObject = QueryFirst(oScope, "SELECT Name, Manufacturer, Model FROM Win32_ComputerSystem")
        Dim oSystem As ManagementBaseObject = QueryFirst(oScope, "SELECT Caption FROM Win32_OperatingSystem")

        If Not oComputer Is Nothing Then oParts.Add(CStr(oComputer("Name")))
        If Not oSystem Is Nothing Then oParts.Add(CStr(oSystem("Caption")).Trim())
        If Not oComputer Is Nothing Then oParts.Add((CStr(oComputer("Manufacturer")) + " " + CStr(oComputer("Model"))).Trim())

        Return String.Join("  " + ChrW(183) + "  ", oParts)
    End Function

    ''' <summary>
    ''' Fails fast (a few seconds) when the RPC endpoint mapper (TCP 135) of a remote computer can't be
    ''' reached, instead of waiting for DCOM's own retries (20+ seconds).
    ''' </summary>
    Public Sub EnsureReachable()
        If IsLocal Then Exit Sub

        Using oClient As New Sockets.TcpClient
            Dim oConnect As Threading.Tasks.Task = oClient.ConnectAsync(Host.Trim(), RpcEndpointMapperPort)
            Dim oCompleted As Boolean
            Try
                oCompleted = oConnect.Wait(TimeSpan.FromSeconds(ReachabilityTimeoutSeconds))
            Catch ex As AggregateException
                If TypeOf ex.InnerException Is Sockets.SocketException AndAlso
                   CType(ex.InnerException, Sockets.SocketException).SocketErrorCode = Sockets.SocketError.HostNotFound Then
                    Throw New TargetUnreachableException(Host.Trim(), TargetUnreachableException.ReasonEnum.NameNotFound, ex.InnerException)
                End If
                Throw New TargetUnreachableException(Host.Trim(), TargetUnreachableException.ReasonEnum.NoResponse, ex.InnerException)
            End Try
            If Not oCompleted Then Throw New TargetUnreachableException(Host.Trim(), TargetUnreachableException.ReasonEnum.NoResponse, Nothing)
        End Using
    End Sub

    Private Shared Function QueryFirst(ByVal pScope As ManagementScope, ByVal pQuery As String) As ManagementBaseObject
        Using oSearcher As New ManagementObjectSearcher(pScope, New ObjectQuery(pQuery))
            For Each oObject As ManagementBaseObject In oSearcher.Get
                Return oObject
            Next
        End Using
        Return Nothing
    End Function

    ''' <summary>Builds (but does not connect) a scope for a WMI namespace such as "root\CIMV2" on this target.</summary>
    Public Function CreateScope(ByVal pNamespace As String) As ManagementScope
        Dim oPath As String = If(IsLocal, pNamespace, "\\" + Host.Trim() + "\" + pNamespace)
        Return New ManagementScope(oPath, BuildConnectionOptions())
    End Function
End Class

''' <summary>The remote computer could not be reached before even trying WMI.</summary>
Public Class TargetUnreachableException
    Inherits Exception

    Public Enum ReasonEnum
        NoResponse
        NameNotFound
    End Enum

    Public ReadOnly Property Reason As ReasonEnum

    Public Sub New(ByVal pHost As String, ByVal pReason As ReasonEnum, ByVal pInner As Exception)
        MyBase.New(If(pReason = ReasonEnum.NameNotFound, "Computer '" + pHost + "' not found.", "Computer '" + pHost + "' did not respond on port 135."), pInner)
        Reason = pReason
    End Sub
End Class
