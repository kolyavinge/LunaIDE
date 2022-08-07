using System;
using System.Linq;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using Luna.IDE.CodeEditor;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }
    CodeTextBoxModel CodeTextBoxModel { get; }
    void NavigateTo(CodeElement codeElement);
    void ReplaceText(CursorPosition start, CursorPosition end, string text);
}

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly ILunaCodeProvider _codeProvider;

    public CodeFileProjectItem ProjectItem { get; }

    public CodeTextBoxModel CodeTextBoxModel { get; }

    public string Header => ProjectItem.Name;

    public CodeFileEditor(CodeFileProjectItem projectItem, ICodeProviderFactory codeProviderFactory, ICodeModelUpdater codeModelUpdater)
    {
        ProjectItem = projectItem;
        _codeModelUpdater = codeModelUpdater;
        _codeModelUpdater.Attach(ProjectItem, OnCodeModelUpdated);
        _codeProvider = codeProviderFactory.Make(projectItem);
        CodeTextBoxModel = new CodeTextBoxModel(_codeProvider, new() { HighlighteredBrackets = "()" });
        CodeTextBoxModel.TextChanged += OnTextChanged;
        CodeTextBoxModel.SetText(ProjectItem.GetText());
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(CodeTextBoxModel));
    }

    public void OnCodeModelUpdated(CodeModelUpdatedEventArgs e)
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

    public void Save()
    {
        ProjectItem.SaveText(CodeTextBoxModel.Text.ToString());
    }

    public void Close()
    {
        ProjectItem.ResetTextGettingStrategy();
        _codeModelUpdater.Detach(ProjectItem);
    }

    public void NavigateTo(CodeElement codeElement)
    {
        CodeTextBoxModel.GotoLine(codeElement.LineIndex);
    }

    public void ReplaceText(CursorPosition start, CursorPosition end, string text)
    {
        CodeTextBoxModel.TextSelection.Set(start, end);
        CodeTextBoxModel.InsertText(text);
    }
}

class EditorTextGettingStrategy : TextFileProjectItem.ITextGettingStrategy
{
    private readonly CodeTextBoxModel _model;

    public EditorTextGettingStrategy(CodeTextBoxModel model)
    {
        _model = model;
    }

    public string GetText()
    {
        return _model.Text.ToString();
    }
}
