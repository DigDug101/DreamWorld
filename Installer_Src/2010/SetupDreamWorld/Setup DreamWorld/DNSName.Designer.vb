﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DNSName
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
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.SaveButton = New System.Windows.Forms.Button()
        Me.NextNameButton = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(6, 10)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(182, 20)
        Me.TextBox1.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.TextBox1, "Alpha-Numeric plus - ( no spaces or special chars)")
        '
        'SaveButton
        '
        Me.SaveButton.Location = New System.Drawing.Point(12, 79)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(64, 23)
        Me.SaveButton.TabIndex = 5
        Me.SaveButton.Text = "Save"
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'NextNameButton
        '
        Me.NextNameButton.Location = New System.Drawing.Point(6, 35)
        Me.NextNameButton.Name = "NextNameButton"
        Me.NextNameButton.Size = New System.Drawing.Size(78, 23)
        Me.NextNameButton.TabIndex = 6
        Me.NextNameButton.Text = "Next Name"
        Me.NextNameButton.UseVisualStyleBackColor = True
        '
        'ToolTip1
        '
        Me.ToolTip1.ToolTipTitle = "If marked Public, this sim will be published to an online directory so others peo" &
    "pkle on the hypergrid can find it."
        '
        'PictureBox4
        '
        Me.PictureBox4.Image = Global.Outworldz.My.Resources.Resources.about
        Me.PictureBox4.Location = New System.Drawing.Point(182, 45)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Size = New System.Drawing.Size(28, 32)
        Me.PictureBox4.TabIndex = 1859
        Me.PictureBox4.TabStop = False
        Me.ToolTip1.SetToolTip(Me.PictureBox4, "Click for Help on DNS Names")
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(105, 83)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(105, 17)
        Me.CheckBox1.TabIndex = 1860
        Me.CheckBox1.Text = "Display Publicly?"
        Me.ToolTip1.SetToolTip(Me.CheckBox1, "GDPR rules require you to opt in to making your sim publicly available on Hyerica" &
        ".com Click to agree.")
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'DNSName
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(252, 114)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.PictureBox4)
        Me.Controls.Add(Me.NextNameButton)
        Me.Controls.Add(Me.SaveButton)
        Me.Controls.Add(Me.TextBox1)
        Me.Name = "DNSName"
        Me.Text = "DNS Name"
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents SaveButton As Button
    Friend WithEvents NextNameButton As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents CheckBox1 As CheckBox
End Class
