using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing
{
    internal interface ICodeFileParsingContext
    {
        CodeFileProjectItem CodeFile { get; }
        ParseResult? ImportDirectivesResult { get; }
        ParseResult? FunctionParserResult { get; }
        void ParseImports();
        void ParseFunctions();
    }

    internal class CodeFileParsingContext : ICodeFileParsingContext
    {
        private ImportDirectiveParser _importDirectiveParser;
        private FunctionParser _functionParser;

        public CodeFileProjectItem CodeFile { get; }
        public ParseResult? ImportDirectivesResult { get; private set; }
        public ParseResult? FunctionParserResult { get; private set; }

        public CodeFileParsingContext(IReadOnlyCollection<CodeFileProjectItem> allCodeFiles, CodeFileProjectItem currentCodeFile)
        {
            CodeFile = currentCodeFile;
            var text = new Text(CodeFile.GetText());
            var scanner = new Scanner();
            var tokens = scanner.GetTokens(new TextIterator(text)).ToList();
            var iter = new TokenIterator(tokens);
            _importDirectiveParser = new ImportDirectiveParser(text, iter, CodeFile.CodeModel!, new ImportDirectiveParserScope(allCodeFiles));
            _functionParser = new FunctionParser(text, iter, CodeFile.CodeModel!, new FunctionParserScope(allCodeFiles.Select(x => x.CodeModel!), currentCodeFile.CodeModel!));
        }

        public void ParseImports() => ImportDirectivesResult = _importDirectiveParser.Parse();

        public void ParseFunctions() => FunctionParserResult = _functionParser.Parse();
    }
}
