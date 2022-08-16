﻿using System.Collections.Generic;
using System.Windows.Threading;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

static class TimersCollection
{
    public static readonly Dictionary<long, DispatcherTimer> Timers = new();
}

[EmbeddedFunctionDeclaration("init_timer", "interval callback")]
internal class InitTimer : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var interval = GetValueOrError<NumericRuntimeValue>(argumentValues, 0).IntegerValue;
        var callback = GetFunctionOrError(argumentValues, 1);

        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(interval);
        timer.Tick += (s, e) => callback.GetValue();
        var id = DateTime.UtcNow.ToBinary();
        TimersCollection.Timers.Add(id, timer);

        return new IntegerRuntimeValue(id);
    }
}
