using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing;

public class TokenIterator
{
    public class AdditionalSettings
    {
        public bool SkipComments { get; set; } = true;
    }

    private int _index;
    private readonly List<Token> _tokens;

    public Token Token { get; private set; }

    public Token PrevToken { get; private set; }

    public bool Eof => _index == _tokens.Count;

    public bool SkipComments { get; }

    public TokenIterator(IEnumerable<Token> tokens, AdditionalSettings? additionalSettings = null)
    {
        Token = PrevToken = Token.Default;
        _tokens = tokens.ToList();
        _index = -1;
        SkipComments = additionalSettings?.SkipComments ?? true;
        MoveNext();
    }

    public void MoveNext()
    {
        if (!Eof)
        {
            PrevToken = Token;
            _index++;
            if (SkipComments)
            {
                while (_index < _tokens.Count && _tokens[_index].Kind == TokenKind.Comment) _index++;
            }
            if (!Eof)
            {
                Token = _tokens[_index];
            }
            else
            {
                PrevToken = Token;
                Token = Token.Default;
            }
        }
    }
}
