namespace Luna.Infrastructure;

public interface ITimerManager
{
    void CreateNew(TimeSpan interval, EventHandler callback);
}
