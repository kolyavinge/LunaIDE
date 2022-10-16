using Luna.CodeElements;
using Luna.ProjectModel;

namespace Luna.Navigation;

public class DeclarationNavigatorResult
{
    public readonly DeclarationCodeElement Declaration;
    public readonly CodeFileProjectItem CodeFile;
    public DeclarationNavigatorResult(CodeFileProjectItem codeFile, DeclarationCodeElement declaration)
    {
        Declaration = declaration;
        CodeFile = codeFile;
    }
}

public interface IDeclarationNavigator
{
    DeclarationNavigatorResult? GetDeclarationFor(CodeFileProjectItem codeFile, CodeElement codeElement);
}

public class DeclarationNavigator : IDeclarationNavigator
{
    public DeclarationNavigatorResult? GetDeclarationFor(CodeFileProjectItem codeFile, CodeElement codeElement)
    {
        if (codeElement is ConstantDeclaration constantDeclaration)
        {
            return GetConstantDeclarationByName(codeFile, constantDeclaration.Name);
        }

        if (codeElement is FunctionDeclaration functionDeclaration)
        {
            return GetFunctionDeclarationByName(codeFile, functionDeclaration.Name);
        }

        if (codeElement is NamedConstantValueElement namedConstant)
        {
            return GetConstantDeclarationByName(codeFile, namedConstant.Name);
        }

        if (codeElement is FunctionValueElement function)
        {
            return GetFunctionDeclarationByName(codeFile, function.Name);
        }

        return null;
    }

    private DeclarationNavigatorResult? GetConstantDeclarationByName(CodeFileProjectItem codeFile, string constName)
    {
        if (codeFile.CodeModel.Constants.Contains(constName))
        {
            return new(codeFile, codeFile.CodeModel.Constants[constName]!);
        }
        foreach (var import in codeFile.CodeModel.Imports)
        {
            if (import.CodeFile.CodeModel.Constants.Contains(constName))
            {
                return new(import.CodeFile, import.CodeFile.CodeModel.Constants[constName]!);
            }
        }

        return null;
    }

    private DeclarationNavigatorResult? GetFunctionDeclarationByName(CodeFileProjectItem codeFile, string funcName)
    {
        if (codeFile.CodeModel.Functions.Contains(funcName))
        {
            return new(codeFile, codeFile.CodeModel.Functions[funcName]!);
        }
        foreach (var import in codeFile.CodeModel.Imports)
        {
            if (import.CodeFile.CodeModel.Functions.Contains(funcName))
            {
                return new(import.CodeFile, import.CodeFile.CodeModel.Functions[funcName]!);
            }
        }

        return null;
    }
}
