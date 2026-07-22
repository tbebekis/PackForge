// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// Project list hosted in the left sidebar.
/// </summary>
public partial class ProjectListForm : AppForm
{
    // ● private fields
    Tripous.Desktop.ToolBar ToolBar;
    Button btnAdd;
    Button btnEdit;
    Button btnDelete;

    // ● private
    void CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;
            btnAdd = ToolBar.AddButton("table_add.png", "Add", AnyClick);
            btnEdit = ToolBar.AddButton("table_edit.png", "Edit", AnyClick);
            btnDelete = ToolBar.AddButton("table_delete.png", "Delete", AnyClick);
        }
    }
    void RefreshProjects()
    {
        lboProjects.ItemsSource = AppHost.Projects.Items;
        if (AppHost.CurrentProject != null)
            lboProjects.SelectedItem = AppHost.CurrentProject;
    }
    void ProjectsChanged(object Sender, EventArgs Args)
    {
        RefreshProjects();
    }
    void CurrentProjectChanged(object Sender, EventArgs Args)
    {
        if (AppHost.CurrentProject != null && !ReferenceEquals(lboProjects.SelectedItem, AppHost.CurrentProject))
            lboProjects.SelectedItem = AppHost.CurrentProject;
    }
    void ProjectSelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (lboProjects.SelectedItem is PublisherProjectSettings Project)
            AppHost.SetCurrentProject(Project);
    }
    PublisherProjectSettings SelectedProject()
    {
        return lboProjects.SelectedItem as PublisherProjectSettings;
    }
    async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        try
        {
            if (Sender == btnAdd)
                await AddProject();
            else if (Sender == btnEdit)
                EditProject();
            else if (Sender == btnDelete)
                AppHost.DeleteCurrentProject();
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, this);
        }
    }
    async Task AddProject()
    {
        InputBoxData Data = await InputBox.ShowModal("Project name", string.Empty, this);
        if (!Data.Result)
            return;
        AppHost.NewProject(Data.Value);
    }
    void EditProject()
    {
        PublisherProjectSettings Project = SelectedProject();
        if (Project == null)
            throw new Exception("No project selected.");
        AppHost.ShowProjectEditor(Project);
    }
    async void ProjectsDoubleTapped(object Sender, TappedEventArgs Args)
    {
        try
        {
            EditProject();
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, this);
        }
    }

    // ● protected
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        ClosableByUser = false;
        TitleText = "Projects";
        CreateToolBar();
        lboProjects.SelectionChanged += ProjectSelectionChanged;
        lboProjects.DoubleTapped += ProjectsDoubleTapped;
        AppHost.ProjectsChanged += ProjectsChanged;
        AppHost.CurrentProjectChanged += CurrentProjectChanged;
        RefreshProjects();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectListForm"/> class.
    /// </summary>
    public ProjectListForm()
    {
        InitializeComponent();
    }
}
