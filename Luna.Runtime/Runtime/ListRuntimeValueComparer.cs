using System.Collections.Generic;
using Luna.Collections;

namespace Luna.Runtime;

internal class ListRuntimeValueComparer : IComparer<IRuntimeValue>
{
    private readonly FunctionRuntimeValue _compareFunc;

    public ListRuntimeValueComparer(FunctionRuntimeValue compareFunc)
    {
        _compareFunc = compareFunc;
    }

    public int Compare(IRuntimeValue? x, IRuntimeValue? y)
    {
        if (x == null && y == null) return 0;
        if (y == null) return -1;
        if (x == null) return 1;

        var result = _compareFunc.GetValue(new[] { x, y }.ToReadonlyArray());
        if (result is not NumericRuntimeValue numeric)
        {
            throw new RuntimeException("The compare function must return a numeric value.");
        }

        return (int)numeric.IntegerValue;
    }
}
