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
    private readonly IInterpreter _interpreter;
    private readonly ISelectedProject _selectedProject;
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IOutputArea _outputArea;

    public RunProgramCommand(IInterpreter interpreter, ISelectedProject selectedProject, IEnvironmentWindowsManager windowsManager, IOutputArea outputArea)
    {
        _interpreter = interpreter;
        _selectedProject = selectedProject;
        _windowsManager = windowsManager;
        _outputArea = outputArea;
    }

    public override void Execute(object parameter)
    {
        if (_selectedProject.Project == null) return;
        _windowsManager.Windows.Each(x => x.Model.Save());
        _outputArea.Clear();
        _interpreter.Run(_selectedProject.Project, _outputArea);
    }
}
