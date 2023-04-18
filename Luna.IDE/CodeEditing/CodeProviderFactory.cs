using CodeHighlighter.CodeProvidering;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

internal interface ICodeProviderFactory
{
    ICodeProvider Make(ProjectItem projectItem);
}

internal class CodeProviderFactory : ICodeProviderFactory
{
    public ICodeProvider Make(ProjectItem projectItem)
    {
        if (projectItem is CodeFileProjectItem codeFileProjectItem)
        {
            return new LunaCodeProvider(new CodeProviderScope(codeFileProjectItem));
        }

        throw new ArgumentException($"CodeProvider for {projectItem.GetType()} isn't exist.");
    }
}
