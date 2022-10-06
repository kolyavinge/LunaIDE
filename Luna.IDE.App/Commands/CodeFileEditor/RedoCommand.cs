using System.Windows.Input;
using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IRedoCommand : ICommand { }

public class RedoCommand : ActionCommand<ICodeFileEditor>, IRedoCommand
{
    public RedoCommand() : base(editor => editor.Redo())
    {
    }
}
