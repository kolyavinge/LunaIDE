using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("stop_timer", "timer_id")]
internal class StopTimer : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var timerId = arguments.GetValueOrError<NumericRuntimeValue>(0).IntegerValue;
        TimersCollection.Timers[timerId].Stop();

        return VoidRuntimeValue.Instance;
    }
}
