using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Luna.IDE.App.Media;
using Luna.IDE.Common;

namespace Luna.IDE.App.Controls.Tree;

public partial class TreeView : UserControl
{
    #region TreeRoot
    public TreeItem TreeRoot
    {
        get { return (TreeItem)GetValue(TreeRootProperty); }
        set { SetValue(TreeRootProperty, value); }
    }

    public static readonly DependencyProperty TreeRootProperty =
        DependencyProperty.Register("TreeRoot", typeof(TreeItem), typeof(TreeView), new PropertyMetadata(OnRootPropertyChanged));

    private static void OnRootPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var root = (TreeItem)e.NewValue;
        var tree = (TreeView)d;
        var vm = (TreeViewModel)tree.DataContext;
        vm.TreeRoot = root;
    }
    #endregion

    #region OpenItemCommand
    public ICommand OpenItemCommand
    {
        get { return (ICommand)GetValue(OpenItemCommandProperty); }
        set { SetValue(OpenItemCommandProperty, value); }
    }

    public static readonly DependencyProperty OpenItemCommandProperty =
        DependencyProperty.Register("OpenItemCommand", typeof(ICommand), typeof(TreeView), new PropertyMetadata(OnOpenItemCommandChanged));

    private static void OnOpenItemCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var command = (ICommand)e.NewValue;
        var tree = (TreeView)d;
        var vm = (TreeViewModel)tree.DataContext;
        vm.OpenItemCommand = command;
    }
    #endregion

    #region ImageCollection
    public IImageCollection? ImageCollection
    {
        get { return (IImageCollection?)GetValue(ImageCollectionProperty); }
        set { SetValue(ImageCollectionProperty, value); }
    }

    public static readonly DependencyProperty ImageCollectionProperty =
        DependencyProperty.Register("ImageCollection", typeof(IImageCollection), typeof(TreeView));
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

    #region SelectionMode
    public SelectionMode SelectionMode
    {
        get { return (SelectionMode)GetValue(SelectionModeProperty); }
        set { SetValue(SelectionModeProperty, value); }
    }

    public static readonly DependencyProperty SelectionModeProperty =
        DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(TreeView), new PropertyMetadata(SelectionMode.Single));
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
