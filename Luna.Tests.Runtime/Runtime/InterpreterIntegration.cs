using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Runtime;

internal class InterpreterIntegration : BaseInterpreterTest
{
    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void ConstList()
    {
        CodeFile(@"
            (func () (123 1.23 '123'))
            (run (func))
");
        Run();
        Assert.AreEqual("(123 1.23 '123')", _resultString);
    }

    [Test]
    public void DeclaredConstList()
    {
        CodeFile(@"
            const INT_CONST 123
            const FLOAT_CONST 1.23
            const STRING_CONST '123'
            (func () (INT_CONST FLOAT_CONST STRING_CONST))
            (run (func))
");
        Run();
        Assert.AreEqual("(123 1.23 '123')", _resultString);
    }

    [Test]
    public void Func()
    {
        CodeFile(@"
            (func (x) x)
            (run (func 123))
");
        Run();
        Assert.AreEqual("123", _resultString);
    }

    [Test]
    public void Lambda()
    {
        CodeFile(@"
            (func (x) (x))
            (run (func (lambda () 1.23)))
");
        Run();
        Assert.AreEqual("1.23", _resultString);
    }

    [Test]
    public void LambdaResult()
    {
        CodeFile(@"
            (func (x) x)
            (run (func (lambda (x) x)))
");
        Run();
        Assert.AreEqual("lambda_0", _resultString);
    }

    [Test]
    public void LambdaWithFuncArgument_1()
    {
        CodeFile(@"
            (func (x) (x))
            (func2 (x) (lambda () x))
            (run (func (func2 4)))
");
        Run();
        Assert.AreEqual("4", _resultString);
    }

    [Test]
    public void LambdaWithFuncArgument_2()
    {
        CodeFile(@"
            (func2 (x) (lambda (y) (1 x y)))
            (run (func2 2 3))
");
        Run();
        Assert.AreEqual("(1 2 3)", _resultString);
    }

    [Test]
    public void Carrying_1()
    {
        CodeFile(@"
            (func (x y) (1 x y))
            (func2 () (func 2))
            (run (func2 3))
");
        Run();
        Assert.AreEqual("(1 2 3)", _resultString);
    }

    [Test]
    public void Carrying_2()
    {
        CodeFile(@"
            (func (x y) (1 x y))
            (func2 (x) (x 3))
            (run (func2 (func 2)))
");
        Run();
        Assert.AreEqual("(1 2 3)", _resultString);
    }

    [Test]
    public void SameArgumentName()
    {
        CodeFile(@"
            (func (x) x)
            (func2 (x) (func x))
            (run (func2 1))
");
        Run();
        Assert.AreEqual("1", _resultString);
    }

    [Test]
    public void If()
    {
        CodeFile(@"
            (run (if true 1 2))
");
        Run();
        Assert.AreEqual("1", _resultString);
    }

    [Test]
    public void Add()
    {
        CodeFile(@"
            (run (+ 1 2))
");
        Run();
        Assert.AreEqual("3", _resultString);
    }

    [Test]
    public void Sub()
    {
        CodeFile(@"
            (run (- 1 2))
");
        Run();
        Assert.AreEqual("-1", _resultString);
    }

    [Test]
    public void Mul()
    {
        CodeFile(@"
            (run (* 3 2))
");
        Run();
        Assert.AreEqual("6", _resultString);
    }

    [Test]
    public void Div()
    {
        CodeFile(@"
            (run (/ 8 2))
");
        Run();
        Assert.AreEqual("4", _resultString);
    }
}
