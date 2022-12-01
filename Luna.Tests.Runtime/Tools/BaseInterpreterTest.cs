using Luna.Infrastructure;
using Luna.Output;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;

namespace Luna.Tests.Tools;

internal class BaseInterpreterTest
{
    protected IRuntimeValue _result;
    protected Mock<IRuntimeOutput> _runtimeOutput;
    protected string _resultString;

    protected void Run(string codeFileContent)
    {
        Run(new[] { ("main.luna", codeFileContent) });
    }

    protected void Run((string, string)[] codeFileContents)
    {
        var fileSystem = new Mock<IFileSystem>();
        _runtimeOutput = new Mock<IRuntimeOutput>();
        var project = new Project("", fileSystem.Object);
        var interpreter = new Interpreter();

        foreach (var codeFileContentTuple in codeFileContents)
        {
            var codeFile = new CodeFileProjectItem(codeFileContentTuple.Item1, project.Root, fileSystem.Object);
            fileSystem.Setup(x => x.ReadFileText(codeFileContentTuple.Item1)).Returns(codeFileContentTuple.Item2);
            project.AddItem(project.Root, codeFile);
        }

        interpreter.Run(project, _runtimeOutput.Object);
        _result = interpreter.Result;
        _resultString = _result?.ToString();
    }
}
