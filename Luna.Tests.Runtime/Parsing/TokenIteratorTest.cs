using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class TokenIteratorTest
{
    private TokenIterator _iterator;

    [Test]
    public void Import()
    {
        _iterator = new TokenIterator(new Token[]
        {
            new("import", 0, 0, TokenKind.ImportDirective),
            new("'file'", 0, 7, TokenKind.String)
        });

        var tokens = GetTokens().ToList();

        Assert.That(tokens.Count, Is.EqualTo(2));
        Assert.That(tokens[0].Name, Is.EqualTo("import"));
        Assert.That(tokens[1].Name, Is.EqualTo("'file'"));
    }

    [Test]
    public void ImportWithComments()
    {
        _iterator = new TokenIterator(new Token[]
        {
            new("import", 0, 0, TokenKind.ImportDirective),
            new("'file'", 0, 7, TokenKind.String),
            new("// comment", 0, 14, TokenKind.Comment)
        });

        var tokens = GetTokens().ToList();

        Assert.That(tokens.Count, Is.EqualTo(2));
        Assert.That(tokens[0].Name, Is.EqualTo("import"));
        Assert.That(tokens[1].Name, Is.EqualTo("'file'"));
    }

    [Test]
    public void ImportWithComments_NoSkip()
    {
        _iterator = new TokenIterator(new Token[]
        {
            new("import", 0, 0, TokenKind.ImportDirective),
            new("'file'", 0, 7, TokenKind.String),
            new("// comment", 0, 14, TokenKind.Comment)
        }, new() { SkipComments = false });

        var tokens = GetTokens().ToList();

        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0].Name, Is.EqualTo("import"));
        Assert.That(tokens[1].Name, Is.EqualTo("'file'"));
        Assert.That(tokens[2].Name, Is.EqualTo("// comment"));
    }

    private IEnumerable<Token> GetTokens()
    {
        while (!_iterator.Eof)
        {
            yield return _iterator.Token;
            _iterator.MoveNext();
        }
    }
}
