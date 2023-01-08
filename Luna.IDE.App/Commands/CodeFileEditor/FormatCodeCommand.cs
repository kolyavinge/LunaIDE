using System.Windows.Input;
using Luna.Formatting;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IFormatCodeCommand : ICommand { }

public class FormatCodeCommand : Command<ICodeFileEditor>, IFormatCodeCommand
{
    private readonly ICodeFormatterFactory _codeFormatterFactory;

    public FormatCodeCommand(ICodeFormatterFactory codeFormatterFactory)
    {
        _codeFormatterFactory = codeFormatterFactory;
    }

    protected override void Execute(ICodeFileEditor editor)
    {
        var formatter = _codeFormatterFactory.Make();
        var formatted = formatter.Format(editor.Text);
        var cursorPosition = editor.CursorPosition;
        var verticalScrollBarValue = editor.Viewport.VerticalScrollBarValue;
        editor.Text = formatted;
        editor.Viewport.VerticalScrollBarValue = verticalScrollBarValue;
        editor.MoveCursorTo(cursorPosition);
    }
}
