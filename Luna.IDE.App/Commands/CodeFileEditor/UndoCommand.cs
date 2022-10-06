using System.Windows.Input;
using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IUndoCommand : ICommand { }

public class UndoCommand : ActionCommand<ICodeFileEditor>, IUndoCommand
{
    public UndoCommand() : base(editor => editor.Undo())
    {
    }
}
