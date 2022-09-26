using System.Windows.Threading;
using Luna.Infrastructure;

namespace Luna.IDE.Infrastructure;

internal class TimerManager : ITimerManager
{
    public ITimer CreateAndStart(TimeSpan interval, EventHandler callback)
    {
        var dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Interval = interval;
        dispatcherTimer.Tick += callback;
        dispatcherTimer.Start();

        return new Timer(dispatcherTimer);
    }

    class Timer : ITimer
    {
        private readonly DispatcherTimer _dispatcherTimer;

        public Timer(DispatcherTimer dispatcherTimer)
        {
            _dispatcherTimer = dispatcherTimer;
        }

        public void Stop()
        {
            _dispatcherTimer.Stop();
        }
    }
}
