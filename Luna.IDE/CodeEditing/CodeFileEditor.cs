using CodeHighlighter;
using CodeHighlighter.Common;
using CodeHighlighter.Core;
using CodeHighlighter.Model;
using Luna.CodeElements;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

[EditorFor(typeof(CodeFileProjectItem))]
internal class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel, ISaveableEnvironmentWindow, ICloseableEnvironmentWindow
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly ITokenKindsUpdater _tokenKindsUpdater;
    private readonly IFoldableRegionsUpdater _foldableRegionsUpdater;
    private readonly ILunaCodeProvider _codeProvider;

    public CodeFileProjectItem ProjectItem { get; }

    public ICodeTextBox CodeTextBox { get; }

    public ILineNumberPanel LineNumberPanel { get; }

    public ILineFoldingPanel LineFoldingPanel { get; }

    public ISearchPanel SearchPanel { get; }

    public string Text { get => CodeTextBox.Text; set => CodeTextBox.Text = value; }

    public CursorPosition CursorPosition => CodeTextBox.CursorPosition;

    public IViewport Viewport => CodeTextBox.Viewport;

    public ITextMeasures TextMeasures => CodeTextBox.TextMeasures;

    public ITextEvents TextEvents => CodeTextBox.TextEvents;

    public ITokens Tokens => CodeTextBox.Tokens;

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
        CodeTextBox = CodeTextBoxFactory.MakeModel(_codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBox.TextEvents.TextChanged += OnTextChanged;
        CodeTextBox.Text = ProjectItem.GetText();
        LineNumberPanel = LineNumberPanelFactory.MakeModel(CodeTextBox);
        LineFoldingPanel = LineFoldingPanelFactory.MakeModel(CodeTextBox);
        SearchPanel = SearchPanelFactory.MakeModel(CodeTextBox);
        SearchPanel.HighlightColor = Color.FromHex("9c5500");
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(CodeTextBox));
        _foldableRegionsUpdater = foldableRegionsUpdaterFactory.Make(CodeTextBox.Folds, CodeTextBox.Tokens);
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

    public void Save() => ProjectItem.SaveText(CodeTextBox.Text.ToString());

    public void Close()
    {
        ProjectItem.ResetTextGettingStrategy();
        ProjectItem.CodeModelUpdated -= OnCodeModelUpdated;
    }

    public TokenCursorPosition? GetTokenCursorPosition() => CodeTextBox.Tokens.GetTokenOnPosition(CodeTextBox.CursorPosition);

    public bool Focus() => CodeTextBox.Focus();

    public void NavigateTo(CodeElement codeElement) => CodeTextBox.GotoLine(codeElement.LineIndex);

    public void ReplaceText(CursorPosition start, CursorPosition end, string text)
    {
        CodeTextBox.MoveCursorTo(start);
        CodeTextBox.ActivateSelection();
        CodeTextBox.MoveCursorTo(end);
        CodeTextBox.CompleteSelection();
        CodeTextBox.InsertText(text);
    }

    public void UndoTextChanges()
    {
        ProjectItem.ResetTextGettingStrategy();
        CodeTextBox.Text = ProjectItem.GetText();
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(CodeTextBox));
        _codeModelUpdater.Request();
    }

    public void DeleteSelectedLines() => CodeTextBox.DeleteSelectedLines();

    public void ToLowerCase() => CodeTextBox.SetTextCase(TextCase.Lower);

    public void ToUpperCase() => CodeTextBox.SetTextCase(TextCase.Upper);

    public void MoveSelectedLinesUp() => CodeTextBox.MoveSelectedLinesUp();

    public void MoveSelectedLinesDown() => CodeTextBox.MoveSelectedLinesDown();

    public void MoveCursorTo(CursorPosition position) => CodeTextBox.MoveCursorTo(position);

    public void Copy() => CodeTextBox.Copy();

    public void Paste() => CodeTextBox.Paste();

    public void Cut() => CodeTextBox.Cut();

    public void Undo() => CodeTextBox.History.Undo();

    public void Redo() => CodeTextBox.History.Redo();

    public void ActivateSearchPattern() => SearchPanel.ActivatePattern();
}

class EditorTextGettingStrategy : TextFileProjectItem.ITextGettingStrategy
{
    private readonly ICodeTextBox _model;

    public EditorTextGettingStrategy(ICodeTextBox model)
    {
        _model = model;
    }

    public string GetText()
    {
        return _model.Text.ToString();
    }
}
