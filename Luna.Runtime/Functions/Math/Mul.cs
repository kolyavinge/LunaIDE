using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("*", "x y")]
internal class Mul : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var x = arguments.GetValueOrError<NumericRuntimeValue>(0);
        var y = arguments.GetValueOrError<NumericRuntimeValue>(1);

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
