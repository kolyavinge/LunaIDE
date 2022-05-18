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
    public void ScopeAddRemoveArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var argument = new IntegerRuntimeValue(123);
        var arguments = new IRuntimeValue[] { argument };
        Eval("func", arguments);
        _scope.Verify(x => x.AddFunctionArgument("x", argument));
        _scope.Verify(x => x.RemoveFunctionArgument("x"));
    }

    [Test]
    public void EvalDeclaredFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var arguments = new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray();
        _scope.Setup(x => x.GetDeclaredFunctionValue("func", arguments)).Returns(new StringRuntimeValue("123"));
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
        _scope.Setup(x => x.GetDeclaredFunctionValue("func", new IRuntimeValue[] { new BooleanRuntimeValue(true), new IntegerRuntimeValue(123) }.ToReadonlyArray())).Returns(new StringRuntimeValue("123"));
        var result = (StringRuntimeValue)Eval("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }, new IRuntimeValue[] { new BooleanRuntimeValue(true) }.ToReadonlyArray());
        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void ArgumentAsFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func", new IRuntimeValue[] { new BooleanRuntimeValue(false) }.ToReadonlyArray())).Returns(new BooleanRuntimeValue(false));
        _scope.Setup(x => x.ArgumentCalledAsFunction("x")).Returns(true);
        _scope.Setup(x => x.GetFunctionArgumentValue("x")).Returns(new FunctionRuntimeValue("func", _scope.Object));
        var result = (BooleanRuntimeValue)Eval("x", new IRuntimeValue[] { new BooleanRuntimeValue(false) });
        Assert.AreEqual(false, result.Value);
    }

    [Test]
    public void IsNotFunction()
    {
        _scope.Setup(x => x.ArgumentCalledAsFunction("x")).Returns(true);
        _scope.Setup(x => x.GetFunctionArgumentValue("x")).Returns(new BooleanRuntimeValue(false));
        try
        {
            Eval("x", new IRuntimeValue[0]);
            Assert.Fail();
        }
        catch (RuntimeException e)
        {
            Assert.AreEqual("Argument x is not a function and cannot be called.", e.Message);
            Assert.Pass();
        }
    }
}
