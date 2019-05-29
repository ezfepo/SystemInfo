Public Class frmSystemInfo
    Private Enum StateEnum
        Init
        Started
        Stopped
    End Enum
    Private Sub SetStateToControls(ByVal pState As StateEnum)
        Select Case pState
            Case StateEnum.Started
                Me.UseWaitCursor = True
                btnGetSystemInfo.Enabled = False
            Case StateEnum.Stopped, StateEnum.Init
                Me.UseWaitCursor = False
                btnGetSystemInfo.Enabled = True
        End Select
    End Sub

    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetSystemInfo.Click
        SetStateToControls(StateEnum.Started)
        prgProgress.Value = 0

        If sender Is btnGetSystemInfo Then bwProcessSystemInfo.RunWorkerAsync(modConfigurations.Complete)

    End Sub
    Private Sub bwProcess_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwProcessSystemInfo.ProgressChanged
        Try
            prgProgress.Value = e.ProgressPercentage

            If Not e.UserState Is Nothing Then
                If e.UserState.GetType Is GetType(String) Then
                    lblProgress.Text = CStr(e.UserState)
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        Finally
            'lnkOuputLog.Text = bwProcess.LogFileName
        End Try
    End Sub
    Private Sub bwProcess_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwProcessSystemInfo.RunWorkerCompleted
        Try
            If e.Cancelled Then
                'txtLog.AppendText("Cancelled" + Environment.NewLine)
            ElseIf Not (e.Error Is Nothing) Then
                MessageBox.Show(e.Error.Message)
            Else
                'txtLog.AppendText(e.Result.ToString + Environment.NewLine)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        Finally
            'lnkOuputLog.Text = bwProcess.LogFileName
            lblProgress.Text = ""
            SetStateToControls(StateEnum.Stopped)
        End Try
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetStateToControls(StateEnum.Init)
    End Sub

    Private Sub btnDetalle_Click(sender As Object, e As EventArgs) Handles btnDetalle.Click
        Dim pfrmMain As New frmMain
        frmMain.Show(Me)
    End Sub
End Class