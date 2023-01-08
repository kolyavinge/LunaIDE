using Luna.Parsing;

namespace Luna.Formatting;

internal interface ICommentFormatter : IFormatter { }

internal class CommentFormatter : ICommentFormatter
{
    public void Format(TokenIterator iter, TokenAppender appender, ref int skippedLinesCount)
    {
        appender.Append(iter.Token, 0, skippedLinesCount);
        iter.MoveNext();
    }
}
