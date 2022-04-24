using System.IO;
using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.ProjectModel;

namespace Luna.IDE.ViewModel
{
    [EditorFor(typeof(CodeFileProjectItem))]
    public class CodeFileEditorViewModel : NotificationObject
    {
        public ICodeFileEditor Model { get; set; }

        public CodeFileEditorViewModel(ICodeFileEditor codeFileEditor)
        {
            Model = codeFileEditor;
        }
    }
}
