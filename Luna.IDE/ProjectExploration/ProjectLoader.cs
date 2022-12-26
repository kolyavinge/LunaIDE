using System.Linq;
using Luna.IDE.Configuration;
using Luna.IDE.Outputing;
using Luna.IDE.WindowsManagement;
using Luna.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.ProjectExploration;

public interface ISelectedProject
{
    IProject? Project { get; }
}

public interface IProjectLoader : ISelectedProject
{
    void OpenProject(string projectFullPath);
    void CloseCurrentProject();
    event EventHandler? ProjectOpened;
}

public class ProjectLoader : IProjectLoader
{
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly IOutputArea _outputArea;
    private readonly ILastOpenedProjectFiles _lastOpenedProjectFiles;
    private readonly IFileSystem _fileSystem;

    public IProject? Project { get; private set; }

    public event EventHandler? ProjectOpened;

    public ProjectLoader(
        IEnvironmentWindowsManager windowsManager,
        ICodeModelUpdater codeModelUpdater,
        IOutputArea outputArea,
        ILastOpenedProjectFiles lastOpenedProjectFiles,
        IFileSystem fileSystem)
    {
        _windowsManager = windowsManager;
        _codeModelUpdater = codeModelUpdater;
        _outputArea = outputArea;
        _lastOpenedProjectFiles = lastOpenedProjectFiles;
        _fileSystem = fileSystem;
    }

    public void OpenProject(string projectFullPath)
    {
        CloseCurrentProject();
        Project = ProjectModel.Project.Open(projectFullPath, _fileSystem);
        _codeModelUpdater.SetCodeFiles(Project.Root.AllChildren.OfType<CodeFileProjectItem>());
        _lastOpenedProjectFiles.RestoreLastOpenedFiles(Project);
        _outputArea.Clear();
        ProjectOpened?.Invoke(this, EventArgs.Empty);
    }

    public void CloseCurrentProject()
    {
        if (Project != null)
        {
            _lastOpenedProjectFiles.SaveOpenedFiles(Project);
            _windowsManager.CloseAllWindows();
        }
    }
}
