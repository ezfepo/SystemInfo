# PreToolUse hook (Edit|Write): block hand edits of generated files.
$data = [Console]::In.ReadToEnd() | ConvertFrom-Json
$path = [string]$data.tool_input.file_path
if ($path -match 'windows11-supported-processors\.txt$') {
    [Console]::Error.WriteLine('windows11-supported-processors.txt is generated. Refresh it with docs\update-supported-processors.ps1, never by hand.')
    exit 2
}
exit 0
