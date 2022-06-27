using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class DivTest : BaseFunctionTest<Div>
{
    [Test]
    public void GetValue_IntegerInteger_Integer()
    {
        var result = GetValue<IntegerRuntimeValue>(new IRuntimeValue[] { new IntegerRuntimeValue(8), new IntegerRuntimeValue(2) });
        Assert.AreEqual(4, result.IntegerValue);
    }

    [Test]
    public void GetValue_FloatInteger_Float()
    {
        var result = GetValue<FloatRuntimeValue>(new IRuntimeValue[] { new FloatRuntimeValue(8.0), new IntegerRuntimeValue(2) });
        Assert.AreEqual(4.0, result.FloatValue);
    }

    [Test]
    public void GetValue_IntegerFloat_Float()
    {
        var result = GetValue<FloatRuntimeValue>(new IRuntimeValue[] { new IntegerRuntimeValue(8), new FloatRuntimeValue(2.0) });
        Assert.AreEqual(4.0, result.FloatValue);
    }

    [Test]
    public void GetValue_FloatFloat_Float()
    {
        var result = GetValue<FloatRuntimeValue>(new IRuntimeValue[] { new FloatRuntimeValue(8.0), new FloatRuntimeValue(2.0) });
        Assert.AreEqual(4.0, result.FloatValue);
    }
}
