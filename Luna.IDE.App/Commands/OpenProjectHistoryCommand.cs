using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.HistoryExploration;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Commands;

public interface IOpenProjectHistoryCommand : ICommand { }

public class OpenProjectHistoryCommand : Command, IOpenProjectHistoryCommand
{
    private const string WindowId = "ProjectHistoryId";

    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IEnvironmentWindowsFactory _environmentWindowsFactory;

    public OpenProjectHistoryCommand(IEnvironmentWindowsManager windowsManager, IEnvironmentWindowsFactory environmentWindowsFactory)
    {
        _windowsManager = windowsManager;
        _environmentWindowsFactory = environmentWindowsFactory;
    }

    public override void Execute(object? parameter)
    {
        var window = _windowsManager.FindWindowById(WindowId);
        if (window is null)
        {
            var components = _environmentWindowsFactory.MakeWindowFor(typeof(ProjectHistory));
            window = _windowsManager.OpenWindow(WindowId, components.Model, components.View);
        }
        _windowsManager.ActivateWindow(window);
    }
}
