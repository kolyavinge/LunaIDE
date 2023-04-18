using System.IO;
using Luna.IDE.Common;
using Luna.IDE.Versioning;

namespace Luna.IDE.HistoryExploration;

internal class CommitedFileTreeItem : TreeItem
{
    public CommitedFile CommitedFile { get; }

    public CommitedFileTreeItem(TreeItem? parent, CommitedFile commitedFile)
        : base(parent, Path.GetFileName(commitedFile.RelativePath), "codefile.png")
    {
        CommitedFile = commitedFile;
        AdditionalInfo = FileActionKindUtils.FileActionKindToString(commitedFile.ActionKind);
    }
}
