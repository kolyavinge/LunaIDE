using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.CodeProvidering;
using Luna.IDE.CodeEditing;
using Moq;
using NUnit.Framework;
using TokenKind = Luna.Parsing.TokenKind;

namespace Luna.Tests.IDE.CodeEditing;

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
        Assert.That(tokens.Count, Is.EqualTo(2));
        Assert.That(tokens[0], Is.EqualTo(new Token("import", 0, 0, (byte)TokenKind.ImportDirective)));
        Assert.That(tokens[1], Is.EqualTo(new Token("'file path'", 0, 7, (byte)TokenKind.String)));
    }

    [Test]
    public void ImportDirective_EmptyFilePath()
    {
        var tokens = GetTokens("import ''");
        Assert.That(tokens.Count, Is.EqualTo(2));
        Assert.That(tokens[0], Is.EqualTo(new Token("import", 0, 0, (byte)TokenKind.ImportDirective)));
        Assert.That(tokens[1], Is.EqualTo(new Token("''", 0, 7, (byte)TokenKind.String)));
    }

    [Test]
    public void Const_IntegerNumber()
    {
        var tokens = GetTokens("const VALUE 123");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("const", 0, 0, (byte)TokenKind.ConstDeclaration)));
        Assert.That(tokens[1], Is.EqualTo(new Token("VALUE", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("123", 0, 12, (byte)TokenKind.IntegerNumber)));
    }

    [Test]
    public void Const_PositiveIntegerNumber()
    {
        var tokens = GetTokens("const VALUE +123");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("const", 0, 0, (byte)TokenKind.ConstDeclaration)));
        Assert.That(tokens[1], Is.EqualTo(new Token("VALUE", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("+123", 0, 12, (byte)TokenKind.IntegerNumber)));
    }

    [Test]
    public void Const_NegativeIntegerNumber()
    {
        var tokens = GetTokens("const VALUE -123");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("const", 0, 0, (byte)TokenKind.ConstDeclaration)));
        Assert.That(tokens[1], Is.EqualTo(new Token("VALUE", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("-123", 0, 12, (byte)TokenKind.IntegerNumber)));
    }

    [Test]
    public void Const_FloatNumber()
    {
        var tokens = GetTokens("const VALUE 1.23");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("const", 0, 0, (byte)TokenKind.ConstDeclaration)));
        Assert.That(tokens[1], Is.EqualTo(new Token("VALUE", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("1.23", 0, 12, (byte)TokenKind.FloatNumber)));
    }

    [Test]
    public void Const_PositiveFloatNumber()
    {
        var tokens = GetTokens("const VALUE +1.23");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("const", 0, 0, (byte)TokenKind.ConstDeclaration)));
        Assert.That(tokens[1], Is.EqualTo(new Token("VALUE", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("+1.23", 0, 12, (byte)TokenKind.FloatNumber)));
    }

    [Test]
    public void Const_NegativeFloatNumber()
    {
        var tokens = GetTokens("const VALUE -1.23");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("const", 0, 0, (byte)TokenKind.ConstDeclaration)));
        Assert.That(tokens[1], Is.EqualTo(new Token("VALUE", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("-1.23", 0, 12, (byte)TokenKind.FloatNumber)));
    }

    [Test]
    public void Const_Comment()
    {
        var tokens = GetTokens("// comment\r\n");
        Assert.That(tokens.Count, Is.EqualTo(1));
        Assert.That(tokens[0], Is.EqualTo(new Token("// comment", 0, 0, (byte)TokenKind.Comment)));
    }

    [Test]
    public void Const_CommentInFunction()
    {
        var tokens = GetTokens("(func (x//))");
        Assert.That(tokens.Count, Is.EqualTo(5));
        Assert.That(tokens[0], Is.EqualTo(new Token("(", 0, 0, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[1], Is.EqualTo(new Token("func", 0, 1, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("(", 0, 6, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[3], Is.EqualTo(new Token("x", 0, 7, (byte)TokenKind.Identificator)));
        Assert.That(tokens[4], Is.EqualTo(new Token("//))", 0, 8, (byte)TokenKind.Comment)));
    }

    [Test]
    public void Const_CommentAfterIntegerNumber()
    {
        var tokens = GetTokens("12//");
        Assert.That(tokens.Count, Is.EqualTo(2));
        Assert.That(tokens[0], Is.EqualTo(new Token("12", 0, 0, (byte)TokenKind.IntegerNumber)));
        Assert.That(tokens[1], Is.EqualTo(new Token("//", 0, 2, (byte)TokenKind.Comment)));
    }

    [Test]
    public void Const_CommentAfterFloatNumber()
    {
        var tokens = GetTokens("12.0//");
        Assert.That(tokens.Count, Is.EqualTo(2));
        Assert.That(tokens[0], Is.EqualTo(new Token("12.0", 0, 0, (byte)TokenKind.FloatNumber)));
        Assert.That(tokens[1], Is.EqualTo(new Token("//", 0, 4, (byte)TokenKind.Comment)));
    }

    [Test]
    public void Const_BooleanTrue()
    {
        var tokens = GetTokens("true");
        Assert.That(tokens.Count, Is.EqualTo(1));
        Assert.That(tokens[0], Is.EqualTo(new Token("true", 0, 0, (byte)TokenKind.BooleanTrue)));
    }

    [Test]
    public void Const_BooleanFalse()
    {
        var tokens = GetTokens("false");
        Assert.That(tokens.Count, Is.EqualTo(1));
        Assert.That(tokens[0], Is.EqualTo(new Token("false", 0, 0, (byte)TokenKind.BooleanFalse)));
    }

    [Test]
    public void ListItemAccessor()
    {
        var tokens = GetTokens("xxx.yyy.zzz");
        Assert.That(tokens.Count, Is.EqualTo(5));
        Assert.That(tokens[0], Is.EqualTo(new Token("xxx", 0, 0, (byte)TokenKind.Identificator)));
        Assert.That(tokens[1], Is.EqualTo(new Token(".", 0, 3, (byte)TokenKind.Dot)));
        Assert.That(tokens[2], Is.EqualTo(new Token("yyy", 0, 4, (byte)TokenKind.Identificator)));
        Assert.That(tokens[3], Is.EqualTo(new Token(".", 0, 7, (byte)TokenKind.Dot)));
        Assert.That(tokens[4], Is.EqualTo(new Token("zzz", 0, 8, (byte)TokenKind.Identificator)));
    }

    [Test]
    public void NamedListItem()
    {
        var tokens = GetTokens("x:1");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("x", 0, 0, (byte)TokenKind.Identificator)));
        Assert.That(tokens[1], Is.EqualTo(new Token(":", 0, 1, (byte)TokenKind.Colon)));
        Assert.That(tokens[2], Is.EqualTo(new Token("1", 0, 2, (byte)TokenKind.IntegerNumber)));
    }

    [Test]
    public void VariableSet()
    {
        var tokens = GetTokens("(set @var 1)");
        Assert.That(tokens.Count, Is.EqualTo(5));
        Assert.That(tokens[0], Is.EqualTo(new Token("(", 0, 0, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[1], Is.EqualTo(new Token("set", 0, 1, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("@var", 0, 5, (byte)TokenKind.Variable)));
        Assert.That(tokens[3], Is.EqualTo(new Token("1", 0, 10, (byte)TokenKind.IntegerNumber)));
        Assert.That(tokens[4], Is.EqualTo(new Token(")", 0, 11, (byte)TokenKind.CloseBracket)));
    }

    [Test]
    public void FunctionDeclaration()
    {
        var tokens = GetTokens("(func (x y z) (funcCall 1 2))");
        Assert.That(tokens.Count, Is.EqualTo(13));
        Assert.That(tokens[0], Is.EqualTo(new Token("(", 0, 0, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[1], Is.EqualTo(new Token("func", 0, 1, (byte)TokenKind.Identificator)));
        Assert.That(tokens[2], Is.EqualTo(new Token("(", 0, 6, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[3], Is.EqualTo(new Token("x", 0, 7, (byte)TokenKind.Identificator)));
        Assert.That(tokens[4], Is.EqualTo(new Token("y", 0, 9, (byte)TokenKind.Identificator)));
        Assert.That(tokens[5], Is.EqualTo(new Token("z", 0, 11, (byte)TokenKind.Identificator)));
        Assert.That(tokens[6], Is.EqualTo(new Token(")", 0, 12, (byte)TokenKind.CloseBracket)));
        Assert.That(tokens[7], Is.EqualTo(new Token("(", 0, 14, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[8], Is.EqualTo(new Token("funcCall", 0, 15, (byte)TokenKind.Identificator)));
        Assert.That(tokens[9], Is.EqualTo(new Token("1", 0, 24, (byte)TokenKind.IntegerNumber)));
        Assert.That(tokens[10], Is.EqualTo(new Token("2", 0, 26, (byte)TokenKind.IntegerNumber)));
        Assert.That(tokens[11], Is.EqualTo(new Token(")", 0, 27, (byte)TokenKind.CloseBracket)));
        Assert.That(tokens[12], Is.EqualTo(new Token(")", 0, 28, (byte)TokenKind.CloseBracket)));
    }

    [Test]
    public void FunctionDeclareScoped()
    {
        _scope.Setup(x => x.IsFunction("func")).Returns(true);
        _scope.Setup(x => x.IsFunction("funcCall")).Returns(true);
        var tokens = GetTokens("(func (x y z) (funcCall 1 2))");
        Assert.That(tokens.Count, Is.EqualTo(13));
        Assert.That(tokens[0], Is.EqualTo(new Token("(", 0, 0, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[1], Is.EqualTo(new Token("func", 0, 1, (byte)TokenKindExtra.Function)));
        Assert.That(tokens[2], Is.EqualTo(new Token("(", 0, 6, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[3], Is.EqualTo(new Token("x", 0, 7, (byte)TokenKind.Identificator)));
        Assert.That(tokens[4], Is.EqualTo(new Token("y", 0, 9, (byte)TokenKind.Identificator)));
        Assert.That(tokens[5], Is.EqualTo(new Token("z", 0, 11, (byte)TokenKind.Identificator)));
        Assert.That(tokens[6], Is.EqualTo(new Token(")", 0, 12, (byte)TokenKind.CloseBracket)));
        Assert.That(tokens[7], Is.EqualTo(new Token("(", 0, 14, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[8], Is.EqualTo(new Token("funcCall", 0, 15, (byte)TokenKindExtra.Function)));
        Assert.That(tokens[9], Is.EqualTo(new Token("1", 0, 24, (byte)TokenKind.IntegerNumber)));
        Assert.That(tokens[10], Is.EqualTo(new Token("2", 0, 26, (byte)TokenKind.IntegerNumber)));
        Assert.That(tokens[11], Is.EqualTo(new Token(")", 0, 27, (byte)TokenKind.CloseBracket)));
        Assert.That(tokens[12], Is.EqualTo(new Token(")", 0, 28, (byte)TokenKind.CloseBracket)));
    }

    [Test]
    public void RunFunction()
    {
        var tokens = GetTokens("(run (myfunc))");
        Assert.That(tokens.Count, Is.EqualTo(6));
        Assert.That(tokens[0], Is.EqualTo(new Token("(", 0, 0, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[1], Is.EqualTo(new Token("run", 0, 1, (byte)TokenKind.RunFunction)));
        Assert.That(tokens[2], Is.EqualTo(new Token("(", 0, 5, (byte)TokenKind.OpenBracket)));
        Assert.That(tokens[3], Is.EqualTo(new Token("myfunc", 0, 6, (byte)TokenKind.Identificator)));
        Assert.That(tokens[4], Is.EqualTo(new Token(")", 0, 12, (byte)TokenKind.CloseBracket)));
        Assert.That(tokens[5], Is.EqualTo(new Token(")", 0, 13, (byte)TokenKind.CloseBracket)));
    }

    [Test]
    public void Unknown()
    {
        var tokens = GetTokens("@123 1x 2.3!");
        Assert.That(tokens.Count, Is.EqualTo(3));
        Assert.That(tokens[0], Is.EqualTo(new Token("@123", 0, 0, (byte)TokenKind.Unknown)));
        Assert.That(tokens[1], Is.EqualTo(new Token("1x", 0, 5, (byte)TokenKind.Unknown)));
        Assert.That(tokens[2], Is.EqualTo(new Token("2.3!", 0, 8, (byte)TokenKind.Unknown)));
    }

    private List<Token> GetTokens(string text)
    {
        return _provider.GetTokens(TextIteratorBuilder.FromString(text)).ToList();
    }
}
