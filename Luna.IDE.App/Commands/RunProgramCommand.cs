using System.Linq;
using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Outputing;
using Luna.IDE.ProjectExploration;
using Luna.IDE.WindowsManagement;
using Luna.Runtime;
using Luna.Utils;

namespace Luna.IDE.App.Commands;

public interface IRunProgramCommand : ICommand { }

public class RunProgramCommand : Command, IRunProgramCommand
{
    private readonly IInterpreterFactory _interpreterFactory;
    private readonly ISelectedProject _selectedProject;
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IOutputArea _outputArea;

    public RunProgramCommand(
        IInterpreterFactory interpreterFactory, ISelectedProject selectedProject, IEnvironmentWindowsManager windowsManager, IOutputArea outputArea)
    {
        _interpreterFactory = interpreterFactory;
        _selectedProject = selectedProject;
        _windowsManager = windowsManager;
        _outputArea = outputArea;
    }

    public override void Execute(object? parameter)
    {
        if (_selectedProject.Project is null) return;
        _windowsManager.Windows.Where(x => x.Model is ISaveableEnvironmentWindow).Each(x => ((ISaveableEnvironmentWindow)x.Model).Save());
        _outputArea.Clear();
        var interpreter = _interpreterFactory.Make(_selectedProject.Project, _outputArea);
        interpreter.Run();
    }
}
