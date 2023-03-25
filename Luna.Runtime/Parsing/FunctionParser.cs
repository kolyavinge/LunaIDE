using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.ProjectModel;

namespace Luna.Parsing;

public class FunctionParser : AbstractParser
{
    private readonly IFunctionParserScope _scope;

    public FunctionParser(TokenIterator iter, CodeModel codeModel, IFunctionParserScope scope)
       : base(iter, codeModel)
    {
        _scope = scope;
    }

    protected override void InnerParse()
    {
        while (!Eof)
        {
            if (Token.Kind == TokenKind.ConstDeclaration)
            {
                ParserMessage? error = null;
                ParseConstDeclaration(ref error);
                if (error != null) _result.AddError(error);
            }
            else if (Token.Kind == TokenKind.ImportDirective)
            {
                _result.AddError(new(ParserMessageType.UnexpectedImport, Token));
                break;
            }
            else if (Token.Kind == TokenKind.OpenBracket)
            {
                ParserMessage? error = null;
                ParseRunFunctionOrFunctionDeclaration(ref error);
                if (error != null)
                {
                    _result.AddError(error);
                    SkipFunctionDeclaration();
                }
            }
            else
            {
                if (!_result.Errors.Any()) _result.AddError(new(ParserMessageType.UnexpectedToken, Token));
                MoveNext();
            }
        }
    }

    private void ParseConstDeclaration(ref ParserMessage? error)
    {
        var constToken = Token;
        MoveNext();
        if (Eof || constToken.LineIndex != Token.LineIndex)
        {
            error = new(ParserMessageType.EmptyConstDeclaration, constToken);
            return;
        }
        else if (Token.Kind != TokenKind.Identificator)
        {
            error = new(ParserMessageType.IncorrectConstName, Token);
            return;
        }
        var constNameToken = Token;
        var constName = GetTokenName();
        if (_scope.IsFunctionExist(constName))
        {
            error = new(ParserMessageType.FunctionNameExist, Token);
            return;
        }
        if (_scope.IsConstantExist(constName))
        {
            error = new(ParserMessageType.ConstNameExist, Token);
            return;
        }
        MoveNext();
        if (Eof || constToken.LineIndex != Token.LineIndex)
        {
            _codeModel.AddConstantDeclaration(new(constName, new FakeValueElement(), constNameToken.LineIndex, constNameToken.StartColumnIndex));
            error = new(ParserMessageType.ConstNoValue, constToken);
            return;
        }
        ValueElement? constValue = null;
        ParseConstValue(ref constValue, ref error);
        MoveNext();
        var unexpectedTokens = GetRemainingTokens(constToken.LineIndex);
        if (unexpectedTokens.Any())
        {
            error = new(ParserMessageType.UnexpectedToken, unexpectedTokens);
        }
        else if (constValue != null)
        {
            _codeModel.AddConstantDeclaration(new(constName, constValue, constNameToken.LineIndex, constNameToken.StartColumnIndex));
        }
        else
        {
            _codeModel.AddConstantDeclaration(new(constName, new FakeValueElement(), constNameToken.LineIndex, constNameToken.StartColumnIndex));
        }
    }

    private void ParseConstValue(ref ValueElement? constValue, ref ParserMessage? error)
    {
        if (Token.Kind == TokenKind.IntegerNumber)
        {
            try
            {
                long value = ParseLongValue();
                constValue = new IntegerValueElement(value, Token.LineIndex, Token.StartColumnIndex);
            }
            catch (OverflowException)
            {
                error = new(ParserMessageType.IntegerValueOverflow, Token);
            }
        }
        else if (Token.Kind == TokenKind.FloatNumber)
        {
            double value = ParseDoubleValue();
            constValue = new FloatValueElement(value, Token.LineIndex, Token.StartColumnIndex);
        }
        else if (Token.Kind == TokenKind.String)
        {
            constValue = new StringValueElement(GetTokenName(), Token.LineIndex, Token.StartColumnIndex);
        }
        else if (Token.Kind == TokenKind.BooleanTrue)
        {
            constValue = new BooleanValueElement(true, Token.LineIndex, Token.StartColumnIndex);
        }
        else if (Token.Kind == TokenKind.BooleanFalse)
        {
            constValue = new BooleanValueElement(false, Token.LineIndex, Token.StartColumnIndex);
        }
        else
        {
            error = new(ParserMessageType.ConstIncorrectValue, Token);
        }
    }

    private void ParseRunFunctionOrFunctionDeclaration(ref ParserMessage? error)
    {
        MoveNext();
        if (Token.Kind == TokenKind.Identificator)
        {
            ParseFunctionDeclaration(ref error);
        }
        else if (Token.Kind == TokenKind.RunFunction)
        {
            ParseRunFunctionCall(ref error);
        }
        else
        {
            error = new(ParserMessageType.IncorrectFunctionName, Token);
        }
    }

    private void ParseFunctionDeclaration(ref ParserMessage? error)
    {
        var funcToken = Token;
        var funcName = GetTokenName();
        if (_scope.IsFunctionExist(funcName))
        {
            error = new(ParserMessageType.FunctionNameExist, Token);
            return;
        }
        if (_scope.IsConstantExist(funcName))
        {
            error = new(ParserMessageType.ConstNameExist, Token);
            return;
        }
        MoveNext();
        var (arguments, body) = ParseFunctionArgumentsAndBody(ref error);
        if (arguments != null && body != null)
        {
            _codeModel.AddFunctionDeclaration(new(_codeModel, funcName, arguments, body, funcToken.LineIndex, funcToken.StartColumnIndex));
        }
        else if (arguments != null)
        {
            _codeModel.AddFunctionDeclaration(new(_codeModel, funcName, arguments, new(), funcToken.LineIndex, funcToken.StartColumnIndex));
        }
    }

    private (List<FunctionArgument>?, FunctionBody?) ParseFunctionArgumentsAndBody(ref ParserMessage? error)
    {
        var arguments = ParseFunctionArguments(ref error);
        if (arguments == null) return (null, null);

        var body = ParseFunctionBody(ref error);
        if (body == null) return (arguments, null);

        return (arguments, body);
    }

    private List<FunctionArgument>? ParseFunctionArguments(ref ParserMessage? error)
    {
        if (Token.Kind != TokenKind.OpenBracket)
        {
            error = new(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, Token);
            return null;
        }
        MoveNext();
        var arguments = new List<FunctionArgument>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            if (Token.Kind != TokenKind.Identificator)
            {
                error = new(ParserMessageType.IncorrectFunctionAgrument, Token);
                return null;
            }
            var argName = GetTokenName();
            arguments.Add(new(argName, Token.LineIndex, Token.StartColumnIndex));
            MoveNext();
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            error = new(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, Token);
            return null;
        }
        MoveNext();

        return arguments;
    }

    private FunctionBody? ParseFunctionBody(ref ParserMessage? error)
    {
        var startToken = Token;
        var items = new List<ValueElement>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            var item = ParseFunctionBodyItem(ref error);
            if (item == null) return null;
            items.Add(item);
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            error = new(ParserMessageType.UnexpectedFunctionEnd, Prev);
            return null;
        }
        var endToken = Token;
        var body = new FunctionBody(startToken.LineIndex, startToken.StartColumnIndex, endToken.LineIndex, endToken.StartColumnIndex, items);
        MoveNext();

        return body;
    }

    private ValueElement? ParseFunctionBodyItem(ref ParserMessage? error)
    {
        ValueElement? body = null;
        if (Token.Kind == TokenKind.IntegerNumber)
        {
            try
            {
                long value = ParseLongValue();
                body = new IntegerValueElement(value, Token.LineIndex, Token.StartColumnIndex);
                MoveNext();
            }
            catch (OverflowException)
            {
                error = new(ParserMessageType.IntegerValueOverflow, Token);
            }
        }
        else if (Token.Kind == TokenKind.FloatNumber)
        {
            double value = ParseDoubleValue();
            body = new FloatValueElement(value, Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.String)
        {
            body = new StringValueElement(GetTokenName(), Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.BooleanTrue)
        {
            body = new BooleanValueElement(true, Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.BooleanFalse)
        {
            body = new BooleanValueElement(false, Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.Identificator)
        {
            var name = GetTokenName();
            if (_scope.IsConstantExist(name))
            {
                body = new NamedConstantValueElement(name, Token.LineIndex, Token.StartColumnIndex);
            }
            else if (_scope.IsFunctionExist(name))
            {
                body = new FunctionValueElement(name, new List<ValueElement>(), Token.LineIndex, Token.StartColumnIndex, Token.LineIndex, Token.StartColumnIndex);
            }
            else
            {
                body = new FunctionArgumentValueElement(name, Token.LineIndex, Token.StartColumnIndex);
            }
            MoveNext();
        }
        else if (Token.Kind is TokenKind.Plus or TokenKind.Minus or TokenKind.Asterisk or TokenKind.Slash or TokenKind.Percent)
        {
            var name = GetTokenName();
            body = new FunctionValueElement(name, new List<ValueElement>(), Token.LineIndex, Token.StartColumnIndex, Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.Variable)
        {
            var name = GetTokenName();
            body = new VariableValueElement(name, Token.LineIndex, Token.StartColumnIndex);
            MoveNext();
        }
        else if (Token.Kind == TokenKind.OpenBracket)
        {
            MoveNext();
            var name = GetTokenName();
            if (Token.Kind == TokenKind.Identificator && _scope.IsConstantExist(name))
            {
                body = ParseList(ref error);
            }
            else if (Token.Kind is TokenKind.Identificator or
                     TokenKind.Plus or TokenKind.Minus or TokenKind.Asterisk or TokenKind.Slash or TokenKind.Percent)
            {
                body = ParseFunctionCall(name, ref error);
            }
            else if (Token.Kind == TokenKind.Lambda)
            {
                body = ParseLambda(ref error);
            }
            else
            {
                body = ParseList(ref error);
            }
        }
        else
        {
            error = new(ParserMessageType.IncorrectToken, Token);
        }

        return body;
    }

    private FunctionValueElement? ParseFunctionCall(string funcName, ref ParserMessage? error)
    {
        var funcToken = Token;
        MoveNext();
        var argumentValues = new List<ValueElement>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            var value = ParseFunctionBodyItem(ref error);
            if (value == null) return null;
            argumentValues.Add(value);
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            error = new(ParserMessageType.UnexpectedToken, Token);
            return null;
        }
        var endToken = Token;
        MoveNext();

        return new FunctionValueElement(funcName, argumentValues, funcToken.LineIndex, funcToken.StartColumnIndex, endToken.LineIndex, endToken.StartColumnIndex);
    }

    private void ParseRunFunctionCall(ref ParserMessage? error)
    {
        if (_scope.IsRunFunctionExist())
        {
            error = new(ParserMessageType.RunFunctionExist, Token);
            return;
        }
        MoveNext();
        if (ParseFunctionBodyItem(ref error) is not FunctionValueElement body)
        {
            error = new(ParserMessageType.IncorrectFunctionCall, Token);
            return;
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            error = new(ParserMessageType.UnexpectedToken, Token);
            return;
        }
        MoveNext();
        _codeModel.RunFunction = body;
    }

    private LambdaValueElement? ParseLambda(ref ParserMessage? error)
    {
        var lambdaToken = Token;
        MoveNext();
        var (lambdaArguments, lambdaBody) = ParseFunctionArgumentsAndBody(ref error);
        if (lambdaArguments != null && lambdaBody != null)
        {
            return new(lambdaArguments, lambdaBody, lambdaToken.LineIndex, lambdaToken.StartColumnIndex);
        }
        else
        {
            return null;
        }
    }

    private ListValueElement? ParseList(ref ParserMessage? error)
    {
        var listToken = Prev;
        var items = new List<ValueElement>();
        while (!Eof && Token.Kind != TokenKind.CloseBracket)
        {
            var item = ParseFunctionBodyItem(ref error);
            if (item == null) return null;
            items.Add(item);
        }
        if (Token.Kind != TokenKind.CloseBracket)
        {
            error = new(ParserMessageType.IncorrectToken, Token);
            return null;
        }
        MoveNext();

        return new(items, listToken.LineIndex, listToken.StartColumnIndex);
    }
}
