﻿using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class ItemTest : BaseFunctionTest<Item>
{
    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void GetValue()
    {
        var list = new ListRuntimeValue(new[] { new FloatRuntimeValue(1.2), new FloatRuntimeValue(2.2), new FloatRuntimeValue(3.2) });

        var result0 = GetValue<FloatRuntimeValue>(new IntegerRuntimeValue(0), list);
        var result1 = GetValue<FloatRuntimeValue>(new IntegerRuntimeValue(1), list);
        var result2 = GetValue<FloatRuntimeValue>(new FloatRuntimeValue(2), list);

        Assert.AreEqual(1.2, result0.FloatValue);
        Assert.AreEqual(2.2, result1.FloatValue);
        Assert.AreEqual(3.2, result2.FloatValue);
    }

    [Test]
    public void NegativeIndex_Error()
    {
        var list = new ListRuntimeValue(new[] { new FloatRuntimeValue(1.2), new FloatRuntimeValue(2.2), new FloatRuntimeValue(3.2) });
        GetValue<IRuntimeValue>(new IntegerRuntimeValue(-1), list);
        _exceptionHandler.Verify(x => x.Handle(new RuntimeException("Index must be within list items range.")), Times.Once());
    }

    [Test]
    public void BigIndex_Error()
    {
        var list = new ListRuntimeValue(new[] { new FloatRuntimeValue(1.2), new FloatRuntimeValue(2.2), new FloatRuntimeValue(3.2) });
        GetValue<IRuntimeValue>(new IntegerRuntimeValue(10), list);
        _exceptionHandler.Verify(x => x.Handle(new RuntimeException("Index must be within list items range.")), Times.Once());
    }
}
