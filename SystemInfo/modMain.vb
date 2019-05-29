Module modMain
    Public Admin As Boolean = False

    Private Const CMD_ADMIN As String = "admin"

    Public Sub CheckCommandLine()
        For Each arg As String In Environment.GetCommandLineArgs()
            If arg.StartsWith("-") Then
                Select Case arg.Substring(1).ToLower()
                    Case CMD_ADMIN
                        Admin = True
                End Select
            End If
        Next
    End Sub

    Public Sub Main()
        Application.EnableVisualStyles()
        CheckCommandLine()

        If Admin Then
            Dim pfrmMain As New frmMain
            Application.Run(pfrmMain)
        Else
            Dim pfrmSystemInfo As New frmSystemInfo
            Application.Run(pfrmSystemInfo)
        End If
    End Sub

End Module
