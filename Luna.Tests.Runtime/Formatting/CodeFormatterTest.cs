using System.Collections.Generic;
using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class CodeFormatterTest
{
    private CodeFormatter _formatter;
    private string _formatted;

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FormatComment()
    {
        Format(new Token[]
        {
            new("// comment", 0, 5, 10, TokenKind.Comment)
        });
        AssertFormatted("// comment\r\n");
    }

    [Test]
    public void FormatImport()
    {
        Format(new Token[]
        {
            new("import", 0, 10, 6, TokenKind.ImportDirective),
            new("'file'", 0, 17, 6, TokenKind.String)
        });
        AssertFormatted("import 'file'\r\n");
    }

    [Test]
    public void FormatImportCommentFirst()
    {
        Format(new Token[]
        {
            new("// comment", 0, 0, 10, TokenKind.Comment),
            new("import", 1, 10, 6, TokenKind.ImportDirective),
            new("'file'", 1, 17, 6, TokenKind.String)
        });
        AssertFormatted("// comment\r\nimport 'file'\r\n");
    }

    [Test]
    public void FormatImportFile()
    {
        Format(new Token[]
        {
            new("import", 0, 10, 6, TokenKind.ImportDirective),
            new("'file'", 0, 20, 6, TokenKind.String)
        });
        AssertFormatted("import 'file'\r\n");
    }

    [Test]
    public void FormatImportWithComment()
    {
        Format(new Token[]
        {
            new("import", 0, 10, 6, TokenKind.ImportDirective),
            new("'file'", 0, 20, 6, TokenKind.String),
            new("// comment", 0, 28, 10, TokenKind.Comment)
        });
        AssertFormatted("import 'file'  // comment\r\n");
    }

    [Test]
    public void RemoveDuplicateImport()
    {
        Format(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'file'", 0, 7, 6, TokenKind.String),
            new("import", 1, 0, 6, TokenKind.ImportDirective),
            new("'file'", 1, 7, 6, TokenKind.String),
            new("// comment", 2, 0, 10, TokenKind.Comment),
        });
        AssertFormatted("import 'file'\r\n// comment\r\n");
    }

    [Test]
    public void NoRemoveDuplicateImportWithErrors_1()
    {
        Format(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'file'", 0, 7, 6, TokenKind.String),
            new("'wrong'", 0, 14, 6, TokenKind.String),
            new("import", 1, 0, 6, TokenKind.ImportDirective),
            new("'file'", 1, 7, 6, TokenKind.String)
        });
        AssertFormatted("import 'file' 'wrong'\r\nimport 'file'\r\n");
    }

    [Test]
    public void NoRemoveDuplicateImportWithErrors_2()
    {
        Format(new Token[]
        {
            new("123", 0, 0, 3, TokenKind.IntegerNumber),
            new("import", 0, 4, 6, TokenKind.ImportDirective),
            new("'file'", 0, 11, 6, TokenKind.String),
            new("import", 1, 0, 6, TokenKind.ImportDirective),
            new("'file'", 1, 7, 6, TokenKind.String)
        });
        AssertFormatted("123 import 'file'\r\nimport 'file'\r\n");
    }

    [Test]
    public void FormatImportWrong_1()
    {
        Format(new Token[]
        {
            new("import", 0, 10, 6, TokenKind.ImportDirective),
            new("'file'", 0, 20, 6, TokenKind.String),
            new("'file'", 0, 28, 6, TokenKind.String),
            new("// comment", 0, 36, 10, TokenKind.Comment)
        });
        AssertFormatted("import    'file'  'file'  // comment\r\n");
    }

    [Test]
    public void FormatImportWrong_2()
    {
        Format(new Token[]
        {
            new("import", 0, 10, 6, TokenKind.ImportDirective),
            new("import", 0, 20, 6, TokenKind.ImportDirective)
        });
        AssertFormatted("import    import\r\n");
    }

    [Test]
    public void FormatImportWrong_3()
    {
        Format(new Token[]
        {
            new("import", 0, 3, 6, TokenKind.ImportDirective),
            new("'file'", 1, 5, 6, TokenKind.String)
        });
        AssertFormatted("import\r\n     'file'\r\n");
    }

    [Test]
    public void FormatImportWrong_4()
    {
        Format(new Token[]
        {
            new("import", 0, 3, 6, TokenKind.ImportDirective),
            new("123", 0, 12, 3, TokenKind.IntegerNumber)
        });
        AssertFormatted("import   123\r\n");
    }

    private void AssertFormatted(string expected)
    {
        Assert.That(_formatted, Is.EqualTo(expected));
    }

    private void Format(IEnumerable<Token> tokens)
    {
        _formatter = new CodeFormatter(tokens);
        _formatted = _formatter.Format();
    }
}
