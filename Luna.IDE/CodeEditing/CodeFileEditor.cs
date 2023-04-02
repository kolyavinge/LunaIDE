using CodeHighlighter;
using CodeHighlighter.Model;
using CodeHighlighter.Core;
using Luna.CodeElements;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel, ISaveableEnvironmentWindow, ICloseableEnvironmentWindow
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly ITokenKindsUpdater _tokenKindsUpdater;
    private readonly IFoldableRegionsUpdater _foldableRegionsUpdater;
    private readonly ILunaCodeProvider _codeProvider;

    public CodeFileProjectItem ProjectItem { get; }

    public ICodeTextBoxModel CodeTextBoxModel { get; }

    public ILineNumberPanelModel LineNumberPanelModel { get; }

    public ILineFoldingPanelModel LineFoldingPanelModel { get; }

    public string Text { get => CodeTextBoxModel.Text; set => CodeTextBoxModel.Text = value; }

    public CursorPosition CursorPosition => CodeTextBoxModel.CursorPosition;

    public IViewport Viewport => CodeTextBoxModel.Viewport;

    public ITextMeasures TextMeasures => CodeTextBoxModel.TextMeasures;

    public ITextEvents TextEvents => CodeTextBoxModel.TextEvents;

    public ITokens Tokens => CodeTextBoxModel.Tokens;

    public string Header => ProjectItem.Name;

    public CodeFileEditor(
        CodeFileProjectItem projectItem,
        ICodeProviderFactory codeProviderFactory,
        ICodeModelUpdater codeModelUpdater,
        ITokenKindsUpdater tokenKindsUpdater,
        IFoldableRegionsUpdaterFactory foldableRegionsUpdaterFactory)
    {
        ProjectItem = projectItem;
        ProjectItem.CodeModelUpdated += OnCodeModelUpdated;
        _codeModelUpdater = codeModelUpdater;
        _tokenKindsUpdater = tokenKindsUpdater;
        _codeProvider = (ILunaCodeProvider)codeProviderFactory.Make(projectItem);
        CodeTextBoxModel = CodeTextBoxModelFactory.MakeModel(_codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBoxModel.TextEvents.TextChanged += OnTextChanged;
        CodeTextBoxModel.Text = ProjectItem.GetText();
        LineNumberPanelModel = LineNumberPanelModelFactory.MakeModel(CodeTextBoxModel);
        LineFoldingPanelModel = LineFoldingPanelModelFactory.MakeModel(CodeTextBoxModel);
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(CodeTextBoxModel));
        _foldableRegionsUpdater = foldableRegionsUpdaterFactory.Make(CodeTextBoxModel.Folds, CodeTextBoxModel.Tokens);
        _foldableRegionsUpdater.Request();
    }

    private void OnCodeModelUpdated(object? sender, CodeModelUpdatedEventArgs e)
    {
        _tokenKindsUpdater.Update(e.Different, _codeProvider);
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        _codeModelUpdater.Request();
        _foldableRegionsUpdater.Request();
    }

    public void Save() => ProjectItem.SaveText(CodeTextBoxModel.Text.ToString());

    public void Close()
    {
        ProjectItem.ResetTextGettingStrategy();
        ProjectItem.CodeModelUpdated -= OnCodeModelUpdated;
    }

    public TokenCursorPosition? GetTokenCursorPosition() => CodeTextBoxModel.Tokens.GetTokenOnPosition(CodeTextBoxModel.CursorPosition);

    public void NavigateTo(CodeElement codeElement) => CodeTextBoxModel.GotoLine(codeElement.LineIndex);

    public void ReplaceText(CursorPosition start, CursorPosition end, string text)
    {
        CodeTextBoxModel.MoveCursorTo(start);
        CodeTextBoxModel.ActivateSelection();
        CodeTextBoxModel.MoveCursorTo(end);
        CodeTextBoxModel.CompleteSelection();
        CodeTextBoxModel.InsertText(text);
    }

    public void UndoTextChanges()
    {
        ProjectItem.ResetTextGettingStrategy();
        CodeTextBoxModel.Text = ProjectItem.GetText();
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(CodeTextBoxModel));
        _codeModelUpdater.Request();
    }

    public void DeleteSelectedLines() => CodeTextBoxModel.DeleteSelectedLines();

    public void ToLowerCase() => CodeTextBoxModel.SetTextCase(TextCase.Lower);

    public void ToUpperCase() => CodeTextBoxModel.SetTextCase(TextCase.Upper);

    public void MoveSelectedLinesUp() => CodeTextBoxModel.MoveSelectedLinesUp();

    public void MoveSelectedLinesDown() => CodeTextBoxModel.MoveSelectedLinesDown();

    public void MoveCursorTo(CursorPosition position) => CodeTextBoxModel.MoveCursorTo(position);

    public void Copy() => CodeTextBoxModel.Copy();

    public void Paste() => CodeTextBoxModel.Paste();

    public void Cut() => CodeTextBoxModel.Cut();

    public void Undo() => CodeTextBoxModel.History.Undo();

    public void Redo() => CodeTextBoxModel.History.Redo();
}

class EditorTextGettingStrategy : TextFileProjectItem.ITextGettingStrategy
{
    private readonly ICodeTextBoxModel _model;

    public EditorTextGettingStrategy(ICodeTextBoxModel model)
    {
        _model = model;
    }

    public string GetText()
    {
        return _model.Text.ToString();
    }
}
