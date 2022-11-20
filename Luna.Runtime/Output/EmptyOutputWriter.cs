using System.Collections.Generic;
using Luna.Parsing;
using Luna.ProjectModel;
using Luna.Runtime;

namespace Luna.Output;

internal class EmptyOutputWriter : IOutputWriter
{
    public void ProgramResult(IRuntimeValue runtimeValue) { }

    public void ProgramStopped() { }

    public void SuccessfullyParsed(CodeFileProjectItem codeFile) { }

    public void WriteCallStack(IEnumerable<IFunctionRuntimeValue> callStack) { }

    public void WriteError(CodeFileProjectItem codeFile, ParserMessage message) { }

    public void WriteError(string errorMessage) { }

    public void WriteWarning(CodeFileProjectItem codeFile, ParserMessage message) { }
}
