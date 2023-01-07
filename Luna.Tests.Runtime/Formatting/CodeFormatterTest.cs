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
            new("// comment", 0, 5, TokenKind.Comment)
        });
        AssertFormatted("// comment\r\n");
    }

    [Test]
    public void FormatImport()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("'file'", 0, 17, TokenKind.String)
        });
        AssertFormatted("import 'file'\r\n");
    }

    [Test]
    public void FormatImportCommentFirst()
    {
        Format(new Token[]
        {
            new("// comment", 0, 0, TokenKind.Comment),
            new("import", 1, 10, TokenKind.ImportDirective),
            new("'file'", 1, 17, TokenKind.String)
        });
        AssertFormatted("// comment\r\nimport 'file'\r\n");
    }

    [Test]
    public void FormatImportFile()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("'file'", 0, 20, TokenKind.String)
        });
        AssertFormatted("import 'file'\r\n");
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
        AssertFormatted("import 'file'  // comment\r\n");
    }

    [Test]
    public void RemoveDuplicateImport()
    {
        Format(new Token[]
        {
            new("import", 0, 0, TokenKind.ImportDirective),
            new("'file'", 0, 7, TokenKind.String),
            new("import", 1, 0, TokenKind.ImportDirective),
            new("'file'", 1, 7, TokenKind.String),
            new("// comment", 2, 0, TokenKind.Comment),
        });
        AssertFormatted("import 'file'\r\n// comment\r\n");
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
        AssertFormatted("import 'file' 'wrong'\r\nimport 'file'\r\n");
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
        AssertFormatted("123 import 'file'\r\nimport 'file'\r\n");
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
        AssertFormatted("import    'file'  'file'  // comment\r\n");
    }

    [Test]
    public void FormatImportWrong_2()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective),
            new("import", 0, 20, TokenKind.ImportDirective)
        });
        AssertFormatted("import    import\r\n");
    }

    [Test]
    public void FormatImportWrong_3()
    {
        Format(new Token[]
        {
            new("import", 0, 10, TokenKind.ImportDirective)
        });
        AssertFormatted("import\r\n");
    }

    [Test]
    public void FormatImportWrong_4()
    {
        Format(new Token[]
        {
            new("import", 0, 3, TokenKind.ImportDirective),
            new("'file'", 1, 5, TokenKind.String)
        });
        AssertFormatted("import\r\n     'file'\r\n");
    }

    [Test]
    public void FormatImportWrong_5()
    {
        Format(new Token[]
        {
            new("import", 0, 3, TokenKind.ImportDirective),
            new("123", 0, 12, TokenKind.IntegerNumber)
        });
        AssertFormatted("import   123\r\n");
    }

    [Test]
    public void FormatConst()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 10, TokenKind.Identificator),
            new("123", 0, 17, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH 123\r\n");
    }

    [Test]
    public void FormatConstWithComment()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 10, TokenKind.Identificator),
            new("123", 0, 17, TokenKind.IntegerNumber),
            new("// comment", 0, 22, TokenKind.Comment)
        });
        AssertFormatted("const WIDTH 123  // comment\r\n");
    }

    [Test]
    public void FormatTwoConsts_NoSign()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH  123\r\nconst HEIGHT 45\r\n");
    }

    [Test]
    public void FormatTwoConsts_WithNegativeSign_1()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("-123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH -123\r\nconst HEIGHT 45\r\n");
    }

    [Test]
    public void FormatTwoConsts_WithPositiveSign_1()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("+123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH +123\r\nconst HEIGHT 45\r\n");
    }

    [Test]
    public void FormatTwoConsts_WithNegativeSign_2()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("-45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH   123\r\nconst HEIGHT -45\r\n");
    }

    [Test]
    public void FormatTwoConsts_WithNegativeSign_3()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("+123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("-45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH  +123\r\nconst HEIGHT -45\r\n");
    }

    [Test]
    public void FormatTwoConsts_WithPositiveSign_2()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("+45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH   123\r\nconst HEIGHT +45\r\n");
    }

    [Test]
    public void FormatTwoConsts_BothNegativeSigns()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("-123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("-45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH  -123\r\nconst HEIGHT -45\r\n");
    }

    [Test]
    public void FormatTwoConsts_BothPositiveSigns()
    {
        Format(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("+123", 0, 20, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 7, TokenKind.Identificator),
            new("+45", 1, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const WIDTH  +123\r\nconst HEIGHT +45\r\n");
    }

    [Test]
    public void FormatConst_Wrong_1()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration)
        });
        AssertFormatted("const\r\n");
    }

    [Test]
    public void FormatConst_Wrong_2()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 10, TokenKind.Identificator)
        });
        AssertFormatted("const  WIDTH\r\n");
    }

    [Test]
    public void FormatConst_Wrong_3()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 10, TokenKind.Identificator),
            new("1", 0, 17, TokenKind.IntegerNumber),
            new("2", 0, 20, TokenKind.IntegerNumber)
        });
        AssertFormatted("const  WIDTH  1  2\r\n");
    }

    private void AssertFormatted(string expected)
    {
        Assert.That(_formatted, Is.EqualTo(expected));
    }

    private void Format(IReadOnlyCollection<Token> tokens)
    {
        _formatter = new CodeFormatter(tokens);
        _formatted = _formatter.Format();
    }
}
