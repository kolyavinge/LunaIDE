using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("draw_text", "context x y text color size")]
internal class DrawText : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var context = GetValueOrError<ObjectRuntimeValue>(argumentValues, 0).Get<DrawingContext>();
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 1).FloatValue;
        var y = GetValueOrError<NumericRuntimeValue>(argumentValues, 2).FloatValue;
        var text = GetValueOrError<IRuntimeValue>(argumentValues, 3).ToString();
        var color = GetValueOrError<StringRuntimeValue>(argumentValues, 4).Value;
        var size = GetValueOrError<NumericRuntimeValue>(argumentValues, 5).FloatValue;

        var foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"#{color}"));
        var typeface = new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal, new FontFamily("Consolas"));
        var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, size, foreground, 1.0);
        var origin = new Point(x, y);
        context.DrawText(formattedText, origin);

        return VoidRuntimeValue.Instance;
    }
}
