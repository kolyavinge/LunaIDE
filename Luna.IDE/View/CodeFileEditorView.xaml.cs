using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.ViewModel;
using Luna.ProjectModel;

namespace Luna.IDE.View
{
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

        private void ScrollBarOnScroll(object sender, ScrollEventArgs e)
        {
            var sb = (ScrollBar)sender;
            if (e.ScrollEventType == ScrollEventType.LargeIncrement)
            {
                sb.Value += 200;
            }
            else if (e.ScrollEventType == ScrollEventType.LargeDecrement)
            {
                sb.Value -= 200;
            }
        }

        private void VerticalScrollBarOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            verticalScrollBar.Value -= e.Delta;
        }
    }
}
