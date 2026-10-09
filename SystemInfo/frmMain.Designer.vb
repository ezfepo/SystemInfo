<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.pnlHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.picComputer = New System.Windows.Forms.PictureBox()
        Me.lblComputer = New System.Windows.Forms.Label()
        Me.lblDetails = New System.Windows.Forms.Label()
        Me.lblBadge = New System.Windows.Forms.Label()
        Me.btnTheme = New System.Windows.Forms.Button()
        Me.cmsTheme = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuThemeAuto = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuThemeLight = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuThemeDark = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnlHeaderLine = New System.Windows.Forms.Panel()
        Me.pnlCommands = New System.Windows.Forms.TableLayoutPanel()
        Me.flpActions = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnScanLocal = New System.Windows.Forms.Button()
        Me.btnScanRemote = New System.Windows.Forms.Button()
        Me.btnOpenReport = New System.Windows.Forms.Button()
        Me.btnCopySummary = New System.Windows.Forms.Button()
        Me.flpView = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblView = New System.Windows.Forms.Label()
        Me.rbShort = New System.Windows.Forms.RadioButton()
        Me.rbSummary = New System.Windows.Forms.RadioButton()
        Me.rbComplete = New System.Windows.Forms.RadioButton()
        Me.pnlCommandsLine = New System.Windows.Forms.Panel()
        Me.stsMain = New System.Windows.Forms.StatusStrip()
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel()
                        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.splitMain = New System.Windows.Forms.SplitContainer()
        Me.tlpClasses = New System.Windows.Forms.TableLayoutPanel()
        Me.txtClassFilter = New System.Windows.Forms.TextBox()
        Me.tvwClasses = New System.Windows.Forms.TreeView()
        Me.tlpProperties = New System.Windows.Forms.TableLayoutPanel()
        Me.txtPropertyFilter = New System.Windows.Forms.TextBox()
        Me.lvwProperties = New System.Windows.Forms.ListView()
        Me.colProperty = New System.Windows.Forms.ColumnHeader()
        Me.colValue = New System.Windows.Forms.ColumnHeader()
        Me.cmsProperties = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuCopyValue = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCopyRow = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnlEmpty = New System.Windows.Forms.TableLayoutPanel()
        Me.scanProgress = New CDE.SystemInfo.ScanProgressPanel()
        Me.flpEmpty = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblEmptyTitle = New System.Windows.Forms.Label()
        Me.lblEmptyText = New System.Windows.Forms.Label()
        Me.flpEmptyActions = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnEmptyScanLocal = New System.Windows.Forms.Button()
        Me.btnEmptyScanRemote = New System.Windows.Forms.Button()
        Me.lblRecent = New System.Windows.Forms.Label()
        Me.flpRecent = New System.Windows.Forms.FlowLayoutPanel()
        Me.lnkOpenReport = New System.Windows.Forms.LinkLabel()
        Me.opfMain = New System.Windows.Forms.OpenFileDialog()
        Me.bwProcessSystemInfo = New CDE.SystemInfo.ProcessSystemInfo()
        Me.tmrStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.pnlHeader.SuspendLayout()
        CType(Me.picComputer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlCommands.SuspendLayout()
        Me.flpActions.SuspendLayout()
        Me.flpView.SuspendLayout()
        Me.stsMain.SuspendLayout()
        Me.pnlContent.SuspendLayout()
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitMain.Panel1.SuspendLayout()
        Me.splitMain.Panel2.SuspendLayout()
        Me.splitMain.SuspendLayout()
        Me.tlpClasses.SuspendLayout()
        Me.tlpProperties.SuspendLayout()
        Me.cmsProperties.SuspendLayout()
        Me.cmsTheme.SuspendLayout()
        Me.pnlEmpty.SuspendLayout()
        Me.flpEmpty.SuspendLayout()
        Me.flpEmptyActions.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.AutoSize = True
        Me.pnlHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlHeader.BackColor = System.Drawing.SystemColors.Window
        Me.pnlHeader.ColumnCount = 4
        Me.pnlHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48.0!))
        Me.pnlHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.pnlHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.pnlHeader.Controls.Add(Me.picComputer, 0, 0)
        Me.pnlHeader.Controls.Add(Me.lblComputer, 1, 0)
        Me.pnlHeader.Controls.Add(Me.lblDetails, 1, 1)
        Me.pnlHeader.Controls.Add(Me.lblBadge, 2, 0)
        Me.pnlHeader.Controls.Add(Me.btnTheme, 3, 0)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Padding = New System.Windows.Forms.Padding(16, 12, 16, 12)
        Me.pnlHeader.RowCount = 2
        Me.pnlHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.pnlHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.pnlHeader.Size = New System.Drawing.Size(1000, 72)
        Me.pnlHeader.TabIndex = 0
        '
        'picComputer
        '
        Me.picComputer.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.picComputer.Location = New System.Drawing.Point(16, 20)
        Me.picComputer.Margin = New System.Windows.Forms.Padding(0, 0, 12, 0)
        Me.picComputer.Name = "picComputer"
        Me.pnlHeader.SetRowSpan(Me.picComputer, 2)
        Me.picComputer.Size = New System.Drawing.Size(32, 32)
        Me.picComputer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picComputer.TabIndex = 0
        Me.picComputer.TabStop = False
        '
        'lblComputer
        '
        Me.lblComputer.AutoSize = True
        Me.lblComputer.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.lblComputer.Location = New System.Drawing.Point(64, 12)
        Me.lblComputer.Margin = New System.Windows.Forms.Padding(0)
        Me.lblComputer.Name = "lblComputer"
        Me.lblComputer.Size = New System.Drawing.Size(84, 21)
        Me.lblComputer.TabIndex = 1
        Me.lblComputer.Text = "SystemInfo"
        '
        'lblDetails
        '
        Me.lblDetails.AutoEllipsis = True
        Me.lblDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDetails.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblDetails.Location = New System.Drawing.Point(64, 33)
        Me.lblDetails.Margin = New System.Windows.Forms.Padding(0, 2, 0, 0)
        Me.lblDetails.Name = "lblDetails"
        Me.lblDetails.Size = New System.Drawing.Size(820, 27)
        Me.lblDetails.TabIndex = 2
        Me.lblDetails.Text = "Hardware and software inventory"
        '
        'lblBadge
        '
        Me.lblBadge.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.lblBadge.AutoSize = True
        Me.lblBadge.Location = New System.Drawing.Point(884, 25)
        Me.lblBadge.Margin = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.lblBadge.Name = "lblBadge"
        Me.lblBadge.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
        Me.pnlHeader.SetRowSpan(Me.lblBadge, 2)
        Me.lblBadge.Size = New System.Drawing.Size(100, 21)
        Me.lblBadge.TabIndex = 3
        Me.lblBadge.Text = "This computer"
        Me.lblBadge.Visible = False
        '
        'btnTheme
        '
        Me.btnTheme.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.btnTheme.Location = New System.Drawing.Point(996, 20)
        Me.btnTheme.Margin = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.btnTheme.MinimumSize = New System.Drawing.Size(40, 32)
        Me.btnTheme.Name = "btnTheme"
        Me.pnlHeader.SetRowSpan(Me.btnTheme, 2)
        Me.btnTheme.Size = New System.Drawing.Size(40, 32)
        Me.btnTheme.TabIndex = 4
        Me.tipMain.SetToolTip(Me.btnTheme, "Color theme: Windows default, light or dark")
        '
        'cmsTheme
        '
        Me.cmsTheme.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuThemeAuto, Me.mnuThemeLight, Me.mnuThemeDark})
        Me.cmsTheme.Name = "cmsTheme"
        Me.cmsTheme.Size = New System.Drawing.Size(180, 70)
        '
        'mnuThemeAuto
        '
        Me.mnuThemeAuto.Name = "mnuThemeAuto"
        Me.mnuThemeAuto.Size = New System.Drawing.Size(179, 22)
        Me.mnuThemeAuto.Text = "Windows default"
        '
        'mnuThemeLight
        '
        Me.mnuThemeLight.Name = "mnuThemeLight"
        Me.mnuThemeLight.Size = New System.Drawing.Size(179, 22)
        Me.mnuThemeLight.Text = "Light"
        '
        'mnuThemeDark
        '
        Me.mnuThemeDark.Name = "mnuThemeDark"
        Me.mnuThemeDark.Size = New System.Drawing.Size(179, 22)
        Me.mnuThemeDark.Text = "Dark"
        '
        'pnlHeaderLine
        '
        Me.pnlHeaderLine.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlHeaderLine.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeaderLine.Location = New System.Drawing.Point(0, 72)
        Me.pnlHeaderLine.Name = "pnlHeaderLine"
        Me.pnlHeaderLine.Size = New System.Drawing.Size(1000, 1)
        Me.pnlHeaderLine.TabIndex = 1
        '
        'pnlCommands
        '
        Me.pnlCommands.AutoSize = True
        Me.pnlCommands.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlCommands.BackColor = System.Drawing.SystemColors.Window
        Me.pnlCommands.ColumnCount = 2
        Me.pnlCommands.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlCommands.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.pnlCommands.Controls.Add(Me.flpActions, 0, 0)
        Me.pnlCommands.Controls.Add(Me.flpView, 1, 0)
        Me.pnlCommands.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlCommands.Location = New System.Drawing.Point(0, 73)
        Me.pnlCommands.Name = "pnlCommands"
        Me.pnlCommands.Padding = New System.Windows.Forms.Padding(12, 8, 12, 8)
        Me.pnlCommands.RowCount = 1
        Me.pnlCommands.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.pnlCommands.Size = New System.Drawing.Size(1000, 56)
        Me.pnlCommands.TabIndex = 2
        '
        'flpActions
        '
        Me.flpActions.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.flpActions.AutoSize = True
        Me.flpActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpActions.Controls.Add(Me.btnScanLocal)
        Me.flpActions.Controls.Add(Me.btnScanRemote)
        Me.flpActions.Controls.Add(Me.btnOpenReport)
        Me.flpActions.Controls.Add(Me.btnCopySummary)
        Me.flpActions.Location = New System.Drawing.Point(12, 8)
        Me.flpActions.Margin = New System.Windows.Forms.Padding(0)
        Me.flpActions.Name = "flpActions"
        Me.flpActions.Size = New System.Drawing.Size(600, 40)
        Me.flpActions.TabIndex = 0
        Me.flpActions.WrapContents = False
        '
        'btnScanLocal
        '
        Me.btnScanLocal.AutoSize = True
        Me.btnScanLocal.Location = New System.Drawing.Point(4, 4)
        Me.btnScanLocal.Margin = New System.Windows.Forms.Padding(4)
        Me.btnScanLocal.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnScanLocal.Name = "btnScanLocal"
        Me.btnScanLocal.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        Me.btnScanLocal.Size = New System.Drawing.Size(130, 32)
        Me.btnScanLocal.TabIndex = 0
        Me.btnScanLocal.Text = "&Scan this PC"
        Me.tipMain.SetToolTip(Me.btnScanLocal, "Collect the inventory of this computer")
        '
        'btnScanRemote
        '
        Me.btnScanRemote.AutoSize = True
        Me.btnScanRemote.Location = New System.Drawing.Point(142, 4)
        Me.btnScanRemote.Margin = New System.Windows.Forms.Padding(4)
        Me.btnScanRemote.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnScanRemote.Name = "btnScanRemote"
        Me.btnScanRemote.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        Me.btnScanRemote.Size = New System.Drawing.Size(160, 32)
        Me.btnScanRemote.TabIndex = 1
        Me.btnScanRemote.Text = "Scan &remote PC" & ChrW(8230)
        Me.tipMain.SetToolTip(Me.btnScanRemote, "Collect the inventory of another computer on your network")
        '
        'btnOpenReport
        '
        Me.btnOpenReport.AutoSize = True
        Me.btnOpenReport.Location = New System.Drawing.Point(310, 4)
        Me.btnOpenReport.Margin = New System.Windows.Forms.Padding(16, 4, 4, 4)
        Me.btnOpenReport.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnOpenReport.Name = "btnOpenReport"
        Me.btnOpenReport.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        Me.btnOpenReport.Size = New System.Drawing.Size(130, 32)
        Me.btnOpenReport.TabIndex = 2
        Me.btnOpenReport.Text = "&Open report" & ChrW(8230)
        Me.tipMain.SetToolTip(Me.btnOpenReport, "Open a previously saved report (Ctrl+O)")
        '
        'btnCopySummary
        '
        Me.btnCopySummary.AutoSize = True
        Me.btnCopySummary.Location = New System.Drawing.Point(448, 4)
        Me.btnCopySummary.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCopySummary.MinimumSize = New System.Drawing.Size(0, 32)
        Me.btnCopySummary.Name = "btnCopySummary"
        Me.btnCopySummary.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        Me.btnCopySummary.Size = New System.Drawing.Size(140, 32)
        Me.btnCopySummary.TabIndex = 3
        Me.btnCopySummary.Text = "&Copy summary"
        Me.tipMain.SetToolTip(Me.btnCopySummary, "Copy a text summary of the report to the clipboard")
        '
        'flpView
        '
        Me.flpView.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpView.AutoSize = True
        Me.flpView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpView.Controls.Add(Me.lblView)
        Me.flpView.Controls.Add(Me.rbShort)
        Me.flpView.Controls.Add(Me.rbSummary)
        Me.flpView.Controls.Add(Me.rbComplete)
        Me.flpView.Location = New System.Drawing.Point(700, 8)
        Me.flpView.Margin = New System.Windows.Forms.Padding(0)
        Me.flpView.Name = "flpView"
        Me.flpView.Size = New System.Drawing.Size(288, 40)
        Me.flpView.TabIndex = 1
        Me.flpView.WrapContents = False
        '
        'lblView
        '
        Me.lblView.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblView.AutoSize = True
        Me.lblView.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblView.Location = New System.Drawing.Point(0, 12)
        Me.lblView.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.lblView.Name = "lblView"
        Me.lblView.Size = New System.Drawing.Size(32, 15)
        Me.lblView.TabIndex = 0
        Me.lblView.Text = "View"
        '
        'rbShort
        '
        Me.rbShort.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbShort.AutoSize = True
        Me.rbShort.Location = New System.Drawing.Point(40, 4)
        Me.rbShort.Margin = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.rbShort.MinimumSize = New System.Drawing.Size(80, 32)
        Me.rbShort.Name = "rbShort"
        Me.rbShort.Size = New System.Drawing.Size(80, 32)
        Me.rbShort.TabIndex = 1
        Me.rbShort.Text = "Short"
        Me.rbShort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.tipMain.SetToolTip(Me.rbShort, "Show the most relevant properties")
        '
        'rbSummary
        '
        Me.rbSummary.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbSummary.AutoSize = True
        Me.rbSummary.Location = New System.Drawing.Point(120, 4)
        Me.rbSummary.Margin = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.rbSummary.MinimumSize = New System.Drawing.Size(80, 32)
        Me.rbSummary.Name = "rbSummary"
        Me.rbSummary.Size = New System.Drawing.Size(80, 32)
        Me.rbSummary.TabIndex = 2
        Me.rbSummary.Text = "Summary"
        Me.rbSummary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.tipMain.SetToolTip(Me.rbSummary, "Show a wider set of properties")
        '
        'rbComplete
        '
        Me.rbComplete.Appearance = System.Windows.Forms.Appearance.Button
        Me.rbComplete.AutoSize = True
        Me.rbComplete.Location = New System.Drawing.Point(200, 4)
        Me.rbComplete.Margin = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.rbComplete.MinimumSize = New System.Drawing.Size(80, 32)
        Me.rbComplete.Name = "rbComplete"
        Me.rbComplete.Size = New System.Drawing.Size(80, 32)
        Me.rbComplete.TabIndex = 3
        Me.rbComplete.Text = "Complete"
        Me.rbComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.tipMain.SetToolTip(Me.rbComplete, "Show every collected property")
        '
        'pnlCommandsLine
        '
        Me.pnlCommandsLine.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlCommandsLine.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlCommandsLine.Location = New System.Drawing.Point(0, 129)
        Me.pnlCommandsLine.Name = "pnlCommandsLine"
        Me.pnlCommandsLine.Size = New System.Drawing.Size(1000, 1)
        Me.pnlCommandsLine.TabIndex = 3
        '
        'stsMain
        '
        Me.stsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.stsMain.Location = New System.Drawing.Point(0, 598)
        Me.stsMain.Name = "stsMain"
        Me.stsMain.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.stsMain.Size = New System.Drawing.Size(1000, 22)
        Me.stsMain.SizingGrip = False
        Me.stsMain.TabIndex = 5
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(976, 17)
        Me.lblStatus.Spring = True
        Me.lblStatus.Text = "Ready"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlContent
        '
        Me.pnlContent.BackColor = System.Drawing.SystemColors.Window
        Me.pnlContent.Controls.Add(Me.scanProgress)
        Me.pnlContent.Controls.Add(Me.splitMain)
        Me.pnlContent.Controls.Add(Me.pnlEmpty)
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Location = New System.Drawing.Point(0, 130)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New System.Drawing.Size(1000, 468)
        Me.pnlContent.TabIndex = 4
        '
        'scanProgress
        '
        Me.scanProgress.Dock = System.Windows.Forms.DockStyle.Fill
        Me.scanProgress.Location = New System.Drawing.Point(0, 0)
        Me.scanProgress.Name = "scanProgress"
        Me.scanProgress.Size = New System.Drawing.Size(1000, 468)
        Me.scanProgress.TabIndex = 2
        Me.scanProgress.Visible = False
        '
        'splitMain
        '
        Me.splitMain.BackColor = System.Drawing.SystemColors.ControlLight
        Me.splitMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitMain.Location = New System.Drawing.Point(0, 0)
        Me.splitMain.Name = "splitMain"
        '
        'splitMain.Panel1
        '
        Me.splitMain.Panel1.BackColor = System.Drawing.SystemColors.Window
        Me.splitMain.Panel1.Controls.Add(Me.tlpClasses)
        Me.splitMain.Panel1MinSize = 180
        '
        'splitMain.Panel2
        '
        Me.splitMain.Panel2.BackColor = System.Drawing.SystemColors.Window
        Me.splitMain.Panel2.Controls.Add(Me.tlpProperties)
        Me.splitMain.Panel2MinSize = 300
        Me.splitMain.Size = New System.Drawing.Size(1000, 468)
        Me.splitMain.SplitterDistance = 280
        Me.splitMain.SplitterWidth = 1
        Me.splitMain.TabIndex = 0
        Me.splitMain.Visible = False
        '
        'tlpClasses
        '
        Me.tlpClasses.ColumnCount = 1
        Me.tlpClasses.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpClasses.Controls.Add(Me.txtClassFilter, 0, 0)
        Me.tlpClasses.Controls.Add(Me.tvwClasses, 0, 1)
        Me.tlpClasses.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpClasses.Location = New System.Drawing.Point(0, 0)
        Me.tlpClasses.Name = "tlpClasses"
        Me.tlpClasses.Padding = New System.Windows.Forms.Padding(12, 12, 8, 0)
        Me.tlpClasses.RowCount = 2
        Me.tlpClasses.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpClasses.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpClasses.Size = New System.Drawing.Size(280, 468)
        Me.tlpClasses.TabIndex = 0
        '
        'txtClassFilter
        '
        Me.txtClassFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtClassFilter.Location = New System.Drawing.Point(12, 12)
        Me.txtClassFilter.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.txtClassFilter.Name = "txtClassFilter"
        Me.txtClassFilter.PlaceholderText = "Filter classes"
        Me.txtClassFilter.Size = New System.Drawing.Size(260, 23)
        Me.txtClassFilter.TabIndex = 0
        '
        'tvwClasses
        '
        Me.tvwClasses.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tvwClasses.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvwClasses.FullRowSelect = True
        Me.tvwClasses.HideSelection = False
        Me.tvwClasses.Location = New System.Drawing.Point(12, 43)
        Me.tvwClasses.Margin = New System.Windows.Forms.Padding(0)
        Me.tvwClasses.Name = "tvwClasses"
        Me.tvwClasses.ShowLines = False
        Me.tvwClasses.Size = New System.Drawing.Size(260, 425)
        Me.tvwClasses.TabIndex = 1
        '
        'tlpProperties
        '
        Me.tlpProperties.ColumnCount = 1
        Me.tlpProperties.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpProperties.Controls.Add(Me.txtPropertyFilter, 0, 0)
        Me.tlpProperties.Controls.Add(Me.lvwProperties, 0, 1)
        Me.tlpProperties.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpProperties.Location = New System.Drawing.Point(0, 0)
        Me.tlpProperties.Name = "tlpProperties"
        Me.tlpProperties.Padding = New System.Windows.Forms.Padding(8, 12, 12, 0)
        Me.tlpProperties.RowCount = 2
        Me.tlpProperties.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpProperties.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpProperties.Size = New System.Drawing.Size(719, 468)
        Me.tlpProperties.TabIndex = 0
        '
        'txtPropertyFilter
        '
        Me.txtPropertyFilter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtPropertyFilter.Location = New System.Drawing.Point(8, 12)
        Me.txtPropertyFilter.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.txtPropertyFilter.Name = "txtPropertyFilter"
        Me.txtPropertyFilter.PlaceholderText = "Filter properties and values (Ctrl+F)"
        Me.txtPropertyFilter.Size = New System.Drawing.Size(699, 23)
        Me.txtPropertyFilter.TabIndex = 0
        '
        'lvwProperties
        '
        Me.lvwProperties.AllowColumnReorder = True
        Me.lvwProperties.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lvwProperties.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colProperty, Me.colValue})
        Me.lvwProperties.ContextMenuStrip = Me.cmsProperties
        Me.lvwProperties.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvwProperties.FullRowSelect = True
        Me.lvwProperties.Location = New System.Drawing.Point(8, 43)
        Me.lvwProperties.Margin = New System.Windows.Forms.Padding(0)
        Me.lvwProperties.MultiSelect = False
        Me.lvwProperties.Name = "lvwProperties"
        Me.lvwProperties.Size = New System.Drawing.Size(699, 425)
        Me.lvwProperties.TabIndex = 1
        Me.lvwProperties.UseCompatibleStateImageBehavior = False
        Me.lvwProperties.View = System.Windows.Forms.View.Details
        '
        'colProperty
        '
        Me.colProperty.Text = "Property"
        Me.colProperty.Width = 220
        '
        'colValue
        '
        Me.colValue.Text = "Value"
        Me.colValue.Width = 460
        '
        'cmsProperties
        '
        Me.cmsProperties.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuCopyValue, Me.mnuCopyRow})
        Me.cmsProperties.Name = "cmsProperties"
        Me.cmsProperties.Size = New System.Drawing.Size(150, 48)
        '
        'mnuCopyValue
        '
        Me.mnuCopyValue.Name = "mnuCopyValue"
        Me.mnuCopyValue.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.mnuCopyValue.Size = New System.Drawing.Size(149, 22)
        Me.mnuCopyValue.Text = "Copy &value"
        '
        'mnuCopyRow
        '
        Me.mnuCopyRow.Name = "mnuCopyRow"
        Me.mnuCopyRow.Size = New System.Drawing.Size(149, 22)
        Me.mnuCopyRow.Text = "Copy &row"
        '
        'pnlEmpty
        '
        Me.pnlEmpty.BackColor = System.Drawing.SystemColors.Window
        Me.pnlEmpty.ColumnCount = 1
        Me.pnlEmpty.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlEmpty.Controls.Add(Me.flpEmpty, 0, 0)
        Me.pnlEmpty.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEmpty.Location = New System.Drawing.Point(0, 0)
        Me.pnlEmpty.Name = "pnlEmpty"
        Me.pnlEmpty.RowCount = 1
        Me.pnlEmpty.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlEmpty.Size = New System.Drawing.Size(1000, 468)
        Me.pnlEmpty.TabIndex = 1
        '
        'flpEmpty
        '
        Me.flpEmpty.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.flpEmpty.AutoSize = True
        Me.flpEmpty.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpEmpty.Controls.Add(Me.lblEmptyTitle)
        Me.flpEmpty.Controls.Add(Me.lblEmptyText)
        Me.flpEmpty.Controls.Add(Me.flpEmptyActions)
        Me.flpEmpty.Controls.Add(Me.lblRecent)
        Me.flpEmpty.Controls.Add(Me.flpRecent)
        Me.flpEmpty.Controls.Add(Me.lnkOpenReport)
        Me.flpEmpty.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpEmpty.Location = New System.Drawing.Point(290, 110)
        Me.flpEmpty.Name = "flpEmpty"
        Me.flpEmpty.Size = New System.Drawing.Size(420, 248)
        Me.flpEmpty.TabIndex = 0
        Me.flpEmpty.WrapContents = False
        '
        'lblEmptyTitle
        '
        Me.lblEmptyTitle.AutoSize = True
        Me.lblEmptyTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!)
        Me.lblEmptyTitle.Location = New System.Drawing.Point(3, 0)
        Me.lblEmptyTitle.Name = "lblEmptyTitle"
        Me.lblEmptyTitle.Size = New System.Drawing.Size(137, 21)
        Me.lblEmptyTitle.TabIndex = 0
        Me.lblEmptyTitle.Text = "No report loaded"
        '
        'lblEmptyText
        '
        Me.lblEmptyText.AutoSize = True
        Me.lblEmptyText.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblEmptyText.Location = New System.Drawing.Point(3, 29)
        Me.lblEmptyText.Margin = New System.Windows.Forms.Padding(3, 8, 3, 0)
        Me.lblEmptyText.MaximumSize = New System.Drawing.Size(420, 0)
        Me.lblEmptyText.Name = "lblEmptyText"
        Me.lblEmptyText.Size = New System.Drawing.Size(410, 30)
        Me.lblEmptyText.TabIndex = 1
        Me.lblEmptyText.Text = "Scan this computer or another PC on your network, or open a previously saved report."
        '
        'flpEmptyActions
        '
        Me.flpEmptyActions.AutoSize = True
        Me.flpEmptyActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpEmptyActions.Controls.Add(Me.btnEmptyScanLocal)
        Me.flpEmptyActions.Controls.Add(Me.btnEmptyScanRemote)
        Me.flpEmptyActions.Location = New System.Drawing.Point(0, 75)
        Me.flpEmptyActions.Margin = New System.Windows.Forms.Padding(0, 16, 0, 0)
        Me.flpEmptyActions.Name = "flpEmptyActions"
        Me.flpEmptyActions.Size = New System.Drawing.Size(320, 40)
        Me.flpEmptyActions.TabIndex = 2
        Me.flpEmptyActions.WrapContents = False
        '
        'btnEmptyScanLocal
        '
        Me.btnEmptyScanLocal.AutoSize = True
        Me.btnEmptyScanLocal.Location = New System.Drawing.Point(3, 4)
        Me.btnEmptyScanLocal.Margin = New System.Windows.Forms.Padding(3, 4, 4, 4)
        Me.btnEmptyScanLocal.MinimumSize = New System.Drawing.Size(0, 36)
        Me.btnEmptyScanLocal.Name = "btnEmptyScanLocal"
        Me.btnEmptyScanLocal.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.btnEmptyScanLocal.Size = New System.Drawing.Size(140, 36)
        Me.btnEmptyScanLocal.TabIndex = 0
        Me.btnEmptyScanLocal.Text = "Scan this PC"
        '
        'btnEmptyScanRemote
        '
        Me.btnEmptyScanRemote.AutoSize = True
        Me.btnEmptyScanRemote.Location = New System.Drawing.Point(151, 4)
        Me.btnEmptyScanRemote.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEmptyScanRemote.MinimumSize = New System.Drawing.Size(0, 36)
        Me.btnEmptyScanRemote.Name = "btnEmptyScanRemote"
        Me.btnEmptyScanRemote.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.btnEmptyScanRemote.Size = New System.Drawing.Size(160, 36)
        Me.btnEmptyScanRemote.TabIndex = 1
        Me.btnEmptyScanRemote.Text = "Scan remote PC" & ChrW(8230)
        '
        'lblRecent
        '
        Me.lblRecent.AutoSize = True
        Me.lblRecent.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblRecent.Location = New System.Drawing.Point(3, 139)
        Me.lblRecent.Margin = New System.Windows.Forms.Padding(3, 24, 3, 4)
        Me.lblRecent.Name = "lblRecent"
        Me.lblRecent.Size = New System.Drawing.Size(88, 15)
        Me.lblRecent.TabIndex = 3
        Me.lblRecent.Text = "Recent reports"
        '
        'flpRecent
        '
        Me.flpRecent.AutoSize = True
        Me.flpRecent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpRecent.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpRecent.Location = New System.Drawing.Point(0, 158)
        Me.flpRecent.Margin = New System.Windows.Forms.Padding(0)
        Me.flpRecent.Name = "flpRecent"
        Me.flpRecent.Size = New System.Drawing.Size(0, 0)
        Me.flpRecent.TabIndex = 4
        Me.flpRecent.WrapContents = False
        '
        'lnkOpenReport
        '
        Me.lnkOpenReport.AutoSize = True
        Me.lnkOpenReport.Location = New System.Drawing.Point(3, 166)
        Me.lnkOpenReport.Margin = New System.Windows.Forms.Padding(3, 8, 3, 0)
        Me.lnkOpenReport.Name = "lnkOpenReport"
        Me.lnkOpenReport.Size = New System.Drawing.Size(84, 15)
        Me.lnkOpenReport.TabIndex = 5
        Me.lnkOpenReport.TabStop = True
        Me.lnkOpenReport.Text = "Open report" & ChrW(8230)
        '
        'opfMain
        '
        Me.opfMain.Filter = "SystemInfo reports (*.xml)|*.xml"
        Me.opfMain.Title = "Open SystemInfo report"
        '
        'bwProcessSystemInfo
        '
        Me.bwProcessSystemInfo.WorkerReportsProgress = True
        Me.bwProcessSystemInfo.WorkerSupportsCancellation = True
        '
        'tmrStatus
        '
        Me.tmrStatus.Interval = 4000
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(1000, 620)
        Me.Controls.Add(Me.pnlContent)
        Me.Controls.Add(Me.stsMain)
        Me.Controls.Add(Me.pnlCommandsLine)
        Me.Controls.Add(Me.pnlCommands)
        Me.Controls.Add(Me.pnlHeaderLine)
        Me.Controls.Add(Me.pnlHeader)
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(820, 480)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SystemInfo"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        CType(Me.picComputer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlCommands.ResumeLayout(False)
        Me.pnlCommands.PerformLayout()
        Me.flpActions.ResumeLayout(False)
        Me.flpActions.PerformLayout()
        Me.flpView.ResumeLayout(False)
        Me.flpView.PerformLayout()
        Me.stsMain.ResumeLayout(False)
        Me.stsMain.PerformLayout()
        Me.pnlContent.ResumeLayout(False)
        Me.splitMain.Panel1.ResumeLayout(False)
        Me.splitMain.Panel2.ResumeLayout(False)
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitMain.ResumeLayout(False)
        Me.tlpClasses.ResumeLayout(False)
        Me.tlpClasses.PerformLayout()
        Me.tlpProperties.ResumeLayout(False)
        Me.tlpProperties.PerformLayout()
        Me.cmsProperties.ResumeLayout(False)
        Me.cmsTheme.ResumeLayout(False)
        Me.pnlEmpty.ResumeLayout(False)
        Me.pnlEmpty.PerformLayout()
        Me.flpEmpty.ResumeLayout(False)
        Me.flpEmpty.PerformLayout()
        Me.flpEmptyActions.ResumeLayout(False)
        Me.flpEmptyActions.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlHeader As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents picComputer As System.Windows.Forms.PictureBox
    Friend WithEvents lblComputer As System.Windows.Forms.Label
    Friend WithEvents lblDetails As System.Windows.Forms.Label
    Friend WithEvents lblBadge As System.Windows.Forms.Label
    Friend WithEvents btnTheme As System.Windows.Forms.Button
    Friend WithEvents cmsTheme As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuThemeAuto As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuThemeLight As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuThemeDark As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlHeaderLine As System.Windows.Forms.Panel
    Friend WithEvents pnlCommands As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents flpActions As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnScanLocal As System.Windows.Forms.Button
    Friend WithEvents btnScanRemote As System.Windows.Forms.Button
    Friend WithEvents btnOpenReport As System.Windows.Forms.Button
    Friend WithEvents btnCopySummary As System.Windows.Forms.Button
    Friend WithEvents flpView As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblView As System.Windows.Forms.Label
    Friend WithEvents rbShort As System.Windows.Forms.RadioButton
    Friend WithEvents rbSummary As System.Windows.Forms.RadioButton
    Friend WithEvents rbComplete As System.Windows.Forms.RadioButton
    Friend WithEvents pnlCommandsLine As System.Windows.Forms.Panel
    Friend WithEvents stsMain As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pnlContent As System.Windows.Forms.Panel
    Friend WithEvents splitMain As System.Windows.Forms.SplitContainer
    Friend WithEvents tlpClasses As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents txtClassFilter As System.Windows.Forms.TextBox
    Friend WithEvents tvwClasses As System.Windows.Forms.TreeView
    Friend WithEvents tlpProperties As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents txtPropertyFilter As System.Windows.Forms.TextBox
    Friend WithEvents lvwProperties As System.Windows.Forms.ListView
    Friend WithEvents colProperty As System.Windows.Forms.ColumnHeader
    Friend WithEvents colValue As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmsProperties As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuCopyValue As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuCopyRow As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents pnlEmpty As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents scanProgress As CDE.SystemInfo.ScanProgressPanel
    Friend WithEvents flpEmpty As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblEmptyTitle As System.Windows.Forms.Label
    Friend WithEvents lblEmptyText As System.Windows.Forms.Label
    Friend WithEvents flpEmptyActions As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnEmptyScanLocal As System.Windows.Forms.Button
    Friend WithEvents btnEmptyScanRemote As System.Windows.Forms.Button
    Friend WithEvents lblRecent As System.Windows.Forms.Label
    Friend WithEvents flpRecent As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lnkOpenReport As System.Windows.Forms.LinkLabel
    Friend WithEvents opfMain As System.Windows.Forms.OpenFileDialog
    Friend WithEvents bwProcessSystemInfo As CDE.SystemInfo.ProcessSystemInfo
    Friend WithEvents tmrStatus As System.Windows.Forms.Timer
    Friend WithEvents tipMain As System.Windows.Forms.ToolTip
End Class
