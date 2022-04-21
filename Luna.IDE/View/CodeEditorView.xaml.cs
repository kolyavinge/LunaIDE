using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Luna.IDE.Infrastructure;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View
{
    public partial class CodeEditorView : UserControl
    {
        public CodeEditorView()
        {
            InitializeComponent();
            DataContext = DependencyContainer.Resolve<CodeEditorViewModel>();
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
