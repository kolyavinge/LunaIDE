using Luna.IDE.App.Controls.Tree;
using Luna.IDE.Common;
using Luna.IDE.HistoryExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

[EnvironmentWindowFor(typeof(ProjectHistory))]
public class ProjectHistoryViewModel : NotificationObject
{
    public IProjectHistory Model { get; }

    public TreeViewModel DetailsTreeViewModel { get; set; }

    public ProjectHistoryViewModel(IProjectHistory projectHistory, TreeViewModel detailsTreeViewModel)
    {
        Model = projectHistory;
        DetailsTreeViewModel = detailsTreeViewModel;
        Model.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == "DetailsRoot")
            {
                DetailsTreeViewModel.TreeRoot = Model.DetailsRoot;
            }
        };
    }
}
