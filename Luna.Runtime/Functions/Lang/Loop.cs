using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("loop", "start count func")]
internal class Loop : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var start = GetValueOrError<IntegerRuntimeValue>(argumentValues, 0).IntegerValue;
        var count = GetValueOrError<IntegerRuntimeValue>(argumentValues, 1).IntegerValue;
        if (count < 0) throw new RuntimeException("Parameter count must be greater than zero.");
        var func = GetFunctionOrError(argumentValues, 2);

        var end = start + count;
        for (int i = start; i < end; i++)
        {
            func.GetValue(new(new[] { new IntegerRuntimeValue(i) }));
        }

        return VoidRuntimeValue.Instance;
    }
}
