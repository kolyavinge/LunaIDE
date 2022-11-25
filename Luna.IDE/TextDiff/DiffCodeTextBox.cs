using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using Luna.IDE.Common;

namespace Luna.IDE.TextDiff;

public interface IDiffCodeTextBox
{
    LinesDecorationCollection LinesDecoration { get; }
    void Init(ICodeProvider codeProvider, string text);
    void GotoLine(int lineIndex);
}

public class DiffCodeTextBox : NotificationObject, IDiffCodeTextBox
{
    private CodeTextBoxModel _codeTextBoxModel;

    public CodeTextBoxModel CodeTextBoxModel
    {
        get => _codeTextBoxModel;
        private set { _codeTextBoxModel = value; RaisePropertyChanged(() => CodeTextBoxModel); }
    }

    public LinesDecorationCollection LinesDecoration => _codeTextBoxModel.LinesDecoration;

    public DiffCodeTextBox()
    {
        _codeTextBoxModel = new CodeTextBoxModel();
    }

    public void Init(ICodeProvider codeProvider, string text)
    {
        CodeTextBoxModel = new CodeTextBoxModel(codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBoxModel.IsReadOnly = false;
        CodeTextBoxModel.SetText(text);
        CodeTextBoxModel.IsReadOnly = true;
    }

    public void GotoLine(int lineIndex)
    {
        CodeTextBoxModel.GotoLine(lineIndex);
    }
}
