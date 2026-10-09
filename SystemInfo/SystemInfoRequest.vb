''' <summary>
''' Argument of <see cref="ProcessSystemInfo"/>: which classes to collect and from which computer.
''' </summary>
Public Class SystemInfoRequest
    Public ReadOnly Property Configuration As XmlDocument
    Public ReadOnly Property Target As RemoteTarget

    Public Sub New(ByVal pConfiguration As XmlDocument, ByVal pTarget As RemoteTarget)
        Configuration = pConfiguration
        Target = pTarget
    End Sub
End Class
