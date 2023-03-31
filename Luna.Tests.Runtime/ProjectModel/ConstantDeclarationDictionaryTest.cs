using System.Linq;
using Luna.CodeElements;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class ConstantDeclarationDictionaryTest
{
    private ConstantDeclarationDictionary _constantDeclarations;

    [SetUp]
    public void Setup()
    {
        _constantDeclarations = new();
    }

    [Test]
    public void Add()
    {
        _constantDeclarations.Add(new("WIDTH", new IntegerValueElement(123), 1, 0));
        var array = _constantDeclarations.ToArray();
        Assert.AreEqual("WIDTH", array[0].Name);

        _constantDeclarations.Add(new("HEIGHT", new IntegerValueElement(123), 1, 10));
        array = _constantDeclarations.ToArray();
        Assert.AreEqual("WIDTH", array[0].Name);
        Assert.AreEqual("HEIGHT", array[1].Name);

        _constantDeclarations.Add(new("LENGTH", new IntegerValueElement(123), 2, 0));
        array = _constantDeclarations.ToArray();
        Assert.AreEqual("WIDTH", array[0].Name);
        Assert.AreEqual("HEIGHT", array[1].Name);
        Assert.AreEqual("LENGTH", array[2].Name);
    }

    [Test]
    public void Add_NextColumnIndexLowerThanPrev_NoError()
    {
        _constantDeclarations.Add(new("WIDTH", new IntegerValueElement(123), 10, 1));
        _constantDeclarations.Add(new("HEIGHT", new IntegerValueElement(123), 1, 10));
        Assert.AreEqual(2, _constantDeclarations.Count);
    }
}
