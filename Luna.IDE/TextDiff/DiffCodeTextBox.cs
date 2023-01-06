﻿using CodeHighlighter;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using Luna.IDE.Common;

namespace Luna.IDE.TextDiff;

public interface IDiffCodeTextBox
{
    ILinesDecorationCollection LinesDecoration { get; }
    void Init(ICodeProvider codeProvider, string text);
    void GotoLine(int lineIndex);
}

public class DiffCodeTextBox : NotificationObject, IDiffCodeTextBox
{
    private ICodeTextBoxModel _codeTextBoxModel;

    public ICodeTextBoxModel CodeTextBoxModel
    {
        get => _codeTextBoxModel;
        private set { _codeTextBoxModel = value; RaisePropertyChanged(() => CodeTextBoxModel); }
    }

    public ILinesDecorationCollection LinesDecoration => _codeTextBoxModel.LinesDecoration;

    public DiffCodeTextBox()
    {
        _codeTextBoxModel = CodeTextBoxModelFactory.MakeModel(new EmptyCodeProvider());
    }

    public void Init(ICodeProvider codeProvider, string text)
    {
        CodeTextBoxModel = CodeTextBoxModelFactory.MakeModel(codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBoxModel.IsReadOnly = false;
        CodeTextBoxModel.Text = text;
        CodeTextBoxModel.IsReadOnly = true;
    }

    public void GotoLine(int lineIndex)
    {
        CodeTextBoxModel.GotoLine(lineIndex);
    }
}