using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("ignore", "")]
internal class Ignore : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        return VoidRuntimeValue.Instance;
    }
}
