using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Luna.IDE.App.Converters;

public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var intValue = (int)value;

        return intValue == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
