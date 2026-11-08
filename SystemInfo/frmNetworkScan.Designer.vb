<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNetworkScan
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
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lvwHosts = New System.Windows.Forms.ListView()
        Me.colName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAddress = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSelect = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.tlpMain.SuspendLayout()
        Me.flpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.AutoSize = True
        Me.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 440.0!))
        Me.tlpMain.Controls.Add(Me.lblStatus, 0, 0)
        Me.tlpMain.Controls.Add(Me.lvwHosts, 0, 1)
        Me.tlpMain.Controls.Add(Me.flpButtons, 0, 2)
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.Padding = New System.Windows.Forms.Padding(20, 16, 20, 16)
        Me.tlpMain.RowCount = 3
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.Size = New System.Drawing.Size(480, 380)
        Me.tlpMain.TabIndex = 0
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(20, 16)
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
        Me.lblStatus.MaximumSize = New System.Drawing.Size(440, 0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 15)
        Me.lblStatus.TabIndex = 0
        '
        'lvwHosts
        '
        Me.lvwHosts.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colName, Me.colAddress})
        Me.lvwHosts.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvwHosts.FullRowSelect = True
        Me.lvwHosts.HideSelection = False
        Me.lvwHosts.Location = New System.Drawing.Point(20, 39)
        Me.lvwHosts.Margin = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.lvwHosts.MultiSelect = False
        Me.lvwHosts.Name = "lvwHosts"
        Me.lvwHosts.Size = New System.Drawing.Size(440, 260)
        Me.lvwHosts.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwHosts.TabIndex = 1
        Me.lvwHosts.UseCompatibleStateImageBehavior = False
        Me.lvwHosts.View = System.Windows.Forms.View.Details
        '
        'colName
        '
        Me.colName.Text = "Name"
        Me.colName.Width = 270
        '
        'colAddress
        '
        Me.colAddress.Text = "IP address"
        Me.colAddress.Width = 130
        '
        'flpButtons
        '
        Me.flpButtons.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpButtons.AutoSize = True
        Me.flpButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.flpButtons.Controls.Add(Me.btnCancel)
        Me.flpButtons.Controls.Add(Me.btnSelect)
        Me.flpButtons.Controls.Add(Me.btnSearch)
        Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.flpButtons.Location = New System.Drawing.Point(150, 315)
        Me.flpButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.flpButtons.Name = "flpButtons"
        Me.flpButtons.Size = New System.Drawing.Size(310, 40)
        Me.flpButtons.TabIndex = 2
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
        'btnSelect
        '
        Me.btnSelect.AutoSize = True
        Me.btnSelect.Enabled = False
        Me.btnSelect.Location = New System.Drawing.Point(134, 4)
        Me.btnSelect.Margin = New System.Windows.Forms.Padding(8, 4, 0, 4)
        Me.btnSelect.MinimumSize = New System.Drawing.Size(84, 32)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(84, 32)
        Me.btnSelect.TabIndex = 1
        Me.btnSelect.Text = "&Select"
        '
        'btnSearch
        '
        Me.btnSearch.AutoSize = True
        Me.btnSearch.Location = New System.Drawing.Point(0, 4)
        Me.btnSearch.Margin = New System.Windows.Forms.Padding(0, 4, 0, 4)
        Me.btnSearch.MinimumSize = New System.Drawing.Size(126, 32)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(126, 32)
        Me.btnSearch.TabIndex = 0
        Me.btnSearch.Text = "Search &again"
        '
        'frmNetworkScan
        '
        Me.AcceptButton = Me.btnSelect
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(480, 380)
        Me.Controls.Add(Me.tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmNetworkScan"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Find computers on the network"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.flpButtons.ResumeLayout(False)
        Me.flpButtons.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tlpMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lvwHosts As System.Windows.Forms.ListView
    Friend WithEvents colName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colAddress As System.Windows.Forms.ColumnHeader
    Friend WithEvents flpButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSelect As System.Windows.Forms.Button
    Friend WithEvents btnSearch As System.Windows.Forms.Button
End Class
