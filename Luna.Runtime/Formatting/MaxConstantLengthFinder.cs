using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Formatting;

internal class MaxConstantLengthFinder
{
    public class Result
    {
        public readonly int Length;
        public readonly bool IsSigned;

        public Result(int length, bool isSigned)
        {
            Length = length;
            IsSigned = isSigned;
        }
    }

    public Result FindMaxConstantLength(IReadOnlyCollection<Token> tokens)
    {
        var maxConstantLength = 0;
        var signedConstantValue = false;
        var iter = new TokenIterator(tokens);
        while (!iter.Eof)
        {
            if (iter.PrevToken.Kind == TokenKind.ConstDeclaration
                && iter.Token.Kind == TokenKind.Identificator
                && iter.PrevToken.LineIndex == iter.Token.LineIndex)
            {
                var constNameToken = iter.Token;
                iter.MoveNext();
                if (iter.Token.Kind is not TokenKind.IntegerNumber and not TokenKind.FloatNumber and not TokenKind.String)
                {
                    continue;
                }
                var valueToken = iter.Token;
                iter.MoveNext();
                if (iter.Eof
                    || iter.PrevToken.LineIndex == iter.Token.LineIndex && iter.Token.Kind == TokenKind.Comment
                    || iter.PrevToken.LineIndex != iter.Token.LineIndex)
                {
                    if (constNameToken.Name.Length > maxConstantLength)
                    {
                        maxConstantLength = constNameToken.Name.Length;
                        signedConstantValue = valueToken.Name[0] == '+' || valueToken.Name[0] == '-';
                    }
                }
            }
            else
            {
                iter.MoveNext();
            }
        }

        return new(maxConstantLength, signedConstantValue);
    }
}
