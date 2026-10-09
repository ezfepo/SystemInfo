Imports System.Management

''' <summary>Snapshot of a running scan, sent to the UI with each ReportProgress call.</summary>
Public Class ScanProgress
    Public Enum StageEnum
        Connecting
        Collecting
        Saving
    End Enum

    Public Property Stage As StageEnum
    Public Property ComputerName As String = String.Empty
    ''' <summary>Overall progress of the whole scan, 0-100.</summary>
    Public Property Percent As Double
    Public Property ClassName As String = String.Empty
    ''' <summary>1-based index of the class being collected.</summary>
    Public Property ClassIndex As Integer
    Public Property ClassCount As Integer
    Public Property ObjectName As String = String.Empty
End Class

Public Class ProcessSystemInfo
    Inherits System.ComponentModel.BackgroundWorker

    Private WithEvents _WMINamespace As WMINamespace

    Private Sub bwProcessSystemInfo_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MyBase.DoWork
        Dim oRequest As SystemInfoRequest = DirectCast(e.Argument, SystemInfoRequest)

        Report(New ScanProgress With {.Stage = ScanProgress.StageEnum.Connecting, .ComputerName = oRequest.Target.DisplayName})
        _WMINamespace = New WMINamespace(oRequest)
        _WMINamespace.Connect()
        _WMINamespace.Load()

        Dim oClassCount As Integer = _WMINamespace.Count
        Dim oClassIndex As Integer = 0

        'Overall progress = finished classes + fraction of the current class (by objects collected)
        For Each oWMIClass As WMIClass In _WMINamespace
            If MyBase.CancellationPending Then Exit For

            Dim oProgress As New ScanProgress With {
                .Stage = ScanProgress.StageEnum.Collecting,
                .ComputerName = _WMINamespace.ComputerName,
                .ClassName = oWMIClass.Name,
                .ClassIndex = oClassIndex + 1,
                .ClassCount = oClassCount,
                .Percent = 100.0 * oClassIndex / oClassCount
            }
            Report(oProgress)

            oWMIClass.Load()

            Dim oObjectIndex As Integer = 0
            For Each oWMIManagementObject As WMIManagementObject In oWMIClass
                If MyBase.CancellationPending Then Exit For

                oWMIManagementObject.Load()
                For Each oWMIPropertyData As WMIPropertyData In oWMIManagementObject
                    If MyBase.CancellationPending Then Exit For
                    oWMIPropertyData.Load()
                Next

                oObjectIndex += 1
                oProgress.ObjectName = oWMIManagementObject.Name
                oProgress.Percent = 100.0 * (oClassIndex + oObjectIndex / oWMIClass.Count) / oClassCount
                Report(oProgress)
            Next

            oClassIndex += 1
        Next

        If MyBase.CancellationPending Then
            e.Cancel = True
            Exit Sub
        End If

        Report(New ScanProgress With {.Stage = ScanProgress.StageEnum.Saving, .ComputerName = _WMINamespace.ComputerName, .Percent = 100})
        _WMINamespace.Save()
        e.Result = _WMINamespace.FileName
    End Sub

    Private Sub Report(ByVal pProgress As ScanProgress)
        'Send a copy: the worker keeps updating its own instance while the UI thread reads this one
        MyBase.ReportProgress(CInt(pProgress.Percent), New ScanProgress With {
            .Stage = pProgress.Stage,
            .ComputerName = pProgress.ComputerName,
            .Percent = pProgress.Percent,
            .ClassName = pProgress.ClassName,
            .ClassIndex = pProgress.ClassIndex,
            .ClassCount = pProgress.ClassCount,
            .ObjectName = pProgress.ObjectName
        })
    End Sub

    Private Sub DoCancel(ByRef pCancel As Boolean) Handles _WMINamespace.DoCancel
        pCancel = MyBase.CancellationPending
    End Sub

End Class
