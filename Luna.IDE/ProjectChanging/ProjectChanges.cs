﻿using System.Collections.Generic;
using System.Linq;
using Luna.IDE.CodeEditing;
using Luna.IDE.Common;
using Luna.IDE.Versioning;
using Luna.Infrastructure;

namespace Luna.IDE.ProjectChanging;

public interface IProjectChanges
{
    event EventHandler? RepositoryOpened;
    event EventHandler? StatusUpdated;
    bool IsRepositoryExist { get; }
    bool IsCommitAllowed { get; }
    string Comment { get; set; }
    VersionedDirectoryTreeItem Included { get; }
    VersionedDirectoryTreeItem Excluded { get; }
    void CreateRepository();
    void Activate();
    void Deactivate();
    void IncludeToCommit(IEnumerable<TreeItem> items);
    void ExcludeFromCommit(IEnumerable<TreeItem> items);
    void MakeCommit();
}

public class ProjectChanges : NotificationObject, IProjectChanges
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICodeEditorSaver _codeEditorSaver;
    private readonly ITimerManager _timerManager;
    private ITimer? _timer;
    private bool _isRepositoryExist;
    private bool _isCommitAllowed;
    private string _comment;
    private bool _isActivated;
    private VersionedDirectoryTreeItem _excluded;
    private VersionedDirectoryTreeItem _included;

    public event EventHandler? RepositoryOpened;

    public event EventHandler? StatusUpdated;

    public bool IsRepositoryExist
    {
        get => _isRepositoryExist;
        set
        {
            _isRepositoryExist = value;
            RaisePropertyChanged(() => IsRepositoryExist);
        }
    }

    public bool IsCommitAllowed
    {
        get => _isCommitAllowed;
        private set
        {
            _isCommitAllowed = value;
            RaisePropertyChanged(() => IsCommitAllowed);
        }
    }

    public string Comment
    {
        get => _comment;
        set
        {
            _comment = value;
            RaisePropertyChanged(() => Comment);
            UpdateCommitAllowed();
        }
    }

    public VersionedDirectoryTreeItem Included
    {
        get => _included;
        private set
        {
            _included = value;
            RaisePropertyChanged(() => Included);
        }
    }

    public VersionedDirectoryTreeItem Excluded
    {
        get => _excluded;
        private set
        {
            _excluded = value;
            RaisePropertyChanged(() => Excluded);
        }
    }

    public ProjectChanges(
        IProjectRepository projectRepository,
        ICodeEditorSaver codeEditorSaver,
        ITimerManager timerManager)
    {
        _projectRepository = projectRepository;
        _codeEditorSaver = codeEditorSaver;
        _projectRepository.RepositoryInitialized += OnRepositoryInitialized;
        _timerManager = timerManager;
        _comment = "";
        _included = new VersionedDirectoryTreeItem(null, new VersionedDirectory(""));
        _excluded = new VersionedDirectoryTreeItem(null, new VersionedDirectory(""));
    }

    private void OnRepositoryInitialized(object? sender, EventArgs e)
    {
        _timer?.Stop();
        IsRepositoryExist = _projectRepository.IsRepositoryExist;
        Included = new VersionedDirectoryTreeItem(null, _projectRepository.Included);
        Excluded = new VersionedDirectoryTreeItem(null, _projectRepository.Excluded);
        if (_isActivated) Activate();
        RepositoryOpened?.Invoke(this, EventArgs.Empty);
    }

    public void CreateRepository()
    {
        _projectRepository.OpenOrCreateRepository();
    }

    public void Activate()
    {
        _isActivated = true;
        if (_projectRepository.IsRepositoryExist)
        {
            UpdateStatus();
            _timer = _timerManager.CreateAndStart(TimeSpan.FromSeconds(2), OnTimer);
        }
    }

    private void OnTimer(object? sender, EventArgs e)
    {
        _codeEditorSaver.SaveOpenedEditors();
        UpdateStatus();
    }

    public void Deactivate()
    {
        _isActivated = false;
        _timer?.Stop();
    }

    public void IncludeToCommit(IEnumerable<TreeItem> items)
    {
        var versionedFiles = items.SelectMany(x => x.AllChildren).OfType<VersionedFileTreeItem>().Select(x => x.VersionedFile).ToList();
        _projectRepository.IncludeToCommit(versionedFiles);
        UpdateCommitAllowed();
    }

    public void ExcludeFromCommit(IEnumerable<TreeItem> items)
    {
        var versionedFiles = items.SelectMany(x => x.AllChildren).OfType<VersionedFileTreeItem>().Select(x => x.VersionedFile).ToList();
        _projectRepository.ExcludeFromCommit(versionedFiles);
        UpdateCommitAllowed();
    }

    public void MakeCommit()
    {
        _codeEditorSaver.SaveOpenedEditors();
        _projectRepository.MakeCommit(Comment);
        Comment = "";
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        _projectRepository.UpdateStatus();
        UpdateCommitAllowed();
        StatusUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateCommitAllowed()
    {
        IsCommitAllowed = !string.IsNullOrWhiteSpace(_comment) && _projectRepository.Included.AllFiles.Any();
    }
}
