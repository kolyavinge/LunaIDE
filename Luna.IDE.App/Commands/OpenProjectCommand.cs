using System.Windows.Input;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Mvvm;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.App.Commands;

public interface IOpenProjectCommand : ICommand { }

public class OpenProjectCommand : Command, IOpenProjectCommand
{
    private readonly IProjectLoader _projectLoader;
    private readonly IOpenFileDialog _openFileDialog;

    public OpenProjectCommand(IProjectLoader projectLoader, IOpenFileDialog openFileDialog)
    {
        _projectLoader = projectLoader;
        _openFileDialog = openFileDialog;
    }

    public override void Execute(object? parameter)
    {
        _openFileDialog.IsFolderPicker = true;
        if (_openFileDialog.ShowDialog() == DialogResult.Ok)
        {
            _projectLoader.OpenProject(_openFileDialog.SelectedPath!);
        }
    }
}
