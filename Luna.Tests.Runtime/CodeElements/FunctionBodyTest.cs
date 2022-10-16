using Luna.CodeElements;
using NUnit.Framework;

namespace Luna.Tests.CodeElements;

internal class FunctionBodyTest
{
    [Test]
    public void Equals_Empty()
    {
        var body1 = new FunctionBody();
        var body2 = new FunctionBody();

        Assert.True(body1.Equals(body2));
        Assert.True(body2.Equals(body1));
    }

    [Test]
    public void Equals()
    {
        var body1 = new FunctionBody();
        var body2 = new FunctionBody();

        body1.Add(new IntegerValueElement(123));
        body2.Add(new IntegerValueElement(123));

        Assert.True(body1.Equals(body2));
        Assert.True(body2.Equals(body1));
    }

    [Test]
    public void Diff()
    {
        var body1 = new FunctionBody();
        var body2 = new FunctionBody();

        body1.Add(new IntegerValueElement(123));
        body2.Add(new FloatValueElement(123));

        Assert.False(body1.Equals(body2));
        Assert.False(body2.Equals(body1));
    }
}
