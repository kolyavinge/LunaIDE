using System;
using System.Collections.Generic;
using System.Globalization;

namespace Luna.Parsing
{
    public class ParseResult
    {
        public ParserMessage? Error { get; private set; }
        public List<ParserMessage> Warnings { get; } = new();

        public void SetError(ParserMessageType type, in Token token)
        {
            Error = new ParserMessage(type, in token);
        }

        public void SetError(ParserMessageType type, IEnumerable<Token> tokens)
        {
            Error = new ParserMessage(type, tokens);
        }
    }

    public abstract class AbstractParser
    {
        protected readonly CodeModel _codeModel;
        private readonly Text _text;
        private readonly TokenIterator _iter;
        protected Token _prev, _token;
        protected ParseResult _result;

        protected AbstractParser(Text text, TokenIterator iter, CodeModel codeModel)
        {
            _text = text;
            _iter = iter;
            _codeModel = codeModel;
            MoveNext();
        }

        public ParseResult Parse()
        {
            _result = new ParseResult();
            InnerParse();

            return _result;
        }

        protected abstract void InnerParse();

        protected List<Token> GetRemainingTokens(int lineIndex)
        {
            var result = new List<Token>();
            while (!_iter.Eof && lineIndex == _token.LineIndex)
            {
                result.Add(_token);
                MoveNext();
            }

            return result;
        }

        protected string GetTokenName()
        {
            if (_token.Kind == TokenKind.String)
            {
                return _text.GetLine(_token.LineIndex).Substring(_token.StartColumnIndex + 1, _token.Length - 2);
            }
            else
            {
                return _text.GetLine(_token.LineIndex).Substring(_token.StartColumnIndex, _token.Length);
            }
        }

        protected int GetIntegerValue()
        {
            var stringValue = GetTokenName();
            return Int32.Parse(stringValue);
        }

        protected double GetDoubleValue()
        {
            var stringValue = GetTokenName();
            return Double.Parse(stringValue, new NumberFormatInfo { NumberDecimalSeparator = "." });
        }

        protected bool Eof => _iter.Eof;

        protected void MoveNext()
        {
            _iter!.MoveNext();
            _prev = _iter.PrevToken;
            _token = _iter.Token;
        }
    }
}
