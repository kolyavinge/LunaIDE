using System;
using System.Linq;

namespace Luna.Parsing
{
    public class ImportDirectiveParser : AbstractParser
    {
        public ImportDirectiveParser(Text text, TokenIterator iter, CodeModel codeModel)
            : base(text, iter, codeModel) { }

        protected override void InnerParse()
        {
            while (!Eof && _token.Kind == TokenKind.ImportDirective)
            {
                var directive = ParseImportDirective();
                if (directive == null) return;
                _codeModel!.Imports.Add(directive);
            }
        }

        private ImportDirective? ParseImportDirective()
        {
            string filePath;
            var importToken = _token;
            MoveNext();
            if (Eof || importToken.LineIndex != _token.LineIndex)
            {
                _result.SetError(ParserMessageType.ImportNoFilePath, importToken);
                return null;
            }
            else if (_token.Kind == TokenKind.IntegerNumber || _token.Kind == TokenKind.FloatNumber)
            {
                _result.SetError(ParserMessageType.ImportFilePathNotString, _token);
                return null;
            }
            else if (_token.Kind == TokenKind.String)
            {
                filePath = GetTokenName();
                if (String.IsNullOrWhiteSpace(filePath))
                {
                    _result.SetError(ParserMessageType.ImportEmptyFilePath, _token);
                    return null;
                }
            }
            else
            {
                _result.SetError(ParserMessageType.ImportIncorrectTokenAfter, _token);
                return null;
            }
            MoveNext();
            var unexpectedTokens = GetRemainingTokens(importToken.LineIndex);
            if (unexpectedTokens.Any())
            {
                _result.SetError(ParserMessageType.UnexpectedToken, unexpectedTokens);
                return null;
            }

            return new ImportDirective(filePath, importToken.LineIndex, importToken.StartColumnIndex);
        }
    }
}
