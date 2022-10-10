using System.Windows.Input;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Controls.Tree;
using Luna.IDE.Common;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.App.ViewModel;

public class ProjectExplorerViewModel : NotificationObject
{
    public ICommand OpenProjectHistoryCommand { get; }

    public TreeViewModel TreeViewModel { get; }

    public ProjectExplorerViewModel(
        IProjectLoader projectLoader,
        IProjectExplorer projectExplorer,
        IProjectExplorerItemOpenCommand itemOpenCommand,
        IOpenProjectHistoryCommand openProjectHistoryCommand,
        TreeViewModel treeViewModel)
    {
        OpenProjectHistoryCommand = openProjectHistoryCommand;
        TreeViewModel = treeViewModel;
        TreeViewModel.OpenItemCommand = itemOpenCommand;
        projectLoader.ProjectOpened += (s, e) => TreeViewModel.TreeRoot = projectExplorer.ProjectTreeRoot;
    }
}
