using Luna.Collections;
using Luna.Runtime;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class ListRuntimeValueComparerTest
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

    private ListRuntimeValueComparer _comparer;

    [SetUp]
    public void Setup()
    {
        _comparer = new ListRuntimeValueComparer(new IntegerCompareFunc());
    }

    [Test]
    public void Compare_XNull()
    {
        var result = _comparer.Compare(null, new IntegerRuntimeValue(2));
        Assert.AreEqual(1, result);
    }

    [Test]
    public void Compare_YNull()
    {
        var result = _comparer.Compare(new IntegerRuntimeValue(1), null);
        Assert.AreEqual(-1, result);
    }

    [Test]
    public void Compare_Nulls()
    {
        var result = _comparer.Compare(null, null);
        Assert.AreEqual(0, result);
    }

    [Test]
    public void Compare()
    {
        var result = _comparer.Compare(new IntegerRuntimeValue(1), new IntegerRuntimeValue(2));
        Assert.AreEqual(-1, result);

        result = _comparer.Compare(new IntegerRuntimeValue(2), new IntegerRuntimeValue(1));
        Assert.AreEqual(1, result);

        result = _comparer.Compare(new IntegerRuntimeValue(1), new IntegerRuntimeValue(1));
        Assert.AreEqual(0, result);
    }
}
