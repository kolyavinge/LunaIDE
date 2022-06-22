using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class SetTest : BaseFunctionTest<Set>
{
    [Test]
    public void SetValue()
    {
        var variable = new VariableRuntimeValue();
        var value = new IntegerRuntimeValue(123);
        var result = GetValue(new IRuntimeValue[] { variable, value });
        Assert.AreEqual(value, result);
        Assert.AreEqual(new VariableRuntimeValue(value), variable.GetValue());
    }
}
