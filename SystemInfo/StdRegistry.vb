Imports System.Management

''' <summary>
''' Reads HKEY_LOCAL_MACHINE of the target computer through the WMI StdRegProv provider, so the same code
''' works locally and remotely without the Remote Registry service. Every read returns Nothing (or an empty
''' array) when the key or value does not exist or can't be read.
''' </summary>
Friend NotInheritable Class StdRegistry
    Implements IDisposable

    Private Const HKEY_LOCAL_MACHINE As UInteger = &H80000002UI

    Private ReadOnly _Provider As ManagementClass

    Friend Sub New(ByVal pTarget As RemoteTarget)
        Dim oScope As ManagementScope = pTarget.CreateScope("root\default")
        oScope.Connect()
        _Provider = New ManagementClass(oScope, New ManagementPath("StdRegProv"), Nothing)
    End Sub

    Friend Function EnumKey(ByVal pSubKey As String) As String()
        Using oResult As ManagementBaseObject = Invoke("EnumKey", pSubKey, Nothing)
            If oResult Is Nothing Then Return New String() {}
            Return If(TryCast(oResult("sNames"), String()), New String() {})
        End Using
    End Function

    Friend Function GetString(ByVal pSubKey As String, ByVal pValueName As String) As String
        Using oResult As ManagementBaseObject = Invoke("GetStringValue", pSubKey, pValueName)
            Return If(oResult Is Nothing, Nothing, TryCast(oResult("sValue"), String))
        End Using
    End Function

    Friend Function GetDWord(ByVal pSubKey As String, ByVal pValueName As String) As Long?
        Using oResult As ManagementBaseObject = Invoke("GetDWORDValue", pSubKey, pValueName)
            If oResult Is Nothing OrElse oResult("uValue") Is Nothing Then Return Nothing
            Return Convert.ToInt64(oResult("uValue"))
        End Using
    End Function

    ''' <summary>Calls a StdRegProv method; returns Nothing when it reports an error (missing key or value).</summary>
    Private Function Invoke(ByVal pMethod As String, ByVal pSubKey As String, ByVal pValueName As String) As ManagementBaseObject
        Dim oInParams As ManagementBaseObject = _Provider.GetMethodParameters(pMethod)
        oInParams("hDefKey") = HKEY_LOCAL_MACHINE
        oInParams("sSubKeyName") = pSubKey
        If Not pValueName Is Nothing Then oInParams("sValueName") = pValueName

        Dim oOutParams As ManagementBaseObject = _Provider.InvokeMethod(pMethod, oInParams, Nothing)
        If Convert.ToUInt32(oOutParams("ReturnValue")) <> 0UI Then
            oOutParams.Dispose()
            Return Nothing
        End If
        Return oOutParams
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        _Provider.Dispose()
    End Sub
End Class
