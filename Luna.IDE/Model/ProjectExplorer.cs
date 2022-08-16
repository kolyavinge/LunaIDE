using System.Linq;
using Luna.IDE.Infrastructure;
using Luna.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface IProjectExplorer
{
    Project? Project { get; }
    DirectoryTreeItem? ProjectTreeRoot { get; }
    void OpenProject(string path);
    event EventHandler ProjectOpened;
}

public class ProjectExplorer : IProjectExplorer
{
    private readonly ICodeModelUpdater _codeModelUpdater;

    [Inject]
    public IFileSystem? FileSystem { get; set; }

    public Project? Project { get; private set; }

    public DirectoryTreeItem? ProjectTreeRoot { get; private set; }

    public event EventHandler? ProjectOpened;

    public ProjectExplorer(ICodeModelUpdater codeModelUpdater)
    {
        _codeModelUpdater = codeModelUpdater;
    }

    public void OpenProject(string path)
    {
        Project = Project.Open(path, FileSystem!);
        _codeModelUpdater.SetCodeFiles(Project.Root.AllChildren.OfType<CodeFileProjectItem>());
        ProjectTreeRoot = new DirectoryTreeItem(null, Project.Root);
        ProjectOpened?.Invoke(this, EventArgs.Empty);
    }
}
