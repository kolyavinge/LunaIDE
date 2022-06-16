using System.Collections.Generic;
using System.Linq;
using Luna.Functions;

namespace Luna.ProjectModel;

public class CodeModelScope
{
    private readonly EmbeddedFunctionDeclarationsCollection _embeddedFunctions = new();

    public bool IsConstantExist(CodeModel codeModel, string name)
    {
        return codeModel.Constants.Contains(name) || codeModel.Imports.Any(x => x.CodeFile.CodeModel.Constants.Contains(name));
    }

    public bool IsFunctionExist(CodeModel codeModel, string name)
    {
        return _embeddedFunctions.Contains(name) ||
            codeModel.Functions.Contains(name) ||
            codeModel.Imports.Any(x => x.CodeFile.CodeModel.Functions.Contains(name));
    }

    public CodeModelScopeIdentificators GetScopeIdentificators(CodeModel codeModel)
    {
        return new(
            codeModel.Constants,
            codeModel.Functions,
            codeModel.Imports.SelectMany(x => x.CodeFile.CodeModel.Constants),
            codeModel.Imports.SelectMany(x => x.CodeFile.CodeModel.Functions));
    }
}

public class CodeModelScopeIdentificators
{
    public IConstantDeclarationDictionary DeclaredConstants { get; }
    public IFunctionDeclarationDictionary DeclaredFunctions { get; }
    public IConstantDeclarationDictionary ImportedConstants { get; }
    public IFunctionDeclarationDictionary ImportedFunctions { get; }

    public CodeModelScopeIdentificators(
        IEnumerable<ConstantDeclaration> declaredConstants,
        IEnumerable<FunctionDeclaration> declaredFunctions,
        IEnumerable<ConstantDeclaration> importedConstants,
        IEnumerable<FunctionDeclaration> importedFunctions)
    {
        DeclaredConstants = new ConstantDeclarationDictionary(declaredConstants);
        DeclaredFunctions = new FunctionDeclarationDictionary(declaredFunctions);
        ImportedConstants = new ConstantDeclarationDictionary(importedConstants);
        ImportedFunctions = new FunctionDeclarationDictionary(importedFunctions);
    }
}
