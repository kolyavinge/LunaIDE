using System.Linq;
using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;

namespace Luna.Tests.Tools;

internal class BaseFunctionTest<TFunction> where TFunction : EmbeddedFunction, new()
{
    protected readonly TFunction _function = new();

    protected TValue GetValue<TValue>(params TValue[] values) where TValue : IRuntimeValue
    {
        _function.SetArgumentValues(new ReadonlyArray<IRuntimeValue>(values.Cast<IRuntimeValue>()));
        return (TValue)_function.GetValue();
    }
}
