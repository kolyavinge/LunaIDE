using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Luna.Functions;

public class EmbeddedFunctionDeclarationsCollection
{
    private readonly Dictionary<string, EmbeddedFunctionDeclaration> _functions;

    public EmbeddedFunctionDeclarationsCollection()
    {
        var functions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Select(t => t.GetCustomAttribute<EmbeddedFunctionDeclaration>())
            .Where(x => x != null)
            .ToList();

        _functions = functions.ToDictionary(k => k.Name, v => v);
    }

    public bool Contains(string functionName)
    {
        return _functions.ContainsKey(functionName);
    }
}
