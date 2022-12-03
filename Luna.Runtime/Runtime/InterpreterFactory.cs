using Luna.Output;
using Luna.ProjectModel;

namespace Luna.Runtime;

public interface IInterpreterFactory
{
    IInterpreter Make(IProject project, IRuntimeOutput output);
}

public class InterpreterFactory : IInterpreterFactory
{
    public IInterpreter Make(IProject project, IRuntimeOutput output)
    {
        var outputWriter = new OutputWriter(output);
        var codeModelBuilder = new CodeModelBuilder(outputWriter);
        var callStack = new CallStack();
        var runtimeExceptionHandler = new RuntimeExceptionHandler(callStack, outputWriter);
        var evaluator = new ValueElementEvaluator();

        return new Interpreter(
            project,
            outputWriter,
            codeModelBuilder,
            runtimeExceptionHandler,
            callStack,
            evaluator);
    }
}
