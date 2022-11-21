using Luna.Collections;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class ListRuntimeValueTest
{
    class TestFunction : FunctionRuntimeValue
    {
        public TestFunction() : base("TestFunction", new Mock<IRuntimeScope>().Object) { }
        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null) => new IntegerRuntimeValue(789);
    }

    [Test]
    public void String()
    {
        var listElement = new ListRuntimeValue(new IRuntimeValue[]
        {
            new BooleanRuntimeValue(true),
            new IntegerRuntimeValue(123),
            new FloatRuntimeValue(1.23),
            new StringRuntimeValue("123"),
            new ListRuntimeValue(new[] { new IntegerRuntimeValue(888) }),
            new TestFunction()
        });

        Assert.That(listElement.ToString(), Is.EqualTo("(true 123 1.23 '123' (888) TestFunction)"));
    }

    [Test]
    public void String_GetValue()
    {
        var listElement = new ListRuntimeValue(new IRuntimeValue[]
        {
            new BooleanRuntimeValue(true),
            new IntegerRuntimeValue(123),
            new FloatRuntimeValue(1.23),
            new StringRuntimeValue("123"),
            new ListRuntimeValue(new[] { new IntegerRuntimeValue(888) }),
            new TestFunction()
        }).GetValue();

        Assert.That(listElement.ToString(), Is.EqualTo("(true 123 1.23 '123' (888) 789)"));
    }

    [Test]
    public void GetValue()
    {
        var result = new ListRuntimeValue(new IRuntimeValue[]
        {
            new BooleanRuntimeValue(true),
            new IntegerRuntimeValue(123),
            new FloatRuntimeValue(1.23),
            new StringRuntimeValue("123"),
            new ListRuntimeValue(new[] { new IntegerRuntimeValue(888) }),
            new TestFunction()
        }).GetValue() as ListRuntimeValue;

        Assert.That(((BooleanRuntimeValue)result.GetItem(0)).Value, Is.EqualTo(true));
        Assert.That(((IntegerRuntimeValue)result.GetItem(1)).IntegerValue, Is.EqualTo(123));
        Assert.That(((FloatRuntimeValue)result.GetItem(2)).FloatValue, Is.EqualTo(1.23));
        Assert.That(((StringRuntimeValue)result.GetItem(3)).Value, Is.EqualTo("123"));
        Assert.That(((IntegerRuntimeValue)((ListRuntimeValue)result.GetItem(4)).GetItem(0)).IntegerValue, Is.EqualTo(888));
        Assert.That(((IntegerRuntimeValue)result.GetItem(5)).IntegerValue, Is.EqualTo(789));
    }
}
