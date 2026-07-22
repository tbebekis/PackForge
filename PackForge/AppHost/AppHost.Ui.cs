// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace PackForge;

/// <summary>
/// PackForge top-level UI navigation operations.
/// </summary>
static internal partial class AppHost
{
    // ● public
    /// <summary>
    /// Shows the permanent left sidebar pages.
    /// </summary>
    static public void ShowSideBarPages()
    {
        ShowProjects();
        SideBarHandler.Pager.SelectedIndex = 0;
    }
    /// <summary>
    /// Shows the project list in the left sidebar.
    /// </summary>
    static public AppForm ShowProjects()
    {
        FormContext Context = FormContext.Create("Projects", typeof(ProjectListForm).FullName, FormDisplayMode.TabItem, MainWindow);
        Context.Title = "Projects";
        return SideBarHandler.ShowAppForm(Context);
    }
    /// <summary>
    /// Returns the project editor form id for a project name.
    /// </summary>
    static public string GetProjectEditorFormId(string ProjectName) => "ProjectEditor." + (ProjectName ?? string.Empty);
    /// <summary>
    /// Shows the project editor in the content area.
    /// </summary>
    static public AppForm ShowProjectEditor(PublisherProjectSettings Project)
    {
        if (Project == null)
            throw new Exception("No project selected.");
        SetCurrentProject(Project);

        FormContext Context = FormContext.Create(GetProjectEditorFormId(Project.Name), typeof(ProjectEditorForm).FullName, FormDisplayMode.TabItem, MainWindow, Project);
        Context.Title = Project.Name;
        return ContentHandler.ShowAppForm(Context);
    }
}
