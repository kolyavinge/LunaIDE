using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;

namespace Luna.Tests.Tools;

internal class FunctionsTest<TFunction> where TFunction : EmbeddedFunction, new()
{
    protected readonly TFunction _function = new();

    protected TValue GetValue<TValue>(params TValue[] values) where TValue : RuntimeValue
    {
        return (TValue)_function.GetValue(new ReadonlyArray<RuntimeValue>(values));
    }
}
