using System.Collections.Generic;

namespace Luna.IDE.Versioning;

public interface IProjectRepository
{
    event EventHandler? RepositoryInitialized;

    event EventHandler? CommitMade;

    event EventHandler<ChangesUndoneEventArgs>? ChangesUndone;

    bool IsRepositoryExist { get; }

    VersionControl.Core.VersionedStatus Status { get; }

    public VersionedDirectory Included { get; }

    public VersionedDirectory Excluded { get; }

    void OpenOrCreateRepository();

    void UpdateStatus();

    void IncludeToCommit(IReadOnlyCollection<VersionedFile> files);

    void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> files);

    VersionControl.Core.CommitResult MakeCommit(string comment);

    IReadOnlyCollection<ICommit> FindCommits(VersionControl.Core.FindCommitsFilter filter);

    void UndoChanges(IReadOnlyCollection<VersionedFile> files);
}

public class ChangesUndoneEventArgs : EventArgs
{
    public IReadOnlyCollection<VersionedFile> VersionedFiles { get; }
    public ChangesUndoneEventArgs(IReadOnlyCollection<VersionedFile> versionedFiles)
    {
        VersionedFiles = versionedFiles;
    }
}
