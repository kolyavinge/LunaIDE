﻿using Luna.CodeElements;
using Luna.Navigation;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.Navigation;

internal class DeclarationNavigatorTest
{
    private ConstantDeclaration _constant, _importedConstant, _wrongConstant;
    private FunctionDeclaration _func, _importedFunc, _wrongFunc;
    private CodeFileProjectItem _codeFile, _importedCodeFile;
    private DeclarationNavigator _navigator;

    [SetUp]
    public void Setup()
    {
        _importedConstant = new("IMPORTED_CONST", new IntegerValueElement(1));
        _importedFunc = new("IMPORTED_FUNC", new FunctionArgument[0], new());
        _importedCodeFile = new("", null, null);
        _importedCodeFile.CodeModel.AddConstantDeclaration(_importedConstant);
        _importedCodeFile.CodeModel.AddFunctionDeclaration(_importedFunc);

        _constant = new("CONST", new IntegerValueElement(1));
        _func = new("FUNC", new FunctionArgument[0], new());
        _codeFile = new("", null, null);
        _codeFile.CodeModel.AddImportDirective(new("", _importedCodeFile));
        _codeFile.CodeModel.AddConstantDeclaration(_constant);
        _codeFile.CodeModel.AddFunctionDeclaration(_func);

        _wrongConstant = new ConstantDeclaration("WRONG_CONST", new IntegerValueElement(1));
        _wrongFunc = new FunctionDeclaration("WRONG_FUNC", new FunctionArgument[0], new());

        _navigator = new();
    }

    [Test]
    public void ConstantDeclaration()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, _constant);
        Assert.NotNull(result);
        Assert.AreEqual(_constant, result.Declaration);
        Assert.AreEqual(_codeFile, result.CodeFile);
    }

    [Test]
    public void ConstantDeclaration_Imported()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, _importedConstant);
        Assert.NotNull(result);
        Assert.AreEqual(_importedConstant, result.Declaration);
        Assert.AreEqual(_importedCodeFile, result.CodeFile);
    }

    [Test]
    public void ConstantDeclaration_Wrong()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, _wrongConstant);
        Assert.Null(result);
    }

    [Test]
    public void FunctionDeclaration()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, _func);
        Assert.NotNull(result);
        Assert.AreEqual(_func, result.Declaration);
        Assert.AreEqual(_codeFile, result.CodeFile);
    }

    [Test]
    public void FunctionDeclaration_Imported()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, _importedFunc);
        Assert.NotNull(result);
        Assert.AreEqual(_importedFunc, result.Declaration);
        Assert.AreEqual(_importedCodeFile, result.CodeFile);
    }

    [Test]
    public void FunctionDeclaration_Wrong()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, _wrongFunc);
        Assert.Null(result);
    }

    [Test]
    public void NamedConstant()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, new NamedConstantValueElement("CONST"));
        Assert.NotNull(result);
        Assert.AreEqual(_constant, result.Declaration);
        Assert.AreEqual(_codeFile, result.CodeFile);
    }

    [Test]
    public void NamedConstant_Imported()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, new NamedConstantValueElement("IMPORTED_CONST"));
        Assert.NotNull(result);
        Assert.AreEqual(_importedConstant, result.Declaration);
        Assert.AreEqual(_importedCodeFile, result.CodeFile);
    }

    [Test]
    public void Function()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, new FunctionValueElement("FUNC", new ValueElement[0]));
        Assert.NotNull(result);
        Assert.AreEqual(_func, result.Declaration);
        Assert.AreEqual(_codeFile, result.CodeFile);
    }

    [Test]
    public void Function_Imported()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, new FunctionValueElement("IMPORTED_FUNC", new ValueElement[0]));
        Assert.NotNull(result);
        Assert.AreEqual(_importedFunc, result.Declaration);
        Assert.AreEqual(_importedCodeFile, result.CodeFile);
    }

    [Test]
    public void Function_WrongCodeElement()
    {
        var result = _navigator.GetDeclarationFor(_codeFile, new IntegerValueElement(1));
        Assert.Null(result);
    }
}
