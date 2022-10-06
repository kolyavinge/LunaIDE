using Luna.ProjectModel;

namespace Luna.IDE.App.CodeEditor;

public interface ICodeProviderScope
{
    bool IsConstant(string name);
    bool IsFunction(string tokenName);
}

public class CodeProviderScope : ICodeProviderScope
{
    private readonly CodeFileProjectItem _codeFile;
    private readonly CodeModelScope _scope;

    public CodeProviderScope(CodeFileProjectItem codeFile)
    {
        _codeFile = codeFile;
        _scope = new CodeModelScope();
    }

    public bool IsConstant(string name)
    {
        return _scope.IsConstantExist(_codeFile.CodeModel, name);
    }

    public bool IsFunction(string name)
    {
        return _scope.IsFunctionExist(_codeFile.CodeModel, name);
    }
}
