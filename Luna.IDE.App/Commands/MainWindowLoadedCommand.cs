using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Commands;

public interface IMainWindowLoadedCommand : ICommand { }

public class MainWindowLoadedCommand : Command, IMainWindowLoadedCommand
{
    private readonly IEnvironmentWindowsInitializer _environmentWindowsInitializer;

    public MainWindowLoadedCommand(IEnvironmentWindowsInitializer environmentWindowsInitializer)
    {
        _environmentWindowsInitializer = environmentWindowsInitializer;
    }

    public override void Execute(object? parameter)
    {
        _environmentWindowsInitializer.Init();
    }
}
