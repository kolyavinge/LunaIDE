using System.Windows.Media;

namespace Luna.IDE.Utils;

public static class ColorUtils
{
    public static Color FromHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString($"#ff{hex}");
    }
}
