using System.Collections.Generic;
using Luna.CodeElements;

namespace Luna.ProjectModel;

public class CodeModel
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
}
