using System.Collections.Generic;
using Luna.IDE.Common;
using Luna.IDE.Versioning;

namespace Luna.IDE.ProjectChanging;

public interface IProjectChanges
{
    event EventHandler? RepositoryOpened;

    bool IsRepositoryExist { get; }

    bool IsCommitAllowed { get; }

    string Comment { get; set; }

    VersionedDirectoryTreeItem Included { get; }

    VersionedDirectoryTreeItem Excluded { get; }

    IEnumerable<VersionedFile> SelectedIncludedVersionedFiles { get; }

    IEnumerable<VersionedFile> SelectedExcludedVersionedFiles { get; }

    void CreateRepository();

    void Activate();

    void Deactivate();

    void IncludeToCommit(IEnumerable<TreeItem> items);

    void ExcludeFromCommit(IEnumerable<TreeItem> items);

    void MakeCommit();

    void UndoChanges(IEnumerable<TreeItem> items);
}
