using System.Linq;
using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Common;
using Luna.IDE.Configuration;
using Luna.IDE.ProjectExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

public class RecentProjectsViewModel : NotificationObject
{
    private bool _isVisible;

    public IRecentProjects Model { get; }

    public ICommand OpenProjectCommand { get; }

    public bool IsVisible
    {
        get => _isVisible;
        set { _isVisible = value; RaisePropertyChanged(() => IsVisible); }
    }

    public RecentProjectsViewModel(
        IRecentProjects recentProjects,
        IProjectLoader projectLoader,
        IEnvironmentWindowsManager windowsManager)
    {
        Model = recentProjects;
        OpenProjectCommand = new ActionCommand<RecentProject>(rp => projectLoader.OpenProject(rp.FullPath));
        IsVisible = Model.AnyProjects;
        windowsManager.WindowOpened += (s, e) => IsVisible = false;
        windowsManager.WindowClosed += (s, e) => IsVisible = !windowsManager.Windows.Any() && Model.AnyProjects;
    }
}
