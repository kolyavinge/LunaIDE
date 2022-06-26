using System;
using Luna.Collections;
using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class AndTest : BaseFunctionTest<And>
{
    class ErrorBooleanRuntimeValue : BooleanRuntimeValue
    {
        public ErrorBooleanRuntimeValue() : base(false) { }
        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            throw new Exception();
        }
    }

    [Test]
    public void GetValue_1()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new BooleanRuntimeValue(true), new BooleanRuntimeValue(true) });
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_2()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new BooleanRuntimeValue(false), new ErrorBooleanRuntimeValue() });
        Assert.False(result.Value);
    }

    [Test]
    public void GetValue_3()
    {
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { new BooleanRuntimeValue(true), new BooleanRuntimeValue(false) });
        Assert.False(result.Value);
    }
}
