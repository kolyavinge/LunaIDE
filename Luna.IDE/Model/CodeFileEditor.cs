using System.Linq;
using System.Windows.Input;
using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Luna.IDE.Mvvm;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }
    void NavigateTo(CodeElement codeElement);
}

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel
{
    private readonly ICodeModelUpdater _codeModelUpdater;

    public CodeFileProjectItem ProjectItem { get; }

    public CodeTextBoxCommands TextBoxCommands { get; }

    public TextHolder TextHolder { get; set; }

    public ICommand TextChangedCommand { get; set; }

    public ILunaCodeProvider CodeProvider { get; set; }

    public string Header => ProjectItem.Name;

    public CodeFileEditor(
        CodeFileProjectItem projectItem,
        ICodeProviderFactory codeProviderFactory,
        ICodeModelUpdater codeModelUpdater)
    {
        ProjectItem = projectItem;
        _codeModelUpdater = codeModelUpdater;
        TextBoxCommands = new CodeTextBoxCommands();
        TextHolder = new TextHolder(projectItem.GetText());
        TextChangedCommand = new ActionCommand(OnTextChanged);
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(TextHolder));
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
        ProjectItem.SaveText(TextHolder.TextValue);
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
}

class EditorTextGettingStrategy : TextFileProjectItem.ITextGettingStrategy
{
    private readonly TextHolder _textHolder;

    public EditorTextGettingStrategy(TextHolder textHolder)
    {
        _textHolder = textHolder;
    }

    public string GetText()
    {
        return _textHolder.TextValue;
    }
}
