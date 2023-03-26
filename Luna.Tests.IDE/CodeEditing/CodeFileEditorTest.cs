using Luna.IDE.CodeEditing;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.CodeEditing;

internal class CodeFileEditorTest
{
    private Mock<IFileSystem> _fileSystem;
    private CodeFileProjectItem _codeFileProjectItem;
    private Mock<ICodeProviderFactory> _codeProviderFactory;
    private Mock<ILunaCodeProvider> _codeProvider;
    private Mock<ICodeModelUpdater> _codeModelUpdater;
    private Mock<ITokenKindsUpdater> _tokenKindsUpdater;
    private Mock<IFoldableRegionsUpdater> _foldableRegionsUpdater;
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
        _tokenKindsUpdater = new Mock<ITokenKindsUpdater>();
        _foldableRegionsUpdater = new Mock<IFoldableRegionsUpdater>();
        _codeProviderFactory.Setup(x => x.Make(_codeFileProjectItem)).Returns(_codeProvider.Object);
        _editor = new CodeFileEditor(_codeFileProjectItem, _codeProviderFactory.Object, _codeModelUpdater.Object, _tokenKindsUpdater.Object, _foldableRegionsUpdater.Object);
    }

    [Test]
    public void Close()
    {
        _editor.Close();
    }
}
