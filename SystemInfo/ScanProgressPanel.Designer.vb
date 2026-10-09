<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ScanProgressPanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.tlpCenter = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpCard = New System.Windows.Forms.TableLayoutPanel()
        Me.picIcon = New System.Windows.Forms.PictureBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.prgScan = New CDE.SystemInfo.AccentProgressBar()
        Me.lblStage = New System.Windows.Forms.Label()
        Me.lblPercent = New System.Windows.Forms.Label()
        Me.lblDetail = New System.Windows.Forms.Label()
        Me.lblElapsed = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tmrElapsed = New System.Windows.Forms.Timer(Me.components)
        Me.tlpCenter.SuspendLayout()
        Me.tlpCard.SuspendLayout()
        CType(Me.picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tlpCenter
        '
        Me.tlpCenter.ColumnCount = 1
        Me.tlpCenter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpCenter.Controls.Add(Me.tlpCard, 0, 0)
        Me.tlpCenter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpCenter.Location = New System.Drawing.Point(0, 0)
        Me.tlpCenter.Name = "tlpCenter"
        Me.tlpCenter.RowCount = 1
        Me.tlpCenter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpCenter.Size = New System.Drawing.Size(800, 460)
        Me.tlpCenter.TabIndex = 0
        '
        'tlpCard
        '
        Me.tlpCard.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tlpCard.AutoSize = True
        Me.tlpCard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpCard.ColumnCount = 2
        Me.tlpCard.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400.0!))
        Me.tlpCard.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60.0!))
        Me.tlpCard.Controls.Add(Me.picIcon, 0, 0)
        Me.tlpCard.Controls.Add(Me.lblTitle, 0, 1)
        Me.tlpCard.Controls.Add(Me.lblSubtitle, 0, 2)
        Me.tlpCard.Controls.Add(Me.prgScan, 0, 3)
        Me.tlpCard.Controls.Add(Me.lblStage, 0, 4)
        Me.tlpCard.Controls.Add(Me.lblPercent, 1, 4)
        Me.tlpCard.Controls.Add(Me.lblDetail, 0, 5)
        Me.tlpCard.Controls.Add(Me.lblElapsed, 0, 6)
        Me.tlpCard.Controls.Add(Me.btnCancel, 0, 7)
        Me.tlpCard.Location = New System.Drawing.Point(170, 70)
        Me.tlpCard.Name = "tlpCard"
        Me.tlpCard.RowCount = 8
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpCard.Size = New System.Drawing.Size(460, 320)
        Me.tlpCard.TabIndex = 0
        '
        'picIcon
        '
        Me.tlpCard.SetColumnSpan(Me.picIcon, 2)
        Me.picIcon.Location = New System.Drawing.Point(0, 0)
        Me.picIcon.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.picIcon.Name = "picIcon"
        Me.picIcon.Size = New System.Drawing.Size(40, 40)
        Me.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.picIcon.TabIndex = 0
        Me.picIcon.TabStop = False
        '
        'lblTitle
        '
        Me.lblTitle.AutoEllipsis = True
        Me.tlpCard.SetColumnSpan(Me.lblTitle, 2)
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 14.0!)
        Me.lblTitle.Location = New System.Drawing.Point(0, 52)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(460, 32)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "Scanning"
        '
        'lblSubtitle
        '
        Me.lblSubtitle.AutoSize = True
        Me.tlpCard.SetColumnSpan(Me.lblSubtitle, 2)
        Me.lblSubtitle.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblSubtitle.Location = New System.Drawing.Point(0, 88)
        Me.lblSubtitle.Margin = New System.Windows.Forms.Padding(0, 4, 0, 20)
        Me.lblSubtitle.MaximumSize = New System.Drawing.Size(460, 0)
        Me.lblSubtitle.Name = "lblSubtitle"
        Me.lblSubtitle.Size = New System.Drawing.Size(440, 30)
        Me.lblSubtitle.TabIndex = 2
        Me.lblSubtitle.Text = "Collecting the hardware and software inventory. This usually takes one to two minutes; the rest of the window is locked until it finishes."
        '
        'prgScan
        '
        Me.tlpCard.SetColumnSpan(Me.prgScan, 2)
        Me.prgScan.Dock = System.Windows.Forms.DockStyle.Fill
        Me.prgScan.Location = New System.Drawing.Point(0, 138)
        Me.prgScan.Margin = New System.Windows.Forms.Padding(0, 0, 0, 10)
        Me.prgScan.Name = "prgScan"
        Me.prgScan.Size = New System.Drawing.Size(460, 8)
        Me.prgScan.TabIndex = 3
        '
        'lblStage
        '
        Me.lblStage.AutoEllipsis = True
        Me.lblStage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblStage.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblStage.Location = New System.Drawing.Point(0, 156)
        Me.lblStage.Margin = New System.Windows.Forms.Padding(0)
        Me.lblStage.Name = "lblStage"
        Me.lblStage.Size = New System.Drawing.Size(400, 20)
        Me.lblStage.TabIndex = 4
        Me.lblStage.Text = "Connecting"
        Me.lblStage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPercent
        '
        Me.lblPercent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPercent.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblPercent.Location = New System.Drawing.Point(400, 156)
        Me.lblPercent.Margin = New System.Windows.Forms.Padding(0)
        Me.lblPercent.Name = "lblPercent"
        Me.lblPercent.Size = New System.Drawing.Size(60, 20)
        Me.lblPercent.TabIndex = 5
        Me.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblDetail
        '
        Me.lblDetail.AutoEllipsis = True
        Me.tlpCard.SetColumnSpan(Me.lblDetail, 2)
        Me.lblDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDetail.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblDetail.Location = New System.Drawing.Point(0, 178)
        Me.lblDetail.Margin = New System.Windows.Forms.Padding(0, 2, 0, 0)
        Me.lblDetail.Name = "lblDetail"
        Me.lblDetail.Size = New System.Drawing.Size(460, 20)
        Me.lblDetail.TabIndex = 6
        Me.lblDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblElapsed
        '
        Me.lblElapsed.AutoSize = True
        Me.tlpCard.SetColumnSpan(Me.lblElapsed, 2)
        Me.lblElapsed.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblElapsed.Location = New System.Drawing.Point(0, 198)
        Me.lblElapsed.Margin = New System.Windows.Forms.Padding(0, 0, 0, 24)
        Me.lblElapsed.Name = "lblElapsed"
        Me.lblElapsed.Size = New System.Drawing.Size(80, 15)
        Me.lblElapsed.TabIndex = 7
        Me.lblElapsed.Text = "Elapsed 0:00"
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.Location = New System.Drawing.Point(0, 237)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(0)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(110, 34)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.btnCancel.Size = New System.Drawing.Size(110, 34)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Cancel scan"
        '
        'tmrElapsed
        '
        Me.tmrElapsed.Interval = 1000
        '
        'ScanProgressPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.Controls.Add(Me.tlpCenter)
        Me.Name = "ScanProgressPanel"
        Me.Size = New System.Drawing.Size(800, 460)
        Me.tlpCenter.ResumeLayout(False)
        Me.tlpCenter.PerformLayout()
        Me.tlpCard.ResumeLayout(False)
        Me.tlpCard.PerformLayout()
        CType(Me.picIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tlpCenter As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tlpCard As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents picIcon As System.Windows.Forms.PictureBox
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblSubtitle As System.Windows.Forms.Label
    Friend WithEvents prgScan As CDE.SystemInfo.AccentProgressBar
    Friend WithEvents lblStage As System.Windows.Forms.Label
    Friend WithEvents lblPercent As System.Windows.Forms.Label
    Friend WithEvents lblDetail As System.Windows.Forms.Label
    Friend WithEvents lblElapsed As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents tmrElapsed As System.Windows.Forms.Timer
End Class
