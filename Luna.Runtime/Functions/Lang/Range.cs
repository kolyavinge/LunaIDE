using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("range", "start count")]
internal class Range : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var start = (int)arguments.GetValueOrError<IntegerRuntimeValue>(0).IntegerValue;
        var count = (int)arguments.GetValueOrError<IntegerRuntimeValue>(1).IntegerValue;
        if (count < 0)
        {
            throw new RuntimeException("Count must be zero or greater.");
        }

        var items = Enumerable.Range(start, count).Select(i => new IntegerRuntimeValue(i)).ToList();

        return new ListRuntimeValue(items);
    }
}
