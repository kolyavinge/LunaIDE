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
        Init();
    }

    [Test]
    public void ScopePushPopFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var argument = new IntegerRuntimeValue(123);
        var arguments = new IRuntimeValue[] { argument };

        MakeFunctionAndGetValue("func", arguments);

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

        var result = (StringRuntimeValue)MakeFunctionAndGetValue("func", arguments);

        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void EvalEmbeddedFunction()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        var arguments = new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray();
        _scope.Setup(x => x.IsEmbeddedFunction("func")).Returns(true);
        _scope.Setup(x => x.GetEmbeddedFunctionValue("func", arguments)).Returns(new StringRuntimeValue("123"));

        var result = (StringRuntimeValue)MakeFunctionAndGetValue("func", arguments);

        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void ToManyArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });

        MakeFunctionAndGetValue("func", new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2) });

        _runtimeExceptionHandler.Verify(x => x.Handle(new RuntimeException("Function func has too many passed arguments and cannot be evaluated.")), Times.Once());
    }

    [Test]
    public void LessArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x", "y" });
        var arguments = new IRuntimeValue[] { new IntegerRuntimeValue(123) };

        var result = (FunctionRuntimeValue)MakeFunctionAndGetValue("func", arguments);

        Assert.NotNull(result.AlreadyPassedArguments);
        Assert.AreEqual(1, result.AlreadyPassedArguments.Count);
        Assert.AreEqual(new IntegerRuntimeValue(123), result.AlreadyPassedArguments[0]);
    }

    [Test]
    public void AlreadyPassedArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x", "y" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new StringRuntimeValue("123"));

        var result = (StringRuntimeValue)MakeFunctionAndGetValue("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }, new IRuntimeValue[] { new BooleanRuntimeValue(true) }.ToReadonlyArray());

        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void AlreadyPassedArgumentsWithoutDuplicateChecking()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x", "y" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new StringRuntimeValue("123"));

        var result = (StringRuntimeValue)MakeFunctionAndGetValue("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }, new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray());

        Assert.AreEqual("123", result.Value);
    }

    [Test]
    public void ResultAsFunctionWithArguments()
    {
        _scope.Setup(x => x.GetFunctionArgumentNames("funcResult")).Returns(new[] { "x" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("funcResult")).Returns(new IntegerRuntimeValue(123));
        _scope.Setup(x => x.GetDeclaredFunctionValue("func")).Returns(new FunctionRuntimeValue("funcResult", _scope.Object));

        var result = (IntegerRuntimeValue)MakeFunctionAndGetValue("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray());

        Assert.AreEqual(123, result.IntegerValue);
    }

    [Test]
    public void GetValue_Exception()
    {
        MakeFunction("func");
        var rte = new RuntimeException("exception");
        _scope.Setup(x => x.PushCallStack(_function)).Throws(rte);

        var result = (VoidRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray());

        Assert.True(result is VoidRuntimeValue);
        _runtimeExceptionHandler.Verify(x => x.Handle(rte), Times.Once());
    }

    [Test]
    public void GetValue_Exception_NoExceptionHandler()
    {
        MakeFunction("func");
        var rte = new RuntimeException("exception");
        _scope.Setup(x => x.PushCallStack(_function)).Throws(rte);
        RuntimeEnvironment.ExceptionHandler = null;

        var result = (VoidRuntimeValue)GetValue(new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray());

        Assert.True(result is VoidRuntimeValue);
        _runtimeExceptionHandler.Verify(x => x.Handle(rte), Times.Never());
    }
}
