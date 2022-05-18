using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing;

public class FunctionParser : AbstractParser
{
    private readonly IFunctionParserScope _scope;

    public FunctionParser(Text text, TokenIterator iter, CodeModel codeModel, IFunctionParserScope scope)
       : base(text, iter, codeModel)
    {
        _scope = scope;
    }

    protected override void InnerParse()
    {
        switch (State.Begin)
        {
            case State.Begin:
                if (Eof) break;
                else if (Token.Kind == TokenKind.ConstDeclaration) { ParseConstDeclaration(); goto case State.Begin; }
                else if (Token.Kind == TokenKind.ImportDirective) { _result.SetError(ParserMessageType.UnexpectedImport, Token); break; }
                else if (Token.Kind == TokenKind.OpenBracket) { ParseRunFunctionOrFunctionDeclaration(); goto case State.Begin; }
                else goto case State.Error;
            case State.Error:
                if (_result.Error == null) _result.SetError(ParserMessageType.UnexpectedToken, Token);
                break;
        }
    }

    private void ParseConstDeclaration()
    {
        var constToken = Token;
        MoveNext();
        if (Eof || constToken.LineIndex != Token.LineIndex)
        {
            _result.SetError(ParserMessageType.EmptyConstDeclaration, constToken);
            return;
        }
        else if (Token.Kind != TokenKind.Identificator)
        {
            _result.SetError(ParserMessageType.IncorrectConstName, Token);
            return;
        }
        var constNameToken = Token;
        var constName = GetTokenName();
        if (_scope.IsFunctionExist(constName))
        {
            _result.SetError(ParserMessageType.FunctionNameExist, Token);
            return;
        }
        if (_scope.IsConstExist(constName))
        {
            _result.SetError(ParserMessageType.ConstNameExist, Token);
            return;
        }
        MoveNext();
        ValueElement? constValue = null;
        ParseConstValue(in constToken, ref constValue);
        MoveNext();
        var unexpectedTokens = GetRemainingTokens(constToken.LineIndex);
        if (unexpectedTokens.Any())
        {
            _result.SetError(ParserMessageType.UnexpectedToken, unexpectedTokens);
        }
        else if (constValue != null)
        {
            _codeModel.AddConstDeclaration(new ConstDeclaration(constName, constValue, constNameToken.LineIndex, constNameToken.StartColumnIndex));
        }
    }

    private void ParseConstValue(in Token constToken, ref ValueElement? constValue)
    {
        if (Eof || constToken.LineIndex != Token.LineIndex)
        {
            _result.SetError(ParserMessageType.ConstNoValue, constToken);
        }
        else if (Token.Kind == TokenKind.IntegerNumber)
        {
            constValue = new IntegerValue(GetIntegerValue(), Token.LineIndex, Token.StartColumnIndex);
        }
        else if (Token.Kind == TokenKind.FloatNumber)
        {
            constValue = new FloatValueElement(GetDoubleValue(), Token.LineIndex, Token.StartColumnIndex);
        }
        else if (Token.Kind == TokenKind.String)
        {
            constValue = new StringValueElement(GetTokenName(), Token.LineIndex, Token.StartColumnIndex);
        }
        else
        {
            _result.SetError(ParserMessageType.ConstIncorrectValue, Token);
        }
    }

    private void ParseRunFunctionOrFunctionDeclaration()
    {
        MoveNext();
        if (Token.Kind == TokenKind.Identificator)
        {
            ParseFunctionDeclaration();
        }
        else if (Token.Kind == TokenKind.RunFunction)
        {
            ParseRunFunctionCall();
        }
        else
        {
            _result.SetError(ParserMessageType.IncorrectFunctionName, Token);
        }
    }

    private void ParseFunctionDeclaration()
    {
        var funcToken = Token;
        var funcName = GetTokenName();
        if (_scope.IsFunctionExist(funcName))
        {
            _result.SetError(ParserMessageType.FunctionNameExist, Token);
            return;
        }
        if (_scope.IsConstExist(funcName))
        {
            _result.SetError(ParserMessageType.ConstNameExist, Token);
            return;
        }
        MoveNext();
        (var arguments, var body) = ParseFunctionArgumentsAndBody();
        if (arguments != null && body != null)
        {
            _codeModel.AddFunctionDeclaration(new FunctionDeclaration(funcName, arguments, body, funcToken.LineIndex, funcToken.StartColumnIndex));
        }
    }

    private (List<FunctionArgument>?, FunctionBody?) ParseFunctionArgumentsAndBody()
    {
        var arguments = ParseFunctionArguments();
        if (arguments == null) return (null, null);

        var body = ParseFunctionBody();
        if (body == null) return (null, null);

        return (arguments, body);
    }

    private List<FunctionArgument>? ParseFunctionArguments()
    {
        if (Token.Kind != TokenKind.OpenBracket)
        {
            _result.SetError(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, Token);
            return null;
        }
        MoveNext();
        var arguments = new List<FunctionArgument>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            if (Token.Kind != TokenKind.Identificator)
            {
                _result.SetError(ParserMessageType.IncorrectFunctionAgrument, Token);
                return null;
            }
            var argName = GetTokenName();
            arguments.Add(new FunctionArgument(argName, Token.LineIndex, Token.StartColumnIndex));
            MoveNext();
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            _result.SetError(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, Token);
            return null;
        }
        MoveNext();

        return arguments;
    }

    private FunctionBody? ParseFunctionBody()
    {
        var body = new FunctionBody();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            var item = ParseFunctionBodyItem();
            if (item == null)
            {
                _result.SetError(ParserMessageType.IncorrectFunctionBody, Token);
                return null;
            }
            body.Add(item);
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            _result.SetError(ParserMessageType.UnexpectedFunctionEnd, Prev);
            return null;
        }
        MoveNext();

        return body;
    }

    private ValueElement? ParseFunctionBodyItem()
    {
        ValueElement? body = null;
        if (Token.Kind == TokenKind.IntegerNumber)
        {
            body = new IntegerValue(GetIntegerValue(), Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.FloatNumber)
        {
            body = new FloatValueElement(GetDoubleValue(), Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.String)
        {
            body = new StringValueElement(GetTokenName(), Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.Identificator)
        {
            var name = GetTokenName();
            if (!_scope.IsConstExist(name) && !_scope.IsFunctionExist(name))
            {
                body = new VariableValueElement(name, Token.LineIndex, Token.StartColumnIndex);
            }
            else if (_scope.IsConstExist(name))
            {
                body = new NamedConstantValueElement(name, Token.LineIndex, Token.StartColumnIndex);
            }
            else if (_scope.IsFunctionExist(name))
            {
                body = new FunctionValueElement(name, new List<ValueElement>(), Token.LineIndex, Token.StartColumnIndex);
            }
            else
            {
                _result.SetError(ParserMessageType.UnknownIdentificator, Token);
            }
            MoveNext();
        }
        else if (Token.Kind == TokenKind.OpenBracket)
        {
            MoveNext();
            var funcName = GetTokenName();
            if (Token.Kind == TokenKind.Identificator && _scope.IsFunctionExist(funcName))
            {
                body = ParseFunctionCall(funcName);
            }
            else if (Token.Kind == TokenKind.Lambda)
            {
                body = ParseLambda();
            }
            else
            {
                body = ParseList();
            }
        }
        else
        {
            _result.SetError(ParserMessageType.IncorrectToken, Token);
        }

        return body;
    }

    private FunctionValueElement? ParseFunctionCall(string funcName)
    {
        MoveNext();
        var argumentValues = new List<ValueElement>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            var value = ParseFunctionBodyItem();
            if (value == null)
            {
                _result.SetError(ParserMessageType.UnexpectedToken, Token);
                return null;
            }
            argumentValues.Add(value);
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            _result.SetError(ParserMessageType.UnexpectedToken, Token);
            return null;
        }
        MoveNext();

        return new FunctionValueElement(funcName, argumentValues, Token.LineIndex, Token.StartColumnIndex);
    }

    private void ParseRunFunctionCall()
    {
        if (_scope.IsRunFunctionExist())
        {
            _result.SetError(ParserMessageType.RunFunctionExist, Token);
            return;
        }
        MoveNext();
        var body = ParseFunctionBodyItem() as FunctionValueElement;
        if (body == null)
        {
            _result.SetError(ParserMessageType.IncorrectFunctionCall, Token);
            return;
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            _result.SetError(ParserMessageType.UnexpectedToken, Token);
            return;
        }
        MoveNext();
        _codeModel.RunFunction = body;
    }

    private LambdaValueElement? ParseLambda()
    {
        var lambdaToken = Token;
        MoveNext();
        (var lambdaArguments, var lambdaBody) = ParseFunctionArgumentsAndBody();
        if (lambdaArguments != null && lambdaBody != null)
        {
            return new LambdaValueElement(lambdaArguments, lambdaBody, lambdaToken.LineIndex, lambdaToken.StartColumnIndex);
        }
        else
        {
            return null;
        }
    }

    private ListValueElement? ParseList()
    {
        var listToken = Prev;
        var items = new List<ValueElement>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            var item = ParseFunctionBodyItem();
            if (item == null) return null;
            items.Add(item);
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            _result.SetError(ParserMessageType.IncorrectToken, Token);
            return null;
        }
        MoveNext();

        return new ListValueElement(items, listToken.LineIndex, listToken.StartColumnIndex);
    }

    enum State
    {
        Begin,
        Error
    }
}
