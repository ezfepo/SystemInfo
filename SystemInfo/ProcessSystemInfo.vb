Imports System.Management

Public Class ProcessSystemInfo
    Inherits System.ComponentModel.BackgroundWorker

    Private WithEvents _WMINamespace As WMINamespace

    Private _Progress As Double
    Private _ProgressMax As Double
    Private _ProgressClass As Double
    Private _ProgressStepClasses As Double
    Private _ProgressStepProperties As Double

    Private Sub bwProcessSystemInfo_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MyBase.DoWork
        _Progress = 0
        _ProgressMax = 100

        MyBase.ReportProgress(CInt((_ProgressMax / 3) * 1), "Loading Information")
        _WMINamespace = New WMINamespace(e.Argument)
        _WMINamespace.Load()
        _ProgressStepClasses = (_ProgressMax / _WMINamespace.Count)

        For Each oWMIClass As WMIClass In _WMINamespace
            If MyBase.CancellationPending Then Exit For

            _Progress = 0
            MyBase.ReportProgress(CInt(_Progress), "Class " + oWMIClass.Name)

            oWMIClass.Load()
            _ProgressClass = _Progress
            _ProgressStepClasses = 100 / oWMIClass.Count

            For Each oWMIManagementObject As WMIManagementObject In oWMIClass
                If MyBase.CancellationPending Then Exit For

                _Progress = _ProgressClass
                MyBase.ReportProgress(CInt(_Progress), "Class " + oWMIClass.Name + " - " + oWMIManagementObject.Name)
                oWMIManagementObject.Load()

                _ProgressStepProperties = (_ProgressStepClasses / oWMIManagementObject.Count)
                For Each oWMIPropertyData As WMIPropertyData In oWMIManagementObject
                    If MyBase.CancellationPending Then Exit For

                    _Progress += _ProgressStepProperties
                    MyBase.ReportProgress(CInt(_Progress), "Class " + oWMIClass.Name + " - " + oWMIManagementObject.Name + " - " + oWMIPropertyData.Name)
                    oWMIPropertyData.Load()
                Next
            Next
        Next

        If MyBase.CancellationPending Then
            e.Cancel = True
            e.Result = "Cancelled"
        Else
            MyBase.ReportProgress(CInt(_ProgressMax), "Saving Information")
            _WMINamespace.Save()
        End If

        If Not e.Cancel Then
            Dim pStartInfo As New System.Diagnostics.ProcessStartInfo(_WMINamespace.FileName)
            pStartInfo.UseShellExecute = True
            System.Diagnostics.Process.Start(pStartInfo)
            e.Result = "Finished"
        End If
    End Sub

    Private Sub DoCancel(ByRef pCancel As Boolean) Handles _WMINamespace.DoCancel
        pCancel = MyBase.CancellationPending
    End Sub

End Class
