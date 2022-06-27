using System.Linq;
using Luna.Collections;
using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class MapTest : BaseFunctionTest<Map>
{
    class TestFunctionRuntimeValue : FunctionRuntimeValue
    {
        public TestFunctionRuntimeValue() : base("", null) { }

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            return argumentValues.First();
        }
    }

    private TestFunctionRuntimeValue _func;

    [SetUp]
    public void Setup()
    {
        _func = new TestFunctionRuntimeValue();
    }

    [Test]
    public void GetValue()
    {
        var list = new[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2), new IntegerRuntimeValue(3) };

        var result = GetValue<ListRuntimeValue>(new IRuntimeValue[] { new ListRuntimeValue(list), _func });

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(new IntegerRuntimeValue(1), result.GetItem(0));
        Assert.AreEqual(new IntegerRuntimeValue(2), result.GetItem(1));
        Assert.AreEqual(new IntegerRuntimeValue(3), result.GetItem(2));
    }
}
