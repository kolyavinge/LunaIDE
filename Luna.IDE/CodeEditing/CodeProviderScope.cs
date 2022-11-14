using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public class CodeProviderScope : ICodeProviderScope
{
    private readonly CodeModelScope _scope;
    private readonly CodeFileProjectItem _codeFile;

    public CodeProviderScope(CodeFileProjectItem codeFile)
    {
        // нужен весь CodeFileProjectItem, чтобы корректно получать обновления модели
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
