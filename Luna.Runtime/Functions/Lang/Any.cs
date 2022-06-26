using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("any", "list")]
internal class Any : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var list = GetValueOrError<ListRuntimeValue>(0);
        return new BooleanRuntimeValue(list.Any());
    }
}
