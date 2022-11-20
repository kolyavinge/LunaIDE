using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("int", "x")]
internal class Int : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        return new IntegerRuntimeValue(x.IntegerValue);
    }
}
