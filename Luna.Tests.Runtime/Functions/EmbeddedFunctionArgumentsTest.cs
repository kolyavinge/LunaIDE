using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions;

internal class EmbeddedFunctionArgumentsTest
{
    class ArgumentFunction : FunctionRuntimeValue
    {
        public ArgumentFunction() : base("", new Mock<IRuntimeScope>().Object) { }

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
        {
            return new BooleanRuntimeValue(true);
        }
    }

    private EmbeddedFunctionArguments _arguments;

    [Test]
    public void ArgumentCannotBeGotten()
    {
        try
        {
            var value = new IntegerRuntimeValue(123);
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            _arguments = new EmbeddedFunctionArguments(argumentValues);

            _arguments.GetValueOrError<StringRuntimeValue>(0);

            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be gotten.", exp.Message);
        }
    }

    [Test]
    public void GetValueOrError()
    {
        var value = new IntegerRuntimeValue(123);
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        _arguments = new EmbeddedFunctionArguments(argumentValues);

        var argumentValue = _arguments.GetValueOrError<IntegerRuntimeValue>(0);

        Assert.AreEqual(123, argumentValue.IntegerValue);
    }

    [Test]
    public void GetValueOrError_Variable()
    {
        var value = new VariableRuntimeValue(new IntegerRuntimeValue(123));
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        _arguments = new EmbeddedFunctionArguments(argumentValues);

        var argumentValue = _arguments.GetValueOrError<IntegerRuntimeValue>(0);

        Assert.AreEqual(123, argumentValue.IntegerValue);
    }

    [Test]
    public void GetValueOrError_VariableWrongType()
    {
        try
        {
            var value = new VariableRuntimeValue(new IntegerRuntimeValue(123));
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            _arguments = new EmbeddedFunctionArguments(argumentValues);

            _arguments.GetValueOrError<StringRuntimeValue>(0);

            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be gotten.", exp.Message);
        }
    }

    [Test]
    public void GetFunctionOrError()
    {
        var value = new ArgumentFunction();
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        _arguments = new EmbeddedFunctionArguments(argumentValues);

        var argumentValue = _arguments.GetFunctionOrError(0);

        Assert.True(argumentValue is FunctionRuntimeValue);
    }

    [Test]
    public void GetFunctionOrError_WrongType()
    {
        try
        {
            var value = new IntegerRuntimeValue(123);
            var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
            _arguments = new EmbeddedFunctionArguments(argumentValues);

            var argumentValue = _arguments.GetFunctionOrError(0);

            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be gotten.", exp.Message);
        }
    }

    [Test]
    public void GetVariableOrError()
    {
        var value = new VariableRuntimeValue(new IntegerRuntimeValue(123));
        var argumentValues = new IRuntimeValue[] { value }.ToReadonlyArray();
        _arguments = new EmbeddedFunctionArguments(argumentValues);

        var argumentValue = _arguments.GetVariableOrError(0);

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
            _arguments = new EmbeddedFunctionArguments(argumentValues);

            var argumentValue = _arguments.GetVariableOrError(0);

            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Embedded function argument cannot be gotten.", exp.Message);
        }
    }
}
