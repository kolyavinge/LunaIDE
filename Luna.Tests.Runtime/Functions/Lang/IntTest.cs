using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class IntTest : BaseFunctionTest<Int>
{
    [Test]
    public void GetValue()
    {
        var integerResult = GetValue<IntegerRuntimeValue>(new FloatRuntimeValue(2.5));
        Assert.AreEqual(2, integerResult.IntegerValue);
    }
}
