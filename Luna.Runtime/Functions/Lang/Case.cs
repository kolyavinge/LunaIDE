using System.Linq;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("case", "value cases")]
internal class Case : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var value = arguments.GetValueOrError<IRuntimeValue>(0);
        var cases = arguments.GetValueOrError<ListRuntimeValue>(1);
        if (!cases.Any()) throw new RuntimeException("Cases cannot be empty.");
        foreach (var item in cases.SkipLast(1))
        {
            if (item is ListRuntimeValue caseItem && caseItem.Count == 2)
            {
                if (caseItem.GetItem(0).Equals(value))
                {
                    return caseItem.GetItem(1).GetValue();
                }
            }
            else throw new RuntimeException("Items in cases must be a list with two items.");
        }

        return cases.Last().GetValue();
    }
}
