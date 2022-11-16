using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Luna.IDE.App.Controls;

public class ScrollBarExt : ScrollBar
{
    public ScrollBarExt()
    {
        Cursor = Cursors.Arrow;
        Scroll += OnScroll;
        MouseWheel += OnMouseWheel;
    }

    private void OnScroll(object sender, ScrollEventArgs e)
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

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var sb = (ScrollBar)sender;
        if (sb.Orientation == System.Windows.Controls.Orientation.Vertical)
        {
            sb.Value -= e.Delta;
        }
    }
}
