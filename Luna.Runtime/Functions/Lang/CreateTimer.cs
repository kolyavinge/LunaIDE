using System.Collections.Generic;
using System.Windows.Threading;
using Luna.Runtime;

namespace Luna.Functions.Lang;

static class AppTimersCollection
{
    public static readonly Dictionary<long, IAppTimer> Timers = new();
}

[EmbeddedFunctionDeclaration("create_timer", "interval callback")]
internal class CreateTimer : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var interval = arguments.GetValueOrError<NumericRuntimeValue>(0).IntegerValue;
        var callback = arguments.GetFunctionOrError(1);

        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(interval);
        timer.Tick += (s, e) => callback.GetValue();
        var appTimer = new AppTimer(timer);
        var id = DateTime.UtcNow.ToBinary();
        AppTimersCollection.Timers.Add(id, appTimer);

        return new IntegerRuntimeValue(id);
    }
}

interface IAppTimer
{
    void Start();
    void Stop();
}

class AppTimer : IAppTimer
{
    private readonly DispatcherTimer _timer;

    public AppTimer(DispatcherTimer timer)
    {
        _timer = timer;
    }

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();
}
