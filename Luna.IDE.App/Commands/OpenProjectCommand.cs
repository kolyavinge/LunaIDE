using System.Windows.Input;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Model;

namespace Luna.IDE.App.Commands;

public interface IOpenProjectCommand : ICommand { }

public class OpenProjectCommand : Command, IOpenProjectCommand
{
    private readonly IProjectExplorer _projectExplorer;
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IOpenFileDialog _openFileDialog;

    public OpenProjectCommand(IProjectExplorer projectExplorer, IEnvironmentWindowsManager windowsManager, IOpenFileDialog openFileDialog)
    {
        _projectExplorer = projectExplorer;
        _windowsManager = windowsManager;
        _openFileDialog = openFileDialog;
    }

    public override void Execute(object parameter)
    {
        _openFileDialog.IsFolderPicker = true;
        if (_openFileDialog.ShowDialog() == DialogResult.Ok)
        {
            _windowsManager.CloseAllWindows();
            _projectExplorer.OpenProject(_openFileDialog.SelectedPath!);
        }
    }
}
