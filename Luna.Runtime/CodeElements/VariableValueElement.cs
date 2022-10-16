namespace Luna.CodeElements;

public class VariableValueElement : ValueElement, IEquatable<VariableValueElement?>
{
    public string Name { get; }

    public VariableValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal VariableValueElement(string name) : this(name, 0, 0) { }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VariableValueElement);
    }

    public bool Equals(VariableValueElement? other)
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
