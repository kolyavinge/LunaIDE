using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface ICodeProviderFactory
{
    ILunaCodeProvider Make(ProjectItem projectItem);
}

public class CodeProviderFactory : ICodeProviderFactory
{
    public ILunaCodeProvider Make(ProjectItem projectItem)
    {
        if (projectItem is CodeFileProjectItem codeFileProjectItem)
        {
            return new LunaCodeProvider(new CodeProviderScope(codeFileProjectItem));
        }

        throw new ArgumentException($"CodeProvider for {projectItem.GetType()} isn't exist.");
    }
}
