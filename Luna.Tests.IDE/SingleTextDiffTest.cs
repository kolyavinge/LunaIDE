using System.Collections.Generic;
using CodeHighlighter.CodeProvidering;
using DiffTool.Visualization;
using Luna.IDE.TextDiff;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE;

internal class SingleTextDiffTest
{
    private Mock<ITextDiffEngine> _textDiffEngine;
    private Mock<ITextDiffCodeProviderFactory> _textDiffCodeProviderFactory;
    private Mock<ICodeProvider> _codeProvider;
    private Mock<IFileSystem> _fileSystem;
    private SingleTextDiff _singleTextDiff;

    [SetUp]
    public void Setup()
    {
        _textDiffEngine = new Mock<ITextDiffEngine>();
        _textDiffCodeProviderFactory = new Mock<ITextDiffCodeProviderFactory>();
        _codeProvider = new Mock<ICodeProvider>();
        _fileSystem = new Mock<IFileSystem>();
        _singleTextDiff = new SingleTextDiff(_textDiffEngine.Object, _textDiffCodeProviderFactory.Object);
    }

    [Test]
    public void MakeDiff()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffTool.Core.DiffKind.Same),
            new(DiffTool.Core.DiffKind.Add),
            new(DiffTool.Core.DiffKind.Remove),
            new(DiffTool.Core.DiffKind.Change, TextKind.Old),
            new(DiffTool.Core.DiffKind.Change, TextKind.New)
        };
        var diffResult = new SingleTextVisualizerResult("diff text", linesDiff);
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("old text", "new text")).ReturnsAsync(diffResult);
        _textDiffCodeProviderFactory.Setup(x => x.Make(".ext", "old text", "new text")).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff(".ext", "old text", "new text").Wait();

        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.Text.ToString(), Is.EqualTo("diff text"));
        Assert.Null(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[0]);
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[1].Background, Is.EqualTo(SingleTextDiff.BrushAdd));
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[2].Background, Is.EqualTo(SingleTextDiff.BrushRemove));
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[3].Background, Is.EqualTo(SingleTextDiff.BrushRemove));
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[4].Background, Is.EqualTo(SingleTextDiff.BrushAdd));
    }

    [Test]
    public void MakeDiff_OldTextNull()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffTool.Core.DiffKind.Same),
            new(DiffTool.Core.DiffKind.Add),
            new(DiffTool.Core.DiffKind.Remove),
            new(DiffTool.Core.DiffKind.Change, TextKind.Old),
            new(DiffTool.Core.DiffKind.Change, TextKind.New)
        };
        var diffResult = new SingleTextVisualizerResult("diff text", linesDiff);
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("", "new text")).ReturnsAsync(diffResult);
        _textDiffCodeProviderFactory.Setup(x => x.Make(".ext", "", "new text")).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff(".ext", null, "new text").Wait();

        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.Text.ToString(), Is.EqualTo("diff text"));
    }

    [Test]
    public void MakeDiff_TextFileProjectItem()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffTool.Core.DiffKind.Same),
            new(DiffTool.Core.DiffKind.Add),
            new(DiffTool.Core.DiffKind.Remove),
            new(DiffTool.Core.DiffKind.Change, TextKind.Old),
            new(DiffTool.Core.DiffKind.Change, TextKind.New)
        };
        var diffResult = new SingleTextVisualizerResult("diff text", linesDiff);
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("old text", "new text")).ReturnsAsync(diffResult);
        _fileSystem.Setup(x => x.ReadFileText("code file path")).Returns("new text");
        var codeFile = new CodeFileProjectItem("code file path", null, _fileSystem.Object);
        _textDiffCodeProviderFactory.Setup(x => x.Make("old text", codeFile)).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff("old text", codeFile).Wait();

        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.Text.ToString(), Is.EqualTo("diff text"));
        Assert.Null(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[0]);
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[1].Background, Is.EqualTo(SingleTextDiff.BrushAdd));
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[2].Background, Is.EqualTo(SingleTextDiff.BrushRemove));
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[3].Background, Is.EqualTo(SingleTextDiff.BrushRemove));
        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.LinesDecoration[4].Background, Is.EqualTo(SingleTextDiff.BrushAdd));
    }

    [Test]
    public void MakeDiff_TextFileProjectItem_OldTextNull()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffTool.Core.DiffKind.Same),
            new(DiffTool.Core.DiffKind.Add),
            new(DiffTool.Core.DiffKind.Remove),
            new(DiffTool.Core.DiffKind.Change, TextKind.Old),
            new(DiffTool.Core.DiffKind.Change, TextKind.New)
        };
        var diffResult = new SingleTextVisualizerResult("diff text", linesDiff);
        _textDiffEngine.Setup(x => x.GetSingleTextResultAsync("", "new text")).ReturnsAsync(diffResult);
        _fileSystem.Setup(x => x.ReadFileText("code file path")).Returns("new text");
        var codeFile = new CodeFileProjectItem("code file path", null, _fileSystem.Object);
        _textDiffCodeProviderFactory.Setup(x => x.Make("", codeFile)).Returns(_codeProvider.Object);

        _singleTextDiff.MakeDiff(null, codeFile).Wait();

        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.Text.ToString(), Is.EqualTo("diff text"));
    }
}
