using System.Collections.Generic;
using System.IO;
using System.Linq;
using Luna.IDE.App.Controls.Tree;
using Luna.IDE.App.Media;
using VersionControl.Core;

namespace Luna.IDE.App.Model;

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

public class VersionedFileTreeItem : TreeItem
{
    public VersionedFile VersionedFile { get; }

    public VersionedFileTreeItem(VersionedDirectoryTreeItem parent, VersionedFile versionedFile) :
        base(parent, Path.GetFileName(versionedFile.FullPath), () => ImageCollection.GetImage("codefile.png"))
    {
        VersionedFile = versionedFile;
        AdditionalInfo = GetAdditionalInfo();
    }

    private string GetAdditionalInfo()
    {
        return VersionedFile.ActionKind switch
        {
            FileActionKind.Add => "[add]",
            FileActionKind.Modify => "[edit]",
            FileActionKind.Replace => "[rename]",
            FileActionKind.ModifyAndReplace => "[edit]",
            FileActionKind.Delete => "[del]",
            _ => throw new NotImplementedException()
        };
    }
}
