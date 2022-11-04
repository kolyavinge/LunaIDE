using CodeHighlighter.CodeProvidering;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface ICodeProviderFactory
{
    ICodeProvider Make(ProjectItem projectItem);
}

public class CodeProviderFactory : ICodeProviderFactory
{
    public ICodeProvider Make(ProjectItem projectItem)
    {
        if (projectItem is CodeFileProjectItem codeFileProjectItem)
        {
            return new LunaCodeProvider(new CodeProviderScope(codeFileProjectItem.CodeModel));
        }

        throw new ArgumentException($"CodeProvider for {projectItem.GetType()} isn't exist.");
    }
}
