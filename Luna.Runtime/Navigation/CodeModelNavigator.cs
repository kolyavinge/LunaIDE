using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.ProjectModel;

namespace Luna.Navigation;

public class CodeModelNavigatorResult
{
    public readonly CodeElement CodeElement;
    public readonly CodeElement[] Chain;
    public CodeModelNavigatorResult(CodeElement codeElement, CodeElement[] chain)
    {
        CodeElement = codeElement;
        Chain = chain;
    }
}

public interface ICodeModelNavigator
{
    CodeModelNavigatorResult? GetCodeElementByPosition(CodeModel codeModel, int lineIndex, int columnIndex);
}

public class CodeModelNavigator : ICodeModelNavigator
{
    public CodeModelNavigatorResult? GetCodeElementByPosition(CodeModel codeModel, int lineIndex, int columnIndex)
    {
        var parent = GetParentElement(codeModel, lineIndex, columnIndex);
        if (parent == null) return null;
        var searchLogic = new DepthSearchLogic();
        var result = searchLogic.Seach(parent, e => e.LineIndex == lineIndex && e.ColumnIndex == columnIndex);
        if (result == null) return null;

        return new(result.CodeElement, result.Chain);
    }

    private CodeElement? GetParentElement(CodeModel codeModel, int lineIndex, int columnIndex)
    {
        var allElements = new List<CodeElement>();
        allElements.AddRange(codeModel.Constants);
        allElements.AddRange(codeModel.Functions);
        if (codeModel.RunFunction != null) allElements.Add(codeModel.RunFunction);
        allElements.Sort(new ReverseCodeElementComparer(new CodeElementComparer()));

        return allElements.FirstOrDefault(e => IsParent(e, lineIndex, columnIndex));
    }

    private bool IsParent(CodeElement element, int lineIndex, int columnIndex)
    {
        return element.LineIndex <= lineIndex && element.ColumnIndex <= columnIndex;
    }
}
