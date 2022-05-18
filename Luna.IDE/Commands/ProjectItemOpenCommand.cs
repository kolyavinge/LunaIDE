using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.ProjectModel;

namespace Luna.IDE.Commands;

public interface IProjectItemOpenCommand : ICommand { }

public class ProjectItemOpenCommand : Command, IProjectItemOpenCommand
{
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IProjectItemEditorFactory _editorFactory;

    public ProjectItemOpenCommand(IEnvironmentWindowsManager windowsManager, IProjectItemEditorFactory editorFactory)
    {
        _windowsManager = windowsManager;
        _editorFactory = editorFactory;
    }

    public override void Execute(object parameter)
    {
        var projectItems = ((IEnumerable<ProjectItem>)parameter).ToList();
        if (!projectItems.Any()) return;

        var firstItem = projectItems.First();
        var firstWindow = OpenWindowFor(firstItem);
        _windowsManager.ActivateWindow(firstWindow);

        foreach (var item in projectItems.Skip(1))
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
