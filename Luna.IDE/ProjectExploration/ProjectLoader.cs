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
    event EventHandler<ProjectOpenedEventArgs>? ProjectOpened;
}

public class ProjectOpenedEventArgs : EventArgs
{
    public IProject Project { get; }

    public ProjectOpenedEventArgs(IProject project)
    {
        Project = project;
    }
}

internal class ProjectLoader : IProjectLoader
{
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly IOutputArea _outputArea;
    private readonly ILastOpenedProjectFiles _lastOpenedProjectFiles;
    private readonly IFileSystem _fileSystem;

    public IProject? Project { get; private set; }

    public event EventHandler<ProjectOpenedEventArgs>? ProjectOpened;

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
        ProjectOpened?.Invoke(this, new(Project));
    }

    public void CloseCurrentProject()
    {
        if (Project is not null)
        {
            _lastOpenedProjectFiles.SaveOpenedFiles(Project);
            _windowsManager.CloseAllWindows();
        }
    }
}
