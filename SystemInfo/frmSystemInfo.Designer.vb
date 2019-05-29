<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSystemInfo
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
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.prgProgress = New System.Windows.Forms.ProgressBar()
        Me.btnGetSystemInfo = New System.Windows.Forms.Button()
        Me.btnDetalle = New System.Windows.Forms.Button()
        Me.bwProcessSystemInfo = New CDE.SystemInfo.ProcessSystemInfo()
        Me.SuspendLayout
        '
        'lblProgress
        '
        Me.lblProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lblProgress.BackColor = System.Drawing.SystemColors.Window
        Me.lblProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblProgress.Font = New System.Drawing.Font("Arial Narrow", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblProgress.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblProgress.Location = New System.Drawing.Point(17, 52)
        Me.lblProgress.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(273, 22)
        Me.lblProgress.TabIndex = 0
        '
        'prgProgress
        '
        Me.prgProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.prgProgress.Location = New System.Drawing.Point(16, 78)
        Me.prgProgress.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.prgProgress.MarqueeAnimationSpeed = 0
        Me.prgProgress.Name = "prgProgress"
        Me.prgProgress.Size = New System.Drawing.Size(273, 28)
        Me.prgProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.prgProgress.TabIndex = 1
        '
        'btnGetSystemInfo
        '
        Me.btnGetSystemInfo.Location = New System.Drawing.Point(16, 15)
        Me.btnGetSystemInfo.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnGetSystemInfo.Name = "btnGetSystemInfo"
        Me.btnGetSystemInfo.Size = New System.Drawing.Size(275, 30)
        Me.btnGetSystemInfo.TabIndex = 2
        Me.btnGetSystemInfo.Text = "Get System Information"
        Me.btnGetSystemInfo.UseVisualStyleBackColor = true
        '
        'btnDetalle
        '
        Me.btnDetalle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnDetalle.Location = New System.Drawing.Point(16, 118)
        Me.btnDetalle.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnDetalle.Name = "btnDetalle"
        Me.btnDetalle.Size = New System.Drawing.Size(275, 30)
        Me.btnDetalle.TabIndex = 3
        Me.btnDetalle.Text = "View Detail..."
        Me.btnDetalle.UseVisualStyleBackColor = true
        '
        'bwProcessSystemInfo
        '
        Me.bwProcessSystemInfo.WorkerReportsProgress = true
        Me.bwProcessSystemInfo.WorkerSupportsCancellation = true
        '
        'frmSystemInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8!, 16!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(305, 164)
        Me.Controls.Add(Me.btnDetalle)
        Me.Controls.Add(Me.btnGetSystemInfo)
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.prgProgress)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "frmSystemInfo"
        Me.Text = "System Information"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents lblProgress As System.Windows.Forms.Label
    Friend WithEvents prgProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents btnGetSystemInfo As System.Windows.Forms.Button
    Friend WithEvents bwProcessSystemInfo As CDE.SystemInfo.ProcessSystemInfo
    Friend WithEvents btnDetalle As System.Windows.Forms.Button
End Class
