using System.IO;
using Luna.IDE.Common;
using Luna.IDE.Media;
using Luna.IDE.Versioning;
using VersionControl.Core;

namespace Luna.IDE.ProjectChanging;

public class VersionedFileTreeItem : TreeItem
{
    public VersionedFile VersionedFile { get; }

    public VersionedFileTreeItem(VersionedDirectoryTreeItem parent, VersionedFile versionedFile) :
        base(parent, Path.GetFileName(versionedFile.FullPath), () => ImageCollection.GetImage("codefile.png"))
    {
        VersionedFile = versionedFile;
        AdditionalInfo = FileActionKindUtils.FileActionKindToString(VersionedFile.ActionKind);
    }
}
