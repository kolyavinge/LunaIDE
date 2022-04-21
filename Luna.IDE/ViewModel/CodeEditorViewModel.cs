using CodeHighlighter;
using Luna.IDE.CodeEditor;
using Luna.IDE.Mvvm;
using System.IO;

namespace Luna.IDE.ViewModel
{
    public class CodeEditorViewModel : NotificationObject
    {
        public TextHolder TextHolder { get; set; }

        public ICodeProvider CodeProvider { get; set; }

        public CodeEditorViewModel()
        {
            TextHolder = new TextHolder(File.ReadAllText(@"D:\Projects\LunaIDE\Examples\15\main.luna"));
            CodeProvider = new CodeProvider();
        }
    }
}
