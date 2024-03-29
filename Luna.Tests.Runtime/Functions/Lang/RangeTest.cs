﻿using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;
using Range = Luna.Functions.Lang.Range;

namespace Luna.Tests.Functions.Lang;

internal class RangeTest : BaseFunctionTest<Range>
{
    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void GetValue()
    {
        var result = GetValue<ListRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(5));
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(new IntegerRuntimeValue(1), result.GetItem(0));
        Assert.AreEqual(new IntegerRuntimeValue(2), result.GetItem(1));
        Assert.AreEqual(new IntegerRuntimeValue(3), result.GetItem(2));
        Assert.AreEqual(new IntegerRuntimeValue(4), result.GetItem(3));
        Assert.AreEqual(new IntegerRuntimeValue(5), result.GetItem(4));
    }

    [Test]
    public void Empty()
    {
        var list = GetValue<ListRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(0));
        Assert.AreEqual(0, list.Count);
    }

    [Test]
    public void NegativeCount_Error()
    {
        try
        {
            GetValue<VoidRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(-1));
            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte, Is.EqualTo(new RuntimeException("Count must be zero or greater.")));
        }
    }
}
