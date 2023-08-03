using System.Windows.Controls;
using Luna.IDE.App.ViewModel;
using Luna.IDE.Outputing;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.View;

[EnvironmentWindowFor(typeof(OutputConsole))]
public partial class OutputConsoleView : UserControl
{
    public OutputConsoleView()
    {
        InitializeComponent();
    }

    public OutputConsoleView(OutputConsoleViewModel vm) : this()
    {
        DataContext = vm;
    }
}
