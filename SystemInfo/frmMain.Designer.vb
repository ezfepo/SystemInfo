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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.opfMain = New System.Windows.Forms.OpenFileDialog()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.tvwMain = New System.Windows.Forms.TreeView()
        Me.lvwMain = New System.Windows.Forms.ListView()
        Me.colProperty = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.colValue = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.tsMain = New System.Windows.Forms.ToolStrip()
        Me.btnLoad = New System.Windows.Forms.ToolStripButton()
        Me.tsbType = New System.Windows.Forms.ToolStripSplitButton()
        Me.optShort = New System.Windows.Forms.ToolStripMenuItem()
        Me.optResume = New System.Windows.Forms.ToolStripMenuItem()
        Me.optComplete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnExit = New System.Windows.Forms.ToolStripButton()
        Me.btnTest = New System.Windows.Forms.ToolStripButton()
        Me.btnResumen = New System.Windows.Forms.ToolStripButton()
        Me.SplitContainer1.Panel1.SuspendLayout
        Me.SplitContainer1.Panel2.SuspendLayout
        Me.SplitContainer1.SuspendLayout
        Me.tsMain.SuspendLayout
        Me.SuspendLayout
        '
        'opfMain
        '
        Me.opfMain.Filter = "XML Files|*.xml"
        Me.opfMain.Title = "SystemInfo.xml File"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 34)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvwMain)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lvwMain)
        Me.SplitContainer1.Size = New System.Drawing.Size(928, 453)
        Me.SplitContainer1.SplitterDistance = 430
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 3
        '
        'tvwMain
        '
        Me.tvwMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvwMain.FullRowSelect = true
        Me.tvwMain.HideSelection = false
        Me.tvwMain.Location = New System.Drawing.Point(0, 0)
        Me.tvwMain.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.tvwMain.Name = "tvwMain"
        Me.tvwMain.Size = New System.Drawing.Size(430, 453)
        Me.tvwMain.TabIndex = 1
        '
        'lvwMain
        '
        Me.lvwMain.AllowColumnReorder = true
        Me.lvwMain.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colProperty, Me.colValue})
        Me.lvwMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvwMain.FullRowSelect = true
        Me.lvwMain.Location = New System.Drawing.Point(0, 0)
        Me.lvwMain.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.lvwMain.MultiSelect = false
        Me.lvwMain.Name = "lvwMain"
        Me.lvwMain.Size = New System.Drawing.Size(493, 453)
        Me.lvwMain.TabIndex = 0
        Me.lvwMain.UseCompatibleStateImageBehavior = false
        Me.lvwMain.View = System.Windows.Forms.View.Details
        '
        'colProperty
        '
        Me.colProperty.Text = "Property"
        Me.colProperty.Width = 100
        '
        'colValue
        '
        Me.colValue.Text = "Value"
        Me.colValue.Width = 200
        '
        'tsMain
        '
        Me.tsMain.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnLoad, Me.tsbType, Me.ToolStripSeparator1, Me.btnExit, Me.btnTest, Me.btnResumen})
        Me.tsMain.Location = New System.Drawing.Point(0, 0)
        Me.tsMain.Name = "tsMain"
        Me.tsMain.Size = New System.Drawing.Size(928, 27)
        Me.tsMain.TabIndex = 4
        '
        'btnLoad
        '
        Me.btnLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnLoad.Image = CType(resources.GetObject("btnLoad.Image"),System.Drawing.Image)
        Me.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(57, 24)
        Me.btnLoad.Text = "Load"
        '
        'tsbType
        '
        Me.tsbType.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbType.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.optShort, Me.optResume, Me.optComplete})
        Me.tsbType.Image = CType(resources.GetObject("tsbType.Image"),System.Drawing.Image)
        Me.tsbType.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbType.Name = "tsbType"
        Me.tsbType.Size = New System.Drawing.Size(58, 24)
        Me.tsbType.Text = "Type"
        Me.tsbType.ToolTipText = "Type"
        '
        'optShort
        '
        Me.optShort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.optShort.Name = "optShort"
        Me.optShort.Size = New System.Drawing.Size(181, 26)
        Me.optShort.Text = "Short"
        '
        'optResume
        '
        Me.optResume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.optResume.Name = "optResume"
        Me.optResume.Size = New System.Drawing.Size(181, 26)
        Me.optResume.Text = "Summary"
        '
        'optComplete
        '
        Me.optComplete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.optComplete.Name = "optComplete"
        Me.optComplete.Size = New System.Drawing.Size(181, 26)
        Me.optComplete.Text = "Complete"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 27)
        '
        'btnExit
        '
        Me.btnExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnExit.Image = CType(resources.GetObject("btnExit.Image"),System.Drawing.Image)
        Me.btnExit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(42, 24)
        Me.btnExit.Text = "Exit"
        '
        'btnTest
        '
        Me.btnTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnTest.Enabled = false
        Me.btnTest.Image = CType(resources.GetObject("btnTest.Image"),System.Drawing.Image)
        Me.btnTest.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(39, 24)
        Me.btnTest.Text = "Test"
        Me.btnTest.Visible = false
        '
        'btnResumen
        '
        Me.btnResumen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnResumen.Image = CType(resources.GetObject("btnResumen.Image"),System.Drawing.Image)
        Me.btnResumen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnResumen.Name = "btnResumen"
        Me.btnResumen.Size = New System.Drawing.Size(73, 24)
        Me.btnResumen.Text = "Summary"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8!, 16!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(928, 486)
        Me.Controls.Add(Me.tsMain)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "frmMain"
        Me.Text = "SystemInfo Detail"
        Me.SplitContainer1.Panel1.ResumeLayout(false)
        Me.SplitContainer1.Panel2.ResumeLayout(false)
        Me.SplitContainer1.ResumeLayout(false)
        Me.tsMain.ResumeLayout(false)
        Me.tsMain.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents opfMain As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents tvwMain As System.Windows.Forms.TreeView
    Friend WithEvents tsMain As System.Windows.Forms.ToolStrip
    Friend WithEvents btnLoad As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnExit As System.Windows.Forms.ToolStripButton
    Friend WithEvents lvwMain As System.Windows.Forms.ListView
    Friend WithEvents colProperty As System.Windows.Forms.ColumnHeader
    Friend WithEvents colValue As System.Windows.Forms.ColumnHeader
    Friend WithEvents tsbType As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents optShort As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents optResume As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents optComplete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnTest As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnResumen As ToolStripButton
End Class
