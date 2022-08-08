using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands.CodeFileEditor;

public interface IToUpperCaseCommand : ICommand { }

public class ToUpperCaseCommand : ActionCommand<ICodeFileEditor>, IToUpperCaseCommand
{
    public ToUpperCaseCommand() : base(editor => editor.ToUpperCase())
    {
    }
}
