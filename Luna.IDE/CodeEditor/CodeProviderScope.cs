using System.Linq;
using Luna.Functions;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditor;

public interface ICodeProviderScope
{
    bool IsConstant(string name);
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

    public bool IsConstant(string name)
    {
        return _codeFile.CodeModel.Constants.Contains(name) || _codeFile.CodeModel.Imports.Any(x => x.CodeFile.CodeModel.Constants.Contains(name));
    }

    public bool IsFunction(string name)
    {
        return _embeddedFunctions.Contains(name) ||
            _codeFile.CodeModel.Functions.Contains(name) ||
            _codeFile.CodeModel.Imports.Any(x => x.CodeFile.CodeModel.Functions.Contains(name));
    }
}
