using Luna.Collections;
using NUnit.Framework;

namespace Luna.Tests.Collections;

internal class ReadonlyArrayTest
{
    [Test]
    public void Equals_Empty()
    {
        var array1 = new ReadonlyArray<int>();
        var array2 = new ReadonlyArray<int>();

        Assert.True(array1.Equals(array2));
        Assert.True(array2.Equals(array1));
    }

    [Test]
    public void Equals()
    {
        var array1 = new ReadonlyArray<int>(new[] { 1, 2, 3 });
        var array2 = new ReadonlyArray<int>(new[] { 1, 2, 3 });

        Assert.True(array1.Equals(array2));
        Assert.True(array2.Equals(array1));
    }

    [Test]
    public void DiffOrder()
    {
        var array1 = new ReadonlyArray<int>(new[] { 1, 2, 3 });
        var array2 = new ReadonlyArray<int>(new[] { 3, 2, 1 });

        Assert.False(array1.Equals(array2));
        Assert.False(array2.Equals(array1));
    }

    [Test]
    public void Diff()
    {
        var array1 = new ReadonlyArray<int>(new int[] { 1 });
        var array2 = new ReadonlyArray<int>(new int[0]);

        Assert.False(array1.Equals(array2));
        Assert.False(array2.Equals(array1));
    }
}
