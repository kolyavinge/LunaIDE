using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class CommentFormatterTest : BaseFormatterTest
{
    protected override IFormatter MakeFormatter() => new CommentFormatter();

    [Test]
    public void FormatComment()
    {
        Format(new Token[]
        {
            new("// comment", 0, 5, TokenKind.Comment)
        });
        AssertFormatted("// comment");
    }
}
