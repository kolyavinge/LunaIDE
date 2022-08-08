using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands.CodeFileEditor;

public interface IToLowerCaseCommand : ICommand { }

public class ToLowerCaseCommand : ActionCommand<ICodeFileEditor>, IToLowerCaseCommand
{
    public ToLowerCaseCommand() : base(editor => editor.ToLowerCase())
    {
    }
}
