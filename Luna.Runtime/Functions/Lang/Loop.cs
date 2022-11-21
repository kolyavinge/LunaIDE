using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("loop", "start count func")]
internal class Loop : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var start = arguments.GetValueOrError<IntegerRuntimeValue>(0).IntegerValue;
        var count = arguments.GetValueOrError<IntegerRuntimeValue>(1).IntegerValue;
        if (count < 0) throw new RuntimeException("Parameter count must be greater than zero.");
        var func = arguments.GetFunctionOrError(2);

        var end = start + count;
        for (var i = start; i < end; i++)
        {
            func.GetValue(new(new[] { new IntegerRuntimeValue(i) }));
        }

        return VoidRuntimeValue.Instance;
    }
}
