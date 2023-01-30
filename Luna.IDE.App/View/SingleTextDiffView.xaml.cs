using System.Windows;
using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;
using Luna.IDE.TextDiff;

namespace Luna.IDE.App.View;

public partial class SingleTextDiffView : UserControl
{
    #region Model
    public ISingleTextDiff Model
    {
        get { return (ISingleTextDiff)GetValue(ModelProperty); }
        set { SetValue(ModelProperty, value); }
    }

    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(ISingleTextDiff), typeof(SingleTextDiffView), new PropertyMetadata(OnModelPropertyChanged));

    private static void OnModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (SingleTextDiffView)d;
        var model = (ISingleTextDiff)e.NewValue;
        var vm = (SingleTextDiffViewModel)view.DataContext;
        vm.Model = model;
    }
    #endregion

    public SingleTextDiffView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<SingleTextDiffViewModel>();
    }
}
