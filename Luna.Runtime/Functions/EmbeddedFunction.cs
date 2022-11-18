using System.Reflection;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions;

internal abstract class EmbeddedFunction
{
    public string Name { get; }

    public string[] Arguments { get; }

    protected EmbeddedFunction()
    {
        var attr = GetType().GetCustomAttribute<EmbeddedFunctionDeclaration>() ?? throw new NullReferenceException();
        Name = attr.Name;
        Arguments = attr.Arguments;
    }

    public TValue GetValueOrError<TValue>(ReadonlyArray<IRuntimeValue> argumentValues, int argumentIndex) where TValue : IRuntimeValue
    {
        var value = argumentValues[argumentIndex].GetValue();
        if (value is VariableRuntimeValue variable)
        {
            if (variable.Value is TValue typedValue)
            {
                return typedValue;
            }
        }
        if (value is TValue valueConverted)
        {
            return valueConverted;
        }

        throw RuntimeException.ArgumentСannotGet();
    }

    public FunctionRuntimeValue GetFunctionOrError(ReadonlyArray<IRuntimeValue> argumentValues, int argumentIndex)
    {
        var value = argumentValues[argumentIndex];
        if (value is not FunctionRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
        }

        return valueConverted;
    }

    public VariableRuntimeValue GetVariableOrError(ReadonlyArray<IRuntimeValue> argumentValues, int argumentIndex)
    {
        var value = argumentValues[argumentIndex];
        if (value is not VariableRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
        }

        return valueConverted;
    }

    public abstract IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues);
}

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
