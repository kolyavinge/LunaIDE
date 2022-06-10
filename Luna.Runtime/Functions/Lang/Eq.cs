using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("eq", "x y")]
internal class Eq : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var x = GetValueOrError<IRuntimeValue>(0);
        var y = GetValueOrError<IRuntimeValue>(1);

        var result = x.Equals(y);

        return new BooleanRuntimeValue(result);
    }
}
