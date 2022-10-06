using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Luna.IDE.App.View;

public class PointToThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var point = (Point)value;
        return new Thickness(point.X, point.Y, 0, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
