using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class CountTest : BaseFunctionTest<Count>
{
    [Test]
    public void GetValue()
    {
        var list = new ListRuntimeValue(new[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2), new IntegerRuntimeValue(3) });
        var result = GetValue<IntegerRuntimeValue>(new IRuntimeValue[] { list });
        Assert.AreEqual(3, result.IntegerValue);
    }

    [Test]
    public void GetValue_Empty()
    {
        var list = new ListRuntimeValue(new IRuntimeValue[0]);
        var result = GetValue<IntegerRuntimeValue>(new IRuntimeValue[] { list });
        Assert.AreEqual(0, result.IntegerValue);
    }
}
