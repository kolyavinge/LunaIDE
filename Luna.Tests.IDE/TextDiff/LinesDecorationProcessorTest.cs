using System.Collections.Generic;
using CodeHighlighter.Ancillary;
using DiffTool.Visualization;
using Luna.IDE.TextDiff;
using NUnit.Framework;

namespace Luna.Tests.IDE.TextDiff;

internal class LinesDecorationProcessorTest
{
    private LinesDecorationCollection _linesDecorationCollection;
    private LinesDecorationProcessor _processor;

    [SetUp]
    public void Setup()
    {
        _linesDecorationCollection = new LinesDecorationCollection();
        _processor = new LinesDecorationProcessor();
    }

    [Test]
    public void SetLineColors()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffTool.Core.DiffKind.Same),
            new(DiffTool.Core.DiffKind.Add),
            new(DiffTool.Core.DiffKind.Remove),
            new(DiffTool.Core.DiffKind.Change, TextKind.Old),
            new(DiffTool.Core.DiffKind.Change, TextKind.New)
        };

        _processor.SetLineColors(linesDiff, _linesDecorationCollection);

        Assert.Null(_linesDecorationCollection[0]);
        Assert.That(_linesDecorationCollection[1].Background, Is.EqualTo(LinesDecorationProcessor.BrushAdd));
        Assert.That(_linesDecorationCollection[2].Background, Is.EqualTo(LinesDecorationProcessor.BrushRemove));
        Assert.That(_linesDecorationCollection[3].Background, Is.EqualTo(LinesDecorationProcessor.BrushRemove));
        Assert.That(_linesDecorationCollection[4].Background, Is.EqualTo(LinesDecorationProcessor.BrushAdd));
    }
}
