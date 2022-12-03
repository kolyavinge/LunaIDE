using Luna.Infrastructure;
using Luna.Output;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;

namespace Luna.Tests.Tools;

internal class BaseInterpreterTest
{
    protected Mock<IRuntimeOutput> _runtimeOutput;
    protected Mock<IOutputWriter> _outputWriter;
    protected ICodeModelBuilder _codeModelBuilder;
    protected Mock<IRuntimeExceptionHandler> _runtimeExceptionHandler;
    protected CallStack _callStack;
    protected IValueElementEvaluator _evaluator;
    protected IRuntimeValue _result;
    protected string _resultString;

    protected void Init()
    {
        _runtimeOutput = new Mock<IRuntimeOutput>();
        _outputWriter = new Mock<IOutputWriter>();
        _codeModelBuilder = new CodeModelBuilder(_outputWriter.Object);
        _runtimeExceptionHandler = new Mock<IRuntimeExceptionHandler>();
        _callStack = new CallStack();
        _evaluator = new ValueElementEvaluator();
        RuntimeEnvironment.StandartOutput = _runtimeOutput.Object;
    }

    protected void Run(string codeFileContent)
    {
        Run(new[] { ("main.luna", codeFileContent) });
    }

    protected void Run((string, string)[] codeFileContents)
    {
        var fileSystem = new Mock<IFileSystem>();
        var project = new Project("", fileSystem.Object);

        var interpreter = new Interpreter(
            project,
            _outputWriter.Object,
            _codeModelBuilder,
            _runtimeExceptionHandler.Object,
            _callStack,
            _evaluator);

        foreach (var codeFileContentTuple in codeFileContents)
        {
            var codeFile = new CodeFileProjectItem(codeFileContentTuple.Item1, project.Root, fileSystem.Object);
            fileSystem.Setup(x => x.ReadFileText(codeFileContentTuple.Item1)).Returns(codeFileContentTuple.Item2);
            project.AddItem(project.Root, codeFile);
        }

        interpreter.Run();
        _result = interpreter.Result;
        _resultString = _result?.ToString();
    }
}
