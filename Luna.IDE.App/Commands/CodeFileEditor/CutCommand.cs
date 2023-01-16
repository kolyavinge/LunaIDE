using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface ICutCommand : ICommand { }

public class CutCommand : ActionCommand<ICodeFileEditor>, ICutCommand
{
    public CutCommand() : base(editor => editor.Cut())
    {
    }
}
