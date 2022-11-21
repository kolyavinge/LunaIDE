using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("eq", "x y")]
internal class Eq : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var x = arguments.GetValueOrError<IRuntimeValue>(0);
        var y = arguments.GetValueOrError<IRuntimeValue>(1);

        var result = x.Equals(y);

        return new BooleanRuntimeValue(result);
    }
}
