using System.Windows.Controls;
using Luna.IDE.App.ViewModel;
using Luna.IDE.ProjectChanging;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.View;

[EnvironmentWindowFor(typeof(ProjectChanges))]
public partial class ProjectChangesView : UserControl
{
    public ProjectChangesView()
    {
        InitializeComponent();
    }

    public ProjectChangesView(ProjectChangesViewModel vm) : this()
    {
        DataContext = vm;
    }
}
