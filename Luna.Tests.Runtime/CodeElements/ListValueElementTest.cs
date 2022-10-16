using Luna.CodeElements;
using NUnit.Framework;

namespace Luna.Tests.CodeElements;

internal class ListValueElementTest
{
    [Test]
    public void Equals_Empty()
    {
        var list1 = new ListValueElement(new ValueElement[0]);
        var list2 = new ListValueElement(new ValueElement[0]);

        Assert.True(list1.Equals(list2));
        Assert.True(list2.Equals(list1));
    }

    [Test]
    public void Equals()
    {
        var list1 = new ListValueElement(new ValueElement[] { new IntegerValueElement(123) });
        var list2 = new ListValueElement(new ValueElement[] { new IntegerValueElement(123) });

        Assert.True(list1.Equals(list2));
        Assert.True(list2.Equals(list1));
    }

    [Test]
    public void Diff1()
    {
        var list1 = new ListValueElement(new ValueElement[] { new IntegerValueElement(123) });
        var list2 = new ListValueElement(new ValueElement[0]);

        Assert.False(list1.Equals(list2));
        Assert.False(list2.Equals(list1));
    }

    [Test]
    public void Diff2()
    {
        var list1 = new ListValueElement(new ValueElement[] { new IntegerValueElement(123) });
        var list2 = new ListValueElement(new ValueElement[] { new FloatValueElement(123) });

        Assert.False(list1.Equals(list2));
        Assert.False(list2.Equals(list1));
    }
}
