namespace Luna.Runtime;

static class FriendlyName
{
    public static string Get<TRuntimeValue>()
    {
        return Get(typeof(TRuntimeValue));
    }

    public static string Get(IRuntimeValue value)
    {
        return Get(value.GetType());
    }

    public static string Get(Type runtimeValueType)
    {
        if (runtimeValueType == typeof(VoidRuntimeValue)) return "void";
        if (runtimeValueType == typeof(BooleanRuntimeValue)) return "boolean";
        if (runtimeValueType == typeof(NumericRuntimeValue)) return "numeric";
        if (runtimeValueType == typeof(IntegerRuntimeValue)) return "numeric";
        if (runtimeValueType == typeof(FloatRuntimeValue)) return "numeric";
        if (runtimeValueType == typeof(StringRuntimeValue)) return "string";
        if (runtimeValueType == typeof(ListRuntimeValue)) return "list";
        if (runtimeValueType == typeof(FunctionRuntimeValue)) return "function";
        if (runtimeValueType == typeof(VariableRuntimeValue)) return "variable";

        throw new ArgumentException("Wrong runtime value type.", nameof(runtimeValueType));
    }
}
