﻿Imports System.IO
Public Class ClrCache

    Public Sub WipeScripts()

        If Not Form1.pOpensimIsRunning() Then
            Dim folders() = Directory.GetFiles(Form1.pOpensimBinPath & "bin\ScriptEngines\", "*", SearchOption.AllDirectories)
            Form1.Print("Clearing Script cache. This may take a long time!")
            Dim ctr As Integer = 0
            For Each script As String In folders
                Dim ext = Path.GetExtension(script)
                If ext.ToLower(Form1.Usa) <> ".state" And ext.ToLower(Form1.Usa) <> ".keep" Then
                    Try
                        My.Computer.FileSystem.DeleteFile(script)
                    Catch
                    End Try

                    ctr = ctr + 1
                    Form1.Print("Updated " & ctr.ToString(Form1.Usa) & " scripts")
                    Application.DoEvents()
                End If
            Next
        End If

    End Sub

    Public Sub WipeBakes()

        Form1.Print("Clearing bake cache")
        Try
            My.Computer.FileSystem.DeleteDirectory(Form1.pOpensimBinPath & "bin\bakes\", FileIO.DeleteDirectoryOption.DeleteAllContents)
        Catch
        End Try


    End Sub

    Public Sub WipeAssets()

        Form1.Print("Clearing Asset cache. This may take a long time!")
        Try
            Dim folders() = Directory.GetDirectories(Form1.pOpensimBinPath & "bin\Assetcache\", "*", SearchOption.AllDirectories)
            Dim ctr As Integer = 0
            For Each folder As String In folders
                My.Computer.FileSystem.DeleteDirectory(folder, FileIO.DeleteDirectoryOption.DeleteAllContents)
                ctr = ctr + 1
                If ctr Mod 100 = 0 Then Form1.Print("Deleted " & ctr.ToString(Form1.Usa))
                Application.DoEvents()
            Next
        Catch
        End Try

    End Sub

    Public Sub WipeImage()

        Try
            Form1.Print("Clearing Image cache.")
            Dim folders() = IO.Directory.GetDirectories(Form1.pOpensimBinPath & "bin\j2kDecodeCache\")
            Dim ctr = 0
            For Each folder As String In folders
                My.Computer.FileSystem.DeleteDirectory(folder, FileIO.DeleteDirectoryOption.DeleteAllContents)
                ctr = ctr + 1
                If ctr Mod 100 = 0 Then Form1.Print("Deleted " & ctr.ToString(Form1.Usa))
                Application.DoEvents()
            Next
        Catch
        End Try

    End Sub

    Public Sub WipeMesh()

        Try
            Form1.Print("Clearing Mesh cache")
            Dim fCount As Integer = Directory.GetFiles(Form1.pOpensimBinPath & "bin\MeshCache\", "*", SearchOption.AllDirectories).Length

            Dim folders() = Directory.GetFiles(Form1.pOpensimBinPath & "bin\MeshCache\", "*", SearchOption.AllDirectories)
            Dim ctr As Integer = 0
            For Each folder As String In folders
                My.Computer.FileSystem.DeleteDirectory(Form1.pOpensimBinPath & "bin\MeshCache\", FileIO.DeleteDirectoryOption.DeleteAllContents)
                ctr = ctr + 1
                Form1.Print(ctr.ToString(Form1.Usa) + " of " + fCount.ToString(Form1.Usa))
                Application.DoEvents()
            Next
        Catch
        End Try

    End Sub

End Class
