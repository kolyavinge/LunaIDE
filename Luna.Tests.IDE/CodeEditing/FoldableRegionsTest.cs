using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.Core;
using Luna.IDE.CodeEditing;
using Moq;
using NUnit.Framework;
using TokenKind = Luna.Parsing.TokenKind;

namespace Luna.Tests.IDE.CodeEditing;

internal class FoldableRegionsTest
{
    private Mock<ITokens> _tokens;
    private FoldableRegions _foldableRegions;

    [SetUp]
    public void Setup()
    {
        _tokens = new Mock<ITokens>();
        for (int i = 0; i < 20; i++)
        {
            _tokens.Setup(x => x.GetTokens(i)).Returns(TokenList.FromEnumerable(Enumerable.Empty<Token>()));
        }
        _foldableRegions = new FoldableRegions();
    }

    [Test]
    public void GetRegions_Empty_1()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(0);
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_Empty_2()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(10);
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_OneImport()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(1);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ImportDirective) }));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoImports()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(2);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ImportDirective) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ImportDirective) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_ThreeImports()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(4);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ImportDirective) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ImportDirective) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ImportDirective) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_OneComment()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(1);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Comment) }));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoComments()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(2);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Comment) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Comment) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_ThreeComments()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(4);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Comment) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Comment) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Comment) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_OneConst()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(1);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoConsts()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(3);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(2)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(2));
    }

    [Test]
    public void GetRegions_ConstsAndOneFuncBetween()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(3);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(2)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket), new("", 0, (byte)TokenKind.CloseBracket) }));
        _tokens.Setup(x => x.GetTokens(4)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoConstsAndOneFuncBetween()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(5);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(2)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket), new("", 0, (byte)TokenKind.CloseBracket) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(4)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
        Assert.That(result[1].LineIndex, Is.EqualTo(3));
        Assert.That(result[1].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_ConstsAndOneFuncEnd()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(3);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.ConstDeclaration) }));
        _tokens.Setup(x => x.GetTokens(2)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket), new("", 0, (byte)TokenKind.CloseBracket) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_OneInlineFunction()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(1);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket), new("", 0, (byte)TokenKind.CloseBracket) }));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_OneFunction()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(4);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.CloseBracket) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_TwoFunctions()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(7);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.CloseBracket) }));
        _tokens.Setup(x => x.GetTokens(5)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket) }));
        _tokens.Setup(x => x.GetTokens(6)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.CloseBracket) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
        Assert.That(result[1].LineIndex, Is.EqualTo(5));
        Assert.That(result[1].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_OneFunctionWithInners_OneLine()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(2);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket), new("", 0, (byte)TokenKind.OpenBracket) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.CloseBracket), new("", 0, (byte)TokenKind.CloseBracket) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_OneFunctionWithInners_ThreeLines()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(4);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.OpenBracket) }));
        _tokens.Setup(x => x.GetTokens(2)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.CloseBracket) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.CloseBracket) }));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].LineIndex, Is.EqualTo(1));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
        Assert.That(result[1].LineIndex, Is.EqualTo(0));
        Assert.That(result[1].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_WrongTokens()
    {
        _tokens.SetupGet(x => x.LinesCount).Returns(4);
        _tokens.Setup(x => x.GetTokens(0)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Identificator) }));
        _tokens.Setup(x => x.GetTokens(1)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.Identificator) }));
        _tokens.Setup(x => x.GetTokens(2)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.FloatNumber) }));
        _tokens.Setup(x => x.GetTokens(3)).Returns(TokenList.FromEnumerable(new Token[] { new("", 0, (byte)TokenKind.FloatNumber) }));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    private List<FoldableRegion> GetRegions()
    {
        return _foldableRegions.GetRegions(_tokens.Object).ToList();
    }
}
