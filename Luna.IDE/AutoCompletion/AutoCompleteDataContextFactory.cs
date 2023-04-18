using Luna.IDE.CodeEditing;

namespace Luna.IDE.AutoCompletion;

public interface IAutoCompleteDataContextFactory
{
    IAutoCompleteDataContext Make(ICodeFileEditor editor);
}

internal class AutoCompleteDataContextFactory : IAutoCompleteDataContextFactory
{
    public IAutoCompleteDataContext Make(ICodeFileEditor editor)
    {
        return new AutoCompleteDataContext(editor);
    }
}
