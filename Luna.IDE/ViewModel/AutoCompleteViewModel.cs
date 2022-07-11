using System.Windows;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.IDE.View;

namespace Luna.IDE.ViewModel;

public class AutoCompleteViewModel : NotificationObject
{
    private bool _isVisible;
    private Point _screenPosition;
    private IControl? _measures;
    private double _parentWidth, _parentHeight;

    public AutoComplete Model { get; } = new();

    public ICommand? CompleteCommand { get; set; }

    public ICommand MouseClickCommand { get; set; }

    public ICommand LoadedCommand { get; set; }

    public bool IsVisible
    {
        get => _isVisible;
        set { _isVisible = value; RaisePropertyChanged(() => IsVisible); }
    }

    public Point AbsolutePosition { get; set; }

    public Point ScreenPosition
    {
        get => _screenPosition;
        set { _screenPosition = value; RaisePropertyChanged(() => ScreenPosition); }
    }

    public AutoCompleteViewModel()
    {
        LoadedCommand = new ActionCommand<IControl>(measures => _measures = measures);
        MouseClickCommand = new ActionCommand(Complete);
    }

    public void Show(
        Point absolutePosition,
        double verticalScrollBarValue,
        double horizontalScrollBarValue,
        double parentWidth,
        double parentHeight)
    {
        Model.UpdateItems();
        AbsolutePosition = absolutePosition;
        ScreenPosition = new(AbsolutePosition.X - horizontalScrollBarValue, AbsolutePosition.Y - verticalScrollBarValue);
        _parentWidth = parentWidth;
        _parentHeight = parentHeight;
        IsVisible = true;
    }

    public void Complete()
    {
        IsVisible = false;
        CompleteCommand?.Execute(Model.SelectedItem);
    }

    public void CorrectByVerticalScrollBarValue(double verticalScrollBarValue)
    {
        var y = AbsolutePosition.Y - verticalScrollBarValue;
        if (y < 0)
        {
            y = 0;
        }
        else if (y + _measures!.ActualHeight > _parentHeight)
        {
            y = _parentHeight - _measures.ActualHeight;
        }
        ScreenPosition = new(ScreenPosition.X, y);
    }

    public void CorrectByHorizontalScrollBarValue(double horizontalScrollBarValue)
    {
        var x = AbsolutePosition.X - horizontalScrollBarValue;
        if (x < 0)
        {
            x = 0;
        }
        else if (x + _measures!.ActualWidth > _parentWidth)
        {
            x = _parentWidth - _measures.ActualWidth;
        }
        ScreenPosition = new(x, ScreenPosition.Y);
    }
}
