using System;
using System.Collections.Generic;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.Output;

internal interface IOutputWriter
{
    void SuccessfullyParsed(CodeFileProjectItem codeFile);
    void WriteWarning(CodeFileProjectItem codeFile, ParserMessage message);
    void WriteError(CodeFileProjectItem codeFile, ParserMessage message);
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
        _output.NewMessage(new OutputMessage(new OutputMessageItem[]
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
        _output.NewMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("Warning in file ", OutputMessageKind.Warning),
            new(codeFile.Name, OutputMessageKind.Info),
            new(String.Format(". Line {0}, col {1}. {2}.", token.LineIndex + 1, token.StartColumnIndex + 1, _textMessage[message.Type]), OutputMessageKind.Warning)
        })
        { ProjectItem = codeFile });
    }

    public void WriteError(CodeFileProjectItem codeFile, ParserMessage message)
    {
        var token = message.Token;
        _output.NewMessage(new OutputMessage(new OutputMessageItem[]
        {
            new("Error in file ", OutputMessageKind.Error),
            new(codeFile.Name, OutputMessageKind.Info),
            new(String.Format(". Line {0}, col {1}. {2}.", token.LineIndex + 1, token.StartColumnIndex + 1, _textMessage[message.Type]), OutputMessageKind.Error)
        })
        { ProjectItem = codeFile });
    }

    public void ProgramStopped()
    {
        _output.NewMessage(new OutputMessage(new OutputMessageItem[]
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
        { ParserMessageType.IncorrectFunctionBody, "Incorrect function body" }
    };
}
