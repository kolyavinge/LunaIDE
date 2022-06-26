using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("lse", "x y")]
internal class Lse : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var x = GetValueOrError<NumericRuntimeValue>(0);
        var y = GetValueOrError<NumericRuntimeValue>(1);

        return new BooleanRuntimeValue(x.FloatValue <= y.FloatValue);
    }
}
