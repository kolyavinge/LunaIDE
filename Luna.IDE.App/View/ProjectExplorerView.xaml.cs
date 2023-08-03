using System.Windows.Controls;
using Luna.IDE.App.ViewModel;
using Luna.IDE.ProjectExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.View;

[EnvironmentWindowFor(typeof(ProjectExplorer))]
public partial class ProjectExplorerView : UserControl
{
    public ProjectExplorerView()
    {
        InitializeComponent();
    }

    public ProjectExplorerView(ProjectExplorerViewModel vm) : this()
    {
        DataContext = vm;
    }
}
