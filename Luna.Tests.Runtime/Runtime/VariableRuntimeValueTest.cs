using Luna.Runtime;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class VariableRuntimeValueTest
{
    private VariableRuntimeValue _var;

    [SetUp]
    public void Setup()
    {
        _var = new VariableRuntimeValue();
    }

    [Test]
    public void Init()
    {
        Assert.AreEqual(new VariableRuntimeValue(VoidRuntimeValue.Instance), _var.GetValue());
        Assert.AreEqual("void", _var.ToString());
    }

    [Test]
    public void SetValue()
    {
        var value = new FloatRuntimeValue(1.23);
        _var.SetValue(value);
        Assert.AreEqual(new VariableRuntimeValue(value), _var.GetValue());
    }
}
