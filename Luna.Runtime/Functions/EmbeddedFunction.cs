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
        var attr = GetType().GetCustomAttribute<EmbeddedFunctionDeclaration>() ?? throw new NullReferenceException();
        Name = attr.Name;
        Arguments = attr.Arguments;
    }

    public void SetArgumentValues(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        _argumentValues = argumentValues;
    }

    public TValue GetValueOrError<TValue>(int argumentIndex) where TValue : IRuntimeValue
    {
        if (_argumentValues == null) throw RuntimeException.ArgumentsNotPassed();
        var value = _argumentValues[argumentIndex].GetValue();
        if (value is not TValue && value is VariableRuntimeValue variable)
        {
            value = variable.Value;
        }
        if (value is not TValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
        }

        return valueConverted;
    }

    public FunctionRuntimeValue GetFunctionOrError(int argumentIndex)
    {
        if (_argumentValues == null) throw RuntimeException.ArgumentsNotPassed();
        var value = _argumentValues[argumentIndex];
        if (value is not FunctionRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
        }

        return valueConverted;
    }

    public VariableRuntimeValue GetVariableOrError(int argumentIndex)
    {
        if (_argumentValues == null) throw RuntimeException.ArgumentsNotPassed();
        var value = _argumentValues[argumentIndex];
        if (value is not VariableRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
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
