using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Luna.IDE.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }
}

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditor : ICodeFileEditor, IEnvironmentWindowModel
{
    public CodeFileProjectItem ProjectItem { get; }

    public TextHolder TextHolder { get; set; }

    public ICodeProvider CodeProvider { get; set; }

    public string Header => ProjectItem.Name;

    public CodeFileEditor(CodeFileProjectItem projectItem, ICodeProviderFactory codeProviderFactory)
    {
        ProjectItem = projectItem;
        TextHolder = new TextHolder(projectItem.GetFromFile());
        CodeProvider = codeProviderFactory.Make(projectItem);
    }

    public void SaveToFile()
    {
        ProjectItem.SaveToFile(TextHolder.TextValue);
    }
}
