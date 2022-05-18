using System;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing;

public class ImportDirectiveParser : AbstractParser
{
    private readonly IImportDirectiveParserScope _scope;

    public ImportDirectiveParser(Text text, TokenIterator iter, CodeModel codeModel, IImportDirectiveParserScope scope)
        : base(text, iter, codeModel)
    {
        _scope = scope;
    }

    protected override void InnerParse()
    {
        while (!Eof && Token.Kind == TokenKind.ImportDirective)
        {
            var import = ParseImportDirective();
            if (import == null) return;
            _codeModel.AddImportDirective(import);
        }
    }

    private ImportDirective? ParseImportDirective()
    {
        string filePath;
        var importToken = Token;
        MoveNext();
        if (Eof || importToken.LineIndex != Token.LineIndex)
        {
            _result.SetError(ParserMessageType.ImportNoFilePath, importToken);
            return null;
        }
        else if (Token.Kind == TokenKind.IntegerNumber || Token.Kind == TokenKind.FloatNumber)
        {
            _result.SetError(ParserMessageType.ImportFilePathNotString, Token);
            return null;
        }
        else if (Token.Kind == TokenKind.String)
        {
            filePath = GetTokenName();
            if (String.IsNullOrWhiteSpace(filePath))
            {
                _result.SetError(ParserMessageType.ImportEmptyFilePath, Token);
                return null;
            }
        }
        else
        {
            _result.SetError(ParserMessageType.IncorrectTokenAfterImport, Token);
            return null;
        }
        MoveNext();
        var unexpectedTokens = GetRemainingTokens(importToken.LineIndex);
        if (unexpectedTokens.Any())
        {
            _result.SetError(ParserMessageType.UnexpectedToken, unexpectedTokens);
            return null;
        }
        var codeFile = _scope.GetCodeFileByPath(filePath);
        if (codeFile == null)
        {
            _result.SetError(ParserMessageType.ImportFileNotFound, importToken);
            return null;
        }

        return new ImportDirective(filePath, codeFile, importToken.LineIndex, importToken.StartColumnIndex);
    }
}
