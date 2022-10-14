using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Media;
using Luna.IDE.Common;
using Luna.IDE.HistoryExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

[EnvironmentWindowFor(typeof(ProjectHistory))]
public class ProjectHistoryViewModel : NotificationObject
{
    public IProjectHistory Model { get; }

    [Inject]
    public IImageCollection? ImageCollection { get; set; }

    public ProjectHistoryViewModel(IProjectHistory projectHistory)
    {
        Model = projectHistory;
    }
}
