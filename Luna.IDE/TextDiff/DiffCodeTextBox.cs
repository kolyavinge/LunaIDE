﻿using CodeHighlighter;
using CodeHighlighter.Ancillary;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Core;
using CodeHighlighter.Model;
using Luna.IDE.Common;

namespace Luna.IDE.TextDiff;

public interface IDiffCodeTextBox
{
    ICodeTextBoxModel? CodeTextBoxModel { get; }
    ILinesDecorationCollection LinesDecoration { get; }
    ILineGapCollection Gaps { get; }
    IViewport Viewport { get; }
    void Init(ICodeProvider codeProvider, string text);
    void GotoLine(int lineIndex);
}

public class DiffCodeTextBox : NotificationObject, IDiffCodeTextBox
{
    private ICodeTextBoxModel? _codeTextBoxModel;

    public ICodeTextBoxModel? CodeTextBoxModel
    {
        get => _codeTextBoxModel;
        private set { _codeTextBoxModel = value; RaisePropertyChanged(() => CodeTextBoxModel!); }
    }

    public ILinesDecorationCollection LinesDecoration => CodeTextBoxModel!.LinesDecoration;

    public ILineGapCollection Gaps => CodeTextBoxModel!.Gaps;

    public IViewport Viewport => CodeTextBoxModel!.Viewport;

    public void Init(ICodeProvider codeProvider, string text)
    {
        CodeTextBoxModel = CodeTextBoxModelFactory.MakeModel(codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBoxModel.IsReadOnly = false;
        CodeTextBoxModel.Text = text;
        CodeTextBoxModel.IsReadOnly = true;
    }

    public void GotoLine(int lineIndex)
    {
        CodeTextBoxModel!.GotoLine(lineIndex);
    }
}
