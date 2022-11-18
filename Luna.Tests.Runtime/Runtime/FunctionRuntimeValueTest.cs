using Luna.Collections;
using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class FunctionRuntimeValueTest : BaseFunctionRuntimeValueTest
{
    [SetUp]
    public void Setup()
    {
        _scope = new Mock<IRuntimeScope>();
    }

    [Test]
    public void ScopePushPopFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var argument = new IntegerRuntimeValue(123);
        var arguments = new IRuntimeValue[] { argument };
        Eval("func", arguments);
        _scope.Verify(x => x.AddFunctionArgument("x", argument), Times.Once());
        _scope.Verify(x => x.PushCallStack(_function), Times.Once());
        _scope.Verify(x => x.PopCallStack(), Times.Once());
    }

    [Test]
    public void EvalDeclaredFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var arguments = new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray();
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new StringRuntimeValue("123"));
        var result = (StringRuntimeValue)Eval("func", arguments);
        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void EvalEmbeddedFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var arguments = new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray();
        _scope.Setup(x => x.IsEmbeddedFunction("func")).Returns(true);
        _scope.Setup(x => x.GetEmbeddedFunctionValue("func", arguments)).Returns(new StringRuntimeValue("123"));
        var result = (StringRuntimeValue)Eval("func", arguments);
        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void ToManyArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        try
        {
            Eval("func", new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2) });
            Assert.Fail();
        }
        catch (RuntimeException e)
        {
            Assert.AreEqual("Function func has too many passed arguments and cannot be evaluated.", e.Message);
            Assert.Pass();
        }
    }

    [Test]
    public void LessArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x", "y" });
        var arguments = new IRuntimeValue[] { new IntegerRuntimeValue(123) };
        var result = (FunctionRuntimeValue)Eval("func", arguments);
        Assert.NotNull(result.AlreadyPassedArguments);
        Assert.AreEqual(1, result.AlreadyPassedArguments.Count);
        Assert.AreEqual(new IntegerRuntimeValue(123), result.AlreadyPassedArguments[0]);
    }

    [Test]
    public void AlreadyPassedArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x", "y" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new StringRuntimeValue("123"));
        var result = (StringRuntimeValue)Eval("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }, new IRuntimeValue[] { new BooleanRuntimeValue(true) }.ToReadonlyArray());
        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void AlreadyPassedArgumentsWithoutDuplicateChecking()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x", "y" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new StringRuntimeValue("123"));
        var result = (StringRuntimeValue)Eval("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }, new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray());
        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void ResultAsFunctionWithArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("funcResult")).Returns(new[] { "x" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("funcResult")).Returns(new IntegerRuntimeValue(123));
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new FunctionRuntimeValue("funcResult", _scope.Object));
        var result = (IntegerRuntimeValue)Eval("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray());
        Assert.AreEqual(123, result.IntegerValue);
    }
}
