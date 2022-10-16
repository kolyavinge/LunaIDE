namespace Luna.CodeElements;

public class FunctionArgument : CodeElement, IEquatable<FunctionArgument?>
{
    public string Name { get; }

    public FunctionArgument(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal FunctionArgument(string name) : this(name, 0, 0) { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FunctionArgument);
    }

    public bool Equals(FunctionArgument? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name);
    }
}
