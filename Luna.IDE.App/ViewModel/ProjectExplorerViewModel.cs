using Luna.IDE.App.Commands;
using Luna.IDE.App.Controls.Tree;
using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;

namespace Luna.IDE.App.ViewModel;

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
