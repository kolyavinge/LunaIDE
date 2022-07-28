using System.Linq;
using Luna.Functions.Lang;
using Luna.Output;
using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class PrintFTest : BaseFunctionTest<PrintF>
{
    [Test]
    public void GetValue()
    {
        OutputMessage message = null;
        var output = new Mock<IRuntimeOutput>();
        output.Setup(x => x.SendMessage(It.IsAny<OutputMessage>())).Callback((OutputMessage m) => message = m);
        RuntimeEnvironment.StandartOutput = output.Object;
        var format = new StringRuntimeValue("format {0} {1} {2}");
        var param = new ListRuntimeValue(new IRuntimeValue[]
        {
            new IntegerRuntimeValue(123),
            new StringRuntimeValue("str"),
            new ListRuntimeValue(new [] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(2) }),
        });

        GetValue<VoidRuntimeValue>(format, param);

        Assert.AreEqual("format 123 'str' (1 2)", message.Text);
        Assert.AreEqual(1, message.Items.Count);
        Assert.AreEqual("format 123 'str' (1 2)", message.Items.First().Text);
        Assert.AreEqual(OutputMessageKind.Text, message.Items.First().Kind);
    }
}
