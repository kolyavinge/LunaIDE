using System.Collections;
using System.Linq;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands
{
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
            var treeItems = ((IEnumerable)parameter).Cast<ProjectTreeItem>().ToList();
            if (!treeItems.Any()) return;
            if (treeItems.Count == 1 && treeItems.First().Kind == ProjectTreeItemKind.Directory)
            {
                var projectTreeItem = treeItems.First();
                projectTreeItem.IsExpanded = !projectTreeItem.IsExpanded;
            }
            else
            {
                _projectItemOpenCommand.Execute(treeItems.Where(x => x.Kind != ProjectTreeItemKind.Directory).Select(x => x.ProjecItem));
            }
        }
    }
}
