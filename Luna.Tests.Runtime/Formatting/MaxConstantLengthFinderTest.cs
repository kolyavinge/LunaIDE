using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class MaxConstantLengthFinderTest
{
    private MaxConstantLengthFinder _finder;

    [SetUp]
    public void Setup()
    {
        _finder = new MaxConstantLengthFinder();
    }

    [Test]
    public void FindMaxConstantLength_1()
    {
        var result = _finder.FindMaxConstantLength(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 11, TokenKind.IntegerNumber)
        });

        Assert.That(result.Length, Is.EqualTo(5));
        Assert.False(result.IsSigned);
    }

    [Test]
    public void FindMaxConstantLength_2()
    {
        var result = _finder.FindMaxConstantLength(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 11, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 6, TokenKind.Identificator),
            new("123", 1, 12, TokenKind.IntegerNumber)
        });

        Assert.That(result.Length, Is.EqualTo(6));
        Assert.False(result.IsSigned);
    }

    [Test]
    public void FindMaxConstantLength_2_PositiveSigned()
    {
        var result = _finder.FindMaxConstantLength(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 0, 6, TokenKind.Identificator),
            new("+123", 0, 12, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 1, 6, TokenKind.Identificator),
            new("123", 1, 11, TokenKind.IntegerNumber)
        });

        Assert.That(result.Length, Is.EqualTo(6));
        Assert.True(result.IsSigned);
    }

    [Test]
    public void FindMaxConstantLength_2_NegativeSigned()
    {
        var result = _finder.FindMaxConstantLength(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 0, 6, TokenKind.Identificator),
            new("-123", 0, 12, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 1, 6, TokenKind.Identificator),
            new("123", 1, 11, TokenKind.IntegerNumber)
        });

        Assert.That(result.Length, Is.EqualTo(6));
        Assert.True(result.IsSigned);
    }

    [Test]
    public void FindMaxConstantLength_WrongDeclaration()
    {
        var result = _finder.FindMaxConstantLength(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 11, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 6, TokenKind.Identificator),
            new("123", 1, 12, TokenKind.IntegerNumber),
            new("123", 1, 15, TokenKind.IntegerNumber),
        });

        Assert.That(result.Length, Is.EqualTo(5));
        Assert.False(result.IsSigned);
    }

    [Test]
    public void FindMaxConstantLength_WrongValue()
    {
        var result = _finder.FindMaxConstantLength(new Token[]
        {
            new("const", 0, 0, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, TokenKind.Identificator),
            new("123", 0, 11, TokenKind.IntegerNumber),
            new("const", 1, 0, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 6, TokenKind.Identificator),
            new("WRONG", 1, 12, TokenKind.Identificator)
        });

        Assert.That(result.Length, Is.EqualTo(5));
        Assert.False(result.IsSigned);
    }
}
