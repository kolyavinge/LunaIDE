using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.View;

public partial class RecentProjectsView : UserControl
{
    public RecentProjectsView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<RecentProjectsViewModel>();
    }
}
