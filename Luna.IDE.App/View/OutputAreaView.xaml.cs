using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.View;

public partial class OutputAreaView : UserControl
{
    public OutputAreaView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<OutputAreaViewModel>();
    }
}
