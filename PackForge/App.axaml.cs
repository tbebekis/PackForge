// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// PackForge desktop application object.
/// </summary>
public partial class App : Application
{
    // ● public
    /// <summary>
    /// Initializes application resources.
    /// </summary>
    public override void Initialize()
    {
        DesktopExceptionHandler.Initialize();
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Completes desktop application initialization.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = AppHost.StartupWindow;
            desktop.MainWindow.Opened += async (Sender, Args) =>
            {
                await Dispatcher.UIThread.InvokeAsync(async () => await AppHost.Start(desktop), DispatcherPriority.Background);
            };
        }
    }
}
