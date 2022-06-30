using System.Collections;
using System.Linq;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.ProjectModel;

namespace Luna.IDE.Commands;

public interface IProjectTreeItemOpenCommand : ICommand { }

public class ProjectTreeItemOpenCommand : Command, IProjectTreeItemOpenCommand
{
    private readonly IProjectItemOpenCommand _projectItemOpenCommand;

    public ProjectTreeItemOpenCommand(IProjectItemOpenCommand projectItemOpenCommand)
    {
        _projectItemOpenCommand = projectItemOpenCommand;
    }

    public override void Execute(object parameter)
    {
        var treeItems = ((IEnumerable)parameter).Cast<ProjectTreeItem>().Where(x => x.Parent != null).ToList();
        if (!treeItems.Any()) return;
        if (treeItems.Count == 1 && treeItems.First().ProjecItem is DirectoryProjectItem)
        {
            var projectTreeItem = treeItems.First();
            projectTreeItem.IsExpanded = !projectTreeItem.IsExpanded;
        }
        else
        {
            _projectItemOpenCommand.Execute(treeItems.Where(x => x.ProjecItem is CodeFileProjectItem).Select(x => x.ProjecItem));
        }
    }
}
