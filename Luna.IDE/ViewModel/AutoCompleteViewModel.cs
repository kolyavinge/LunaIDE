using System.Windows;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.IDE.View;

namespace Luna.IDE.ViewModel;

public class AutoCompleteViewModel : NotificationObject
{
    private Point _absolutePosition;
    private Thickness _screenPosition;
    private IControl? _control;
    private double _parentWidth, _parentHeight;
    private VerticalAlignment _verticalAlignment;

    public AutoComplete Model { get; }

    public ICommand LoadedCommand { get; set; }

    public ICommand MouseClickCommand { get; set; }

    public VerticalAlignment VerticalAlignment
    {
        get => _verticalAlignment;
        set { _verticalAlignment = value; RaisePropertyChanged(() => VerticalAlignment); }
    }

    public Thickness ScreenPosition
    {
        get => _screenPosition;
        set { _screenPosition = value; RaisePropertyChanged(() => ScreenPosition); }
    }

    public AutoCompleteViewModel(AutoComplete model)
    {
        Model = model;
        LoadedCommand = new ActionCommand<IControl>(x => _control = x);
        MouseClickCommand = new ActionCommand(Model.Complete);
    }

    public void Show(double verticalScrollBarValue, double horizontalScrollBarValue, double parentWidth, double parentHeight)
    {
        _parentWidth = parentWidth;
        _parentHeight = parentHeight;
        (var cursorLineIndex, var cursorColumnIndex) = Model.DataContext.CursorPosition;
        var cursorToken = Model.GetCursorToken();
        var y = cursorLineIndex * Model.DataContext.TextLineHeight;
        var x = cursorToken != null
            ? cursorToken.StartColumnIndex * Model.DataContext.TextLetterWidth
            : cursorColumnIndex * Model.DataContext.TextLetterWidth;
        _absolutePosition = new(x, y);
        var screenX = x - horizontalScrollBarValue;
        var screenY = y - verticalScrollBarValue;
        if (screenY + _control!.MaxHeight <= parentHeight)
        {
            VerticalAlignment = VerticalAlignment.Top;
            screenY += Model.DataContext.TextLineHeight;
            ScreenPosition = new(screenX, screenY, 0, 0);
        }
        else
        {
            VerticalAlignment = VerticalAlignment.Bottom;
            screenY = parentHeight - screenY;
            ScreenPosition = new(screenX, 0, 0, screenY);
        }
        Model.Show();
    }

    public void CorrectByVerticalScrollBarValue(double verticalScrollBarValue)
    {
        if (!Model.IsVisible) return;
        var screenY = _absolutePosition.Y - verticalScrollBarValue;
        if (VerticalAlignment == VerticalAlignment.Top)
        {
            screenY += Model.DataContext.TextLineHeight;
            if (screenY < 0) screenY = 0;
            else if (screenY + _control!.ActualHeight > _parentHeight) screenY = _parentHeight - _control.ActualHeight;
            ScreenPosition = new(ScreenPosition.Left, screenY, 0, 0);
        }
        else
        {
            screenY = _parentHeight - screenY;
            if (screenY < 0) screenY = 0;
            else if (screenY + _control!.ActualHeight > _parentHeight) screenY = _parentHeight - _control!.ActualHeight;
            ScreenPosition = new(ScreenPosition.Left, 0, 0, screenY);
        }
    }

    public void CorrectByHorizontalScrollBarValue(double horizontalScrollBarValue)
    {
        if (!Model.IsVisible) return;
        var x = _absolutePosition.X - horizontalScrollBarValue;
        if (x < 0) x = 0;
        else if (x + _control!.ActualWidth > _parentWidth) x = _parentWidth - _control.ActualWidth;
        ScreenPosition = new(x, ScreenPosition.Top, ScreenPosition.Right, ScreenPosition.Bottom);
    }
}
