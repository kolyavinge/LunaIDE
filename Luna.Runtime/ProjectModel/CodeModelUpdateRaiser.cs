using System.Collections.Generic;
using System.Linq;

namespace Luna.ProjectModel;

internal interface ICodeModelUpdateRaiser
{
    void StoreOldCodeModels(IEnumerable<CodeFileProjectItem> codeFiles);
    void RaiseUpdateCodeModelWithDiff();
}

internal class CodeModelUpdateRaiser : ICodeModelUpdateRaiser
{
    private readonly CodeModelScope _scope = new();
    private (CodeFileProjectItem, CodeModel, CodeModelScopeIdentificators)[]? _oldCodeModels;

    public void StoreOldCodeModels(IEnumerable<CodeFileProjectItem> codeFiles)
    {
        _oldCodeModels = codeFiles.
            Select(x => (x, x.CodeModel, _scope.GetScopeIdentificators(x.CodeModel))).
            ToArray();
    }

    public void RaiseUpdateCodeModelWithDiff()
    {
        foreach (var item in _oldCodeModels!)
        {
            var (codeFile, oldCodeModel, oldScopeIdentificators) = item;
            var newScopeIdentificators = _scope.GetScopeIdentificators(codeFile.CodeModel);
            var diff = IdentificatorsComparator.GetDifferent(oldScopeIdentificators, newScopeIdentificators);
            codeFile.RaiseUpdateCodeModel(oldCodeModel, codeFile.CodeModel, diff);
        }
    }
}
