using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing
{
    public class FunctionParser : AbstractParser
    {
        private readonly IScope _scope;

        public FunctionParser(Text text, TokenIterator iter, CodeModel codeModel, IScope scope)
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
                    else if (_token.Kind == TokenKind.ConstDeclaration) { ParseConstDeclaration(); goto case State.Begin; }
                    else if (_token.Kind == TokenKind.ImportDirective) { _result.SetError(ParserMessageType.UnexpectedImport, _token); break; }
                    else if (_token.Kind == TokenKind.OpenBracket) { ParseRunFunctionOrFunctionDeclaration(); goto case State.Begin; }
                    else goto case State.Error;
                case State.Error:
                    if (_result.Error == null) _result.SetError(ParserMessageType.UnexpectedToken, _token);
                    break;
            }
        }

        private void ParseConstDeclaration()
        {
            var constToken = _token;
            MoveNext();
            if (Eof || constToken.LineIndex != _token.LineIndex)
            {
                _result.SetError(ParserMessageType.ConstEmptyDeclaration, constToken);
                return;
            }
            else if (_token.Kind != TokenKind.Identificator)
            {
                _result.SetError(ParserMessageType.ConstIncorrectName, _token);
                return;
            }
            var constName = GetTokenName();
            if (_scope.IsFunctionExist(constName))
            {
                _result.SetError(ParserMessageType.FunctionNameExist, _token);
                return;
            }
            if (_scope.IsConstExist(constName))
            {
                _result.SetError(ParserMessageType.ConstNameExist, _token);
                return;
            }
            MoveNext();
            Value? constValue = null;
            ParseConstValue(in constToken, ref constValue);
            MoveNext();
            var unexpectedTokens = GetRemainingTokens(constToken.LineIndex);
            if (unexpectedTokens.Any())
            {
                _result.SetError(ParserMessageType.UnexpectedToken, unexpectedTokens);
            }
            else if (constValue != null)
            {
                _codeModel.Constants.Add(new ConstDirective(constName, constValue, constToken.LineIndex, constToken.StartColumnIndex));
            }
        }

        private void ParseConstValue(in Token constToken, ref Value? constValue)
        {
            if (Eof || constToken.LineIndex != _token.LineIndex)
            {
                _result.SetError(ParserMessageType.ConstNoValue, constToken);
            }
            else if (_token.Kind == TokenKind.IntegerNumber)
            {
                constValue = new IntegerValue(GetIntegerValue(), _token.LineIndex, _token.StartColumnIndex);
            }
            else if (_token.Kind == TokenKind.FloatNumber)
            {
                constValue = new FloatValue(GetDoubleValue(), _token.LineIndex, _token.StartColumnIndex);
            }
            else if (_token.Kind == TokenKind.String)
            {
                constValue = new StringValue(GetTokenName(), _token.LineIndex, _token.StartColumnIndex);
            }
            else
            {
                _result.SetError(ParserMessageType.ConstIncorrectValue, _token);
            }
        }

        private void ParseRunFunctionOrFunctionDeclaration()
        {
            MoveNext();
            if (_token.Kind == TokenKind.Identificator)
            {
                ParseFunctionDeclaration();
            }
            else if (_token.Kind == TokenKind.RunFunction)
            {
                ParseRunFunctionCall();
            }
            else
            {
                _result.SetError(ParserMessageType.IncorrectFunctionName, _token);
            }
        }

        private void ParseFunctionDeclaration()
        {
            var funcToken = _token;
            var funcName = GetTokenName();
            if (_scope.IsFunctionExist(funcName))
            {
                _result.SetError(ParserMessageType.FunctionNameExist, _token);
                return;
            }
            if (_scope.IsConstExist(funcName))
            {
                _result.SetError(ParserMessageType.ConstNameExist, _token);
                return;
            }
            MoveNext();
            (var arguments, var body) = ParseFunctionArgumentsAndBody();
            if (arguments != null && body != null)
            {
                _codeModel.Functions.Add(new FunctionDeclaration(funcName, arguments, body, funcToken.LineIndex, funcToken.StartColumnIndex));
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
            if (_token.Kind != TokenKind.OpenBracket)
            {
                _result.SetError(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, _token);
                return null;
            }
            MoveNext();
            var arguments = new List<FunctionArgument>();
            while (!Eof && _token.Kind != TokenKind.CloseBracket)
            {
                if (_token.Kind != TokenKind.Identificator)
                {
                    _result.SetError(ParserMessageType.IncorrectFunctionAgrument, _token);
                    return null;
                }
                var argName = GetTokenName();
                arguments.Add(new FunctionArgument(argName, _token.LineIndex, _token.StartColumnIndex));
                MoveNext();
            }
            if (_token.Kind != TokenKind.CloseBracket)
            {
                _result.SetError(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, _token);
                return null;
            }
            MoveNext();

            return arguments;
        }

        private FunctionBody? ParseFunctionBody()
        {
            var body = new FunctionBody();
            while (!Eof && _token.Kind != TokenKind.CloseBracket)
            {
                var item = ParseFunctionBodyItem();
                if (item == null)
                {
                    _result.SetError(ParserMessageType.IncorrectFunctionBody, _token);
                    return null;
                }
                body.Add(item);
            }
            if (_token.Kind != TokenKind.CloseBracket)
            {
                _result.SetError(ParserMessageType.UnexpectedFunctionEnd, _prev);
                return null;
            }
            MoveNext();

            return body;
        }

        private Value? ParseFunctionBodyItem()
        {
            Value? body = null;
            if (_token.Kind == TokenKind.IntegerNumber)
            {
                body = new IntegerValue(GetIntegerValue(), _token.LineIndex, _token.StartColumnIndex);
                MoveNext();
            }
            else if (_token.Kind == TokenKind.FloatNumber)
            {
                body = new FloatValue(GetDoubleValue(), _token.LineIndex, _token.StartColumnIndex);
                MoveNext();
            }
            else if (_token.Kind == TokenKind.String)
            {
                body = new StringValue(GetTokenName(), _token.LineIndex, _token.StartColumnIndex);
                MoveNext();
            }
            else if (_token.Kind == TokenKind.Identificator)
            {
                var name = GetTokenName();
                if (!_scope.IsConstExist(name) && !_scope.IsFunctionExist(name))
                {
                    body = new Variable(name, _token.LineIndex, _token.StartColumnIndex);
                }
                else if (_scope.IsConstExist(name))
                {
                    body = new NamedConstant(name, _token.LineIndex, _token.StartColumnIndex);
                }
                else if (_scope.IsFunctionExist(name))
                {
                    body = new Function(name, new List<Value>(), _token.LineIndex, _token.StartColumnIndex);
                }
                else
                {
                    _result.SetError(ParserMessageType.UnknownIdentificator, _token);
                }
                MoveNext();
            }
            else if (_token.Kind == TokenKind.OpenBracket)
            {
                MoveNext();
                var funcName = GetTokenName();
                if (_token.Kind == TokenKind.Identificator && _scope.IsFunctionExist(funcName))
                {
                    body = ParseFunctionCall(funcName);
                }
                else if (_token.Kind == TokenKind.Lambda)
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
                _result.SetError(ParserMessageType.IncorrectToken, _token);
            }

            return body;
        }

        private Function? ParseFunctionCall(string funcName)
        {
            MoveNext();
            var argumentValues = new List<Value>();
            while (!Eof && _token.Kind != TokenKind.CloseBracket)
            {
                var value = ParseFunctionBodyItem();
                if (value == null)
                {
                    _result.SetError(ParserMessageType.UnexpectedToken, _token);
                    return null;
                }
                argumentValues.Add(value);
            }
            if (_token.Kind != TokenKind.CloseBracket)
            {
                _result.SetError(ParserMessageType.UnexpectedToken, _token);
                return null;
            }
            MoveNext();

            return new Function(funcName, argumentValues, _token.LineIndex, _token.StartColumnIndex);
        }

        private void ParseRunFunctionCall()
        {
            if (_scope.IsRunFunctionExist())
            {
                _result.SetError(ParserMessageType.RunFunctionExist, _token);
                return;
            }
            MoveNext();
            var body = ParseFunctionBodyItem() as Function;
            if (body == null)
            {
                _result.SetError(ParserMessageType.IncorrectFunctionCall, _token);
                return;
            }
            if (_token.Kind != TokenKind.CloseBracket)
            {
                _result.SetError(ParserMessageType.UnexpectedToken, _token);
                return;
            }
            MoveNext();
            _codeModel.RunFunction = body;
        }

        private Lambda? ParseLambda()
        {
            var lambdaToken = _token;
            MoveNext();
            (var lambdaArguments, var lambdaBody) = ParseFunctionArgumentsAndBody();
            if (lambdaArguments != null && lambdaBody != null)
            {
                return new Lambda(lambdaArguments, lambdaBody, lambdaToken.LineIndex, lambdaToken.StartColumnIndex);
            }
            else
            {
                return null;
            }
        }

        private ListValue? ParseList()
        {
            var listToken = _token;
            var items = new List<Value>();
            while (!Eof && _token.Kind != TokenKind.CloseBracket)
            {
                var item = ParseFunctionBodyItem();
                if (item == null) return null;
                items.Add(item);
            }
            if (_token.Kind != TokenKind.CloseBracket)
            {
                _result.SetError(ParserMessageType.IncorrectToken, _token);
                return null;
            }
            MoveNext();

            return new ListValue(items, listToken.LineIndex, listToken.StartColumnIndex);
        }

        enum State
        {
            Begin,
            Error
        }
    }
}
