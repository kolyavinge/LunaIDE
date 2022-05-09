using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing
{
    internal interface ICodeFileOrderLogic
    {
        IEnumerable<CodeFileProjectItem> ByImports(IEnumerable<CodeFileProjectItem> codeFiles);
    }

    internal class CodeFileOrderLogic : ICodeFileOrderLogic
    {
        private HashSet<CodeFileProjectItem>? _resultSet;
        private List<CodeFileProjectItem>? _resultList;

        public IEnumerable<CodeFileProjectItem> ByImports(IEnumerable<CodeFileProjectItem> codeFiles)
        {
            _resultSet = new HashSet<CodeFileProjectItem>();
            _resultList = new List<CodeFileProjectItem>();
            foreach (var codeFile in codeFiles)
            {
                var chain = new HashSet<CodeFileProjectItem>();
                ProcessFile(codeFile, chain);
            }

            return _resultList;
        }

        private void ProcessFile(CodeFileProjectItem codeFile, HashSet<CodeFileProjectItem> chain)
        {
            if (_resultSet!.Contains(codeFile)) return;

            if (codeFile.CodeModel!.Imports.Any())
            {
                chain.Add(codeFile);
                foreach (var import in codeFile.CodeModel!.Imports)
                {
                    if (!chain.Contains(import.CodeFile))
                    {
                        ProcessFile(import.CodeFile, chain);
                    }
                }
                chain.Remove(codeFile);
            }

            _resultSet.Add(codeFile);
            _resultList!.Add(codeFile);
        }
    }
}
