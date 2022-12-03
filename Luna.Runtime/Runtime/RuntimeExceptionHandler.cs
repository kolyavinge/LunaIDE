using Luna.Functions.Lang;
using Luna.Functions.Windows;
using Luna.Output;
using Luna.Utils;

namespace Luna.Runtime;

internal interface IRuntimeExceptionHandler
{
    void Handle(RuntimeException rte);
}

internal class RuntimeExceptionHandler : IRuntimeExceptionHandler
{
    private readonly CallStack _callStack;
    private readonly IOutputWriter _outputWriter;

    public RuntimeExceptionHandler(CallStack callStack, IOutputWriter outputWriter)
    {
        _callStack = callStack;
        _outputWriter = outputWriter;
    }

    public void Handle(RuntimeException rte)
    {
        _outputWriter.WriteError(rte.Message);
        _outputWriter.WriteCallStack(_callStack);
        AppTimersCollection.Timers.Each(x => x.Value.Stop());
        AppWindowsCollection.Windows.Each(x => x.Close());
    }
}
