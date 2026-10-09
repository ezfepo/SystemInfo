<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRemoteTarget
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
        Me.lblComputer = New System.Windows.Forms.Label()
        Me.cboComputer = New System.Windows.Forms.ComboBox()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.txtUser = New System.Windows.Forms.TextBox()
        Me.lblUserHint = New System.Windows.Forms.Label()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.tlpPassword = New System.Windows.Forms.TableLayoutPanel()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.chkShowPassword = New System.Windows.Forms.CheckBox()
        Me.lblResult = New System.Windows.Forms.Label()
        Me.lnkSetup = New System.Windows.Forms.LinkLabel()
        Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnScan = New System.Windows.Forms.Button()
        Me.btnTest = New System.Windows.Forms.Button()
        Me.tlpMain.SuspendLayout()
        Me.tlpPassword.SuspendLayout()
        Me.flpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.AutoSize = True
        Me.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400.0!))
        Me.tlpMain.Controls.Add(Me.lblComputer, 0, 0)
        Me.tlpMain.Controls.Add(Me.cboComputer, 0, 1)
        Me.tlpMain.Controls.Add(Me.lblUser, 0, 2)
        Me.tlpMain.Controls.Add(Me.txtUser, 0, 3)
        Me.tlpMain.Controls.Add(Me.lblUserHint, 0, 4)
        Me.tlpMain.Controls.Add(Me.lblPassword, 0, 5)
        Me.tlpMain.Controls.Add(Me.tlpPassword, 0, 6)
        Me.tlpMain.Controls.Add(Me.lblResult, 0, 7)
        Me.tlpMain.Controls.Add(Me.lnkSetup, 0, 8)
        Me.tlpMain.Controls.Add(Me.flpButtons, 0, 9)
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.Padding = New System.Windows.Forms.Padding(20, 16, 20, 16)
        Me.tlpMain.RowCount = 10
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.Size = New System.Drawing.Size(440, 330)
        Me.tlpMain.TabIndex = 0
        '
        'lblComputer
        '
        Me.lblComputer.AutoSize = True
        Me.lblComputer.Location = New System.Drawing.Point(20, 16)
        Me.lblComputer.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.lblComputer.Name = "lblComputer"
        Me.lblComputer.Size = New System.Drawing.Size(80, 15)
        Me.lblComputer.TabIndex = 0
        Me.lblComputer.Text = "&Computer"
        '
        'cboComputer
        '
        Me.cboComputer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboComputer.Location = New System.Drawing.Point(20, 35)
        Me.cboComputer.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.cboComputer.Name = "cboComputer"
        Me.cboComputer.Size = New System.Drawing.Size(400, 23)
        Me.cboComputer.TabIndex = 1
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Location = New System.Drawing.Point(20, 70)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(65, 15)
        Me.lblUser.TabIndex = 2
        Me.lblUser.Text = "&User name"
        '
        'txtUser
        '
        Me.txtUser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtUser.Location = New System.Drawing.Point(20, 89)
        Me.txtUser.Margin = New System.Windows.Forms.Padding(0)
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(400, 23)
        Me.txtUser.TabIndex = 3
        '
        'lblUserHint
        '
        Me.lblUserHint.AutoSize = True
        Me.lblUserHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblUserHint.Location = New System.Drawing.Point(20, 116)
        Me.lblUserHint.Margin = New System.Windows.Forms.Padding(0, 4, 0, 12)
        Me.lblUserHint.MaximumSize = New System.Drawing.Size(400, 0)
        Me.lblUserHint.Name = "lblUserHint"
        Me.lblUserHint.Size = New System.Drawing.Size(390, 30)
        Me.lblUserHint.TabIndex = 4
        Me.lblUserHint.Text = "An administrator account of the remote computer, such as the 'systeminfo' account created by the remote computer setup. Microsoft accounts that sign in with a PIN often fail."
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Location = New System.Drawing.Point(20, 158)
        Me.lblPassword.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(57, 15)
        Me.lblPassword.TabIndex = 5
        Me.lblPassword.Text = "&Password"
        '
        'tlpPassword
        '
        Me.tlpPassword.AutoSize = True
        Me.tlpPassword.ColumnCount = 2
        Me.tlpPassword.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpPassword.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpPassword.Controls.Add(Me.txtPassword, 0, 0)
        Me.tlpPassword.Controls.Add(Me.chkShowPassword, 1, 0)
        Me.tlpPassword.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpPassword.Location = New System.Drawing.Point(20, 177)
        Me.tlpPassword.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
        Me.tlpPassword.Name = "tlpPassword"
        Me.tlpPassword.RowCount = 1
        Me.tlpPassword.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpPassword.Size = New System.Drawing.Size(400, 25)
        Me.tlpPassword.TabIndex = 6
        '
        'txtPassword
        '
        Me.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtPassword.Location = New System.Drawing.Point(0, 0)
        Me.txtPassword.Margin = New System.Windows.Forms.Padding(0)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(340, 23)
        Me.txtPassword.TabIndex = 0
        Me.txtPassword.UseSystemPasswordChar = True
        '
        'chkShowPassword
        '
        Me.chkShowPassword.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.chkShowPassword.AutoSize = True
        Me.chkShowPassword.Location = New System.Drawing.Point(348, 3)
        Me.chkShowPassword.Margin = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.chkShowPassword.Name = "chkShowPassword"
        Me.chkShowPassword.Size = New System.Drawing.Size(55, 19)
        Me.chkShowPassword.TabIndex = 1
        Me.chkShowPassword.Text = "S&how"
        '
        'lblResult
        '
        Me.lblResult.AutoSize = True
        Me.lblResult.Location = New System.Drawing.Point(20, 214)
        Me.lblResult.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.lblResult.MaximumSize = New System.Drawing.Size(400, 0)
        Me.lblResult.Name = "lblResult"
        Me.lblResult.Size = New System.Drawing.Size(0, 15)
        Me.lblResult.TabIndex = 7
        '
        'lnkSetup
        '
        Me.lnkSetup.AutoSize = True
        Me.lnkSetup.Location = New System.Drawing.Point(20, 237)
        Me.lnkSetup.Margin = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.lnkSetup.Name = "lnkSetup"
        Me.lnkSetup.Size = New System.Drawing.Size(210, 15)
        Me.lnkSetup.TabIndex = 8
        Me.lnkSetup.TabStop = True
        Me.lnkSetup.Text = "First time? Remote computer setup" & ChrW(8230)
        '
        'flpButtons
        '
        Me.flpButtons.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpButtons.AutoSize = True
        Me.flpButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpButtons.Controls.Add(Me.btnCancel)
        Me.flpButtons.Controls.Add(Me.btnScan)
        Me.flpButtons.Controls.Add(Me.btnTest)
        Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.flpButtons.Location = New System.Drawing.Point(110, 268)
        Me.flpButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.flpButtons.Name = "flpButtons"
        Me.flpButtons.Size = New System.Drawing.Size(310, 40)
        Me.flpButtons.TabIndex = 9
        Me.flpButtons.WrapContents = False
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(226, 4)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(8, 4, 0, 4)
        Me.btnCancel.MinimumSize = New System.Drawing.Size(84, 32)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(84, 32)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        '
        'btnScan
        '
        Me.btnScan.AutoSize = True
        Me.btnScan.Location = New System.Drawing.Point(134, 4)
        Me.btnScan.Margin = New System.Windows.Forms.Padding(8, 4, 0, 4)
        Me.btnScan.MinimumSize = New System.Drawing.Size(84, 32)
        Me.btnScan.Name = "btnScan"
        Me.btnScan.Size = New System.Drawing.Size(84, 32)
        Me.btnScan.TabIndex = 1
        Me.btnScan.Text = "&Scan"
        '
        'btnTest
        '
        Me.btnTest.AutoSize = True
        Me.btnTest.Location = New System.Drawing.Point(0, 4)
        Me.btnTest.Margin = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.btnTest.MinimumSize = New System.Drawing.Size(126, 32)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(126, 32)
        Me.btnTest.TabIndex = 0
        Me.btnTest.Text = "&Test connection"
        '
        'frmRemoteTarget
        '
        Me.AcceptButton = Me.btnScan
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(440, 330)
        Me.Controls.Add(Me.tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmRemoteTarget"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Scan remote computer"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.tlpPassword.ResumeLayout(False)
        Me.tlpPassword.PerformLayout()
        Me.flpButtons.ResumeLayout(False)
        Me.flpButtons.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblComputer As System.Windows.Forms.Label
    Friend WithEvents cboComputer As System.Windows.Forms.ComboBox
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents txtUser As System.Windows.Forms.TextBox
    Friend WithEvents lblUserHint As System.Windows.Forms.Label
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents tlpPassword As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
    Friend WithEvents lblResult As System.Windows.Forms.Label
    Friend WithEvents lnkSetup As System.Windows.Forms.LinkLabel
    Friend WithEvents flpButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnScan As System.Windows.Forms.Button
    Friend WithEvents btnTest As System.Windows.Forms.Button
End Class
