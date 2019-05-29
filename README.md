# SystemInfo

Windows desktop utility (VB.NET / WinForms) that generates a hardware and software inventory of a PC by querying WMI, and saves it as an XML file named `SystemInfo.{ComputerName}.{date-time}.xml`.

## Requirements

- Windows
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Build and run

```bash
dotnet build SystemInfo.slnx
dotnet run --project SystemInfo/SystemInfo.vbproj
```

## Usage modes

- **User mode** (default): a simple window with a "Get System Information" button that creates the XML report, and a "View Detail..." button to open the exploration view.
- **Detail mode**: can also be opened directly by running with the `-admin` argument, skipping the main window. Lets you explore a generated XML (category tree + property list) and open previous reports.

```bash
SystemInfo.exe -admin
```

## Collection profiles

Report generation uses one of three profiles defined in `modConfigurations.vb`:

- **Short**: minimal subset of attributes per category (motherboard, battery, BIOS, CPU, RAM, disks, network, OS, video, monitors, etc.).
- **Resume**: intermediate subset of attributes.
- **Complete**: all relevant WMI classes (~50 `Win32_*` classes) with all their attributes.

## Configuration

`SystemInfo/appsettings.json` (copied next to the executable) lets you configure the folder where generated reports are saved and where Detail mode looks for them by default:

```json
{
  "OutputFolder": ""
}
```

If `OutputFolder` is empty, the executable's folder is used (default behavior).

## License

MIT. See [LICENSE](LICENSE).
