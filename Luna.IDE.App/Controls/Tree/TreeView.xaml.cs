using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Luna.IDE.Common;

namespace Luna.IDE.App.Controls.Tree;

public partial class TreeView : UserControl
{
    #region TreeRoot
    public static readonly DependencyProperty TreeRootProperty =
        DependencyProperty.Register("TreeRoot", typeof(TreeItem), typeof(TreeView), new PropertyMetadata(OnRootPropertyChanged));

    public TreeItem TreeRoot
    {
        get { return (TreeItem)GetValue(TreeRootProperty); }
        set { SetValue(TreeRootProperty, value); }
    }

    private static void OnRootPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var root = (TreeItem)e.NewValue;
        var tree = (TreeView)d;
        var vm = (TreeViewModel)tree.DataContext;
        vm.TreeRoot = root;
    }
    #endregion

    #region OpenItemCommand
    public static readonly DependencyProperty OpenItemCommandProperty =
        DependencyProperty.Register("OpenItemCommand", typeof(ICommand), typeof(TreeView), new PropertyMetadata(OnOpenItemCommandChanged));

    public ICommand OpenItemCommand
    {
        get { return (ICommand)GetValue(OpenItemCommandProperty); }
        set { SetValue(OpenItemCommandProperty, value); }
    }

    private static void OnOpenItemCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var command = (ICommand)e.NewValue;
        var tree = (TreeView)d;
        var vm = (TreeViewModel)tree.DataContext;
        vm.OpenItemCommand = command;
    }
    #endregion

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
        DataContext = new TreeViewModel();
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var vm = (TreeViewModel)DataContext;
            vm.OpenItemCommand?.Execute(lb.SelectedItems);
        }
    }
}
