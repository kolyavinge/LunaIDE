using System.Collections.Generic;
using System.Linq;
using Luna.IDE.CodeEditing;
using Luna.IDE.Common;
using Luna.IDE.Versioning;
using Luna.Infrastructure;
using Luna.Utils;

namespace Luna.IDE.ProjectChanging;

internal class ProjectChanges : NotificationObject, IProjectChanges
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICodeEditorSaver _codeEditorSaver;
    private readonly ICodeEditorUndoChangesLogic _codeEditorUndoChangesLogic;
    private readonly ITimerManager _timerManager;
    private readonly IMessageBox _messageBox;
    private ITimer? _timer;
    private bool _isRepositoryExist;
    private bool _isCommitAllowed;
    private string _comment;
    private bool _isActivated;
    private VersionedDirectoryTreeItem _excluded;
    private VersionedDirectoryTreeItem _included;

    public event EventHandler? RepositoryOpened;

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
        set
        {
            _included = value;
            RaisePropertyChanged(() => Included);
        }
    }

    public VersionedDirectoryTreeItem Excluded
    {
        get => _excluded;
        set
        {
            _excluded = value;
            RaisePropertyChanged(() => Excluded);
        }
    }

    public IReadOnlyCollection<VersionedFile> SelectedIncludedVersionedFiles
    {
        get => Included.AllChildren.Where(x => x.IsSelected).SelectMany(x => x.AllChildren).OfType<VersionedFileTreeItem>().Select(x => x.VersionedFile).Distinct().ToList();
    }

    public IReadOnlyCollection<VersionedFile> SelectedExcludedVersionedFiles
    {
        get => Excluded.AllChildren.Where(x => x.IsSelected).SelectMany(x => x.AllChildren).OfType<VersionedFileTreeItem>().Select(x => x.VersionedFile).Distinct().ToList();
    }

    public ProjectChanges(
        IProjectRepository projectRepository,
        ICodeEditorSaver codeEditorSaver,
        ICodeEditorUndoChangesLogic codeEditorUndoChangesLogic,
        ITimerManager timerManager,
        IMessageBox messageBox)
    {
        _projectRepository = projectRepository;
        _codeEditorSaver = codeEditorSaver;
        _codeEditorUndoChangesLogic = codeEditorUndoChangesLogic;
        _projectRepository.RepositoryInitialized += OnRepositoryInitialized;
        _timerManager = timerManager;
        _messageBox = messageBox;
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
        Included.AllChildren.Each(x => x.Selected += (s, e) => { RaisePropertyChanged(() => SelectedIncludedVersionedFiles); });
        Excluded.AllChildren.Each(x => x.Selected += (s, e) => { RaisePropertyChanged(() => SelectedExcludedVersionedFiles); });
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

    public void IncludeToCommit(IReadOnlyCollection<VersionedFile> versionedFiles)
    {
        _projectRepository.IncludeToCommit(versionedFiles);
        UpdateCommitAllowed();
    }

    public void ExcludeFromCommit(IReadOnlyCollection<VersionedFile> versionedFiles)
    {
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

    public void UndoChanges(IReadOnlyCollection<VersionedFile> versionedFiles)
    {
        if (versionedFiles.Any() && _messageBox.Show("Undo changes", "Do you want to undo changes in the selected files?", MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
        {
            _projectRepository.UndoChanges(versionedFiles);
            _codeEditorUndoChangesLogic.UndoTextChanges(versionedFiles);
            UpdateStatus();
        }
    }

    private void UpdateStatus()
    {
        _projectRepository.UpdateStatus();
        UpdateCommitAllowed();
    }

    private void UpdateCommitAllowed()
    {
        IsCommitAllowed = !string.IsNullOrWhiteSpace(_comment) && _projectRepository.Included.AllFiles.Any();
    }
}
