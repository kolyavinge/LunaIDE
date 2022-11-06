using System.Collections.Generic;
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

    IReadOnlyCollection<VersionedFile> SelectedIncludedVersionedFiles { get; }

    IReadOnlyCollection<VersionedFile> SelectedExcludedVersionedFiles { get; }

    void CreateRepository();

    void Activate();

    void Deactivate();

    void IncludeToCommit(IReadOnlyCollection<VersionedFile> versionedFiles);

    void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> versionedFiles);

    void MakeCommit();

    void UndoChanges(IReadOnlyCollection<VersionedFile> versionedFiles);
}
