using System;
using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Luna.IDE.Model;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.Model;

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

    //[Test]
    //public void OnCodeModelUpdated_NoDifferent()
    //{
    //    _editor.OnCodeModelUpdated(new(new CodeModel(), new CodeModel()));
    //    _codeProvider.Verify(x => x.UpdateTokenKinds(It.IsAny<IEnumerable<UpdatedTokenKind>>()), Times.Never());
    //}

    //[Test]
    //public void OnCodeModelUpdated()
    //{
    //    var oldModel = new CodeModel();
    //    var newModel = new CodeModel();
    //    oldModel.AddFunctionDeclaration(new("removed", Enumerable.Empty<FunctionArgument>(), new()));
    //    newModel.AddFunctionDeclaration(new("added", Enumerable.Empty<FunctionArgument>(), new()));
    //    _editor.OnCodeModelUpdated(new(oldModel, newModel));
    //    _codeProvider.Verify(x => x.UpdateTokenKinds(new UpdatedTokenKind[] { new("removed", (byte)TokenKind.Identificator), new("added", (byte)TokenKindExtra.Function) }), Times.Once());
    //}

    [Test]
    public void Close()
    {
        _editor.Close();
        _codeModelUpdater.Verify(x => x.Detach(_codeFileProjectItem), Times.Once());
    }
}
