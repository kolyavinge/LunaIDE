using System.Collections;
using System.Collections.Generic;
using Luna.Utils;

namespace Luna.ProjectModel;

public interface IConstantDeclarationDictionary : IEnumerable<ConstantDeclaration>
{
    ConstantDeclaration? this[string name] { get; }
    int Count { get; }
    bool Contains(string name);
    IConstantDeclarationDictionary Subtraction(IConstantDeclarationDictionary x);
}

internal class ConstantDeclarationDictionary : IConstantDeclarationDictionary
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
}

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
