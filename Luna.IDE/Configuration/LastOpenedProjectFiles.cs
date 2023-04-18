using System.Linq;
using Luna.IDE.CodeEditing;
using Luna.IDE.ProjectExploration;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.Configuration;

internal interface ILastOpenedProjectFiles
{
    void RestoreLastOpenedFiles(IProject project);
    void SaveOpenedFiles(IProject project);
}

internal class LastOpenedProjectFiles : ILastOpenedProjectFiles
{
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IProjectItemOpenCommand _projectItemOpenCommand;
    private readonly IConfigStorage _configStorage;

    public LastOpenedProjectFiles(
        IEnvironmentWindowsManager windowsManager,
        IProjectItemOpenCommand projectItemOpenCommand,
        IConfigStorage configStorage)
    {
        _windowsManager = windowsManager;
        _projectItemOpenCommand = projectItemOpenCommand;
        _configStorage = configStorage;
    }

    public void RestoreLastOpenedFiles(IProject project)
    {
        var projectFiles = _configStorage.GetById<LastOpenedProjectFilesPoco>(project.Root.FullPath);
        if (projectFiles == null) return;
        var projectItems = projectFiles.FilesRelativePathes.Select(project.FindItemByPath).ToList();
        _projectItemOpenCommand.Execute(projectItems);
    }

    public void SaveOpenedFiles(IProject project)
    {
        var poco = new LastOpenedProjectFilesPoco
        {
            ProjectFullPath = project.Root.FullPath,
            FilesRelativePathes = _windowsManager.Windows
                .Where(w => w.Model is ICodeFileEditor)
                .Select(w => ((ICodeFileEditor)w.Model).ProjectItem.PathFromRoot)
                .ToList()
        };

        _configStorage.Save(poco);
    }
}
