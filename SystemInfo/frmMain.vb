''' <summary>
''' Main (and only) window: scans this or a remote computer, saves the XML report and shows it.
''' </summary>
Public Class frmMain
    Private Const MaxRecentReports As Integer = 5

    Private _WMIViewer As New WMIViewer
    Private _SortingColumn As ColumnHeader
    Private _Scanning As Boolean

    ''' <summary>Report to open as soon as the window loads (used when the app restarts to change the theme).</summary>
    Public Property StartupReport As String

#Region "Startup"
    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Icon = New Icon(IO.Path.Combine(AppContext.BaseDirectory, "SystemInfo.ico"))
        picComputer.Image = New Icon(Me.Icon, 32, 32).ToBitmap()
        UiTheme.Apply(Me)

        UiTheme.StylePrimaryButton(btnScanLocal)
        UiTheme.SetGlyph(btnScanLocal, UiTheme.Glyphs.Play)
        UiTheme.StyleSecondaryButton(btnScanRemote)
        UiTheme.SetGlyph(btnScanRemote, UiTheme.Glyphs.RemoteComputer)
        UiTheme.StyleSecondaryButton(btnOpenReport)
        UiTheme.SetGlyph(btnOpenReport, UiTheme.Glyphs.OpenFolder)
        UiTheme.StyleSecondaryButton(btnCopySummary)
        UiTheme.SetGlyph(btnCopySummary, UiTheme.Glyphs.Copy)
        UiTheme.StylePrimaryButton(btnEmptyScanLocal)
        UiTheme.SetGlyph(btnEmptyScanLocal, UiTheme.Glyphs.Play)
        UiTheme.StyleSecondaryButton(btnEmptyScanRemote)
        UiTheme.SetGlyph(btnEmptyScanRemote, UiTheme.Glyphs.RemoteComputer)
        UiTheme.StyleToggleButton(rbShort)
        UiTheme.StyleToggleButton(rbSummary)
        UiTheme.StyleToggleButton(rbComplete)
        UiTheme.StyleSecondaryButton(btnTheme)
        UiTheme.SetGlyph(btnTheme, UiTheme.Glyphs.Theme)
        btnTheme.ImageAlign = ContentAlignment.MiddleCenter 'icon only

        lblBadge.BackColor = UiTheme.AccentTintColor
        lblBadge.ForeColor = UiTheme.AccentTintTextColor
        UiTheme.StyleLink(lnkOpenReport)
        ThemeApplier.ThemeToolTip(tipMain)

        'Roomier rows: the TreeView has ItemHeight; the ListView takes its row height from the small image list
        tvwClasses.ItemHeight = UiTheme.Dp(tvwClasses, 26)
        lvwProperties.SmallImageList = New ImageList With {.ImageSize = New Size(1, UiTheme.Dp(lvwProperties, 24))}

        rbShort.Checked = True
        _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Short

        ShowEmptyState()
        If Not String.IsNullOrEmpty(StartupReport) AndAlso IO.File.Exists(StartupReport) Then LoadReport(StartupReport)
    End Sub
#End Region

#Region "Report display"
    Private Sub ShowEmptyState()
        RefreshRecentReports()
        pnlEmpty.Visible = True
        splitMain.Visible = False
        UpdateCommandState()
        lblComputer.Text = "SystemInfo"
        lblDetails.Text = "Hardware and software inventory"
        lblBadge.Visible = False
    End Sub

    Private Sub LoadReport(ByVal pFilePath As String)
        Me.Cursor = Cursors.WaitCursor
        Try
            If Not _WMIViewer.Load(pFilePath) Then
                MessageBox.Show(Me, "The file is not a valid SystemInfo report:" + Environment.NewLine + pFilePath,
                                "Open report", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            UpdateHeader()
            txtClassFilter.Clear()
            txtPropertyFilter.Clear()
            FillTree()

            pnlEmpty.Visible = False
            splitMain.Visible = True
            UpdateCommandState()
            SetStatus("Loaded " + IO.Path.GetFileName(pFilePath))
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Open report", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub UpdateHeader()
        lblComputer.Text = _WMIViewer.ComputerName

        Dim oDetails As New List(Of String)
        Dim oOperatingSystem As String = _WMIViewer.GetXMLClassResume("OperatingSystem", "Caption").Trim()
        Dim oModel As String = _WMIViewer.GetXMLClassResume("ComputerSystem", "Manufacturer", "Model").Trim()
        If oOperatingSystem.Length > 0 Then oDetails.Add(oOperatingSystem)
        If oModel.Length > 0 Then oDetails.Add(oModel)
        oDetails.Add("Scanned " + IO.File.GetLastWriteTime(_WMIViewer.FilePath).ToString("g"))
        lblDetails.Text = String.Join("  " + ChrW(183) + "  ", oDetails)

        Dim oIsThisComputer As Boolean = String.Equals(_WMIViewer.ComputerName, Environment.MachineName, StringComparison.OrdinalIgnoreCase)
        lblBadge.Text = If(oIsThisComputer, "This computer", "Remote computer")
        lblBadge.Visible = True
    End Sub

    Private Sub FillTree()
        If Not _WMIViewer.IsLoaded Then Exit Sub

        Dim oSelectedName As String = If(tvwClasses.SelectedNode Is Nothing, Nothing, tvwClasses.SelectedNode.Name)

        tvwClasses.BeginUpdate()
        Try
            _WMIViewer.Fill(tvwClasses, txtClassFilter.Text)
        Finally
            tvwClasses.EndUpdate()
        End Try

        'Keep the selected class when it survives the filter
        If Not oSelectedName Is Nothing Then
            Dim oFound() As TreeNode = tvwClasses.Nodes.Find(oSelectedName, True)
            If oFound.Length > 0 Then tvwClasses.SelectedNode = oFound(0)
        End If
        If tvwClasses.SelectedNode Is Nothing Then FillProperties()
    End Sub

    Private Sub FillProperties()
        ListViewSorter.ClearSort(lvwProperties, _SortingColumn)
        _WMIViewer.Fill(lvwProperties, tvwClasses.SelectedNode, txtPropertyFilter.Text)
        ResizeValueColumn()
    End Sub

    Private Sub ResizeValueColumn()
        Dim oWidth As Integer = lvwProperties.ClientSize.Width - colProperty.Width - SystemInformation.VerticalScrollBarWidth
        colValue.Width = Math.Max(UiTheme.Dp(lvwProperties, 160), oWidth)
    End Sub

    Private Sub RefreshRecentReports()
        For Each oControl As Control In flpRecent.Controls
            oControl.Dispose()
        Next
        flpRecent.Controls.Clear()

        Dim oFiles() As IO.FileInfo = {}
        Try
            Dim oFolder As New IO.DirectoryInfo(AppConfiguration.OutputFolder)
            If oFolder.Exists Then oFiles = oFolder.GetFiles(ReportFilePrefix() + ".*.xml")
        Catch ex As Exception
            Console.WriteLine("RefreshRecentReports: " + ex.Message)
        End Try
        Array.Sort(oFiles, Function(a, b) b.LastWriteTime.CompareTo(a.LastWriteTime))

        For i As Integer = 0 To Math.Min(oFiles.Length, MaxRecentReports) - 1
            Dim oLink As New LinkLabel
            oLink.AutoSize = True
            oLink.TabStop = True 'reachable from the keyboard
            oLink.Margin = New Padding(3, 2, 3, 2)
            UiTheme.StyleLink(oLink)
            oLink.Text = oFiles(i).Name + "   " + ChrW(183) + "   " + oFiles(i).LastWriteTime.ToString("g")
            oLink.LinkArea = New LinkArea(0, oFiles(i).Name.Length)
            oLink.Tag = oFiles(i).FullName
            AddHandler oLink.LinkClicked, AddressOf RecentReport_LinkClicked
            flpRecent.Controls.Add(oLink)
        Next

        lblRecent.Visible = flpRecent.Controls.Count > 0
    End Sub

    ''' <summary>Same prefix WMINamespace.FileName uses for generated reports.</summary>
    Private Shared Function ReportFilePrefix() As String
        Return AppDomain.CurrentDomain.FriendlyName.Replace(".exe", String.Empty)
    End Function

    Private Sub SetStatus(ByVal pText As String, Optional ByVal pTemporary As Boolean = False)
        lblStatus.Text = pText
        tmrStatus.Stop()
        If pTemporary Then tmrStatus.Start()
    End Sub

    Private Sub tmrStatus_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrStatus.Tick
        tmrStatus.Stop()
        lblStatus.Text = If(_WMIViewer.IsLoaded, "Loaded " + IO.Path.GetFileName(_WMIViewer.FilePath), "Ready")
    End Sub
#End Region

#Region "Scanning"
    Private Sub StartScan(ByVal pTarget As RemoteTarget)
        If bwProcessSystemInfo.IsBusy Then Exit Sub

        SetScanning(True)
        scanProgress.Start(pTarget.DisplayName, Not pTarget.IsLocal)
        SetStatus("Scanning " + pTarget.DisplayName + ChrW(8230))
        bwProcessSystemInfo.RunWorkerAsync(New SystemInfoRequest(modConfigurations.Complete, pTarget))
    End Sub

    ''' <summary>
    ''' Locks the window while scanning: the progress panel covers the content, and every command that
    ''' could start another scan or load a report is disabled.
    ''' </summary>
    Private Sub SetScanning(ByVal pScanning As Boolean)
        _Scanning = pScanning
        btnScanLocal.Enabled = Not pScanning
        btnScanRemote.Enabled = Not pScanning
        btnOpenReport.Enabled = Not pScanning
        btnTheme.Enabled = Not pScanning
        UpdateCommandState()
        splitMain.Enabled = Not pScanning
        pnlEmpty.Enabled = Not pScanning

        scanProgress.Visible = pScanning
        If pScanning Then
            scanProgress.BringToFront()
            scanProgress.Focus()
        Else
            scanProgress.Finish()
        End If
    End Sub

    ''' <summary>
    ''' The commands that act on the open report (copy summary and the Short/Summary/Complete view) only make
    ''' sense when a report is loaded and no scan is running. The view buttons are disabled one by one, not
    ''' through their container: a disabled "View" label is drawn in a hard-to-read gray.
    ''' </summary>
    Private Sub UpdateCommandState()
        Dim oEnabled As Boolean = Not _Scanning AndAlso _WMIViewer.IsLoaded
        btnCopySummary.Enabled = oEnabled
        rbShort.Enabled = oEnabled
        rbSummary.Enabled = oEnabled
        rbComplete.Enabled = oEnabled
    End Sub

    Private Sub bwProcessSystemInfo_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwProcessSystemInfo.ProgressChanged
        Dim oProgress As ScanProgress = TryCast(e.UserState, ScanProgress)
        If Not oProgress Is Nothing Then scanProgress.UpdateProgress(oProgress)
    End Sub

    Private Sub bwProcessSystemInfo_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwProcessSystemInfo.RunWorkerCompleted
        SetScanning(False)

        If Not e.Error Is Nothing Then
            SetStatus("Scan failed")
            MessageBox.Show(Me, ConnectionErrors.Describe(e.Error), "Scan failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        ElseIf e.Cancelled Then
            SetStatus("Scan cancelled", True)
        Else
            LoadReport(CStr(e.Result))
            If Not _WMIViewer.IsLoaded Then RefreshRecentReports()
        End If
    End Sub

    Private Sub scanProgress_CancelRequested(ByVal sender As Object, ByVal e As EventArgs) Handles scanProgress.CancelRequested
        If Not bwProcessSystemInfo.IsBusy Then Exit Sub
        lblStatus.Text = "Cancelling" + ChrW(8230)
        bwProcessSystemInfo.CancelAsync()
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If Not bwProcessSystemInfo.IsBusy Then Exit Sub

        If MessageBox.Show(Me, "A scan is in progress. Cancel it and close SystemInfo?", "SystemInfo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            bwProcessSystemInfo.CancelAsync()
        Else
            e.Cancel = True
        End If
    End Sub
#End Region

#Region "Commands"
    Private Sub btnScanLocal_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnScanLocal.Click, btnEmptyScanLocal.Click
        StartScan(RemoteTarget.Local)
    End Sub

    Private Sub btnScanRemote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnScanRemote.Click, btnEmptyScanRemote.Click
        If bwProcessSystemInfo.IsBusy Then Exit Sub
        Using oDialog As New frmRemoteTarget
            If oDialog.ShowDialog(Me) = DialogResult.OK Then StartScan(oDialog.Target)
        End Using
    End Sub

    Private Sub btnTheme_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnTheme.Click
        Dim oCurrent As String = AppConfiguration.Theme
        mnuThemeAuto.Checked = oCurrent = "auto"
        mnuThemeLight.Checked = oCurrent = "light"
        mnuThemeDark.Checked = oCurrent = "dark"
        cmsTheme.Show(btnTheme, New Point(0, btnTheme.Height))
    End Sub

    Private Sub mnuTheme_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuThemeAuto.Click, mnuThemeLight.Click, mnuThemeDark.Click
        Dim oTheme As String = If(sender Is mnuThemeLight, "light", If(sender Is mnuThemeDark, "dark", "auto"))
        If oTheme = AppConfiguration.Theme Then Exit Sub

        UserSettings.Theme = oTheme
        If MessageBox.Show(Me, "SystemInfo has to restart to apply the new theme. Restart now?", "SystemInfo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            RestartApplication()
        Else
            SetStatus("Theme saved: it applies the next time SystemInfo starts", True)
        End If
    End Sub

    ''' <summary>Starts a new instance (reopening the current report, if any) and closes this one.</summary>
    Private Sub RestartApplication()
        Dim oStart As New ProcessStartInfo(Environment.ProcessPath) With {.UseShellExecute = False}
        'Started with "dotnet SystemInfo.dll": the process is dotnet.exe and the first argument must be the dll
        If String.Equals(IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath), "dotnet", StringComparison.OrdinalIgnoreCase) Then
            oStart.ArgumentList.Add(Environment.GetCommandLineArgs()(0))
        End If
        If _WMIViewer.IsLoaded Then oStart.ArgumentList.Add(_WMIViewer.FilePath)

        Process.Start(oStart)
        Me.Close()
    End Sub

    Private Sub btnOpenReport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpenReport.Click
        OpenReport()
    End Sub

    Private Sub lnkOpenReport_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles lnkOpenReport.LinkClicked
        OpenReport()
    End Sub

    Private Sub OpenReport()
        If bwProcessSystemInfo.IsBusy Then Exit Sub
        opfMain.InitialDirectory = AppConfiguration.OutputFolder
        opfMain.FileName = String.Empty
        If opfMain.ShowDialog(Me) = DialogResult.OK Then LoadReport(opfMain.FileName)
    End Sub

    Private Sub RecentReport_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs)
        LoadReport(CStr(DirectCast(sender, LinkLabel).Tag))
    End Sub

    Private Sub btnCopySummary_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCopySummary.Click
        If Not _WMIViewer.IsLoaded Then Exit Sub
        Dim oResume As String = _WMIViewer.Resume()
        If String.IsNullOrEmpty(oResume) Then Exit Sub
        Clipboard.SetText(oResume)
        SetStatus("Summary copied to clipboard", True)
    End Sub

    Private Sub rbConfigurationType_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rbShort.CheckedChanged, rbSummary.CheckedChanged, rbComplete.CheckedChanged
        Dim oButton As RadioButton = DirectCast(sender, RadioButton)
        If Not oButton.Checked Then Exit Sub

        If oButton Is rbShort Then
            _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Short
        ElseIf oButton Is rbSummary Then
            _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Resume
        Else
            _WMIViewer.ConfigurationType = ConfigurationTypeEnum.Complete
        End If

        FillTree()
    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.F AndAlso splitMain.Visible AndAlso splitMain.Enabled Then
            txtPropertyFilter.Focus()
            txtPropertyFilter.SelectAll()
            e.Handled = True
        ElseIf e.Control AndAlso e.KeyCode = Keys.O Then
            OpenReport()
            e.Handled = True
        End If
    End Sub
#End Region

#Region "Tree and list"
    Private Sub tvwClasses_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles tvwClasses.AfterSelect
        FillProperties()
    End Sub

    Private Sub txtClassFilter_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtClassFilter.TextChanged
        FillTree()
    End Sub

    Private Sub txtPropertyFilter_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtPropertyFilter.TextChanged
        FillProperties()
    End Sub

    Private Sub lvwProperties_ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs) Handles lvwProperties.ColumnClick
        lvwProperties.ShowGroups = False
        ListViewSorter.Sort(lvwProperties, _SortingColumn, e.Column)
    End Sub

    Private Sub lvwProperties_Resize(ByVal sender As Object, ByVal e As EventArgs) Handles lvwProperties.Resize
        ResizeValueColumn()
    End Sub

    Private Sub cmsProperties_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cmsProperties.Opening
        e.Cancel = lvwProperties.SelectedItems.Count = 0
    End Sub

    Private Sub mnuCopyValue_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuCopyValue.Click
        If lvwProperties.SelectedItems.Count = 0 Then Exit Sub
        Clipboard.SetText(lvwProperties.SelectedItems(0).SubItems(1).Text)
        SetStatus("Value copied to clipboard", True)
    End Sub

    Private Sub mnuCopyRow_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuCopyRow.Click
        If lvwProperties.SelectedItems.Count = 0 Then Exit Sub
        Dim oItem As ListViewItem = lvwProperties.SelectedItems(0)
        Clipboard.SetText(oItem.Text + vbTab + oItem.SubItems(1).Text)
        SetStatus("Row copied to clipboard", True)
    End Sub
#End Region

End Class
