using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("and", "x y")]
internal class And : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        return new BooleanRuntimeValue(
            arguments.GetValueOrError<BooleanRuntimeValue>(0).Value && arguments.GetValueOrError<BooleanRuntimeValue>(1).Value);
    }
}
