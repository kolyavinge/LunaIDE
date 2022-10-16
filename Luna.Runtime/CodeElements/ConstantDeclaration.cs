using System.Collections.Generic;

namespace Luna.CodeElements;

public class ConstantDeclaration : DeclarationCodeElement, IEquatable<ConstantDeclaration?>
{
    public string Name { get; }
    public ValueElement Value { get; }

    public ConstantDeclaration(string name, ValueElement value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
        Value = value;
    }

    internal ConstantDeclaration(string name, ValueElement value) : this(name, value, 0, 0) { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ConstantDeclaration);
    }

    public bool Equals(ConstantDeclaration? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Name == other.Name &&
               EqualityComparer<ValueElement>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, Value);
    }
}
