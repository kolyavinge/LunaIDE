using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("start_timer", "timer_id")]
internal class StartTimer : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var timerId = GetValueOrError<NumericRuntimeValue>(argumentValues, 0).IntegerValue;
        TimersCollection.Timers[timerId].Start();

        return VoidRuntimeValue.Instance;
    }
}
