using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("int", "x")]
internal class Int : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var x = arguments.GetValueOrError<NumericRuntimeValue>(0);
        return new IntegerRuntimeValue(x.IntegerValue);
    }
}
