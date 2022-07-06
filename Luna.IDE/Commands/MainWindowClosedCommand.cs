using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands;

public interface IMainWindowClosedCommand : ICommand { }

public class MainWindowClosedCommand : Command, IMainWindowClosedCommand
{
    private readonly IEnvironmentWindowsManager _windowsManager;

    public MainWindowClosedCommand(IEnvironmentWindowsManager windowsManager)
    {
        _windowsManager = windowsManager;
    }

    public override void Execute(object parameter)
    {
        _windowsManager.CloseAllWindows();
    }
}
