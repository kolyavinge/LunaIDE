using CodeHighlighter.Core;
using Luna.IDE.CodeEditing;
using Luna.ProjectModel;

namespace Luna.IDE.AutoCompletion;

public interface IAutoCompleteDataContext
{
    event EventHandler TextChanged;
    CodeModel CodeModel { get; }
    CursorPosition CursorPosition { get; }
    double TextLetterWidth { get; }
    double TextLineHeight { get; }
    Token? GetTokenOnCursorPosition();
    void ReplaceText(CursorPosition start, CursorPosition end, string text);
}

internal class AutoCompleteDataContext : IAutoCompleteDataContext
{
    private readonly ICodeFileEditor _editor;

    public event EventHandler? TextChanged;
    public CodeModel CodeModel => _editor.ProjectItem.CodeModel;
    public CursorPosition CursorPosition => _editor.CursorPosition;
    public double TextLetterWidth => _editor.TextMeasures.LetterWidth;
    public double TextLineHeight => _editor.TextMeasures.LineHeight;

    public AutoCompleteDataContext(ICodeFileEditor editor)
    {
        _editor = editor;
        _editor.TextEvents.TextChanged += (s, e) => TextChanged?.Invoke(s, e);
    }

    public Token? GetTokenOnCursorPosition()
    {
        var position = _editor.Tokens.GetTokenOnPosition(CursorPosition);
        return position?.TokenOnPosition;
    }

    public void ReplaceText(CursorPosition start, CursorPosition end, string text)
    {
        _editor.ReplaceText(start, end, text);
    }
}
