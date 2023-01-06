using Luna.Formatting;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class CodeStringBuilderTest
{
    private CodeStringBuilder _builder;

    [SetUp]
    public void Setup()
    {
        _builder = new CodeStringBuilder();
    }

    [Test]
    public void Simple()
    {
        _builder.Append(0, 0, "text");
        Assert.That(_builder.ToString(), Is.EqualTo("text"));
    }

    [Test]
    public void ColumnShift()
    {
        _builder.Append(0, 5, "text");
        Assert.That(_builder.ToString(), Is.EqualTo("     text"));
    }

    [Test]
    public void LineShift()
    {
        _builder.Append(1, 0, "text");
        Assert.That(_builder.ToString(), Is.EqualTo("\r\ntext"));
    }

    [Test]
    public void LineShiftColumnShift()
    {
        _builder.Append(1, 5, "text");
        Assert.That(_builder.ToString(), Is.EqualTo("\r\n     text"));
    }
}
