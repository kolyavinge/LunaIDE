using System.Windows.Input;
using Luna.Formatting;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;
using Luna.Parsing;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IFormatCodeCommand : ICommand { }

public class FormatCodeCommand : Command<ICodeFileEditor>, IFormatCodeCommand
{
    protected override void Execute(ICodeFileEditor editor)
    {
        var scanner = new Scanner();
        var tokens = scanner.GetTokens(new TextIterator(new Text(editor.Text)));
        var formatter = new CodeFormatter(tokens);
        var formatted = formatter.Format();
        var cursorPosition = editor.CursorPosition;
        var verticalScrollBarValue = editor.Viewport.VerticalScrollBarValue;
        editor.Text = formatted;
        editor.Viewport.VerticalScrollBarValue = verticalScrollBarValue;
        editor.MoveCursorTo(cursorPosition);
    }
}
