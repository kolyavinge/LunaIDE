using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IToLowerCaseCommand : ICommand { }

public class ToLowerCaseCommand : ActionCommand<ICodeFileEditor>, IToLowerCaseCommand
{
    public ToLowerCaseCommand() : base(editor => editor.ToLowerCase())
    {
    }
}
