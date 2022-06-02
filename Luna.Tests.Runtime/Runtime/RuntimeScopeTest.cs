using System.Collections.Generic;
using System.Linq;
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
    private List<ConstDeclaration> _constDeclarations;
    private RuntimeScope _scope;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
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

    [Test]
    public void FromCodeModel()
    {
        var codeModel = new CodeModel();
        codeModel.AddConstDeclaration(new ConstDeclaration("const_1", new BooleanValueElement(true)));
        codeModel.AddFunctionDeclaration(new FunctionDeclaration("func_1", Enumerable.Empty<FunctionArgument>(), new FunctionBody()));
        var importCodeModel = new CodeModel();
        importCodeModel.AddConstDeclaration(new ConstDeclaration("import_const_1", new IntegerValueElement(123)));
        importCodeModel.AddFunctionDeclaration(new FunctionDeclaration("import_func_1", Enumerable.Empty<FunctionArgument>(), new FunctionBody()));
        codeModel.AddImportDirective(new ImportDirective("importFile", new CodeFileProjectItem("", null, _fileSystem.Object) { CodeModel = importCodeModel }));

        var result = RuntimeScope.FromCodeModel(codeModel, _evaluator.Object, _embeddedFunctions.Object);

        Assert.AreEqual(typeof(BooleanValueElement), result.GetConstantValue("const_1").GetType());
        Assert.AreEqual(typeof(IntegerValueElement), result.GetConstantValue("import_const_1").GetType());
        Assert.True(result.IsDeclaredFunction("func_1"));
        Assert.True(result.IsDeclaredFunction("import_func_1"));
    }
}
