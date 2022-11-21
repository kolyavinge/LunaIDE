using Luna.Functions.Lang;
using Luna.Functions.Windows;
using Luna.Output;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class RuntimeExceptionHandlerTest
{
    private CallStack _callStack;
    private Mock<IOutputWriter> _outputWriter;
    private Mock<IAppWindow> _appWindow;
    private Mock<IAppTimer> _appTimer;
    private RuntimeExceptionHandler _handler;

    [SetUp]
    public void Setup()
    {
        _callStack = new CallStack();
        _outputWriter = new Mock<IOutputWriter>();
        _appWindow = new Mock<IAppWindow>();
        _appTimer = new Mock<IAppTimer>();
        _handler = new RuntimeExceptionHandler(_callStack, _outputWriter.Object);
    }

    [Test]
    public void Handle()
    {
        AppWindowsCollection.Windows.Add(_appWindow.Object);
        AppTimersCollection.Timers.Add(1, _appTimer.Object);

        _handler.Handle(new RuntimeException("exception"));

        _outputWriter.Verify(x => x.WriteError("exception"), Times.Once());
        _outputWriter.Verify(x => x.WriteCallStack(_callStack), Times.Once());
        _appWindow.Verify(x => x.Close(), Times.Once());
        _appTimer.Verify(x => x.Stop(), Times.Once());
    }
}
