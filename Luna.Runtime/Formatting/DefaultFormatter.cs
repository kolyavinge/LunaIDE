using Luna.Parsing;

namespace Luna.Formatting;

internal interface IDefaultFormatter : IFormatter { }

internal class DefaultFormatter : IDefaultFormatter
{
    public void Format(TokenIterator iter, TokenAppender appender, ref int skippedLinesCount)
    {
        appender.AppendRecentTokensInLine(iter, iter.Token.LineIndex, skippedLinesCount, 0);
    }
}
