// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Locates the Inno Setup compiler executable on Windows machines.
/// </summary>
static public class InnoSetupLocator
{
    // ● private
    static IEnumerable<string> CandidateFolders()
    {
        string[] Roots =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetEnvironmentVariable("ProgramW6432") ?? string.Empty,
            Environment.GetEnvironmentVariable("ProgramFiles") ?? string.Empty,
            Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? string.Empty
        };

        string[] Names =
        {
            "Inno Setup 7",
            "Inno Setup 6",
            "Inno Setup 5"
        };

        foreach (string Root in Roots.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
            foreach (string Name in Names)
                yield return Path.Combine(Root, Name);
    }

    // ● public
    /// <summary>
    /// Converts a compiler folder or executable path to the executable path.
    /// </summary>
    static public string NormalizeCompilerPath(string PathText)
    {
        if (string.IsNullOrWhiteSpace(PathText))
            return string.Empty;
        PathText = Environment.ExpandEnvironmentVariables(PathText.Trim());
        if (Directory.Exists(PathText))
            PathText = Path.Combine(PathText, "ISCC.exe");
        return File.Exists(PathText) ? PathText : string.Empty;
    }
    /// <summary>
    /// Searches known Windows installation folders for ISCC.exe.
    /// </summary>
    static public string FindCompilerPath()
    {
        if (!OperatingSystem.IsWindows())
            return string.Empty;

        foreach (string Folder in CandidateFolders())
        {
            string FilePath = Path.Combine(Folder, "ISCC.exe");
            if (File.Exists(FilePath))
                return FilePath;
        }

        return string.Empty;
    }
}
