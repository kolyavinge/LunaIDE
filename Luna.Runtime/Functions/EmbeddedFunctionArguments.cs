using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions;

internal class EmbeddedFunctionArguments
{
    private readonly ReadonlyArray<IRuntimeValue> _argumentValues;

    public EmbeddedFunctionArguments(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        _argumentValues = argumentValues;
    }

    public TValue GetValueOrError<TValue>(int argumentIndex) where TValue : IRuntimeValue
    {
        var value = _argumentValues[argumentIndex].GetValue();
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

    public FunctionRuntimeValue GetFunctionOrError(int argumentIndex)
    {
        var value = _argumentValues[argumentIndex];
        if (value is not FunctionRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
        }

        return valueConverted;
    }

    public VariableRuntimeValue GetVariableOrError(int argumentIndex)
    {
        var value = _argumentValues[argumentIndex];
        if (value is not VariableRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentСannotGet();
        }

        return valueConverted;
    }
}
