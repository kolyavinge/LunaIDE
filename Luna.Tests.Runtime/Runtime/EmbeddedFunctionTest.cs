using System;
using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;
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
}
