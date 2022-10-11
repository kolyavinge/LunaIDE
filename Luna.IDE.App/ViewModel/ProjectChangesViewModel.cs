using System.Linq;
using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Common;
using Luna.IDE.ProjectChanging;

namespace Luna.IDE.App.ViewModel;

public class ProjectChangesViewModel : NotificationObject
{
    private bool _isAnyFilesIncluded;
    private bool _isAnyFilesExcluded;

    public IProjectChanges Model { get; }

    public bool IsAnyFilesIncluded
    {
        get => _isAnyFilesIncluded;
        set
        {
            _isAnyFilesIncluded = value;
            RaisePropertyChanged(() => IsAnyFilesIncluded);
        }
    }

    public bool IsAnyFilesExcluded
    {
        get => _isAnyFilesExcluded;
        set
        {
            _isAnyFilesExcluded = value;
            RaisePropertyChanged(() => IsAnyFilesExcluded);
        }
    }

    public ICommand LoadedCommand { get; }

    public ICommand UnloadedCommand { get; }

    public ICommand CreateRepositoryCommand { get; }

    public ICommand IncludeToCommitCommand { get; }

    public ICommand ExcludeFromCommitCommand { get; }

    public ICommand MakeCommitCommand { get; }

    public ProjectChangesViewModel(IProjectChanges projectChanges)
    {
        Model = projectChanges;
        LoadedCommand = new ActionCommand(Model.Activate);
        UnloadedCommand = new ActionCommand(Model.Deactivate);
        CreateRepositoryCommand = new ActionCommand(Model.CreateRepository);
        IncludeToCommitCommand = new ActionCommand(IncludeToCommit);
        ExcludeFromCommitCommand = new ActionCommand(ExcludeFromCommit);
        MakeCommitCommand = new ActionCommand(Model.MakeCommit);
    }

    private void IncludeToCommit()
    {
        var selected = Model.Excluded.AllChildren.Where(x => x.IsSelected);
        Model.IncludeToCommit(selected);
    }

    private void ExcludeFromCommit()
    {
        var selected = Model.Included.AllChildren.Where(x => x.IsSelected);
        Model.ExcludeFromCommit(selected);
    }
}
