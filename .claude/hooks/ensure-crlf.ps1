# PostToolUse hook (Edit|Write): keep repo text files CRLF, as CLAUDE.md requires.
$ErrorActionPreference = 'Stop'
try {
    $data = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $path = $data.tool_input.file_path
    if (-not $path -or -not (Test-Path -LiteralPath $path -PathType Leaf)) { exit 0 }
    if ($path -match '[\\/](bin|obj|\.git|\.vs)[\\/]') { exit 0 }
    if ($path -notmatch '\.(vb|vbproj|slnx|props|md|json|txt|ps1|yml|yaml|editorconfig|gitattributes|gitignore)$') { exit 0 }
    $bytes = [IO.File]::ReadAllBytes($path)
    $text = [Text.Encoding]::UTF8.GetString($bytes)
    $fixed = [regex]::Replace($text, "(?<!`r)`n", "`r`n")
    if ($fixed -ne $text) {
        $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        [IO.File]::WriteAllText($path, $fixed, (New-Object Text.UTF8Encoding($hasBom)))
    }
} catch { }
exit 0
