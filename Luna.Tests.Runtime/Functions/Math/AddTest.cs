﻿using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class AddTest : BaseFunctionTest<Add>
{
    [Test]
    public void GetValue_IntegerInteger_Integer()
    {
        var result = GetValue<IntegerRuntimeValue>(new IntegerRuntimeValue(2), new IntegerRuntimeValue(8));
        Assert.AreEqual(10, result.IntegerValue);
    }

    [Test]
    public void GetValue_FloatInteger_Float()
    {
        var result = GetValue<FloatRuntimeValue>(new FloatRuntimeValue(2.0), new IntegerRuntimeValue(8));
        Assert.AreEqual(10.0, result.FloatValue);
    }

    [Test]
    public void GetValue_IntegerFloat_Float()
    {
        var result = GetValue<FloatRuntimeValue>(new IntegerRuntimeValue(8), new FloatRuntimeValue(2.0));
        Assert.AreEqual(10.0, result.FloatValue);
    }

    [Test]
    public void GetValue_FloatFloat_Float()
    {
        var result = GetValue<FloatRuntimeValue>(new FloatRuntimeValue(2.0), new FloatRuntimeValue(8.0));
        Assert.AreEqual(10.0, result.FloatValue);
    }
}
