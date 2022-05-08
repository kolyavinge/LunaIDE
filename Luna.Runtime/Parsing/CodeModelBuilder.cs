using System.Collections.Generic;
using System.Linq;
using Luna.Output;
using Luna.ProjectModel;
using Luna.Utils;

namespace Luna.Parsing
{
    internal struct CodeModelBuilderResult
    {
        public bool HasErrors { get; internal set; }
    }

    internal class CodeModelBuilder
    {
        private readonly IOutputWriter _outputWriter;
        private ICodeFileParsingContextFactory _contextFactory;
        private ICodeFileOrderLogic _orderLogic;

        public CodeModelBuilder(IOutputWriter outputWriter) : this(new CodeFileParsingContextFactory(), new CodeFileOrderLogic(), outputWriter) { }

        internal CodeModelBuilder(ICodeFileParsingContextFactory contextFactory, ICodeFileOrderLogic orderLogic, IOutputWriter outputWriter)
        {
            _contextFactory = contextFactory;
            _orderLogic = orderLogic;
            _outputWriter = outputWriter;
        }

        public CodeModelBuilderResult BuildCodeModelsFor(IReadOnlyCollection<CodeFileProjectItem> codeFiles)
        {
            codeFiles.Each(x => x.CodeModel = new CodeModel());
            var contexts = codeFiles.Select(codeFile => _contextFactory.MakeContext(codeFiles, codeFile)).ToList();
            contexts.Each(x => x.ParseImports());
            if (WriteErrorsAndWarnings(contexts.Select(x => x.ImportDirectivesResult!).ToList()))
            {
                return new() { HasErrors = true };
            }
            var orderedCodeFiles = _orderLogic.ByImports(codeFiles).ToList();
            var contextsDictionary = contexts.ToDictionary(k => k.CodeFile, v => v);
            orderedCodeFiles.Each(codeFile => contextsDictionary[codeFile].ParseFunctions());
            if (WriteErrorsAndWarnings(contexts.Select(x => x.FunctionParserResult!).ToList()))
            {
                return new() { HasErrors = true };
            }

            return new() { HasErrors = false };
        }

        private bool WriteErrorsAndWarnings(List<ParseResult> parseResults)
        {
            parseResults.SelectMany(x => x.Warnings).Each(_outputWriter.WriteParserMessage);
            var errorMessage = parseResults.FirstOrDefault(x => x.Error != null);
            if (errorMessage != null)
            {
                _outputWriter.WriteParserMessage(errorMessage.Error!);
                return true;
            }

            return false;
        }
    }

    internal interface ICodeFileParsingContextFactory
    {
        ICodeFileParsingContext MakeContext(IReadOnlyCollection<CodeFileProjectItem> allCodeFiles, CodeFileProjectItem currentCodeFile);
    }

    internal class CodeFileParsingContextFactory : ICodeFileParsingContextFactory
    {
        public ICodeFileParsingContext MakeContext(IReadOnlyCollection<CodeFileProjectItem> allCodeFiles, CodeFileProjectItem currentCodeFile)
            => new CodeFileParsingContext(allCodeFiles, currentCodeFile);
    }
}
