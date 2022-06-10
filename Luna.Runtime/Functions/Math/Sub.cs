using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("-", "x y")]
internal class Sub : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var x = GetValueOrError<NumericRuntimeValue>(0);
        var y = GetValueOrError<NumericRuntimeValue>(1);

        if (x is IntegerRuntimeValue xint && y is IntegerRuntimeValue yint)
        {
            return new IntegerRuntimeValue(xint.IntegerValue - yint.IntegerValue);
        }
        else
        {
            return new FloatRuntimeValue(x.FloatValue - y.FloatValue);
        }
    }
}
