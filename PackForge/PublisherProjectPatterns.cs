// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Resolves project setting pattern tokens.
/// </summary>
static public class PublisherProjectPatterns
{
    // ● private
    static string ReplaceCore(string Text, PublisherProjectSettings Project, string PublishRootFolder, int Depth)
    {
        if (string.IsNullOrWhiteSpace(Text) || Project == null || Depth > 4)
            return Text ?? string.Empty;

        string Result = Text;
        Result = Result.Replace("{ProjectName}", Project.Name ?? string.Empty);
        Result = Result.Replace("{PackageName}", Project.PackageName ?? string.Empty);
        Result = Result.Replace("{AppName}", Project.AppName ?? string.Empty);
        Result = Result.Replace("{Version}", Project.Version ?? string.Empty);
        Result = Result.Replace("{PublishRootFolder}", PublishRootFolder ?? string.Empty);
        Result = Result.Replace("{Architecture}", Project.Deb?.Architecture ?? string.Empty);

        if (Result == Text)
            return Result;

        return ReplaceCore(Result, Project, PublishRootFolder, Depth + 1);
    }
    static bool IsRootedPathText(string PathText)
    {
        return !string.IsNullOrWhiteSpace(PathText)
               && (Path.IsPathRooted(PathText) || Regex.IsMatch(PathText, "^[A-Za-z]:") || PathText.StartsWith("\\\\"));
    }
    static string CombinePathText(string Folder, string Name)
    {
        Folder = Folder ?? string.Empty;
        Name = Name ?? string.Empty;
        if (string.IsNullOrWhiteSpace(Folder) || IsRootedPathText(Name))
            return Name;
        if (string.IsNullOrWhiteSpace(Name))
            return Folder;

        bool IsWindowsPath = Folder.Contains('\\') || Regex.IsMatch(Folder, "^[A-Za-z]:");
        char Separator = IsWindowsPath ? '\\' : Path.DirectorySeparatorChar;
        return Folder.TrimEnd('\\', '/') + Separator + Name.TrimStart('\\', '/');
    }

    // ● public
    /// <summary>
    /// Resolves known project pattern tokens in the specified text.
    /// </summary>
    static public string Resolve(string Text, PublisherProjectSettings Project)
    {
        return ReplaceCore(Text, Project, Project?.PublishRootFolder, 0);
    }
    /// <summary>
    /// Resolves known project pattern tokens in the specified text for a publish target.
    /// </summary>
    static public string Resolve(string Text, PublisherProjectSettings Project, DotNetPublishSettings Settings)
    {
        return ReplaceCore(Text, Project, ResolvePublishRootFolder(Project, Settings), 0);
    }
    /// <summary>
    /// Resolves known project pattern tokens for build settings, without a publish root folder.
    /// </summary>
    static public string ResolveBuild(string Text, PublisherProjectSettings Project)
    {
        return ReplaceCore(Text, Project, string.Empty, 0);
    }
    /// <summary>
    /// Resolves the versioned publish root folder for a project.
    /// </summary>
    static public string ResolvePublishRootFolder(PublisherProjectSettings Project)
    {
        if (Project == null)
            return string.Empty;

        string PublishRootFolder = Resolve(Project.PublishRootFolder, Project);
        if (string.IsNullOrWhiteSpace(PublishRootFolder))
            return string.Empty;
        string Version = Sys.StrToValidFileName(Resolve(Project.Version, Project));
        return string.IsNullOrWhiteSpace(Version) ? PublishRootFolder : Path.Combine(PublishRootFolder, Version);
    }
    /// <summary>
    /// Resolves the versioned publish root folder for a publish target.
    /// </summary>
    static public string ResolvePublishRootFolder(PublisherProjectSettings Project, DotNetPublishSettings Settings)
    {
        if (Project == null)
            return string.Empty;

        string PublishRootFolder = Resolve(Settings?.PublishRootFolder, Project);
        if (string.IsNullOrWhiteSpace(PublishRootFolder))
            PublishRootFolder = Resolve(Project.PublishRootFolder, Project);
        if (string.IsNullOrWhiteSpace(PublishRootFolder))
            return string.Empty;
        string Version = Sys.StrToValidFileName(Resolve(Project.Version, Project));
        return string.IsNullOrWhiteSpace(Version) ? PublishRootFolder : Path.Combine(PublishRootFolder, Version);
    }
    /// <summary>
    /// Resolves the publish output folder for a publish target.
    /// </summary>
    static public string ResolvePublishOutputFolder(PublisherProjectSettings Project, DotNetPublishSettings Settings)
    {
        string PublishRootFolder = ResolvePublishRootFolder(Project, Settings);
        string OutputFolderName = Resolve(Settings?.OutputFolderName, Project, Settings);
        if (string.IsNullOrWhiteSpace(PublishRootFolder) || string.IsNullOrWhiteSpace(OutputFolderName))
            return string.Empty;
        return Path.Combine(PublishRootFolder, OutputFolderName);
    }
    /// <summary>
    /// Resolves the Linux source folder for Debian package generation.
    /// </summary>
    static public string ResolveLinuxSourceFolder(PublisherProjectSettings Project)
    {
        if (Project == null)
            return string.Empty;

        return ResolveBuild(Project.Deb.LinuxSourceFolder, Project);
    }
    /// <summary>
    /// Resolves the Debian build output folder.
    /// </summary>
    static public string ResolveDebBuildOutputFolder(PublisherProjectSettings Project)
    {
        if (Project == null)
            return string.Empty;

        string SourceFolder = ResolveLinuxSourceFolder(Project);
        string OutputFolder = ResolveBuild(Project.Deb.BuildOutputFolder, Project);
        if (string.IsNullOrWhiteSpace(OutputFolder))
            OutputFolder = "Output";
        if (!IsRootedPathText(OutputFolder) && string.IsNullOrWhiteSpace(SourceFolder))
            return string.Empty;
        return IsRootedPathText(OutputFolder) ? OutputFolder : CombinePathText(SourceFolder, OutputFolder);
    }
    /// <summary>
    /// Resolves the Windows source folder for Inno Setup generation.
    /// </summary>
    static public string ResolveWindowsSourceFolder(PublisherProjectSettings Project)
    {
        if (Project == null)
            return string.Empty;

        return ResolveBuild(Project.Inno.WindowsSourceFolder, Project);
    }
    /// <summary>
    /// Resolves the Inno Setup build output folder.
    /// </summary>
    static public string ResolveInnoBuildOutputFolder(PublisherProjectSettings Project)
    {
        if (Project == null)
            return string.Empty;

        string SourceFolder = ResolveWindowsSourceFolder(Project);
        string OutputFolder = ResolveBuild(Project.Inno.BuildOutputFolder, Project);
        if (string.IsNullOrWhiteSpace(OutputFolder))
            OutputFolder = "Output";
        if (!IsRootedPathText(OutputFolder) && string.IsNullOrWhiteSpace(SourceFolder))
            return string.Empty;
        return IsRootedPathText(OutputFolder) ? OutputFolder : CombinePathText(SourceFolder, OutputFolder);
    }
}
