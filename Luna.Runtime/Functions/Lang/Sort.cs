using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("sort", "list compare_func")]
internal class Sort : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var list = arguments.GetValueOrError<ListRuntimeValue>(0);
        var compareFunc = arguments.GetFunctionOrError(1);

        var comparer = new ListRuntimeValueComparer(compareFunc);

        try
        {
            var result = list.OrderBy(x => x, comparer).ToList();
            return new ListRuntimeValue(result);
        }
        catch (InvalidOperationException e)
        {
            throw e.InnerException!;
        }
    }
}
