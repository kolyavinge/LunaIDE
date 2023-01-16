using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface ICopyCommand : ICommand { }

public class CopyCommand : ActionCommand<ICodeFileEditor>, ICopyCommand
{
    public CopyCommand() : base(editor => editor.Copy())
    {
    }
}
