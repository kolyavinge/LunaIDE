using Luna.Formatting;
using NUnit.Framework;

namespace Luna.Tests.Formatting;

internal class CodeFormatterIntegration
{
    private CodeFormatter _formatter;

    [SetUp]
    public void Setup()
    {
        _formatter = new(new CommentFormatter(), new ImportFormatter(), new ConstantFormatter(), new DefaultFormatter());
    }

    [Test]
    public void Format()
    {
        var formatted = _formatter.Format(@"// comment

import 'file'
import 'file'

const WIDTH  123
const HEIGHT 45

// comment");

        Assert.That(formatted, Is.EqualTo(@"// comment

import 'file'

const WIDTH  123
const HEIGHT 45

// comment
"));
    }

    [Test]
    public void Default()
    {
        var formatted = _formatter.Format(@"// comment
   123
// comment");

        Assert.That(formatted, Is.EqualTo(@"// comment
   123
// comment
"));
    }
}
