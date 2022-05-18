using System.Collections;
using System.Collections.Generic;

namespace Luna.ProjectModel;

public interface IConstDeclarationDictionary : IEnumerable<ConstDeclaration>
{
    ConstDeclaration? this[string name] { get; }
    int Count { get; }
    bool Contains(string name);
}

internal class ConstDeclarationDictionary : IConstDeclarationDictionary, IEnumerable<ConstDeclaration>
{
    private readonly Dictionary<string, ConstDeclaration> _dictionary = new();

    public ConstDeclaration? this[string name] => _dictionary.ContainsKey(name) ? _dictionary[name] : null;

    public int Count => _dictionary.Count;

    public void Add(ConstDeclaration constDeclaration)
    {
        _dictionary.Add(constDeclaration.Name, constDeclaration);
    }

    public bool Contains(string name) => _dictionary.ContainsKey(name);

    public IEnumerator<ConstDeclaration> GetEnumerator() => _dictionary.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _dictionary.Values.GetEnumerator();
}

public interface IFunctionDeclarationDictionary : IEnumerable<FunctionDeclaration>
{
    FunctionDeclaration? this[string name] { get; }
    int Count { get; }
    bool Contains(string name);
}

internal class FunctionDeclarationDictionary : IFunctionDeclarationDictionary, IEnumerable<FunctionDeclaration>
{
    private readonly Dictionary<string, FunctionDeclaration> _dictionary = new();

    public FunctionDeclaration? this[string name] => _dictionary.ContainsKey(name) ? _dictionary[name] : null;

    public int Count => _dictionary.Count;

    public void Add(FunctionDeclaration functionDeclaration)
    {
        _dictionary.Add(functionDeclaration.Name, functionDeclaration);
    }

    public bool Contains(string name) => _dictionary.ContainsKey(name);

    public IEnumerator<FunctionDeclaration> GetEnumerator() => _dictionary.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _dictionary.Values.GetEnumerator();
}
