using System.Collections.Generic;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using DiffTool.Visualization;
using Luna.IDE.TextDiff;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.TextDiff;

internal class SingleTextDiffTest
{
    private Mock<ITextDiffEngine> _textDiffEngine;
    private Mock<ITextDiffCodeProviderFactory> _textDiffCodeProviderFactory;
    private Mock<ILinesDecorationProcessor> _linesDecorationProcessor;
    private Mock<ILineNumberProcessor> _lineNumberProcessor;
    private Mock<ICodeProvider> _codeProvider;
    private Mock<IDiffCodeTextBox> _diffCodeTextBox;
    private Mock<IFileSystem> _fileSystem;
    private List<SingleTextVisualizerLineDiff> _linesDiff;
    private SingleTextDiffResult _diffResult;
    private SingleTextDiff _singleTextDiff;

    [SetUp]
    public void Setup()
    {
        _textDiffEngine = new Mock<ITextDiffEngine>();
        _textDiffCodeProviderFactory = new Mock<ITextDiffCodeProviderFactory>();
        _linesDecorationProcessor = new Mock<ILinesDecorationProcessor>();
        _lineNumberProcessor = new Mock<ILineNumberProcessor>();
        _codeProvider = new Mock<ICodeProvider>();
        _diffCodeTextBox = new Mock<IDiffCodeTextBox>();
        _fileSystem = new Mock<IFileSystem>();
        _linesDiff = new List<SingleTextVisualizerLineDiff>();
        _diffResult = new SingleTextDiffResult(1, 1, new SingleTextVisualizerResult("diff text", _linesDiff));
        _singleTextDiff = new SingleTextDiff(
            _textDiffEngine.Object,
            _textDiffCodeProviderFactory.Object,
            _diffCodeTextBox.Object,
            _linesDecorationProcessor.Object,
            _lineNumberProcessor.Object);
    }

    [Test]
    public void MakeDiff()
    {
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("old text", "new text")).ReturnsAsync(_diffResult);
        _textDiffCodeProviderFactory.Setup(x => x.Make(".ext", "old text", "new text")).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff(".ext", "old text", "new text").Wait();

        _diffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "diff text"), Times.Once());
        _linesDecorationProcessor.Verify(x => x.SetLineColors(_linesDiff, It.IsAny<LinesDecorationCollection>()), Times.Once());
    }

    [Test]
    public void MakeDiff_OldTextNull()
    {
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("", "new text")).ReturnsAsync(_diffResult);
        _textDiffCodeProviderFactory.Setup(x => x.Make(".ext", "", "new text")).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff(".ext", null, "new text").Wait();

        _diffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "diff text"), Times.Once());
        _linesDecorationProcessor.Verify(x => x.SetLineColors(_linesDiff, It.IsAny<LinesDecorationCollection>()), Times.Never());
    }

    [Test]
    public void MakeDiff_TextFileProjectItem()
    {
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("old text", "new text")).ReturnsAsync(_diffResult);
        _fileSystem.Setup(x => x.ReadFileText("code file path")).Returns("new text");
        var codeFile = new CodeFileProjectItem("code file path", null, _fileSystem.Object);
        _textDiffCodeProviderFactory.Setup(x => x.Make("old text", codeFile)).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff("old text", codeFile).Wait();

        _diffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "diff text"), Times.Once());
        _linesDecorationProcessor.Verify(x => x.SetLineColors(_linesDiff, It.IsAny<LinesDecorationCollection>()), Times.Once());
    }

    [Test]
    public void MakeDiff_TextFileProjectItem_OldTextNull()
    {
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("", "new text")).ReturnsAsync(_diffResult);
        _fileSystem.Setup(x => x.ReadFileText("code file path")).Returns("new text");
        var codeFile = new CodeFileProjectItem("code file path", null, _fileSystem.Object);
        _textDiffCodeProviderFactory.Setup(x => x.Make("", codeFile)).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff(null, codeFile).Wait();

        _diffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "diff text"), Times.Once());
        _linesDecorationProcessor.Verify(x => x.SetLineColors(_linesDiff, It.IsAny<LinesDecorationCollection>()), Times.Never());
    }
}
