# SystemInfo

Windows desktop utility (VB.NET / WinForms) that generates a hardware and software inventory of a PC — this one or another one on your network — by querying WMI, and saves it as an XML file named `SystemInfo.{ComputerName}.{date-time}.xml`.

## Requirements

- Windows
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) to build, or the .NET 8 Desktop Runtime to run

## Build and run

```bash
dotnet build SystemInfo.slnx
dotnet run --project SystemInfo/SystemInfo.vbproj
```

## Usage

Everything happens in a single window:

- **Scan this PC** collects the inventory, saves the XML report and shows it right away (category tree + property list, with filters).
- **Scan remote PC…** does the same for another computer on your network (see [Scanning a remote computer](#scanning-a-remote-computer)).
- **Open report…** (or a link in *Recent reports*) loads a previously saved XML.
- **View** switches between the Short, Summary and Complete profiles; **Copy summary** copies a text summary to the clipboard.

The sun icon at the top right chooses the color theme: *Windows default*, *Light* or *Dark*. The choice is saved per Windows user and SystemInfo restarts (reopening the current report) to apply it.

While a scan runs, a progress panel covers the window (overall progress, class being read, elapsed time) and the rest of the commands are locked. **Cancel scan** stops it without saving a report.

## Scanning a remote computer

SystemInfo connects to the other computer through WMI over DCOM, using a user name and password you enter in the *Scan remote computer* dialog. Nothing is installed on the remote computer, and passwords are never stored (only the host and user name of recent computers are remembered, in `%LOCALAPPDATA%\SystemInfo\recent-targets.json`).

### One-time setup on the remote computer

Run [`docs/enable-remote-wmi.ps1`](docs/enable-remote-wmi.ps1) in PowerShell **as administrator** on the computer you want to scan (the dialog's *First time? Remote computer setup…* link shows the same script, with Copy and Save as buttons):

```powershell
.\enable-remote-wmi.ps1            # asks whether to create the 'systeminfo' account, then applies the settings
.\enable-remote-wmi.ps1 -CreateUser
.\enable-remote-wmi.ps1 -Undo      # removes everything, including the account
```

It does three things:

1. Optionally creates a local administrator account, `systeminfo`, hidden from the sign-in screen (recommended — see below).
2. Enables the built-in *Windows Management Instrumentation (WMI)* inbound firewall rules.
3. Sets `LocalAccountTokenFilterPolicy = 1`, so a remote local administrator gets a full token. Without it, UAC makes remote WMI fail with *Access denied* on computers that are not in a domain. This also allows remote administrative access with local accounts over other protocols (for example SMB admin shares), so only do it on networks you trust.

### Notes and troubleshooting

- Both computers must be on the **same subnet**: the built-in WMI firewall rules only accept connections from the local subnet.
- Scan with the **local administrator account** (`systeminfo`). Microsoft accounts that sign in with a PIN often fail network logons even with the correct password. On a computer without a domain, type the user name without a prefix (`systeminfo`).
- Error messages end with `Details: <exception> 0x…`. If *Access denied* persists, enable auditing on the remote computer (`auditpol /set /subcategory:"{0CCE9215-69AE-11D9-BED3-505054503030}" /success:enable /failure:enable`) and read the failed logon (event 4625) in its Security log: `SubStatus 0xC000006A` means a wrong password, `0xC0000064` an unknown user name.
- A remote scan collects the same classes as a local one. The `Monitor` section (model, serial number and manufacture date of the connected monitors) comes from the `WmiMonitorID` WMI class, which works without administrator rights; the EDID in the registry only adds the device description. If a computer does not expose `WmiMonitorID`, SystemInfo falls back to the registry, which may list nothing.

## What is collected

Besides the hardware classes (`Win32_*`: board, BIOS, CPU, RAM modules, disks, network, video, monitors, batteries, printers, …) a report includes:

- **Installed programs** (name, version, publisher, install date), read from the Uninstall registry keys. Updates and system components are left out; programs installed only for a user who is not logged on are not visible.
- **Windows**: edition, feature update (for example 25H2) and exact build, activation status, installed updates, and firmware type with Secure Boot state.
- **Storage and memory**: SSD/HDD, bus (NVMe, SATA, USB) and health of each physical disk, and memory slots used versus maximum RAM.
- **Security**: antivirus products, Microsoft Defender status (real-time protection, signature date) and devices with driver problems. **TPM** and **BitLocker** are only reported when SystemInfo runs as administrator, or, in a remote scan, when the account you enter is an administrator of that computer.

- **Battery**: health (current full-charge capacity against the design capacity) and charge cycles; laptops only.
- **Network and users**: IP addresses, gateway, DNS, DHCP and MAC of the adapters in use; the user logged on; local user accounts, user profiles and the members of the local Administrators group. Windows only reports the Administrators members it can resolve to a name: on a computer joined to Azure AD the Azure AD accounts are not listed.
- **Software and services**: Microsoft 365 / Office (Click-to-Run) version and update channel, programs that start with Windows, services that do not run from the Windows folder (remote-access tools, backup and management agents), display and network driver versions, shared folders and the page file.
- **Security and readiness**: virtualization-based security (memory integrity, Credential Guard) and a Windows 11 readiness check (memory, system drive, processor, UEFI/Secure Boot, TPM 2.0). The processor model is checked against Microsoft's official lists of supported processors, which are embedded in the executable (no internet needed when scanning). Intel is matched by generation and AMD by model; a processor newer than the lists is reported as "supported (newer than the list)", as Microsoft states that newer generations are supported. Clearly old Intel hardware (Pentium II/III/4/M, Celeron M/D, Celeron and Pentium older than the 4000 series, and pre-Scalable Xeon) is reported as not supported; modern Xeon, newer Celeron/Pentium and unusual processors are not classified and are reported as "still to check". To refresh the lists, run `docs/update-supported-processors.ps1` (it downloads Microsoft's pages and rewrites `SystemInfo/Data/windows11-supported-processors.txt`) and rebuild. Disk wear (SSD life used, temperature) needs administrator rights like TPM and BitLocker.

A class that a computer does not have (for example BitLocker on Windows Home) or that Windows denies access to is simply left out of that report. Scanning takes one to two minutes, more on a remote computer with many programs.

## Collection profiles

Report generation uses the **Complete** profile; the **View** selector then decides how much of each report is displayed. The three profiles are defined in `modConfigurations.vb`:

- **Short**: minimal subset of attributes per category (motherboard, battery, BIOS, CPU, RAM, disks, network, OS, video, monitors, etc.).
- **Resume**: intermediate subset of attributes.
- **Complete**: all relevant WMI classes (~50 `Win32_*` classes) with all their attributes.

## Configuration

`SystemInfo/appsettings.json` (copied next to the executable) lets you configure:

```json
{
  "OutputFolder": "",
  "Theme": "Auto"
}
```

- `OutputFolder`: folder where generated reports are saved and where the report list and *Open report…* look for them by default. If empty, the executable's folder is used.
- `Theme`: `Auto` (default) follows the Windows app mode (Settings › Personalization › Colors), `Light` and `Dark` force one. It is read when the app starts. The choice made with the theme button is saved in `%LOCALAPPDATA%\SystemInfo\settings.json` and takes precedence over this setting. In dark mode the window, dialogs, menus and tooltips are dark; standard Windows dialogs such as *Open report…* and message boxes stay light, because Windows draws them.

## License

MIT. See [LICENSE](LICENSE).
