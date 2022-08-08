﻿using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("stop_timer", "timer_id")]
internal class StopTimer : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var timerId = GetValueOrError<NumericRuntimeValue>(argumentValues, 0).IntegerValue;
        TimersCollection.Timers[timerId].Stop();

        return VoidRuntimeValue.Instance;
    }
}