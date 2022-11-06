using System.Linq;
using Luna.IDE.Outputing;
using Luna.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.ProjectExploration;

public interface ISelectedProject
{
    IProject? Project { get; }
}

public interface IProjectLoader : ISelectedProject
{
    void OpenProject(string path);
    event EventHandler? ProjectOpened;
}

public class ProjectLoader : IProjectLoader
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly IOutputArea _outputArea;
    private readonly IFileSystem _fileSystem;

    public IProject? Project { get; private set; }

    public event EventHandler? ProjectOpened;

    public ProjectLoader(ICodeModelUpdater codeModelUpdater, IOutputArea outputArea, IFileSystem fileSystem)
    {
        _codeModelUpdater = codeModelUpdater;
        _outputArea = outputArea;
        _fileSystem = fileSystem;
    }

    public void OpenProject(string path)
    {
        Project = Luna.ProjectModel.Project.Open(path, _fileSystem);
        _codeModelUpdater.SetCodeFiles(Project.Root.AllChildren.OfType<CodeFileProjectItem>());
        _outputArea.Clear();
        ProjectOpened?.Invoke(this, EventArgs.Empty);
    }
}
