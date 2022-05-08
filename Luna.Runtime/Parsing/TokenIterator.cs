using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing
{
    public class TokenIterator
    {
        private int _index;
        private readonly List<Token> _tokens;

        public Token Token { get; private set; }

        public Token PrevToken { get; private set; }

        public bool Eof => _index == _tokens.Count;

        public TokenIterator(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _index = -1;
            MoveNext();
        }

        public void MoveNext()
        {
            if (!Eof)
            {
                PrevToken = Token;
                _index++;
                while (_index < _tokens.Count && _tokens[_index].Kind == TokenKind.Comment) _index++;
                if (!Eof)
                {
                    Token = _tokens[_index];
                }
                else
                {
                    PrevToken = Token;
                    Token = default;
                }
            }
        }

        public void SkipTokensInLine()
        {
            var lineIndex = Token.LineIndex;
            MoveNext();
            while (!Eof && lineIndex == Token.LineIndex) MoveNext();
        }
    }
}
