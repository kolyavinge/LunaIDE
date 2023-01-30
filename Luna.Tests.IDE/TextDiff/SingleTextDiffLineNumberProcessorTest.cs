using System.Collections.Generic;
using CodeHighlighter.Model;
using DiffTool.Core;
using DiffTool.Visualization;
using Luna.IDE.TextDiff;
using NUnit.Framework;

namespace Luna.Tests.IDE.TextDiff;

internal class SingleTextDiffLineNumberProcessorTest
{
    private ILineGapCollection _oldGaps;
    private ILineGapCollection _newGaps;
    private SingleTextDiffGapProcessor _processor;

    [SetUp]
    public void Setup()
    {
        _oldGaps = new LineGapCollection();
        _newGaps = new LineGapCollection();
        _processor = new SingleTextDiffGapProcessor();
    }

    [Test]
    public void SetLineGaps_Same()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Same),
            new(DiffKind.Same)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.False(_oldGaps.AnyItems);
        Assert.False(_newGaps.AnyItems);
    }

    [Test]
    public void SetLineGaps_Add_GapsInOldPanel()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Add),
            new(DiffKind.Add)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.That(_oldGaps[0].CountBefore, Is.EqualTo(2));
    }

    [Test]
    public void SetLineGaps_Add_GapsInOldPanel_2()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Add),
            new(DiffKind.Add),
            new(DiffKind.Same)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.That(_oldGaps[0].CountBefore, Is.EqualTo(2));
    }

    [Test]
    public void SetLineGaps_Remove_GapsInOldPanel()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Remove),
            new(DiffKind.Remove)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.That(_newGaps[0].CountBefore, Is.EqualTo(2));
    }

    [Test]
    public void SetLineGaps_Remove_GapsInOldPanel_2()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Remove),
            new(DiffKind.Remove),
            new(DiffKind.Same)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.That(_newGaps[0].CountBefore, Is.EqualTo(2));
    }

    [Test]
    public void SetLineGaps_TwoChanges()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Change, TextKind.Old),
            new(DiffKind.Change, TextKind.New),
            new(DiffKind.Change, TextKind.Old),
            new(DiffKind.Change, TextKind.New)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.Null(_oldGaps[0]);
        Assert.That(_newGaps[0].CountBefore, Is.EqualTo(1));
        Assert.That(_oldGaps[1].CountBefore, Is.EqualTo(1));
        Assert.That(_newGaps[1].CountBefore, Is.EqualTo(1));
    }

    [Test]
    public void SetLineGaps_TwoChangesAndSame()
    {
        var linesDiff = new List<SingleTextVisualizerLineDiff>
        {
            new(DiffKind.Change, TextKind.Old),
            new(DiffKind.Change, TextKind.New),
            new(DiffKind.Same),
            new(DiffKind.Change, TextKind.Old),
            new(DiffKind.Change, TextKind.New)
        };

        _processor.SetLineGaps(linesDiff, _oldGaps, _newGaps);

        Assert.Null(_oldGaps[0]);
        Assert.That(_newGaps[0].CountBefore, Is.EqualTo(1));

        Assert.That(_oldGaps[1].CountBefore, Is.EqualTo(1));
        Assert.Null(_newGaps[1]);

        Assert.Null(_oldGaps[2]);
        Assert.That(_newGaps[2].CountBefore, Is.EqualTo(1));
    }
}
