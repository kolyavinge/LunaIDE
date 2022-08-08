using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands.CodeFileEditor;

public interface IRedoCommand : ICommand { }

public class RedoCommand : ActionCommand<ICodeFileEditor>, IRedoCommand
{
    public RedoCommand() : base(editor => editor.Redo())
    {
    }
}
