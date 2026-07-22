# PackForge

PackForge is a small desktop tool for managing publish and installer projects for .NET desktop applications.

It stores each packaging project as an editable JSON file, runs `dotnet publish`, generates Debian package build scripts, generates Inno Setup scripts, and keeps command output visible in the application log.

## Features

- Manage multiple packaging projects from a left sidebar.
- Store project metadata in JSON files under the application data folder.
- Edit general app metadata, publish settings, Debian settings, and Inno Setup settings.
- Run Linux and Windows `dotnet publish` profiles.
- Create a `.tar.gz` artifact for Linux publish output.
- Create a `.zip` artifact for Windows publish output.
- Generate executable Debian `build-deb.sh` scripts.
- Build `.deb` packages on Linux through `dpkg-deb`.
- Generate Inno Setup `.iss` scripts for Windows installers.
- Optionally compile `.iss` scripts on Windows when `ISCC.exe` is available.
- Configure application defaults and the Inno Setup compiler path.
- Show process output in the application log.

## Project Files

PackForge stores projects in the user application data folder.

Example Linux layout:

```text
~/.config/PackForge/
  AppSettings.json
  Projects/
    MyApp.json
    OtherTool.json
```

Each project contains:

- application name, package name, version, maintainer, homepage, and descriptions
- solution folder, `.csproj` path, publish root folder, and installer output folder
- Linux and Windows publish profiles
- Debian package metadata
- Inno Setup installer metadata
- stable Inno Setup `AppId`

## Generated Output

For a project named `MyApp`, the publish root folder can contain files like:

```text
Publish/
  MyApp-Linux-x64/
  MyApp-Windows-x64/
  build-deb.sh
  MyApp.iss
  MyApp_2026.7.22_amd64.deb
  MyApp-2026.7.22-linux.tar.gz
  MyApp-2026.7.22-windows.zip
  DebBuild/
```

Generated scripts are intentionally visible and editable. PackForge is a generator and runner, not a hidden packaging system.

## Requirements

- .NET 10 SDK
- Tripous.Avalon source checkout next to this repository
- Linux: `dpkg-deb` for building Debian packages
- Windows: Inno Setup compiler `ISCC.exe` for compiling `.iss` scripts

The current project references Tripous libraries by relative path:

```text
../Tripous.Avalon/
```

Expected local folder layout:

```text
CSharp/
  PackForge/
  Tripous.Avalon/
```

## Build

From the repository root:

```bash
dotnet build PackForge/PackForge.csproj
```

Run:

```bash
dotnet run --project PackForge/PackForge.csproj
```

## Inno Setup

PackForge can generate `.iss` scripts on any supported OS.

Compiling the installer is Windows-only. Set the compiler path in `Application Settings`, or leave it empty and PackForge will search common installation folders such as:

```text
C:\Program Files\Inno Setup 7
C:\Program Files\Inno Setup 6
C:\Program Files (x86)\Inno Setup 6
```

When working on Linux, the expected workflow is:

- generate Windows publish output
- generate the `.iss` script
- copy the publish output, icons, and `.iss` file to a Windows machine or VM
- compile the setup executable with Inno Setup

## License

PackForge is licensed under the MIT License. See [LICENSE](LICENSE).
