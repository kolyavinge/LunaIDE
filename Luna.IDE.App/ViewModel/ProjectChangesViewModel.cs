using System.Linq;
using System.Windows.Input;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Media;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Common;
using Luna.IDE.ProjectChanging;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

[EnvironmentWindowFor(typeof(ProjectChanges))]
public class ProjectChangesViewModel : NotificationObject
{
    private bool _isAnyFilesIncluded;
    private bool _isAnyFilesExcluded;

    public IProjectChanges Model { get; }

    [Inject]
    public IImageCollection? ImageCollection { get; set; }

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

    public ICommand UndoChangesCommand { get; }

    [Inject]
    public IVersionedFilesChangesCommand? VersionedFilesChangesCommand { get; set; }

    public ProjectChangesViewModel(IProjectChanges projectChanges)
    {
        Model = projectChanges;
        LoadedCommand = new ActionCommand(Model.Activate);
        UnloadedCommand = new ActionCommand(Model.Deactivate);
        CreateRepositoryCommand = new ActionCommand(Model.CreateRepository);
        IncludeToCommitCommand = new ActionCommand(IncludeToCommit);
        ExcludeFromCommitCommand = new ActionCommand(ExcludeFromCommit);
        MakeCommitCommand = new ActionCommand(Model.MakeCommit);
        UndoChangesCommand = new ActionCommand(UndoChanges);
    }

    private void IncludeToCommit()
    {
        Model.IncludeToCommit(Model.SelectedExcludedVersionedFiles);
    }

    private void ExcludeFromCommit()
    {
        Model.ExcludeFromCommit(Model.SelectedIncludedVersionedFiles);
    }

    private void UndoChanges()
    {
        Model.UndoChanges(Model.SelectedIncludedVersionedFiles.Union(Model.SelectedExcludedVersionedFiles).ToList());
    }
}
