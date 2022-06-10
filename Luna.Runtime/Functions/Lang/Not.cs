using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("not", "x")]
internal class Not : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var x = GetValueOrError<BooleanRuntimeValue>(0);
        return new BooleanRuntimeValue(!x.Value);
    }
}
