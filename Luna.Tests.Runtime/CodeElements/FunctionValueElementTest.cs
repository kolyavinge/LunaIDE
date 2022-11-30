using Luna.CodeElements;
using NUnit.Framework;

namespace Luna.Tests.CodeElements;

internal class FunctionValueElementTest
{
    [Test]
    public void Equals()
    {
        var func1 = new FunctionValueElement("func", new ValueElement[0]);
        var func2 = new FunctionValueElement("func", new ValueElement[0]);

        Assert.True(func1.Equals(func2));
        Assert.True(func2.Equals(func1));
    }

    [Test]
    public void EqualsWithArguments()
    {
        var func1 = new FunctionValueElement("func", new ValueElement[] { new IntegerValueElement(1) });
        var func2 = new FunctionValueElement("func", new ValueElement[] { new IntegerValueElement(1) });

        Assert.True(func1.Equals(func2));
        Assert.True(func2.Equals(func1));
    }

    [Test]
    public void DiffArguments()
    {
        var func1 = new FunctionValueElement("func", new ValueElement[] { new IntegerValueElement(1) });
        var func2 = new FunctionValueElement("func", new ValueElement[] { new FloatValueElement(1) });

        Assert.False(func1.Equals(func2));
        Assert.False(func2.Equals(func1));
    }
}
