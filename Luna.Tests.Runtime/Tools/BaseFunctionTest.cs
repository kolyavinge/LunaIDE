using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;

namespace Luna.Tests.Tools;

internal class BaseFunctionTest<TFunction> where TFunction : EmbeddedFunction, new()
{
    protected readonly TFunction _function = new();

    protected void Init()
    {
    }

    protected TResult GetValue<TResult>(IRuntimeValue arg1) where TResult : IRuntimeValue
    {
        return (TResult)_function.GetValue(new[] { arg1 }.ToReadonlyArray());
    }

    protected TResult GetValue<TResult>(IRuntimeValue arg1, IRuntimeValue arg2) where TResult : IRuntimeValue
    {
        return (TResult)_function.GetValue(new[] { arg1, arg2 }.ToReadonlyArray());
    }

    protected TResult GetValue<TResult>(IRuntimeValue arg1, IRuntimeValue arg2, IRuntimeValue arg3) where TResult : IRuntimeValue
    {
        return (TResult)_function.GetValue(new[] { arg1, arg2, arg3 }.ToReadonlyArray());
    }

    protected TResult GetValue<TResult>(IRuntimeValue arg1, IRuntimeValue arg2, IRuntimeValue arg3, IRuntimeValue arg4) where TResult : IRuntimeValue
    {
        return (TResult)_function.GetValue(new[] { arg1, arg2, arg3, arg4 }.ToReadonlyArray());
    }

    protected TResult GetValue<TResult>(IRuntimeValue arg1, IRuntimeValue arg2, IRuntimeValue arg3, IRuntimeValue arg4, IRuntimeValue arg5) where TResult : IRuntimeValue
    {
        return (TResult)_function.GetValue(new[] { arg1, arg2, arg3, arg4, arg5 }.ToReadonlyArray());
    }
}
