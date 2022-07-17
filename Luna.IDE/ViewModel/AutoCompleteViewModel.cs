using System.Windows;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.IDE.View;

namespace Luna.IDE.ViewModel;

public class AutoCompleteViewModel : NotificationObject
{
    private Point _screenPosition;
    private IControl? _listBox;
    private double _parentWidth, _parentHeight;

    public AutoComplete Model { get; }

    public ICommand MouseClickCommand { get; set; }

    public ICommand LoadedCommand { get; set; }

    public Point AbsolutePosition { get; set; }

    public Point ScreenPosition
    {
        get => _screenPosition;
        set { _screenPosition = value; RaisePropertyChanged(() => ScreenPosition); }
    }

    public AutoCompleteViewModel(AutoComplete model)
    {
        Model = model;
        LoadedCommand = new ActionCommand<IControl>(lb => _listBox = lb);
        MouseClickCommand = new ActionCommand(Model.Complete);
    }

    public void Show(
        Point absolutePosition,
        double verticalScrollBarValue,
        double horizontalScrollBarValue,
        double parentWidth,
        double parentHeight)
    {
        AbsolutePosition = absolutePosition;
        ScreenPosition = new(AbsolutePosition.X - horizontalScrollBarValue, AbsolutePosition.Y - verticalScrollBarValue);
        _parentWidth = parentWidth;
        _parentHeight = parentHeight;
        Model.Show();
    }

    public void CorrectByVerticalScrollBarValue(double verticalScrollBarValue)
    {
        if (!Model.IsVisible) return;
        var y = AbsolutePosition.Y - verticalScrollBarValue;
        if (y < 0)
        {
            y = 0;
        }
        else if (y + _listBox!.ActualHeight > _parentHeight)
        {
            y = _parentHeight - _listBox.ActualHeight;
        }
        ScreenPosition = new(ScreenPosition.X, y);
    }

    public void CorrectByHorizontalScrollBarValue(double horizontalScrollBarValue)
    {
        if (!Model.IsVisible) return;
        var x = AbsolutePosition.X - horizontalScrollBarValue;
        if (x < 0)
        {
            x = 0;
        }
        else if (x + _listBox!.ActualWidth > _parentWidth)
        {
            x = _parentWidth - _listBox.ActualWidth;
        }
        ScreenPosition = new(x, ScreenPosition.Y);
    }
}
