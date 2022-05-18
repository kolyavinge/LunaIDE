using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class NotTest : BaseFunctionTest<Not>
{
    [Test]
    public void GetValue()
    {
        var result = GetValue(new BooleanRuntimeValue(true));
        Assert.AreEqual(false, result.Value);

        result = GetValue(new BooleanRuntimeValue(false));
        Assert.AreEqual(true, result.Value);
    }
}
