﻿using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class GtTest : BaseFunctionTest<Gt>
{
    [Test]
    public void GetValue_1()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_2()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(2), new IntegerRuntimeValue(1) });
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_3()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(1) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_4()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new FloatRuntimeValue(1.0), new FloatRuntimeValue(2.0) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_5()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new FloatRuntimeValue(2.0), new FloatRuntimeValue(1.0) });
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_6()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new FloatRuntimeValue(1.0), new FloatRuntimeValue(1.0) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_7()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new FloatRuntimeValue(2.0) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_8()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(2), new FloatRuntimeValue(1.0) });
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_9()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new FloatRuntimeValue(1.0) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_10()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new FloatRuntimeValue(1.0), new IntegerRuntimeValue(2) });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_11()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new FloatRuntimeValue(2.0), new IntegerRuntimeValue(1) });
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_12()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new FloatRuntimeValue(1.0), new IntegerRuntimeValue(1) });
        Assert.False(result.Value);
    }
}
