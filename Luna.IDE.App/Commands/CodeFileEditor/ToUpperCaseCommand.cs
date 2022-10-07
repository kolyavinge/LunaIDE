using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IToUpperCaseCommand : ICommand { }

public class ToUpperCaseCommand : ActionCommand<ICodeFileEditor>, IToUpperCaseCommand
{
    public ToUpperCaseCommand() : base(editor => editor.ToUpperCase())
    {
    }
}
