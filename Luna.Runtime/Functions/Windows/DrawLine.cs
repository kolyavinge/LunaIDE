using System.Windows.Media;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("draw_line", "context from_x from_y to_x to_y color")]
internal class DrawLine : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var context = GetValueOrError<ObjectRuntimeValue>(0).Get<DrawingContext>();
        var fromX = GetValueOrError<NumericRuntimeValue>(1).FloatValue;
        var fromY = GetValueOrError<NumericRuntimeValue>(2).FloatValue;
        var toX = GetValueOrError<NumericRuntimeValue>(3).FloatValue;
        var toY = GetValueOrError<NumericRuntimeValue>(4).FloatValue;
        var color = GetValueOrError<StringRuntimeValue>(5).Value;

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        var pen = new Pen(brush, 1.0);
        context.DrawLine(pen, new(fromX, fromY), new(toX, toY));

        return VoidRuntimeValue.Instance;
    }
}
