using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Common;
using Luna.IDE.Media;
using Luna.IDE.Versioning;

namespace Luna.IDE.ProjectChanging;

public class VersionedDirectoryTreeItem : TreeItem
{
    private readonly VersionedDirectory _versionedDirectory;

    public VersionedDirectoryTreeItem(VersionedDirectoryTreeItem? parent, VersionedDirectory versionedDirectory) :
        base(parent, versionedDirectory.Name, () => ImageCollection.GetImage("directory.png"))
    {
        _versionedDirectory = versionedDirectory;
        _versionedDirectory.ChildrenRefreshed += (s, e) => RefreshChildren();
        IsExpanded = true;
    }

    protected override IEnumerable<TreeItem> GetChildren()
    {
        foreach (var directory in _versionedDirectory.InnerDirectories.OrderBy(x => x.Name)) yield return new VersionedDirectoryTreeItem(this, directory);
        foreach (var file in _versionedDirectory.InnerFiles.OrderBy(x => x.RelativePath)) yield return new VersionedFileTreeItem(this, file);
    }
}
