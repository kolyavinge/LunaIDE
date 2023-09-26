using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Formatting;

internal interface IConstantFormatter : IFormatter
{
    void Init(IReadOnlyCollection<Token> tokens);
}

internal class ConstantFormatter : IConstantFormatter
{
    private MaxConstantLengthFinder.Result? _maxConstantLength;

    public void Init(IReadOnlyCollection<Token> tokens)
    {
        var finder = new MaxConstantLengthFinder();
        _maxConstantLength = finder.FindMaxConstantLength(tokens);
    }

    public void Format(TokenIterator iter, TokenAppender appender, ref int skippedLinesCount)
    {
        if (_maxConstantLength is null) throw new InvalidOperationException();
        var constToken = iter.Token;
        iter.MoveNext();
        if (iter.Token.Kind != TokenKind.Identificator)
        {
            appender.Append(constToken, 0, skippedLinesCount);
            appender.AppendRecentTokensInLine(iter, constToken.LineIndex, skippedLinesCount, constToken.StartColumnIndex);
            return;
        }
        var constNameToken = iter.Token;
        iter.MoveNext();
        if (iter.Token.Kind is not TokenKind.IntegerNumber and not TokenKind.FloatNumber and not TokenKind.String)
        {
            appender.Append(constToken, 0, skippedLinesCount);
            appender.Append(constNameToken, constNameToken.StartColumnIndex - constToken.StartColumnIndex, skippedLinesCount);
            appender.AppendRecentTokensInLine(iter, constToken.LineIndex, skippedLinesCount, constToken.StartColumnIndex);
            return;
        }
        var valueToken = iter.Token;
        iter.MoveNext();
        if (!(iter.Eof
             || constToken.LineIndex == iter.Token.LineIndex && iter.Token.Kind == TokenKind.Comment
             || constToken.LineIndex != iter.Token.LineIndex))
        {
            appender.Append(constToken, 0, skippedLinesCount);
            appender.Append(constNameToken, constNameToken.StartColumnIndex - constToken.StartColumnIndex, skippedLinesCount);
            appender.Append(valueToken, valueToken.StartColumnIndex - constToken.StartColumnIndex, skippedLinesCount);
            appender.AppendRecentTokensInLine(iter, constToken.LineIndex, skippedLinesCount, constToken.StartColumnIndex);
            return;
        }
        appender.Append(constToken, 0, skippedLinesCount);
        appender.Append(constNameToken, 6, skippedLinesCount);
        if (constNameToken.Length < _maxConstantLength.Length)
        {
            if (_maxConstantLength.IsSigned && IsSigned(valueToken))
            {
                appender.Append(valueToken, _maxConstantLength.Length + 7, skippedLinesCount);
            }
            else if (_maxConstantLength.IsSigned && !IsSigned(valueToken))
            {
                appender.Append(valueToken, _maxConstantLength.Length + 8, skippedLinesCount);
            }
            else if (!_maxConstantLength.IsSigned && IsSigned(valueToken))
            {
                appender.Append(valueToken, _maxConstantLength.Length + 6, skippedLinesCount);
            }
            else
            {
                appender.Append(valueToken, _maxConstantLength.Length + 7, skippedLinesCount);
            }
        }
        else
        {
            appender.Append(valueToken, _maxConstantLength.Length + 7, skippedLinesCount);
        }
        int skippedColumnsCount = iter.Token.StartColumnIndex - (valueToken.StartColumnIndex + valueToken.Length) + 3;
        appender.AppendRecentTokensInLine(iter, constToken.LineIndex, skippedLinesCount, skippedColumnsCount);
    }

    private bool IsSigned(Token token) => token.Name[0] == '+' || token.Name[0] == '-';
}
