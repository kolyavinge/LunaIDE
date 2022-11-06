using System.Windows.Controls;
using Luna.IDE.App.ViewModel;
using Luna.IDE.TextDiff;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.View;

[EnvironmentWindowFor(typeof(ProjectItemChanges))]
public partial class ProjectItemChangesView : UserControl
{
    public ProjectItemChangesView()
    {
        InitializeComponent();
    }

    public ProjectItemChangesView(ProjectItemChangesViewModel vm) : this()
    {
        DataContext = vm;
    }
}
