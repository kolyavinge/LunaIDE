using System.Collections.Generic;
using Luna.Functions;
using Luna.Functions.Lang;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class RuntimeScopeTest
{
    private Mock<IValueElementEvaluator> _evaluator;
    private Mock<IEmbeddedFunctionsCollection> _embeddedFunctions;
    private List<FunctionDeclaration> _declaredFunctions;
    private List<ConstDeclaration> _constDeclarations;
    private RuntimeScope _scope;

    [SetUp]
    public void Setup()
    {
        _evaluator = new Mock<IValueElementEvaluator>();
        _embeddedFunctions = new Mock<IEmbeddedFunctionsCollection>();
        _declaredFunctions = new List<FunctionDeclaration>();
        _constDeclarations = new List<ConstDeclaration>();
    }

    private void MakeScope()
    {
        _scope = new RuntimeScope(_evaluator.Object, _embeddedFunctions.Object, _declaredFunctions, _constDeclarations);
    }

    [Test]
    public void IsFunction()
    {
        MakeScope();

        Assert.False(_scope.ArgumentCalledAsFunction("wrong"));

        _scope.AddFunctionArgument("x", new BooleanRuntimeValue(false));
        Assert.False(_scope.ArgumentCalledAsFunction("x"));
        _scope.RemoveFunctionArgument("x");

        _scope.AddFunctionArgument("x", new FunctionRuntimeValue("func", _scope));
        Assert.True(_scope.ArgumentCalledAsFunction("x"));
        _scope.RemoveFunctionArgument("x");
    }

    [Test]
    public void GetFunctionArgumentNames()
    {
        _embeddedFunctions.Setup(x => x.Contains("eq")).Returns(true);
        _embeddedFunctions.Setup(x => x.GetByName("eq")).Returns(new Eq());
        _declaredFunctions.Add(new FunctionDeclaration("func", new[] { new FunctionArgument("z") }, new()));

        MakeScope();

        Assert.AreEqual(new[] { "x", "y" }, _scope.GetFunctionArgumentNames("eq"));
        Assert.AreEqual(new[] { "z" }, _scope.GetFunctionArgumentNames("func"));
    }

    [Test]
    public void GetFunctionArgumentValue()
    {
        MakeScope();
        _scope.AddFunctionArgument("x", new IntegerRuntimeValue(123));
        _scope.PushFunctionArguments();
        _scope.AddFunctionArgument("x", new StringRuntimeValue("123"));

        Assert.AreEqual(new StringRuntimeValue("123"), _scope.GetFunctionArgumentValue("x"));
        _scope.PopFunctionArguments();
        Assert.AreEqual(new IntegerRuntimeValue(123), _scope.GetFunctionArgumentValue("x"));
    }
}
