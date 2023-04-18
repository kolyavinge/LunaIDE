using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

internal class CompositeCodeProviderScope : ICodeProviderScope
{
    private readonly CodeModelScope _scope;
    private readonly IReadOnlyCollection<CodeModel> _codeModels;

    public CompositeCodeProviderScope(IReadOnlyCollection<CodeModel> codeModels)
    {
        // модели без CodeFileProjectItem, если они обновятся, тут останутся старые экземпляры
        _codeModels = codeModels;
        _scope = new CodeModelScope();
    }

    public bool IsConstant(string name)
    {
        return _codeModels.Any(codeModel => _scope.IsConstantExist(codeModel, name));
    }

    public bool IsFunction(string name)
    {
        return _codeModels.Any(codeModel => _scope.IsFunctionExist(codeModel, name));
    }
}
