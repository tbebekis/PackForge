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
}
