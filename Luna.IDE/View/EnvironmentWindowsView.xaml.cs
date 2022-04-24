using System.Windows.Controls;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View
{
    public partial class EnvironmentWindowsView : UserControl
    {
        public EnvironmentWindowsView()
        {
            InitializeComponent();
            DataContext = DependencyContainer.Resolve<EnvironmentWindowsViewModel>();
        }
    }
}
