using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing;

public class ParserMessage
{
    public ParserMessageType Type { get; }
    public Token Token { get; }
    public List<Token> Tokens { get; }

    public ParserMessage(ParserMessageType type, in Token token)
    {
        Type = type;
        Token = token;
        Tokens = new List<Token> { token };
    }

    public ParserMessage(ParserMessageType type, IEnumerable<Token> tokens)
    {
        Type = type;
        Tokens = tokens.ToList();
        Token = Tokens.First();
    }
}

public enum ParserMessageType
{
    IncorrectToken,
    UnexpectedToken,
    UnknownIdentificator,
    IncorrectTokenAfterImport,
    ImportEmptyFilePath,
    ImportNoFilePath,
    ImportFilePathNotString,
    ImportFileNotFound,
    UnexpectedImport,
    EmptyConstDeclaration,
    IncorrectConstName,
    ConstNameExist,
    ConstNoValue,
    ConstIncorrectValue,
    IncorrectFunctionName,
    FunctionNameExist,
    RunFunctionExist,
    IncorrectFunctionCall,
    UnexpectedFunctionEnd,
    IncorrectFunctionAgrumentsDeclaration,
    IncorrectFunctionAgrument,
    IncorrectFunctionBody,
    IntegerValueOverflow,
    FloatValueOverflow
}
