// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Startup operations for PackForge.
/// </summary>
static internal partial class AppHost
{
    // ● private
    static void InitializeConfigs()
    {
        SysConfig.ApplicationMode = ApplicationMode.Desktop;
        SysConfig.MainAssembly = typeof(AppHost).Assembly;
        SysConfig.AppName = "PackForge";
        StartupWindow.SetApplicationTitle(SysConfig.AppName);
    }
    static void InitializeLibraries()
    {
        TypeStore.RegisterLoadedAssemblies();
    }
    static void LoadSettings()
    {
        Settings.Load();
    }
    static void LoadProjects()
    {
        Projects.Load();
    }

    // ● public
    /// <summary>
    /// Starts PackForge and replaces the startup window with the main window.
    /// </summary>
    static public async Task Start(IClassicDesktopStyleApplicationLifetime AvaloniaDesktop)
    {
        bool Flag = true;
        AppHost.AvaloniaDesktop = AvaloniaDesktop;
        Ui.MainWindow = StartupWindow;

        try
        {
            StartupWindow.SetMessage("Checking startup state...");
            InitializeConfigs();

            StartupWindow.SetMessage("Loading application libraries...");
            InitializeLibraries();

            StartupWindow.SetMessage("Loading application settings...");
            LoadSettings();

            StartupWindow.SetMessage("Loading projects...");
            LoadProjects();

            StartupWindow.SetMessage("Opening main window...");
            MainWindow = new MainWindow();
            Ui.MainWindow = MainWindow;
            AvaloniaDesktop.MainWindow = MainWindow;
            MainWindow.Show();
            StartupWindow.Close();
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, Ui.MainWindow);
            Flag = false;
        }

        if (!Flag)
        {
            Ui.MainWindow.Close();
            return;
        }
    }
    /// <summary>
    /// Initializes application UI handlers.
    /// </summary>
    static public void InitializeUi(AppFormPagerHandler SideBarHandler, AppFormPagerHandler ContentHandler)
    {
        if (AppHost.SideBarHandler == null)
        {
            AppHost.SideBarHandler = SideBarHandler;
            AppHost.ContentHandler = ContentHandler;
            ShowSideBarPages();
        }
    }
}
