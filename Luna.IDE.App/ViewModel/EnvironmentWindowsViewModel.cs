using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Common;
using Luna.IDE.Model;

namespace Luna.IDE.App.ViewModel;

public class EnvironmentWindowsViewModel : NotificationObject
{
    public IEnvironmentWindowsManager WindowsManager { get; }

    public ICommand CloseWindowCommand { get; }

    public EnvironmentWindowsViewModel(IEnvironmentWindowsManager windowsManager)
    {
        WindowsManager = windowsManager;
        CloseWindowCommand = new ActionCommand<EnvironmentWindow>(CloseWindow);
    }

    private void CloseWindow(EnvironmentWindow window)
    {
        WindowsManager.CloseWindow(window);
    }
}
