using Luna.Functions;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditor;

public interface ICodeProviderScope
{
    bool IsFunction(string tokenName);
}

public class CodeProviderScope : ICodeProviderScope
{
    private readonly EmbeddedFunctionDeclarationsCollection _embeddedFunctions = new();
    private readonly CodeFileProjectItem _codeFile;

    public CodeProviderScope(CodeFileProjectItem codeFile)
    {
        _codeFile = codeFile;
    }

    public bool IsFunction(string tokenName)
    {
        return _embeddedFunctions.Contains(tokenName) || (_codeFile.CodeModel?.Functions.Contains(tokenName) ?? false);
    }
}
