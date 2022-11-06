using Luna.IDE.Common;
using Luna.IDE.TextDiff;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

[EnvironmentWindowFor(typeof(ProjectItemChanges))]
public class ProjectItemChangesViewModel : NotificationObject
{
    public IProjectItemChanges Model { get; }

    public ProjectItemChangesViewModel(IProjectItemChanges model)
    {
        Model = model;
    }
}
