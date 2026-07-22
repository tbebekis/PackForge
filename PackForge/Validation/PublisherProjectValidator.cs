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
    static string Resolve(string Text, PublisherProjectSettings Project) => PublisherProjectPatterns.Resolve(Text, Project);

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
        string PublishRootFolder = Resolve(Project.PublishRootFolder, Project);
        string InstallerOutputFolder = Resolve(Project.InstallerOutputFolderOrFallback, Project);
        string LinuxIconFilePath = Resolve(Project.LinuxIconFilePath, Project);
        string WindowsIconFilePath = Resolve(Project.WindowsIconFilePath, Project);

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
        if (string.IsNullOrWhiteSpace(PublishRootFolder))
            Add(Result, "Error", "PublishRoot.Required", "Publish root folder is required.");
        else if (File.Exists(PublishRootFolder))
            Add(Result, "Error", "PublishRoot.IsFile", "Publish root folder points to an existing file, not a folder.", PublishRootFolder);
        if (!string.IsNullOrWhiteSpace(InstallerOutputFolder) && !Directory.Exists(InstallerOutputFolder))
        {
            if (File.Exists(InstallerOutputFolder))
                Add(Result, "Error", "InstallerOutputFolder.IsFile", "Installer output folder points to an existing file, not a folder.", InstallerOutputFolder);
            else
                Add(Result, "Warning", "InstallerOutputFolder.Missing", "Installer output folder does not exist yet.", InstallerOutputFolder);
        }

        if (Project.Deb.IsEnabled)
        {
            string LinuxOutputFolderName = Resolve(Project.LinuxPublish.OutputFolderName, Project);
            string DebCommandName = Resolve(Project.Deb.CommandName, Project);

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

            string LinuxPublishFolder = CombinePath(PublishRootFolder, LinuxOutputFolderName);
            if (!string.IsNullOrWhiteSpace(LinuxOutputFolderName) && !Directory.Exists(LinuxPublishFolder))
                Add(Result, "Warning", "Deb.PublishFolder.Missing", "Linux publish folder does not exist yet.", LinuxPublishFolder);
        }

        if (Project.Inno.IsEnabled)
        {
            string WindowsPublishFolderName = Resolve(Project.Inno.WindowsPublishFolderName, Project);
            string OutputBaseFilename = Resolve(Project.Inno.OutputBaseFilename, Project);
            string WizardImageFile = Resolve(Project.Inno.WizardImageFile, Project);
            string WizardSmallImageFile = Resolve(Project.Inno.WizardSmallImageFile, Project);

            if (!string.IsNullOrWhiteSpace(WindowsIconFilePath) && !File.Exists(WindowsIconFilePath))
                Add(Result, "Error", "Inno.Icon.Missing", "Windows ICO icon file does not exist.", WindowsIconFilePath);
            if (!string.IsNullOrWhiteSpace(WizardImageFile) && !File.Exists(WizardImageFile))
                Add(Result, "Error", "Inno.WizardImage.Missing", "Inno Setup wizard image file does not exist.", WizardImageFile);
            if (!string.IsNullOrWhiteSpace(WizardSmallImageFile) && !File.Exists(WizardSmallImageFile))
                Add(Result, "Error", "Inno.WizardSmallImage.Missing", "Inno Setup small wizard image file does not exist.", WizardSmallImageFile);
            if (string.IsNullOrWhiteSpace(Project.AppId))
                Add(Result, "Error", "Inno.AppId.Required", "Inno Setup AppId is required.");
            string WindowsPublishFolder = CombinePath(PublishRootFolder, WindowsPublishFolderName);
            if (!string.IsNullOrWhiteSpace(WindowsPublishFolderName) && !Directory.Exists(WindowsPublishFolder))
                Add(Result, "Warning", "Inno.PublishFolder.Missing", "Windows publish folder does not exist yet.", WindowsPublishFolder);
            if (string.IsNullOrWhiteSpace(OutputBaseFilename))
                Add(Result, "Error", "Inno.OutputBaseFilename.Required", "Inno output base filename is required.");
        }

        return Result;
    }
}
