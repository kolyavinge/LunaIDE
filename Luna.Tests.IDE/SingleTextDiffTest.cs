using System.Collections.Generic;
using CodeHighlighter.CodeProvidering;
using DiffTool.Visualization;
using Luna.IDE.TextDiff;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE;

internal class SingleTextDiffTest
{
    private Mock<ITextDiffEngine> _textDiffEngine;
    private Mock<ITextDiffCodeProviderFactory> _textDiffCodeProviderFactory;
    private SingleTextDiff _singleTextDiff;

    [SetUp]
    public void Setup()
    {
        _textDiffEngine = new Mock<ITextDiffEngine>();
        _textDiffCodeProviderFactory = new Mock<ITextDiffCodeProviderFactory>();
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
        var codeProvider = new Mock<ICodeProvider>();
        _textDiffCodeProviderFactory.Setup(x => x.Make(".ext", "old text", "new text")).Returns(codeProvider.Object);

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
        var codeProvider = new Mock<ICodeProvider>();
        _textDiffCodeProviderFactory.Setup(x => x.Make(".ext", "", "new text")).Returns(codeProvider.Object);

        _singleTextDiff.MakeDiff(".ext", null, "new text").Wait();

        Assert.That(_singleTextDiff.DiffCodeTextBoxModel.Text.ToString(), Is.EqualTo("diff text"));
    }
}
