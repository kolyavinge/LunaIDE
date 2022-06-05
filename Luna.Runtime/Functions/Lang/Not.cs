using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunction("not", "x")]
internal class Not : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var x = GetValueOrError<BooleanRuntimeValue>(0);
        return new BooleanRuntimeValue(!x.Value);
    }
}
