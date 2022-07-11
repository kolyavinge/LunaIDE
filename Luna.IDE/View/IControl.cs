namespace Luna.IDE.View;

public interface IControl
{
    double ActualWidth { get; }

    double ActualHeight { get; }

    void Focus();
}
