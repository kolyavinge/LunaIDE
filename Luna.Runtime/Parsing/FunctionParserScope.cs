using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing;

public interface IFunctionParserScope
{
    bool IsConstantExist(string name);
    bool IsFunctionExist(string name);
    bool IsRunFunctionExist();
}

public class FunctionParserScope : IFunctionParserScope
{
    private readonly List<CodeModel> _allCodeModels;
    private readonly CodeModel _currentCodeModel;
    private readonly CodeModelScope _codeModelScope;

    public FunctionParserScope(IEnumerable<CodeModel> allCodeModels, CodeModel currentCodeModel)
    {
        _allCodeModels = allCodeModels.ToList();
        _currentCodeModel = currentCodeModel;
        _codeModelScope = new CodeModelScope();
    }

    public bool IsConstantExist(string name)
    {
        return _codeModelScope.IsConstantExist(_currentCodeModel, name);
    }

    public bool IsFunctionExist(string name)
    {
        return _codeModelScope.IsFunctionExist(_currentCodeModel, name);
    }

    public bool IsRunFunctionExist()
    {
        return _allCodeModels.Any(x => x.RunFunction != null);
    }
}
