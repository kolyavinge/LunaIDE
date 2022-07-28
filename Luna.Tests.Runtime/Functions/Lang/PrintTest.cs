using System.Linq;
using Luna.Collections;
using Luna.Functions.Lang;
using Luna.Output;
using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class PrintTest : BaseFunctionTest<Print>
{
    private OutputMessage _message;

    [SetUp]
    public void Setup()
    {
        var output = new Mock<IRuntimeOutput>();
        output.Setup(x => x.SendMessage(It.IsAny<OutputMessage>())).Callback((OutputMessage m) => _message = m);
        RuntimeEnvironment.StandartOutput = output.Object;
    }

    [Test]
    public void TextKindAndOneMessageItem()
    {
        GetValue<VoidRuntimeValue>(new StringRuntimeValue("text for output"));
        Assert.AreEqual(1, _message.Items.Count);
        Assert.AreEqual(OutputMessageKind.Text, _message.Items.First().Kind);
    }

    [Test]
    public void Void()
    {
        GetValue<VoidRuntimeValue>(VoidRuntimeValue.Instance);
        Assert.AreEqual("void", _message.Text);
    }

    [Test]
    public void Boolean()
    {
        GetValue<VoidRuntimeValue>(new BooleanRuntimeValue(true));
        Assert.AreEqual("true", _message.Text);
    }

    [Test]
    public void Integer()
    {
        GetValue<VoidRuntimeValue>(new IntegerRuntimeValue(123));
        Assert.AreEqual("123", _message.Text);
    }

    [Test]
    public void Float()
    {
        GetValue<VoidRuntimeValue>(new FloatRuntimeValue(1.23));
        Assert.AreEqual("1.23", _message.Text);
    }

    [Test]
    public void String()
    {
        GetValue<VoidRuntimeValue>(new StringRuntimeValue("text for output"));
        Assert.AreEqual("text for output", _message.Text);
    }

    [Test]
    public void List()
    {
        GetValue<VoidRuntimeValue>(new ListRuntimeValue(new[] { new IntegerRuntimeValue(789) }));
        Assert.AreEqual("(789)", _message.Text);
    }

    [Test]
    public void Variable()
    {
        GetValue<VoidRuntimeValue>(new VariableRuntimeValue(new IntegerRuntimeValue(789)));
        Assert.AreEqual("789", _message.Text);
    }

    [Test]
    public void Function()
    {
        GetValue<VoidRuntimeValue>(new TextFunctionRuntimeValue());
        Assert.AreEqual("result_func", _message.Text);
    }

    class TextFunctionRuntimeValue : FunctionRuntimeValue
    {
        public TextFunctionRuntimeValue() : base("", null) { }
        public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues = null)
        {
            return new FunctionRuntimeValue("result_func", null);
        }
    }
}
