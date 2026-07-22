// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Contains PackForge application settings persisted as JSON.
/// </summary>
public class AppSettings: SettingsBase
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettings"/> class.
    /// </summary>
    public AppSettings()
    {
    }

    // ● public
    /// <summary>
    /// Returns the configured or auto-detected Inno Setup compiler path.
    /// </summary>
    public string ResolveInnoSetupCompilerPath()
    {
        string Result = InnoSetupCompilerPath ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(Result))
            Result = InnoSetupLocator.NormalizeCompilerPath(Result);
        if (string.IsNullOrWhiteSpace(Result))
            Result = InnoSetupLocator.FindCompilerPath();
        return Result ?? string.Empty;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the explicit Inno Setup compiler executable path.
    /// </summary>
    public string InnoSetupCompilerPath { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default project maintainer.
    /// </summary>
    public string DefaultMaintainer { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default project homepage.
    /// </summary>
    public string DefaultHomepage { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default publish configuration.
    /// </summary>
    public string DefaultConfiguration { get; set; } = "Release";
    /// <summary>
    /// Gets or sets the default Linux runtime identifier.
    /// </summary>
    public string DefaultLinuxRuntimeIdentifier { get; set; } = "linux-x64";
    /// <summary>
    /// Gets or sets the default Windows runtime identifier.
    /// </summary>
    public string DefaultWindowsRuntimeIdentifier { get; set; } = "win-x64";
    /// <summary>
    /// Gets or sets a value indicating whether new publish profiles are self-contained by default.
    /// </summary>
    public bool DefaultSelfContained { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether new publish profiles use single-file publish by default.
    /// </summary>
    public bool DefaultPublishSingleFile { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether new publish profiles use trimming by default.
    /// </summary>
    public bool DefaultPublishTrimmed { get; set; }
    /// <summary>
    /// Gets or sets the default Debian architecture.
    /// </summary>
    public string DefaultDebArchitecture { get; set; } = "amd64";
    /// <summary>
    /// Gets or sets the default Debian section.
    /// </summary>
    public string DefaultDebSection { get; set; } = "devel";
    /// <summary>
    /// Gets or sets the default Debian priority.
    /// </summary>
    public string DefaultDebPriority { get; set; } = "optional";
    /// <summary>
    /// Gets or sets the default Debian desktop categories.
    /// </summary>
    public string DefaultDesktopCategories { get; set; } = "Development;";
    /// <summary>
    /// Gets or sets the default Debian desktop keywords.
    /// </summary>
    public string DefaultDesktopKeywords { get; set; } = "dotnet;desktop;";
    /// <summary>
    /// Gets or sets the default Inno Setup compression.
    /// </summary>
    public string DefaultInnoCompression { get; set; } = "lzma2/ultra64";
    /// <summary>
    /// Gets or sets a value indicating whether new Inno Setup scripts use solid compression by default.
    /// </summary>
    public bool DefaultInnoSolidCompression { get; set; } = true;
    /// <summary>
    /// Gets or sets the default Inno Setup privileges requirement.
    /// </summary>
    public string DefaultInnoPrivilegesRequired { get; set; } = "lowest";
    /// <summary>
    /// Gets or sets the default Windows install scope.
    /// </summary>
    public string DefaultInnoInstallScope { get; set; } = "User only";
}
