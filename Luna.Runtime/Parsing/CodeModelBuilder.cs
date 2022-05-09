﻿using System.Collections.Generic;
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
        private readonly ICodeFileParsingContextFactory _contextFactory;
        private readonly ICodeFileOrderLogic _orderLogic;

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
            var hasErrors = false;
            foreach (var context in contexts)
            {
                hasErrors |= WriteErrorsAndWarnings(context.CodeFile, context.ImportDirectivesResult!);
            }
            if (hasErrors) return new() { HasErrors = true };
            var orderedCodeFiles = _orderLogic.ByImports(codeFiles).ToList();
            var contextsDictionary = contexts.ToDictionary(k => k.CodeFile, v => v);
            orderedCodeFiles.Each(codeFile => contextsDictionary[codeFile].ParseFunctions());
            foreach (var context in contexts.Where(x => x.FunctionParserResult?.Error == null))
            {
                _outputWriter.SuccessfullyParsed(context.CodeFile);
            }
            foreach (var context in contexts)
            {
                hasErrors |= WriteErrorsAndWarnings(context.CodeFile, context.FunctionParserResult!);
            }
            if (hasErrors) return new() { HasErrors = true };

            return new() { HasErrors = false };
        }

        private bool WriteErrorsAndWarnings(CodeFileProjectItem codeFile, ParseResult parseResult)
        {
            parseResult.Warnings.Each(warning => _outputWriter.WriteWarning(codeFile, warning));
            if (parseResult.Error != null)
            {
                _outputWriter.WriteError(codeFile, parseResult.Error);
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