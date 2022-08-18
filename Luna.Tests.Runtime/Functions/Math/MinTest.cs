using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class MinText : BaseFunctionTest<Min>
{
    [Test]
    public void GetValue_EmptyList()
    {
        try
        {
            GetValue<NumericRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[0]));
            Assert.Fail();
        }
        catch (RuntimeException e)
        {
            Assert.AreEqual("the list cannot be empty", e.Message);
        }
    }

    [Test]
    public void GetValue_NonNumeric()
    {
        try
        {
            GetValue<NumericRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new BooleanRuntimeValue(true) }));
            Assert.Fail();
        }
        catch (RuntimeException e)
        {
            Assert.AreEqual("all the list items must be a numeric values", e.Message);
        }
    }

    [Test]
    public void GetValue_IntegerRuntimeValue()
    {
        var result = GetValue<NumericRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new FloatRuntimeValue(2.2), new IntegerRuntimeValue(2) }));
        Assert.AreEqual(typeof(IntegerRuntimeValue), result.GetType());
        Assert.AreEqual(1, result.IntegerValue);
    }

    [Test]
    public void GetValue_FloatRuntimeValue()
    {
        var result = GetValue<NumericRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(10), new FloatRuntimeValue(2.2), new IntegerRuntimeValue(5) }));
        Assert.AreEqual(typeof(FloatRuntimeValue), result.GetType());
        Assert.AreEqual(2.2, result.FloatValue);
    }
}
