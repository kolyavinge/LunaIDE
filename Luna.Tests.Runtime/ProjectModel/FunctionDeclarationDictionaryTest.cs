using System;
using System.Linq;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class FunctionDeclarationDictionaryTest
{
    private FunctionDeclarationDictionary _functionDeclarations;

    [SetUp]
    public void Setup()
    {
        _functionDeclarations = new();
    }

    [Test]
    public void Add()
    {
        _functionDeclarations.Add(new("WIDTH", new FunctionArgument[0], new(), 1, 0));
        var array = _functionDeclarations.ToArray();
        Assert.AreEqual("WIDTH", array[0].Name);

        _functionDeclarations.Add(new("HEIGHT", new FunctionArgument[0], new(), 1, 10));
        array = _functionDeclarations.ToArray();
        Assert.AreEqual("WIDTH", array[0].Name);
        Assert.AreEqual("HEIGHT", array[1].Name);

        _functionDeclarations.Add(new("LENGTH", new FunctionArgument[0], new(), 2, 0));
        array = _functionDeclarations.ToArray();
        Assert.AreEqual("WIDTH", array[0].Name);
        Assert.AreEqual("HEIGHT", array[1].Name);
        Assert.AreEqual("LENGTH", array[2].Name);
    }

    [Test]
    public void Add_Wrong()
    {
        _functionDeclarations.Add(new("WIDTH", new FunctionArgument[0], new(), 10, 1));
        try
        {
            _functionDeclarations.Add(new("HEIGHT", new FunctionArgument[0], new(), 1, 10));
            Assert.Fail();
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("New item position must be greater then line index 10 and column index 1", ex.Message);
        }
    }
}
