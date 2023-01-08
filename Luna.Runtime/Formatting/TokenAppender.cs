using Luna.Parsing;

namespace Luna.Formatting;

internal class TokenAppender
{
    private readonly CodeStringBuilder _formatted;

    public TokenAppender(CodeStringBuilder formatted)
    {
        _formatted = formatted;
    }

    public void Append(Token token, int columnIndex, int skippedLinesCount)
    {
        _formatted.Append(token.LineIndex - skippedLinesCount, columnIndex, token.Name);
    }

    public void AppendRecentTokensInLine(TokenIterator iter, int lineIndex, int skippedLinesCount, int skippedColumnsCount)
    {
        while (!iter.Eof && iter.Token.LineIndex == lineIndex)
        {
            _formatted.Append(iter.Token.LineIndex - skippedLinesCount, iter.Token.StartColumnIndex - skippedColumnsCount, iter.Token.Name);
            iter.MoveNext();
        }
    }
}
