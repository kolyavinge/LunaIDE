using System.Collections.Generic;
using System.Windows.Threading;
using Luna.Runtime;

namespace Luna.Functions.Lang;

static class TimersCollection
{
    public static readonly Dictionary<long, DispatcherTimer> Timers = new();
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
        var id = DateTime.UtcNow.ToBinary();
        TimersCollection.Timers.Add(id, timer);

        return new IntegerRuntimeValue(id);
    }
}
