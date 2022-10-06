using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Model;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IRedoCommand : ICommand { }

public class RedoCommand : ActionCommand<ICodeFileEditor>, IRedoCommand
{
    public RedoCommand() : base(editor => editor.Redo())
    {
    }
}
