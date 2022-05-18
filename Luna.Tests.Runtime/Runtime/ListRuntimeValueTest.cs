using Luna.Runtime;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class ListRuntimeValueTest
{
    [Test]
    public void String()
    {
        var listElement = new ListRuntimeValue(new IRuntimeValue[]
        {
            new BooleanRuntimeValue(true),
            new IntegerRuntimeValue(123),
            new FloatRuntimeValue(1.23),
            new StringRuntimeValue("123"),
            new ListRuntimeValue(new[] { new IntegerRuntimeValue(888) })
        });

        Assert.AreEqual("(true 123 1.23 '123' (888))", listElement.ToString());
    }
}
