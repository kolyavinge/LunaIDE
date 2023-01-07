using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Formatting;

public class CodeFormatter
{
    private readonly TokenIterator _iterator;
    private readonly CodeStringBuilder _formatted;
    private readonly HashSet<string> _importedFiles;
    private readonly MaxConstantLengthFinder.Result _maxConstantLength;
    private int _skippedLinesCount;

    public Token Token => _iterator.Token;
    public bool Eof => _iterator.Eof;

    public CodeFormatter(IReadOnlyCollection<Token> tokens)
    {
        _iterator = new TokenIterator(tokens, new() { SkipComments = false });
        _formatted = new CodeStringBuilder();
        _importedFiles = new HashSet<string>();
        var finder = new MaxConstantLengthFinder();
        _maxConstantLength = finder.FindMaxConstantLength(tokens);
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
            else if (Token.Kind == TokenKind.ConstDeclaration)
            {
                FormatConstant();
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
            Append(importToken.LineIndex, 0, "import");
            return;
        }
        MoveNext();
        if (importFileToken.Kind == TokenKind.String
            && (Eof || importToken.LineIndex == Token.LineIndex && Token.Kind == TokenKind.Comment || importToken.LineIndex != Token.LineIndex))
        {
            if (!_importedFiles.Contains(importFileToken.Name))
            {
                Append(importToken.LineIndex, 0, "import");
                Append(importFileToken.LineIndex, 7, importFileToken.Name);
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
            Append(importToken.LineIndex, 0, "import");
            Append(importFileToken.LineIndex, importFileToken.StartColumnIndex - importToken.StartColumnIndex, importFileToken.Name);
            AppendRecentTokensInLine(importToken.LineIndex, importToken.StartColumnIndex);
        }
    }

    private void FormatComment()
    {
        Append(Token.LineIndex - _skippedLinesCount, 0, Token.Name);
        MoveNext();
    }

    private void FormatConstant()
    {
        var constToken = Token;
        MoveNext();
        if (Token.Kind != TokenKind.Identificator)
        {
            Append(constToken.LineIndex, 0, "const");
            AppendRecentTokensInLine(constToken.LineIndex, constToken.StartColumnIndex);
            return;
        }
        var constNameToken = Token;
        MoveNext();
        if (Token.Kind is not TokenKind.IntegerNumber and not TokenKind.FloatNumber and not TokenKind.String)
        {
            Append(constToken.LineIndex, 0, "const");
            Append(constNameToken.LineIndex, constNameToken.StartColumnIndex - constToken.StartColumnIndex, constNameToken.Name);
            AppendRecentTokensInLine(constToken.LineIndex, constToken.StartColumnIndex);
            return;
        }
        var valueToken = Token;
        MoveNext();
        if (!(Eof
             || constToken.LineIndex == Token.LineIndex && Token.Kind == TokenKind.Comment
             || constToken.LineIndex != Token.LineIndex))
        {
            Append(constToken.LineIndex, 0, "const");
            Append(constNameToken.LineIndex, constNameToken.StartColumnIndex - constToken.StartColumnIndex, constNameToken.Name);
            Append(valueToken.LineIndex, valueToken.StartColumnIndex - constToken.StartColumnIndex, valueToken.Name);
            AppendRecentTokensInLine(constToken.LineIndex, constToken.StartColumnIndex);
            return;
        }
        Append(constToken.LineIndex, 0, "const");
        Append(constNameToken.LineIndex, 6, constNameToken.Name);
        if (constNameToken.Length < _maxConstantLength.Length)
        {
            if (_maxConstantLength.IsSigned && IsSigned(valueToken))
            {
                Append(constNameToken.LineIndex, _maxConstantLength.Length + 7, valueToken.Name);
            }
            else if (_maxConstantLength.IsSigned && !IsSigned(valueToken))
            {
                Append(constNameToken.LineIndex, _maxConstantLength.Length + 8, valueToken.Name);
            }
            else if (!_maxConstantLength.IsSigned && IsSigned(valueToken))
            {
                Append(constNameToken.LineIndex, _maxConstantLength.Length + 6, valueToken.Name);
            }
            else
            {
                Append(constNameToken.LineIndex, _maxConstantLength.Length + 7, valueToken.Name);
            }
        }
        else
        {
            Append(constNameToken.LineIndex, _maxConstantLength.Length + 7, valueToken.Name);
        }
        int columnsToSkip = Token.StartColumnIndex - (valueToken.StartColumnIndex + valueToken.Length) + 3;
        AppendRecentTokensInLine(constToken.LineIndex, columnsToSkip);
    }

    private void AppendRecentTokensInLine(int lineIndex, int skippedColumnsCount)
    {
        while (!Eof && Token.LineIndex == lineIndex)
        {
            Append(Token.LineIndex - _skippedLinesCount, Token.StartColumnIndex - skippedColumnsCount, Token.Name);
            MoveNext();
        }
    }

    private void Append(int lineIndex, int columnIndex, string text) => _formatted.Append(lineIndex, columnIndex, text);

    private bool IsSigned(Token token) => token.Name[0] == '+' || token.Name[0] == '-';

    private void MoveNext() => _iterator.MoveNext();
}
