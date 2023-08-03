using Luna.IDE.Common;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.ProjectExploration;

public interface IProjectExplorer
{
    IProject? Project { get; }
    DirectoryTreeItem? ProjectTreeRoot { get; }
}

public class ProjectExplorer : NotificationObject, IProjectExplorer, IEnvironmentWindowModel
{
    private readonly IProjectLoader _projectLoader;
    private DirectoryTreeItem? _projectTreeRoot;

    public string Header => "Project Explorer";

    public IProject? Project => _projectLoader.Project;

    public DirectoryTreeItem? ProjectTreeRoot
    {
        get => _projectTreeRoot;
        private set
        {
            _projectTreeRoot = value;
            RaisePropertyChanged(() => ProjectTreeRoot!);
        }
    }

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
