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
dotnet run --project SystemInfo/SystemInfo.vbproj   # run (user mode)
```

No automated tests or CI configured.

## Architecture (main flow)

```
modMain.vb (Sub Main, parses -admin)
  └─ frmSystemInfo.vb (user mode, with "View Detail..." button) / frmMain.vb (detail view, browses XMLs)
       └─ ProcessSystemInfo.vb (BackgroundWorker orchestrating collection)
            └─ WMINamespace.vb
                 ├─ WMINamespace: represents root\CIMV2, builds the output XML file name
                 ├─ WMIClass: runs WQL queries (SELECT * FROM Win32_XXX)
                 ├─ WMIManagementObject / WMIPropertyData: dump properties to XML
                 ├─ WMIExtMonitor: monitors via registry (EDID), not standard WMI
                 └─ WMIViewer: loads a saved XML and displays it in TreeView/ListView
```

- `modConfigurations.vb` defines the 3 collection profiles (`Short`, `Resume`, `Complete`) as lists of WMI classes + attributes to include.
- `ListViewSorter.vb` / `ConsoleRedir.vb` are UI utilities (column sorting, console redirection to a RichTextBox).
- There is no mail-sending functionality (removed along with `MailSender.vb`); once the XML is generated, it opens with the system's associated application.
- `AppConfiguration.vb` reads `appsettings.json` (key `OutputFolder`) via `Microsoft.Extensions.Configuration`; it controls where `WMINamespace.FileName` saves the XML and where `frmMain` looks for reports by default. Empty/absent = executable's folder.

## Important notes

- `bin/`, `SystemInfo/obj/`, and `.vs/` are in `.gitignore` and must not be tracked again.
- The project was migrated from a classic `.vbproj` targeting .NET Framework 2.0 to SDK-style / .NET 8. If undefined WinForms/XML type errors appear, check the `<Import>` entries in `SystemInfo.vbproj` (SDK-style VB does not bring in implicit imports for `System.Windows.Forms`/`System.Xml`/etc. the way the classic project did).
