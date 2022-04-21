using CodeHighlighter;
using Luna.IDE.CodeEditor;
using System.IO;

namespace Luna.IDE.ViewModel
{
    public class EditorViewModel
    {
        public TextHolder TextHolder { get; set; }

        public ICodeProvider CodeProvider { get; set; }

        public EditorViewModel()
        {
            TextHolder = new TextHolder(File.ReadAllText(@"D:\Projects\LunaIDE\Examples\15\main.luna"));
            CodeProvider = new CodeProvider();
        }
    }
}
