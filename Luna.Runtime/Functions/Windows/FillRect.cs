using System.Windows;
using System.Windows.Media;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("fill_rect", "context x y width height color")]
internal class FillRect : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var context = GetValueOrError<ObjectRuntimeValue>(argumentValues, 0).Get<DrawingContext>();
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 1).FloatValue;
        var y = GetValueOrError<NumericRuntimeValue>(argumentValues, 2).FloatValue;
        var width = GetValueOrError<NumericRuntimeValue>(argumentValues, 3).FloatValue;
        var height = GetValueOrError<NumericRuntimeValue>(argumentValues, 4).FloatValue;
        var color = GetValueOrError<StringRuntimeValue>(argumentValues, 5).Value;

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        context.DrawRectangle(brush, null, new Rect(new Point(x, y), new Size(width, height)));

        return VoidRuntimeValue.Instance;
    }
}
