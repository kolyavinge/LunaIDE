using System.Windows;
using System.Windows.Input;

namespace Luna.IDE.View
{
    public partial class MainView : Window
    {
        private Point? _lastMousePosition;

        public MainView()
        {
            InitializeComponent();
        }

        private void OnTitlePanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                _lastMousePosition = e.GetPosition(this);
                Mouse.Capture((IInputElement)sender);
            }
            else if (e.ClickCount == 2)
            {
                SwitchMaximizeState();
            }
        }

        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            if (_lastMousePosition != null)
            {
                var pos = e.GetPosition(this);
                var deltaX = pos.X - _lastMousePosition.Value.X;
                var deltaY = pos.Y - _lastMousePosition.Value.Y;
                Left += deltaX;
                Top += deltaY;
            }
        }

        private void OnWindowMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = null;
            Mouse.Capture(null);
        }

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            SwitchMaximizeState();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SwitchMaximizeState()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
    }
}
