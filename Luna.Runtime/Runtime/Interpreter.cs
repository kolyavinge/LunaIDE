using System.Linq;
using Luna.Output;
using Luna.ProjectModel;

namespace Luna.Runtime;

public interface IInterpreter
{
    void Run();
}

public class Interpreter : IInterpreter
{
    private readonly IProject _project;
    private readonly IOutputWriter _outputWriter;
    private readonly ICodeModelBuilder _codeModelBuilder;
    private readonly IRuntimeExceptionHandler _runtimeExceptionHandler;
    private readonly CallStack _callStack;
    private readonly IValueElementEvaluator _evaluator;

    internal IRuntimeValue? Result { get; private set; }

    internal Interpreter(
        IProject project,
        IOutputWriter outputWriter,
        ICodeModelBuilder codeModelBuilder,
        IRuntimeExceptionHandler runtimeExceptionHandler,
        CallStack callStack,
        IValueElementEvaluator evaluator)
    {
        _project = project;
        _outputWriter = outputWriter;
        _codeModelBuilder = codeModelBuilder;
        _runtimeExceptionHandler = runtimeExceptionHandler;
        _callStack = callStack;
        _evaluator = evaluator;
    }

    public void Run()
    {
        var codeFiles = _project.Root.AllChildren.OfType<CodeFileProjectItem>().ToList();
        var builderResult = _codeModelBuilder.BuildFor(codeFiles);
        if (builderResult.HasErrors) { _outputWriter.ProgramStopped(); return; }
        var codeModels = codeFiles.Select(x => x.CodeModel).ToList();
        var scopes = RuntimeScopesCollection.BuildForCodeModels(codeModels, _evaluator, _callStack);
        _evaluator.Scopes = scopes;
        RuntimeEnvironment.ExceptionHandler = _runtimeExceptionHandler;
        var mainCodeModel = codeModels.First(x => x.RunFunction != null);
        var mainScope = scopes.GetForCodeModel(mainCodeModel);
        mainScope.PushCallStack(new RunFunctionStub());
        Result = _evaluator.Eval(mainScope, mainCodeModel.RunFunction!).GetValue();
        mainScope.PopCallStack();
        _outputWriter.ProgramResult(Result!);
    }
}
