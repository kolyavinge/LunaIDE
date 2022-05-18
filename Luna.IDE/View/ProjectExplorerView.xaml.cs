using System.Windows.Controls;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View;

public partial class ProjectExplorerView : UserControl
{
    public ProjectExplorerView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<ProjectExplorerViewModel>();
    }
}
