using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class MaxText : BaseFunctionTest<Max>
{
    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void GetValue_EmptyList()
    {
        try
        {
            GetValue<VoidRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[0]));
            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte, Is.EqualTo(new RuntimeException("The list cannot be empty.")));
        }
    }

    [Test]
    public void GetValue_NonNumeric()
    {
        try
        {
            GetValue<VoidRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new BooleanRuntimeValue(true) }));
            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte, Is.EqualTo(new RuntimeException("All the list items must be a numeric values.")));
        }
    }

    [Test]
    public void GetValue_IntegerRuntimeValue()
    {
        var result = GetValue<NumericRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(10), new FloatRuntimeValue(2.2), new IntegerRuntimeValue(-1) }));
        Assert.AreEqual(typeof(IntegerRuntimeValue), result.GetType());
        Assert.AreEqual(10, result.IntegerValue);
    }

    [Test]
    public void GetValue_FloatRuntimeValue()
    {
        var result = GetValue<NumericRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new FloatRuntimeValue(2.2), new IntegerRuntimeValue(-1) }));
        Assert.AreEqual(typeof(FloatRuntimeValue), result.GetType());
        Assert.AreEqual(2.2, result.FloatValue);
    }
}
