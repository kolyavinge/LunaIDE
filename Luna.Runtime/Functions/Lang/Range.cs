using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("range", "start count")]
internal class Range : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var start = GetValueOrError<IntegerRuntimeValue>(0).IntegerValue;
        var count = GetValueOrError<IntegerRuntimeValue>(1).IntegerValue;
        if (count < 0)
        {
            throw new RuntimeException("Count must be zero or greater.");
        }

        var items = Enumerable.Range(start, count).Select(i => new IntegerRuntimeValue(i)).ToList();

        return new ListRuntimeValue(items);
    }
}
