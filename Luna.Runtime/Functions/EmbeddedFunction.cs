using System.Reflection;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions;

[AttributeUsage(AttributeTargets.Class)]
public class EmbeddedFunctionDeclaration : Attribute
{
    public string Name { get; }

    public string[] Arguments { get; }

    public EmbeddedFunctionDeclaration(string name, string arguments)
    {
        Name = name;
        Arguments = arguments.Split(' ');
    }
}

internal abstract class EmbeddedFunction
{
    public string Name { get; }

    public string[] Arguments { get; }

    protected EmbeddedFunction()
    {
        var attr = GetType().GetCustomAttribute<EmbeddedFunctionDeclaration>() ?? throw new ArgumentException();
        Name = attr.Name;
        Arguments = attr.Arguments;
    }

    protected abstract IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments);

    public IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        try
        {
            return InnerGetValue(new(argumentValues));
        }
        catch (RuntimeException rte)
        {
            RuntimeEnvironment.ExceptionHandler?.Handle(rte);
            return VoidRuntimeValue.Instance;
        }
    }
}
