using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Formatting;

public class CodeFormatter
{
    private readonly TokenIterator _iterator;
    private readonly CodeStringBuilder _formatted;
    private readonly HashSet<string> _importedFiles;
    private int _skippedLinesCount;

    public Token Token => _iterator.Token;
    public bool Eof => _iterator.Eof;

    public CodeFormatter(IEnumerable<Token> tokens)
    {
        _iterator = new TokenIterator(tokens, new() { SkipComments = false });
        _formatted = new CodeStringBuilder();
        _importedFiles = new HashSet<string>();
    }

    public string Format()
    {
        while (!Eof)
        {
            if (Token.Kind == TokenKind.ImportDirective)
            {
                FormatImport();
            }
            else if (Token.Kind == TokenKind.Comment)
            {
                FormatComment();
            }
            else
            {
                AppendRecentTokensInLine(Token.LineIndex, 0);
            }
        }
        _formatted.AppendLine();

        return _formatted.ToString();
    }

    private void FormatImport()
    {
        var importToken = Token;
        MoveNext();
        var importFileToken = Token;
        if (importToken.LineIndex != importFileToken.LineIndex)
        {
            _formatted.Append(importToken.LineIndex, 0, "import");
            return;
        }
        MoveNext();
        if (importFileToken.Kind == TokenKind.String
            && (Eof || importToken.LineIndex == Token.LineIndex && Token.Kind == TokenKind.Comment || importToken.LineIndex != Token.LineIndex))
        {
            if (!_importedFiles.Contains(importFileToken.Name))
            {
                _formatted.Append(importToken.LineIndex, 0, "import");
                _formatted.Append(importFileToken.LineIndex, 7, importFileToken.Name);
                _importedFiles.Add(importFileToken.Name);
                int columnsToSkip = importToken.StartColumnIndex + importFileToken.StartColumnIndex - (importToken.StartColumnIndex + importToken.Length) - 1;
                AppendRecentTokensInLine(importToken.LineIndex, columnsToSkip);
            }
            else
            {
                _skippedLinesCount++;
            }
        }
        else
        {
            _formatted.Append(importToken.LineIndex, 0, "import");
            int skippedColumnsCount = importToken.StartColumnIndex;
            _formatted.Append(importFileToken.LineIndex, importFileToken.StartColumnIndex - skippedColumnsCount, importFileToken.Name);
            AppendRecentTokensInLine(importToken.LineIndex, skippedColumnsCount);
        }
    }

    private void FormatComment()
    {
        _formatted.Append(Token.LineIndex - _skippedLinesCount, 0, Token.Name);
        MoveNext();
    }

    private void AppendRecentTokensInLine(int lineIndex, int skippedColumnsCount)
    {
        while (!Eof && Token.LineIndex == lineIndex)
        {
            _formatted.Append(Token.LineIndex - _skippedLinesCount, Token.StartColumnIndex - skippedColumnsCount, Token.Name);
            MoveNext();
        }
    }

    //private void SkipLine(int lineIndex)
    //{
    //    while (!Eof && Token.LineIndex == lineIndex)
    //    {
    //        MoveNext();
    //    }
    //}

    private void MoveNext() => _iterator.MoveNext();
}
