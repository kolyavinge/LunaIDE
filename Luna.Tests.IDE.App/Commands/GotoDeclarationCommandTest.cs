using CodeHighlighter.Model;
using Luna.CodeElements;
using Luna.IDE.App.Commands;
using Luna.IDE.CodeEditing;
using Luna.Navigation;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.App.Commands;

internal class GotoDeclarationCommandTest
{
    private CodeFileProjectItem _projectItem;
    private Mock<ICodeModelNavigator> _codeModelNavigator;
    private Mock<IDeclarationNavigator> _declarationNavigator;
    private Mock<ICodeElementNavigateCommand> _codeElementNavigateCommand;
    private Mock<ICodeFileEditor> _codeFileEditor;
    private GotoDeclarationCommand _command;

    [SetUp]
    public void Setup()
    {
        _projectItem = new("", null, null);
        _codeModelNavigator = new Mock<ICodeModelNavigator>();
        _declarationNavigator = new Mock<IDeclarationNavigator>();
        _codeElementNavigateCommand = new Mock<ICodeElementNavigateCommand>();
        _codeFileEditor = new Mock<ICodeFileEditor>();
        _codeFileEditor.SetupGet(x => x.ProjectItem).Returns(_projectItem);
        _command = new(_codeModelNavigator.Object, _declarationNavigator.Object, _codeElementNavigateCommand.Object);
    }

    [Test]
    public void TokenOnPosition()
    {
        _codeFileEditor.Setup(x => x.GetTokenCursorPosition()).Returns(new TokenCursorPosition(new("WIDTH", 1, (byte)TokenKind.Identificator), null));
        var codeElement = new NamedConstantValueElement("WIDTH");
        var declaration = new ConstantDeclaration("WIDTH", new IntegerValueElement(1));
        _codeModelNavigator.Setup(x => x.GetCodeElementByPosition(_projectItem.CodeModel, 0, 1)).Returns(new CodeModelNavigatorResult(codeElement, new CodeElement[0]));
        _declarationNavigator.Setup(x => x.GetDeclarationFor(_projectItem, codeElement)).Returns(new DeclarationNavigatorResult(_projectItem, declaration));

        _command.Execute(_codeFileEditor.Object);

        _codeElementNavigateCommand.Verify(x => x.Execute(new CodeElementNavigateCommandParameter(_projectItem, declaration)), Times.Once());
    }

    [Test]
    public void NeighbourToken_TokenOnPositionIdentificator()
    {
        _codeFileEditor.Setup(x => x.GetTokenCursorPosition()).Returns(new TokenCursorPosition(new("WIDTH", 1, (byte)TokenKind.Identificator), new("(", 6, 0)));
        var codeElement = new NamedConstantValueElement("WIDTH");
        var declaration = new ConstantDeclaration("WIDTH", new IntegerValueElement(1));
        _codeModelNavigator.Setup(x => x.GetCodeElementByPosition(_projectItem.CodeModel, 0, 1)).Returns(new CodeModelNavigatorResult(codeElement, new CodeElement[0]));
        _declarationNavigator.Setup(x => x.GetDeclarationFor(_projectItem, codeElement)).Returns(new DeclarationNavigatorResult(_projectItem, declaration));

        _command.Execute(_codeFileEditor.Object);

        _codeElementNavigateCommand.Verify(x => x.Execute(new CodeElementNavigateCommandParameter(_projectItem, declaration)), Times.Once());
    }

    [Test]
    public void NeighbourToken_NeighbourIdentificator()
    {
        _codeFileEditor.Setup(x => x.GetTokenCursorPosition()).Returns(new TokenCursorPosition(new("(", 1, 0), new("WIDTH", 2, (byte)TokenKind.Identificator)));
        var codeElement = new NamedConstantValueElement("WIDTH");
        var declaration = new ConstantDeclaration("WIDTH", new IntegerValueElement(1));
        _codeModelNavigator.Setup(x => x.GetCodeElementByPosition(_projectItem.CodeModel, 0, 2)).Returns(new CodeModelNavigatorResult(codeElement, new CodeElement[0]));
        _declarationNavigator.Setup(x => x.GetDeclarationFor(_projectItem, codeElement)).Returns(new DeclarationNavigatorResult(_projectItem, declaration));

        _command.Execute(_codeFileEditor.Object);

        _codeElementNavigateCommand.Verify(x => x.Execute(new CodeElementNavigateCommandParameter(_projectItem, declaration)), Times.Once());
    }

    [Test]
    public void Execute_NoTokenOnPosition()
    {
        _codeFileEditor.Setup(x => x.GetTokenCursorPosition()).Returns(default(TokenCursorPosition));

        _command.Execute(_codeFileEditor.Object);

        _codeElementNavigateCommand.Verify(x => x.Execute(It.IsAny<CodeElementNavigateCommandParameter>()), Times.Never());
    }

    [Test]
    public void NoCodeModelNavigatorResult()
    {
        _codeFileEditor.Setup(x => x.GetTokenCursorPosition()).Returns(new TokenCursorPosition(new("WIDTH", 1, (byte)TokenKind.Identificator), null));
        var codeElement = new NamedConstantValueElement("WIDTH");
        var declaration = new ConstantDeclaration("WIDTH", new IntegerValueElement(1));
        _codeModelNavigator.Setup(x => x.GetCodeElementByPosition(_projectItem.CodeModel, 0, 1)).Returns(default(CodeModelNavigatorResult));

        _command.Execute(_codeFileEditor.Object);

        _codeElementNavigateCommand.Verify(x => x.Execute(It.IsAny<CodeElementNavigateCommandParameter>()), Times.Never());
    }

    [Test]
    public void NoDeclarationNavigatorResult()
    {
        _codeFileEditor.Setup(x => x.GetTokenCursorPosition()).Returns(new TokenCursorPosition(new("WIDTH", 1, (byte)TokenKind.Identificator), null));
        var codeElement = new NamedConstantValueElement("WIDTH");
        var declaration = new ConstantDeclaration("WIDTH", new IntegerValueElement(1));
        _codeModelNavigator.Setup(x => x.GetCodeElementByPosition(_projectItem.CodeModel, 0, 1)).Returns(new CodeModelNavigatorResult(codeElement, new CodeElement[0]));
        _declarationNavigator.Setup(x => x.GetDeclarationFor(_projectItem, codeElement)).Returns(default(DeclarationNavigatorResult));

        _command.Execute(_codeFileEditor.Object);

        _codeElementNavigateCommand.Verify(x => x.Execute(It.IsAny<CodeElementNavigateCommandParameter>()), Times.Never());
    }
}
