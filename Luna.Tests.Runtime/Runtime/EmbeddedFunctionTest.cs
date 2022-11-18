using System;
using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class EmbeddedFunctionTest
{
    [EmbeddedFunctionDeclaration("test", "x")]
    class TestEmbeddedFunction : EmbeddedFunction
    {
        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
        {
            throw new NotImplementedException();
        }
    }

    class ArgumentFunction : FunctionRuntimeValue
    {
        public ArgumentFunction() : base("", new Mock<IRuntimeScope>().Object) { }

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
        {
            return new BooleanRuntimeValue(true);
        }
    }

    private EmbeddedFunction _func;

    [SetUp]
    public void Setup()
    {
        _func = new TestEmbeddedFunction();
    }

    [Test]
    public void ArgumentCannotBeGet()
    {
        try
        {
            var value = new IntegerRuntimeValue(123);
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            _func.GetValueOrError<StringRuntimeValue>(argumentValues, 0);
            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be get.", exp.Message);
        }
    }

    [Test]
    public void GetValueOrError()
    {
        var value = new IntegerRuntimeValue(123);
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        var argumentValue = _func.GetValueOrError<IntegerRuntimeValue>(argumentValues, 0);
        Assert.AreEqual(123, argumentValue.IntegerValue);
    }

    [Test]
    public void GetValueOrError_Variable()
    {
        var value = new VariableRuntimeValue(new IntegerRuntimeValue(123));
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        var argumentValue = _func.GetValueOrError<IntegerRuntimeValue>(argumentValues, 0);
        Assert.AreEqual(123, argumentValue.IntegerValue);
    }

    [Test]
    public void GetValueOrError_VariableWrongType()
    {
        try
        {
            var value = new VariableRuntimeValue(new IntegerRuntimeValue(123));
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            _func.GetValueOrError<StringRuntimeValue>(argumentValues, 0);
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be get.", exp.Message);
        }
    }

    [Test]
    public void GetFunctionOrError()
    {
        var value = new ArgumentFunction();
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        var argumentValue = _func.GetFunctionOrError(argumentValues, 0);
        Assert.True(argumentValue is FunctionRuntimeValue);
    }

    [Test]
    public void GetFunctionOrError_WrongType()
    {
        try
        {
            var value = new IntegerRuntimeValue(123);
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            var argumentValue = _func.GetFunctionOrError(argumentValues, 0);
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be get.", exp.Message);
        }
    }

    [Test]
    public void GetVariableOrError()
    {
        var value = new VariableRuntimeValue(new IntegerRuntimeValue(123));
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        var argumentValue = _func.GetVariableOrError(argumentValues, 0);
        Assert.True(argumentValue.Value is IntegerRuntimeValue);
        Assert.AreEqual(123, ((IntegerRuntimeValue)argumentValue.Value).IntegerValue);
    }

    [Test]
    public void GetVariableOrError_WrongType()
    {
        try
        {
            var value = new IntegerRuntimeValue(123);
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            var argumentValue = _func.GetVariableOrError(argumentValues, 0);
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be get.", exp.Message);
        }
    }
}
