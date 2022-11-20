using System.Collections.Generic;
using Luna.Functions;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal interface IRuntimeScopesCollection
{
    IRuntimeScope GetForCodeModel(CodeModel codeModel);
}

internal class RuntimeScopesCollection : IRuntimeScopesCollection
{
    public static RuntimeScopesCollection BuildForCodeModels(
        IEnumerable<CodeModel> codeModels, IValueElementEvaluator evaluator, CallStack callStack)
    {
        var scopes = new RuntimeScopesCollection();
        var embeddedFunctions = new EmbeddedFunctionsCollection();
        foreach (var codeModel in codeModels)
        {
            var scope = RuntimeScope.FromCodeModel(codeModel, evaluator, embeddedFunctions, callStack);
            scopes.Add(codeModel, scope);
        }

        return scopes;
    }

    private readonly Dictionary<CodeModel, IRuntimeScope> _scopes = new();

    public IRuntimeScope GetForCodeModel(CodeModel codeModel)
    {
        return _scopes[codeModel];
    }

    public void Add(CodeModel codeModel, IRuntimeScope scope)
    {
        _scopes.Add(codeModel, scope);
    }
}
