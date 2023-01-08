using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class DefaultFormatterTest : BaseFormatterTest
{
    protected override IFormatter MakeFormatter() => new DefaultFormatter();

    [Test]
    public void Format()
    {
        Format(new Token[]
        {
            new("123", 0, 5, TokenKind.IntegerNumber),
            new("import", 0, 9, TokenKind.ImportDirective),
            new("'file'", 0, 16, TokenKind.String)
        });
        AssertFormatted("     123 import 'file'");
    }
}
