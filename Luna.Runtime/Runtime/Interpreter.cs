using System.Linq;
using Luna.Output;
using Luna.ProjectModel;

namespace Luna.Runtime;

public interface IInterpreter
{
    void Run(IProject project, IRuntimeOutput output);
}

public class Interpreter : IInterpreter
{
    private readonly ValueElementEvaluator _evaluator;

    internal IRuntimeValue? Result { get; private set; }

    public Interpreter()
    {
        _evaluator = new ValueElementEvaluator();
    }

    public void Run(IProject project, IRuntimeOutput output)
    {
        var codeFiles = project.Root.AllChildren.OfType<CodeFileProjectItem>().ToList();
        var outputWriter = new OutputWriter(output);
        var codeModelBuilder = new CodeModelBuilder(outputWriter);
        var builderResult = codeModelBuilder.BuildFor(codeFiles);
        if (builderResult.HasErrors) { outputWriter.ProgramStopped(); return; }
        var codeModels = codeFiles.Select(x => x.CodeModel).ToList();
        var callStack = new CallStack();
        var scopes = RuntimeScopesCollection.BuildForCodeModels(codeModels, _evaluator, callStack);
        _evaluator.Scopes = scopes;
        RuntimeEnvironment.ExceptionHandler = new RuntimeExceptionHandler(callStack, outputWriter);
        var mainCodeModel = codeModels.First(x => x.RunFunction != null);
        var mainScope = scopes.GetForCodeModel(mainCodeModel);
        mainScope.PushCallStack(new RunFunctionStub());
        Result = _evaluator.Eval(mainScope, mainCodeModel.RunFunction!).GetValue();
        mainScope.PopCallStack();
        outputWriter.ProgramResult(Result!);
    }
}
