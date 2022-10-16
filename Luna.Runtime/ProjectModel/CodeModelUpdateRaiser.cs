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
    private List<KeyValuePair<CodeFileProjectItem, CodeModel>> _oldCodeModels = new();

    public void StoreOldCodeModels(IEnumerable<CodeFileProjectItem> codeFiles)
    {
        _oldCodeModels = codeFiles.Select(x => new KeyValuePair<CodeFileProjectItem, CodeModel>(x, x.CodeModel)).ToList();
    }

    public void RaiseUpdateCodeModelWithDiff()
    {
        var scope = new CodeModelScope();
        foreach (var kv in _oldCodeModels)
        {
            var codeFile = kv.Key;
            var oldCodeModel = kv.Value;

            var oldScopeIdentificators = scope.GetScopeIdentificators(oldCodeModel);
            var newScopeIdentificators = scope.GetScopeIdentificators(codeFile.CodeModel);

            var diff = IdentificatorsComparator.GetDifferent(oldScopeIdentificators, newScopeIdentificators);

            codeFile.RaiseUpdateCodeModel(diff);
        }
    }
}
