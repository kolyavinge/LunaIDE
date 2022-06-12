using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using Luna.Infrastructure;

namespace Luna.IDE.Infrastructure;

internal class TimerManager : ITimerManager
{
    public void CreateNew(TimeSpan interval, EventHandler callback)
    {
        var timer = new DispatcherTimer();
        timer.Interval = interval;
        timer.Tick += callback;
        timer.Start();
    }
}
