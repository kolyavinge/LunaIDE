using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Common;
using Luna.IDE.WindowsManagement;
using WindowsEnvironment.Model;

namespace Luna.IDE.App.ViewModel;

public class EnvironmentWindowsViewModel : NotificationObject
{
    public IFlexWindowsEnvironment FlexEnvironment { get; }
    public IEnvironmentWindowsManager WindowsManager { get; }
    public ICommand CloseWindowCommand { get; }

    public EnvironmentWindowsViewModel(
        IFlexWindowsEnvironment flexEnvironment,
        IEnvironmentWindowsManager windowsManager)
    {
        FlexEnvironment = flexEnvironment;
        WindowsManager = windowsManager;
        CloseWindowCommand = new ActionCommand<EnvironmentWindow>(CloseWindow);
    }

    private void CloseWindow(EnvironmentWindow window)
    {
        WindowsManager.CloseWindow(window);
    }
}
