<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRemoteSetup
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
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.lblIntro = New System.Windows.Forms.Label()
        Me.txtScript = New System.Windows.Forms.TextBox()
        Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCopy = New System.Windows.Forms.Button()
        Me.sfdScript = New System.Windows.Forms.SaveFileDialog()
        Me.tlpMain.SuspendLayout()
        Me.flpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Controls.Add(Me.lblIntro, 0, 0)
        Me.tlpMain.Controls.Add(Me.txtScript, 0, 1)
        Me.tlpMain.Controls.Add(Me.flpButtons, 0, 2)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.Padding = New System.Windows.Forms.Padding(20, 16, 20, 16)
        Me.tlpMain.RowCount = 3
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.Size = New System.Drawing.Size(720, 540)
        Me.tlpMain.TabIndex = 0
        '
        'lblIntro
        '
        Me.lblIntro.AutoSize = True
        Me.lblIntro.Location = New System.Drawing.Point(20, 16)
        Me.lblIntro.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.lblIntro.MaximumSize = New System.Drawing.Size(680, 0)
        Me.lblIntro.Name = "lblIntro"
        Me.lblIntro.Size = New System.Drawing.Size(670, 45)
        Me.lblIntro.TabIndex = 0
        Me.lblIntro.Text = "Run this script once, in PowerShell as administrator, on the computer you want to scan. It creates a local administrator account to scan with (systeminfo, recommended), enables the WMI firewall rules and lets that account connect remotely. Run it with -Undo to remove everything. Both computers must be on the same subnet."
        '
        'txtScript
        '
        Me.txtScript.BackColor = System.Drawing.SystemColors.Control
        Me.txtScript.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtScript.Font = New System.Drawing.Font("Consolas", 9.0!)
        Me.txtScript.Location = New System.Drawing.Point(20, 73)
        Me.txtScript.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.txtScript.Multiline = True
        Me.txtScript.Name = "txtScript"
        Me.txtScript.ReadOnly = True
        Me.txtScript.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtScript.Size = New System.Drawing.Size(680, 399)
        Me.txtScript.TabIndex = 1
        Me.txtScript.WordWrap = False
        '
        'flpButtons
        '
        Me.flpButtons.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpButtons.AutoSize = True
        Me.flpButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpButtons.Controls.Add(Me.btnClose)
        Me.flpButtons.Controls.Add(Me.btnSave)
        Me.flpButtons.Controls.Add(Me.btnCopy)
        Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.flpButtons.Location = New System.Drawing.Point(390, 484)
        Me.flpButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.flpButtons.Name = "flpButtons"
        Me.flpButtons.Size = New System.Drawing.Size(310, 40)
        Me.flpButtons.TabIndex = 2
        Me.flpButtons.WrapContents = False
        '
        'btnClose
        '
        Me.btnClose.AutoSize = True
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(226, 4)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(8, 4, 0, 4)
        Me.btnClose.MinimumSize = New System.Drawing.Size(84, 32)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(84, 32)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "Close"
        '
        'btnSave
        '
        Me.btnSave.AutoSize = True
        Me.btnSave.Location = New System.Drawing.Point(124, 4)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(8, 4, 0, 4)
        Me.btnSave.MinimumSize = New System.Drawing.Size(94, 32)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(94, 32)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "&Save as" & ChrW(8230)
        '
        'btnCopy
        '
        Me.btnCopy.AutoSize = True
        Me.btnCopy.Location = New System.Drawing.Point(0, 4)
        Me.btnCopy.Margin = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.btnCopy.MinimumSize = New System.Drawing.Size(116, 32)
        Me.btnCopy.Name = "btnCopy"
        Me.btnCopy.Size = New System.Drawing.Size(116, 32)
        Me.btnCopy.TabIndex = 0
        Me.btnCopy.Text = "&Copy script"
        '
        'sfdScript
        '
        Me.sfdScript.DefaultExt = "ps1"
        Me.sfdScript.FileName = "enable-remote-wmi.ps1"
        Me.sfdScript.Filter = "PowerShell script (*.ps1)|*.ps1"
        Me.sfdScript.Title = "Save remote computer setup script"
        '
        'frmRemoteSetup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(720, 540)
        Me.Controls.Add(Me.tlpMain)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(560, 400)
        Me.Name = "frmRemoteSetup"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Remote computer setup"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.flpButtons.ResumeLayout(False)
        Me.flpButtons.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblIntro As System.Windows.Forms.Label
    Friend WithEvents txtScript As System.Windows.Forms.TextBox
    Friend WithEvents flpButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCopy As System.Windows.Forms.Button
    Friend WithEvents sfdScript As System.Windows.Forms.SaveFileDialog
End Class
