using System.IO;
using Luna.IDE.Common;
using Luna.IDE.Versioning;

namespace Luna.IDE.ProjectChanging;

internal class VersionedFileTreeItem : TreeItem
{
    public VersionedFile VersionedFile { get; }

    public VersionedFileTreeItem(VersionedDirectoryTreeItem parent, VersionedFile versionedFile) :
        base(parent, Path.GetFileName(versionedFile.FullPath), "codefile.png")
    {
        VersionedFile = versionedFile;
        AdditionalInfo = FileActionKindUtils.FileActionKindToString(VersionedFile.ActionKind);
    }
}
