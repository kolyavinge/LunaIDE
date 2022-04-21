using System.Windows.Controls;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View
{
    public partial class ProjectTreeView : UserControl
    {
        public ProjectTreeView()
        {
            InitializeComponent();
            DataContext = DependencyContainer.Resolve<ProjectTreeViewModel>();
        }
    }
}
