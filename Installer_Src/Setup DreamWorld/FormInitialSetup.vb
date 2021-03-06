﻿Public Class FormInitialSetup

    Private _firstName As String
    Private _lastName As String
    Private _email As String
    Private _password As String


#Region "Properties"
    Public Property FirstName As String
        Get
            Return _firstName
        End Get
        Set(value As String)
            _firstName = value
        End Set
    End Property

    Public Property LastName As String
        Get
            Return _lastName
        End Get
        Set(value As String)
            _lastName = value
        End Set
    End Property

    Public Property Email As String
        Get
            Return _email
        End Get
        Set(value As String)
            _email = value
        End Set
    End Property

    Public Property Password As String
        Get
            Return _password
        End Get
        Set(value As String)
            _password = value
        End Set
    End Property


#End Region

#Region "Load"
    Private Sub Form_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Password1TextBox.UseSystemPasswordChar = True
        Password2TextBox.UseSystemPasswordChar = True

    End Sub


#End Region

#Region "Subs"
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

        Password1TextBox.UseSystemPasswordChar = False
        Password2TextBox.UseSystemPasswordChar = False
        Application.DoEvents()

    End Sub

    Private Sub Password2TextBox_TextChanged(sender As Object, e As EventArgs) Handles Password2TextBox.TextChanged

        ValidateForm()

    End Sub
    Private Sub ValidateForm()

        If Password1TextBox.Text <> Password2TextBox.Text And Password1TextBox.Text.Length > 0 Then
            Password2TextBox.BackColor = Color.Red
            SaveButton.Enabled = False
            Return
        End If

        If Password1TextBox.Text = Password2TextBox.Text And
            Password1TextBox.Text.Length > 0 And
            FirstNameTextBox.Text.Length > 0 And
            LastNameTextBox.Text.Length > 0 Then

            Password1TextBox.BackColor = Color.FromName("Window")
            Password2TextBox.BackColor = Color.FromName("Window")
            SaveButton.Enabled = True

        Else
            SaveButton.Enabled = False
        End If

    End Sub

    Private Sub EmailTextBox_TextChanged(sender As Object, e As EventArgs) Handles EmailTextBox.TextChanged

        ValidateForm()
        Email = EmailTextBox.Text

    End Sub

    Private Sub Password1TextBox_TextChanged(sender As Object, e As EventArgs) Handles Password1TextBox.TextChanged

        ValidateForm()
        Password = Password1TextBox.Text

    End Sub

    Private Sub LastNameTextBox_TextChanged(sender As Object, e As EventArgs) Handles LastNameTextBox.TextChanged

        ValidateForm()
        LastName = LastNameTextBox.Text

    End Sub

    Private Sub FirstNameTextBox_TextChanged(sender As Object, e As EventArgs) Handles FirstNameTextBox.TextChanged

        ValidateForm()
        FirstName = FirstNameTextBox.Text

    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        DialogResult = DialogResult.OK
    End Sub

#End Region
End Class