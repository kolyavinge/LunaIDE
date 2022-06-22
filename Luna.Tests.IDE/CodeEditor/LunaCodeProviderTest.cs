using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Moq;
using NUnit.Framework;
using TokenKind = Luna.Parsing.TokenKind;

namespace Luna.Tests.IDE.CodeEditor;

public class LunaCodeProviderTest
{
    private Mock<ICodeProviderScope> _scope;
    private LunaCodeProvider _provider;

    [SetUp]
    public void Setup()
    {
        _scope = new Mock<ICodeProviderScope>();
        _provider = new LunaCodeProvider(_scope.Object);
    }

    [Test]
    public void ImportDirective()
    {
        var tokens = GetTokens("import 'file path'");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("import", 0, 0, 6, (byte)TokenKind.ImportDirective), tokens[0]);
        Assert.AreEqual(new Token("'file path'", 0, 7, 11, (byte)TokenKind.String), tokens[1]);
    }

    [Test]
    public void ImportDirective_EmptyFilePath()
    {
        var tokens = GetTokens("import ''");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("import", 0, 0, 6, (byte)TokenKind.ImportDirective), tokens[0]);
        Assert.AreEqual(new Token("''", 0, 7, 2, (byte)TokenKind.String), tokens[1]);
    }

    [Test]
    public void Const_IntegerNumber()
    {
        var tokens = GetTokens("const VALUE 123");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, 5, (byte)TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, 5, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("123", 0, 12, 3, (byte)TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void Const_PositiveIntegerNumber()
    {
        var tokens = GetTokens("const VALUE +123");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, 5, (byte)TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, 5, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("123", 0, 12, 3, (byte)TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void Const_NegativeIntegerNumber()
    {
        var tokens = GetTokens("const VALUE -123");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, 5, (byte)TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, 5, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("-123", 0, 12, 4, (byte)TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void Const_FloatNumber()
    {
        var tokens = GetTokens("const VALUE 1.23");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, 5, (byte)TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, 5, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("1.23", 0, 12, 4, (byte)TokenKind.FloatNumber), tokens[2]);
    }

    [Test]
    public void Const_PositiveFloatNumber()
    {
        var tokens = GetTokens("const VALUE +1.23");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, 5, (byte)TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, 5, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("1.23", 0, 12, 4, (byte)TokenKind.FloatNumber), tokens[2]);
    }

    [Test]
    public void Const_NegativeFloatNumber()
    {
        var tokens = GetTokens("const VALUE -1.23");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("const", 0, 0, 5, (byte)TokenKind.ConstDeclaration), tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 6, 5, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("-1.23", 0, 12, 5, (byte)TokenKind.FloatNumber), tokens[2]);
    }

    [Test]
    public void Const_Comment()
    {
        var tokens = GetTokens("// comment\r\n");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("// comment", 0, 0, 10, (byte)TokenKind.Comment), tokens[0]);
    }

    [Test]
    public void Const_CommentInFunction()
    {
        var tokens = GetTokens("(func (x//))");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, 1, (byte)TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("func", 0, 1, 4, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 6, 1, (byte)TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("x", 0, 7, 1, (byte)TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token("//))", 0, 8, 4, (byte)TokenKind.Comment), tokens[4]);
    }

    [Test]
    public void Const_CommentAfterIntegerNumber()
    {
        var tokens = GetTokens("12//");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("12", 0, 0, 2, (byte)TokenKind.IntegerNumber), tokens[0]);
        Assert.AreEqual(new Token("//", 0, 2, 2, (byte)TokenKind.Comment), tokens[1]);
    }

    [Test]
    public void Const_CommentAfterFloatNumber()
    {
        var tokens = GetTokens("12.0//");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(new Token("12.0", 0, 0, 4, (byte)TokenKind.FloatNumber), tokens[0]);
        Assert.AreEqual(new Token("//", 0, 4, 2, (byte)TokenKind.Comment), tokens[1]);
    }

    [Test]
    public void Const_BooleanTrue()
    {
        var tokens = GetTokens("true");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("true", 0, 0, 4, (byte)TokenKind.BooleanTrue), tokens[0]);
    }

    [Test]
    public void Const_BooleanFalse()
    {
        var tokens = GetTokens("false");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(new Token("false", 0, 0, 5, (byte)TokenKind.BooleanFalse), tokens[0]);
    }

    [Test]
    public void ListItemAccessor()
    {
        var tokens = GetTokens("xxx.yyy.zzz");
        Assert.AreEqual(5, tokens.Count);
        Assert.AreEqual(new Token("xxx", 0, 0, 3, (byte)TokenKind.Identificator), tokens[0]);
        Assert.AreEqual(new Token(".", 0, 3, 1, (byte)TokenKind.Dot), tokens[1]);
        Assert.AreEqual(new Token("yyy", 0, 4, 3, (byte)TokenKind.Identificator), tokens[2]);
        Assert.AreEqual(new Token(".", 0, 7, 1, (byte)TokenKind.Dot), tokens[3]);
        Assert.AreEqual(new Token("zzz", 0, 8, 3, (byte)TokenKind.Identificator), tokens[4]);
    }

    [Test]
    public void NamedListItem()
    {
        var tokens = GetTokens("x:1");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(new Token("x", 0, 0, 1, (byte)TokenKind.Identificator), tokens[0]);
        Assert.AreEqual(new Token(":", 0, 1, 1, (byte)TokenKind.Colon), tokens[1]);
        Assert.AreEqual(new Token("1", 0, 2, 1, (byte)TokenKind.IntegerNumber), tokens[2]);
    }

    [Test]
    public void FunctionDeclaration()
    {
        var tokens = GetTokens("(func (x y z) (funcCall 1 2))");
        Assert.AreEqual(13, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, 1, (byte)TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("func", 0, 1, 4, (byte)TokenKind.Identificator), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 6, 1, (byte)TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("x", 0, 7, 1, (byte)TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token("y", 0, 9, 1, (byte)TokenKind.Identificator), tokens[4]);
        Assert.AreEqual(new Token("z", 0, 11, 1, (byte)TokenKind.Identificator), tokens[5]);
        Assert.AreEqual(new Token(")", 0, 12, 1, (byte)TokenKind.CloseBracket), tokens[6]);
        Assert.AreEqual(new Token("(", 0, 14, 1, (byte)TokenKind.OpenBracket), tokens[7]);
        Assert.AreEqual(new Token("funcCall", 0, 15, 8, (byte)TokenKind.Identificator), tokens[8]);
        Assert.AreEqual(new Token("1", 0, 24, 1, (byte)TokenKind.IntegerNumber), tokens[9]);
        Assert.AreEqual(new Token("2", 0, 26, 1, (byte)TokenKind.IntegerNumber), tokens[10]);
        Assert.AreEqual(new Token(")", 0, 27, 1, (byte)TokenKind.CloseBracket), tokens[11]);
        Assert.AreEqual(new Token(")", 0, 28, 1, (byte)TokenKind.CloseBracket), tokens[12]);
    }

    [Test]
    public void FunctionDeclareScoped()
    {
        _scope.Setup(x => x.IsFunction("func")).Returns(true);
        _scope.Setup(x => x.IsFunction("funcCall")).Returns(true);
        var tokens = GetTokens("(func (x y z) (funcCall 1 2))");
        Assert.AreEqual(13, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, 1, (byte)TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("func", 0, 1, 4, (byte)TokenKindExtra.Function), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 6, 1, (byte)TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("x", 0, 7, 1, (byte)TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token("y", 0, 9, 1, (byte)TokenKind.Identificator), tokens[4]);
        Assert.AreEqual(new Token("z", 0, 11, 1, (byte)TokenKind.Identificator), tokens[5]);
        Assert.AreEqual(new Token(")", 0, 12, 1, (byte)TokenKind.CloseBracket), tokens[6]);
        Assert.AreEqual(new Token("(", 0, 14, 1, (byte)TokenKind.OpenBracket), tokens[7]);
        Assert.AreEqual(new Token("funcCall", 0, 15, 8, (byte)TokenKindExtra.Function), tokens[8]);
        Assert.AreEqual(new Token("1", 0, 24, 1, (byte)TokenKind.IntegerNumber), tokens[9]);
        Assert.AreEqual(new Token("2", 0, 26, 1, (byte)TokenKind.IntegerNumber), tokens[10]);
        Assert.AreEqual(new Token(")", 0, 27, 1, (byte)TokenKind.CloseBracket), tokens[11]);
        Assert.AreEqual(new Token(")", 0, 28, 1, (byte)TokenKind.CloseBracket), tokens[12]);
    }

    [Test]
    public void RunFunction()
    {
        var tokens = GetTokens("(run (myfunc))");
        Assert.AreEqual(6, tokens.Count);
        Assert.AreEqual(new Token("(", 0, 0, 1, (byte)TokenKind.OpenBracket), tokens[0]);
        Assert.AreEqual(new Token("run", 0, 1, 3, (byte)TokenKind.RunFunction), tokens[1]);
        Assert.AreEqual(new Token("(", 0, 5, 1, (byte)TokenKind.OpenBracket), tokens[2]);
        Assert.AreEqual(new Token("myfunc", 0, 6, 6, (byte)TokenKind.Identificator), tokens[3]);
        Assert.AreEqual(new Token(")", 0, 12, 1, (byte)TokenKind.CloseBracket), tokens[4]);
        Assert.AreEqual(new Token(")", 0, 13, 1, (byte)TokenKind.CloseBracket), tokens[5]);
    }

    [Test]
    public void Unknown()
    {
        var tokens = GetTokens("@123 1x 2.3!");
        Assert.AreEqual(4, tokens.Count);
        Assert.AreEqual(new Token("@", 0, 0, 1, (byte)TokenKind.Unknown), tokens[0]);
        Assert.AreEqual(new Token("123", 0, 1, 3, (byte)TokenKind.IntegerNumber), tokens[1]);
        Assert.AreEqual(new Token("1x", 0, 5, 2, (byte)TokenKind.Unknown), tokens[2]);
        Assert.AreEqual(new Token("2.3!", 0, 8, 4, (byte)TokenKind.Unknown), tokens[3]);
    }

    private List<Token> GetTokens(string text)
    {
        return _provider.GetTokens(TextIteratorBuilder.FromString(text)).ToList();
    }
}
