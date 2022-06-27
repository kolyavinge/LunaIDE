using System.Windows.Media;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("draw_line", "context from_x from_y to_x to_y color")]
internal class DrawLine : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var context = GetValueOrError<ObjectRuntimeValue>(argumentValues, 0).Get<DrawingContext>();
        var fromX = GetValueOrError<NumericRuntimeValue>(argumentValues, 1).FloatValue;
        var fromY = GetValueOrError<NumericRuntimeValue>(argumentValues, 2).FloatValue;
        var toX = GetValueOrError<NumericRuntimeValue>(argumentValues, 3).FloatValue;
        var toY = GetValueOrError<NumericRuntimeValue>(argumentValues, 4).FloatValue;
        var color = GetValueOrError<StringRuntimeValue>(argumentValues, 5).Value;

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        var pen = new Pen(brush, 1.0);
        context.DrawLine(pen, new(fromX, fromY), new(toX, toY));

        return VoidRuntimeValue.Instance;
    }
}
