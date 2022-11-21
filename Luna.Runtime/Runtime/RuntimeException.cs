using Luna.CodeElements;

namespace Luna.Runtime;

public class RuntimeException : Exception
{
    internal static RuntimeException CannotConvert(ValueElement v) => new($"Type {v.GetType()} cannot be converted to RuntimeValue.");

    internal static RuntimeException CannotEval(IRuntimeValue v) => new($"Runtime value of {v.GetType()} cannot be evaluated.");

    internal static RuntimeException IsNotFunction(string funcName) => new($"Argument {funcName} is not a function and cannot be called.");

    internal static RuntimeException ToManyArguments(string funcName) => new($"Function {funcName} has too many passed arguments and cannot be evaluated.");

    internal static RuntimeException ArgumentsNotPassed() => new("Argument values have not been passed.");

    internal static RuntimeException ArgumentMustBe(string name, string expectedType, string actualType) => new($"Argument {name} must be {expectedType} instead of {actualType}.");

    internal RuntimeException(string message) : base(message)
    {
    }

    public override bool Equals(object? obj)
    {
        return obj is RuntimeException rte &&
            Message == rte.Message;
    }

    public override int GetHashCode()
    {
        return Message.GetHashCode();
    }
}
