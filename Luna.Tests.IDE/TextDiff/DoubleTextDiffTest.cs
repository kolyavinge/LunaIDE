using System;
using System.Collections.Generic;
using CodeHighlighter.Ancillary;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Core;
using CodeHighlighter.Model;
using DiffTool.Core;
using Luna.IDE.TextDiff;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.TextDiff;

internal class DoubleTextDiffTest
{
    private Mock<ITextDiffCodeProviderFactory> _textDiffCodeProviderFactory;
    private Mock<IDiffCodeTextBox> _oldDiffCodeTextBox;
    private Mock<IDiffCodeTextBox> _newDiffCodeTextBox;
    private Mock<ILineNumberPanelModel> _oldLineNumberPanel;
    private Mock<ILineNumberPanelModel> _newLineNumberPanel;
    private Mock<IViewport> _viewport;
    private Mock<ILinesDecorationProcessor> _linesDecorationProcessor;
    private Mock<IDoubleTextDiffGapProcessor> _gapProcessor;
    private Mock<ICodeProvider> _codeProvider;
    private Mock<IFileSystem> _fileSystem;
    private IReadOnlyList<LineDiff> _linesDiff;
    private DoubleTextDiff _doubleTextDiff;

    [SetUp]
    public void Setup()
    {
        _textDiffCodeProviderFactory = new Mock<ITextDiffCodeProviderFactory>();
        _viewport = new Mock<IViewport>();

        _oldDiffCodeTextBox = new Mock<IDiffCodeTextBox>();
        _newDiffCodeTextBox = new Mock<IDiffCodeTextBox>();
        _oldDiffCodeTextBox.SetupGet(x => x.Gaps).Returns(new Mock<ILineGapCollection>().Object);
        _newDiffCodeTextBox.SetupGet(x => x.Gaps).Returns(new Mock<ILineGapCollection>().Object);
        _oldDiffCodeTextBox.SetupGet(x => x.Viewport).Returns(_viewport.Object);
        _newDiffCodeTextBox.SetupGet(x => x.Viewport).Returns(_viewport.Object);

        _oldLineNumberPanel = new Mock<ILineNumberPanelModel>();
        _newLineNumberPanel = new Mock<ILineNumberPanelModel>();
        _oldLineNumberPanel.SetupGet(x => x.Gaps).Returns(new Mock<ILineGapCollection>().Object);
        _newLineNumberPanel.SetupGet(x => x.Gaps).Returns(new Mock<ILineGapCollection>().Object);

        _linesDecorationProcessor = new Mock<ILinesDecorationProcessor>();
        _gapProcessor = new Mock<IDoubleTextDiffGapProcessor>();
        _codeProvider = new Mock<ICodeProvider>();
        _fileSystem = new Mock<IFileSystem>();
        _linesDiff = new List<LineDiff>();
        _doubleTextDiff = new DoubleTextDiff(
            _textDiffCodeProviderFactory.Object,
            _oldDiffCodeTextBox.Object,
            _newDiffCodeTextBox.Object,
            _oldLineNumberPanel.Object,
            _newLineNumberPanel.Object,
            _linesDecorationProcessor.Object,
            _gapProcessor.Object);
    }

    [Test]
    public void MakeDiff_TextFileProjectItem()
    {
        var diffResult = new TextDiffResult(new("old text"), new("new text"), Array.Empty<LineDiff>());
        _fileSystem.Setup(x => x.ReadFileText("code file path")).Returns("new text");
        var codeFile = new CodeFileProjectItem("code file path", null, _fileSystem.Object);
        _textDiffCodeProviderFactory.Setup(x => x.Make("old text", codeFile)).Returns(_codeProvider.Object);

        _doubleTextDiff.MakeDiff(diffResult, "old text", codeFile);

        _oldDiffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "old text"), Times.Once());
        _newDiffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "new text"), Times.Once());
        _linesDecorationProcessor.Verify(x => x.SetLineColors(_linesDiff, It.IsAny<LinesDecorationCollection>(), It.IsAny<LinesDecorationCollection>()), Times.Once());
    }

    [Test]
    public void MakeDiff_TextFileProjectItem_OldTextNull()
    {
        var diffResult = new TextDiffResult(new(""), new("new text"), Array.Empty<LineDiff>());
        _fileSystem.Setup(x => x.ReadFileText("code file path")).Returns("new text");
        var codeFile = new CodeFileProjectItem("code file path", null, _fileSystem.Object);
        _textDiffCodeProviderFactory.Setup(x => x.Make("", codeFile)).Returns(_codeProvider.Object);

        _doubleTextDiff.MakeDiff(diffResult, null, codeFile);

        _oldDiffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, ""), Times.Once());
        _newDiffCodeTextBox.Verify(x => x.Init(_codeProvider.Object, "new text"), Times.Once());
        _linesDecorationProcessor.Verify(x => x.SetLineColors(_linesDiff, It.IsAny<LinesDecorationCollection>(), It.IsAny<LinesDecorationCollection>()), Times.Never());
    }
}
