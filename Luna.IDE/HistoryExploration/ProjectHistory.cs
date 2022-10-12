using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Luna.IDE.Common;
using Luna.IDE.ProjectExploration;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.HistoryExploration;

public interface IProjectHistory : INotifyPropertyChanged
{
    IReadOnlyCollection<ICommit> Commits { get; }
    bool AnyCommits { get; }
    ICommit? SelectedCommit { get; set; }
    CommitedDirectoryTreeItem? DetailsRoot { get; }
}

public class ProjectHistory : NotificationObject, IProjectHistory, IEnvironmentWindowModel
{
    private readonly IProjectRepository _projectRepository;
    private IReadOnlyCollection<ICommit> _commits;
    private ICommit? _selectedCommit;
    private CommitedDirectoryTreeItem? _detailsRoot;

    public string Header => "History";

    public IReadOnlyCollection<ICommit> Commits
    {
        get
        {
            _commits = _projectRepository.FindCommits(new());
            return _commits;
        }
    }

    public bool AnyCommits => _commits.Any();

    public ICommit? SelectedCommit
    {
        get => _selectedCommit;
        set
        {
            _selectedCommit = value;
            if (_selectedCommit != null)
            {
                DetailsRoot = new CommitedDirectoryTreeItem(null, _selectedCommit.Details);
                RaisePropertyChanged(() => SelectedCommit!);
            }
        }
    }

    public CommitedDirectoryTreeItem? DetailsRoot
    {
        get => _detailsRoot;
        set
        {
            _detailsRoot = value;
            RaisePropertyChanged(() => DetailsRoot!);
        }
    }

    public ProjectHistory(IProjectLoader projectLoader, IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
        _projectRepository.CommitMade += OnCommitMade;
        projectLoader.ProjectOpened += OnProjectOpened;
        _commits = new List<Commit>();
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        SelectedCommit = null;
        RaiseCommitsChanged();
    }

    private void OnCommitMade(object? sender, EventArgs e)
    {
        var selectedCommit = SelectedCommit;
        RaiseCommitsChanged();
        if (selectedCommit != null)
        {
            SelectedCommit = _commits.First(x => x.Id == selectedCommit.Id);
        }
    }

    private void RaiseCommitsChanged()
    {
        RaisePropertyChanged(() => Commits);
        RaisePropertyChanged(() => AnyCommits);
    }

    public void Save() { }

    public void Close() { }
}
