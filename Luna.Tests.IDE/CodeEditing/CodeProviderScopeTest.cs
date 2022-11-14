using Luna.CodeElements;
using Luna.IDE.CodeEditing;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.IDE.CodeEditing;

internal class CodeProviderScopeTest
{
    private CodeFileProjectItem _codeFile;
    private CodeProviderScope _scope;

    [SetUp]
    public void Setup()
    {
        _codeFile = new CodeFileProjectItem("", null, null);
        _scope = new CodeProviderScope(_codeFile);
    }

    [Test]
    public void IsConstant()
    {
        Assert.False(_scope.IsConstant("const123"));

        _codeFile.CodeModel.AddConstantDeclaration(new("const123", new IntegerValueElement(123)));
        Assert.True(_scope.IsConstant("const123"));
    }

    [Test]
    public void IsFunction()
    {
        Assert.False(_scope.IsFunction("func123"));

        _codeFile.CodeModel.AddFunctionDeclaration(new("func123", new FunctionArgument[0], new()));
        Assert.True(_scope.IsFunction("func123"));
    }
}
