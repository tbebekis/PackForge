// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Contains all publish and installer settings for a PackForge project.
/// </summary>
public class PublisherProjectSettings
{
    // ● public
    /// <summary>
    /// Returns the display name of this project.
    /// </summary>
    public override string ToString() => string.IsNullOrWhiteSpace(Name) ? "(New Project)" : Name;

    // ● properties
    /// <summary>
    /// Gets or sets the PackForge project name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the application display name.
    /// </summary>
    public string AppName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Debian package name.
    /// </summary>
    public string PackageName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the application version.
    /// </summary>
    public string Version { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the package maintainer.
    /// </summary>
    public string Maintainer { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the application homepage.
    /// </summary>
    public string Homepage { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the short package description.
    /// </summary>
    public string DescriptionShort { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the long package description.
    /// </summary>
    public string DescriptionLong { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the solution folder.
    /// </summary>
    public string SolutionFolder { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the .NET project file path.
    /// </summary>
    public string ProjectFilePath { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the publish root folder.
    /// </summary>
    public string PublishRootFolder { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the installer output folder.
    /// </summary>
    public string InstallerOutputFolder { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the legacy icon file path.
    /// </summary>
    public string IconFilePath { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Linux PNG icon file path.
    /// </summary>
    public string LinuxIconFilePath { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Windows ICO icon file path.
    /// </summary>
    public string WindowsIconFilePath { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Linux dotnet publish settings.
    /// </summary>
    public DotNetPublishSettings LinuxPublish { get; set; } = new() { RuntimeIdentifier = "linux-x64" };
    /// <summary>
    /// Gets or sets the Windows dotnet publish settings.
    /// </summary>
    public DotNetPublishSettings WindowsPublish { get; set; } = new() { RuntimeIdentifier = "win-x64" };
    /// <summary>
    /// Gets or sets the Debian package settings.
    /// </summary>
    public DebPackageSettings Deb { get; set; } = new();
    /// <summary>
    /// Gets or sets the Inno Setup settings.
    /// </summary>
    public InnoSetupSettings Inno { get; set; } = new();
    /// <summary>
    /// Gets or sets the stable Inno Setup application id.
    /// </summary>
    public string AppId { get; set; } = Guid.NewGuid().ToString("B").ToUpperInvariant();
    /// <summary>
    /// Gets or sets the last persisted project name.
    /// </summary>
    [JsonIgnore]
    public string StoredName { get; set; } = string.Empty;
    /// <summary>
    /// Gets the installer output folder, falling back to the publish root folder when empty.
    /// </summary>
    [JsonIgnore]
    public string InstallerOutputFolderOrFallback => string.IsNullOrWhiteSpace(InstallerOutputFolder) ? PublishRootFolder : InstallerOutputFolder;
}

/// <summary>
/// Contains dotnet publish settings for a target runtime.
/// </summary>
public class DotNetPublishSettings
{
    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether this publish target is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets the .NET runtime identifier.
    /// </summary>
    public string RuntimeIdentifier { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the publish configuration.
    /// </summary>
    public string Configuration { get; set; } = "Release";
    /// <summary>
    /// Gets or sets a value indicating whether the publish is self-contained.
    /// </summary>
    public bool SelfContained { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether publish single file is enabled.
    /// </summary>
    public bool PublishSingleFile { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether publish trimmed is enabled.
    /// </summary>
    public bool PublishTrimmed { get; set; }
    /// <summary>
    /// Gets or sets the output folder name under the publish root folder.
    /// </summary>
    public string OutputFolderName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets additional dotnet publish arguments.
    /// </summary>
    public string ExtraArguments { get; set; } = string.Empty;
}

/// <summary>
/// Contains Debian package generation settings.
/// </summary>
public class DebPackageSettings
{
    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether Debian package generation is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets the Debian architecture.
    /// </summary>
    public string Architecture { get; set; } = "amd64";
    /// <summary>
    /// Gets or sets the executable file name.
    /// </summary>
    public string ExecutableName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the command name installed in /usr/bin.
    /// </summary>
    public string CommandName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets desktop file categories.
    /// </summary>
    public string DesktopCategories { get; set; } = "Development;";
    /// <summary>
    /// Gets or sets desktop file keywords.
    /// </summary>
    public string DesktopKeywords { get; set; } = "dotnet;desktop;";
    /// <summary>
    /// Gets or sets Debian package dependencies.
    /// </summary>
    public string Dependencies { get; set; } = "libx11-6, libice6, libsm6, libfontconfig1, ca-certificates, tzdata, libc6, libgcc1 | libgcc-s1, libgssapi-krb5-2, libstdc++6, zlib1g, libssl3, libicu70 | libicu72 | libicu74 | libicu76";
    /// <summary>
    /// Gets or sets the Linux installation directory.
    /// </summary>
    public string InstallDir { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Debian package section.
    /// </summary>
    public string Section { get; set; } = "devel";
    /// <summary>
    /// Gets or sets the Debian package priority.
    /// </summary>
    public string Priority { get; set; } = "optional";
    /// <summary>
    /// Gets or sets the generated Debian build script file name.
    /// </summary>
    public string ScriptFileName { get; set; } = "build-deb.sh";
    /// <summary>
    /// Gets or sets the Debian output file name pattern.
    /// </summary>
    public string OutputFileNamePattern { get; set; } = "{AppName}_{Version}_{Architecture}.deb";
}

/// <summary>
/// Contains Inno Setup script generation settings.
/// </summary>
public class InnoSetupSettings
{
    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether Inno Setup script generation is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets the generated Inno Setup script file name.
    /// </summary>
    public string ScriptFileName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the application publisher.
    /// </summary>
    public string AppPublisher { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the publisher URL.
    /// </summary>
    public string AppPublisherUrl { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the support URL.
    /// </summary>
    public string AppSupportUrl { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the updates URL.
    /// </summary>
    public string AppUpdatesUrl { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Windows executable file name.
    /// </summary>
    public string AppExeName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Windows publish folder name.
    /// </summary>
    public string WindowsPublishFolderName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the setup output base file name.
    /// </summary>
    public string OutputBaseFilename { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default installation directory.
    /// </summary>
    public string DefaultDirName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Windows installation scope.
    /// </summary>
    public string InstallScope { get; set; } = "User only";
    /// <summary>
    /// Gets or sets the default start menu group name.
    /// </summary>
    public string DefaultGroupName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the setup icon file.
    /// </summary>
    public string SetupIconFile { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Inno Setup wizard image file.
    /// </summary>
    public string WizardImageFile { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Inno Setup small wizard image file.
    /// </summary>
    public string WizardSmallImageFile { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the setup compression.
    /// </summary>
    public string Compression { get; set; } = "lzma";
    /// <summary>
    /// Gets or sets a value indicating whether solid compression is enabled.
    /// </summary>
    public bool SolidCompression { get; set; } = true;
    /// <summary>
    /// Gets or sets the allowed architectures.
    /// </summary>
    public string ArchitecturesAllowed { get; set; } = "x64compatible";
    /// <summary>
    /// Gets or sets the architectures installed in 64-bit mode.
    /// </summary>
    public string ArchitecturesInstallIn64BitMode { get; set; } = "x64compatible";
    /// <summary>
    /// Gets or sets the setup privileges requirement.
    /// </summary>
    public string PrivilegesRequired { get; set; } = "lowest";
}
