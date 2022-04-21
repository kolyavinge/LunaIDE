using System.Linq;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands
{
    public class ProjectTreeItemOpenCommand : Command
    {
        private readonly IProjectExplorer _projectExplorer;

        public ProjectTreeItemOpenCommand(IProjectExplorer projectExplorer)
        {
            _projectExplorer = projectExplorer;
        }

        public override void Execute(object parameter)
        {
            var selectedItems = _projectExplorer.SelectedItems.ToList();
            if (selectedItems.Count == 1)
            {
                var projectTreeItem = selectedItems.First();
                if (projectTreeItem.Kind == ProjectTreeItemKind.Directory)
                {
                    projectTreeItem.IsExpanded = !projectTreeItem.IsExpanded;
                }
            }
        }
    }
}
