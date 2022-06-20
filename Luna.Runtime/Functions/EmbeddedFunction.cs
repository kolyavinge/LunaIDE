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

    protected EmbeddedFunction()
    {
        var attr = GetType().GetCustomAttribute<EmbeddedFunctionDeclaration>();
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
        if (value is not TValue valueConverted)
        {
            throw new RuntimeException("Embedded function argument cannot be get.");
        }

        return valueConverted;
    }

    public FunctionRuntimeValue GetFunctionOrError(int argumentIndex)
    {
        if (_argumentValues == null) throw new RuntimeException("Argument values have not been passed.");
        var value = _argumentValues[argumentIndex];
        if (value is not FunctionRuntimeValue valueConverted)
        {
            throw new RuntimeException("Embedded function argument cannot be get.");
        }

        return valueConverted;
    }

    public abstract IRuntimeValue GetValue();
}

[AttributeUsage(AttributeTargets.Class)]
internal class EmbeddedFunctionDeclaration : Attribute
{
    public string Name { get; }

    public string[] Arguments { get; }

    public EmbeddedFunctionDeclaration(string name, string arguments)
    {
        Name = name;
        Arguments = arguments.Split(' ');
    }
}
