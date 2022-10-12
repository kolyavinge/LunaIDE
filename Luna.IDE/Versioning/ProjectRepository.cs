using System.Collections.Generic;
using System.Linq;
using Luna.IDE.ProjectExploration;
using VersionControl.Core;

namespace Luna.IDE.Versioning;

public interface IProjectRepository
{
    event EventHandler? RepositoryInitialized;
    event EventHandler? CommitMade;
    bool IsRepositoryExist { get; }
    VersionedStatus Status { get; }
    public VersionedDirectory Included { get; }
    public VersionedDirectory Excluded { get; }
    void OpenOrCreateRepository();
    void UpdateStatus();
    void IncludeToCommit(IReadOnlyCollection<VersionedFile> files);
    void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> files);
    CommitResult MakeCommit(string comment);
    IReadOnlyCollection<ICommit> FindCommits(FindCommitsFilter filter);
}

public class ProjectRepository : IProjectRepository
{
    private IVersionControlRepository _versionControlRepository;
    private readonly IVersionControlRepositoryFactory _versionControlRepositoryFactory;
    private readonly IProjectLoader _projectLoader;

    public event EventHandler? RepositoryInitialized;

    public event EventHandler? CommitMade;

    public bool IsRepositoryExist => _projectLoader.Project != null && _versionControlRepositoryFactory.IsRepositoryExist(_projectLoader.Project.Root.FullPath);

    public VersionedStatus Status { get; private set; }

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
        Status = VersionedStatus.Empty;
        Included = new VersionedDirectory("");
        Excluded = new VersionedDirectory("");
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        _versionControlRepository = _versionControlRepositoryFactory.GetDummyRepository();
        Status = VersionedStatus.Empty;
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
        Status = VersionedStatus.Empty;
        Included = new VersionedDirectory(_projectLoader.Project.Root.Name);
        Excluded = new VersionedDirectory(_projectLoader.Project.Root.Name);
        RepositoryInitialized?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateStatus()
    {
        var newStatus = _versionControlRepository.GetStatus();
        var diff = VersionedStatusDiff.MakeDiff(Status, newStatus);
        if (diff.NoDifference) return;
        Excluded.AddFiles(diff.Added);
        Excluded.DeleteFiles(diff.Deleted);
        Included.DeleteFiles(diff.Deleted);
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

    public CommitResult MakeCommit(string comment)
    {
        var result = _versionControlRepository.MakeCommit(comment, Included.AllFiles);
        CommitMade?.Invoke(this, EventArgs.Empty);

        return result;
    }

    public IReadOnlyCollection<ICommit> FindCommits(FindCommitsFilter filter)
    {
        var commits = _versionControlRepository
            .FindCommits(filter)
            .Select(commit => new Commit(_projectLoader.Project!.Root.Name, _versionControlRepository, commit))
            .ToList();

        return commits;
    }
}
