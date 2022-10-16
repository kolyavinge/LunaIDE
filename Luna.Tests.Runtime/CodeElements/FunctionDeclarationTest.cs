using Luna.CodeElements;
using NUnit.Framework;

namespace Luna.Tests.CodeElements;

internal class FunctionDeclarationTest
{
    [Test]
    public void Equals()
    {
        var fd1 = new FunctionDeclaration("func", new FunctionArgument[] { new("x"), new("y") }, new());
        var fd2 = new FunctionDeclaration("func", new FunctionArgument[] { new("x"), new("y") }, new());

        Assert.True(fd1.Equals(fd2));
        Assert.True(fd2.Equals(fd1));
    }

    [Test]
    public void DiffNames()
    {
        var fd1 = new FunctionDeclaration("func1", new FunctionArgument[] { new("x"), new("y") }, new());
        var fd2 = new FunctionDeclaration("func2", new FunctionArgument[] { new("x"), new("y") }, new());

        Assert.False(fd1.Equals(fd2));
        Assert.False(fd2.Equals(fd1));
    }

    [Test]
    public void DiffArguments1()
    {
        var fd1 = new FunctionDeclaration("func", new FunctionArgument[] { new("y"), new("x") }, new());
        var fd2 = new FunctionDeclaration("func", new FunctionArgument[] { new("x"), new("y") }, new());

        Assert.False(fd1.Equals(fd2));
        Assert.False(fd2.Equals(fd1));
    }

    [Test]
    public void DiffArguments2()
    {
        var fd1 = new FunctionDeclaration("func", new FunctionArgument[] { new("y") }, new());
        var fd2 = new FunctionDeclaration("func", new FunctionArgument[] { new("x") }, new());

        Assert.False(fd1.Equals(fd2));
        Assert.False(fd2.Equals(fd1));
    }

    [Test]
    public void DiffArguments3()
    {
        var fd1 = new FunctionDeclaration("func", new FunctionArgument[0], new());
        var fd2 = new FunctionDeclaration("func", new FunctionArgument[] { new("x") }, new());

        Assert.False(fd1.Equals(fd2));
        Assert.False(fd2.Equals(fd1));
    }
}
