using Luna.IDE.Common;
using Luna.IDE.HistoryExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

[EnvironmentWindowFor(typeof(ProjectHistory))]
public class ProjectHistoryViewModel : NotificationObject
{
    public IProjectHistory Model { get; }

    public ProjectHistoryViewModel(IProjectHistory projectHistory)
    {
        Model = projectHistory;
    }
}
