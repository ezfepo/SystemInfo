---
name: new-datapoint
description: Add a new WMI class or inventory data point to the SystemInfo report, following the repo's registration checklist.
disable-model-invocation: true
argument-hint: <WMI class or data point, e.g. Win32_Fan>
---

Add the data point `$ARGUMENTS` to the inventory. Read CLAUDE.md ("Architecture") first, then:

1. Check the class exists and which namespace it lives in (use the Microsoft Learn MCP tools if available for `Win32_*` docs).
2. List the class in the `Complete` profile in `SystemInfo\modConfigurations.vb`; list the attributes worth showing in `Resume` and `Short`.
3. If it is not a plain root\CIMV2 `SELECT *` query (other namespace, needs a WHERE filter, or comes from a reader), register it in `WMIClassSources.vb`. For a reader, add it to `InventoryReaders.vb` using `StdRegistry.Query` so "no data" and "not allowed" stay distinguishable.
4. Decode coded values in `WMIViewer.GetValue`, checking `pClassName` when the property name already exists for another class.
5. Build with `dotnet build SystemInfo.slnx`. Fix warnings you introduced.
6. Confirm CRLF line endings on every touched file (`git ls-files --eol`, look for `w/lf`).
7. All text, comments and UI strings in English. Do not commit unless asked.
