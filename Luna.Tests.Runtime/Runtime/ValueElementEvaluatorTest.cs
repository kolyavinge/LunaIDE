using Luna.Collections;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class ValueElementEvaluatorTest
{
    private readonly CodeModel _codeModel = new();
    private Mock<IRuntimeScope> _scope;
    private Mock<IRuntimeScopesCollection> _scopes;
    private ValueElementEvaluator _evaluator;

    [SetUp]
    public void Setup()
    {
        _scope = new Mock<IRuntimeScope>();
        _scopes = new Mock<IRuntimeScopesCollection>();
        _scopes.Setup(x => x.GetForCodeModel(_codeModel)).Returns(_scope.Object);
        _evaluator = new ValueElementEvaluator { Scopes = _scopes.Object };
    }

    [Test]
    public void ListRuntimeValue()
    {
        _scope.Setup(x => x.GetConstantValue("const")).Returns(new IntegerValueElement(789));
        _scope.Setup(x => x.GetFunctionArgumentValue("x")).Returns(new FloatRuntimeValue(7.89));
        var listElement = new ListValueElement(new ValueElement[]
        {
            new BooleanValueElement(true),
            new IntegerValueElement(123),
            new FloatValueElement(1.23),
            new StringValueElement("123"),
            new NamedConstantValueElement("const"),
            new FunctionArgumentValueElement("x"),
            new ListValueElement(new[] { new IntegerValueElement(888) })
        });
        var result = (ListRuntimeValue)_evaluator.Eval(_scope.Object, listElement);
        Assert.AreEqual(7, result.Count);
        Assert.AreEqual(new BooleanRuntimeValue(true), result.GetItem(0));
        Assert.AreEqual(new IntegerRuntimeValue(123), result.GetItem(1));
        Assert.AreEqual(new FloatRuntimeValue(1.23), result.GetItem(2));
        Assert.AreEqual(new StringRuntimeValue("123"), result.GetItem(3));
        Assert.AreEqual(new IntegerRuntimeValue(789), result.GetItem(4));
        Assert.AreEqual(new FloatRuntimeValue(7.89), result.GetItem(5));
        Assert.AreEqual(new ListRuntimeValue(new[] { new IntegerRuntimeValue(888) }), result.GetItem(6));
    }

    [Test]
    public void DeclaredFunctionRuntimeValue()
    {
        var arguments = new[] { new IntegerValueElement(123) };
        var func = new FunctionValueElement(_codeModel, "func", arguments);
        _scope.Setup(x => x.IsDeclaredFunction("func")).Returns(true);
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray())).Returns(new IntegerRuntimeValue(888));
        var result = (IntegerRuntimeValue)_evaluator.Eval(_scope.Object, func);
        Assert.AreEqual(888, result.Value);
        _scope.Verify(x => x.PushFunctionArguments(), Times.Once());
        _scope.Verify(x => x.PopFunctionArguments(), Times.Once());
    }

    [Test]
    public void ArgumentAsFunctionRuntimeValue()
    {
        var arguments = new[] { new IntegerValueElement(123) };
        var func = new FunctionValueElement(_codeModel, "func", arguments);
        _scope.Setup(x => x.IsDeclaredFunction("func")).Returns(false);
        _scope.Setup(x => x.GetFunctionArgumentNames("func")).Returns(new[] { "x" });
        _scope.Setup(x => x.GetDeclaredFunctionValue("func", new IRuntimeValue[] { new IntegerRuntimeValue(123) }.ToReadonlyArray())).Returns(new IntegerRuntimeValue(888));
        var result = (IntegerRuntimeValue)_evaluator.Eval(_scope.Object, func);
        Assert.AreEqual(888, result.Value);
        _scope.Verify(x => x.PushFunctionArguments(), Times.Never());
        _scope.Verify(x => x.PopFunctionArguments(), Times.Never());
    }

    [Test]
    public void LambdaValueElement()
    {
        var arguments = new[] { new FunctionArgument("x") };
        var lambda = new LambdaValueElement(arguments, new FunctionBody());
        _scope.Setup(x => x.AddLambda(lambda)).Returns("lambda_0");
        var result = (FunctionRuntimeValue)_evaluator.Eval(_scope.Object, lambda);
        _scope.Verify(x => x.AddLambda(lambda), Times.Once());
        _scope.Verify(x => x.PushFunctionArguments(), Times.Never());
        _scope.Verify(x => x.PopFunctionArguments(), Times.Never());
    }
}
