using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class CallStackTest
{
    private Mock<IFunctionRuntimeValue> _func;
    private CallStack _stack;

    [SetUp]
    public void Setup()
    {
        _func = new Mock<IFunctionRuntimeValue>();
        _stack = new CallStack();
    }

    [Test]
    public void Stackoverflow()
    {
        for (int i = 0; i < CallStack.MaxDepth; i++)
        {
            _stack.Push(_func.Object);
        }

        try
        {
            _stack.Push(_func.Object);
            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte.Message, Is.EqualTo("Stack overflow."));
        }
    }
}
