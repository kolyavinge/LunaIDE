using CodeHighlighter;
using CodeHighlighter.Ancillary;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Core;
using CodeHighlighter.Model;
using Luna.IDE.Common;

namespace Luna.IDE.TextDiff;

public interface IDiffCodeTextBox
{
    ICodeTextBox? CodeTextBox { get; }
    ILinesDecorationCollection LinesDecoration { get; }
    ILineGapCollection Gaps { get; }
    IViewport Viewport { get; }
    void Init(ICodeProvider codeProvider, string text);
    void GotoLine(int lineIndex);
}

internal class DiffCodeTextBox : NotificationObject, IDiffCodeTextBox
{
    private ICodeTextBox? _codeTextBox;

    public ICodeTextBox? CodeTextBox
    {
        get => _codeTextBox;
        private set { _codeTextBox = value; RaisePropertyChanged(() => CodeTextBox!); }
    }

    public ILinesDecorationCollection LinesDecoration => CodeTextBox!.LinesDecoration;

    public ILineGapCollection Gaps => CodeTextBox!.Gaps;

    public IViewport Viewport => CodeTextBox!.Viewport;

    public void Init(ICodeProvider codeProvider, string text)
    {
        CodeTextBox = CodeTextBoxFactory.MakeModel(codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBox.IsReadOnly = false;
        CodeTextBox.Text = text;
        CodeTextBox.IsReadOnly = true;
    }

    public void GotoLine(int lineIndex)
    {
        CodeTextBox!.GotoLine(lineIndex);
    }
}
