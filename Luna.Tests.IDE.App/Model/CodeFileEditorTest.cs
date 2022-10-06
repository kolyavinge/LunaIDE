using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.CodeProvidering;
using Luna.IDE.App.CodeEditor;
using Luna.IDE.App.Model;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.App.Model;

internal class CodeFileEditorTest
{
    private Mock<IFileSystem> _fileSystem;
    private CodeFileProjectItem _codeFileProjectItem;
    private Mock<ICodeProviderFactory> _codeProviderFactory;
    private Mock<ILunaCodeProvider> _codeProvider;
    private Mock<ICodeModelUpdater> _codeModelUpdater;
    private CodeFileEditor _editor;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _fileSystem.Setup(x => x.ReadFileText("file")).Returns("");
        _codeFileProjectItem = new CodeFileProjectItem("file", null, _fileSystem.Object);
        _codeProviderFactory = new Mock<ICodeProviderFactory>();
        _codeProvider = new Mock<ILunaCodeProvider>();
        _codeModelUpdater = new Mock<ICodeModelUpdater>();
        _codeProviderFactory.Setup(x => x.Make(_codeFileProjectItem)).Returns(_codeProvider.Object);
        _editor = new CodeFileEditor(_codeFileProjectItem, _codeProviderFactory.Object, _codeModelUpdater.Object);
    }

    [Test]
    public void Constructor()
    {
        _codeModelUpdater.Verify(x => x.Attach(_codeFileProjectItem, It.IsAny<CodeModelUpdatedCallback>()), Times.Once());
    }

    [Test]
    public void OnCodeModelUpdated_NoDifferent()
    {
        _editor.OnCodeModelUpdated(new(new CodeModel(), new CodeModelScopeIdentificatorsDifferent()));
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

        _editor.OnCodeModelUpdated(new(new CodeModel(), diff));

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

    [Test]
    public void Close()
    {
        _editor.Close();
        _codeModelUpdater.Verify(x => x.Detach(_codeFileProjectItem), Times.Once());
    }
}
