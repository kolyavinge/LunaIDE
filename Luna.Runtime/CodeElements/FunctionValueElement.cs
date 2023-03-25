using System.Collections.Generic;
using Luna.Collections;

namespace Luna.CodeElements;

public class FunctionValueElement : ValueElement, IEquatable<FunctionValueElement?>
{
    public string Name { get; }
    public ReadonlyArray<ValueElement> ArgumentValues { get; }
    public int EndLineIndex { get; }
    public int EndColumnIndex { get; }

    public FunctionValueElement(string name, IEnumerable<ValueElement> argumentValues, int lineIndex, int columnIndex, int endLineIndex, int endColumnIndex)
        : base(lineIndex, columnIndex)
    {
        Name = name;
        EndLineIndex = endLineIndex;
        EndColumnIndex = endColumnIndex;
        ArgumentValues = new ReadonlyArray<ValueElement>(argumentValues);
    }

    internal FunctionValueElement(string name, IEnumerable<ValueElement> argumentValues) : this(name, argumentValues, 0, 0, 0, 0) { }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FunctionValueElement);
    }

    public bool Equals(FunctionValueElement? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Name == other.Name &&
               ArgumentValues.Equals(other.ArgumentValues);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, ArgumentValues);
    }
}
