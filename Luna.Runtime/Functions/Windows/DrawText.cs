using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("draw_text", "context x y text color size")]
internal class DrawText : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var context = GetValueOrError<ObjectRuntimeValue>(0).Get<DrawingContext>();
        var x = GetValueOrError<NumericRuntimeValue>(1).FloatValue;
        var y = GetValueOrError<NumericRuntimeValue>(2).FloatValue;
        var text = GetValueOrError<StringRuntimeValue>(3).Value;
        var color = GetValueOrError<StringRuntimeValue>(4).Value;
        var size = GetValueOrError<NumericRuntimeValue>(5).FloatValue;

        var foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        var typeface = new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, new FontFamily("Consolas"));
        var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, size, foreground, 1.0);
        var origin = new Point(x, y);
        context.DrawText(formattedText, origin);

        return VoidRuntimeValue.Instance;
    }
}
