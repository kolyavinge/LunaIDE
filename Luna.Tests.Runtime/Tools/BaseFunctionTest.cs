using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;

namespace Luna.Tests.Tools;

internal class BaseFunctionTest<TFunction> where TFunction : EmbeddedFunction, new()
{
    protected readonly TFunction _function = new();

    protected TResult GetValue<TResult>(IRuntimeValue[] values) where TResult : IRuntimeValue
    {
        return (TResult)_function.GetValue(new ReadonlyArray<IRuntimeValue>(values));
    }
}
