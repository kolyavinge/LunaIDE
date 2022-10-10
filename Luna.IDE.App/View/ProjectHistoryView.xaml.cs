using System.Windows.Controls;
using Luna.IDE.App.ViewModel;
using Luna.IDE.HistoryExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.View;

[EnvironmentWindowFor(typeof(ProjectHistory))]
public partial class ProjectHistoryView : UserControl
{
    public ProjectHistoryView()
    {
        InitializeComponent();
    }

    public ProjectHistoryView(ProjectHistoryViewModel vm) : this()
    {
        DataContext = vm;
    }
}
