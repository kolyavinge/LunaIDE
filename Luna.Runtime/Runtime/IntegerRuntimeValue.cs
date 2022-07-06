namespace Luna.Runtime;

internal class IntegerRuntimeValue : NumericRuntimeValue
{

    public IntegerRuntimeValue(long value) : base(value)
    {
    }

    public override string ToString()
    {
        return IntegerValue.ToString();
    }
}
