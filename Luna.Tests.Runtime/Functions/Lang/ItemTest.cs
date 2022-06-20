using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class ItemTest : BaseFunctionTest<Item>
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetValue()
    {
        var list = new ListRuntimeValue(new[] { new FloatRuntimeValue(1.2), new FloatRuntimeValue(2.2), new FloatRuntimeValue(3.2) });

        var result0 = GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(0), list });
        var result1 = GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), list });
        var result2 = GetValue(new IRuntimeValue[] { new FloatRuntimeValue(2), list });

        Assert.AreEqual(typeof(FloatRuntimeValue), result0.GetType());
        Assert.AreEqual(typeof(FloatRuntimeValue), result1.GetType());
        Assert.AreEqual(typeof(FloatRuntimeValue), result2.GetType());

        Assert.AreEqual(1.2, ((FloatRuntimeValue)result0).FloatValue);
        Assert.AreEqual(2.2, ((FloatRuntimeValue)result1).FloatValue);
        Assert.AreEqual(3.2, ((FloatRuntimeValue)result2).FloatValue);
    }

    [Test]
    public void NegativeIndex_Error()
    {
        try
        {
            var list = new ListRuntimeValue(new[] { new FloatRuntimeValue(1.2), new FloatRuntimeValue(2.2), new FloatRuntimeValue(3.2) });
            GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(-1), list });
            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Index must be within list items range.", exp.Message);
        }
    }

    [Test]
    public void BigIndex_Error()
    {
        try
        {
            var list = new ListRuntimeValue(new[] { new FloatRuntimeValue(1.2), new FloatRuntimeValue(2.2), new FloatRuntimeValue(3.2) });
            GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(10), list });
            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Index must be within list items range.", exp.Message);
        }
    }
}
