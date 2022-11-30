using Luna.CodeElements;
using Luna.Navigation;
using NUnit.Framework;

namespace Luna.Tests.Navigation;

internal class DepthSearchLogicTest
{
    private DepthSearchLogic _logic;

    [SetUp]
    public void Setup()
    {
        _logic = new();
    }

    [Test]
    public void BooleanValueElement()
    {
        var root = new BooleanValueElement(true);
        static bool crit(CodeElement e) => e is BooleanValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void IntegerValueElement()
    {
        var root = new IntegerValueElement(123);
        static bool crit(CodeElement e) => e is IntegerValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void FloatValueElement()
    {
        var root = new FloatValueElement(1.23);
        static bool crit(CodeElement e) => e is FloatValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void StringValueElement()
    {
        var root = new StringValueElement("123");
        static bool crit(CodeElement e) => e is StringValueElement;

        var result = _logic.Seach(root, crit);

        Assert.AreEqual(root, result.CodeElement);
        Assert.NotNull(result);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void NamedConstantValueElement()
    {
        var root = new NamedConstantValueElement("x");
        static bool crit(CodeElement e) => e is NamedConstantValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void VariableValueElement()
    {
        var root = new VariableValueElement("x");
        static bool crit(CodeElement e) => e is VariableValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void FunctionArgumentValueElement()
    {
        var root = new FunctionArgumentValueElement("x");
        static bool crit(CodeElement e) => e is FunctionArgumentValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void ConstantDeclaration()
    {
        var root = new ConstantDeclaration("WIDTH", new IntegerValueElement(123));
        static bool crit(CodeElement e) => e is ConstantDeclaration { Name: "WIDTH" };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void ConstantDeclaration_Value()
    {
        var value = new IntegerValueElement(123);
        var root = new ConstantDeclaration("WIDTH", value);
        static bool crit(CodeElement e) => e is IntegerValueElement { Value: 123 };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(value, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void FunctionDeclaration()
    {
        var root = new FunctionDeclaration("func", new FunctionArgument[0], new());
        static bool crit(CodeElement e) => e is FunctionDeclaration { Name: "func" };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void FunctionDeclaration_Argument()
    {
        var arg = new FunctionArgument("x");
        var root = new FunctionDeclaration("func", new[] { arg }, new());
        static bool crit(CodeElement e) => e is FunctionArgument { Name: "x" };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(arg, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void FunctionDeclaration_Body_Empty()
    {
        var root = new FunctionDeclaration("func", new FunctionArgument[0], new());
        static bool crit(CodeElement e) => false;

        var result = _logic.Seach(root, crit);

        Assert.Null(result);
    }

    [Test]
    public void FunctionDeclaration_Body_1()
    {
        var body = new FloatValueElement(1.2);
        var root = new FunctionDeclaration("func", new FunctionArgument[0], new() { body });
        static bool crit(CodeElement e) => e is FloatValueElement { Value: 1.2 };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(body, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void FunctionDeclaration_Body_2()
    {
        var body1 = new FloatValueElement(1.2);
        var body2 = new BooleanValueElement(true);
        var root = new FunctionDeclaration("func", new FunctionArgument[0], new() { body1, body2 });
        static bool crit(CodeElement e) => e is BooleanValueElement { Value: true };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(body2, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void ListValueElement()
    {
        var root = new ListValueElement(new ValueElement[0]);
        static bool crit(CodeElement e) => e is ListValueElement { Items.Count: 0 };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void ListValueElement_Empty()
    {
        var root = new ListValueElement(new[] { new ListValueElement(new ValueElement[0]) });
        static bool crit(CodeElement e) => false;

        var result = _logic.Seach(root, crit);

        Assert.Null(result);
    }

    [Test]
    public void ListValueElement_Item()
    {
        var item = new StringValueElement("123");
        var root = new ListValueElement(new[] { item });
        static bool crit(CodeElement e) => e is StringValueElement { Value: "123" };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(item, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void LambdaValueElement()
    {
        var root = new LambdaValueElement(new FunctionArgument[0], new());
        static bool crit(CodeElement e) => e is LambdaValueElement;

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void LambdaValueElement_Argument()
    {
        var arg = new FunctionArgument("x");
        var root = new LambdaValueElement(new[] { arg }, new());
        static bool crit(CodeElement e) => e is FunctionArgument { Name: "x" };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(arg, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void LambdaValueElement_Body_Empty()
    {
        var root = new LambdaValueElement(new FunctionArgument[0], new());
        static bool crit(CodeElement e) => false;

        var result = _logic.Seach(root, crit);

        Assert.Null(result);
    }

    [Test]
    public void LambdaValueElement_Body_1()
    {
        var body = new FloatValueElement(1.2);
        var root = new LambdaValueElement(new FunctionArgument[0], new() { body });
        static bool crit(CodeElement e) => e is FloatValueElement { Value: 1.2 };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(body, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void LambdaValueElement_Body_2()
    {
        var body1 = new FloatValueElement(1.2);
        var body2 = new BooleanValueElement(true);
        var root = new LambdaValueElement(new FunctionArgument[0], new() { body1, body2 });
        static bool crit(CodeElement e) => e is BooleanValueElement { Value: true };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(body2, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void FunctionValueElement()
    {
        var root = new FunctionValueElement("func_call", new FunctionValueElement[0]);
        static bool crit(CodeElement e) => e is FunctionValueElement { Name: "func_call" };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(root, result.CodeElement);
        Assert.AreEqual(0, result.Chain.Length);
    }

    [Test]
    public void FunctionValueElement_NoResult()
    {
        var root = new FunctionValueElement("func_call", new FunctionValueElement[0]);
        static bool crit(CodeElement e) => false;

        var result = _logic.Seach(root, crit);

        Assert.Null(result);
    }

    [Test]
    public void FunctionValueElement_Argument()
    {
        var arg = new FloatValueElement(1.2);
        var root = new FunctionValueElement("func_call", new[] { arg });
        static bool crit(CodeElement e) => e is FloatValueElement { Value: 1.2 };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(arg, result.CodeElement);
        Assert.AreEqual(1, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
    }

    [Test]
    public void NoResult()
    {
        var root = new ConstantDeclaration("WIDTH", new IntegerValueElement(123));
        static bool crit(CodeElement e) => e is FunctionValueElement;

        var result = _logic.Seach(root, crit);

        Assert.Null(result);
    }

    [Test]
    public void LongChain()
    {
        var value = new IntegerValueElement(123);
        var innerList = new ListValueElement(new[] { value });
        var list = new ListValueElement(new[] { innerList });
        var lambda = new LambdaValueElement(new FunctionArgument[0], new() { list });
        var root = new FunctionDeclaration("func", new FunctionArgument[0], new() { lambda });
        static bool crit(CodeElement e) => e is IntegerValueElement { Value: 123 };

        var result = _logic.Seach(root, crit);

        Assert.NotNull(result);
        Assert.AreEqual(value, result.CodeElement);
        Assert.AreEqual(4, result.Chain.Length);
        Assert.AreEqual(root, result.Chain[0]);
        Assert.AreEqual(lambda, result.Chain[1]);
        Assert.AreEqual(list, result.Chain[2]);
        Assert.AreEqual(innerList, result.Chain[3]);
    }
}
