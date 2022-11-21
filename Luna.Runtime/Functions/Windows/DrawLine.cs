using System.Windows.Media;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("draw_line", "context from_x from_y to_x to_y color")]
internal class DrawLine : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var context = arguments.GetValueOrError<ObjectRuntimeValue>(0).Get<DrawingContext>();
        var fromX = arguments.GetValueOrError<NumericRuntimeValue>(1).FloatValue;
        var fromY = arguments.GetValueOrError<NumericRuntimeValue>(2).FloatValue;
        var toX = arguments.GetValueOrError<NumericRuntimeValue>(3).FloatValue;
        var toY = arguments.GetValueOrError<NumericRuntimeValue>(4).FloatValue;
        var color = arguments.GetValueOrError<StringRuntimeValue>(5).Value;

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        var pen = new Pen(brush, 1.0);
        context.DrawLine(pen, new(fromX, fromY), new(toX, toY));

        return VoidRuntimeValue.Instance;
    }
}
