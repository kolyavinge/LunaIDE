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
        var codeFile = new CodeFileProjectItem("main.luna", _project.Root, _fileSystem.Object);
        _fileSystem.Setup(x => x.ReadFileText("main.luna")).Returns(codeFileContent);
        _project.AddItem(_project.Root, codeFile);
    }

    protected void CodeFiles((string, string)[] codeFileContents)
    {
        foreach (var codeFileContentTuple in codeFileContents)
        {
            var codeFile = new CodeFileProjectItem(codeFileContentTuple.Item1, _project.Root, _fileSystem.Object);
            _fileSystem.Setup(x => x.ReadFileText(codeFileContentTuple.Item1)).Returns(codeFileContentTuple.Item2);
            _project.AddItem(_project.Root, codeFile);
        }
    }
}
