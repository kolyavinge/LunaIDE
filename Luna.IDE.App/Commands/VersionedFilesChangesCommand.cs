using System.Collections.Generic;
using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.ProjectExploration;
using Luna.IDE.TextDiff;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.App.Commands;

public interface IVersionedFilesChangesCommand : ICommand { }

public class VersionedFilesChangesCommand : Command<IEnumerable<VersionedFile>>, IVersionedFilesChangesCommand
{
    private readonly ISelectedProject _selectedProject;
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IEnvironmentWindowsFactory _environmentWindowsFactory;

    public VersionedFilesChangesCommand(
        ISelectedProject selectedProject,
        IEnvironmentWindowsManager windowsManager,
        IEnvironmentWindowsFactory environmentWindowsFactory)
    {
        _selectedProject = selectedProject;
        _windowsManager = windowsManager;
        _environmentWindowsFactory = environmentWindowsFactory;
    }

    protected override void Execute(IEnumerable<VersionedFile> versionedFiles)
    {
        EnvironmentWindow? window = null;
        foreach (var versionedFile in versionedFiles)
        {
            var fileProjectItem = (TextFileProjectItem)_selectedProject.Project!.FindItemByPath(versionedFile.RelativePath)!;
            var windowId = $"Changes_{versionedFile.RelativePath}";
            window = _windowsManager.FindWindowById(windowId);
            if (window == null)
            {
                var components = _environmentWindowsFactory.MakeWindowFor(typeof(ProjectItemChanges));
                ((IProjectItemChanges)components.Model).MakeDiff(versionedFile.LastCommitContent, fileProjectItem);
                window = _windowsManager.OpenWindow(windowId, components.Model, components.View);
            }
        }
        if (window != null)
        {
            _windowsManager.ActivateWindow(window);
        }
    }
}
