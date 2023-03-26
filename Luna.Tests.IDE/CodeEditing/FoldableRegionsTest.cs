using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.IDE.CodeEditing;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.IDE.CodeEditing;

internal class FoldableRegionsTest
{
    private CodeModel _codeModel;
    private FoldableRegions _foldableRegions;

    [SetUp]
    public void Setup()
    {
        _codeModel = new CodeModel();
        _foldableRegions = new FoldableRegions();
    }

    [Test]
    public void GetRegions_EmptyModel()
    {
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_OneImport()
    {
        _codeModel.AddImportDirective(new("file path", new CodeFileProjectItem("", null, null), 0, 0));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoImports()
    {
        _codeModel.AddImportDirective(new("file path 1", new CodeFileProjectItem("", null, null), 0, 0));
        _codeModel.AddImportDirective(new("file path 2", new CodeFileProjectItem("", null, null), 1, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_ThreeImports()
    {
        _codeModel.AddImportDirective(new("file path 1", new CodeFileProjectItem("", null, null), 1, 0));
        _codeModel.AddImportDirective(new("file path 2", new CodeFileProjectItem("", null, null), 2, 0));
        _codeModel.AddImportDirective(new("file path 3", new CodeFileProjectItem("", null, null), 4, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(1));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_OneConst()
    {
        _codeModel.AddConstantDeclaration(new("const", new IntegerValueElement(1), 1, 0));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoConsts()
    {
        _codeModel.AddConstantDeclaration(new("const 1", new IntegerValueElement(1), 1, 0));
        _codeModel.AddConstantDeclaration(new("const 2", new IntegerValueElement(1), 3, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(1));
        Assert.That(result[0].LinesCount, Is.EqualTo(2));
    }

    [Test]
    public void GetRegions_ConstsAndOneFuncBetween()
    {
        _codeModel.AddConstantDeclaration(new("const 1", new IntegerValueElement(1), 1, 0));
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(), 3, 0));
        _codeModel.AddConstantDeclaration(new("const 2", new IntegerValueElement(1), 5, 0));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_TwoConstsAndOneFuncBetween()
    {
        _codeModel.AddConstantDeclaration(new("const 1", new IntegerValueElement(1), 1, 0));
        _codeModel.AddConstantDeclaration(new("const 2", new IntegerValueElement(1), 2, 0));
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(), 3, 0));
        _codeModel.AddConstantDeclaration(new("const 3", new IntegerValueElement(1), 4, 0));
        _codeModel.AddConstantDeclaration(new("const 4", new IntegerValueElement(1), 5, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].LineIndex, Is.EqualTo(1));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
        Assert.That(result[1].LineIndex, Is.EqualTo(4));
        Assert.That(result[1].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_ConstsAndOneFuncEnd()
    {
        _codeModel.AddConstantDeclaration(new("const 1", new IntegerValueElement(1), 1, 0));
        _codeModel.AddConstantDeclaration(new("const 2", new IntegerValueElement(1), 2, 0));
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(), 3, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(1));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_OneEmptyFunction()
    {
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(), 0, 0));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_OneInlineFunction()
    {
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(0, 10, 0, 15), 0, 0));
        var result = GetRegions();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetRegions_OneFunction()
    {
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(1, 0, 3, 0), 0, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_TwoFunctions()
    {
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func 1", Enumerable.Empty<FunctionArgument>(), new(1, 0, 3, 0), 0, 0));
        _codeModel.AddFunctionDeclaration(new(_codeModel, "func 2", Enumerable.Empty<FunctionArgument>(), new(11, 0, 13, 0), 10, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
        Assert.That(result[1].LineIndex, Is.EqualTo(10));
        Assert.That(result[1].LinesCount, Is.EqualTo(3));
    }

    [Test]
    public void GetRegions_OneFunctionWithInners_OneLine()
    {
        _codeModel.AddFunctionDeclaration(
            new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(0, 0, 1, 0, new[] { new FunctionValueElement("f", Enumerable.Empty<ValueElement>(), 1, 0, 1, 0) }), 0, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(1));
    }

    [Test]
    public void GetRegions_OneFunctionWithInners_ThreeLines()
    {
        _codeModel.AddFunctionDeclaration(
            new(_codeModel, "func", Enumerable.Empty<FunctionArgument>(), new(0, 0, 3, 0, new[] { new FunctionValueElement("f", Enumerable.Empty<ValueElement>(), 1, 0, 3, 0) }), 0, 0));
        var result = GetRegions();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].LineIndex, Is.EqualTo(0));
        Assert.That(result[0].LinesCount, Is.EqualTo(3));
        Assert.That(result[1].LineIndex, Is.EqualTo(1));
        Assert.That(result[1].LinesCount, Is.EqualTo(2));
    }

    private List<FoldableRegion> GetRegions()
    {
        return _foldableRegions.GetRegions(_codeModel).ToList();
    }
}
