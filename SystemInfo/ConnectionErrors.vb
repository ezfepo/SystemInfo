Imports System.Management
Imports System.Runtime.InteropServices

''' <summary>
''' Turns the low-level exceptions WMI/DCOM throws while connecting into messages a user can act on.
''' </summary>
Public Module ConnectionErrors
    Private Const E_ACCESSDENIED As Integer = &H80070005
    Private Const ERROR_LOGON_FAILURE As Integer = &H8007052E
    Private Const RPC_S_SERVER_UNAVAILABLE As Integer = &H800706BA
    Private Const RPC_S_CALL_FAILED As Integer = &H800706BE
    Private Const RPC_E_OBJECT_EXPORTER_NOT_FOUND As Integer = &H80070776
    Private Const RPC_S_UNKNOWN_IF As Integer = &H800706B5

    ''' <summary>User-facing message plus the technical code, so a screenshot is enough to diagnose.</summary>
    Public Function Describe(ByVal pException As Exception) As String
        Dim oException As Exception = pException
        If TypeOf oException Is AggregateException AndAlso Not oException.InnerException Is Nothing Then
            oException = oException.InnerException
        End If

        Dim oMessage As String = Translate(oException)
        Dim oDetails As String = oException.GetType().Name + " 0x" + oException.HResult.ToString("X8")
        If Not String.Equals(oMessage, oException.Message) Then oDetails += ": " + oException.Message.Trim()

        Return oMessage + Environment.NewLine + Environment.NewLine + "Details: " + oDetails
    End Function

    Private Function Translate(ByVal oException As Exception) As String
        Dim oUnreachable As TargetUnreachableException = TryCast(oException, TargetUnreachableException)
        If Not oUnreachable Is Nothing Then
            If oUnreachable.Reason = TargetUnreachableException.ReasonEnum.NameNotFound Then
                Return "The computer name could not be found on the network. Check the name, or use its IP address."
            End If
            Return Unreachable()
        End If

        If TypeOf oException Is UnauthorizedAccessException Then Return AccessDenied()

        Dim oManagement As ManagementException = TryCast(oException, ManagementException)
        If Not oManagement Is Nothing Then
            Select Case oManagement.ErrorCode
                Case ManagementStatus.AccessDenied
                    Return AccessDenied()
                Case ManagementStatus.Timedout
                    Return TimedOut()
                Case ManagementStatus.InvalidNamespace, ManagementStatus.InvalidClass
                    Return "The remote computer does not provide the requested WMI information (" + oManagement.Message.Trim() + ")."
            End Select
        End If

        Dim oCom As COMException = TryCast(oException, COMException)
        If Not oCom Is Nothing Then
            Select Case oCom.ErrorCode
                Case E_ACCESSDENIED
                    Return AccessDenied()
                Case ERROR_LOGON_FAILURE
                    Return "The user name or password is incorrect."
                Case RPC_S_SERVER_UNAVAILABLE, RPC_E_OBJECT_EXPORTER_NOT_FOUND, RPC_S_UNKNOWN_IF, RPC_S_CALL_FAILED
                    Return Unreachable()
            End Select
        End If

        If TypeOf oException Is TimeoutException Then Return TimedOut()

        Return oException.Message
    End Function

    Private Function AccessDenied() As String
        Return "Access denied. Check the user name and password. The most reliable option is the 'systeminfo' " +
               "account created by the remote computer setup; Microsoft accounts that sign in with a PIN often fail. " +
               "On computers without a domain, the setup must also have been run (LocalAccountTokenFilterPolicy)."
    End Function

    Private Function Unreachable() As String
        Return "Cannot reach the computer. Check that it is on, that the name or IP address is correct, " +
               "that it is on the same subnet, and that the WMI firewall rules are enabled (remote computer setup)."
    End Function

    Private Function TimedOut() As String
        Return "The computer did not respond in time. Check that it is on and reachable."
    End Function
End Module
