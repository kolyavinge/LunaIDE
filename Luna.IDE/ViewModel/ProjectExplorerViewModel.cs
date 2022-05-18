using DependencyInjection;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel;

public class ProjectExplorerViewModel : NotificationObject
{
    [Inject]
    public ProjectTreeViewModel? ProjectTreeViewModel { get; set; }
}
