using System;
using System.Reflection;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions;

internal abstract class EmbeddedFunction
{
    public string Name { get; }

    public string[] Arguments { get; }

    public EmbeddedFunction()
    {
        var attr = GetType().GetCustomAttribute<EmbeddedFunctionAttribute>();
        Name = attr.Name;
        Arguments = attr.Arguments;
    }

    protected TValue GetValueOrError<TValue>(ReadonlyArray<IRuntimeValue> argumentValues, int argumentIndex) where TValue : IRuntimeValue
    {
        if (argumentValues[argumentIndex] is not TValue value)
        {
            throw new RuntimeException("Embedded function argument cannot be get.");
        }

        return value;
    }

    public abstract IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues);
}

[AttributeUsage(AttributeTargets.Class)]
internal class EmbeddedFunctionAttribute : Attribute
{
    public string Name { get; }

    public string[] Arguments { get; }

    public EmbeddedFunctionAttribute(string name, string arguments)
    {
        Name = name;
        Arguments = arguments.Split(' ');
    }
}
