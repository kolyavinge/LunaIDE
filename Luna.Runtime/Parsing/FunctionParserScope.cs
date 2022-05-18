using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing;

public interface IFunctionParserScope
{
    bool IsConstExist(string name);
    bool IsFunctionExist(string name);
    bool IsRunFunctionExist();
}

public class FunctionParserScope : IFunctionParserScope
{
    private readonly List<CodeModel> _allCodeModels;
    private readonly CodeModel _currentCodeModel;

    public FunctionParserScope(IEnumerable<CodeModel> allCodeModels, CodeModel currentCodeModel)
    {
        _allCodeModels = allCodeModels.ToList();
        _currentCodeModel = currentCodeModel;
    }

    public bool IsConstExist(string name)
    {
        return _currentCodeModel.Constants.Contains(name) || _currentCodeModel.Imports.Any(x => x.CodeFile.CodeModel!.Constants.Contains(name));
    }

    public bool IsFunctionExist(string name)
    {
        return _currentCodeModel.Functions.Contains(name) || _currentCodeModel.Imports.Any(x => x.CodeFile.CodeModel!.Functions.Contains(name));
    }

    public bool IsRunFunctionExist()
    {
        return _allCodeModels.Any(x => x.RunFunction != null);
    }
}
