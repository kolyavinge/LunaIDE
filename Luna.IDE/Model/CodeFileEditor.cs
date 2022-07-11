using System.Linq;
using System.Windows.Input;
using CodeHighlighter;
using CodeHighlighter.Contracts;
using Luna.IDE.CodeEditor;
using Luna.IDE.Mvvm;
using Luna.IDE.Utils;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }
    CodeTextBoxModel CodeTextBoxModel { get; set; }
    void NavigateTo(CodeElement codeElement);
    void InsertText(string text);
}

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private CodeTextBoxModel? _codeTextBoxModel;

    public CodeFileProjectItem ProjectItem { get; }

    public CodeTextBoxCommands TextBoxCommands { get; }

    public CodeTextBoxModel CodeTextBoxModel
    {
        get => _codeTextBoxModel ?? throw new HasNotInitializedYetException(nameof(CodeTextBoxModel));
        set
        {
            _codeTextBoxModel = value;
            _codeTextBoxModel.Text.TextContent = ProjectItem.GetText();
            ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(_codeTextBoxModel));
        }
    }

    public ICommand TextChangedCommand { get; set; }

    public ILunaCodeProvider CodeProvider { get; set; }

    public string Header => ProjectItem.Name;

    public CodeFileEditor(CodeFileProjectItem projectItem, ICodeProviderFactory codeProviderFactory, ICodeModelUpdater codeModelUpdater)
    {
        ProjectItem = projectItem;
        _codeModelUpdater = codeModelUpdater;
        TextBoxCommands = new CodeTextBoxCommands();
        TextChangedCommand = new ActionCommand(OnTextChanged);
        CodeProvider = codeProviderFactory.Make(projectItem);
        _codeModelUpdater.Attach(ProjectItem, OnCodeModelUpdated);
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
            CodeProvider.UpdateTokenKinds(updatedTokens);
        }
    }

    private void OnTextChanged()
    {
        _codeModelUpdater.UpdateRequest();
    }

    public void Save()
    {
        ProjectItem.SaveText(CodeTextBoxModel.Text.TextContent);
    }

    public void Close()
    {
        ProjectItem.ResetTextGettingStrategy();
        _codeModelUpdater.Detach(ProjectItem);
    }

    public void NavigateTo(CodeElement codeElement)
    {
        TextBoxCommands.GotoLineCommand.Execute(new CodeHighlighter.Commands.GotoLineCommandParameter(codeElement.LineIndex));
    }

    public void InsertText(string text)
    {
        TextBoxCommands.InsertTextCommand.Execute(new CodeHighlighter.Commands.InsertTextCommandParameter(text));
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
        return _model.Text.TextContent;
    }
}
