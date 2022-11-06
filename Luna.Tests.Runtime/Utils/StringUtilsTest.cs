using Luna.Utils;
using NUnit.Framework;

namespace Luna.Tests.Utils;

internal class StringUtilsTest
{
    [Test]
    public void StringEquals()
    {
        Assert.False(StringUtils.StringEquals(null, null, 0));
        Assert.False(StringUtils.StringEquals("abc", null, 0));
        Assert.False(StringUtils.StringEquals(null, new[] { 'a', 'b', 'c' }, 3));
        Assert.True(StringUtils.StringEquals("", new char[0], 0));
        Assert.True(StringUtils.StringEquals("abc", new[] { 'a', 'b', 'c' }, 3));
        Assert.True(StringUtils.StringEquals("abcd", new[] { 'a', 'b', 'c' }, 3));
        Assert.False(StringUtils.StringEquals("abc", new[] { 'a', 'b', 'c', 'd' }, 4));
        Assert.True(StringUtils.StringEquals("abc", new[] { 'a', 'b', 'c', 'd' }, 3));
    }
}
