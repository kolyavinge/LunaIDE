using System;
using DiffTool.Core;
using Luna.IDE.TextDiff;
using NUnit.Framework;

namespace Luna.Tests.IDE.TextDiff;

internal class TextDiffEngineTest
{
    private TextDiffEngine _engine;

    [SetUp]
    public void Setup()
    {
        _engine = new TextDiffEngine();
    }

    [Test]
    public void GetSingleTextResult_EmptyTextes()
    {
        var diffResult = new TextDiffResult(new(""), new(""), Array.Empty<LineDiff>());
        var result = _engine.GetSingleTextResult(diffResult);

        Assert.That(result.OldTextLinesCount, Is.EqualTo(0));
        Assert.That(result.NewTextLinesCount, Is.EqualTo(0));
    }

    [Test]
    public void GetSingleTextResult()
    {
        var diffResult = new TextDiffResult(new("old text"), new("new text"), Array.Empty<LineDiff>());
        var result = _engine.GetSingleTextResult(diffResult);

        Assert.That(result.OldTextLinesCount, Is.EqualTo(1));
        Assert.That(result.NewTextLinesCount, Is.EqualTo(1));
    }
}
