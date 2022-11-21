using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("max", "list")]
internal class Max : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var list = arguments.GetValueOrError<ListRuntimeValue>(0);
        if (!list.Any()) throw new RuntimeException("The list cannot be empty.");

        var evaluated = list.Select(x => x.GetValue()).ToList();
        if (evaluated.Any(x => x is not NumericRuntimeValue)) throw new RuntimeException("All the list items must be a numeric values.");
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
