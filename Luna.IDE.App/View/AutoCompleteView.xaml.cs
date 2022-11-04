using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Media;
using Luna.IDE.App.ViewModel;
using Luna.IDE.AutoCompletion;

namespace Luna.IDE.App.View;

public partial class AutoCompleteView : UserControl
{
    #region Model
    public IAutoComplete Model
    {
        get { return (IAutoComplete)GetValue(ModelProperty); }
        set { SetValue(ModelProperty, value); }
    }

    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(IAutoComplete), typeof(AutoCompleteView), new PropertyMetadata(ModelChangedCallback));

    private static void ModelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (AutoCompleteView)d;
        var vm = (AutoCompleteViewModel)view.DataContext;
        vm.Model = (IAutoComplete)e.NewValue;
    }
    #endregion

    #region ImageCollection
    public IImageCollection? ImageCollection
    {
        get { return (IImageCollection?)GetValue(ImageCollectionProperty); }
        set { SetValue(ImageCollectionProperty, value); }
    }

    public static readonly DependencyProperty ImageCollectionProperty =
        DependencyProperty.Register("ImageCollection", typeof(IImageCollection), typeof(AutoCompleteView));
    #endregion

    #region AdditionalInfoForeground
    public Brush AdditionalInfoForeground
    {
        get { return (Brush)GetValue(AdditionalInfoForegroundProperty); }
        set { SetValue(AdditionalInfoForegroundProperty, value); }
    }

    public static readonly DependencyProperty AdditionalInfoForegroundProperty =
        DependencyProperty.Register("AdditionalInfoForeground", typeof(Brush), typeof(AutoCompleteView));
    #endregion

    #region VerticalScrollBarValue
    public double VerticalScrollBarValue
    {
        get { return (double)GetValue(VerticalScrollBarValueProperty); }
        set { SetValue(VerticalScrollBarValueProperty, value); }
    }

    public static readonly DependencyProperty VerticalScrollBarValueProperty =
        DependencyProperty.Register("VerticalScrollBarValue", typeof(double), typeof(AutoCompleteView), new PropertyMetadata(VerticalScrollBarValueChangedCallback));

    private static void VerticalScrollBarValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (AutoCompleteView)d;
        var vm = (AutoCompleteViewModel)view.DataContext;
        vm.CorrectByVerticalScrollBarValue((double)e.NewValue);
    }
    #endregion

    #region HorizontalScrollBarValue
    public double HorizontalScrollBarValue
    {
        get { return (double)GetValue(HorizontalScrollBarValueProperty); }
        set { SetValue(HorizontalScrollBarValueProperty, value); }
    }

    public static readonly DependencyProperty HorizontalScrollBarValueProperty =
        DependencyProperty.Register("HorizontalScrollBarValue", typeof(double), typeof(AutoCompleteView), new PropertyMetadata(HorizontalScrollBarValueChangedCallback));

    private static void HorizontalScrollBarValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (AutoCompleteView)d;
        var vm = (AutoCompleteViewModel)view.DataContext;
        vm.CorrectByHorizontalScrollBarValue((double)e.NewValue);
    }
    #endregion

    #region ParentWidth
    public double ParentWidth
    {
        get { return (double)GetValue(ParentWidthProperty); }
        set { SetValue(ParentWidthProperty, value); }
    }

    public static readonly DependencyProperty ParentWidthProperty =
        DependencyProperty.Register("ParentWidth", typeof(double), typeof(AutoCompleteView));
    #endregion

    #region ParentHeight
    public double ParentHeight
    {
        get { return (double)GetValue(ParentHeightProperty); }
        set { SetValue(ParentHeightProperty, value); }
    }

    public static readonly DependencyProperty ParentHeightProperty =
        DependencyProperty.Register("ParentHeight", typeof(double), typeof(AutoCompleteView));
    #endregion

    static AutoCompleteView()
    {
        VisibilityProperty.OverrideMetadata(typeof(AutoCompleteView), new FrameworkPropertyMetadata(VisibilityChanged));
    }

    private static void VisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (AutoCompleteView)d;
        var vm = (AutoCompleteViewModel)view.DataContext;
        if ((Visibility)e.NewValue == Visibility.Visible)
        {
            vm.Show(view.VerticalScrollBarValue, view.HorizontalScrollBarValue, view.ParentWidth, view.ParentHeight);
        }
    }

    public AutoCompleteView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<AutoCompleteViewModel>();
    }

    private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var lb = (ListBox)sender;
        lb.ScrollIntoView(lb.SelectedItem);
    }
}
