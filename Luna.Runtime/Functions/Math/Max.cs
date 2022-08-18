﻿using System.Linq;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("max", "list")]
internal class Max : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        if (!list.Any()) throw new RuntimeException("the list cannot be empty");

        var evaluated = list.Select(x => x.GetValue()).ToList();
        if (evaluated.Any(x => x is not NumericRuntimeValue)) throw new RuntimeException("all the list items must be a numeric values");
        var evaluatedNumeric = evaluated.Cast<NumericRuntimeValue>().ToList();

        var result = evaluatedNumeric[0];
        double maxValue = evaluatedNumeric[0].FloatValue;
        for (int i = 1; i < evaluatedNumeric.Count; i++)
        {
            if (maxValue < evaluatedNumeric[i].FloatValue)
            {
                maxValue = evaluatedNumeric[i].FloatValue;
                result = evaluatedNumeric[i];
            }
        }

        return result;
    }
}
