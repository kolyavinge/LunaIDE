using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Luna.Functions;

internal interface IEmbeddedFunctionsCollection
{
    bool Contains(string functionName);
    EmbeddedFunction GetByName(string functionName);
}

internal class EmbeddedFunctionsCollection : IEmbeddedFunctionsCollection
{
    private readonly Dictionary<string, EmbeddedFunction> _functions;

    public EmbeddedFunctionsCollection()
    {
        var functions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<EmbeddedFunctionDeclaration>() != null)
            .Select(Activator.CreateInstance)
            .Cast<EmbeddedFunction>()
            .ToList();

        _functions = functions.ToDictionary(k => k.Name, v => v);
    }

    public bool Contains(string functionName)
    {
        return _functions.ContainsKey(functionName);
    }

    public EmbeddedFunction GetByName(string functionName)
    {
        return _functions[functionName];
    }
}
