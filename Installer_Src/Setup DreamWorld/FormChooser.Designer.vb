﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Choice
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Choice))
        Me.DataGridView = New System.Windows.Forms.DataGridView()
        Me.Group = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OKButton1 = New System.Windows.Forms.Button()
        Me.CancelButton1 = New System.Windows.Forms.Button()
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView
        '
        Me.DataGridView.AllowUserToAddRows = False
        Me.DataGridView.AllowUserToDeleteRows = False
        Me.DataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridView.ColumnHeadersHeight = 34
        Me.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Group})
        Me.DataGridView.Location = New System.Drawing.Point(1, -1)
        Me.DataGridView.MultiSelect = False
        Me.DataGridView.Name = "DataGridView"
        Me.DataGridView.ReadOnly = True
        Me.DataGridView.RowHeadersWidth = 40
        Me.DataGridView.Size = New System.Drawing.Size(292, 189)
        Me.DataGridView.TabIndex = 2
        '
        'Group
        '
        Me.Group.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Group.HeaderText = "Group"
        Me.Group.MaxInputLength = 50
        Me.Group.MinimumWidth = 8
        Me.Group.Name = "Group"
        Me.Group.ReadOnly = True
        Me.Group.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Group.ToolTipText = Global.Outworldz.My.Resources.Resources.Click_2_Choose
        '
        'OKButton1
        '
        Me.OKButton1.Location = New System.Drawing.Point(35, 193)
        Me.OKButton1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.OKButton1.Name = "OKButton1"
        Me.OKButton1.Size = New System.Drawing.Size(85, 24)
        Me.OKButton1.TabIndex = 3
        Me.OKButton1.Text = Global.Outworldz.My.Resources.Resources.Ok
        Me.OKButton1.UseVisualStyleBackColor = True
        '
        'CancelButton1
        '
        Me.CancelButton1.Location = New System.Drawing.Point(170, 193)
        Me.CancelButton1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.CancelButton1.Name = "CancelButton1"
        Me.CancelButton1.Size = New System.Drawing.Size(83, 24)
        Me.CancelButton1.TabIndex = 4
        Me.CancelButton1.Text = Global.Outworldz.My.Resources.Resources.Cancel_word
        Me.CancelButton1.UseVisualStyleBackColor = True
        '
        'Choice
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(298, 225)
        Me.Controls.Add(Me.CancelButton1)
        Me.Controls.Add(Me.OKButton1)
        Me.Controls.Add(Me.DataGridView)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Choice"
        Me.Text = Global.Outworldz.My.Resources.Resources.Choose_Region_word
        Me.TopMost = True
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DataGridView As DataGridView
    Friend WithEvents Group As DataGridViewTextBoxColumn
    Friend WithEvents OKButton1 As Button
    Friend WithEvents CancelButton1 As Button
End Class
