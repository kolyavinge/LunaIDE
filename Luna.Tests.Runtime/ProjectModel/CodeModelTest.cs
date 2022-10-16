using Luna.CodeElements;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class CodeModelTest
{
    private CodeModel _codeModel1;
    private CodeModel _codeModel2;

    [SetUp]
    public void Setup()
    {
        _codeModel1 = new CodeModel();
        _codeModel2 = new CodeModel();
    }

    [Test]
    public void Equals_Empty()
    {
        Assert.True(_codeModel1.Equals(_codeModel2));
        Assert.True(_codeModel2.Equals(_codeModel1));
    }

    [Test]
    public void Equals_Null()
    {
        Assert.False(_codeModel1.Equals(null));
    }

    [Test]
    public void Equals_AddImport()
    {
        _codeModel1.AddImportDirective(new("", new("", null, null)));

        Assert.False(_codeModel1.Equals(_codeModel2));
        Assert.False(_codeModel2.Equals(_codeModel1));
    }

    [Test]
    public void Equals_AddConstant()
    {
        _codeModel1.AddConstantDeclaration(new("", new IntegerValueElement(1)));

        Assert.False(_codeModel1.Equals(_codeModel2));
        Assert.False(_codeModel2.Equals(_codeModel1));
    }

    [Test]
    public void Equals_AddFunction()
    {
        _codeModel1.AddFunctionDeclaration(new("", new FunctionArgument[0], new()));

        Assert.False(_codeModel1.Equals(_codeModel2));
        Assert.False(_codeModel2.Equals(_codeModel1));
    }
}
