using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.Utils;

namespace Luna.ProjectModel;

public interface IFunctionDeclarationDictionary : IEnumerable<FunctionDeclaration>
{
    FunctionDeclaration? this[string name] { get; }
    int Count { get; }
    bool Contains(string name);
    IFunctionDeclarationDictionary Subtraction(IFunctionDeclarationDictionary x);
}

internal class FunctionDeclarationDictionary : IFunctionDeclarationDictionary
{
    private readonly Dictionary<string, FunctionDeclaration> _dictionary = new();

    public FunctionDeclaration? this[string name] => _dictionary.ContainsKey(name) ? _dictionary[name] : null;

    public int Count => _dictionary.Count;

    public FunctionDeclarationDictionary() { }

    public FunctionDeclarationDictionary(IEnumerable<FunctionDeclaration> functionDeclarations)
    {
        functionDeclarations.Each(Add);
    }

    public void Add(FunctionDeclaration functionDeclaration)
    {
        var last = this.LastOrDefault();
        if (last != null &&
            (functionDeclaration.LineIndex < last.LineIndex || functionDeclaration.LineIndex == last.LineIndex && functionDeclaration.ColumnIndex < last.ColumnIndex))
        {
            throw new ArgumentException($"New item position must be greater then line index {last.LineIndex} and column index {last.ColumnIndex}");
        }

        _dictionary.Add(functionDeclaration.Name, functionDeclaration);
    }

    public bool Contains(string name) => _dictionary.ContainsKey(name);

    public IFunctionDeclarationDictionary Subtraction(IFunctionDeclarationDictionary x)
    {
        var result = new FunctionDeclarationDictionary();
        foreach (var item in this)
        {
            if (!x.Contains(item.Name))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public IEnumerator<FunctionDeclaration> GetEnumerator() => _dictionary.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _dictionary.Values.GetEnumerator();
}
