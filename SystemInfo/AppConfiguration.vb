Imports Microsoft.Extensions.Configuration

Public Module AppConfiguration

    Private ReadOnly _Configuration As IConfigurationRoot = New ConfigurationBuilder() _
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) _
        .AddJsonFile("appsettings.json", optional:=True, reloadOnChange:=False) _
        .Build()

    ''' <summary>
    ''' Folder where XML reports are generated and searched for.
    ''' If "OutputFolder" is not configured (or is empty) in appsettings.json, the application folder is used.
    ''' </summary>
    Public ReadOnly Property OutputFolder() As String
        Get
            Dim oFolder As String = _Configuration("OutputFolder")
            If String.IsNullOrWhiteSpace(oFolder) Then
                Return AppDomain.CurrentDomain.BaseDirectory
            End If
            Return IO.Path.GetFullPath(oFolder)
        End Get
    End Property

End Module
