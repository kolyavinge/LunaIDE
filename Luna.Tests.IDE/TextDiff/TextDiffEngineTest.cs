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
        var result = _engine.GetSingleTextResult("", "");

        Assert.That(result.OldTextLinesCount, Is.EqualTo(0));
        Assert.That(result.NewTextLinesCount, Is.EqualTo(0));
    }

    [Test]
    public void GetSingleTextResult()
    {
        var result = _engine.GetSingleTextResult("old", "new");

        Assert.That(result.OldTextLinesCount, Is.EqualTo(1));
        Assert.That(result.NewTextLinesCount, Is.EqualTo(1));
    }
}
