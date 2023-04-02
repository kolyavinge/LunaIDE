﻿using CodeHighlighter.Core;
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

public class AutoCompleteDataContext : IAutoCompleteDataContext
{
    private readonly ICodeFileEditor _model;

    public event EventHandler? TextChanged;
    public CodeModel CodeModel => _model.ProjectItem.CodeModel;
    public CursorPosition CursorPosition => _model.CursorPosition;
    public double TextLetterWidth => _model.TextMeasures.LetterWidth;
    public double TextLineHeight => _model.TextMeasures.LineHeight;

    public AutoCompleteDataContext(ICodeFileEditor model)
    {
        _model = model;
        _model.TextEvents.TextChanged += (s, e) => TextChanged?.Invoke(s, e);
    }

    public Token? GetTokenOnCursorPosition()
    {
        var position = _model.Tokens.GetTokenOnPosition(CursorPosition);
        return position?.TokenOnPosition;
    }

    public void ReplaceText(CursorPosition start, CursorPosition end, string text)
    {
        _model.ReplaceText(start, end, text);
    }
}
