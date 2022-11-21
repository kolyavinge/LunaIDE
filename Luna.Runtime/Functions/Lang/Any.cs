using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("any", "list")]
internal class Any : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var list = arguments.GetValueOrError<ListRuntimeValue>(0);
        return new BooleanRuntimeValue(list.Any());
    }
}
