using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("sqrt", "x")]
internal class Sqrt : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        return new FloatRuntimeValue(System.Math.Sqrt(x.FloatValue));
    }
}
