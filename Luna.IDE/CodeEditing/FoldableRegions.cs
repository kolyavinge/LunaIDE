using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public readonly struct FoldableRegion
{
    public readonly int StartLine;
    public readonly int LinesCount;

    public FoldableRegion(int startLine, int linesCount)
    {
        StartLine = startLine;
        LinesCount = linesCount;
    }
}

public interface IFoldableRegions
{
    IEnumerable<FoldableRegion> GetRegions(CodeModel codeModel);
}

public class FoldableRegions : IFoldableRegions
{
    public IEnumerable<FoldableRegion> GetRegions(CodeModel codeModel)
    {
        return GetImports(codeModel).Union(GetConstants(codeModel)).Union(GetFunctions(codeModel));
    }

    private IEnumerable<FoldableRegion> GetImports(CodeModel codeModel)
    {
        if (codeModel.Imports.Any())
        {
            var firstImport = codeModel.Imports.First();
            var lastImport = codeModel.Imports.Last();
            if (firstImport.LineIndex < lastImport.LineIndex)
            {
                yield return new FoldableRegion(firstImport.LineIndex, lastImport.LineIndex - firstImport.LineIndex + 1);
            }
        }
    }

    private IEnumerable<FoldableRegion> GetConstants(CodeModel codeModel)
    {
        if (!codeModel.Constants.Any()) yield break;
        var items = codeModel.Constants.Select(x => new { x.LineIndex, IsConstant = true })
            .Union(codeModel.Functions.Select(x => new { x.LineIndex, IsConstant = false }))
            .OrderBy(x => x.LineIndex);
        int state = 1, startLine = 0, endLine = 0;
        foreach (var item in items)
        {
            if (state == 1)
            {
                if (item.IsConstant)
                {
                    state = 2;
                    startLine = item.LineIndex;
                }
            }
            else if (state == 2)
            {
                if (item.IsConstant)
                {
                    endLine = item.LineIndex;
                }
                else
                {
                    state = 1;
                    if (endLine > startLine)
                    {
                        yield return new FoldableRegion(startLine, endLine - startLine + 1);
                    }
                }
            }
        }
        if (state == 2 && endLine > startLine)
        {
            yield return new FoldableRegion(startLine, endLine - startLine + 1);
        }
    }

    private IEnumerable<FoldableRegion> GetFunctions(CodeModel codeModel)
    {
        foreach (var f in codeModel.Functions)
        {
            if (f.LineIndex < f.Body.EndLineIndex)
            {
                yield return new FoldableRegion(f.LineIndex, f.Body.EndLineIndex - f.LineIndex + 1);
            }
        }
    }
}
