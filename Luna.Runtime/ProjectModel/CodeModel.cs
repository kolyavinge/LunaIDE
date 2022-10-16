using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;

namespace Luna.ProjectModel;

public class CodeModel : IEquatable<CodeModel?>
{
    private readonly List<ImportDirective> _imports = new();
    private readonly ConstantDeclarationDictionary _constants = new();
    private readonly FunctionDeclarationDictionary _functions = new();

    public IReadOnlyCollection<ImportDirective> Imports => _imports;
    public IConstantDeclarationDictionary Constants => _constants;
    public IFunctionDeclarationDictionary Functions => _functions;
    public FunctionValueElement? RunFunction { get; internal set; }

    internal void AddImportDirective(ImportDirective import)
    {
        _imports.Add(import);
    }

    internal void AddConstantDeclaration(ConstantDeclaration constDirective)
    {
        _constants.Add(constDirective);
    }

    internal void AddFunctionDeclaration(FunctionDeclaration function)
    {
        _functions.Add(function);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CodeModel);
    }

    public bool Equals(CodeModel? other)
    {
        return other is not null &&
               _imports.SequenceEqual(other._imports) &&
               _constants.Equals(other._constants) &&
               _functions.Equals(other._functions) &&
               (RunFunction?.Equals(other.RunFunction) ?? true);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _imports)
        {
            hashCode.Add(item);
        }
        hashCode.Add(_constants);
        hashCode.Add(_functions);
        hashCode.Add(RunFunction);

        return hashCode.ToHashCode();
    }
}
