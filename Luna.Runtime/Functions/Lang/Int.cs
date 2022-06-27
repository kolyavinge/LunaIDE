using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("int", "x")]
internal class Int : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        return new IntegerRuntimeValue(x.IntegerValue);
    }
}
