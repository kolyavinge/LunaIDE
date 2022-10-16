using System.Collections.Generic;
using Luna.Collections;

namespace Luna.CodeElements;

public class LambdaValueElement : ValueElement, IEquatable<LambdaValueElement?>
{
    public ReadonlyArray<FunctionArgument> Arguments { get; }
    public FunctionBody Body { get; }

    public LambdaValueElement(IEnumerable<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Arguments = new ReadonlyArray<FunctionArgument>(arguments);
        Body = body;
    }

    internal LambdaValueElement(IEnumerable<FunctionArgument> arguments, FunctionBody body) : this(arguments, body, 0, 0) { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LambdaValueElement);
    }

    public bool Equals(LambdaValueElement? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Arguments.Equals(other.Arguments) &&
               Body.Equals(other.Body);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Arguments, Body);
    }
}
