namespace Luna.Infrastructure;

public interface ITimerManager
{
    ITimer CreateAndStart(TimeSpan interval, EventHandler callback);
}

public interface ITimer
{
    void Stop();
}
