namespace Luna.CodeElements;

public abstract class CodeElement : IEquatable<CodeElement?>
{
    public int LineIndex { get; }
    public int ColumnIndex { get; }

    protected CodeElement(int lineIndex, int columnIndex)
    {
        LineIndex = lineIndex;
        ColumnIndex = columnIndex;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CodeElement);
    }

    public bool Equals(CodeElement? other)
    {
        return other is not null &&
               LineIndex == other.LineIndex &&
               ColumnIndex == other.ColumnIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LineIndex, ColumnIndex);
    }
}
