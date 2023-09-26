using System.Globalization;
using System.Windows.Data;
using Luna.IDE.App.Media;

namespace Luna.IDE.App.Converters;

public class ImageConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var ic = values[0] as IImageCollection;
        if (ic is null) return null;

        if (values[1] is not string) return null;

        return ic.GetImage((string)values[1]);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
