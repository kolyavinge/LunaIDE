using Luna.Runtime;
using Luna.Tests.Tools;
using Moq;
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
        Run(@"
            (func () (123 1.23 '123'))
            (run (func))");
        Assert.AreEqual("(123 1.23 '123')", _resultString);
    }

    [Test]
    public void DeclaredConstList()
    {
        Run(@"
            const INT_CONST 123
            const FLOAT_CONST 1.23
            const STRING_CONST '123'
            (func () (INT_CONST FLOAT_CONST STRING_CONST))
            (run (func))");
        Assert.AreEqual("(123 1.23 '123')", _resultString);
    }

    [Test]
    public void ListGetValue()
    {
        Run(@"
            (get_list () ((+ 1 2) (+ 3 4)))
            (run (get_list))");
        Assert.AreEqual("(3 7)", _resultString);
    }

    [Test]
    public void Func()
    {
        Run(@"
            (func (x) x)
            (run (func 123))");
        Assert.AreEqual("123", _resultString);
    }

    [Test]
    public void Lambda()
    {
        Run(@"
            (func (x) (x))
            (run (func (lambda () 1.23)))");
        Assert.AreEqual("1.23", _resultString);
    }

    [Test]
    public void LambdaResult()
    {
        Run(@"
            (func (x) x)
            (run (func (lambda (x) x)))");
        Assert.AreEqual("#lambda_0", _resultString);
    }

    [Test]
    public void LambdaWithFuncArgument_1()
    {
        Run(@"
            (func (x) (x))
            (func2 (x) (lambda () x))
            (run (func (func2 4)))");
        Assert.AreEqual("4", _resultString);
    }

    [Test]
    public void LambdaWithFuncArgument_2()
    {
        Run(@"
            (func2 (x) (lambda (y) (1 x y)))
            (run (func2 2 3))");
        Assert.AreEqual("(1 2 3)", _resultString);
    }

    [Test]
    public void Carrying_1()
    {
        Run(@"
            (func (x y) (1 x y))
            (func2 () (func 2))
            (run (func2 3))");
        Assert.AreEqual("(1 2 3)", _resultString);
    }

    [Test]
    public void Carrying_2()
    {
        Run(@"
            (func (x y) (1 x y))
            (func2 (x) (x 3))
            (run (func2 (func 2)))");
        Assert.AreEqual("(1 2 3)", _resultString);
    }

    [Test]
    public void Carrying_EmbeddedFunction()
    {
        Run(@"
            (add () (+ 1))
            (run (add 2))");
        Assert.AreEqual("3", _resultString);
    }

    [Test]
    public void SameArgumentName()
    {
        Run(@"
            (func (x) x)
            (func2 (x) (func x))
            (run (func2 1))");
        Assert.AreEqual("1", _resultString);
    }

    [Test]
    public void If()
    {
        Run(@"
            (run (if true 1 2))");
        Assert.AreEqual("1", _resultString);
    }

    [Test]
    public void Add()
    {
        Run(@"
            (run (+ 1 2))");
        Assert.AreEqual("3", _resultString);
    }

    [Test]
    public void Sub()
    {
        Run(@"
            (run (- 1 2))");
        Assert.AreEqual("-1", _resultString);
    }

    [Test]
    public void Mul()
    {
        Run(@"
            (run (* 3 2))");
        Assert.AreEqual("6", _resultString);
    }

    [Test]
    public void Div()
    {
        Run(@"
            (run (/ 8 2))");
        Assert.AreEqual("4", _resultString);
    }

    [Test]
    public void Remainder()
    {
        Run(@"
            (run (% 8 3))");
        Assert.AreEqual("2", _resultString);
    }

    [Test]
    public void SetVariable()
    {
        Run(@"
            (set_get_var () (set @var 123) @var)
            (run (set_get_var))");
        Assert.AreEqual("123", _resultString);
    }

    [Test]
    public void SetVariableDifferentFiles()
    {
        Run(new[]
        {
            // main
            ("main.luna", @"
            import 'imported.luna'
            (get_result ()
                (set @var 456)
                ((get_var) @var)
            )
            (run (get_result))"),
            // imported
            ("imported.luna", @"
            (get_var ()
                (set @var 123)
                @var
            )")
        });
        Assert.AreEqual("(123 456)", _resultString);
    }

    [Test]
    public void EqVariableTrue()
    {
        Run(@"
            (eq_var ()
                (set @var true)
                (eq @var true))
            (run (eq_var))");
        Assert.AreEqual("true", _resultString);
    }

    [Test]
    public void InnerEmbeddedFunctions()
    {
        Run(@"
            (run (+ (+ 1 2) (+ 3 4)))");
        Assert.AreEqual("10", _resultString);
    }

    [Test]
    public void ListsEqual()
    {
        Run(@"
            (run (eq (1 2 3) (1.0 2.0 3.0)))");
        Assert.AreEqual("true", _resultString);
    }

    [Test]
    public void ListsNotEqual()
    {
        Run(@"
            (run (eq (1 2 3) (3 2 1)))");
        Assert.AreEqual("false", _resultString);
    }

    [Test]
    public void CodeModelBuildError_Stopped()
    {
        Run(@"
            (run wrong lexems)");
        Assert.Null(_resultString);
        _outputWriter.Verify(x => x.ProgramStopped(), Times.Once());
    }

    [Test]
    public void Stackoverflow_1()
    {
        Run(@"
            (recursive () (recursive))
            (run (recursive))");
        Assert.AreEqual("void", _resultString);
        _runtimeExceptionHandler.Verify(x => x.Handle(RuntimeException.Stackoverflow()), Times.Once());
    }

    [Test]
    public void Stackoverflow_2()
    {
        Run(@"
            (recursive () (recursive))
            (run (print (recursive)))");
        Assert.AreEqual("void", _resultString);
        _runtimeExceptionHandler.Verify(x => x.Handle(RuntimeException.Stackoverflow()), Times.Once());
    }
}
