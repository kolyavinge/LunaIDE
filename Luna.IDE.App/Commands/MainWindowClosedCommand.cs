using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.App.Commands;

public interface IMainWindowClosedCommand : ICommand { }

public class MainWindowClosedCommand : Command, IMainWindowClosedCommand
{
    private readonly IProjectLoader _projectLoader;

    public MainWindowClosedCommand(IProjectLoader projectLoader)
    {
        _projectLoader = projectLoader;
    }

    public override void Execute(object? parameter)
    {
        _projectLoader.CloseCurrentProject();
    }
}
