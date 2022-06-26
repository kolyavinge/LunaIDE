namespace Luna.Runtime;

internal class IntegerRuntimeValue : NumericRuntimeValue
{

    public IntegerRuntimeValue(int value) : base(value)
    {
    }

    public override string ToString()
    {
        return IntegerValue.ToString();
    }
}
