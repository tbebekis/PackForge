// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Validates PackForge project settings.
/// </summary>
static public class PublisherProjectValidator
{
    // ● private
    static void Add(List<ValidationIssue> List, string Severity, string Code, string Message, string Path = "")
    {
        List.Add(new ValidationIssue
        {
            Severity = Severity,
            Code = Code,
            Message = Message,
            Path = Path ?? string.Empty
        });
    }
    static string CombinePath(string Folder, string Name)
    {
        if (string.IsNullOrWhiteSpace(Folder) || string.IsNullOrWhiteSpace(Name))
            return string.Empty;
        return System.IO.Path.Combine(Folder, Name);
    }
    static string ResolvePathFromFolder(string PathText, string Folder)
    {
        if (string.IsNullOrWhiteSpace(PathText) || string.IsNullOrWhiteSpace(Folder) || System.IO.Path.IsPathRooted(PathText))
            return PathText ?? string.Empty;
        return System.IO.Path.Combine(Folder, PathText);
    }
    static string Resolve(string Text, PublisherProjectSettings Project) => PublisherProjectPatterns.Resolve(Text, Project);
    static string Resolve(string Text, PublisherProjectSettings Project, DotNetPublishSettings Settings) => PublisherProjectPatterns.Resolve(Text, Project, Settings);
    static string ResolveBuild(string Text, PublisherProjectSettings Project) => PublisherProjectPatterns.ResolveBuild(Text, Project);

    // ● public
    /// <summary>
    /// Validates the specified project settings.
    /// </summary>
    static public List<ValidationIssue> Validate(PublisherProjectSettings Project)
    {
        List<ValidationIssue> Result = new();
        if (Project == null)
        {
            Add(Result, "Error", "Project.Null", "Project settings are null.");
            return Result;
        }

        if (string.IsNullOrWhiteSpace(Project.Name))
            Add(Result, "Error", "Project.Name.Required", "Project name is required.");
        string AppName = Resolve(Project.AppName, Project);
        string PackageName = Resolve(Project.PackageName, Project);
        string ProjectFilePath = Resolve(Project.ProjectFilePath, Project);
        string SolutionFolder = Resolve(Project.SolutionFolder, Project);
        string LinuxPublishRootFolder = PublisherProjectPatterns.ResolvePublishRootFolder(Project, Project.LinuxPublish);
        string WindowsPublishRootFolder = PublisherProjectPatterns.ResolvePublishRootFolder(Project, Project.WindowsPublish);
        string LinuxIconFilePath = ResolveBuild(Project.LinuxIconFilePath, Project);

        if (string.IsNullOrWhiteSpace(AppName))
            Add(Result, "Error", "App.Name.Required", "App name is required.");
        if (string.IsNullOrWhiteSpace(PackageName))
            Add(Result, "Error", "Package.Name.Required", "Package name is required.");
        if (string.IsNullOrWhiteSpace(Project.Version))
            Add(Result, "Error", "Version.Required", "Version is required.");
        if (string.IsNullOrWhiteSpace(ProjectFilePath))
            Add(Result, "Error", "ProjectFile.Required", ".csproj path is required.");
        else if (!File.Exists(ProjectFilePath))
            Add(Result, "Error", "ProjectFile.Missing", ".csproj file does not exist.", ProjectFilePath);
        if (!string.IsNullOrWhiteSpace(SolutionFolder) && !Directory.Exists(SolutionFolder))
            Add(Result, "Warning", "SolutionFolder.Missing", "Solution folder does not exist.", SolutionFolder);
        if (!string.IsNullOrWhiteSpace(LinuxPublishRootFolder) && File.Exists(LinuxPublishRootFolder))
            Add(Result, "Error", "LinuxPublishRoot.IsFile", "Linux publish root folder points to an existing file, not a folder.", LinuxPublishRootFolder);
        if (!string.IsNullOrWhiteSpace(WindowsPublishRootFolder) && File.Exists(WindowsPublishRootFolder))
            Add(Result, "Error", "WindowsPublishRoot.IsFile", "Windows publish root folder points to an existing file, not a folder.", WindowsPublishRootFolder);
        if (Project.LinuxPublish.IsEnabled && string.IsNullOrWhiteSpace(LinuxPublishRootFolder))
            Add(Result, "Error", "LinuxPublishRoot.Required", "Linux publish root folder is required.");
        if (Project.WindowsPublish.IsEnabled && string.IsNullOrWhiteSpace(WindowsPublishRootFolder))
            Add(Result, "Error", "WindowsPublishRoot.Required", "Windows publish root folder is required.");
        if (Project.Deb.IsEnabled)
        {
            string LinuxSourceFolder = PublisherProjectPatterns.ResolveLinuxSourceFolder(Project);
            string DebBuildOutputFolder = PublisherProjectPatterns.ResolveDebBuildOutputFolder(Project);
            string DebCommandName = ResolveBuild(Project.Deb.CommandName, Project);
            LinuxIconFilePath = ResolvePathFromFolder(LinuxIconFilePath, LinuxSourceFolder);

            if (string.IsNullOrWhiteSpace(LinuxSourceFolder))
                Add(Result, "Error", "Deb.SourceFolder.Required", "Linux source folder is required.");
            if (!string.IsNullOrWhiteSpace(DebBuildOutputFolder) && File.Exists(DebBuildOutputFolder))
                Add(Result, "Error", "Deb.BuildOutputFolder.IsFile", "Debian build output folder points to an existing file, not a folder.", DebBuildOutputFolder);
            if (!string.IsNullOrWhiteSpace(LinuxIconFilePath) && !File.Exists(LinuxIconFilePath))
                Add(Result, "Error", "Deb.Icon.Missing", "Linux PNG icon file does not exist.", LinuxIconFilePath);
            if (!Regex.IsMatch(PackageName ?? string.Empty, "^[a-z0-9][a-z0-9+.-]+$"))
                Add(Result, "Error", "Deb.PackageName.Invalid", "Debian package name is invalid.");
            if (string.IsNullOrWhiteSpace(Project.Deb.Architecture))
                Add(Result, "Error", "Deb.Architecture.Required", "Debian architecture is required.");
            if (string.IsNullOrWhiteSpace(Project.Deb.Dependencies))
                Add(Result, "Error", "Deb.Dependencies.Required", "Debian dependencies are required.");
            if (!Regex.IsMatch(DebCommandName ?? string.Empty, "^[a-z0-9][a-z0-9._-]*$"))
                Add(Result, "Error", "Deb.CommandName.Invalid", "Command name must be lowercase and shell-safe.");

            if (!string.IsNullOrWhiteSpace(LinuxSourceFolder) && !Directory.Exists(LinuxSourceFolder))
                Add(Result, "Warning", "Deb.SourceFolder.Missing", "Linux source folder does not exist yet.", LinuxSourceFolder);
        }

        if (Project.Inno.IsEnabled)
        {
            string WindowsSourceFolder = PublisherProjectPatterns.ResolveWindowsSourceFolder(Project);
            string InnoBuildOutputFolder = PublisherProjectPatterns.ResolveInnoBuildOutputFolder(Project);
            string OutputBaseFilename = ResolveBuild(Project.Inno.OutputBaseFilename, Project);
            string SetupIconFile = ResolveBuild(Project.Inno.SetupIconFile, Project);
            string WizardImageFile = ResolveBuild(Project.Inno.WizardImageFile, Project);
            string WizardSmallImageFile = ResolveBuild(Project.Inno.WizardSmallImageFile, Project);
            SetupIconFile = ResolvePathFromFolder(SetupIconFile, WindowsSourceFolder);
            WizardImageFile = ResolvePathFromFolder(WizardImageFile, WindowsSourceFolder);
            WizardSmallImageFile = ResolvePathFromFolder(WizardSmallImageFile, WindowsSourceFolder);

            if (string.IsNullOrWhiteSpace(WindowsSourceFolder))
                Add(Result, "Error", "Inno.SourceFolder.Required", "Windows source folder is required.");
            if (!string.IsNullOrWhiteSpace(InnoBuildOutputFolder) && File.Exists(InnoBuildOutputFolder))
                Add(Result, "Error", "Inno.BuildOutputFolder.IsFile", "Inno Setup build output folder points to an existing file, not a folder.", InnoBuildOutputFolder);
            if (!string.IsNullOrWhiteSpace(SetupIconFile) && !File.Exists(SetupIconFile))
                Add(Result, "Error", "Inno.SetupIcon.Missing", "Inno Setup icon file does not exist.", SetupIconFile);
            if (!string.IsNullOrWhiteSpace(WizardImageFile) && !File.Exists(WizardImageFile))
                Add(Result, "Error", "Inno.WizardImage.Missing", "Inno Setup wizard image file does not exist.", WizardImageFile);
            if (!string.IsNullOrWhiteSpace(WizardSmallImageFile) && !File.Exists(WizardSmallImageFile))
                Add(Result, "Error", "Inno.WizardSmallImage.Missing", "Inno Setup small wizard image file does not exist.", WizardSmallImageFile);
            if (string.IsNullOrWhiteSpace(Project.AppId))
                Add(Result, "Error", "Inno.AppId.Required", "Inno Setup AppId is required.");
            if (!string.IsNullOrWhiteSpace(WindowsSourceFolder) && !Directory.Exists(WindowsSourceFolder))
                Add(Result, "Warning", "Inno.SourceFolder.Missing", "Windows source folder does not exist yet.", WindowsSourceFolder);
            if (string.IsNullOrWhiteSpace(OutputBaseFilename))
                Add(Result, "Error", "Inno.OutputBaseFilename.Required", "Inno output base filename is required.");
        }

        return Result;
    }
}
