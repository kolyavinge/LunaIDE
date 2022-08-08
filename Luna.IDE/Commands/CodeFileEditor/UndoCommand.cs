using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands.CodeFileEditor;

public interface IUndoCommand : ICommand { }

public class UndoCommand : ActionCommand<ICodeFileEditor>, IUndoCommand
{
    public UndoCommand() : base(editor => editor.Undo())
    {
    }
}
