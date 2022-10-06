using System.Windows;
using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.App.View;
using Luna.IDE.Common;
using Luna.IDE.Model;

namespace Luna.IDE.App.ViewModel;

public class AutoCompleteViewModel : NotificationObject
{
    private Point _absolutePosition;
    private Thickness _screenPosition;
    private IControl? _control;
    private double _parentWidth, _parentHeight;
    private VerticalAlignment _verticalAlignment;
    private AutoComplete? _model;

    public AutoComplete? Model
    {
        get => _model;
        set
        {
            _model = value ?? throw new ArgumentNullException();
            MouseClickCommand = new ActionCommand(_model.Complete);
            RaisePropertyChanged(() => Model!);
        }
    }

    public ICommand LoadedCommand { get; set; }

    public ICommand? MouseClickCommand { get; set; }

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

    public AutoCompleteViewModel()
    {
        LoadedCommand = new ActionCommand<IControl>(x => _control = x);
    }

    public void Show(double verticalScrollBarValue, double horizontalScrollBarValue, double parentWidth, double parentHeight)
    {
        if (Model == null) return;
        _parentWidth = parentWidth;
        _parentHeight = parentHeight;
        var cursor = Model.DataContext.CursorPosition;
        var cursorToken = Model.GetCursorToken();
        var y = cursor.LineIndex * Model.DataContext.TextLineHeight;
        var x = cursorToken != null
            ? cursorToken.StartColumnIndex * Model.DataContext.TextLetterWidth
            : cursor.ColumnIndex * Model.DataContext.TextLetterWidth;
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
        if (Model == null) return;
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
        if (Model == null) return;
        if (!Model.IsVisible) return;
        var x = _absolutePosition.X - horizontalScrollBarValue;
        if (x < 0) x = 0;
        else if (x + _control!.ActualWidth > _parentWidth) x = _parentWidth - _control.ActualWidth;
        ScreenPosition = new(x, ScreenPosition.Top, ScreenPosition.Right, ScreenPosition.Bottom);
    }
}
