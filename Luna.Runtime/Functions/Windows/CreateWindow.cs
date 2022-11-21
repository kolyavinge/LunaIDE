using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("create_window", "title render_func mouse_handler_func")]
internal class CreateWindow : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var title = arguments.GetValueOrError<StringRuntimeValue>(0).Value;
        var renderFunc = arguments.GetFunctionOrError(1);
        var mouseHandlerFunc = arguments.GetFunctionOrError(2);

        var window = new AppWindow(renderFunc, mouseHandlerFunc);
        window.Title = title;
        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        window.Width = 800;
        window.Height = 600;

        return new ObjectRuntimeValue(window);
    }
}

class AppWindow : System.Windows.Window
{
    private readonly FunctionRuntimeValue _renderFunc;
    private readonly FunctionRuntimeValue _mouseHandlerFunc;

    public AppWindow(FunctionRuntimeValue renderFunc, FunctionRuntimeValue mouseHandlerFunc)
    {
        _renderFunc = renderFunc;
        _mouseHandlerFunc = mouseHandlerFunc;
        var template = new ControlTemplate(typeof(AppWindow));
        template.VisualTree = new FrameworkElementFactory(typeof(Grid), "RootLayout");
        Template = template;
    }

    protected override void OnRender(DrawingContext context)
    {
        var rect = new Rect(new Point(0, 0), new Size(ActualWidth, ActualHeight));
        context.PushClip(new RectangleGeometry(rect));
        context.DrawRectangle(Brushes.White, null, rect);
        _renderFunc.GetValue(new IRuntimeValue[] { new ObjectRuntimeValue(context) }.ToReadonlyArray());
        context.Pop();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        var pos = e.GetPosition(this);
        var window = new ObjectRuntimeValue(this);
        var eventName = new StringRuntimeValue("click");
        var x = new FloatRuntimeValue(pos.X);
        var y = new FloatRuntimeValue(pos.Y);
        _mouseHandlerFunc.GetValue(new IRuntimeValue[] { eventName, x, y }.ToReadonlyArray());
    }
}
