using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.Functions;
using Luna.Functions.Lang;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class RuntimeScopeTest
{
    private Mock<IFileSystem> _fileSystem;
    private Mock<IValueElementEvaluator> _evaluator;
    private Mock<IEmbeddedFunctionsCollection> _embeddedFunctions;
    private List<FunctionDeclaration> _declaredFunctions;
    private List<ConstantDeclaration> _constantDeclarations;
    private Mock<IRuntimeValue> _runtimeValue;
    private Mock<IFunctionRuntimeValue> _func;
    private CallStack _callStack;
    private RuntimeScope _scope;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _evaluator = new Mock<IValueElementEvaluator>();
        _embeddedFunctions = new Mock<IEmbeddedFunctionsCollection>();
        _declaredFunctions = new List<FunctionDeclaration>();
        _constantDeclarations = new List<ConstantDeclaration>();
        _runtimeValue = new Mock<IRuntimeValue>();
        _func = new Mock<IFunctionRuntimeValue>();
        _callStack = new CallStack();
    }

    private void MakeScope()
    {
        _scope = new RuntimeScope(_evaluator.Object, _embeddedFunctions.Object, _declaredFunctions, _constantDeclarations, _callStack);
    }

    [Test]
    public void FromCodeModel()
    {
        var codeModel = new CodeModel();
        codeModel.AddConstantDeclaration(new ConstantDeclaration("const_1", new BooleanValueElement(true)));
        codeModel.AddFunctionDeclaration(new FunctionDeclaration("func_1", Enumerable.Empty<FunctionArgument>(), new FunctionBody()));
        var importCodeModel = new CodeModel();
        importCodeModel.AddConstantDeclaration(new ConstantDeclaration("import_const_1", new IntegerValueElement(123)));
        importCodeModel.AddFunctionDeclaration(new FunctionDeclaration("import_func_1", Enumerable.Empty<FunctionArgument>(), new FunctionBody()));
        codeModel.AddImportDirective(new ImportDirective("importFile", new CodeFileProjectItem("", null, _fileSystem.Object) { CodeModel = importCodeModel }));

        var result = RuntimeScope.FromCodeModel(codeModel, _evaluator.Object, _embeddedFunctions.Object, _callStack);

        Assert.AreEqual(typeof(BooleanValueElement), result.GetConstantValue("const_1").GetType());
        Assert.AreEqual(typeof(IntegerValueElement), result.GetConstantValue("import_const_1").GetType());
        Assert.True(result.IsDeclaredOrEmbeddedFunction("func_1"));
        Assert.True(result.IsDeclaredOrEmbeddedFunction("import_func_1"));
    }

    [Test]
    public void GetConstantValue()
    {
        _constantDeclarations.Add(new("const", new IntegerValueElement(123)));
        MakeScope();
        var value = _scope.GetConstantValue("const");
        Assert.True(value is IntegerValueElement);
        Assert.AreEqual(123, ((IntegerValueElement)value).Value);
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

        _scope.PushCallStack(_func.Object);
        _scope.AddFunctionArgument("x", new IntegerRuntimeValue(123));
        _scope.PushCallStack(_func.Object);
        _scope.AddFunctionArgument("x", new StringRuntimeValue("123"));

        Assert.AreEqual(new StringRuntimeValue("123"), _scope.GetFunctionArgumentValue("x"));
        _scope.PopCallStack();
        Assert.AreEqual(new IntegerRuntimeValue(123), _scope.GetFunctionArgumentValue("x"));
    }

    [Test]
    public void VoidEmptyBody()
    {
        _declaredFunctions.Add(new FunctionDeclaration("func", Enumerable.Empty<FunctionArgument>(), new FunctionBody()));
        MakeScope();
        Assert.AreEqual(VoidRuntimeValue.Instance, _scope.GetDeclaredFunctionValue("func"));
    }

    [Test]
    public void CallStack()
    {
        MakeScope();

        Assert.That(_callStack.Count, Is.EqualTo(0));

        _scope.PushCallStack(new FunctionRuntimeValue("func1", _scope));
        Assert.That(_callStack.First().Name, Is.EqualTo("func1"));

        _scope.PushCallStack(new FunctionRuntimeValue("func2", _scope));
        Assert.That(_callStack.First().Name, Is.EqualTo("func2"));

        _scope.PopCallStack();
        Assert.That(_callStack.First().Name, Is.EqualTo("func1"));

        _scope.PopCallStack();
        Assert.That(_callStack.Count, Is.EqualTo(0));
    }

    [Test]
    public void AddLambda_AlreadyPassedArguments()
    {
        MakeScope();
        _scope.PushCallStack(_func.Object);
        _scope.AddFunctionArgument("x", _runtimeValue.Object);

        var result = _scope.AddLambda(new(new FunctionArgument[] { new("y") }, new()));

        Assert.That(result.AlreadyPassedArguments, Has.Count.EqualTo(1));
        Assert.That(result.AlreadyPassedArguments[0], Is.EqualTo(_runtimeValue.Object));
    }

    [Test]
    public void AddLambda_Arguments()
    {
        MakeScope();
        _scope.PushCallStack(_func.Object);
        _scope.AddFunctionArgument("x", _runtimeValue.Object);

        var lambda = new LambdaValueElement(new FunctionArgument[] { new("y") }, new());
        var addLambdaResult = _scope.AddLambda(lambda);
        var result = _scope.GetFunctionArgumentNames(addLambdaResult.Name);

        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result[0], Is.EqualTo("x"));
        Assert.That(result[1], Is.EqualTo("y"));
    }

    [Test]
    public void GetDeclaredFunctionValue()
    {
        MakeScope();
        _scope.PushCallStack(_func.Object);
        _scope.AddFunctionArgument("x", _runtimeValue.Object);
        _evaluator.Setup(x => x.Eval(_scope, new IntegerValueElement(1))).Returns(new IntegerRuntimeValue(1));
        _evaluator.Setup(x => x.Eval(_scope, new StringValueElement("1"))).Returns(new StringRuntimeValue("1"));

        var lambda = new LambdaValueElement(new FunctionArgument[] { new("y") }, new() { new IntegerValueElement(1), new StringValueElement("1") });
        var addLambdaResult = _scope.AddLambda(lambda);
        var result = _scope.GetDeclaredFunctionValue(addLambdaResult.Name);

        Assert.That(result, Is.EqualTo(new StringRuntimeValue("1")));
        _evaluator.Verify(x => x.Eval(_scope, new IntegerValueElement(1)), Times.Once());
        _evaluator.Verify(x => x.Eval(_scope, new StringValueElement("1")), Times.Once());
    }
}
