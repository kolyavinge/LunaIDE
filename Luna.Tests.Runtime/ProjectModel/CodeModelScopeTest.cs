using System.Linq;
using Luna.CodeElements;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class CodeModelScopeTest
{
    private CodeModel _codeModel;
    private CodeModel _importedCodeModel;
    private CodeModelScope _scope;

    [SetUp]
    public void Setup()
    {
        _codeModel = new CodeModel();
        _importedCodeModel = new CodeModel();
        _codeModel.AddImportDirective(new ImportDirective("", new CodeFileProjectItem("", null, null) { CodeModel = _importedCodeModel }));
        _scope = new CodeModelScope();
    }

    [Test]
    public void IsConstantExist()
    {
        _codeModel.AddConstantDeclaration(new("const", new IntegerValueElement(1)));
        _importedCodeModel.AddConstantDeclaration(new("importConst", new IntegerValueElement(1)));
        _scope.IsConstantExist(_codeModel, "const");
        _scope.IsConstantExist(_codeModel, "importConst");
    }

    [Test]
    public void IsFunctionExist()
    {
        _codeModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        _importedCodeModel.AddFunctionDeclaration(new("importFunc", Enumerable.Empty<FunctionArgument>(), new()));
        _scope.IsFunctionExist(_codeModel, "func");
        _scope.IsFunctionExist(_codeModel, "importFunc");
        _scope.IsFunctionExist(_codeModel, "eq");
    }

    [Test]
    public void GetScopeIdentificators()
    {
        _codeModel.AddConstantDeclaration(new("const", new IntegerValueElement(1)));
        _importedCodeModel.AddConstantDeclaration(new("importConst", new IntegerValueElement(1)));
        _codeModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        _importedCodeModel.AddFunctionDeclaration(new("importFunc", Enumerable.Empty<FunctionArgument>(), new()));

        var result = _scope.GetScopeIdentificators(_codeModel);

        Assert.AreEqual(1, result.DeclaredConstants.Count);
        Assert.AreEqual(1, result.DeclaredFunctions.Count);
        Assert.AreEqual(1, result.ImportedConstants.Count);
        Assert.AreEqual(1, result.ImportedFunctions.Count);
        Assert.True(result.DeclaredConstants.Contains("const"));
        Assert.True(result.DeclaredFunctions.Contains("func"));
        Assert.True(result.ImportedConstants.Contains("importConst"));
        Assert.True(result.ImportedFunctions.Contains("importFunc"));
    }
}
