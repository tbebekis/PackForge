// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Project editor hosted in the content area.
/// </summary>
public partial class ProjectEditorForm : AppForm
{
    // ● private fields
    PublisherProjectSettings fProject;
    Tripous.Desktop.ToolBar ToolBar;
    Button btnSave;
    Button btnPublishLinux;
    Button btnPublishWindows;
    Button btnGenerateDebScript;
    Button btnGenerateInnoScript;
    Button btnBuildDeb;
    Button btnBuildInno;
    Button btnOpenOutputFolder;
    Button btnClose;
    bool fIsBusy;

    // ● private
    string TextOf(TextBox Edit) => Edit.Text ?? string.Empty;
    string TextOf(ComboBox Combo)
    {
        if (Combo.SelectedItem is ComboBoxItem Item)
            return Item.Content?.ToString() ?? string.Empty;
        return Combo.SelectedItem?.ToString() ?? string.Empty;
    }
    string TextOf(ListBox List)
    {
        List<string> Parts = new();
        foreach (object SelectedItem in List.SelectedItems)
        {
            if (SelectedItem is ListBoxItem Item)
                Parts.Add(Item.Content?.ToString() ?? string.Empty);
            else if (SelectedItem != null)
                Parts.Add(SelectedItem.ToString());
        }
        return string.Join(" ", Parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
    void SetText(TextBox Edit, string Text)
    {
        Edit.Text = Text ?? string.Empty;
    }
    void SetText(ComboBox Combo, string Text)
    {
        Text = Text ?? string.Empty;
        foreach (object ItemObject in Combo.Items)
        {
            if (ItemObject is ComboBoxItem Item && Text.IsSameText(Item.Content?.ToString()))
            {
                Combo.SelectedItem = Item;
                return;
            }
        }
        Combo.SelectedIndex = -1;
    }
    void SetText(ListBox List, string Text)
    {
        List.SelectedItems.Clear();
        string[] Parts = (Text ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (object ItemObject in List.Items)
        {
            if (ItemObject is ComboBoxItem)
                continue;
            if (ItemObject is ListBoxItem Item && Parts.Any(x => x.IsSameText(Item.Content?.ToString())))
                List.SelectedItems.Add(Item);
        }
    }
    string GetDefaultDirName(string InstallScope)
    {
        return InstallScope.IsSameText("All Users") ? "{autopf}\\{ProjectName}" : "{localappdata}\\Programs\\{ProjectName}";
    }
    string GetInstallScope(InnoSetupSettings Settings)
    {
        if (!string.IsNullOrWhiteSpace(Settings.InstallScope))
            return Settings.InstallScope;
        if ((Settings.DefaultDirName ?? string.Empty).StartsWith("{autopf}", StringComparison.OrdinalIgnoreCase))
            return "All Users";
        return "User only";
    }
    void CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;
            btnSave = ToolBar.AddButton("disk.png", "Save", AnyClick);
            ToolBar.AddSeparator();
            btnPublishLinux = ToolBar.AddButton("download_for_linux.png", "Publish Linux", AnyClick);
            btnPublishWindows = ToolBar.AddButton("download_for_windows.png", "Publish Windows", AnyClick);
            ToolBar.AddSeparator();
            btnGenerateDebScript = ToolBar.AddButton("script_lightning.png", "Generate Deb Script", AnyClick);
            btnGenerateInnoScript = ToolBar.AddButton("script_lightning.png", "Generate Inno Script", AnyClick);
            ToolBar.AddSeparator();
            btnBuildDeb = ToolBar.AddButton("compile.png", "Build Deb", AnyClick);
            btnBuildInno = ToolBar.AddButton("compile.png", "Build Inno", AnyClick);
            btnOpenOutputFolder = ToolBar.AddButton("folder.png", "Open Output Folder", AnyClick);
            ToolBar.AddSeparator();
            btnClose = ToolBar.AddButton("door_out.png", "Close", AnyClick);
        }
    }
    void LoadProject(PublisherProjectSettings Project)
    {
        if (Project == null)
        {
            Clear();
            return;
        }

        SetText(edtName, Project.Name);
        SetText(edtAppName, Project.AppName);
        SetText(edtPackageName, Project.PackageName);
        SetText(edtVersion, Project.Version);
        SetText(edtMaintainer, Project.Maintainer);
        SetText(edtHomepage, Project.Homepage);
        SetText(edtDescriptionShort, Project.DescriptionShort);
        SetText(edtDescriptionLong, Project.DescriptionLong);
        SetText(edtSolutionFolder, Project.SolutionFolder);
        SetText(edtProjectFilePath, Project.ProjectFilePath);
        SetText(edtPublishRootFolder, Project.PublishRootFolder);
        SetText(edtInstallerOutputFolder, Project.InstallerOutputFolder);
        SetText(edtLinuxIconFilePath, Project.LinuxIconFilePath);
        SetText(edtWindowsIconFilePath, Project.WindowsIconFilePath);

        chkLinuxEnabled.IsChecked = Project.LinuxPublish.IsEnabled;
        SetText(cboLinuxConfiguration, Project.LinuxPublish.Configuration);
        SetText(cboLinuxRuntime, Project.LinuxPublish.RuntimeIdentifier);
        SetText(edtLinuxOutput, Project.LinuxPublish.OutputFolderName);
        SetText(edtLinuxExtra, Project.LinuxPublish.ExtraArguments);
        chkLinuxSelfContained.IsChecked = Project.LinuxPublish.SelfContained;
        chkLinuxSingleFile.IsChecked = Project.LinuxPublish.PublishSingleFile;
        chkLinuxTrimmed.IsChecked = Project.LinuxPublish.PublishTrimmed;

        chkWindowsEnabled.IsChecked = Project.WindowsPublish.IsEnabled;
        SetText(cboWindowsConfiguration, Project.WindowsPublish.Configuration);
        SetText(cboWindowsRuntime, Project.WindowsPublish.RuntimeIdentifier);
        SetText(edtWindowsOutput, Project.WindowsPublish.OutputFolderName);
        SetText(edtWindowsExtra, Project.WindowsPublish.ExtraArguments);
        chkWindowsSelfContained.IsChecked = Project.WindowsPublish.SelfContained;
        chkWindowsSingleFile.IsChecked = Project.WindowsPublish.PublishSingleFile;
        chkWindowsTrimmed.IsChecked = Project.WindowsPublish.PublishTrimmed;

        chkDebEnabled.IsChecked = Project.Deb.IsEnabled;
        SetText(cboDebArchitecture, Project.Deb.Architecture);
        SetText(edtDebExecutable, Project.Deb.ExecutableName);
        SetText(edtDebCommand, Project.Deb.CommandName);
        SetText(edtDebCategories, Project.Deb.DesktopCategories);
        SetText(edtDebKeywords, Project.Deb.DesktopKeywords);
        SetText(edtDebDependencies, Project.Deb.Dependencies);
        SetText(edtDebInstallDir, Project.Deb.InstallDir);
        SetText(cboDebSection, Project.Deb.Section);
        SetText(cboDebPriority, Project.Deb.Priority);
        SetText(edtDebScriptFile, Project.Deb.ScriptFileName);
        SetText(edtDebOutputPattern, Project.Deb.OutputFileNamePattern);

        chkInnoEnabled.IsChecked = Project.Inno.IsEnabled;
        SetText(edtAppId, Project.AppId);
        SetText(edtInnoScriptFile, Project.Inno.ScriptFileName);
        SetText(edtInnoPublisher, Project.Inno.AppPublisher);
        SetText(edtInnoPublisherUrl, Project.Inno.AppPublisherUrl);
        SetText(edtInnoAppExe, Project.Inno.AppExeName);
        SetText(edtInnoWindowsPublish, Project.Inno.WindowsPublishFolderName);
        SetText(edtInnoOutputBase, Project.Inno.OutputBaseFilename);
        SetText(cboInnoInstallScope, GetInstallScope(Project.Inno));
        SetText(edtInnoDefaultGroup, Project.Inno.DefaultGroupName);
        SetText(edtInnoSetupIcon, Project.Inno.SetupIconFile);
        SetText(edtInnoWizardImage, Project.Inno.WizardImageFile);
        SetText(edtInnoWizardSmallImage, Project.Inno.WizardSmallImageFile);
        SetText(cboInnoCompression, Project.Inno.Compression);
        chkInnoSolidCompression.IsChecked = Project.Inno.SolidCompression;
        SetText(cboInnoPrivileges, Project.Inno.PrivilegesRequired);
        SetText(edtInnoSupportUrl, Project.Inno.AppSupportUrl);
        SetText(edtInnoUpdatesUrl, Project.Inno.AppUpdatesUrl);
        SetText(lboInnoArchitecturesAllowed, Project.Inno.ArchitecturesAllowed);
        SetText(lboInnoArchitectures64, Project.Inno.ArchitecturesInstallIn64BitMode);
    }
    void SaveToProject(PublisherProjectSettings Project)
    {
        if (Project == null)
            return;

        Project.Name = TextOf(edtName);
        Project.AppName = TextOf(edtAppName);
        Project.PackageName = TextOf(edtPackageName);
        Project.Version = TextOf(edtVersion);
        Project.Maintainer = TextOf(edtMaintainer);
        Project.Homepage = TextOf(edtHomepage);
        Project.DescriptionShort = TextOf(edtDescriptionShort);
        Project.DescriptionLong = TextOf(edtDescriptionLong);
        Project.SolutionFolder = TextOf(edtSolutionFolder);
        Project.ProjectFilePath = TextOf(edtProjectFilePath);
        Project.PublishRootFolder = TextOf(edtPublishRootFolder);
        Project.InstallerOutputFolder = TextOf(edtInstallerOutputFolder);
        Project.LinuxIconFilePath = TextOf(edtLinuxIconFilePath);
        Project.WindowsIconFilePath = TextOf(edtWindowsIconFilePath);
        Project.IconFilePath = Project.LinuxIconFilePath;

        Project.LinuxPublish.IsEnabled = chkLinuxEnabled.IsChecked == true;
        Project.LinuxPublish.Configuration = TextOf(cboLinuxConfiguration);
        Project.LinuxPublish.RuntimeIdentifier = TextOf(cboLinuxRuntime);
        Project.LinuxPublish.OutputFolderName = TextOf(edtLinuxOutput);
        Project.LinuxPublish.ExtraArguments = TextOf(edtLinuxExtra);
        Project.LinuxPublish.SelfContained = chkLinuxSelfContained.IsChecked == true;
        Project.LinuxPublish.PublishSingleFile = chkLinuxSingleFile.IsChecked == true;
        Project.LinuxPublish.PublishTrimmed = chkLinuxTrimmed.IsChecked == true;

        Project.WindowsPublish.IsEnabled = chkWindowsEnabled.IsChecked == true;
        Project.WindowsPublish.Configuration = TextOf(cboWindowsConfiguration);
        Project.WindowsPublish.RuntimeIdentifier = TextOf(cboWindowsRuntime);
        Project.WindowsPublish.OutputFolderName = TextOf(edtWindowsOutput);
        Project.WindowsPublish.ExtraArguments = TextOf(edtWindowsExtra);
        Project.WindowsPublish.SelfContained = chkWindowsSelfContained.IsChecked == true;
        Project.WindowsPublish.PublishSingleFile = chkWindowsSingleFile.IsChecked == true;
        Project.WindowsPublish.PublishTrimmed = chkWindowsTrimmed.IsChecked == true;

        Project.Deb.IsEnabled = chkDebEnabled.IsChecked == true;
        Project.Deb.Architecture = TextOf(cboDebArchitecture);
        Project.Deb.ExecutableName = TextOf(edtDebExecutable);
        Project.Deb.CommandName = TextOf(edtDebCommand);
        Project.Deb.DesktopCategories = TextOf(edtDebCategories);
        Project.Deb.DesktopKeywords = TextOf(edtDebKeywords);
        Project.Deb.Dependencies = TextOf(edtDebDependencies);
        Project.Deb.InstallDir = TextOf(edtDebInstallDir);
        Project.Deb.Section = TextOf(cboDebSection);
        Project.Deb.Priority = TextOf(cboDebPriority);
        Project.Deb.ScriptFileName = TextOf(edtDebScriptFile);
        Project.Deb.OutputFileNamePattern = TextOf(edtDebOutputPattern);

        Project.Inno.IsEnabled = chkInnoEnabled.IsChecked == true;
        Project.AppId = TextOf(edtAppId);
        Project.Inno.ScriptFileName = TextOf(edtInnoScriptFile);
        Project.Inno.AppPublisher = TextOf(edtInnoPublisher);
        Project.Inno.AppPublisherUrl = TextOf(edtInnoPublisherUrl);
        Project.Inno.AppExeName = TextOf(edtInnoAppExe);
        Project.Inno.WindowsPublishFolderName = TextOf(edtInnoWindowsPublish);
        Project.Inno.OutputBaseFilename = TextOf(edtInnoOutputBase);
        Project.Inno.InstallScope = TextOf(cboInnoInstallScope);
        Project.Inno.DefaultDirName = GetDefaultDirName(Project.Inno.InstallScope);
        Project.Inno.DefaultGroupName = TextOf(edtInnoDefaultGroup);
        Project.Inno.SetupIconFile = TextOf(edtInnoSetupIcon);
        Project.Inno.WizardImageFile = TextOf(edtInnoWizardImage);
        Project.Inno.WizardSmallImageFile = TextOf(edtInnoWizardSmallImage);
        Project.Inno.Compression = TextOf(cboInnoCompression);
        Project.Inno.SolidCompression = chkInnoSolidCompression.IsChecked == true;
        Project.Inno.PrivilegesRequired = TextOf(cboInnoPrivileges);
        Project.Inno.AppSupportUrl = TextOf(edtInnoSupportUrl);
        Project.Inno.AppUpdatesUrl = TextOf(edtInnoUpdatesUrl);
        Project.Inno.ArchitecturesAllowed = TextOf(lboInnoArchitecturesAllowed);
        Project.Inno.ArchitecturesInstallIn64BitMode = TextOf(lboInnoArchitectures64);
    }
    void Clear()
    {
        LoadProject(new PublisherProjectSettings());
    }
    string Resolve(string Text) => PublisherProjectPatterns.Resolve(Text, fProject);
    string QuoteArg(string Text) => "\"" + (Text ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    string QuotePathArg(string Text) => "\"" + (Text ?? string.Empty).Replace("\"", "\\\"") + "\"";
    string GetPublishRootFolder() => PublisherProjectPatterns.ResolvePublishRootFolder(fProject);
    string GetInstallerOutputFolder() => PublisherProjectPatterns.ResolveInstallerOutputFolder(fProject);
    string GetPublishOutputFolder(DotNetPublishSettings Settings) => Path.Combine(GetPublishRootFolder(), Resolve(Settings.OutputFolderName));
    string GetDebScriptFilePath() => Path.Combine(GetPublishRootFolder(), Resolve(fProject.Deb.ScriptFileName));
    string GetInnoScriptFilePath() => Path.Combine(GetPublishRootFolder(), Resolve(fProject.Inno.ScriptFileName));
    void EnsureFolderCanBeCreated(string FolderPath, string FieldTitle)
    {
        if (string.IsNullOrWhiteSpace(FolderPath))
            throw new Exception(FieldTitle + " is required.");
        if (File.Exists(FolderPath))
            throw new Exception(FieldTitle + " points to an existing file, not a folder." + Environment.NewLine + FolderPath);
        Directory.CreateDirectory(FolderPath);
    }
    string CreateDotNetPublishArguments(DotNetPublishSettings Settings)
    {
        string OutputFolder = GetPublishOutputFolder(Settings);
        List<string> Args = new()
        {
            "publish",
            QuoteArg(Resolve(fProject.ProjectFilePath)),
            "-c",
            QuoteArg(Resolve(Settings.Configuration)),
            "-r",
            QuoteArg(Resolve(Settings.RuntimeIdentifier)),
            "--self-contained",
            Settings.SelfContained ? "true" : "false",
            "-p:PublishSingleFile=" + (Settings.PublishSingleFile ? "true" : "false"),
            "-p:PublishTrimmed=" + (Settings.PublishTrimmed ? "true" : "false"),
            "-o",
            QuoteArg(OutputFolder)
        };

        string ExtraArguments = Resolve(Settings.ExtraArguments);
        if (!string.IsNullOrWhiteSpace(ExtraArguments))
            Args.Add(ExtraArguments);

        return string.Join(" ", Args);
    }
    string CreateArtifactBaseName(string Suffix)
    {
        string Name = Resolve("{ProjectName}-{Version}-" + Suffix);
        return Sys.StrToValidFileName(Name);
    }
    void CreateWindowsZip()
    {
        string SourceFolder = GetPublishOutputFolder(fProject.WindowsPublish);
        string OutputFolder = GetInstallerOutputFolder();
        EnsureFolderCanBeCreated(OutputFolder, "Installer output folder");
        string ZipFilePath = Path.Combine(OutputFolder, CreateArtifactBaseName("windows") + ".zip");

        if (File.Exists(ZipFilePath))
            File.Delete(ZipFilePath);

        ZipFile.CreateFromDirectory(SourceFolder, ZipFilePath, CompressionLevel.Optimal, false);
        AppHost.Log("Windows zip created: " + ZipFilePath);
    }
    void CreateLinuxTarGz()
    {
        string SourceFolder = GetPublishOutputFolder(fProject.LinuxPublish);
        string OutputFolder = GetInstallerOutputFolder();
        EnsureFolderCanBeCreated(OutputFolder, "Installer output folder");
        string TarGzFilePath = Path.Combine(OutputFolder, CreateArtifactBaseName("linux") + ".tar.gz");

        if (File.Exists(TarGzFilePath))
            File.Delete(TarGzFilePath);

        using FileStream FileStream = File.Create(TarGzFilePath);
        using GZipStream GZipStream = new(FileStream, CompressionLevel.Optimal);
        TarFile.CreateFromDirectory(SourceFolder, GZipStream, false);
        AppHost.Log("Linux tar.gz created: " + TarGzFilePath);
    }
    string GetWorkingDirectory()
    {
        string Result = Resolve(fProject.SolutionFolder);
        if (!string.IsNullOrWhiteSpace(Result))
            return Result;
        string ProjectFilePath = Resolve(fProject.ProjectFilePath);
        return string.IsNullOrWhiteSpace(ProjectFilePath) ? Environment.CurrentDirectory : Path.GetDirectoryName(ProjectFilePath);
    }
    void SetBusy(bool Value)
    {
        fIsBusy = Value;
        btnSave.IsEnabled = !Value;
        btnPublishLinux.IsEnabled = !Value;
        btnPublishWindows.IsEnabled = !Value;
        btnGenerateDebScript.IsEnabled = !Value;
        btnGenerateInnoScript.IsEnabled = !Value;
        btnBuildDeb.IsEnabled = !Value;
        btnBuildInno.IsEnabled = !Value;
        btnOpenOutputFolder.IsEnabled = !Value;
        btnClose.IsEnabled = !Value;
    }
    async Task RunBusy(string Message, Func<Task> Action)
    {
        if (fIsBusy)
            return;

        PleaseWaitDialog Dialog = Ui.PleaseWait(Message, AppHost.MainWindow);
        Exception Error = null;

        try
        {
            SetBusy(true);
            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
            await Action();
        }
        catch (Exception e)
        {
            Error = e;
            AppHost.Log(e.ToString());
        }
        finally
        {
            SetBusy(false);
            Dialog.CloseDialog();
        }

        if (Error != null)
            throw Error;
    }
    async Task Publish(DotNetPublishSettings Settings, string Title)
    {
        if (!Settings.IsEnabled)
            throw new Exception(Title + " is disabled.");

        SaveToProject(fProject);
        AppHost.SaveProject(fProject);
        EnsureFolderCanBeCreated(GetPublishRootFolder(), "Publish root folder");

        ProcessRunner Runner = new();
        ProcessRunResult Result = await Runner.RunAsync("dotnet", CreateDotNetPublishArguments(Settings), GetWorkingDirectory());
        if (Result.ExitCode != 0)
            throw new Exception(Title + " failed. Exit code: " + Result.ExitCode.ToString(CultureInfo.InvariantCulture));

        if (ReferenceEquals(Settings, fProject.WindowsPublish))
            CreateWindowsZip();
        else if (ReferenceEquals(Settings, fProject.LinuxPublish))
            CreateLinuxTarGz();
    }
    void GenerateDebScript()
    {
        SaveToProject(fProject);
        AppHost.SaveProject(fProject);
        string FilePath = DebScriptGenerator.Generate(fProject);
        AppHost.Log("Debian script generated: " + FilePath);
    }
    void GenerateInnoScript()
    {
        SaveToProject(fProject);
        AppHost.SaveProject(fProject);
        string FilePath = InnoSetupScriptGenerator.Generate(fProject);
        AppHost.Log("Inno Setup script generated: " + FilePath);
    }
    async Task BuildDeb()
    {
        SaveToProject(fProject);
        AppHost.SaveProject(fProject);

        string ScriptFilePath = GetDebScriptFilePath();
        if (!File.Exists(ScriptFilePath))
            ScriptFilePath = DebScriptGenerator.Generate(fProject);

        ProcessRunner Runner = new();
        ProcessRunResult Result = await Runner.RunAsync("bash", QuoteArg(ScriptFilePath), GetPublishRootFolder());
        if (Result.ExitCode != 0)
            throw new Exception("Build Deb failed. Exit code: " + Result.ExitCode.ToString(CultureInfo.InvariantCulture));
    }
    async Task BuildInno()
    {
        if (!OperatingSystem.IsWindows())
            throw new Exception("Inno Setup compilation can run only on Windows. Generate the .iss file here, then compile it in a Windows VM or on a Windows machine.");

        SaveToProject(fProject);
        AppHost.SaveProject(fProject);

        string CompilerPath = AppHost.Settings.ResolveInnoSetupCompilerPath();
        if (string.IsNullOrWhiteSpace(CompilerPath))
            throw new Exception("Inno Setup compiler was not found. Set the ISCC.exe path in Application Settings or install Inno Setup.");

        string ScriptFilePath = GetInnoScriptFilePath();
        if (!File.Exists(ScriptFilePath))
            ScriptFilePath = InnoSetupScriptGenerator.Generate(fProject);

        ProcessRunner Runner = new();
        ProcessRunResult Result = await Runner.RunAsync(CompilerPath, QuotePathArg(ScriptFilePath), GetPublishRootFolder());
        if (Result.ExitCode != 0)
            throw new Exception("Build Inno failed. Exit code: " + Result.ExitCode.ToString(CultureInfo.InvariantCulture));
    }
    void OpenOutputFolder()
    {
        string Folder = GetInstallerOutputFolder();
        if (string.IsNullOrWhiteSpace(Folder))
            Folder = GetPublishRootFolder();
        EnsureFolderCanBeCreated(Folder, "Output folder");
        Sys.OpenFileExplorer(Folder);
    }
    async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        try
        {
            if (Sender == btnSave)
                Save();
            else if (Sender == btnPublishLinux)
                await RunBusy("Publishing Linux output. Please wait...", async () => await Publish(fProject.LinuxPublish, "Publish Linux"));
            else if (Sender == btnPublishWindows)
                await RunBusy("Publishing Windows output. Please wait...", async () => await Publish(fProject.WindowsPublish, "Publish Windows"));
            else if (Sender == btnGenerateDebScript)
                GenerateDebScript();
            else if (Sender == btnGenerateInnoScript)
                GenerateInnoScript();
            else if (Sender == btnBuildDeb)
                await RunBusy("Building Debian package. Please wait...", BuildDeb);
            else if (Sender == btnBuildInno)
                await RunBusy("Building Inno Setup installer. Please wait...", BuildInno);
            else if (Sender == btnOpenOutputFolder)
                OpenOutputFolder();
            else if (Sender == btnClose)
                CloseForm();
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, this);
        }
    }
    void Save()
    {
        SaveToProject(fProject);
        AppHost.SaveProject(fProject);
        TitleText = fProject.Name;
    }
    // ● protected
    /// <summary>
    /// Processes keyboard shortcuts.
    /// </summary>
    protected override bool ProcessKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            Save();
            return true;
        }

        return base.ProcessKeyDown(e);
    }
    /// <summary>
    /// Sets up the form context.
    /// </summary>
    protected override void Setup()
    {
        fProject = Context.Tag as PublisherProjectSettings;
    }
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        if (fProject == null)
            throw new Exception("No project selected.");
        TitleText = fProject.Name;
        CreateToolBar();
        LoadProject(fProject);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectEditorForm"/> class.
    /// </summary>
    public ProjectEditorForm()
    {
        InitializeComponent();
    }
}
