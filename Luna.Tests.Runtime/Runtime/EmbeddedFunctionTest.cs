using System;
using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class EmbeddedFunctionTest
{
    [EmbeddedFunction("test", "x")]
    class TestEmbeddedFunction : EmbeddedFunction
    {
        public override IRuntimeValue GetValue()
        {
            throw new NotImplementedException();
        }
    }

    private EmbeddedFunction _func;

    [SetUp]
    public void Setup()
    {
        _func = new TestEmbeddedFunction();
    }

    [Test]
    public void ArgumentsNotPassed()
    {
        try
        {
            var value = _func.GetValueOrError<IRuntimeValue>(0);
            Assert.Fail();
        }
        catch (RuntimeException exp)
        {
            Assert.AreEqual("Argument values have not been passed.", exp.Message);
        }
    }

    [Test]
    public void ArgumentCannotBeGet()
    {
        try
        {
            var value = new Mock<IRuntimeValue>();
            value.Setup(x => x.GetValue(default)).Returns(new IntegerRuntimeValue(123));
            var argumentValues = new IRuntimeValue[] { value.Object }.ToReadonlyArray();
            _func.SetArgumentValues(argumentValues);
            _func.GetValueOrError<StringRuntimeValue>(0);
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
        var value = new Mock<IRuntimeValue>();
        value.Setup(x => x.GetValue(default)).Returns(new IntegerRuntimeValue(123));
        var argumentValues = new IRuntimeValue[] { value.Object }.ToReadonlyArray();
        _func.SetArgumentValues(argumentValues);
        var argumentValue = _func.GetValueOrError<IntegerRuntimeValue>(0);
        Assert.AreEqual(123, argumentValue.Value);
    }
}
