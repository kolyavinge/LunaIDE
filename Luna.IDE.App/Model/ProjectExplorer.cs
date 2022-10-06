using System.Linq;
using Luna.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.App.Model;

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
    private readonly IOutputArea _outputArea;
    private readonly IFileSystem _fileSystem;

    public Project? Project { get; private set; }

    public DirectoryTreeItem? ProjectTreeRoot { get; private set; }

    public event EventHandler? ProjectOpened;

    public ProjectExplorer(ICodeModelUpdater codeModelUpdater, IOutputArea outputArea, IFileSystem fileSystem)
    {
        _codeModelUpdater = codeModelUpdater;
        _outputArea = outputArea;
        _fileSystem = fileSystem;
    }

    public void OpenProject(string path)
    {
        Project = Project.Open(path, _fileSystem);
        _codeModelUpdater.SetCodeFiles(Project.Root.AllChildren.OfType<CodeFileProjectItem>());
        _outputArea.Clear();
        ProjectTreeRoot = new DirectoryTreeItem(null, Project.Root);
        ProjectOpened?.Invoke(this, EventArgs.Empty);
    }
}
