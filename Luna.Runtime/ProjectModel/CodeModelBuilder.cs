using System.Collections.Generic;
using System.Linq;
using Luna.Output;
using Luna.Parsing;
using Luna.Utils;

namespace Luna.ProjectModel;

internal struct CodeModelBuilderResult
{
    public bool HasErrors { get; internal set; }
}

internal interface ICodeModelBuilder
{
    CodeModelBuilderResult BuildFor(IReadOnlyCollection<CodeFileProjectItem> codeFiles);
}

internal class CodeModelBuilder : ICodeModelBuilder
{
    private readonly IOutputWriter _outputWriter;
    private readonly ICodeFileParsingContextFactory _contextFactory;
    private readonly ICodeFileOrderLogic _orderLogic;
    private readonly ICodeModelUpdateRaiser _codeModelUpdateRaiser;

    public CodeModelBuilder(IOutputWriter outputWriter)
        : this(new CodeFileParsingContextFactory(), new CodeFileOrderLogic(), new CodeModelUpdateRaiser(), outputWriter) { }

    internal CodeModelBuilder(
        ICodeFileParsingContextFactory contextFactory,
        ICodeFileOrderLogic orderLogic,
        ICodeModelUpdateRaiser codeModelUpdateRaiser,
        IOutputWriter outputWriter)
    {
        _contextFactory = contextFactory;
        _orderLogic = orderLogic;
        _codeModelUpdateRaiser = codeModelUpdateRaiser;
        _outputWriter = outputWriter;
    }

    public CodeModelBuilderResult BuildFor(IReadOnlyCollection<CodeFileProjectItem> codeFiles)
    {
        _codeModelUpdateRaiser.StoreOldCodeModels(codeFiles);
        codeFiles.Each(x => x.CodeModel = new CodeModel());
        var contexts = codeFiles.Select(codeFile => _contextFactory.MakeContext(codeFiles, codeFile)).ToList();
        contexts.Each(x => x.ParseImports());
        var hasErrors = false;
        foreach (var context in contexts)
        {
            hasErrors |= WriteErrorsAndWarnings(context.CodeFile, context.ImportDirectivesResult!);
        }
        var orderedCodeFiles = _orderLogic.ByImports(codeFiles).ToList();
        var contextsDictionary = contexts.ToDictionary(k => k.CodeFile, v => v);
        orderedCodeFiles.Each(codeFile => contextsDictionary[codeFile].ParseFunctions());
        foreach (var context in contexts.Where(x => !x.ImportDirectivesResult!.Errors.Any() && !x.FunctionParserResult!.Errors.Any()))
        {
            _outputWriter.SuccessfullyParsed(context.CodeFile);
        }
        foreach (var context in contexts)
        {
            hasErrors |= WriteErrorsAndWarnings(context.CodeFile, context.FunctionParserResult!);
        }
        _codeModelUpdateRaiser.RaiseUpdateCodeModelWithDiff();
        if (hasErrors) return new() { HasErrors = true };

        return new() { HasErrors = false };
    }

    private bool WriteErrorsAndWarnings(CodeFileProjectItem codeFile, ParseResult parseResult)
    {
        parseResult.Warnings.Each(warning => _outputWriter.WriteWarning(codeFile, warning));
        parseResult.Errors.Each(error => _outputWriter.WriteError(codeFile, error));

        return parseResult.Errors.Any();
    }
}
