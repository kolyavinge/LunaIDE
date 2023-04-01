using System.Threading;

namespace Luna.Infrastructure;

public abstract class DelayedAction
{
    private int _updateRequest;

    public DelayedAction(ITimerManager timerManager)
    {
        timerManager.CreateAndStart(TimeSpan.FromSeconds(2), (s, e) => InnerDo());
    }

    public void Request()
    {
        Interlocked.Exchange(ref _updateRequest, 1);
    }

    public void Do()
    {
        if (_updateRequest == 0) return;
        Interlocked.Exchange(ref _updateRequest, 0);
        InnerDo();
    }

    protected abstract void InnerDo();
}
