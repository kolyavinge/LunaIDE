using System.Collections.Generic;
using System.Linq;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.Versioning;

internal class ProjectRepository : IProjectRepository
{
    private VersionControl.Core.IVersionControlRepository _versionControlRepository;
    private readonly IVersionControlRepositoryFactory _versionControlRepositoryFactory;
    private readonly IProjectLoader _projectLoader;

    public event EventHandler? RepositoryInitialized;

    public event EventHandler? CommitMade;

    public event EventHandler<ChangesUndoneEventArgs>? ChangesUndone;

    public bool IsRepositoryExist => _projectLoader.Project != null && _versionControlRepositoryFactory.IsRepositoryExist(_projectLoader.Project.Root.FullPath);

    public VersionControl.Core.VersionedStatus Status { get; private set; }

    public VersionedDirectory Included { get; private set; }

    public VersionedDirectory Excluded { get; private set; }

    public ProjectRepository(
        IVersionControlRepositoryFactory versionControlRepositoryFactory,
        IProjectLoader projectLoader)
    {
        _versionControlRepositoryFactory = versionControlRepositoryFactory;
        _projectLoader = projectLoader;
        _projectLoader.ProjectOpened += OnProjectOpened;
        _versionControlRepository = _versionControlRepositoryFactory.GetDummyRepository();
        Status = VersionControl.Core.VersionedStatus.Empty;
        Included = new VersionedDirectory("");
        Excluded = new VersionedDirectory("");
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        _versionControlRepository = _versionControlRepositoryFactory.GetDummyRepository();
        Status = VersionControl.Core.VersionedStatus.Empty;
        if (_versionControlRepositoryFactory.IsRepositoryExist(_projectLoader.Project!.Root.FullPath))
        {
            OpenOrCreateRepository();
        }
        else
        {
            RepositoryInitialized?.Invoke(this, EventArgs.Empty);
        }
    }

    public void OpenOrCreateRepository()
    {
        if (_projectLoader.Project == null) return;
        _versionControlRepository = _versionControlRepositoryFactory.OpenRepository(_projectLoader.Project.Root.FullPath);
        Status = VersionControl.Core.VersionedStatus.Empty;
        Included = new VersionedDirectory(_projectLoader.Project.Root.Name);
        Excluded = new VersionedDirectory(_projectLoader.Project.Root.Name);
        RepositoryInitialized?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateStatus()
    {
        var newStatus = _versionControlRepository.GetStatus();
        var diff = VersionControl.Core.VersionedStatusDiff.MakeDiff(Status, newStatus);
        if (diff.NoDifference) return;
        var added = diff.Added.Select(file => new VersionedFile(_versionControlRepository, file)).ToList();
        var deleted = diff.Deleted.Select(file => new VersionedFile(_versionControlRepository, file)).ToList();
        Excluded.AddFiles(added);
        Excluded.DeleteFiles(deleted);
        Included.DeleteFiles(deleted);
        Status = newStatus;
    }

    public void IncludeToCommit(IReadOnlyCollection<VersionedFile> files)
    {
        Excluded.DeleteFiles(files);
        Included.AddFiles(files);
    }

    public void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> files)
    {
        Excluded.AddFiles(files);
        Included.DeleteFiles(files);
    }

    public VersionControl.Core.CommitResult MakeCommit(string comment)
    {
        var result = _versionControlRepository.MakeCommit(comment, Included.AllFiles.Select(x => x.InnerVersionedFile).ToList());
        CommitMade?.Invoke(this, EventArgs.Empty);

        return result;
    }

    public IReadOnlyCollection<ICommit> FindCommits(VersionControl.Core.FindCommitsFilter filter)
    {
        var commits = _versionControlRepository
            .FindCommits(filter)
            .Select(commit => new Commit(_projectLoader.Project!.Root.Name, _versionControlRepository, commit))
            .ToList();

        return commits;
    }

    public void UndoChanges(IReadOnlyCollection<VersionedFile> files)
    {
        _versionControlRepository.UndoChanges(files.Select(x => x.InnerVersionedFile).ToList());
        ChangesUndone?.Invoke(this, new(files));
    }
}
