using System.Linq;
using Luna.CodeElements;
using Luna.ProjectModel;

namespace Luna.Parsing;

public class ImportDirectiveParser : AbstractParser
{
    private readonly IImportDirectiveParserScope _scope;

    public ImportDirectiveParser(TokenIterator iter, CodeModel codeModel, IImportDirectiveParserScope scope)
        : base(iter, codeModel)
    {
        _scope = scope;
    }

    protected override void InnerParse()
    {
        while (!Eof && Token.Kind == TokenKind.ImportDirective)
        {
            ParserMessage? error = null;
            ParseImportDirective(ref error);
            if (error != null)
            {
                _result.AddError(error);
            }
        }
    }

    private void ParseImportDirective(ref ParserMessage? error)
    {
        string filePath;
        var importToken = Token;
        MoveNext();
        if (Eof || importToken.LineIndex != Token.LineIndex)
        {
            error = new(ParserMessageType.ImportNoFilePath, importToken);
            return;
        }
        else if (Token.Kind == TokenKind.IntegerNumber || Token.Kind == TokenKind.FloatNumber)
        {
            error = new(ParserMessageType.ImportFilePathNotString, Token);
            SkipLine(importToken.LineIndex);
            return;
        }
        else if (Token.Kind == TokenKind.String)
        {
            filePath = GetTokenName();
            if (String.IsNullOrWhiteSpace(filePath))
            {
                error = new(ParserMessageType.ImportEmptyFilePath, Token);
                SkipLine(importToken.LineIndex);
                return;
            }
        }
        else
        {
            error = new(ParserMessageType.IncorrectTokenAfterImport, Token);
            SkipLine(importToken.LineIndex);
            return;
        }
        MoveNext();
        var unexpectedTokens = GetRemainingTokens(importToken.LineIndex);
        if (unexpectedTokens.Any())
        {
            error = new(ParserMessageType.UnexpectedToken, unexpectedTokens);
            return;
        }
        var codeFile = _scope.GetCodeFileByPath(filePath);
        if (codeFile == null)
        {
            error = new(ParserMessageType.ImportFileNotFound, importToken);
            return;
        }
        if (_codeModel.Imports.All(x => x.FilePath != filePath))
        {
            var import = new ImportDirective(filePath, codeFile, importToken.LineIndex, importToken.StartColumnIndex);
            _codeModel.AddImportDirective(import);
        }
        else
        {
            _result.AddWarning(new(ParserMessageType.DuplicateImport, importToken));
        }
    }
}
