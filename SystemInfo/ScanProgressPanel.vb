''' <summary>
''' Full-content overlay shown while a scan runs: what is being scanned, overall progress, the class
''' and object being read, elapsed time and a Cancel button.
''' </summary>
Public Class ScanProgressPanel
    Private ReadOnly _Elapsed As New Stopwatch

    Public Event CancelRequested As EventHandler

    Private Sub ScanProgressPanel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        UiTheme.StyleSecondaryButton(btnCancel)
        UiTheme.SetGlyph(btnCancel, UiTheme.Glyphs.Cancel)
    End Sub

    ''' <summary>Resets the panel for a new scan of <paramref name="pComputerName"/>.</summary>
    Public Sub Start(ByVal pComputerName As String, ByVal pIsRemote As Boolean)
        picIcon.Image = UiTheme.GlyphIcon(If(pIsRemote, UiTheme.Glyphs.RemoteComputer, UiTheme.Glyphs.Computer),
                                          UiTheme.Dp(Me, 36), UiTheme.AccentOnSurface)
        lblTitle.Text = "Scanning " + pComputerName
        lblStage.Text = "Connecting" + ChrW(8230)
        lblPercent.Text = String.Empty
        lblDetail.Text = If(pIsRemote, "Opening a WMI connection to the remote computer", "Opening a WMI connection")
        lblElapsed.Text = "Elapsed 0:00"
        btnCancel.Enabled = True
        btnCancel.Text = "Cancel scan"

        prgScan.Reset()
        prgScan.Active = False
        prgScan.Indeterminate = True
        _Elapsed.Restart()
        tmrElapsed.Start()
    End Sub

    Public Sub UpdateProgress(ByVal pProgress As ScanProgress)
        Select Case pProgress.Stage
            Case ScanProgress.StageEnum.Connecting
                Exit Sub

            Case ScanProgress.StageEnum.Collecting
                prgScan.Indeterminate = False
                prgScan.Active = True
                prgScan.Value = pProgress.Percent
                lblTitle.Text = "Scanning " + pProgress.ComputerName
                lblStage.Text = pProgress.ClassName.Replace("Win32_", "") + "   " + ChrW(183) + "   class " +
                                pProgress.ClassIndex.ToString() + " of " + pProgress.ClassCount.ToString()
                lblDetail.Text = If(String.IsNullOrEmpty(pProgress.ObjectName), "Querying " + pProgress.ClassName, pProgress.ObjectName)

            Case ScanProgress.StageEnum.Saving
                prgScan.Indeterminate = False
                prgScan.Active = False
                prgScan.Value = 100
                lblStage.Text = "Saving report" + ChrW(8230)
                lblDetail.Text = String.Empty
        End Select

        lblPercent.Text = CInt(Math.Floor(pProgress.Percent)).ToString() + " %"
    End Sub

    Public Sub Finish()
        tmrElapsed.Stop()
        _Elapsed.Stop()
        prgScan.Indeterminate = False
        prgScan.Active = False
    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        btnCancel.Enabled = False
        btnCancel.Text = "Cancelling" + ChrW(8230)
        lblStage.Text = "Cancelling" + ChrW(8230)
        RaiseEvent CancelRequested(Me, EventArgs.Empty)
    End Sub

    Private Sub tmrElapsed_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrElapsed.Tick
        lblElapsed.Text = "Elapsed " + _Elapsed.Elapsed.ToString("m\:ss")
    End Sub
End Class
