using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("stop_timer", "timer_id")]
internal class StopTimer : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var timerId = GetValueOrError<NumericRuntimeValue>(argumentValues, 0).IntegerValue;
        TimersCollection.Timers[timerId].Stop();

        return VoidRuntimeValue.Instance;
    }
}
