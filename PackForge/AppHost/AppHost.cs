// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Central application host for startup state and top-level UI services.
/// </summary>
static internal partial class AppHost
{
    // ● constructor
    static AppHost()
    {
#if DEBUG
        Sys.DebugMode = true;
#endif
        StartupWindow = new();
        Settings = new();
        Projects = new();
        Sys.UiLogProc = Log;
    }

    // ● public
    /// <summary>
    /// Appends a line to the application log when it is initialized.
    /// </summary>
    static public void Log(string Text)
    {
        if (LogBox.IsInitialized)
            LogBox.AppendLine(Text);
    }
    /// <summary>
    /// Creates a new project using initial defaults.
    /// </summary>
    static public PublisherProjectSettings NewProject(string ProjectName)
    {
        if (string.IsNullOrWhiteSpace(ProjectName))
            throw new Exception("Project name is required.");
        ProjectName = ProjectName.Trim();
        Projects.ValidateName(ProjectName);
        if (Projects.Find(ProjectName) != null)
            throw new Exception($"Project already exists: {ProjectName}");

        AppSettings Settings = AppHost.Settings;
        string PackageName = ProjectName.ToLowerInvariant();
        PublisherProjectSettings Project = new()
        {
            Name = ProjectName,
            AppName = "{ProjectName}",
            PackageName = PackageName,
            Version = DateTime.Today.ToString("yyyy.M.d", CultureInfo.InvariantCulture),
            Maintainer = Settings.DefaultMaintainer,
            Homepage = Settings.DefaultHomepage,
            LinuxPublish =
            {
                RuntimeIdentifier = Settings.DefaultLinuxRuntimeIdentifier,
                Configuration = Settings.DefaultConfiguration,
                SelfContained = Settings.DefaultSelfContained,
                PublishSingleFile = Settings.DefaultPublishSingleFile,
                PublishTrimmed = Settings.DefaultPublishTrimmed
            },
            WindowsPublish =
            {
                RuntimeIdentifier = Settings.DefaultWindowsRuntimeIdentifier,
                Configuration = Settings.DefaultConfiguration,
                SelfContained = Settings.DefaultSelfContained,
                PublishSingleFile = Settings.DefaultPublishSingleFile,
                PublishTrimmed = Settings.DefaultPublishTrimmed
            },
            Deb =
            {
                Architecture = Settings.DefaultDebArchitecture,
                ExecutableName = "{ProjectName}",
                CommandName = "{PackageName}",
                DesktopCategories = Settings.DefaultDesktopCategories,
                DesktopKeywords = Settings.DefaultDesktopKeywords,
                InstallDir = "/usr/lib/{PackageName}",
                Section = Settings.DefaultDebSection,
                Priority = Settings.DefaultDebPriority
            },
            Inno =
            {
                ScriptFileName = "{ProjectName}.iss",
                AppExeName = "{ProjectName}.exe",
                WindowsPublishFolderName = "{ProjectName}-Windows-x64",
                OutputBaseFilename = "{ProjectName}_Setup_{Version}",
                DefaultDirName = Settings.DefaultInnoInstallScope.IsSameText("All Users") ? "{autopf}\\{ProjectName}" : "{localappdata}\\Programs\\{ProjectName}",
                InstallScope = Settings.DefaultInnoInstallScope,
                DefaultGroupName = "{ProjectName}",
                SetupIconFile = string.Empty,
                WizardImageFile = string.Empty,
                WizardSmallImageFile = string.Empty,
                Compression = Settings.DefaultInnoCompression,
                SolidCompression = Settings.DefaultInnoSolidCompression,
                PrivilegesRequired = Settings.DefaultInnoPrivilegesRequired
            }
        };
        Project.LinuxPublish.OutputFolderName = "{ProjectName}-Linux-x64";
        Project.WindowsPublish.OutputFolderName = "{ProjectName}-Windows-x64";
        Projects.Items.Add(Project);
        SetCurrentProject(Project);
        Projects.Save(Project);
        ProjectsChanged?.Invoke(null, EventArgs.Empty);
        ShowProjectEditor(Project);
        return Project;
    }
    /// <summary>
    /// Saves the current project.
    /// </summary>
    static public void SaveCurrentProject()
    {
        if (CurrentProject == null)
            throw new Exception("No project selected.");
        SaveProject(CurrentProject);
    }
    /// <summary>
    /// Saves a project.
    /// </summary>
    static public void SaveProject(PublisherProjectSettings Project)
    {
        if (Project == null)
            throw new Exception("No project selected.");
        Projects.Save(Project);
        ProjectsChanged?.Invoke(null, EventArgs.Empty);
        Log($"Project saved: {Project.Name}");
    }
    /// <summary>
    /// Deletes the current project.
    /// </summary>
    static public void DeleteCurrentProject()
    {
        if (CurrentProject == null)
            throw new Exception("No project selected.");
        string Name = CurrentProject.Name;
        Projects.Delete(Name);
        CurrentProject = null;
        ProjectsChanged?.Invoke(null, EventArgs.Empty);
        CurrentProjectChanged?.Invoke(null, EventArgs.Empty);
        ContentHandler?.CloseForm(GetProjectEditorFormId(Name));
        Log($"Project deleted: {Name}");
    }
    /// <summary>
    /// Selects the current project.
    /// </summary>
    static public void SetCurrentProject(PublisherProjectSettings Project)
    {
        if (ReferenceEquals(CurrentProject, Project))
            return;
        CurrentProject = Project;
        CurrentProjectChanged?.Invoke(null, EventArgs.Empty);
    }
    /// <summary>
    /// Validates the current project and writes validation messages to the log.
    /// </summary>
    static public List<ValidationIssue> ValidateCurrentProject()
    {
        List<ValidationIssue> Result = PublisherProjectValidator.Validate(CurrentProject);
        Log($"Validation issues: {Result.Count}");
        foreach (ValidationIssue Issue in Result)
            Log(Issue.ToString());
        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets the startup window.
    /// </summary>
    static public StartupWindow StartupWindow { get; private set; }
    /// <summary>
    /// Gets the main application window.
    /// </summary>
    static public MainWindow MainWindow { get; private set; }
    /// <summary>
    /// Gets the desktop application lifetime.
    /// </summary>
    static public IClassicDesktopStyleApplicationLifetime AvaloniaDesktop { get; private set; }
    /// <summary>
    /// Gets the left sidebar form pager handler.
    /// </summary>
    static public AppFormPagerHandler SideBarHandler { get; private set; }
    /// <summary>
    /// Gets the right content form pager handler.
    /// </summary>
    static public AppFormPagerHandler ContentHandler { get; private set; }
    /// <summary>
    /// Gets the file-backed project store.
    /// </summary>
    static public PublisherProjectStore Projects { get; private set; }
    /// <summary>
    /// Gets the application settings.
    /// </summary>
    static public AppSettings Settings { get; private set; }
    /// <summary>
    /// Gets the currently selected project.
    /// </summary>
    static public PublisherProjectSettings CurrentProject { get; private set; }
    /// <summary>
    /// Occurs when the project list changes.
    /// </summary>
    static public event EventHandler ProjectsChanged;
    /// <summary>
    /// Occurs when the current project changes.
    /// </summary>
    static public event EventHandler CurrentProjectChanged;
}
