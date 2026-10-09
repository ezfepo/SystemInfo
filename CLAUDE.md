# CLAUDE.md

Guide for working in this repository with Claude Code.

## What this project is

Windows desktop application (VB.NET / WinForms) that uses WMI to generate a hardware and software inventory of the PC, and saves it as XML (`SystemInfo.{ComputerName}.{date-time}.xml`). See [README.md](README.md) for usage and run modes.

## Stack

- VB.NET, SDK-style `.vbproj`, `net8.0-windows`
- WinForms (`UseWindowsForms=true`)
- WMI via `System.Management` (NuGet package, not included in the base SDK)

## Commands

```bash
dotnet build SystemInfo.slnx      # build
dotnet run --project SystemInfo/SystemInfo.vbproj   # run
```

`Directory.Build.props` defaults `Configuration` to `Release` when `-c`/`--configuration` isn't passed, so plain `dotnet build`/`clean`/`build -t:Rebuild` (and the VS Code tasks in `.vscode/tasks.json`) all produce Release output. Pass `-c Debug` explicitly to get a Debug build.

No automated tests or CI configured.

## Architecture (main flow)

```
modMain.vb (Sub Main: DPI mode, default font, runs frmMain)
  └─ frmMain.vb (single window: scan, open recent/saved reports, browse the XML)
       ├─ frmRemoteTarget.vb → RemoteTarget (host + credentials); frmRemoteSetup.vb shows docs\enable-remote-wmi.ps1
       ├─ ScanProgressPanel.vb (overlay + AccentProgressBar.vb) fed by ScanProgress snapshots
       └─ ProcessSystemInfo.vb (BackgroundWorker; argument = SystemInfoRequest(configuration, RemoteTarget))
            └─ WMINamespace.vb
                 ├─ WMINamespace: Connect() opens ONE ManagementScope (root\CIMV2, local or \\host) shared by all classes; builds the output XML file name from the real computer name
                 ├─ WMIClass: runs WQL queries (SELECT * FROM Win32_XXX) on that scope; WMIClassSources.vb says which classes live in another namespace (GetScope caches one scope per namespace and remembers failures), need a WHERE filter, or come from a reader (InventoryReaders.vb: installed programs, Windows release, Secure Boot, battery health, disk wear, local administrators, Office, services outside Windows, Windows 11 readiness; built on StdRegistry.vb and its `Query` helper, which returns Nothing when a query can't run so a reader can tell "no data" from "not allowed")
                 ├─ WMIManagementObject / WMIPropertyData: dump properties to XML (values go through RemoveInvalidXmlChars: some devices report control characters that XML 1.0 forbids)
                 ├─ WMIExtMonitor: connected monitors from WmiMonitorID (root\wmi, no admin rights needed), enriched with the EDID/DeviceDesc read from the registry through StdRegProv; registry-only fallback (needs a "Control" subkey that Win11 may hide)
                 └─ WMIViewer: loads a saved XML and displays it in TreeView/ListView (with filters)
```

- `modConfigurations.vb` defines the 3 collection profiles (`Short`, `Resume`, `Complete`) as lists of WMI classes + attributes to include. A scan always collects `Complete`; the View selector only changes what is displayed. To add a data point: list the class in `Complete`, its attributes in `Resume`/`Short`, register it in `WMIClassSources.vb` when it is not a plain root\CIMV2 query, and add decoding of coded values (numbers that mean something) to `WMIViewer.GetValue`, checking `pClassName` when the property name already exists for another class (for example `MediaType`, `Location`). Classes a computer lacks, or that need administrator rights (TPM, BitLocker), just come back empty; the viewer hides empty classes.
- `ProcessorSupport.vb` classifies a processor (Win32_Processor.Name) against Microsoft's Windows 11 supported-processor lists, embedded as `SystemInfo\Data\windows11-supported-processors.txt` (generated, ASCII, CRLF; refresh it with `docs\update-supported-processors.ps1`, never by hand). It returns Supported / SupportedByGeneration / NotSupported / Unknown and is used by the Windows 11 readiness reader; Clearly pre-list Intel hardware (Pentium II/III/4/M/D, Celeron M/D, Celeron/Pentium N/J/G below the 4000 series, old Xeon E/X/L/W and Xeon(TM)) is NotSupported (`OldIntelReason`); Scalable Xeon, W-1200, E-2100+ and newer Celeron/Pentium are deliberately left Unknown.
- `UserSettings.vb` keeps what the user changes in the UI (today the theme) in `%LOCALAPPDATA%\SystemInfo\settings.json`; it wins over `appsettings.json`. Changing the theme restarts the app (`frmMain.RestartApplication`), passing the open report as the only command-line argument; `modMain` opens a first `.xml` argument at startup.
- `RemoteTarget.vb` builds the `ConnectionOptions` (Impersonate, PacketPrivacy as the DCOM hardening requires, EnablePrivileges) and `IsLocal` (WMI rejects explicit credentials on local connections). `EnsureReachable()` probes TCP 135 first so an unreachable host fails in ~3 s instead of DCOM's ~26 s.
- `ConnectionErrors.vb` turns DCOM/WMI exceptions into actionable messages and appends `Details: <exception> 0x…`; `RecentTargets.vb` keeps host + user name (never passwords) in `%LOCALAPPDATA%\SystemInfo\recent-targets.json`.
- `UiTheme.vb` holds fonts, the semantic light/dark palette (`SurfaceColor`, `TextColor`, `MutedTextColor`, `AccentOnSurface`…), the Windows accent color (darkened until white text reaches 4.5:1), DPI helpers (`Dp`), button styles, Segoe Fluent Icons glyphs and rounded corners. Use the palette properties, never `SystemColors`, in new code. `Theme` in `appsettings.json` (`Auto`/`Light`/`Dark`) picks the palette at startup. Every form calls `UiTheme.Apply(Me)` first thing in `Load`; in dark mode that runs `ThemeApplier.vb`, which recolors the controls the designer set with system colors, asks Windows for dark scroll bars/combo/list header, draws the ListView header and disabled buttons by hand (Windows ignores `ForeColor` there) and owner-draws tooltips. Light mode keeps the plain system look. .NET 8 WinForms has no dark mode of its own; MessageBox and the common file dialogs stay light. The app is per-monitor DPI aware (`AutoScaleMode.Dpi`); don't hardcode pixel sizes without `Dp`.
- `ListViewSorter.vb` / `ConsoleRedir.vb` are UI utilities (column sorting, console redirection to a RichTextBox).
- There is no mail-sending functionality (removed along with `MailSender.vb`). When a scan finishes, the report is loaded into the viewer; it is not opened externally.
- `AppConfiguration.vb` reads `appsettings.json` (key `OutputFolder`) via `Microsoft.Extensions.Configuration`; it controls where `WMINamespace.FileName` saves the XML and where `frmMain` looks for reports by default. Empty/absent = executable's folder.
- `docs\enable-remote-wmi.ps1` is embedded in the executable as a resource (`LogicalName="enable-remote-wmi.ps1"` in the `.vbproj`) and must stay ASCII-only (Windows PowerShell 5.1 reads BOM-less files as ANSI). It creates the optional `systeminfo` admin account, enables the `WMI-*-In-TCP` firewall rules (they only accept the local subnet) and sets `LocalAccountTokenFilterPolicy`; `-Undo` reverts all of it.

## Conventions

- Everything committed (code, comments, UI strings, docs, commit messages, PR descriptions) is written in English.
- Every file in the repo uses **CRLF** line endings (`.editorconfig` has `end_of_line = crlf`; `core.autocrlf=true`). Whenever you create or edit a file, make sure it stays CRLF. A file created with a tool that writes LF must be converted before finishing (for example `sed -i 's/$/\r/' file`), and then re-checked with `git ls-files --eol` (look for `w/lf`) or `grep -c $'\r$' file`.
- Windows 11 Microsoft accounts that sign in with a PIN often fail remote logons; scan remote computers with a local administrator account.

## Important notes

- `bin/`, `SystemInfo/obj/`, and `.vs/` are in `.gitignore` and must not be tracked again.
- The project was migrated from a classic `.vbproj` targeting .NET Framework 2.0 to SDK-style / .NET 8. If undefined WinForms/XML type errors appear, check the `<Import>` entries in `SystemInfo.vbproj` (SDK-style VB does not bring in implicit imports for `System.Windows.Forms`/`System.Xml`/etc. the way the classic project did).
