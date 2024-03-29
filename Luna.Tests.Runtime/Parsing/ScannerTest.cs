﻿using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class ScannerTest
{
    private Scanner _scanner;

    [SetUp]
    public void Setup()
    {
        _scanner = new Scanner();
    }

    [Test]
    public void ImportDirective()
    {
        var tokens = GetTokens("import 'file path'");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("import", 0, 0, TokenKind.ImportDirective), tokens[0]);
        Assert.AreEqual(new Token("'file path'", 0, 7, TokenKind.String), tokens[1]);
    }

    [Test]
    public void ImportDirective_EmptyFilePath()
    {
        var tokens = GetTokens("import ''");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("import", 0, 0, TokenKind.ImportDirective), tokens[0]);
        Assert.AreEqual(new Token("''", 0, 7, TokenKind.String), tokens[1]);
    }

    [Test]
    public void Const_IntegerNumber()
    {
        var tokens = GetTokens("const VALUE 123");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("123", 0, 12, TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void Const_PositiveIntegerNumber()
    {
        var tokens = GetTokens("const VALUE +123");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("+123", 0, 12, TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void Const_NegativeIntegerNumber()
    {
        var tokens = GetTokens("const VALUE -123");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("-123", 0, 12, TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void Const_FloatNumber()
    {
        var tokens = GetTokens("const VALUE 1.23");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("1.23", 0, 12, TokenKind.FloatNumber), tokens[2]);
    }

    [Test]
    public void Const_PositiveFloatNumber()
    {
        var tokens = GetTokens("const VALUE +1.23");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("+1.23", 0, 12, TokenKind.FloatNumber), tokens[2]);
    }

    [Test]
    public void Const_NegativeFloatNumber()
    {
        var tokens = GetTokens("const VALUE -1.23");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("-1.23", 0, 12, TokenKind.FloatNumber), tokens[2]);
    }

    [Test]
    public void Const_Comment()
    {
        var tokens = GetTokens("// comment\r\n");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("// comment", 0, 0, TokenKind.Comment), tokens[0]);
    }

    [Test]
    public void Function_CommentInFunction()
    {
        var tokens = GetTokens("(func (x//))");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("func", 0, 1, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 6, TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("x", 0, 7, TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token("//))", 0, 8, TokenKind.Comment), tokens[4]);
    }

    [Test]
    public void Const_CommentAfterIntegerNumber()
    {
        var tokens = GetTokens("12//");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("12", 0, 0, TokenKind.IntegerNumber), tokens[0]);
        Assert.AreEqual(new Token("//", 0, 2, TokenKind.Comment), tokens[1]);
    }

    [Test]
    public void Const_CommentAfterFloatNumber()
    {
        var tokens = GetTokens("12.0//");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("12.0", 0, 0, TokenKind.FloatNumber), tokens[0]);
        Assert.AreEqual(new Token("//", 0, 4, TokenKind.Comment), tokens[1]);
    }

    [Test]
    public void Const_IncorrectString()
    {
        var tokens = GetTokens("'");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("'", 0, 0, TokenKind.Unknown), tokens[0]);
    }

    [Test]
    public void Const_UncompleteString()
    {
        var tokens = GetTokens("'string\r\n");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("'string", 0, 0, TokenKind.Unknown), tokens[0]);
    }

    [Test]
    public void Const_BooleanTrue()
    {
        var tokens = GetTokens("true");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("true", 0, 0, TokenKind.BooleanTrue), tokens[0]);
    }

    [Test]
    public void Const_BooleanFalse()
    {
        var tokens = GetTokens("false");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("false", 0, 0, TokenKind.BooleanFalse), tokens[0]);
    }

    [Test]
    public void ListItemAccessor()
    {
        var tokens = GetTokens("xxx.yyy.zzz");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("xxx", 0, 0, TokenKind.Identificator), tokens[0]);
        Assert.AreEqual(new Token(".", 0, 3, TokenKind.Dot), tokens[1]);
        Assert.AreEqual(new Token("yyy", 0, 4, TokenKind.Identificator), tokens[2]);
        Assert.AreEqual(new Token(".", 0, 7, TokenKind.Dot), tokens[3]);
        Assert.AreEqual(new Token("zzz", 0, 8, TokenKind.Identificator), tokens[4]);
    }

    [Test]
    public void NamedListItem()
    {
        var tokens = GetTokens("x:1");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("x", 0, 0, TokenKind.Identificator), tokens[0]);
        Assert.AreEqual(new Token(":", 0, 1, TokenKind.Colon), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 2, TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void VariableSet()
    {
        var tokens = GetTokens("(set @var 1)");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("set", 0, 1, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("@var", 0, 5, TokenKind.Variable), tokens[2]);
        Assert.AreEqual(new Token("1", 0, 10, TokenKind.IntegerNumber), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 11, TokenKind.CloseBracket), tokens[4]);
    }

    [Test]
    public void FunctionDeclaration()
    {
        var tokens = GetTokens("(func (x y z) (funcCall 1 2))");
        Assert.AreEqual(13, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("func", 0, 1, TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 6, TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("x", 0, 7, TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token("y", 0, 9, TokenKind.Identificator), tokens[4]);
        Assert.AreEqual(new Token("z", 0, 11, TokenKind.Identificator), tokens[5]);
        Assert.AreEqual(new Token(")", 0, 12, TokenKind.CloseBracket), tokens[6]);
        Assert.AreEqual(new Token("(", 0, 14, TokenKind.OpenBracket), tokens[7]);
        Assert.AreEqual(new Token("funcCall", 0, 15, TokenKind.Identificator), tokens[8]);
        Assert.AreEqual(new Token("1", 0, 24, TokenKind.IntegerNumber), tokens[9]);
        Assert.AreEqual(new Token("2", 0, 26, TokenKind.IntegerNumber), tokens[10]);
        Assert.AreEqual(new Token(")", 0, 27, TokenKind.CloseBracket), tokens[11]);
        Assert.AreEqual(new Token(")", 0, 28, TokenKind.CloseBracket), tokens[12]);
    }

    [Test]
    public void FunctionCall_Add()
    {
        var tokens = GetTokens("(+ 1 2)");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("+", 0, 1, TokenKind.Plus), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 3, TokenKind.IntegerNumber), tokens[2]);
        Assert.AreEqual(new Token("2", 0, 5, TokenKind.IntegerNumber), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 6, TokenKind.CloseBracket), tokens[4]);
    }

    [Test]
    public void FunctionCall_Sub()
    {
        var tokens = GetTokens("(- 1 2)");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("-", 0, 1, TokenKind.Minus), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 3, TokenKind.IntegerNumber), tokens[2]);
        Assert.AreEqual(new Token("2", 0, 5, TokenKind.IntegerNumber), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 6, TokenKind.CloseBracket), tokens[4]);
    }

    [Test]
    public void FunctionCall_Mul()
    {
        var tokens = GetTokens("(* 1 2)");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("*", 0, 1, TokenKind.Asterisk), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 3, TokenKind.IntegerNumber), tokens[2]);
        Assert.AreEqual(new Token("2", 0, 5, TokenKind.IntegerNumber), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 6, TokenKind.CloseBracket), tokens[4]);
    }

    [Test]
    public void FunctionCall_Div()
    {
        var tokens = GetTokens("(/ 1 2)");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("/", 0, 1, TokenKind.Slash), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 3, TokenKind.IntegerNumber), tokens[2]);
        Assert.AreEqual(new Token("2", 0, 5, TokenKind.IntegerNumber), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 6, TokenKind.CloseBracket), tokens[4]);
    }

    [Test]
    public void FunctionCall_Remainder()
    {
        var tokens = GetTokens("(% 1 2)");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("%", 0, 1, TokenKind.Percent), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 3, TokenKind.IntegerNumber), tokens[2]);
        Assert.AreEqual(new Token("2", 0, 5, TokenKind.IntegerNumber), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 6, TokenKind.CloseBracket), tokens[4]);
    }

    [Test]
    public void RunFunction()
    {
        var tokens = GetTokens("(run (myfunc))");
        Assert.AreEqual(6, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("run", 0, 1, TokenKind.RunFunction), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 5, TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("myfunc", 0, 6, TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 12, TokenKind.CloseBracket), tokens[4]);
        Assert.AreEqual(new Token(")", 0, 13, TokenKind.CloseBracket), tokens[5]);
    }

    [Test]
    public void Unknown()
    {
        var tokens = GetTokens("^ @123 1x 2.3! a^");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("^", 0, 0, TokenKind.Unknown), tokens[0]);
        Assert.AreEqual(new Token("@123", 0, 2, TokenKind.Unknown), tokens[1]);
        Assert.AreEqual(new Token("1x", 0, 7, TokenKind.Unknown), tokens[2]);
        Assert.AreEqual(new Token("2.3!", 0, 10, TokenKind.Unknown), tokens[3]);
        Assert.AreEqual(new Token("a^", 0, 15, TokenKind.Unknown), tokens[4]);
    }

    private List<Token> GetTokens(string text)
    {
        return _scanner.GetTokens(new TextIterator(new Text(text))).ToList();
    }
}
