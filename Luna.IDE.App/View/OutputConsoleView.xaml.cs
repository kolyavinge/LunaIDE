using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.View;

public partial class OutputConsoleView : UserControl
{
    public OutputConsoleView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<OutputConsoleViewModel>();
    }
}
