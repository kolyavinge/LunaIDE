using System.Collections.Generic;
using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class ImportFormatterTest : BaseFormatterTest
{
    protected override IFormatter MakeFormatter() => new ImportFormatter();

    protected override void Init(IFormatter formatter, IReadOnlyCollection<Token> tokens)
    {
        ((IImportFormatter)formatter).Init();
    }

    [Test]
    public void FormatImport()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("'file'", 0, 17, TokenKind.String)
        });
        AssertFormatted("import 'file'");
    }

    [Test]
    public void FormatImportCommentFirst()
    {
        Format(new Token[]
        {
            new("import", 1, 10, TokenKind.ImportDirective),
            new("'file'", 1, 17, TokenKind.String)
        });
        AssertFormatted("\r\nimport 'file'");
    }

    [Test]
    public void FormatImportFile()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("'file'", 0, 20, TokenKind.String)
        });
        AssertFormatted("import 'file'");
    }

    [Test]
    public void FormatImportWithComment()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("'file'", 0, 20, TokenKind.String),
            new("// comment", 0, 28, TokenKind.Comment)
        });
        AssertFormatted("import 'file'  // comment");
    }

    [Test]
    public void RemoveDuplicateImport()
    {
        Format(new Token[]
        {
            new("import", 0, 0, TokenKind.ImportDirective),
            new("'file'", 0, 7, TokenKind.String),
            new("import", 1, 0, TokenKind.ImportDirective),
            new("'file'", 1, 7, TokenKind.String)
        });
        AssertFormatted("import 'file'");
        Assert.That(_skippedLinesCount, Is.EqualTo(1));
    }

    [Test]
    public void NoRemoveDuplicateImportWithErrors_1()
    {
        Format(new Token[]
        {
            new("import", 0, 0, TokenKind.ImportDirective),
            new("'file'", 0, 7, TokenKind.String),
            new("'wrong'", 0, 14, TokenKind.String),
            new("import", 1, 0, TokenKind.ImportDirective),
            new("'file'", 1, 7, TokenKind.String)
        });
        AssertFormatted("import 'file' 'wrong'\r\nimport 'file'");
    }

    [Test]
    public void NoRemoveDuplicateImportWithErrors_2()
    {
        Format(new Token[]
        {
            new("123", 0, 0, TokenKind.IntegerNumber),
            new("import", 0, 4, TokenKind.ImportDirective),
            new("'file'", 0, 11, TokenKind.String),
            new("import", 1, 0, TokenKind.ImportDirective),
            new("'file'", 1, 7, TokenKind.String)
        });
        AssertFormatted("123 import 'file'\r\nimport 'file'");
    }

    [Test]
    public void FormatImportWrong_1()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("'file'", 0, 20, TokenKind.String),
            new("'file'", 0, 28, TokenKind.String),
            new("// comment", 0, 36, TokenKind.Comment)
        });
        AssertFormatted("import    'file'  'file'  // comment");
    }

    [Test]
    public void FormatImportWrong_2()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("import", 0, 20, TokenKind.ImportDirective)
        });
        AssertFormatted("import    import");
    }

    [Test]
    public void FormatImportWrong_3()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective)
        });
        AssertFormatted("import");
    }

    [Test]
    public void FormatImportWrong_4()
    {
        Format(new Token[]
        {
            new("import", 0, 3, TokenKind.ImportDirective),
            new("123", 0, 12, TokenKind.IntegerNumber)
        });
        AssertFormatted("import   123");
    }
}
