Imports System.Security
Imports System.Threading.Tasks

''' <summary>
''' Asks for the remote computer and the credentials to scan it, and can test the connection first.
''' </summary>
Public Class frmRemoteTarget
    Private Const DefaultUserName As String = "systeminfo" 'account created by docs\enable-remote-wmi.ps1

    Private _RecentTargets As List(Of RecentTargets.Entry)
    Private _Testing As Boolean

    ''' <summary>The computer to scan; valid after the dialog returns OK.</summary>
    Public ReadOnly Property Target As RemoteTarget

    Private Sub frmRemoteTarget_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        UiTheme.Apply(Me)
        UiTheme.StylePrimaryButton(btnScan)
        UiTheme.StyleSecondaryButton(btnTest)
        UiTheme.StyleSecondaryButton(btnCancel)
        UiTheme.StyleLink(lnkSetup)

        _RecentTargets = RecentTargets.Load()
        For Each oEntry As RecentTargets.Entry In _RecentTargets
            cboComputer.Items.Add(oEntry)
        Next
        If cboComputer.Items.Count > 0 Then cboComputer.SelectedIndex = 0
        If txtUser.Text.Length = 0 Then txtUser.Text = DefaultUserName
    End Sub

    Private Sub frmRemoteTarget_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Shown
        'Jump to the first empty field
        If cboComputer.Text.Length = 0 Then
            cboComputer.Focus()
        ElseIf txtUser.Text.Length = 0 Then
            txtUser.Focus()
        Else
            txtPassword.Focus()
        End If
    End Sub

    Private Sub cboComputer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboComputer.SelectedIndexChanged
        Dim oEntry As RecentTargets.Entry = TryCast(cboComputer.SelectedItem, RecentTargets.Entry)
        If Not oEntry Is Nothing Then txtUser.Text = oEntry.UserName
        ShowResult(String.Empty, UiTheme.TextColor)
    End Sub

    Private Sub chkShowPassword_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkShowPassword.CheckedChanged
        txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
    End Sub

    Private Function BuildTarget() As RemoteTarget
        Dim oPassword As New SecureString
        For Each oChar As Char In txtPassword.Text
            oPassword.AppendChar(oChar)
        Next
        oPassword.MakeReadOnly()

        Return New RemoteTarget With {
            .Host = cboComputer.Text.Trim(),
            .UserName = txtUser.Text.Trim(),
            .Password = oPassword
        }
    End Function

    Private Function ValidateInput() As Boolean
        If cboComputer.Text.Trim().Length = 0 Then
            ShowResult("Enter the name or IP address of the computer.", UiTheme.ErrorColor)
            cboComputer.Focus()
            Return False
        End If
        Return True
    End Function

    Private Async Sub btnTest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnTest.Click
        If _Testing OrElse Not ValidateInput() Then Exit Sub

        Dim oTarget As RemoteTarget = BuildTarget()
        SetTesting(True)
        ShowResult("Connecting to " + oTarget.DisplayName + ChrW(8230), UiTheme.MutedTextColor)
        Try
            Dim oDescription As String = Await Task.Run(Function() oTarget.Probe())
            ShowResult(ChrW(&H2714) + " Connected: " + oDescription, UiTheme.SuccessColor)
        Catch ex As Exception
            ShowResult(ChrW(&H2716) + " " + ConnectionErrors.Describe(ex), UiTheme.ErrorColor)
        Finally
            SetTesting(False)
        End Try
    End Sub

    Private Sub SetTesting(ByVal pTesting As Boolean)
        _Testing = pTesting
        cboComputer.Enabled = Not pTesting
        txtUser.Enabled = Not pTesting
        txtPassword.Enabled = Not pTesting
        btnTest.Enabled = Not pTesting
        btnScan.Enabled = Not pTesting
        Me.UseWaitCursor = pTesting
    End Sub

    Private Sub ShowResult(ByVal pText As String, ByVal pColor As Color)
        lblResult.Text = pText
        lblResult.ForeColor = pColor
    End Sub

    Private Sub btnScan_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnScan.Click
        If _Testing OrElse Not ValidateInput() Then Exit Sub

        _Target = BuildTarget()
        If Not _Target.IsLocal Then RecentTargets.Remember(_Target.Host, _Target.UserName)
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub frmRemoteTarget_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        'Don't close while a test runs: its continuation would touch disposed controls
        If _Testing Then
            e.Cancel = True
        Else
            txtPassword.Clear()
        End If
    End Sub

    Private Sub lnkSetup_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles lnkSetup.LinkClicked
        Using oSetup As New frmRemoteSetup
            oSetup.ShowDialog(Me)
        End Using
    End Sub
End Class
