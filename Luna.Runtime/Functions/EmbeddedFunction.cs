using System;
using System.Reflection;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions;

internal abstract class EmbeddedFunction
{
    private ReadonlyArray<IRuntimeValue>? _argumentValues;

    public string Name { get; }

    public string[] Arguments { get; }

    public EmbeddedFunction()
    {
        var attr = GetType().GetCustomAttribute<EmbeddedFunctionAttribute>();
        Name = attr.Name;
        Arguments = attr.Arguments;
    }

    public void SetArgumentValues(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        _argumentValues = argumentValues;
    }

    public TValue GetValueOrError<TValue>(int argumentIndex) where TValue : IRuntimeValue
    {
        if (_argumentValues == null) throw new RuntimeException("Argument values have not been passed.");
        var value = _argumentValues[argumentIndex].GetValue();
        if (value is not TValue valueConvered)
        {
            throw new RuntimeException("Embedded function argument cannot be get.");
        }

        return valueConvered;
    }

    public abstract IRuntimeValue GetValue();
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
