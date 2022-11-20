using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions.Math;

internal class MinText : BaseFunctionTest<Min>
{
    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void GetValue_EmptyList()
    {
        GetValue<VoidRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[0]));
        _exceptionHandler.Verify(x => x.Handle(new RuntimeException("The list cannot be empty.")), Times.Once());
    }

    [Test]
    public void GetValue_NonNumeric()
    {
        GetValue<VoidRuntimeValue>(new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new BooleanRuntimeValue(true) }));
        _exceptionHandler.Verify(x => x.Handle(new RuntimeException("All the list items must be a numeric values.")), Times.Once());
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
