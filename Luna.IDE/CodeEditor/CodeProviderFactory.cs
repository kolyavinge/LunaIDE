using System;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditor;

public interface ICodeProviderFactory
{
    CodeProvider Make(ProjectItem projectItem);
}

public class CodeProviderFactory : ICodeProviderFactory
{
    public CodeProvider Make(ProjectItem projectItem)
    {
        if (projectItem is CodeFileProjectItem codeFileProjectItem)
        {
            return new CodeProvider(new CodeProviderScope(codeFileProjectItem));
        }

        throw new ArgumentException($"CodeProvider for {projectItem.GetType()} isn't exist.");
    }
}
