using System.Globalization;
using System.Windows.Data;

namespace Luna.IDE.App.View;

public class ControlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var c = (System.Windows.Controls.Control)value;
        return new Control(c);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    class Control : IControl
    {
        private readonly System.Windows.Controls.Control _control;

        public double ActualWidth => _control.ActualWidth;

        public double ActualHeight => _control.ActualHeight;

        public double MaxWidth => _control.MaxWidth;

        public double MaxHeight => _control.MaxHeight;

        public Control(System.Windows.Controls.Control control)
        {
            _control = control;
        }

        public void Focus()
        {
            _control.Focus();
        }
    }
}
