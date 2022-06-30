using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Luna.IDE.Mvvm;
using Luna.Utils;

namespace Luna.IDE.Controls.Tree;

public class TreeViewModel : NotificationObject
{
    private IEnumerable<TreeItem> _treeItems;
    private TreeItem? _treeRoot;

    public ICommand? OpenItemCommand { get; set; }

    public TreeItem? TreeRoot
    {
        get => _treeRoot;
        set
        {
            _treeRoot = value;
            UpdateTreeItems();
            RaisePropertyChanged(() => TreeRoot!);
        }
    }

    public IEnumerable<TreeItem> TreeItems
    {
        get => _treeItems;
        private set
        {
            _treeItems = value;
            RaisePropertyChanged(() => TreeItems);
        }
    }

    public TreeViewModel()
    {
        _treeItems = new List<TreeItem>();
    }

    private void OnTreeItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsExpanded")
        {
            UpdateTreeItems();
        }
    }

    private void UpdateTreeItems()
    {
        TreeItems.Each(x => x.PropertyChanged -= OnTreeItemPropertyChanged);
        TreeItems = MakeFlatList(TreeRoot!);
        TreeItems.Each(x => x.PropertyChanged += OnTreeItemPropertyChanged);
    }

    private List<TreeItem> MakeFlatList(TreeItem item)
    {
        var result = new List<TreeItem> { item };
        if (item.IsExpanded && item.Children != null)
        {
            item.Children.Each(child => result.AddRange(MakeFlatList(child)));
        }

        return result;
    }
}
