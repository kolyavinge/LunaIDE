using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions;

internal class EmbeddedFunctionArguments
{
    private readonly ReadonlyArray<IRuntimeValue> _argumentValues;
    private readonly string[] _arguments;

    public EmbeddedFunctionArguments(ReadonlyArray<IRuntimeValue> argumentValues, string[] arguments)
    {
        _argumentValues = argumentValues;
        _arguments = arguments;
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
            else
            {
                throw RuntimeException.ArgumentMustBe(_arguments[argumentIndex], FriendlyName.Get<TValue>(), FriendlyName.Get(variable.Value));
            }
        }
        if (value is TValue valueConverted)
        {
            return valueConverted;
        }
        else
        {
            throw RuntimeException.ArgumentMustBe(_arguments[argumentIndex], FriendlyName.Get<TValue>(), FriendlyName.Get(value));
        }
    }

    public FunctionRuntimeValue GetFunctionOrError(int argumentIndex)
    {
        var value = _argumentValues[argumentIndex];
        if (value is not FunctionRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentMustBe(_arguments[argumentIndex], FriendlyName.Get<FunctionRuntimeValue>(), FriendlyName.Get(value));
        }

        return valueConverted;
    }

    public VariableRuntimeValue GetVariableOrError(int argumentIndex)
    {
        var value = _argumentValues[argumentIndex];
        if (value is not VariableRuntimeValue valueConverted)
        {
            throw RuntimeException.ArgumentMustBe(_arguments[argumentIndex], FriendlyName.Get<VariableRuntimeValue>(), FriendlyName.Get(value));
        }

        return valueConverted;
    }
}
