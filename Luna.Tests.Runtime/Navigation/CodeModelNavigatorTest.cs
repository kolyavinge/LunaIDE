using Luna.CodeElements;
using Luna.Navigation;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.Navigation;

internal class CodeModelNavigatorTest
{
    private CodeModel _codeModel;
    private CodeModelNavigator _navigator;
    private CodeModelNavigatorResult _result;

    [SetUp]
    public void Setup()
    {
        _codeModel = new();

        _codeModel.AddConstantDeclaration(new("WIDTH", new IntegerValueElement(1, 0, 12), 0, 6));

        _codeModel.AddFunctionDeclaration(new(
            _codeModel,
            "get_value",
            new FunctionArgument[] { new("x", 5, 12), new("y", 5, 14) },
            new() { new IntegerValueElement(1, 6, 4), new FloatValueElement(1.0, 7, 4) },
            5, 1));

        _codeModel.AddFunctionDeclaration(new(
            _codeModel,
            "get_list",
            new FunctionArgument[0],
            new() { new ListValueElement(new[] { new StringValueElement("123", 9, 6) }, 9, 4) },
            8, 1));

        _codeModel.AddFunctionDeclaration(new(
            _codeModel,
            "with_lambda",
            new FunctionArgument[] { new("x", 12, 12) },
            new()
            {
                new LambdaValueElement(
                    new FunctionArgument[] { new("x", 12, 5) },
                    new() { new BooleanValueElement(true, 13, 6), new FunctionArgumentValueElement("x", 13, 10) },
                    13, 4)
            },
            12, 1));

        _codeModel.AddFunctionDeclaration(new(
            _codeModel,
            "func_call",
            new FunctionArgument[] { new("x", 16, 12) },
            new()
            {
                new FunctionValueElement(
                    "func",
                    new ValueElement[] { new FunctionArgumentValueElement("x", 17, 8), new FloatValueElement(1.0, 17, 10) },
                    17, 4, 17, 4)
            },
            16, 1));

        _codeModel.RunFunction = new FunctionValueElement("func_in_run", new[] { new FloatValueElement(1.0, 20, 25) }, 20, 1, 20, 1);

        _navigator = new();
    }

    [Test]
    public void ConstantDeclaration_Name()
    {
        GetCodeElementByPosition(0, 6);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(0, _result.CodeElement.LineIndex);
        Assert.AreEqual(6, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(ConstantDeclaration), _result.CodeElement.GetType());
        Assert.AreEqual("WIDTH", ((ConstantDeclaration)_result.CodeElement).Name);
        Assert.AreEqual(0, _result.Chain.Length);
    }

    [Test]
    public void ConstantDeclaration_Value()
    {
        GetCodeElementByPosition(0, 12);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(0, _result.CodeElement.LineIndex);
        Assert.AreEqual(12, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(IntegerValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1, ((IntegerValueElement)_result.CodeElement).Value);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(ConstantDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Name()
    {
        GetCodeElementByPosition(5, 1);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(5, _result.CodeElement.LineIndex);
        Assert.AreEqual(1, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.CodeElement.GetType());
        Assert.AreEqual("get_value", ((FunctionDeclaration)_result.CodeElement).Name);
        Assert.AreEqual(0, _result.Chain.Length);
    }

    [Test]
    public void FunctionDeclaration_Argument_1()
    {
        GetCodeElementByPosition(5, 12);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(5, _result.CodeElement.LineIndex);
        Assert.AreEqual(12, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionArgument), _result.CodeElement.GetType());
        Assert.AreEqual("x", ((FunctionArgument)_result.CodeElement).Name);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Argument_2()
    {
        GetCodeElementByPosition(5, 14);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(5, _result.CodeElement.LineIndex);
        Assert.AreEqual(14, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionArgument), _result.CodeElement.GetType());
        Assert.AreEqual("y", ((FunctionArgument)_result.CodeElement).Name);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body1_Integer()
    {
        GetCodeElementByPosition(6, 4);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(6, _result.CodeElement.LineIndex);
        Assert.AreEqual(4, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(IntegerValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1, ((IntegerValueElement)_result.CodeElement).Value);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body2_Float()
    {
        GetCodeElementByPosition(7, 4);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(7, _result.CodeElement.LineIndex);
        Assert.AreEqual(4, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FloatValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1.0, ((FloatValueElement)_result.CodeElement).Value);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_List()
    {
        GetCodeElementByPosition(9, 4);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(9, _result.CodeElement.LineIndex);
        Assert.AreEqual(4, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(ListValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1, ((ListValueElement)_result.CodeElement).Items.Count);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_StringInList()
    {
        GetCodeElementByPosition(9, 6);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(9, _result.CodeElement.LineIndex);
        Assert.AreEqual(6, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(StringValueElement), _result.CodeElement.GetType());
        Assert.AreEqual("123", ((StringValueElement)_result.CodeElement).Value);
        Assert.AreEqual(2, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
        Assert.AreEqual(typeof(ListValueElement), _result.Chain[1].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_Lambda()
    {
        GetCodeElementByPosition(13, 4);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(13, _result.CodeElement.LineIndex);
        Assert.AreEqual(4, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(LambdaValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_BooleanInLambda()
    {
        GetCodeElementByPosition(13, 6);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(13, _result.CodeElement.LineIndex);
        Assert.AreEqual(6, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(BooleanValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(true, ((BooleanValueElement)_result.CodeElement).Value);
        Assert.AreEqual(2, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
        Assert.AreEqual(typeof(LambdaValueElement), _result.Chain[1].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_LambdaArgument()
    {
        GetCodeElementByPosition(12, 5);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(12, _result.CodeElement.LineIndex);
        Assert.AreEqual(5, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionArgument), _result.CodeElement.GetType());
        Assert.AreEqual("x", ((FunctionArgument)_result.CodeElement).Name);
        Assert.AreEqual(2, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
        Assert.AreEqual(typeof(LambdaValueElement), _result.Chain[1].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_FloatAsLambdaArgument()
    {
        GetCodeElementByPosition(13, 10);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(13, _result.CodeElement.LineIndex);
        Assert.AreEqual(10, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionArgumentValueElement), _result.CodeElement.GetType());
        Assert.AreEqual("x", ((FunctionArgumentValueElement)_result.CodeElement).Name);
        Assert.AreEqual(2, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
        Assert.AreEqual(typeof(LambdaValueElement), _result.Chain[1].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_Argument()
    {
        GetCodeElementByPosition(17, 8);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(17, _result.CodeElement.LineIndex);
        Assert.AreEqual(8, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionArgumentValueElement), _result.CodeElement.GetType());
        Assert.AreEqual("x", ((FunctionArgumentValueElement)_result.CodeElement).Name);
        Assert.AreEqual(2, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
        Assert.AreEqual(typeof(FunctionValueElement), _result.Chain[1].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_FunctionCall()
    {
        GetCodeElementByPosition(17, 4);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(17, _result.CodeElement.LineIndex);
        Assert.AreEqual(4, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionValueElement), _result.CodeElement.GetType());
        Assert.AreEqual("func", ((FunctionValueElement)_result.CodeElement).Name);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
    }

    [Test]
    public void FunctionDeclaration_Body_FloatAsFunctionArgument()
    {
        GetCodeElementByPosition(17, 10);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(17, _result.CodeElement.LineIndex);
        Assert.AreEqual(10, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FloatValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1.0, ((FloatValueElement)_result.CodeElement).Value);
        Assert.AreEqual(2, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionDeclaration), _result.Chain[0].GetType());
        Assert.AreEqual(typeof(FunctionValueElement), _result.Chain[1].GetType());
    }

    [Test]
    public void RunFunction()
    {
        GetCodeElementByPosition(20, 1);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(20, _result.CodeElement.LineIndex);
        Assert.AreEqual(1, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FunctionValueElement), _result.CodeElement.GetType());
        Assert.AreEqual("func_in_run", ((FunctionValueElement)_result.CodeElement).Name);
        Assert.AreEqual(0, _result.Chain.Length);
    }

    [Test]
    public void RunFunction_Argument()
    {
        GetCodeElementByPosition(20, 25);
        Assert.NotNull(_result.CodeElement);
        Assert.AreEqual(20, _result.CodeElement.LineIndex);
        Assert.AreEqual(25, _result.CodeElement.ColumnIndex);
        Assert.AreEqual(typeof(FloatValueElement), _result.CodeElement.GetType());
        Assert.AreEqual(1.0, ((FloatValueElement)_result.CodeElement).Value);
        Assert.AreEqual(1, _result.Chain.Length);
        Assert.AreEqual(typeof(FunctionValueElement), _result.Chain[0].GetType());
    }

    [Test]
    public void NoResult()
    {
        GetCodeElementByPosition(0, 1);
        Assert.Null(_result);
    }

    [Test]
    public void ConstantDeclaration_NoElementInParent()
    {
        GetCodeElementByPosition(0, 7);
        Assert.Null(_result);
    }

    private void GetCodeElementByPosition(int lineIndex, int columnIndex)
    {
        _result = _navigator.GetCodeElementByPosition(_codeModel, lineIndex, columnIndex);
    }
}
