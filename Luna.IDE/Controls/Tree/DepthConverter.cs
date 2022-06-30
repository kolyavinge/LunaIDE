using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Luna.IDE.Controls.Tree;

public class DepthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var left = (int)value * 16;
        return new Thickness(left, 0, 0, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
