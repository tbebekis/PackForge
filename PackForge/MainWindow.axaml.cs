// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// The main PackForge desktop window.
/// </summary>
public partial class MainWindow : Window
{
    // ● private fields
    bool fIsWindowInitialized;
    AppFormPagerHandler fSideBarHandler;
    AppFormPagerHandler fContentHandler;
    Tripous.Desktop.ToolBar ToolBar;
    Button btnAppFolder;
    Button btnSettings;
    Button btnToggleLog;
    Button btnClearLog;
    Button btnAbout;
    Button btnExit;

    // ● private
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);
        fSideBarHandler = new AppFormPagerHandler(pagerSideBar);
        fSideBarHandler.IsTabHeaderContextMenuVisible = false;
        fContentHandler = new AppFormPagerHandler(pagerContent);
        fContentHandler.CanUserReorderTabs = true;
        fContentHandler.IsTabHeaderContextMenuVisible = true;
        CreateToolBar();
        SetLogVisible(AppHost.Settings.IsLogVisible);
        AppHost.InitializeUi(fSideBarHandler, fContentHandler);
        UpdateStatusBar();
    }
    void CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;

            btnAppFolder = ToolBar.AddButton("folder.png", "Show App Folder", AnyClick);
            btnSettings = ToolBar.AddButton("setting_tools.png", "Application Settings", AnyClick);
            ToolBar.AddSeparator();
            btnToggleLog = ToolBar.AddButton("error_log.png", "Toggle Log", AnyClick);
            btnClearLog = ToolBar.AddButton("bin.png", "Clear Log", AnyClick);
            ToolBar.AddSeparator();
            btnAbout = ToolBar.AddButton("information.png", "About", AnyClick);
            btnExit = ToolBar.AddButton("door_out.png", "Exit", AnyClick);
        }
    }
    string GetAppVersion()
    {
        Assembly Assembly = typeof(MainWindow).Assembly;
        string Result = Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrWhiteSpace(Result))
        {
            int Index = Result.IndexOf('+');
            if (Index > 0)
                Result = Result.Substring(0, Index);
        }
        if (string.IsNullOrWhiteSpace(Result))
            Result = Assembly.GetName().Version?.ToString();
        return string.IsNullOrWhiteSpace(Result) ? "1.0.0" : Result;
    }
    void UpdateStatusBar()
    {
        lblStatus.Text = $"{SysConfig.AppName} v{GetAppVersion()}";
        lblMessage.Text = "Ready";
    }
    void ShowApplicationFolder()
    {
        Directory.CreateDirectory(SysConfig.AppFolderPath);
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
    }
    void SetLogVisible(bool Value)
    {
        Splitter2.IsVisible = Value;
        edtLog.IsVisible = Value;
    }
    void ToggleLog()
    {
        bool IsVisible = !edtLog.IsVisible;
        SetLogVisible(IsVisible);
        AppHost.Settings.IsLogVisible = IsVisible;
        AppHost.Settings.Save();
    }
    async Task ShowSettings()
    {
        AppSettingsDialogData Data = await AppSettingsDialog.ShowModal(AppHost.Settings, this);
        if (!Data.Result)
            return;

        Data.ApplyTo(AppHost.Settings);
        AppHost.Settings.Save();
        AppHost.Log("Application settings saved.");
        await MessageBox.Info("Application settings saved.", this);
    }

    // ● event handlers
    async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        try
        {
            if (Sender == btnExit)
                Close();
            else if (Sender == btnAppFolder)
                ShowApplicationFolder();
            else if (Sender == btnSettings)
                await ShowSettings();
            else if (Sender == btnToggleLog)
                ToggleLog();
            else if (Sender == btnClearLog)
                LogBox.Clear();
            else if (Sender == btnAbout)
                await AboutDialog.ShowModal(this);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, this);
        }
    }

    // ● overrides
    /// <summary>
    /// Handles the first window open event.
    /// </summary>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (fIsWindowInitialized)
            return;

        WindowInitialize();
        fIsWindowInitialized = true;
        LogBox.AppendLine("Application started.");
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }
}
