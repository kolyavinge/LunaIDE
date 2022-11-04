using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public class CodeProviderScope : ICodeProviderScope
{
    private readonly CodeModel _codeModel;
    private readonly CodeModelScope _scope;

    public CodeProviderScope(CodeModel codeModel)
    {
        _codeModel = codeModel;
        _scope = new CodeModelScope();
    }

    public bool IsConstant(string name)
    {
        return _scope.IsConstantExist(_codeModel, name);
    }

    public bool IsFunction(string name)
    {
        return _scope.IsFunctionExist(_codeModel, name);
    }
}
