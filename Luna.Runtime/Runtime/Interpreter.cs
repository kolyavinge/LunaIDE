using System.Linq;
using Luna.Output;
using Luna.ProjectModel;

namespace Luna.Runtime;

public interface IInterpreter
{
    void Run(Project project, IRuntimeOutput output);
}

public class Interpreter : IInterpreter
{
    internal IRuntimeValue? Result { get; private set; }

    public void Run(Project project, IRuntimeOutput output)
    {
        var codeFiles = project.Root.AllChildren.OfType<CodeFileProjectItem>().ToList();
        var outputWriter = new OutputWriter(output);
        var codeModelBuilder = new CodeModelBuilder(outputWriter);
        var builderResult = codeModelBuilder.BuildFor(codeFiles);
        if (builderResult.HasErrors) { outputWriter.ProgramStopped(); return; }
        var codeModels = codeFiles.Select(x => x.CodeModel).ToList();
        var evaluator = new ValueElementEvaluator();
        var scopes = RuntimeScopesCollection.BuildForCodeModels(codeModels, evaluator);
        evaluator.Scopes = scopes;
        var main = codeModels.First(x => x.RunFunction != null);
        var mainScope = scopes.GetForCodeModel(main);
        Result = evaluator.Eval(mainScope, main.RunFunction!).GetValue();
        outputWriter.ProgramResult(Result!);
    }
}
