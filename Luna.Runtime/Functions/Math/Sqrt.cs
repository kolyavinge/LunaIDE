using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("sqrt", "x")]
internal class Sqrt : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var x = GetValueOrError<NumericRuntimeValue>(0);
        return new FloatRuntimeValue(System.Math.Sqrt(x.FloatValue));
    }
}
