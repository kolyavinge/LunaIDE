using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("sort", "list compareFunc")]
internal class Sort : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var list = GetValueOrError<ListRuntimeValue>(0);
        var compareFunc = GetFunctionOrError(1);

        var comparer = new ListRuntimeValueComparer(compareFunc);

        try
        {
            var result = list.OrderBy(x => x, comparer).ToList();
            return new ListRuntimeValue(result);
        }
        catch (InvalidOperationException e)
        {
            throw e.InnerException;
        }
    }

    class ListRuntimeValueComparer : IComparer<IRuntimeValue>
    {
        private readonly FunctionRuntimeValue _compareFunc;

        public ListRuntimeValueComparer(FunctionRuntimeValue compareFunc)
        {
            _compareFunc = compareFunc;
        }

        public int Compare(IRuntimeValue x, IRuntimeValue y)
        {
            var result = _compareFunc.GetValue(new[] { x, y }.ToReadonlyArray());
            if (result is not NumericRuntimeValue numeric)
            {
                throw new RuntimeException("Compare function must return a numeric value.");
            }

            return numeric.IntegerValue;
        }
    }
}
