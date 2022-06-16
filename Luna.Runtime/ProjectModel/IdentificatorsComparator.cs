namespace Luna.ProjectModel;

public class CodeModelScopeIdentificatorsDifferent
{
    public IConstantDeclarationDictionary AddedDeclaredConstants { get; }
    public IFunctionDeclarationDictionary AddedDeclaredFunctions { get; }
    public IConstantDeclarationDictionary RemovedDeclaredConstants { get; }
    public IFunctionDeclarationDictionary RemovedDeclaredFunctions { get; }
    public IConstantDeclarationDictionary AddedImportedConstants { get; }
    public IFunctionDeclarationDictionary AddedImportedFunctions { get; }
    public IConstantDeclarationDictionary RemovedImportedConstants { get; }
    public IFunctionDeclarationDictionary RemovedImportedFunctions { get; }

    public CodeModelScopeIdentificatorsDifferent(
        IConstantDeclarationDictionary addedDeclaredConstants,
        IFunctionDeclarationDictionary addedDeclaredFunctions,
        IConstantDeclarationDictionary removedDeclaredConstants,
        IFunctionDeclarationDictionary removedDeclaredFunctions,
        IConstantDeclarationDictionary addedImportedConstants,
        IFunctionDeclarationDictionary addedImportedFunctions,
        IConstantDeclarationDictionary removedImportedConstants,
        IFunctionDeclarationDictionary removedImportedFunctions)
    {
        AddedDeclaredConstants = addedDeclaredConstants;
        AddedDeclaredFunctions = addedDeclaredFunctions;
        RemovedDeclaredConstants = removedDeclaredConstants;
        RemovedDeclaredFunctions = removedDeclaredFunctions;
        AddedImportedConstants = addedImportedConstants;
        AddedImportedFunctions = addedImportedFunctions;
        RemovedImportedConstants = removedImportedConstants;
        RemovedImportedFunctions = removedImportedFunctions;
    }

    public CodeModelScopeIdentificatorsDifferent() :
        this(
            new ConstantDeclarationDictionary(),
            new FunctionDeclarationDictionary(),
            new ConstantDeclarationDictionary(),
            new FunctionDeclarationDictionary(),
            new ConstantDeclarationDictionary(),
            new FunctionDeclarationDictionary(),
            new ConstantDeclarationDictionary(),
            new FunctionDeclarationDictionary())
    { }
}

internal static class IdentificatorsComparator
{
    public static CodeModelScopeIdentificatorsDifferent GetDifferent(
        CodeModelScopeIdentificators oldIdentificators, CodeModelScopeIdentificators newIdentificators)
    {
        var addedDeclaredConstants = new ConstantDeclarationDictionary(newIdentificators.DeclaredConstants.Subtraction(oldIdentificators.DeclaredConstants));
        var addedDeclaredFunctions = new FunctionDeclarationDictionary(newIdentificators.DeclaredFunctions.Subtraction(oldIdentificators.DeclaredFunctions));

        var removedDeclaredConstants = new ConstantDeclarationDictionary(oldIdentificators.DeclaredConstants.Subtraction(newIdentificators.DeclaredConstants));
        var removedDeclaredFunctions = new FunctionDeclarationDictionary(oldIdentificators.DeclaredFunctions.Subtraction(newIdentificators.DeclaredFunctions));

        var addedImportedConstants = new ConstantDeclarationDictionary(newIdentificators.ImportedConstants.Subtraction(oldIdentificators.ImportedConstants));
        var addedImportedFunctions = new FunctionDeclarationDictionary(newIdentificators.ImportedFunctions.Subtraction(oldIdentificators.ImportedFunctions));

        var removedImportedConstants = new ConstantDeclarationDictionary(oldIdentificators.ImportedConstants.Subtraction(newIdentificators.ImportedConstants));
        var removedImportedFunctions = new FunctionDeclarationDictionary(oldIdentificators.ImportedFunctions.Subtraction(newIdentificators.ImportedFunctions));

        return new(
            addedDeclaredConstants,
            addedDeclaredFunctions,
            removedDeclaredConstants,
            removedDeclaredFunctions,
            addedImportedConstants,
            addedImportedFunctions,
            removedImportedConstants,
            removedImportedFunctions);
    }
}
