using Luna.ProjectModel;

namespace Luna.IDE.ProjectExploration;

public interface IProjectExplorer
{
    Project? Project { get; }
    DirectoryTreeItem? ProjectTreeRoot { get; }
}

public class ProjectExplorer : IProjectExplorer
{
    private readonly IProjectLoader _projectLoader;

    public Project? Project => _projectLoader.Project;

    public DirectoryTreeItem? ProjectTreeRoot { get; private set; }

    public ProjectExplorer(IProjectLoader projectLoader)
    {
        _projectLoader = projectLoader;
        _projectLoader.ProjectOpened += OnProjectOpened;
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        ProjectTreeRoot = new DirectoryTreeItem(null, _projectLoader.Project!.Root);
    }
}
