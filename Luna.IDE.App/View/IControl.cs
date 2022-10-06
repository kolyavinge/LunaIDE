namespace Luna.IDE.App.View;

public interface IControl
{
    double ActualWidth { get; }

    double ActualHeight { get; }

    double MaxWidth { get; }

    double MaxHeight { get; }

    void Focus();
}
