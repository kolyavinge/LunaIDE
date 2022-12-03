using Luna.CodeElements;
using NUnit.Framework;

namespace Luna.Tests.CodeElements;

internal class LambdaValueElementTest
{
    [Test]
    public void Equals()
    {
        var lambda1 = new LambdaValueElement(new FunctionArgument[0], new());
        var lambda2 = new LambdaValueElement(new FunctionArgument[0], new());

        Assert.True(lambda1.Equals(lambda2));
        Assert.True(lambda2.Equals(lambda1));
    }

    [Test]
    public void DiffArguments1()
    {
        var lambda1 = new LambdaValueElement(new FunctionArgument[] { new("y"), new("x") }, new());
        var lambda2 = new LambdaValueElement(new FunctionArgument[] { new("x"), new("y") }, new());

        Assert.False(lambda1.Equals(lambda2));
        Assert.False(lambda2.Equals(lambda1));
    }

    [Test]
    public void DiffArguments2()
    {
        var lambda1 = new LambdaValueElement(new FunctionArgument[] { new("y") }, new());
        var lambda2 = new LambdaValueElement(new FunctionArgument[] { new("x") }, new());

        Assert.False(lambda1.Equals(lambda2));
        Assert.False(lambda2.Equals(lambda1));
    }

    [Test]
    public void DiffArguments3()
    {
        var lambda1 = new LambdaValueElement(new FunctionArgument[0], new());
        var lambda2 = new LambdaValueElement(new FunctionArgument[] { new("x") }, new());

        Assert.False(lambda1.Equals(lambda2));
        Assert.False(lambda2.Equals(lambda1));
    }
}
