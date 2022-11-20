using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("gte", "x y")]
internal class Gte : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        var y = GetValueOrError<NumericRuntimeValue>(argumentValues, 1);

        return new BooleanRuntimeValue(x.FloatValue >= y.FloatValue);
    }
}
