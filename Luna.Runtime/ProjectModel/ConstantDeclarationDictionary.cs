using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.Utils;

namespace Luna.ProjectModel;

public interface IConstantDeclarationDictionary : IEnumerable<ConstantDeclaration>
{
    ConstantDeclaration? this[string name] { get; }
    int Count { get; }
    bool Contains(string name);
    IConstantDeclarationDictionary Subtraction(IConstantDeclarationDictionary x);
}

internal class ConstantDeclarationDictionary : IConstantDeclarationDictionary, IEquatable<ConstantDeclarationDictionary?>
{
    private readonly Dictionary<string, ConstantDeclaration> _dictionary = new();

    public ConstantDeclaration? this[string name] => _dictionary.ContainsKey(name) ? _dictionary[name] : null;

    public int Count => _dictionary.Count;

    public ConstantDeclarationDictionary() { }

    public ConstantDeclarationDictionary(IEnumerable<ConstantDeclaration> constantDeclarations)
    {
        constantDeclarations.Each(Add);
    }

    public void Add(ConstantDeclaration constantDeclaration)
    {
        var last = this.LastOrDefault();
        if (last != null &&
            (constantDeclaration.LineIndex < last.LineIndex || constantDeclaration.LineIndex == last.LineIndex && constantDeclaration.ColumnIndex < last.ColumnIndex))
        {
            throw new ArgumentException($"New item position must be greater then line index {last.LineIndex} and column index {last.ColumnIndex}");
        }

        _dictionary.Add(constantDeclaration.Name, constantDeclaration);
    }

    public bool Contains(string name) => _dictionary.ContainsKey(name);

    public IConstantDeclarationDictionary Subtraction(IConstantDeclarationDictionary x)
    {
        var result = new ConstantDeclarationDictionary();
        foreach (var item in this)
        {
            if (!x.Contains(item.Name))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public IEnumerator<ConstantDeclaration> GetEnumerator() => _dictionary.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _dictionary.Values.GetEnumerator();

    public override bool Equals(object? obj)
    {
        return Equals(obj as ConstantDeclarationDictionary);
    }

    public bool Equals(ConstantDeclarationDictionary? other)
    {
        return other is not null &&
               _dictionary.Values.SequenceEqual(other._dictionary.Values);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _dictionary.Values)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }
}
