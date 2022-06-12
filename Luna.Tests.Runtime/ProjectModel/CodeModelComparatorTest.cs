using System.Linq;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class CodeModelComparatorTest
{
    private CodeModel _oldModel;
    private CodeModel _newModel;
    private CodeModelDifferent _result;

    [SetUp]
    public void Setup()
    {
        _oldModel = new();
        _newModel = new();
    }

    [Test]
    public void Empty()
    {
        MakeResult();
        Assert.AreEqual(0, _result.RemovedFunctions.Count);
        Assert.AreEqual(0, _result.AddedFunctions.Count);
    }

    [Test]
    public void NoDifferent()
    {
        _oldModel.AddFunctionDeclaration(new FunctionDeclaration("func", Enumerable.Empty<FunctionArgument>(), new()));
        _newModel.AddFunctionDeclaration(new FunctionDeclaration("func", Enumerable.Empty<FunctionArgument>(), new()));
        MakeResult();
        Assert.AreEqual(0, _result.RemovedFunctions.Count);
        Assert.AreEqual(0, _result.AddedFunctions.Count);
    }

    [Test]
    public void OneRemoved()
    {
        _oldModel.AddFunctionDeclaration(new FunctionDeclaration("removed", Enumerable.Empty<FunctionArgument>(), new()));
        MakeResult();
        Assert.AreEqual(1, _result.RemovedFunctions.Count);
        Assert.AreEqual("removed", _result.RemovedFunctions.First().Name);
        Assert.AreEqual(0, _result.AddedFunctions.Count);
    }

    [Test]
    public void OneAdded()
    {
        _newModel.AddFunctionDeclaration(new FunctionDeclaration("added", Enumerable.Empty<FunctionArgument>(), new()));
        MakeResult();
        Assert.AreEqual(0, _result.RemovedFunctions.Count);
        Assert.AreEqual(1, _result.AddedFunctions.Count);
        Assert.AreEqual("added", _result.AddedFunctions.First().Name);
    }

    private void MakeResult()
    {
        _result = CodeModelComparator.GetDifferent(_oldModel, _newModel);
    }
}
