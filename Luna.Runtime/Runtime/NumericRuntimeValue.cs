namespace Luna.Runtime;

internal abstract class NumericRuntimeValue : RuntimeValue
{
    public int IntegerValue { get; protected set; }

    public double FloatValue { get; protected set; }

    protected NumericRuntimeValue(int value)
    {
        IntegerValue = value;
        FloatValue = value;
    }

    protected NumericRuntimeValue(double value)
    {
        IntegerValue = (int)value;
        FloatValue = value;
    }
}
