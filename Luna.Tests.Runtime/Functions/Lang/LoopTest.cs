using System.Collections.Generic;
using System.Linq;
using Luna.Collections;
using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class LoopTest : BaseFunctionTest<Loop>
{
    class TestFunctionRuntimeValue : FunctionRuntimeValue
    {
        public TestFunctionRuntimeValue() : base("", null) { }

        public List<IRuntimeValue> Values { get; } = new();

        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            Values.Add(argumentValues.First());
            return argumentValues.First();
        }
    }

    private TestFunctionRuntimeValue _func;

    [SetUp]
    public void Setup()
    {
        Init();
        _func = new TestFunctionRuntimeValue();
    }

    [Test]
    public void GetValue()
    {
        GetValue<VoidRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(5), _func);
        Assert.AreEqual(_func.Values[0], new IntegerRuntimeValue(1));
        Assert.AreEqual(_func.Values[1], new IntegerRuntimeValue(2));
        Assert.AreEqual(_func.Values[2], new IntegerRuntimeValue(3));
        Assert.AreEqual(_func.Values[3], new IntegerRuntimeValue(4));
        Assert.AreEqual(_func.Values[4], new IntegerRuntimeValue(5));
    }

    [Test]
    public void CountMustBeGreaterThanZero()
    {
        GetValue<VoidRuntimeValue>(new IntegerRuntimeValue(1), new IntegerRuntimeValue(-1), _func);
        _exceptionHandler.Verify(x => x.Handle(new RuntimeException("Parameter count must be greater than zero.")), Times.Once());
    }
}
