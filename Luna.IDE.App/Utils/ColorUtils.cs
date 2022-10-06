using System.Windows.Media;

namespace Luna.IDE.App.Utils;

public static class ColorUtils
{
    public static Color FromHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString($"#ff{hex}");
    }
}
