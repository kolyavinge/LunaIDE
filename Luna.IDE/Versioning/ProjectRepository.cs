using System.Collections.Generic;
using System.Linq;
using Luna.IDE.ProjectExploration;
using VersionControl.Core;

namespace Luna.IDE.Versioning;

public interface IProjectRepository
{
    event EventHandler? RepositoryOpened;
    bool IsRepositoryExist { get; }
    VersionedStatus Status { get; }
    public VersionedDirectory Included { get; }
    public VersionedDirectory Excluded { get; }
    void OpenOrCreateRepository();
    void UpdateStatus();
    void IncludeToCommit(IReadOnlyCollection<VersionedFile> files);
    void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> files);
    CommitResult MakeCommit(string comment);
    IReadOnlyCollection<Commit> FindCommits(FindCommitsFilter filter);
}

public class ProjectRepository : IProjectRepository
{
    private IVersionControlRepository _versionControlRepository;
    private readonly IProjectLoader _projectLoader;

    public event EventHandler? RepositoryOpened;

    public bool IsRepositoryExist => _projectLoader.Project != null && VersionControlRepositoryFactory.IsRepositoryExist(_projectLoader.Project.Root.FullPath);

    public VersionedStatus Status { get; private set; }

    public VersionedDirectory Included { get; private set; }

    public VersionedDirectory Excluded { get; private set; }

    public ProjectRepository(IProjectLoader projectLoader)
    {
        _projectLoader = projectLoader;
        _projectLoader.ProjectOpened += OnProjectOpened;
        _versionControlRepository = DummyVersionControlRepository.Instance;
        Status = VersionedStatus.Empty;
        Included = new VersionedDirectory("");
        Excluded = new VersionedDirectory("");
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        if (VersionControlRepositoryFactory.IsRepositoryExist(_projectLoader.Project!.Root.FullPath))
        {
            OpenOrCreateRepository();
        }
    }

    public void OpenOrCreateRepository()
    {
        if (_projectLoader.Project == null) return;
        _versionControlRepository = VersionControlRepositoryFactory.OpenRepository(_projectLoader.Project.Root.FullPath);
        Status = VersionedStatus.Empty;
        Included = new VersionedDirectory(_projectLoader.Project.Root.Name);
        Excluded = new VersionedDirectory(_projectLoader.Project.Root.Name);
        RepositoryOpened?.Invoke(this, EventArgs.Empty);
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
        return _versionControlRepository.MakeCommit(comment, Included.AllFiles);
    }

    public IReadOnlyCollection<Commit> FindCommits(FindCommitsFilter filter)
    {
        var commits = _versionControlRepository
            .FindCommits(filter)
            .Select(commit => new Commit(_projectLoader.Project!.Root.Name, _versionControlRepository, commit))
            .ToList();

        return commits;
    }
}
