using System.Windows.Input;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Media;
using Luna.IDE.Common;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.App.ViewModel;

public class ProjectExplorerViewModel : NotificationObject
{
    public IProjectExplorer Model { get; }

    public ICommand OpenProjectHistoryCommand { get; }

    public ICommand ItemOpenCommand { get; }

    [Inject]
    public IImageCollection? ImageCollection { get; set; }

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
