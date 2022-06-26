using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("and", "x y")]
internal class And : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        return new BooleanRuntimeValue(
            GetValueOrError<BooleanRuntimeValue>(0).Value && GetValueOrError<BooleanRuntimeValue>(1).Value);
    }
}
