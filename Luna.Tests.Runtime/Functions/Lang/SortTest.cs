﻿using Luna.Collections;
using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class SortTest : BaseFunctionTest<Sort>
{
    class IntegerCompareFunc : FunctionRuntimeValue
    {
        public IntegerCompareFunc() : base("", null) { }

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            var x = ((IntegerRuntimeValue)argumentValues[0]).IntegerValue;
            var y = ((IntegerRuntimeValue)argumentValues[1]).IntegerValue;

            return new IntegerRuntimeValue(x.CompareTo(y));
        }
    }

    class FloatCompareFunc : FunctionRuntimeValue
    {
        public FloatCompareFunc() : base("", null) { }

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            var x = ((IntegerRuntimeValue)argumentValues[0]).IntegerValue;
            var y = ((IntegerRuntimeValue)argumentValues[1]).IntegerValue;

            return new FloatRuntimeValue(x.CompareTo(y));
        }
    }

    class WrongCompareFunc : FunctionRuntimeValue
    {
        public WrongCompareFunc() : base("", null) { }

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            return new StringRuntimeValue("123");
        }
    }

    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void GetValue_IntegerCompareFunc()
    {
        var list = new ListRuntimeValue(new[] { new IntegerRuntimeValue(5), new IntegerRuntimeValue(2), new IntegerRuntimeValue(-1) });
        var compareFunc = new IntegerCompareFunc();

        var result = GetValue<ListRuntimeValue>(list, compareFunc);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(new IntegerRuntimeValue(-1), result.GetItem(0));
        Assert.AreEqual(new IntegerRuntimeValue(2), result.GetItem(1));
        Assert.AreEqual(new IntegerRuntimeValue(5), result.GetItem(2));
    }

    [Test]
    public void GetValue_FloatCompareFunc()
    {
        var list = new ListRuntimeValue(new[] { new IntegerRuntimeValue(5), new IntegerRuntimeValue(2), new IntegerRuntimeValue(-1) });
        var compareFunc = new FloatCompareFunc();

        var result = GetValue<ListRuntimeValue>(list, compareFunc);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(new IntegerRuntimeValue(-1), result.GetItem(0));
        Assert.AreEqual(new IntegerRuntimeValue(2), result.GetItem(1));
        Assert.AreEqual(new IntegerRuntimeValue(5), result.GetItem(2));
    }

    [Test]
    public void WrongCompareFunc_Error()
    {
        var list = new ListRuntimeValue(new[] { new IntegerRuntimeValue(5), new IntegerRuntimeValue(2), new IntegerRuntimeValue(-1) });
        var compareFunc = new WrongCompareFunc();
        try
        {
            GetValue<VoidRuntimeValue>(list, compareFunc);
            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte, Is.EqualTo(new RuntimeException("The compare function must return a numeric value.")));
        }
    }
}
