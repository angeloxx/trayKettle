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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.notifyIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.menu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.StartTo100CToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartTo95CToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartTo80CToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StartTo65CToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menu.SuspendLayout()
        Me.SuspendLayout()
        '
        'notifyIcon
        '
        Me.notifyIcon.Icon = CType(resources.GetObject("notifyIcon.Icon"), System.Drawing.Icon)
        Me.notifyIcon.Text = "trayKettle"
        Me.notifyIcon.Visible = True
        '
        'menu
        '
        Me.menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartTo100CToolStripMenuItem, Me.StartTo95CToolStripMenuItem, Me.StartTo80CToolStripMenuItem, Me.StartTo65CToolStripMenuItem, Me.StopToolStripMenuItem, Me.ToolStripMenuItem1, Me.ExitToolStripMenuItem})
        Me.menu.Name = "menu"
        Me.menu.Size = New System.Drawing.Size(203, 142)
        '
        'StartTo100CToolStripMenuItem
        '
        Me.StartTo100CToolStripMenuItem.Name = "StartTo100CToolStripMenuItem"
        Me.StartTo100CToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.StartTo100CToolStripMenuItem.Text = "Start to 100 C - Black tea"
        '
        'StartTo95CToolStripMenuItem
        '
        Me.StartTo95CToolStripMenuItem.Name = "StartTo95CToolStripMenuItem"
        Me.StartTo95CToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.StartTo95CToolStripMenuItem.Text = "Start to 95 C - Coffee"
        '
        'StartTo80CToolStripMenuItem
        '
        Me.StartTo80CToolStripMenuItem.Name = "StartTo80CToolStripMenuItem"
        Me.StartTo80CToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.StartTo80CToolStripMenuItem.Text = "Start to 80 C - White tea"
        '
        'StartTo65CToolStripMenuItem
        '
        Me.StartTo65CToolStripMenuItem.Name = "StartTo65CToolStripMenuItem"
        Me.StartTo65CToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.StartTo65CToolStripMenuItem.Text = "Start to 65 C - Green tea"
        '
        'StopToolStripMenuItem
        '
        Me.StopToolStripMenuItem.Name = "StopToolStripMenuItem"
        Me.StopToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.StopToolStripMenuItem.Text = "Stop"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(199, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(124, 0)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "Form1"
        Me.menu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents notifyIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents menu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents StartTo100CToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartTo95CToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartTo80CToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartTo65CToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StopToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
