using Luna.Collections;
using Luna.Functions;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Functions;

internal class EmbeddedFunctionTest
{
    [EmbeddedFunctionDeclaration("test", "x")]
    class TestEmbeddedFunction : EmbeddedFunction
    {
        protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments) => new BooleanRuntimeValue(true);
    }

    [EmbeddedFunctionDeclaration("test", "x")]
    class ExceptionEmbeddedFunction : EmbeddedFunction
    {
        private readonly RuntimeException _rte;

        public ExceptionEmbeddedFunction(RuntimeException rte)
        {
            _rte = rte;
        }

        protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments) => throw _rte;
    }

    private Mock<IRuntimeExceptionHandler> _exceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _exceptionHandler = new Mock<IRuntimeExceptionHandler>();
        RuntimeEnvironment.ExceptionHandler = _exceptionHandler.Object;
    }

    [Test]
    public void GetValue()
    {
        var func = new TestEmbeddedFunction();

        var result = func.GetValue(new IRuntimeValue[0].ToReadonlyArray());

        Assert.True(result is BooleanRuntimeValue);
        _exceptionHandler.Verify(x => x.Handle(It.IsAny<RuntimeException>()), Times.Never());
    }

    [Test]
    public void GetValue_Exception()
    {
        var rte = new RuntimeException("exception");
        var func = new ExceptionEmbeddedFunction(rte);

        var result = func.GetValue(new IRuntimeValue[0].ToReadonlyArray());

        Assert.True(result is VoidRuntimeValue);
        _exceptionHandler.Verify(x => x.Handle(rte), Times.Once());
    }

    [Test]
    public void GetValue_Exception_NoExceptionHandler()
    {
        var rte = new RuntimeException("exception");
        var func = new ExceptionEmbeddedFunction(rte);
        RuntimeEnvironment.ExceptionHandler = null;

        var result = func.GetValue(new IRuntimeValue[0].ToReadonlyArray());

        Assert.True(result is VoidRuntimeValue);
    }
}
