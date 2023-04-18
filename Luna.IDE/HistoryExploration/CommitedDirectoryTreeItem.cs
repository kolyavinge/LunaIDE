using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Common;
using Luna.IDE.Versioning;

namespace Luna.IDE.HistoryExploration;

public class CommitedDirectoryTreeItem : TreeItem
{
    private readonly CommitedDirectory _commitedDirectory;

    internal CommitedDirectoryTreeItem(CommitedDirectoryTreeItem? parent, CommitedDirectory commitedDirectory)
        : base(parent, commitedDirectory.Name, "directory.png")
    {
        _commitedDirectory = commitedDirectory;
        _commitedDirectory.ChildrenRefreshed += (s, e) => RefreshChildren();
        IsExpanded = true;
    }

    protected override IEnumerable<TreeItem> GetChildren()
    {
        foreach (var directory in _commitedDirectory.InnerDirectories.OrderBy(x => x.Name)) yield return new CommitedDirectoryTreeItem(this, directory);
        foreach (var file in _commitedDirectory.InnerFiles.OrderBy(x => x.RelativePath)) yield return new CommitedFileTreeItem(this, file);
    }
}
