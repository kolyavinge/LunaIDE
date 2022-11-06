using Luna.Infrastructure;
using Luna.Output;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;

namespace Luna.Tests.Tools;

internal class BaseInterpreterTest
{
    protected Mock<IFileSystem> _fileSystem;
    protected Mock<IRuntimeOutput> _runtimeOutput;
    protected Project _project;
    protected Interpreter _interpreter;
    protected IRuntimeValue _result;
    protected string _resultString;

    protected void Init()
    {
        _fileSystem = new Mock<IFileSystem>();
        _runtimeOutput = new Mock<IRuntimeOutput>();
        _project = new Project("", _fileSystem.Object);
        _interpreter = new Interpreter();
    }

    protected void Run()
    {
        _interpreter.Run(_project, _runtimeOutput.Object);
        _result = _interpreter.Result;
        _resultString = _result?.ToString();
    }

    protected void CodeFile(string codeFileContent)
    {
        var codeFile = new CodeFileProjectItem("main", null, _fileSystem.Object);
        _fileSystem.Setup(x => x.ReadFileText("main")).Returns(codeFileContent);
        _project.AddItem(_project.Root, codeFile);
    }
}
