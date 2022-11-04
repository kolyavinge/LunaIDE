using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;
using Luna.IDE.TextDiff;

namespace Luna.IDE.App.View;

public partial class SingleTextDiffView : UserControl
{
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

    public SingleTextDiffView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<SingleTextDiffViewModel>();
    }

    private void ScrollBarOnScroll(object sender, ScrollEventArgs e)
    {
        var sb = (ScrollBar)sender;
        if (e.ScrollEventType == ScrollEventType.LargeIncrement)
        {
            sb.Value += 200;
        }
        else if (e.ScrollEventType == ScrollEventType.LargeDecrement)
        {
            sb.Value -= 200;
        }
    }

    private void VerticalScrollBarOnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        verticalScrollBar.Value -= e.Delta;
    }
}
