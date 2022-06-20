using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class RangeTest : BaseFunctionTest<Range>
{
    [Test]
    public void GetValue()
    {
        var result = GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(5) });
        Assert.AreEqual(typeof(ListRuntimeValue), result.GetType());
        var list = (ListRuntimeValue)result;
        Assert.AreEqual(5, list.Count);
        Assert.AreEqual(new IntegerRuntimeValue(1), list.GetItem(0));
        Assert.AreEqual(new IntegerRuntimeValue(2), list.GetItem(1));
        Assert.AreEqual(new IntegerRuntimeValue(3), list.GetItem(2));
        Assert.AreEqual(new IntegerRuntimeValue(4), list.GetItem(3));
        Assert.AreEqual(new IntegerRuntimeValue(5), list.GetItem(4));
    }

    [Test]
    public void Empty()
    {
        var result = GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(0) });
        Assert.AreEqual(typeof(ListRuntimeValue), result.GetType());
        var list = (ListRuntimeValue)result;
        Assert.AreEqual(0, list.Count);
    }

    [Test]
    public void NegativeCount_Error()
    {
        try
        {
            GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(-1) });
            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Count must be zero or greater.", exp.Message);
        }
    }
}
