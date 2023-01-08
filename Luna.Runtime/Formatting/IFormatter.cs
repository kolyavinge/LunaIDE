using Luna.Parsing;

namespace Luna.Formatting;

internal interface IFormatter
{
    void Format(TokenIterator iter, TokenAppender appender, ref int skippedLinesCount);
}
