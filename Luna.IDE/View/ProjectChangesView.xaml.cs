using System.Windows.Controls;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View;

public partial class ProjectChangesView : UserControl
{
    public ProjectChangesView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<ProjectChangesViewModel>();
    }
}
