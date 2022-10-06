using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.View;

public partial class ProjectExplorerView : UserControl
{
    public ProjectExplorerView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<ProjectExplorerViewModel>();
    }
}
