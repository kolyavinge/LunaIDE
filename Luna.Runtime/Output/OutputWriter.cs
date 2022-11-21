using System.Collections.Generic;
using Luna.Parsing;
using Luna.ProjectModel;
using Luna.Runtime;

namespace Luna.Output;

internal interface IOutputWriter
{
    void SuccessfullyParsed(CodeFileProjectItem codeFile);
    void WriteWarning(CodeFileProjectItem codeFile, ParserMessage message);
    void WriteError(CodeFileProjectItem codeFile, ParserMessage message);
    void WriteError(string errorMessage);
    void WriteCallStack(CallStack callStack);
    void ProgramResult(IRuntimeValue runtimeValue);
    void ProgramStopped();
}

internal class OutputWriter : IOutputWriter
{
    private readonly IRuntimeOutput _output;

    public OutputWriter(IRuntimeOutput output)
    {
        _output = output;
    }

    public void SuccessfullyParsed(CodeFileProjectItem codeFile)
    {
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("File ", OutputMessageKind.Text),
            new(codeFile.Name, OutputMessageKind.Info),
            new(" is successfully parsed.", OutputMessageKind.Text)
        })
        { ProjectItem = codeFile });
    }

    public void WriteWarning(CodeFileProjectItem codeFile, ParserMessage message)
    {
        var token = message.Token;
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("Warning in file ", OutputMessageKind.Warning),
            new(codeFile.Name, OutputMessageKind.Info),
            new($". Line {token.LineIndex + 1}, col {token.StartColumnIndex + 1}. {_textMessage[message.Type]}.", OutputMessageKind.Warning)
        })
        { ProjectItem = codeFile });
    }

    public void WriteError(CodeFileProjectItem codeFile, ParserMessage message)
    {
        var token = message.Token;
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("Error in file ", OutputMessageKind.Error),
            new(codeFile.Name, OutputMessageKind.Info),
            new($". Line {token.LineIndex + 1}, col {token.StartColumnIndex + 1}. {_textMessage[message.Type]}.", OutputMessageKind.Error)
        })
        { ProjectItem = codeFile });
    }

    public void WriteError(string errorMessage)
    {
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new(errorMessage, OutputMessageKind.Error)
        }));
    }

    public void WriteCallStack(CallStack callStack)
    {
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("Stack trace:", OutputMessageKind.Text)
        }));

        foreach (var item in callStack)
        {
            _output.SendMessage(new OutputMessage(new OutputMessageItem[]
            {
                new(item.Name, OutputMessageKind.Text)
            }));
        }
    }

    public void ProgramResult(IRuntimeValue runtimeValue)
    {
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("Program result: ", OutputMessageKind.Info),
            new(runtimeValue.ToString() ?? "", OutputMessageKind.Text)
        }));
    }

    public void ProgramStopped()
    {
        _output.SendMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("The program cannot be run.", OutputMessageKind.Error)
        }));
    }

    private static readonly Dictionary<ParserMessageType, string> _textMessage = new()
    {
        { ParserMessageType.IncorrectToken, "Incorrect token" },
        { ParserMessageType.UnexpectedToken, "Unexpected token" },
        { ParserMessageType.UnknownIdentificator, "Unknown identificator" },
        { ParserMessageType.IncorrectTokenAfterImport, "Incorrect token after import directive" },
        { ParserMessageType.ImportEmptyFilePath, "Empty import file path" },
        { ParserMessageType.ImportNoFilePath, "Import has no file path" },
        { ParserMessageType.ImportFilePathNotString, "Import file path is not string" },
        { ParserMessageType.ImportFileNotFound, "Import file not found" },
        { ParserMessageType.UnexpectedImport, "Unexpected import directive" },
        { ParserMessageType.DuplicateImport, "Duplicate import directive" },
        { ParserMessageType.EmptyConstDeclaration, "Empty constant declaration" },
        { ParserMessageType.IncorrectConstName, "Incorrect constant name" },
        { ParserMessageType.ConstNameExist, "Constant name is already exist" },
        { ParserMessageType.ConstNoValue, "Constant has no value" },
        { ParserMessageType.ConstIncorrectValue, "Constant has an incorrect value" },
        { ParserMessageType.IncorrectFunctionName, "Incorrect function name" },
        { ParserMessageType.FunctionNameExist, "Function name is already exist" },
        { ParserMessageType.RunFunctionExist, "Run function is already exist" },
        { ParserMessageType.IncorrectFunctionCall, "Incorrect function call" },
        { ParserMessageType.UnexpectedFunctionEnd, "Unexpected function end" },
        { ParserMessageType.IncorrectFunctionAgrumentsDeclaration, "Incorrect function agruments declaration" },
        { ParserMessageType.IncorrectFunctionAgrument, "Incorrect function agrument" },
        { ParserMessageType.IncorrectFunctionBody, "Incorrect function body" },
        { ParserMessageType.IntegerValueOverflow, "Integer value is too big" },
        { ParserMessageType.FloatValueOverflow, "Float value is too big" }
    };
}
