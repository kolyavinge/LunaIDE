using System.Windows.Input;
using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;
using Luna.Runtime;
using Luna.Utils;

namespace Luna.IDE.App.Commands;

public interface IRunProgramCommand : ICommand { }

public class RunProgramCommand : Command, IRunProgramCommand
{
    private readonly IInterpreter _interpreter;
    private readonly IProjectExplorer _projectExplorer;
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IOutputArea _outputArea;

    public RunProgramCommand(IInterpreter interpreter, IProjectExplorer projectExplorer, IEnvironmentWindowsManager windowsManager, IOutputArea outputArea)
    {
        _interpreter = interpreter;
        _projectExplorer = projectExplorer;
        _windowsManager = windowsManager;
        _outputArea = outputArea;
    }

    public override void Execute(object parameter)
    {
        if (_projectExplorer.Project == null) return;
        _windowsManager.Windows.Each(x => x.Model.Save());
        _outputArea.Clear();
        _interpreter.Run(_projectExplorer.Project, _outputArea);
    }
}
