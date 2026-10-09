Imports System.Text.Json

''' <summary>
''' Remote computers scanned before (host and user name only — passwords are never stored),
''' kept per Windows user in %LOCALAPPDATA%\SystemInfo\recent-targets.json.
''' </summary>
Public Class RecentTargets
    Private Const MaxEntries As Integer = 10

    Public Class Entry
        Public Property Host As String = String.Empty
        Public Property UserName As String = String.Empty

        Public Overrides Function ToString() As String
            Return Host
        End Function
    End Class

    Private Shared ReadOnly Property FilePath() As String
        Get
            Return IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SystemInfo", "recent-targets.json")
        End Get
    End Property

    Public Shared Function Load() As List(Of Entry)
        Try
            If IO.File.Exists(FilePath) Then
                Dim oEntries As List(Of Entry) = JsonSerializer.Deserialize(Of List(Of Entry))(IO.File.ReadAllText(FilePath))
                If Not oEntries Is Nothing Then Return oEntries
            End If
        Catch ex As Exception
            Console.WriteLine("RecentTargets.Load: " + ex.Message)
        End Try
        Return New List(Of Entry)
    End Function

    ''' <summary>Moves (or adds) the host to the top of the list.</summary>
    Public Shared Sub Remember(ByVal pHost As String, ByVal pUserName As String)
        Try
            Dim oEntries As List(Of Entry) = Load()
            oEntries.RemoveAll(Function(oEntry) String.Equals(oEntry.Host, pHost, StringComparison.OrdinalIgnoreCase))
            oEntries.Insert(0, New Entry With {.Host = pHost, .UserName = pUserName})
            If oEntries.Count > MaxEntries Then oEntries.RemoveRange(MaxEntries, oEntries.Count - MaxEntries)

            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(FilePath))
            IO.File.WriteAllText(FilePath, JsonSerializer.Serialize(oEntries))
        Catch ex As Exception
            Console.WriteLine("RecentTargets.Remember: " + ex.Message)
        End Try
    End Sub
End Class
