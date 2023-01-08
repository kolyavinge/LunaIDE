using System.Collections.Generic;
using Luna.Formatting;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal abstract class BaseFormatterTest
{
    private string _formatted;
    protected int _skippedLinesCount;

    protected void AssertFormatted(string expected)
    {
        Assert.That(_formatted, Is.EqualTo(expected));
    }

    protected void Format(IReadOnlyCollection<Token> tokens)
    {
        var formatter = MakeFormatter();
        Init(formatter, tokens);
        var iter = new TokenIterator(tokens, new() { SkipComments = false });
        var formatted = new CodeStringBuilder();
        var appender = new TokenAppender(formatted);
        _skippedLinesCount = 0;
        while (!iter.Eof)
        {
            formatter.Format(iter, appender, ref _skippedLinesCount);
        }
        _formatted = formatted.ToString();
    }

    protected abstract IFormatter MakeFormatter();

    protected virtual void Init(IFormatter formatter, IReadOnlyCollection<Token> tokens) { }
}
