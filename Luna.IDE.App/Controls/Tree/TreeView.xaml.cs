using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Luna.IDE.Controls.Tree;

public partial class TreeView : UserControl
{
    #region AdditionalInfoForeground
    public Brush AdditionalInfoForeground
    {
        get { return (Brush)GetValue(AdditionalInfoForegroundProperty); }
        set { SetValue(AdditionalInfoForegroundProperty, value); }
    }

    public static readonly DependencyProperty AdditionalInfoForegroundProperty =
        DependencyProperty.Register("AdditionalInfoForeground", typeof(Brush), typeof(TreeView));
    #endregion

    public TreeView()
    {
        InitializeComponent();
    }
}
