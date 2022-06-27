using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class LseTest : BaseFunctionTest<Lse>
{
    [Test]
    public void GetValue_1()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(2));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_2()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(2), new IntegerRuntimeValue(1));
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_3()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(1));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_4()
    {
        var result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(1.0), new FloatRuntimeValue(2.0));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_5()
    {
        var result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(2.0), new FloatRuntimeValue(1.0));
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_6()
    {
        var result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(1.0), new FloatRuntimeValue(1.0));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_7()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(1), new FloatRuntimeValue(2.0));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_8()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(2), new FloatRuntimeValue(1.0));
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_9()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(1), new FloatRuntimeValue(1.0));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_10()
    {
        var result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(1.0), new IntegerRuntimeValue(2));
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_11()
    {
        var result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(2.0), new IntegerRuntimeValue(1));
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_12()
    {
        var result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(1.0), new IntegerRuntimeValue(1));
        Assert.True(result.Value);
    }
}
