using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }
}

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel
{
    private readonly ICodeModelUpdater _codeModelUpdater;

    public CodeFileProjectItem ProjectItem { get; }

    public TextHolder TextHolder { get; set; }

    public ILunaCodeProvider CodeProvider { get; set; }

    public string Header => ProjectItem.Name;

    public CodeFileEditor(
        CodeFileProjectItem projectItem,
        ICodeProviderFactory codeProviderFactory,
        ICodeModelUpdater codeModelUpdater)
    {
        ProjectItem = projectItem;
        _codeModelUpdater = codeModelUpdater;
        TextHolder = new TextHolder(projectItem.GetText());
        ProjectItem.SetTextGettingStrategy(new EditorTextGettingStrategy(TextHolder));
        CodeProvider = codeProviderFactory.Make(projectItem);
        _codeModelUpdater.Attach(ProjectItem, OnCodeModelUpdated);
    }

    public void OnCodeModelUpdated(CodeModelUpdatedEventArgs e)
    {
        CodeProvider.UpdateIdentificators();
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
