using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("start_timer", "timer_id")]
internal class StartTimer : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var timerId = arguments.GetValueOrError<NumericRuntimeValue>(0).IntegerValue;
        TimersCollection.Timers[timerId].Start();

        return VoidRuntimeValue.Instance;
    }
}
