using System.Linq;
using System.Windows.Input;
using Luna.IDE.Controls.Tree;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel;

public class ProjectChangesViewModel : NotificationObject
{
    private TreeViewModel _includedTreeViewModel;
    private TreeViewModel _excludedTreeViewModel;
    private bool _isAnyFilesIncluded;
    private bool _isAnyFilesExcluded;

    public IProjectChanges Model { get; }

    public TreeViewModel IncludedTreeViewModel
    {
        get => _includedTreeViewModel;
        set
        {
            _includedTreeViewModel = value;
            RaisePropertyChanged(() => IncludedTreeViewModel);
        }
    }

    public TreeViewModel ExcludedTreeViewModel
    {
        get => _excludedTreeViewModel;
        set
        {
            _excludedTreeViewModel = value;
            RaisePropertyChanged(() => ExcludedTreeViewModel);
        }
    }

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
        Model.RepositoryOpened += (s, e) => InitTrees();
        Model.StatusUpdated += (s, e) => UpdateIsAnyFilesIncludedExcluded();
        _includedTreeViewModel = new TreeViewModel { TreeRoot = Model.Included };
        _excludedTreeViewModel = new TreeViewModel { TreeRoot = Model.Excluded };
        LoadedCommand = new ActionCommand(Model.Activate);
        UnloadedCommand = new ActionCommand(Model.Deactivate);
        CreateRepositoryCommand = new ActionCommand(Model.CreateRepository);
        IncludeToCommitCommand = new ActionCommand(IncludeToCommit);
        ExcludeFromCommitCommand = new ActionCommand(ExcludeFromCommit);
        MakeCommitCommand = new ActionCommand(MakeCommit);
    }

    private void IncludeToCommit()
    {
        var selected = ExcludedTreeViewModel.TreeRoot!.AllChildren.Where(x => x.IsSelected);
        Model.IncludeToCommit(selected);
        UpdateIsAnyFilesIncludedExcluded();
    }

    private void ExcludeFromCommit()
    {
        var selected = IncludedTreeViewModel.TreeRoot!.AllChildren.Where(x => x.IsSelected);
        Model.ExcludeFromCommit(selected);
        UpdateIsAnyFilesIncludedExcluded();
    }

    private void MakeCommit()
    {
        Model.MakeCommit();
        UpdateIsAnyFilesIncludedExcluded();
    }

    private void InitTrees()
    {
        IncludedTreeViewModel = new TreeViewModel { TreeRoot = Model.Included };
        ExcludedTreeViewModel = new TreeViewModel { TreeRoot = Model.Excluded };
        UpdateIsAnyFilesIncludedExcluded();
    }

    private void UpdateIsAnyFilesIncludedExcluded()
    {
        IsAnyFilesIncluded = Model.Included.Children.Any();
        IsAnyFilesExcluded = Model.Excluded.Children.Any();
    }
}
