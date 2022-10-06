using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Model;
using Luna.ProjectModel;

namespace Luna.IDE.App.Commands;

public interface IProjectItemOpenCommand : ICommand { }

public class ProjectItemOpenCommand : Command<IEnumerable<ProjectItem>>, IProjectItemOpenCommand
{
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IProjectItemEditorFactory _editorFactory;

    public ProjectItemOpenCommand(IEnvironmentWindowsManager windowsManager, IProjectItemEditorFactory editorFactory)
    {
        _windowsManager = windowsManager;
        _editorFactory = editorFactory;
    }

    protected override void Execute(IEnumerable<ProjectItem> projectItems)
    {
        var projectItemsList = projectItems.ToList();
        if (!projectItemsList.Any()) return;

        var firstItem = projectItemsList.First();
        var firstWindow = OpenWindowFor(firstItem);
        _windowsManager.ActivateWindow(firstWindow);

        foreach (var item in projectItemsList.Skip(1))
        {
            OpenWindowFor(item);
        }
    }

    private EnvironmentWindow OpenWindowFor(ProjectItem projectItem)
    {
        var window = _windowsManager.FindWindowById(projectItem);
        if (window == null)
        {
            var components = _editorFactory.MakeEditorFor(projectItem);
            window = _windowsManager.OpenWindow(projectItem, components.Model, components.View);
        }

        return window;
    }
}
