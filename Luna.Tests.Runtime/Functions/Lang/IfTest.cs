using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class IfTest : BaseFunctionTest<If>
{
    [Test]
    public void GetValue()
    {
        var integerResult = GetValue<IntegerRuntimeValue>(new IRuntimeValue[] { new BooleanRuntimeValue(true), new IntegerRuntimeValue(123), new StringRuntimeValue("987") });
        Assert.AreEqual(123, integerResult.IntegerValue);

        var stringResult = GetValue<StringRuntimeValue>(new IRuntimeValue[] { new BooleanRuntimeValue(false), new IntegerRuntimeValue(123), new StringRuntimeValue("987") });
        Assert.AreEqual("987", stringResult.Value);
    }
}
