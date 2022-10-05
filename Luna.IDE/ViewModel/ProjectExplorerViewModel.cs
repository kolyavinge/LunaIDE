using Luna.IDE.Commands;
using Luna.IDE.Controls.Tree;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel;

public class ProjectExplorerViewModel : NotificationObject
{
    public TreeViewModel TreeViewModel { get; }

    public ProjectExplorerViewModel(
        IProjectExplorer projectExplorer,
        IProjectExplorerItemOpenCommand itemOpenCommand,
        TreeViewModel treeViewModel)
    {
        TreeViewModel = treeViewModel;
        TreeViewModel.OpenItemCommand = itemOpenCommand;
        projectExplorer.ProjectOpened += (s, e) => TreeViewModel.TreeRoot = projectExplorer.ProjectTreeRoot;
    }
}
