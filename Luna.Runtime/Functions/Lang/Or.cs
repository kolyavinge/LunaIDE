using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("or", "x y")]
internal class Or : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        return new BooleanRuntimeValue(
            GetValueOrError<BooleanRuntimeValue>(0).Value || GetValueOrError<BooleanRuntimeValue>(1).Value);
    }
}
