using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IPasteCommand : ICommand { }

public class PasteCommand : ActionCommand<ICodeFileEditor>, IPasteCommand
{
    public PasteCommand() : base(editor => editor.Paste())
    {
    }
}
