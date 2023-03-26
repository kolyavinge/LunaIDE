using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.CodeProvidering;
using Luna.CodeElements;
using Luna.IDE.CodeEditing;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.CodeEditing;

internal class TokenKindsUpdaterTest
{
    private Mock<ILunaCodeProvider> _codeProvider;
    private TokenKindsUpdater _updater;

    [SetUp]
    public void Setup()
    {
        _codeProvider = new Mock<ILunaCodeProvider>();
        _updater = new TokenKindsUpdater();
    }

    [Test]
    public void OnCodeModelUpdated_NoDifferent()
    {
        _updater.Update(new CodeModelScopeIdentificatorsDifferent(), _codeProvider.Object);
        _codeProvider.Verify(x => x.UpdateTokenKinds(It.IsAny<IEnumerable<UpdatedTokenKind>>()), Times.Never());
    }

    [Test]
    public void OnCodeModelUpdated()
    {
        var diff = new CodeModelScopeIdentificatorsDifferent(
            new ConstantDeclarationDictionary(new[] { new ConstantDeclaration("addedDeclaredConst", new IntegerValueElement(1)) }),
            new FunctionDeclarationDictionary(new[] { new FunctionDeclaration("addedDeclaredFunc", Enumerable.Empty<FunctionArgument>(), new()) }),
            new ConstantDeclarationDictionary(new[] { new ConstantDeclaration("removedDeclaredConst", new IntegerValueElement(1)) }),
            new FunctionDeclarationDictionary(new[] { new FunctionDeclaration("removedDeclaredFunc", Enumerable.Empty<FunctionArgument>(), new()) }),
            new ConstantDeclarationDictionary(new[] { new ConstantDeclaration("addedImportedConst", new IntegerValueElement(1)) }),
            new FunctionDeclarationDictionary(new[] { new FunctionDeclaration("addedImportedFunc", Enumerable.Empty<FunctionArgument>(), new()) }),
            new ConstantDeclarationDictionary(new[] { new ConstantDeclaration("removedImportedConst", new IntegerValueElement(1)) }),
            new FunctionDeclarationDictionary(new[] { new FunctionDeclaration("removedImportedFunc", Enumerable.Empty<FunctionArgument>(), new()) }));

        _updater.Update(diff, _codeProvider.Object);

        _codeProvider.Verify(x => x.UpdateTokenKinds(new UpdatedTokenKind[]
        {
            new("addedDeclaredConst", (byte)TokenKindExtra.Constant),
            new("addedImportedConst", (byte)TokenKindExtra.Constant),
            new("addedDeclaredFunc", (byte)TokenKindExtra.Function),
            new("addedImportedFunc", (byte)TokenKindExtra.Function),
            new("removedDeclaredConst", (byte)TokenKind.Identificator),
            new("removedImportedConst", (byte)TokenKind.Identificator),
            new("removedDeclaredFunc", (byte)TokenKind.Identificator),
            new("removedImportedFunc", (byte)TokenKind.Identificator)
        }),
        Times.Once());
    }
}
