// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Dialog used to edit PackForge application settings.
/// </summary>
public partial class AppSettingsDialog : DialogWindow
{
    // ● private fields
    AppSettingsDialogData BoxData;

    // ● private
    string TextOf(TextBox Edit) => Edit.Text?.Trim() ?? string.Empty;
    string TextOf(ComboBox Combo)
    {
        if (!string.IsNullOrWhiteSpace(Combo.Text))
            return Combo.Text.Trim();
        if (Combo.SelectedItem is ComboBoxItem Item)
            return Convert.ToString(Item.Content, CultureInfo.CurrentCulture)?.Trim() ?? string.Empty;
        return Convert.ToString(Combo.SelectedItem, CultureInfo.CurrentCulture)?.Trim() ?? string.Empty;
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
            if (ItemObject is ComboBoxItem Item && Text.IsSameText(Convert.ToString(Item.Content, CultureInfo.CurrentCulture)))
            {
                Combo.SelectedItem = Item;
                Combo.Text = Text;
                return;
            }
        }

        Combo.Text = Text;
    }
    void UpdateInnoStatus()
    {
        string CompilerPath = InnoSetupLocator.NormalizeCompilerPath(TextOf(edtInnoSetupCompilerPath));
        if (string.IsNullOrWhiteSpace(CompilerPath))
            CompilerPath = InnoSetupLocator.FindCompilerPath();

        lblInnoStatus.Text = string.IsNullOrWhiteSpace(CompilerPath)
            ? "Inno Setup compiler was not found. Leave the path empty when generating scripts only."
            : "Resolved compiler: " + CompilerPath;
    }
    async Task DetectInno()
    {
        string CompilerPath = InnoSetupLocator.FindCompilerPath();
        if (!string.IsNullOrWhiteSpace(CompilerPath))
        {
            edtInnoSetupCompilerPath.Text = CompilerPath;
            UpdateInnoStatus();
            return;
        }

        UpdateInnoStatus();
        await MessageBox.Info("Inno Setup compiler was not found in the standard Program Files folders.", this);
    }

    // ● event handlers
    async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        if (Sender == btnCancel)
            ModalResult = ModalResult.Cancel;
        else if (Sender == btnDetectInno)
            await DetectInno();
        else if (Sender == btnOK)
        {
            try
            {
                await ControlsToItem();
            }
            catch (Exception e)
            {
                await MessageBox.Error(e.Message, this);
            }
        }
    }

    // ● protected
    /// <summary>
    /// Initializes the window.
    /// </summary>
    protected override async Task WindowInitialize()
    {
        BoxData = InputData as AppSettingsDialogData;
        ResultData = BoxData;

        SetText(edtInnoSetupCompilerPath, BoxData.InnoSetupCompilerPath);
        SetText(edtDefaultMaintainer, BoxData.DefaultMaintainer);
        SetText(edtDefaultHomepage, BoxData.DefaultHomepage);
        SetText(cboDefaultConfiguration, BoxData.DefaultConfiguration);
        SetText(cboDefaultLinuxRuntime, BoxData.DefaultLinuxRuntimeIdentifier);
        SetText(cboDefaultWindowsRuntime, BoxData.DefaultWindowsRuntimeIdentifier);
        chkDefaultSelfContained.IsChecked = BoxData.DefaultSelfContained;
        chkDefaultPublishSingleFile.IsChecked = BoxData.DefaultPublishSingleFile;
        chkDefaultPublishTrimmed.IsChecked = BoxData.DefaultPublishTrimmed;
        SetText(cboDefaultDebArchitecture, BoxData.DefaultDebArchitecture);
        SetText(cboDefaultDebSection, BoxData.DefaultDebSection);
        SetText(cboDefaultDebPriority, BoxData.DefaultDebPriority);
        SetText(edtDefaultDesktopCategories, BoxData.DefaultDesktopCategories);
        SetText(edtDefaultDesktopKeywords, BoxData.DefaultDesktopKeywords);
        SetText(cboDefaultInnoCompression, BoxData.DefaultInnoCompression);
        chkDefaultInnoSolidCompression.IsChecked = BoxData.DefaultInnoSolidCompression;
        SetText(cboDefaultInnoPrivileges, BoxData.DefaultInnoPrivilegesRequired);
        SetText(cboDefaultInnoInstallScope, BoxData.DefaultInnoInstallScope);
        UpdateInnoStatus();
        edtInnoSetupCompilerPath.Focus();

        await Task.CompletedTask;
    }
    /// <summary>
    /// Saves dialog control values to the dialog data.
    /// </summary>
    protected override async Task ControlsToItem()
    {
        string InnoPath = TextOf(edtInnoSetupCompilerPath);
        if (!string.IsNullOrWhiteSpace(InnoPath) && string.IsNullOrWhiteSpace(InnoSetupLocator.NormalizeCompilerPath(InnoPath)))
            throw new Exception("Inno Setup compiler path is invalid. Select ISCC.exe or leave the field empty for auto-detection.");

        BoxData.InnoSetupCompilerPath = InnoPath;
        BoxData.DefaultMaintainer = TextOf(edtDefaultMaintainer);
        BoxData.DefaultHomepage = TextOf(edtDefaultHomepage);
        BoxData.DefaultConfiguration = TextOf(cboDefaultConfiguration);
        BoxData.DefaultLinuxRuntimeIdentifier = TextOf(cboDefaultLinuxRuntime);
        BoxData.DefaultWindowsRuntimeIdentifier = TextOf(cboDefaultWindowsRuntime);
        BoxData.DefaultSelfContained = chkDefaultSelfContained.IsChecked == true;
        BoxData.DefaultPublishSingleFile = chkDefaultPublishSingleFile.IsChecked == true;
        BoxData.DefaultPublishTrimmed = chkDefaultPublishTrimmed.IsChecked == true;
        BoxData.DefaultDebArchitecture = TextOf(cboDefaultDebArchitecture);
        BoxData.DefaultDebSection = TextOf(cboDefaultDebSection);
        BoxData.DefaultDebPriority = TextOf(cboDefaultDebPriority);
        BoxData.DefaultDesktopCategories = TextOf(edtDefaultDesktopCategories);
        BoxData.DefaultDesktopKeywords = TextOf(edtDefaultDesktopKeywords);
        BoxData.DefaultInnoCompression = TextOf(cboDefaultInnoCompression);
        BoxData.DefaultInnoSolidCompression = chkDefaultInnoSolidCompression.IsChecked == true;
        BoxData.DefaultInnoPrivilegesRequired = TextOf(cboDefaultInnoPrivileges);
        BoxData.DefaultInnoInstallScope = TextOf(cboDefaultInnoInstallScope);
        ModalResult = ModalResult.Ok;
        await Task.CompletedTask;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsDialog"/> class.
    /// </summary>
    public AppSettingsDialog()
    {
        InitializeComponent();
    }

    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    static public async Task<AppSettingsDialogData> ShowModal(AppSettings Settings, Control Caller = null)
    {
        AppSettingsDialogData BoxData = new(Settings);
        DialogInfo Info = await ShowModal<AppSettingsDialog>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

/// <summary>
/// Contains application settings dialog data.
/// </summary>
public class AppSettingsDialogData
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsDialogData"/> class.
    /// </summary>
    public AppSettingsDialogData()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsDialogData"/> class.
    /// </summary>
    public AppSettingsDialogData(AppSettings Settings)
    {
        InnoSetupCompilerPath = Settings.InnoSetupCompilerPath;
        DefaultMaintainer = Settings.DefaultMaintainer;
        DefaultHomepage = Settings.DefaultHomepage;
        DefaultConfiguration = Settings.DefaultConfiguration;
        DefaultLinuxRuntimeIdentifier = Settings.DefaultLinuxRuntimeIdentifier;
        DefaultWindowsRuntimeIdentifier = Settings.DefaultWindowsRuntimeIdentifier;
        DefaultSelfContained = Settings.DefaultSelfContained;
        DefaultPublishSingleFile = Settings.DefaultPublishSingleFile;
        DefaultPublishTrimmed = Settings.DefaultPublishTrimmed;
        DefaultDebArchitecture = Settings.DefaultDebArchitecture;
        DefaultDebSection = Settings.DefaultDebSection;
        DefaultDebPriority = Settings.DefaultDebPriority;
        DefaultDesktopCategories = Settings.DefaultDesktopCategories;
        DefaultDesktopKeywords = Settings.DefaultDesktopKeywords;
        DefaultInnoCompression = Settings.DefaultInnoCompression;
        DefaultInnoSolidCompression = Settings.DefaultInnoSolidCompression;
        DefaultInnoPrivilegesRequired = Settings.DefaultInnoPrivilegesRequired;
        DefaultInnoInstallScope = Settings.DefaultInnoInstallScope;
    }

    // ● public
    /// <summary>
    /// Copies dialog values to application settings.
    /// </summary>
    public void ApplyTo(AppSettings Settings)
    {
        Settings.InnoSetupCompilerPath = InnoSetupCompilerPath;
        Settings.DefaultMaintainer = DefaultMaintainer;
        Settings.DefaultHomepage = DefaultHomepage;
        Settings.DefaultConfiguration = DefaultConfiguration;
        Settings.DefaultLinuxRuntimeIdentifier = DefaultLinuxRuntimeIdentifier;
        Settings.DefaultWindowsRuntimeIdentifier = DefaultWindowsRuntimeIdentifier;
        Settings.DefaultSelfContained = DefaultSelfContained;
        Settings.DefaultPublishSingleFile = DefaultPublishSingleFile;
        Settings.DefaultPublishTrimmed = DefaultPublishTrimmed;
        Settings.DefaultDebArchitecture = DefaultDebArchitecture;
        Settings.DefaultDebSection = DefaultDebSection;
        Settings.DefaultDebPriority = DefaultDebPriority;
        Settings.DefaultDesktopCategories = DefaultDesktopCategories;
        Settings.DefaultDesktopKeywords = DefaultDesktopKeywords;
        Settings.DefaultInnoCompression = DefaultInnoCompression;
        Settings.DefaultInnoSolidCompression = DefaultInnoSolidCompression;
        Settings.DefaultInnoPrivilegesRequired = DefaultInnoPrivilegesRequired;
        Settings.DefaultInnoInstallScope = DefaultInnoInstallScope;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the Inno Setup compiler executable path.
    /// </summary>
    public string InnoSetupCompilerPath { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default project maintainer.
    /// </summary>
    public string DefaultMaintainer { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default project homepage.
    /// </summary>
    public string DefaultHomepage { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default publish configuration.
    /// </summary>
    public string DefaultConfiguration { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default Linux runtime identifier.
    /// </summary>
    public string DefaultLinuxRuntimeIdentifier { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default Windows runtime identifier.
    /// </summary>
    public string DefaultWindowsRuntimeIdentifier { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether new publish profiles are self-contained by default.
    /// </summary>
    public bool DefaultSelfContained { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether new publish profiles use single-file publish by default.
    /// </summary>
    public bool DefaultPublishSingleFile { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether new publish profiles use trimming by default.
    /// </summary>
    public bool DefaultPublishTrimmed { get; set; }
    /// <summary>
    /// Gets or sets the default Debian architecture.
    /// </summary>
    public string DefaultDebArchitecture { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default Debian section.
    /// </summary>
    public string DefaultDebSection { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default Debian priority.
    /// </summary>
    public string DefaultDebPriority { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default desktop categories.
    /// </summary>
    public string DefaultDesktopCategories { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default desktop keywords.
    /// </summary>
    public string DefaultDesktopKeywords { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default Inno Setup compression.
    /// </summary>
    public string DefaultInnoCompression { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether Inno Setup solid compression is enabled by default.
    /// </summary>
    public bool DefaultInnoSolidCompression { get; set; }
    /// <summary>
    /// Gets or sets the default Inno Setup privileges requirement.
    /// </summary>
    public string DefaultInnoPrivilegesRequired { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the default Windows install scope.
    /// </summary>
    public string DefaultInnoInstallScope { get; set; } = string.Empty;
    /// <summary>
    /// Gets the dialog information.
    /// </summary>
    public DialogInfo Info { get; internal set; }
    /// <summary>
    /// Gets a value indicating whether the dialog result is OK.
    /// </summary>
    public bool Result => Info != null && Info.Result;
}
