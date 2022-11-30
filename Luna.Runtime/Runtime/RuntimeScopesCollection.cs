using System.Collections.Generic;
using Luna.Functions;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal interface IRuntimeScopesCollection
{
    IRuntimeScope GetForCodeModel(CodeModel codeModel);
    IRuntimeScope GetForFunction(IRuntimeScope scope, string functionName);
}

internal class RuntimeScopesCollection : IRuntimeScopesCollection
{
    public static RuntimeScopesCollection BuildForCodeModels(
        IEnumerable<CodeModel> codeModels, IValueElementEvaluator evaluator, CallStack callStack)
    {
        var embeddedFunctions = new EmbeddedFunctionsCollection();
        var scopes = new RuntimeScopesCollection(embeddedFunctions);
        foreach (var codeModel in codeModels)
        {
            var scope = RuntimeScope.FromCodeModel(codeModel, evaluator, embeddedFunctions, callStack);
            scopes.Add(codeModel, scope);
        }

        return scopes;
    }

    private readonly IEmbeddedFunctionsCollection _embeddedFunctions;
    private readonly Dictionary<CodeModel, IRuntimeScope> _scopes = new();

    public RuntimeScopesCollection(IEmbeddedFunctionsCollection embeddedFunctions)
    {
        _embeddedFunctions = embeddedFunctions;
    }

    public IRuntimeScope GetForCodeModel(CodeModel codeModel)
    {
        return _scopes[codeModel];
    }

    public IRuntimeScope GetForFunction(IRuntimeScope scope, string functionName)
    {
        if (_embeddedFunctions.Contains(functionName))
        {
            return scope;
        }
        else
        {
            var declaration = scope.GetFunctionDeclaration(functionName);
            return _scopes[declaration.CodeModel];
        }
    }

    private void Add(CodeModel codeModel, IRuntimeScope scope)
    {
        _scopes.Add(codeModel, scope);
    }
}
