using Luna.Output;

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
    }
}

internal class EmptyRuntimeExceptionHandler : IRuntimeExceptionHandler
{
    public void Handle(RuntimeException rte) { }
}
