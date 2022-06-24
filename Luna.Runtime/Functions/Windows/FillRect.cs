using System.Windows;
using System.Windows.Media;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("fill_rect", "context x y width height color")]
internal class FillRect : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var context = GetValueOrError<ObjectRuntimeValue>(0).Get<DrawingContext>();
        var x = GetValueOrError<NumericRuntimeValue>(1).FloatValue;
        var y = GetValueOrError<NumericRuntimeValue>(2).FloatValue;
        var width = GetValueOrError<NumericRuntimeValue>(3).FloatValue;
        var height = GetValueOrError<NumericRuntimeValue>(4).FloatValue;
        var color = GetValueOrError<StringRuntimeValue>(5).Value;

        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        context.DrawRectangle(brush, null, new Rect(new Point(x, y), new Size(width, height)));

        return VoidRuntimeValue.Instance;
    }
}
