''' <summary>
''' Shows docs\enable-remote-wmi.ps1 (embedded in the executable) so it can be copied or saved
''' and run on the computer that will be scanned.
''' </summary>
Public Class frmRemoteSetup
    Private Const ScriptResourceName As String = "enable-remote-wmi.ps1"

    Private Sub frmRemoteSetup_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        UiTheme.Apply(Me)
        UiTheme.StylePrimaryButton(btnCopy)
        UiTheme.SetGlyph(btnCopy, UiTheme.Glyphs.Copy)
        UiTheme.StyleSecondaryButton(btnSave)
        UiTheme.StyleSecondaryButton(btnClose)

        txtScript.Text = ReadScript()
        Me.ActiveControl = btnCopy 'a focused read-only TextBox would show the whole script selected
    End Sub

    Private Shared Function ReadScript() As String
        Using oStream As IO.Stream = GetType(frmRemoteSetup).Assembly.GetManifestResourceStream(ScriptResourceName)
            Using oReader As New IO.StreamReader(oStream)
                'Normalize to CRLF so the TextBox shows line breaks whatever the checkout uses
                Return oReader.ReadToEnd().Replace(vbCrLf, vbLf).Replace(vbLf, vbCrLf)
            End Using
        End Using
    End Function

    Private Sub btnCopy_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCopy.Click
        Clipboard.SetText(txtScript.Text)
        btnCopy.Text = "Copied"
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        If sfdScript.ShowDialog(Me) = DialogResult.OK Then
            'UTF-8 with BOM: Windows PowerShell 5.1 reads BOM-less scripts as ANSI
            IO.File.WriteAllText(sfdScript.FileName, txtScript.Text, System.Text.Encoding.UTF8)
        End If
    End Sub
End Class
