﻿using System.Collections.Generic;
using VersionControl.Core;

namespace Luna.IDE.Model;

public interface IProjectRepository
{
    event EventHandler? RepositoryOpened;
    bool IsRepositoryExist { get; }
    VersionedStatus CurrentStatus { get; }
    public VersionedDirectory Included { get; }
    public VersionedDirectory Excluded { get; }
    void OpenOrCreateRepository();
    void UpdateStatus();
    void IncludeToCommit(IReadOnlyCollection<VersionedFile> files);
    void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> files);
    IReadOnlyCollection<VersionedFile> GetFilesToCommit();
    CommitResult MakeCommit(string comment);
}

public class ProjectRepository : IProjectRepository
{
    private readonly IProjectExplorer _projectExplorer;
    private IVersionControlRepository _versionControlRepository;

    public event EventHandler? RepositoryOpened;

    public bool IsRepositoryExist => _projectExplorer.Project != null && VersionControlRepositoryFactory.IsRepositoryExist(_projectExplorer.Project.Root.FullPath);

    public VersionedStatus CurrentStatus { get; private set; }

    public VersionedDirectory Included { get; private set; }

    public VersionedDirectory Excluded { get; private set; }

    public ProjectRepository(IProjectExplorer projectExplorer)
    {
        _projectExplorer = projectExplorer;
        _projectExplorer.ProjectOpened += OnProjectOpened;
        _versionControlRepository = DummyVersionControlRepository.Instance;
        CurrentStatus = VersionedStatus.Empty;
        Included = new VersionedDirectory("");
        Excluded = new VersionedDirectory("");
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        if (VersionControlRepositoryFactory.IsRepositoryExist(_projectExplorer.Project!.Root.FullPath))
        {
            OpenOrCreateRepository();
        }
    }

    public void OpenOrCreateRepository()
    {
        if (_projectExplorer.Project == null) return;
        _versionControlRepository = VersionControlRepositoryFactory.OpenRepository(_projectExplorer.Project.Root.FullPath);
        CurrentStatus = VersionedStatus.Empty;
        Included = new VersionedDirectory(_projectExplorer.Project.Root.Name);
        Excluded = new VersionedDirectory(_projectExplorer.Project.Root.Name);
        RepositoryOpened?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateStatus()
    {
        var newStatus = _versionControlRepository.GetStatus();
        var diff = VersionedStatusDiff.MakeDiff(CurrentStatus, newStatus);
        if (diff.NoDifference) return;
        Excluded.AddFiles(diff.Added);
        Excluded.DeleteFiles(diff.Deleted);
        Included.DeleteFiles(diff.Deleted);
        CurrentStatus = newStatus;
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

    public IReadOnlyCollection<VersionedFile> GetFilesToCommit()
    {
        return Included.AllFiles;
    }

    public CommitResult MakeCommit(string comment)
    {
        return _versionControlRepository.MakeCommit(comment, GetFilesToCommit());
    }
}