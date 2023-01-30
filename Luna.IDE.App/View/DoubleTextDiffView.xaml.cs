using System.Windows;
using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;
using Luna.IDE.TextDiff;

namespace Luna.IDE.App.View;

public partial class DoubleTextDiffView : UserControl
{
    #region Model
    public IDoubleTextDiff Model
    {
        get { return (IDoubleTextDiff)GetValue(ModelProperty); }
        set { SetValue(ModelProperty, value); }
    }

    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(IDoubleTextDiff), typeof(DoubleTextDiffView), new PropertyMetadata(OnModelPropertyChanged));

    private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (DoubleTextDiffView)d;
        var model = (IDoubleTextDiff)e.NewValue;
        var vm = (DoubleTextDiffViewModel)view.DataContext;
        vm.Model = model;
    }
    #endregion

    public DoubleTextDiffView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<DoubleTextDiffViewModel>();
    }
}
