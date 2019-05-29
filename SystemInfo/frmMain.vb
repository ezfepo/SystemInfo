Public Class frmMain
    Private _WMIViewer As New WMIViewer

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        optShort.Checked = True
        _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Short
    End Sub

    Private Sub btnLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoad.Click
        Me.Enabled = False
        Me.Cursor = Cursors.WaitCursor

        Try
            opfMain.InitialDirectory = AppConfiguration.OutputFolder
            opfMain.ShowDialog(Me)

            If opfMain.FileName.Length > 0 Then
                If _WMIViewer.Load(opfMain.FileName) Then
                    _WMIViewer.Fill(tvwMain)
                    lvwMain.Items.Clear()
                End If
            End If
        Catch ex As Exception
        Finally
            Me.Enabled = True
            Me.Cursor = Cursors.Default
        End Try
    End Sub
    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Hide()
        Me.Close()
    End Sub
    Private Sub optConfigurationType_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optShort.Click, optResume.Click, optComplete.Click
        optShort.Checked = False
        optResume.Checked = False
        optComplete.Checked = False

        If sender Is optShort Then
            optShort.Checked = True
            _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Short
        ElseIf sender Is optResume Then
            optResume.Checked = True
            _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Resume
        ElseIf sender Is optComplete Then
            optComplete.Checked = True
            _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Complete
        End If
    End Sub

    Private Sub tvwMain_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvwMain.AfterSelect
        Me.Enabled = False
        Me.Cursor = Cursors.WaitCursor

        Try
            ListViewSorter.ClearSort(lvwMain, _SortingColumn)
            _WMIViewer.Fill(lvwMain, e.Node)
        Catch ex As Exception
        Finally
            Me.Enabled = True
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private _SortingColumn As ColumnHeader
    Private Sub lvwResourceItems_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lvwMain.ColumnClick
        lvwMain.ShowGroups = False
        ListViewSorter.Sort(lvwMain, _SortingColumn, e.Column)
    End Sub

    Private Sub btnTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTest.Click
        WMIExtMonitor.GetMonitors()

        'Dim oDisplayDetails As New DisplayDetails
        ''oDisplayDetails.GetMonitorDetails()
        'oDisplayDetails.StartTest()
    End Sub

    Private Sub btnResumen_Click(sender As Object, e As EventArgs) Handles btnResumen.Click
        Dim oResume As String = _WMIViewer.Resume()
        If Not String.IsNullOrEmpty(oResume) Then Clipboard.SetText(oResume)
    End Sub
End Class
