using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View;

public partial class AutoCompleteView : UserControl
{
    #region AdditionalInfoForeground
    public Brush AdditionalInfoForeground
    {
        get { return (Brush)GetValue(AdditionalInfoForegroundProperty); }
        set { SetValue(AdditionalInfoForegroundProperty, value); }
    }

    public static readonly DependencyProperty AdditionalInfoForegroundProperty =
        DependencyProperty.Register("AdditionalInfoForeground", typeof(Brush), typeof(AutoCompleteView));
    #endregion

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
