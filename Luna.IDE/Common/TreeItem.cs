using System.Collections.Generic;
using System.Linq;
using Luna.Utils;

namespace Luna.IDE.Common;

public abstract class TreeItem : NotificationObject
{
    private IReadOnlyCollection<TreeItem>? _children;
    private bool _isExpanded;
    private bool _isSelected;

    public TreeItem? Parent { get; }

    public string? ImageName { get; }

    public string Name { get; }

    public string? AdditionalInfo { get; set; }

    public IReadOnlyCollection<TreeItem> Children => _children ??= GetChildren().ToList();

    public bool HasChildren => Children.Any();

    public int Depth { get; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            RaisePropertyChanged(() => IsExpanded);
            if (!_isExpanded && AllChildren.Any(x => x.IsSelected))
            {
                AllChildren.Each(c => c.IsSelected = false);
                IsSelected = true;
            }
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            RaisePropertyChanged(() => IsSelected);
        }
    }

    public List<TreeItem> AllChildren
    {
        get
        {
            var result = new List<TreeItem>();
            result.Add(this);
            Children.Each(child => result.AddRange(child.AllChildren));

            return result;
        }
    }

    protected TreeItem(TreeItem? parent, string name, string? imageName)
    {
        Parent = parent;
        Name = name;
        ImageName = imageName;
        Depth = GetDepth();
        _isExpanded = Parent == null;
    }

    protected virtual IEnumerable<TreeItem> GetChildren()
    {
        return Enumerable.Empty<TreeItem>();
    }

    protected void RefreshChildren()
    {
        var lastSelectedNames = Children.Where(x => x.IsSelected).Select(x => x.Name).ToHashSet();
        _children = GetChildren().ToList();
        _children.Each(x => x.IsSelected = lastSelectedNames.Contains(x.Name));
        RaisePropertyChanged(() => Children);
        RaisePropertyChanged(() => HasChildren);
    }

    private int GetDepth()
    {
        var parent = Parent;
        if (parent == null) return 0;
        int depth = -1;
        while (parent != null) { parent = parent.Parent; depth++; }

        return depth;
    }
}
