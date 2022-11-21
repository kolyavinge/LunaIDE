using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("sqrt", "x")]
internal class Sqrt : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var x = arguments.GetValueOrError<NumericRuntimeValue>(0);
        return new FloatRuntimeValue(System.Math.Sqrt(x.FloatValue));
    }
}
