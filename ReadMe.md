# PackForge

PackForge is a small desktop tool for managing publish and installer projects for .NET desktop applications.

It stores each packaging project as an editable JSON file, runs `dotnet publish`, generates Debian package build scripts, generates Inno Setup scripts, and keeps command output visible in the application log.

## Features

- Manage multiple packaging projects from a left sidebar.
- Store project metadata in JSON files under the application data folder.
- Edit general app metadata, publish settings, Debian Build settings, and Inno Setup Build settings.
- Run Linux and Windows `dotnet publish` profiles.
- Create a `.tar.gz` artifact for Linux publish output.
- Create a `.zip` artifact for Windows publish output.
- Generate executable Debian `build-deb.sh` scripts into the Linux publish output folder.
- Build `.deb` packages on Linux through `dpkg-deb`.
- Generate Inno Setup `.iss` scripts into the Windows publish output folder.
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
- solution folder and `.csproj` path
- Linux and Windows publish profiles
- Linux and Windows publish root folders
- Debian and Inno Setup build source folders
- Debian and Inno Setup build output folders
- Debian package metadata
- Inno Setup installer metadata
- stable Inno Setup `AppId`

## Workflow

PackForge separates packaging into three phases.

### Publish

The Publish phase runs `dotnet publish`.

The publish root folder is a path on the machine running PackForge. PackForge appends the version and the target output folder name and writes the published application files there.

For example, on Linux:

```text
/home/teo/Dev/CSharp/MyApp/Publish/2026.7.23/MyApp-Windows-x64/
```

The publish root folder is publish-only. Debian Build and Inno Setup Build do not use it.

### Script Generation

Script generation normally runs on the same machine as Publish.

PackForge writes generated scripts into the matching publish output folder:

- Debian script generation writes `build-deb.sh` into the Linux publish output folder.
- Inno Setup script generation writes `MyApp.iss` into the Windows publish output folder.

Generated scripts are intentionally visible and editable. PackForge is a generator and runner, not a hidden packaging system.

### Build

The Build phase runs the native packaging tool for the target operating system.

The build uses only the build source folder:

- Debian Build uses the Debian build source folder and expects the published Linux files, `build-deb.sh`, and Debian assets to be there.
- Inno Setup Build uses the Inno Setup build source folder and expects the published Windows files, `.iss` script, setup icon, and wizard images to be there.

The build writes installer artifacts only to the build output folder. When the build output folder is empty, PackForge uses an `Output` folder under the matching build source folder.

When building for the other operating system, copy the publish output folder contents, generated script, and required assets to the target machine or VM. For example, when PackForge runs on Linux and the Windows installer must be built in a Windows VM, copy the Windows publish output folder and the generated `.iss` file to the Windows build source folder.

## Output Layout

For a project named `MyApp`, when publishing, script generation, and build use the same folders, output can look like:

```text
Publish/
  2026.7.23/
    MyApp-Linux-x64/
      MyApp
      build-deb.sh
      Output/
        MyApp_2026.7.23_amd64.deb
    MyApp-Windows-x64/
      MyApp.exe
      MyApp.iss
      Output/
        MyApp_Setup_2026.7.23.exe
    MyApp-2026.7.23-linux.tar.gz
    MyApp-2026.7.23-windows.zip
```

The corresponding build source folders may be the same folders when publishing and building on the same operating system, or copied folders on another machine or VM. Build output folders should stay separate from build source folders so repeated builds do not package previous installer artifacts.

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
- copy the publish output, icons, wizard images, and `.iss` file to the Windows build source folder on a Windows machine or VM
- compile the setup executable with Inno Setup
- write the setup executable to the Inno Setup build output folder

## License

PackForge is licensed under the MIT License. See [LICENSE](LICENSE).
