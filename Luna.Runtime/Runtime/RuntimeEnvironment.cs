using Luna.Output;

namespace Luna.Runtime;

public static class RuntimeEnvironment
{
    public static IRuntimeOutput? StandartOutput { get; set; }

    internal static IRuntimeExceptionHandler? ExceptionHandler { get; set; }
}
