using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Luna.Functions;

public class EmbeddedFunctionDeclarationsCollection : IEnumerable<EmbeddedFunctionDeclaration>
{
    private readonly Dictionary<string, EmbeddedFunctionDeclaration> _functions;

    public EmbeddedFunctionDeclarationsCollection()
    {
        var functions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Select(t => t.GetCustomAttribute<EmbeddedFunctionDeclaration>())
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        _functions = functions.ToDictionary(k => k.Name, v => v);
    }

    public bool Contains(string functionName)
    {
        return _functions.ContainsKey(functionName);
    }

    public IEnumerator<EmbeddedFunctionDeclaration> GetEnumerator()
    {
        return _functions.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _functions.Values.GetEnumerator();
    }
}
