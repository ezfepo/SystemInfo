Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Threading
Imports System.Threading.Tasks

''' <summary>A computer found on the local network that accepts RPC/DCOM connections (what remote WMI needs).</summary>
Public Class DiscoveredHost
    Public Property Address As String = String.Empty
    Public Property Name As String = String.Empty

    ''' <summary>What to put in the Computer box: the name when it resolves, the IP address otherwise.</summary>
    Public ReadOnly Property Host As String
        Get
            Return If(Name.Length > 0, Name, Address)
        End Get
    End Property
End Class

''' <summary>
''' Looks for computers on the subnets of this PC. A workgroup has no directory to ask, so it probes TCP 135
''' (RPC endpoint mapper) on every address: hosts that don't answer there can't be scanned through WMI anyway.
''' </summary>
Public Module NetworkDiscovery
    Private Const RpcEndpointMapperPort As Integer = 135
    Private Const ConnectTimeoutMilliseconds As Integer = 800
    Private Const NameTimeoutMilliseconds As Integer = 2000
    Private Const MaxParallelProbes As Integer = 128

    ''' <summary>The addresses to probe (this PC excluded) and a label for the subnets they come from.</summary>
    Public Function GetCandidates(ByRef pDescription As String) As List(Of IPAddress)
        Dim oAddresses As New List(Of IPAddress)
        Dim oSeen As New HashSet(Of UInteger)
        Dim oLabels As New List(Of String)
        Dim oLocal As New HashSet(Of UInteger)

        For Each oNic As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
            If oNic.OperationalStatus <> OperationalStatus.Up Then Continue For
            If oNic.NetworkInterfaceType = NetworkInterfaceType.Loopback OrElse oNic.NetworkInterfaceType = NetworkInterfaceType.Tunnel Then Continue For
            For Each oInfo As UnicastIPAddressInformation In oNic.GetIPProperties().UnicastAddresses
                If oInfo.Address.AddressFamily = AddressFamily.InterNetwork Then oLocal.Add(ToNumber(oInfo.Address))
            Next
        Next

        For Each oNic As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces()
            If oNic.OperationalStatus <> OperationalStatus.Up Then Continue For
            If oNic.NetworkInterfaceType = NetworkInterfaceType.Loopback OrElse oNic.NetworkInterfaceType = NetworkInterfaceType.Tunnel Then Continue For

            For Each oInfo As UnicastIPAddressInformation In oNic.GetIPProperties().UnicastAddresses
                If oInfo.Address.AddressFamily <> AddressFamily.InterNetwork Then Continue For
                Dim oBytes As Byte() = oInfo.Address.GetAddressBytes()
                If oBytes(0) = 169 AndAlso oBytes(1) = 254 Then Continue For 'link-local: no DHCP answer, nobody else there

                Dim oPrefix As Integer = Math.Max(oInfo.PrefixLength, 22) 'bigger networks: only the /22 around this PC
                If oPrefix >= 31 Then Continue For
                Dim oMask As UInteger = UInteger.MaxValue << (32 - oPrefix)
                Dim oOwn As UInteger = ToNumber(oInfo.Address)
                Dim oNetwork As UInteger = oOwn And oMask
                Dim oBroadcast As UInteger = oNetwork Or Not oMask
                oLabels.Add(FromNumber(oNetwork).ToString() + "/" + oPrefix.ToString())

                For oNumber As UInteger = oNetwork + 1UI To oBroadcast - 1UI
                    If oLocal.Contains(oNumber) Then Continue For
                    If oSeen.Add(oNumber) Then oAddresses.Add(FromNumber(oNumber))
                Next
            Next
        Next

        pDescription = If(oLabels.Count = 0, String.Empty, String.Join(", ", oLabels))
        Return oAddresses
    End Function

    ''' <summary>
    ''' Probes every address. pFound is called for each computer as soon as it answers and pChecked after each
    ''' address, both from worker threads (wrap them in Progress(Of T) to reach the UI).
    ''' </summary>
    Public Async Function ScanAsync(ByVal pAddresses As List(Of IPAddress), ByVal pFound As IProgress(Of DiscoveredHost), ByVal pChecked As IProgress(Of Integer), ByVal pToken As CancellationToken) As Task
        Dim oGate As New SemaphoreSlim(MaxParallelProbes)
        Dim oTasks As New List(Of Task)
        Dim oChecked As Integer = 0

        Try
            For Each oAddress As IPAddress In pAddresses
                Await oGate.WaitAsync(pToken)
                Dim oTarget As IPAddress = oAddress
                oTasks.Add(Task.Run(Async Function()
                                        Try
                                            Dim oHost As DiscoveredHost = Await ProbeAsync(oTarget, pToken)
                                            If Not oHost Is Nothing AndAlso Not pToken.IsCancellationRequested Then pFound.Report(oHost)
                                        Finally
                                            oGate.Release()
                                            pChecked.Report(Interlocked.Increment(oChecked))
                                        End Try
                                    End Function))
            Next
        Catch ex As OperationCanceledException
            'Stopped: wait for the probes already started
        End Try

        Try
            Await Task.WhenAll(oTasks)
        Catch ex As Exception
            Console.WriteLine("NetworkDiscovery.ScanAsync: " + ex.Message)
        End Try
    End Function

    Private Async Function ProbeAsync(ByVal pAddress As IPAddress, ByVal pToken As CancellationToken) As Task(Of DiscoveredHost)
        Try
            Using oClient As New TcpClient
                Using oTimeout As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(pToken)
                    oTimeout.CancelAfter(ConnectTimeoutMilliseconds)
                    Await oClient.ConnectAsync(pAddress, RpcEndpointMapperPort, oTimeout.Token)
                End Using
            End Using
        Catch ex As Exception When TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is SocketException
            Return Nothing
        End Try

        Return New DiscoveredHost With {.Address = pAddress.ToString(), .Name = Await ResolveNameAsync(pAddress, pToken)}
    End Function

    Private Async Function ResolveNameAsync(ByVal pAddress As IPAddress, ByVal pToken As CancellationToken) As Task(Of String)
        Try
            Using oTimeout As CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(pToken)
                oTimeout.CancelAfter(NameTimeoutMilliseconds)
                Dim oEntry As IPHostEntry = Await Dns.GetHostEntryAsync(pAddress.ToString(), oTimeout.Token)
                Dim oName As String = If(oEntry.HostName, String.Empty)
                Return If(oName = pAddress.ToString(), String.Empty, oName)
            End Using
        Catch ex As Exception When TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is SocketException
            Return String.Empty 'no reverse DNS record: the IP address is shown instead
        End Try
    End Function

    Private Function ToNumber(ByVal pAddress As IPAddress) As UInteger
        Dim oBytes As Byte() = pAddress.GetAddressBytes()
        Return (CUInt(oBytes(0)) << 24) Or (CUInt(oBytes(1)) << 16) Or (CUInt(oBytes(2)) << 8) Or oBytes(3)
    End Function

    Private Function FromNumber(ByVal pNumber As UInteger) As IPAddress
        Return New IPAddress({CByte(pNumber >> 24), CByte((pNumber >> 16) And 255UI), CByte((pNumber >> 8) And 255UI), CByte(pNumber And 255UI)})
    End Function
End Module
