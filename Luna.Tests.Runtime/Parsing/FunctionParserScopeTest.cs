using System.Collections.Generic;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class FunctionParserScopeTest
{
    private Mock<IFileSystem> _fileSystem;
    private CodeModel _importCodeModel, _currentCodeModel;
    private FunctionParserScope _scope;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();

        _importCodeModel = new CodeModel();
        _importCodeModel.AddConstDeclaration(new ConstDeclaration("importConst", new IntegerValue(1, 0, 0), 0, 0));
        _importCodeModel.AddFunctionDeclaration(new FunctionDeclaration("importFunc", new List<FunctionArgument>(), null, 0, 0));
        _importCodeModel.RunFunction = new FunctionValueElement("", new List<ValueElement>(), 0, 0);

        _currentCodeModel = new CodeModel();
        _currentCodeModel.AddImportDirective(new ImportDirective("", new CodeFileProjectItem("", null, _fileSystem.Object) { CodeModel = _importCodeModel }, 0, 0));
        _currentCodeModel.AddConstDeclaration(new ConstDeclaration("currentConst", new IntegerValue(1, 0, 0), 0, 0));
        _currentCodeModel.AddFunctionDeclaration(new FunctionDeclaration("currentFunc", new List<FunctionArgument>(), null, 0, 0));

        var allCodeModels = new[] { _importCodeModel, _currentCodeModel };

        _scope = new FunctionParserScope(allCodeModels, _currentCodeModel);
    }

    [Test]
    public void IsConstExist()
    {
        Assert.False(_scope.IsConstExist("const"));
        Assert.True(_scope.IsConstExist("currentConst"));
        Assert.True(_scope.IsConstExist("importConst"));
    }

    [Test]
    public void IsFunctionExist()
    {
        Assert.False(_scope.IsFunctionExist("func"));
        Assert.True(_scope.IsFunctionExist("currentFunc"));
        Assert.True(_scope.IsFunctionExist("importFunc"));
    }

    [Test]
    public void IsRunFunctionExist()
    {
        Assert.True(_scope.IsRunFunctionExist());
        _importCodeModel.RunFunction = null;
        Assert.False(_scope.IsRunFunctionExist());
    }
}
