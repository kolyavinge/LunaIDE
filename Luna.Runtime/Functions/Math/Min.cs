using System.Linq;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("min", "list")]
internal class Min : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        if (!list.Any()) throw new RuntimeException("The list cannot be empty.");

        var evaluated = list.Select(x => x.GetValue()).ToList();
        if (evaluated.Any(x => x is not NumericRuntimeValue)) throw new RuntimeException("All the list items must be a numeric values.");
        var evaluatedNumeric = evaluated.Cast<NumericRuntimeValue>().ToList();

        var result = evaluatedNumeric[0];
        double minValue = evaluatedNumeric[0].FloatValue;
        for (int i = 1; i < evaluatedNumeric.Count; i++)
        {
            if (minValue > evaluatedNumeric[i].FloatValue)
            {
                minValue = evaluatedNumeric[i].FloatValue;
                result = evaluatedNumeric[i];
            }
        }

        return result;
    }
}
