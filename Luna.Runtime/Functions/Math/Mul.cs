using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("*", "x y")]
internal class Mul : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        var y = GetValueOrError<NumericRuntimeValue>(argumentValues, 1);

        if (x is IntegerRuntimeValue xint && y is IntegerRuntimeValue yint)
        {
            return new IntegerRuntimeValue(xint.IntegerValue * yint.IntegerValue);
        }
        else
        {
            return new FloatRuntimeValue(x.FloatValue * y.FloatValue);
        }
    }
}
