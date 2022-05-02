using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing
{
    class TokenIterator
    {
        private int _index;
        private readonly List<Token> _tokens;

        public Token Token => _tokens[_index];

        public bool Eof => _index < _tokens.Count;

        public TokenIterator(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();
            _index = 0;
            MoveNext();
        }

        public void MoveNext()
        {
            _index++;
            while (_index < _tokens.Count && Token.Kind == TokenKind.Comment) _index++;
        }
    }
}
