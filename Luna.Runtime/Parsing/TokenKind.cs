using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.Parsing
{
    public enum TokenKind : byte
    {
        ImportDirective,
        ConstDeclare,
        OpenBracket,
        CloseBracket,
        Operator,
        String,
        Identificator,
        LambdaIdentificator,
        RunFunction,
        IntegerNumber,
        FloatNumber,
        Comment,
        Dot
    }
}
