Module modMain

    Public Sub Main()
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.SetDefaultFont(UiTheme.BaseFont)

        Application.Run(New frmMain With {.StartupReport = FindReportArgument()})
    End Sub

    ''' <summary>The first command-line argument that is an existing .xml file: the report to open at startup.</summary>
    Private Function FindReportArgument() As String
        Dim oArguments() As String = Environment.GetCommandLineArgs()
        For i As Integer = 1 To oArguments.Length - 1
            If oArguments(i).EndsWith(".xml", StringComparison.OrdinalIgnoreCase) AndAlso IO.File.Exists(oArguments(i)) Then Return oArguments(i)
        Next
        Return String.Empty
    End Function

End Module
