using System.Windows.Controls;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View;

public partial class OutputConsoleView : UserControl
{
    public OutputConsoleView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<OutputConsoleViewModel>();
    }
}
