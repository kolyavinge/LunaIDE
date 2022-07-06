using System.Linq;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("range", "start count")]
internal class Range : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var start = (int)GetValueOrError<IntegerRuntimeValue>(argumentValues, 0).IntegerValue;
        var count = (int)GetValueOrError<IntegerRuntimeValue>(argumentValues, 1).IntegerValue;
        if (count < 0)
        {
            throw new RuntimeException("Count must be zero or greater.");
        }

        var items = Enumerable.Range(start, count).Select(i => new IntegerRuntimeValue(i)).ToList();

        return new ListRuntimeValue(items);
    }
}
