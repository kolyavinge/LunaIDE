namespace Luna.CodeElements;

public class NamedConstantValueElement : ValueElement, IEquatable<NamedConstantValueElement?>
{
    public string Name { get; }

    public NamedConstantValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal NamedConstantValueElement(string name) : this(name, 0, 0) { }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as NamedConstantValueElement);
    }

    public bool Equals(NamedConstantValueElement? other)
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
