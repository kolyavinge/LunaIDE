using System;
using CodeHighlighter.Model;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface IAutoCompleteDataContext
{
    event EventHandler TextChanged;
    CodeModel CodeModel { get; }
    (int, int) CursorPosition { get; }
    double TextLetterWidth { get; }
    double TextLineHeight { get; }
    Token? GetTokenOnPosition(int lineIndex, int columnIndex);
    void ReplaceText(int cursorStartLineIndex, int cursorStartColumnIndex, int cursorEndLineIndex, int cursorEndColumnIndex, string text);
}

public class AutoCompleteDataContext : IAutoCompleteDataContext
{
    private readonly ICodeFileEditor _model;

    public event EventHandler? TextChanged;
    public CodeModel CodeModel => _model.ProjectItem.CodeModel;
    public (int, int) CursorPosition => (_model.CodeTextBoxModel.TextCursor.LineIndex, _model.CodeTextBoxModel.TextCursor.ColumnIndex);
    public double TextLetterWidth => _model.CodeTextBoxModel.TextMeasures.LetterWidth;
    public double TextLineHeight => _model.CodeTextBoxModel.TextMeasures.LineHeight;

    public AutoCompleteDataContext(ICodeFileEditor model)
    {
        _model = model;
        _model.CodeTextBoxModel.TextChanged += (s, e) => TextChanged?.Invoke(s, e);
    }

    public Token? GetTokenOnPosition(int lineIndex, int columnIndex)
    {
        return _model.CodeTextBoxModel.Tokens.GetTokenOnPosition(lineIndex, columnIndex);
    }

    public void ReplaceText(int cursorStartLineIndex, int cursorStartColumnIndex, int cursorEndLineIndex, int cursorEndColumnIndex, string text)
    {
        _model.CodeTextBoxModel.ReplaceText(cursorStartLineIndex, cursorStartColumnIndex, cursorEndLineIndex, cursorEndColumnIndex, text);
    }
}
