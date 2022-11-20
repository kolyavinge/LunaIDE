using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("sqrt", "x")]
internal class Sqrt : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        return new FloatRuntimeValue(System.Math.Sqrt(x.FloatValue));
    }
}
