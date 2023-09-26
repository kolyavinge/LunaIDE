using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Luna.IDE.App.Controls;

public static class TextboxExtensions
{
    #region OldForeground Для внутренних нужд
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    private static void SetOldForeground(DependencyObject d, Brush value)
    {
        d.SetValue(OldForegroundProperty, value);
    }

    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    private static Brush GetOldForeground(DependencyObject d)
    {
        return (Brush)d.GetValue(OldForegroundProperty);
    }

    private static readonly DependencyProperty OldForegroundProperty =
        DependencyProperty.RegisterAttached("OldForeground", typeof(Brush), typeof(TextboxExtensions));
    #endregion

    #region PlaceholderForeground
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetPlaceholderForeground(DependencyObject d, Brush value)
    {
        d.SetValue(PlaceholderForegroundProperty, value);
    }

    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static Brush GetPlaceholderForeground(DependencyObject d)
    {
        return (Brush)d.GetValue(PlaceholderForegroundProperty);
    }

    public static readonly DependencyProperty PlaceholderForegroundProperty =
        DependencyProperty.RegisterAttached("PlaceholderForeground", typeof(Brush), typeof(TextboxExtensions), new PropertyMetadata(SystemColors.ControlTextBrush));
    #endregion

    #region Placeholder
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetPlaceholder(DependencyObject d, string value)
    {
        d.SetValue(PlaceholderProperty, value);
    }

    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static string GetPlaceholder(DependencyObject d)
    {
        return (string)d.GetValue(PlaceholderProperty);
    }

    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextboxExtensions), new PropertyMetadata(default(string), OnPlaceholderChanged));

    private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var tb = (TextBox)d;

        tb.LostFocus -= OnLostFocus;
        tb.GotFocus -= OnGotFocus;

        if (e.NewValue is not null)
        {
            tb.GotFocus += OnGotFocus;
            tb.LostFocus += OnLostFocus;
        }

        SetPlaceholder(d, (string)e.NewValue!);
        SetOldForeground(d, tb.Foreground);

        if (!tb.IsFocused) ShowPlaceholder(tb);
    }

    private static void OnLostFocus(object sender, RoutedEventArgs e)
    {
        ShowPlaceholder((TextBox)sender);
    }

    private static void OnGotFocus(object sender, RoutedEventArgs e)
    {
        HidePlaceholder((TextBox)sender);
    }

    private static void ShowPlaceholder(TextBox tb)
    {
        if (String.IsNullOrWhiteSpace(tb.Text))
        {
            tb.Text = GetPlaceholder(tb);
            tb.Foreground = GetPlaceholderForeground(tb);
        }
    }

    private static void HidePlaceholder(TextBox tb)
    {
        var placeholder = GetPlaceholder(tb);
        if (tb.Text == placeholder)
        {
            tb.Text = "";
            tb.Foreground = GetOldForeground(tb);
        }
    }
    #endregion
}
