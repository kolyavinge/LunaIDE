using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class AnyTest : BaseFunctionTest<Any>
{
    [Test]
    public void GetValue()
    {
        var list = new ListRuntimeValue(new[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2), new IntegerRuntimeValue(3) });
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { list });
        Assert.True(result.Value);
    }

    [Test]
    public void GetValue_Empty()
    {
        var list = new ListRuntimeValue(new IRuntimeValue[0]);
        var result = (BooleanRuntimeValue)GetValue(new IRuntimeValue[] { list });
        Assert.False(result.Value);
    }
}
