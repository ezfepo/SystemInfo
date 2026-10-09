Imports System.Text.Json

''' <summary>
''' Preferences the user changes from the UI, kept per Windows user in
''' %LOCALAPPDATA%\SystemInfo\settings.json. They take precedence over appsettings.json.
''' </summary>
Public Module UserSettings
    Private Class SettingsFile
        Public Property Theme As String = String.Empty
    End Class

    Private _Loaded As Boolean
    Private _Theme As String = String.Empty

    Private ReadOnly Property FilePath() As String
        Get
            Return IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SystemInfo", "settings.json")
        End Get
    End Property

    ''' <summary>"auto", "light" or "dark" when the user picked one in the UI; empty when they never did.</summary>
    Public Property Theme() As String
        Get
            EnsureLoaded()
            Return _Theme
        End Get
        Set(ByVal value As String)
            EnsureLoaded()
            _Theme = Normalize(value)
            Save()
        End Set
    End Property

    Private Function Normalize(ByVal pValue As String) As String
        Dim oValue As String = If(pValue, String.Empty).Trim().ToLowerInvariant()
        Return If(oValue = "auto" OrElse oValue = "light" OrElse oValue = "dark", oValue, String.Empty)
    End Function

    Private Sub EnsureLoaded()
        If _Loaded Then Exit Sub
        _Loaded = True

        Try
            If IO.File.Exists(FilePath) Then
                Dim oFile As SettingsFile = JsonSerializer.Deserialize(Of SettingsFile)(IO.File.ReadAllText(FilePath))
                If Not oFile Is Nothing Then _Theme = Normalize(oFile.Theme)
            End If
        Catch ex As Exception
            Console.WriteLine("UserSettings.Load: " + ex.Message)
        End Try
    End Sub

    Private Sub Save()
        Try
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(FilePath))
            IO.File.WriteAllText(FilePath, JsonSerializer.Serialize(New SettingsFile With {.Theme = _Theme}))
        Catch ex As Exception
            Console.WriteLine("UserSettings.Save: " + ex.Message)
        End Try
    End Sub
End Module
