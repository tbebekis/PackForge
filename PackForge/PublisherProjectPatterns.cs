// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Resolves project setting pattern tokens.
/// </summary>
static public class PublisherProjectPatterns
{
    // ● private
    static string ReplaceCore(string Text, PublisherProjectSettings Project, int Depth)
    {
        if (string.IsNullOrWhiteSpace(Text) || Project == null || Depth > 4)
            return Text ?? string.Empty;

        string Result = Text;
        Result = Result.Replace("{ProjectName}", Project.Name ?? string.Empty);
        Result = Result.Replace("{PackageName}", Project.PackageName ?? string.Empty);
        Result = Result.Replace("{AppName}", Project.AppName ?? string.Empty);
        Result = Result.Replace("{Version}", Project.Version ?? string.Empty);
        Result = Result.Replace("{PublishRootFolder}", Project.PublishRootFolder ?? string.Empty);
        Result = Result.Replace("{InstallerOutputFolder}", Project.InstallerOutputFolderOrFallback);
        Result = Result.Replace("{Architecture}", Project.Deb?.Architecture ?? string.Empty);

        if (Result == Text)
            return Result;

        return ReplaceCore(Result, Project, Depth + 1);
    }

    // ● public
    /// <summary>
    /// Resolves known project pattern tokens in the specified text.
    /// </summary>
    static public string Resolve(string Text, PublisherProjectSettings Project)
    {
        return ReplaceCore(Text, Project, 0);
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
    /// Resolves the installer output folder for a project.
    /// </summary>
    static public string ResolveInstallerOutputFolder(PublisherProjectSettings Project)
    {
        if (Project == null)
            return string.Empty;

        string PublishRootFolder = ResolvePublishRootFolder(Project);
        string InstallerOutputFolder = Resolve(Project.InstallerOutputFolder, Project);
        if (string.IsNullOrWhiteSpace(InstallerOutputFolder))
            return PublishRootFolder;
        if (Path.IsPathRooted(InstallerOutputFolder))
            return InstallerOutputFolder;
        return Path.Combine(PublishRootFolder, InstallerOutputFolder);
    }
}
