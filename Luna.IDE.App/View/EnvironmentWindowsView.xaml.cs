using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.View;

public partial class EnvironmentWindowsView : UserControl
{
    public EnvironmentWindowsView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<EnvironmentWindowsViewModel>();
    }
}
