using System.Linq;
using CodeHighlighter;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using Luna.CodeElements;
using Luna.IDE.WindowsManagement;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel, ISaveableEnvironmentWindow, ICloseableEnvironmentWindow
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly ILunaCodeProvider _codeProvider;

    public CodeFileProjectItem ProjectItem { get; }

    public ICodeTextBoxModel CodeTextBoxModel { get; }

    public CursorPosition CursorPosition => CodeTextBoxModel.CursorPosition;

    public string Header => ProjectItem.Name;

    public CodeFileEditor(CodeFileProjectItem projectItem, ICodeProviderFactory codeProviderFactory, ICodeModelUpdater codeModelUpdater)
    {
        ProjectItem = projectItem;
        ProjectItem.CodeModelUpdated += OnCodeModelUpdated;
        _codeModelUpdater = codeModelUpdater;
        _codeProvider = (ILunaCodeProvider)codeProviderFactory.Make(projectItem);
        CodeTextBoxModel = CodeTextBoxModelFactory.MakeModel(_codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBoxModel.TextEvents.TextChanged += OnTextChanged;
        CodeTextBoxModel.Text = ProjectItem.GetText();
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(CodeTextBoxModel));
    }

    internal void OnCodeModelUpdated(object? sender, CodeModelUpdatedEventArgs e)
    {
        var diff = e.Different;

        var updatedTokens =
            diff.AddedDeclaredConstants.Concat(diff.AddedImportedConstants).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKindExtra.Constant)).
            Concat(diff.AddedDeclaredFunctions.Concat(diff.AddedImportedFunctions).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKindExtra.Function))).
            Concat(diff.RemovedDeclaredConstants.Concat(diff.RemovedImportedConstants).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKind.Identificator))).
            Concat(diff.RemovedDeclaredFunctions.Concat(diff.RemovedImportedFunctions).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKind.Identificator))).
            ToList();

        if (updatedTokens.Any())
        {
            _codeProvider.UpdateTokenKinds(updatedTokens);
        }
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        _codeModelUpdater.UpdateRequest();
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
        _codeModelUpdater.UpdateRequest();
    }

    public void DeleteSelectedLines() => CodeTextBoxModel.DeleteSelectedLines();

    public void ToLowerCase() => CodeTextBoxModel.SetTextCase(TextCase.Lower);

    public void ToUpperCase() => CodeTextBoxModel.SetTextCase(TextCase.Upper);

    public void MoveSelectedLinesUp() => CodeTextBoxModel.MoveSelectedLinesUp();

    public void MoveSelectedLinesDown() => CodeTextBoxModel.MoveSelectedLinesDown();

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
