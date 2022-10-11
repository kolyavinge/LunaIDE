using System.Windows.Input;
using Luna.IDE.App.Commands;
using Luna.IDE.Common;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.App.ViewModel;

public class ProjectExplorerViewModel : NotificationObject
{
    public IProjectExplorer Model { get; }

    public ICommand OpenProjectHistoryCommand { get; }

    public ICommand ItemOpenCommand { get; }

    public ProjectExplorerViewModel(
        IProjectExplorer projectExplorer,
        IProjectExplorerItemOpenCommand itemOpenCommand,
        IOpenProjectHistoryCommand openProjectHistoryCommand)
    {
        Model = projectExplorer;
        OpenProjectHistoryCommand = openProjectHistoryCommand;
        ItemOpenCommand = itemOpenCommand;
    }
}
