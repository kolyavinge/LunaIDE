using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("set", "var value")]
internal class Set : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        return null;
    }
}
