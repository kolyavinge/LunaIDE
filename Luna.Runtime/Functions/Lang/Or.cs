using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("or", "x y")]
internal class Or : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        return new BooleanRuntimeValue(
            arguments.GetValueOrError<BooleanRuntimeValue>(0).Value || arguments.GetValueOrError<BooleanRuntimeValue>(1).Value);
    }
}
