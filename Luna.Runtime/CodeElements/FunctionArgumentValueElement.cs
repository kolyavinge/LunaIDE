namespace Luna.CodeElements;

public class FunctionArgumentValueElement : ValueElement, IEquatable<FunctionArgumentValueElement?>
{
    public string Name { get; }

    public FunctionArgumentValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal FunctionArgumentValueElement(string name) : this(name, 0, 0) { }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FunctionArgumentValueElement);
    }

    public bool Equals(FunctionArgumentValueElement? other)
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
