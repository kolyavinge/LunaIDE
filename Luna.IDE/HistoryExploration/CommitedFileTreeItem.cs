using System.IO;
using Luna.IDE.Common;
using Luna.IDE.Media;
using Luna.IDE.Versioning;

namespace Luna.IDE.HistoryExploration;

public class CommitedFileTreeItem : TreeItem
{
    public CommitedFile CommitedFile { get; }

    public CommitedFileTreeItem(TreeItem? parent, CommitedFile commitedFile)
        : base(parent, Path.GetFileName(commitedFile.RelativePath), () => ImageCollection.GetImage("codefile.png"))
    {
        CommitedFile = commitedFile;
        AdditionalInfo = FileActionKindUtils.FileActionKindToString(commitedFile.ActionKind);
    }
}
