namespace Luna.IDE.App.Controls;

public interface IControl
{
    double ActualWidth { get; }

    double ActualHeight { get; }

    double MaxWidth { get; }

    double MaxHeight { get; }

    void Focus();
}
