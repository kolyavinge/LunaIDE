using System.Windows.Input;
using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IToLowerCaseCommand : ICommand { }

public class ToLowerCaseCommand : ActionCommand<ICodeFileEditor>, IToLowerCaseCommand
{
    public ToLowerCaseCommand() : base(editor => editor.ToLowerCase())
    {
    }
}
