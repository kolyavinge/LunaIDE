using System.Windows.Controls;
using Luna.IDE.App.ViewModel;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.App.View;

[EditorFor(typeof(CodeFileProjectItem))]
public partial class CodeFileEditorView : UserControl
{
    public CodeFileEditorView()
    {
        InitializeComponent();
    }

    public CodeFileEditorView(CodeFileEditorViewModel vm) : this()
    {
        DataContext = vm;
    }
}
