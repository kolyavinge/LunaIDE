﻿using System.Linq;
using Luna.Output;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.Runtime;

public interface IInterpreter
{
    void Run(Project project, IRuntimeOutput output);
}

public class Interpreter : IInterpreter
{
    public void Run(Project project, IRuntimeOutput output)
    {
        var codeFiles = project.Root.AllChildren.OfType<CodeFileProjectItem>().ToList();
        var outputWriter = new OutputWriter(output);
        var codeModelBuilder = new CodeModelBuilder(outputWriter);
        var codeModelBuilderResult = codeModelBuilder.BuildCodeModelsFor(codeFiles);
        if (codeModelBuilderResult.HasErrors) { outputWriter.ProgramStopped(); return; }
        var codeModels = codeFiles.Select(x => x.CodeModel!).ToList();
        var evaluator = new ValueElementEvaluator();
        var scopes = RuntimeScopesCollection.BuildForCodeModels(codeModels, evaluator);
        evaluator.Scopes = scopes;
        var main = codeModels.First(x => x.RunFunction != null);
        var mainScope = scopes.GetForCodeModel(main);
        var result = evaluator.Eval(mainScope, main.RunFunction!);
        outputWriter.ProgramResult(result);
    }
}
