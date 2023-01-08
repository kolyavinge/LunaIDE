using System.Collections.Generic;
using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class ConstantFormatterTest : BaseFormatterTest
{
    protected override IFormatter MakeFormatter() => new ConstantFormatter();

    protected override void Init(IFormatter formatter, IReadOnlyCollection<Token> tokens)
    {
        ((IConstantFormatter)formatter).Init(tokens);
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
        AssertFormatted("const WIDTH 123");
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
        AssertFormatted("const WIDTH 123  // comment");
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
        AssertFormatted("const WIDTH  123\r\nconst HEIGHT 45");
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
        AssertFormatted("const WIDTH -123\r\nconst HEIGHT 45");
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
        AssertFormatted("const WIDTH +123\r\nconst HEIGHT 45");
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
        AssertFormatted("const WIDTH   123\r\nconst HEIGHT -45");
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
        AssertFormatted("const WIDTH  +123\r\nconst HEIGHT -45");
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
        AssertFormatted("const WIDTH   123\r\nconst HEIGHT +45");
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
        AssertFormatted("const WIDTH  -123\r\nconst HEIGHT -45");
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
        AssertFormatted("const WIDTH  +123\r\nconst HEIGHT +45");
    }

    [Test]
    public void FormatConst_Wrong_1()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration)
        });
        AssertFormatted("const");
    }

    [Test]
    public void FormatConst_Wrong_2()
    {
        Format(new Token[]
        {
            new("const", 0, 3, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 10, TokenKind.Identificator)
        });
        AssertFormatted("const  WIDTH");
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
        AssertFormatted("const  WIDTH  1  2");
    }
}
