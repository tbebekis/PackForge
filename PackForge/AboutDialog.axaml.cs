// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Displays PackForge product information.
/// </summary>
public partial class AboutDialog : DialogWindow
{
    // ● private
    string GetAssemblyVersion(Assembly Assembly)
    {
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
    string GetRuntimeText()
    {
        string Text = RuntimeInformation.FrameworkDescription;
        if (Text.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase))
            return Text;
        return ".NET " + Environment.Version.Major.ToString(CultureInfo.InvariantCulture);
    }
    string CreateSystemInfoText()
    {
        StringBuilder Builder = new();
        Builder.AppendLine("PackForge system info");
        Builder.AppendLine("PackForge: " + lblVersion.Text);
        Builder.AppendLine("Runtime: " + lblRuntime.Text);
        Builder.AppendLine("Project kind: " + lblProjectKind.Text);
        Builder.AppendLine("Tripous: " + GetAssemblyVersion(typeof(Sys).Assembly));
        Builder.AppendLine("OS: " + RuntimeInformation.OSDescription);
        Builder.AppendLine("OS architecture: " + RuntimeInformation.OSArchitecture);
        Builder.AppendLine("Process architecture: " + RuntimeInformation.ProcessArchitecture);
        Builder.AppendLine("App folder: " + SysConfig.AppFolderPath);
        Builder.AppendLine("Data folder: " + SysConfig.AppDataFolderPath);
        Builder.AppendLine("Projects folder: " + AppHost.Projects.FolderPath);
        Builder.AppendLine("Settings file: " + AppHost.Settings.SettingsFilePath);
        Builder.AppendLine("Inno compiler: " + AppHost.Settings.ResolveInnoSetupCompilerPath());
        return Builder.ToString();
    }
    async Task CopySystemInfo()
    {
        TopLevel TopLevel = TopLevel.GetTopLevel(this);
        if (TopLevel?.Clipboard == null)
            return;

        await TopLevel.Clipboard.SetTextAsync(CreateSystemInfoText());
        await MessageBox.Info("System information copied to clipboard.", this);
    }

    // ● event handlers
    async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        if (Sender == btnClose)
            ModalResult = ModalResult.Ok;
        else if (Sender == btnCopySystemInfo)
            await CopySystemInfo();
    }

    // ● protected
    /// <summary>
    /// Initializes the window.
    /// </summary>
    protected override async Task WindowInitialize()
    {
        Title = "About PackForge";
        lblHeader.Text = Title;
        lblVersion.Text = GetAssemblyVersion(typeof(AboutDialog).Assembly);
        lblRuntime.Text = GetRuntimeText();
        lblProjectKind.Text = ".NET";
        await Task.CompletedTask;
    }
    /// <summary>
    /// Handles keyboard input.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            ModalResult = ModalResult.Ok;
            return;
        }

        base.OnKeyDown(e);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutDialog"/> class.
    /// </summary>
    public AboutDialog()
    {
        InitializeComponent();
    }

    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    static public async Task ShowModal(Control Caller = null)
    {
        await ShowModal<AboutDialog>(null, Caller);
    }
}
